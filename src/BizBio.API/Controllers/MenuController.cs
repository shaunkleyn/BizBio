using BizBio.Core.DTOs;
using BizBio.Core.Entities;
using BizBio.Core.Enums;
using BizBio.Core.Interfaces;
using BizBio.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Security.Claims;
using System.Text.Json;

namespace BizBio.API.Controllers;

[Route("api/v1/c")]
[ApiController]
public class MenuController : ControllerBase
{
    private readonly IProfileRepository _profileRepo;
    private readonly ICatalogRepository _catalogRepo;
    private readonly IRestaurantTableRepository _tableRepo;
    private readonly ApplicationDbContext _context;

    public MenuController(
        IProfileRepository profileRepo,
        ICatalogRepository catalogRepo,
        IRestaurantTableRepository tableRepo,
        ApplicationDbContext context)
    {
        _profileRepo = profileRepo;
        _catalogRepo = catalogRepo;
        _tableRepo = tableRepo;
        _context = context;
    }

    /// <summary>
    /// Get menu by slug with optional NFC parameter
    /// Publicly accessible endpoint for viewing restaurant menus
    /// Supports NFC tag scanning for table-specific menus
    /// </summary>
    /// <param name="slug">Restaurant profile slug</param>
    /// <param name="nfc">Optional NFC tag code for table-specific data</param>
    [HttpGet("{slug}")]
    public async Task<IActionResult> GetMenuBySlug(string slug, [FromQuery] string? nfc = null)
    {
        if (string.IsNullOrEmpty(slug))
            return BadRequest(new { success = false, error = "Slug is required" });

        var profile = await _profileRepo.GetBySlugAsync(slug);

        if (profile == null || !profile.IsActive)
            return NotFound(new { success = false, error = "Restaurant not found" });

        RestaurantTable? table = null;

        // Get table info if NFC code provided
        if (!string.IsNullOrEmpty(nfc))
        {
            table = await _tableRepo.GetByNFCCodeAsync(nfc);

            // Log the scan
            if (table != null && table.ProfileId == profile.Id && table.IsActive)
            {
                try
                {
                    var scan = new NFCScan
                    {
                        ProfileId = profile.Id,
                        TableId = table.Id,
                        NFCTagCode = nfc,
                        IPAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown",
                        UserAgent = HttpContext.Request.Headers["User-Agent"].ToString(),
                        DeviceTypeId = (int)DetermineDeviceType(HttpContext.Request.Headers["User-Agent"]),
                        ScannedAt = DateTime.UtcNow,
                        SessionId = HttpContext.Session.Id
                    };

                    _context.NFCScans.Add(scan);
                    await _context.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    // Log scan failure but don't fail the request
                    Console.WriteLine($"Failed to log NFC scan: {ex.Message}");
                }
            }
        }

        // Get catalog and items
        // Note: This endpoint still uses Profile-based lookup (legacy support)
        // TODO: Migrate to Entity-based architecture
        var catalog = await _catalogRepo.GetByProfileIdAsync(profile.Id);
        var items = catalog?.Items.Where(i => i.IsActive).ToList() ?? new List<CatalogItem>();

        // Get categories through catalog junction table
        var catalogCategories = catalog?.Categories
            .Where(cc => cc.IsActive)
            .Select(cc => cc.Category)
            .Distinct()
            .ToList() ?? new List<Category>();

        // Filter by event mode if enabled
        if (profile.EventModeEnabled)
        {
            items = items.Where(i => i.AvailableInEventMode).ToList();
        }

        return Ok(new
        {
            success = true,
            data = new
            {
                restaurant = new
                {
                    id = profile.Id,
                    name = profile.Name,
                    slug = profile.Slug,
                    description = profile.Description,
                    logo = profile.Logo,
                    contactInfo = new
                    {
                        phone = profile.ContactPhone,
                        email = profile.ContactEmail,
                        website = profile.Website
                    },
                    categories = catalogCategories.Select(cat => new
                    {
                        id = cat.Id,
                        name = cat.Name,
                        description = cat.Description,
                        items = cat.CatalogItemCategories
                            .Where(cic => cic.CatalogItem.IsActive && items.Any(i => i.Id == cic.CatalogItemId))
                            .Select(cic => cic.CatalogItemId)
                            .ToList(),
                        icon = cat.Icon,
                        isActive = cat.IsActive,
                        sortOrder = cat.SortOrder
                    }).ToList()
                },
                table = table != null ? new
                {
                    id = table.Id,
                    number = table.TableNumber,
                    name = table.TableName,
                    category = table.TableCategory.ToString(),
                    funFact = table.FunFact,
                    images = ParseJsonArray(table.Images)
                } : null,
                eventMode = new
                {
                    enabled = profile.EventModeEnabled,
                    eventName = profile.EventModeName,
                    description = profile.EventModeDescription
                },
                menu =  items.Select(item => new
                    {
                        item.CategoryId,
                        id = item.Id,
                        name = item.Name,
                        description = item.Description,
                        price = item.Price,
                        images = ParseJsonArray(item.Images),
                        isActive = item.IsActive,
                        sortOrder = item.SortOrder
                }),
                categories = catalogCategories.Select(cat => new
                {
                    id = cat.Id,
                    name = cat.Name,
                    description = cat.Description,
                    items = cat.CatalogItemCategories
                        .Where(cic => cic.CatalogItem.IsActive && items.Any(i => i.Id == cic.CatalogItemId))
                        .Select(cic => cic.CatalogItemId)
                        .ToList(),
                    icon = cat.Icon,
                    isActive = cat.IsActive,
                    sortOrder = cat.SortOrder
                }).ToList()
            }
        });
    }

    /// <summary>
    /// Determine device type from user agent string
    /// </summary>
    private DeviceType DetermineDeviceType(string userAgent)
    {
        if (string.IsNullOrEmpty(userAgent))
            return DeviceType.Unknown;

        userAgent = userAgent.ToLower();

        if (userAgent.Contains("mobile") || userAgent.Contains("android"))
            return DeviceType.Mobile;
        if (userAgent.Contains("tablet") || userAgent.Contains("ipad"))
            return DeviceType.Tablet;
        if (userAgent.Contains("windows") || userAgent.Contains("mac"))
            return DeviceType.Desktop;

        return DeviceType.Unknown;
    }

    /// <summary>
    /// Parse JSON array string into string array
    /// </summary>
    private string[] ParseJsonArray(string? json)
    {
        if (string.IsNullOrEmpty(json))
            return Array.Empty<string>();

        try
        {
            return System.Text.Json.JsonSerializer.Deserialize<string[]>(json) ?? Array.Empty<string>();
        }
        catch
        {
            return Array.Empty<string>();
        }
    }

    /// <summary>
    /// Create a new menu with profile, categories, and items
    /// Requires authentication
    /// </summary>
    [HttpPost]
    [Authorize]
    [Route("/api/v1/menus")]
    public async Task<IActionResult> CreateMenu([FromBody] MenuCreationDto dto)
    {
        try
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
                return Unauthorized(new { success = false, error = "User not authenticated" });

            // Generate slug from business name
            var slug = GenerateSlug(dto.BusinessName);

            // Ensure slug is unique
            var existingProfile = await _profileRepo.GetBySlugAsync(slug);
            if (existingProfile != null)
            {
                slug = $"{slug}-{Guid.NewGuid().ToString().Substring(0, 8)}";
            }

            // Create Profile
            var profile = new Profile
            {
                UserId = userId,
                Slug = slug,
                Name = dto.BusinessName,
                Description = dto.Description,
                ProfileType = "Menu",
                Logo = dto.BusinessLogo,
                ContactPhone = dto.PhoneNumber,
                ContactEmail = dto.Email,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Profiles.Add(profile);
            await _context.SaveChangesAsync();

            // TODO: This endpoint creates Profile-based menus (legacy)
            // Should be migrated to create Entity-based catalogs instead
            // For now, commenting out catalog creation as it requires Entity

            return StatusCode(501, new
            {
                success = false,
                error = "Menu creation is deprecated. Please use Entity-based catalog creation.",
                message = "This endpoint requires migration to the new Entity-based architecture."
            });

            // Legacy code commented out - requires Entity-based architecture
            // var catalog = new Catalog { EntityId = ???, ... };
            // _context.Catalogs.Add(catalog);
            // await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                success = false,
                error = "Failed to create menu",
                details = ex.Message
            });
        }
    }

    /// <summary>
    /// Get user's menus
    /// </summary>
    [HttpGet]
    [Authorize]
    [Route("/api/v1/menus/my")]
    public async Task<IActionResult> GetMyMenus()
    {
        try
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
                return Unauthorized(new { success = false, error = "User not authenticated" });

            // Get profiles with their catalogs
            var profiles = await _context.Profiles
                .Where(p => p.UserId == userId && p.ProfileType == "Menu")
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();

            var result = new List<object>();

            foreach (var profile in profiles)
            {
                // NOTE: Catalog.ProfileId no longer exists - catalogs now belong to Entities
                // This endpoint cannot function with the new architecture
                // Returning basic profile info without catalog details
                int categoryCount = 0;
                int itemCount = 0;
                DateTime lastUpdated = profile.CreatedAt;

                result.Add(new
                {
                    id = profile.Id,
                    name = profile.Name,
                    slug = profile.Slug,
                    description = profile.Description,
                    logo = profile.Logo,
                    createdAt = profile.CreatedAt,
                    isActive = profile.IsActive,
                    categoryCount,
                    itemCount,
                    lastUpdated
                });
            }

            return Ok(new { success = true, data = result });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                success = false,
                error = "Failed to retrieve menus",
                details = ex.Message
            });
        }
    }

    /// <summary>
    /// Get menu by ID for editing
    /// Returns full menu details including catalog structure
    /// </summary>
    [HttpGet]
    [Authorize]
    [Route("/api/v1/menus/{id}")]
    public async Task<IActionResult> GetMenuById(int id)
    {
        try
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
                return Unauthorized(new { success = false, error = "User not authenticated" });

            // Get catalog with full details
            var catalog = await _catalogRepo.GetDetailByIdAsync(id);

            if (catalog == null)
                return NotFound(new { success = false, error = "Menu not found" });

            // Verify ownership via Entity
            if (catalog.Entity?.UserId != userId)
                return Forbid();

            // Map to response DTO
            var response = new
            {
                id = catalog.Id,
                entityId = catalog.EntityId,
                name = catalog.Name,
                description = catalog.Description,
                slug = catalog.Entity?.Slug,
                // catalog.Categories is now CatalogCategory junction records
                categories = catalog.Categories
                    .Where(cc => cc.IsActive)
                    .OrderBy(cc => cc.SortOrder)
                    .Select(cc => new
                    {
                        id = cc.Category.Id,
                        name = cc.Category.Name,
                        description = cc.Category.Description,
                        icon = cc.Category.Icon,
                        sortOrder = cc.SortOrder,
                        itemCount = cc.Category.CatalogItemCategories.Count(cic => cic.CatalogItem.IsActive)
                    })
                    .ToList(),
                items = catalog.Items
                    .Where(i => i.IsActive)
                    .OrderBy(i => i.SortOrder)
                    .Select(i => new
                    {
                        id = i.Id,
                        name = i.Name,
                        description = i.Description,
                        price = i.Price,
                        images = string.IsNullOrEmpty(i.Images)
                            ? new List<string>()
                            : System.Text.Json.JsonSerializer.Deserialize<List<string>>(i.Images) ?? new List<string>(),
                        categoryIds = i.CatalogItemCategories.Select(cic => cic.CategoryId).ToList(),
                        sortOrder = i.SortOrder,
                        variantCount = i.Variants.Count(v => v.IsActive),
                        hasOptions = i.OptionGroupLinks.Any(l => l.IsActive),
                        hasExtras = i.ExtraGroupLinks.Any(l => l.IsActive)
                    })
                    .ToList(),
                bundles = catalog.Bundles
                    .Where(b => b.IsActive)
                    .OrderBy(b => b.SortOrder)
                    .Select(b => new
                    {
                        id = b.Id,
                        name = b.Name,
                        description = b.Description,
                        basePrice = b.BasePrice,
                        images = string.IsNullOrEmpty(b.Images)
                            ? new List<string>()
                            : System.Text.Json.JsonSerializer.Deserialize<List<string>>(b.Images) ?? new List<string>(),
                        categoryIds = b.CatalogBundleCategories
                            .Where(cbc => cbc.IsActive)
                            .Select(cbc => cbc.CategoryId)
                            .ToList(),
                        sortOrder = b.SortOrder
                    })
                    .ToList()
            };

            return Ok(new { success = true, data = response });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                success = false,
                error = "Failed to retrieve menu",
                details = ex.Message
            });
        }
    }

    /// <summary>
    /// Get catalog item details with variants
    /// Publicly accessible endpoint for viewing item details
    /// </summary>
    /// <param name="slug">Restaurant profile slug</param>
    /// <param name="itemId">Catalog item ID</param>
    [HttpGet("{slug}/items/{itemId}")]
    public async Task<IActionResult> GetItemDetails(string slug, int itemId)
    {
        if (string.IsNullOrEmpty(slug))
            return BadRequest(new { success = false, error = "Slug is required" });

        var profile = await _profileRepo.GetBySlugAsync(slug);

        if (profile == null || !profile.IsActive)
            return NotFound(new { success = false, error = "Restaurant not found" });

        var catalog = await _catalogRepo.GetByProfileIdAsync(profile.Id);
        if (catalog == null)
            return NotFound(new { success = false, error = "Menu not found" });

        var item = await _context.CatalogItems
            .Include(i => i.Variants)
            .FirstOrDefaultAsync(i => i.Id == itemId && i.CatalogId == catalog.Id && i.IsActive);

        if (item == null)
            return NotFound(new { success = false, error = "Item not found" });

        return Ok(new
        {
            success = true,
            data = new
            {
                item = new
                {
                    id = item.Id,
                    name = item.Name,
                    description = item.Description,
                    price = item.Price,
                    images = ParseJsonArray(item.Images),
                    itemType = (int)item.ItemType,
                    variants = item.Variants
                        .Where(v => v.IsActive)
                        .Select(v => new
                        {
                            id = v.Id,
                            title = v.Title,
                            name = v.Title,
                            price = v.Price,
                            sizeValue = v.SizeValue,
                            sizeUnit = v.SizeUnit,
                            isDefault = v.IsDefault
                        }).ToList()
                }
            }
        });
    }

    /// <summary>
    /// Get bundle details with steps and options
    /// Publicly accessible endpoint for viewing bundle configuration
    /// </summary>
    /// <param name="slug">Restaurant profile slug</param>
    /// <param name="bundleId">Bundle ID</param>
    [HttpGet("{slug}/bundles/{bundleId}")]
    public async Task<IActionResult> GetBundleDetails(string slug, int bundleId)
    {
        if (string.IsNullOrEmpty(slug))
            return BadRequest(new { success = false, error = "Slug is required" });

        var profile = await _profileRepo.GetBySlugAsync(slug);

        if (profile == null || !profile.IsActive)
            return NotFound(new { success = false, error = "Restaurant not found" });

        var catalog = await _catalogRepo.GetByProfileIdAsync(profile.Id);
        if (catalog == null)
            return NotFound(new { success = false, error = "Menu not found" });

        var bundle = await _context.CatalogBundles
            .Include(b => b.Steps.OrderBy(s => s.StepNumber))
                .ThenInclude(s => s.AllowedProducts)
                    .ThenInclude(p => p.Product)
            .Include(b => b.Steps)
                .ThenInclude(s => s.OptionGroups)
                    .ThenInclude(g => g.Options.Where(o => o.IsActive))
            .FirstOrDefaultAsync(b => b.Id == bundleId && b.CatalogId == catalog.Id && b.IsActive);

        if (bundle == null)
            return NotFound(new { success = false, error = "Bundle not found" });

        return Ok(new
        {
            success = true,
            data = new
            {
                bundle = new
                {
                    id = bundle.Id,
                    name = bundle.Name,
                    description = bundle.Description,
                    basePrice = bundle.BasePrice,
                    images = ParseJsonArray(bundle.Images),
                    steps = bundle.Steps
                        .Where(s => s.IsActive)
                        .OrderBy(s => s.StepNumber)
                        .Select(s => new
                        {
                            id = s.Id,
                            stepNumber = s.StepNumber,
                            name = s.Name,
                            minSelect = s.MinSelect,
                            maxSelect = s.MaxSelect,
                            products = s.AllowedProducts
                                .Where(p => p.IsActive && p.Product.IsActive)
                                .Select(p => new
                                {
                                    id = p.Product.Id,
                                    name = p.Product.Name,
                                    description = p.Product.Description,
                                    priceModifier = 0m,
                                    images = ParseJsonArray(p.Product.Images)
                                }).ToList(),
                            optionGroups = s.OptionGroups
                                .Where(g => g.IsActive)
                                .Select(g => new
                                {
                                    id = g.Id,
                                    name = g.Name,
                                    isRequired = g.IsRequired,
                                    minSelect = g.MinSelect,
                                    maxSelect = g.MaxSelect,
                                    options = g.Options
                                        .Where(o => o.IsActive)
                                        .Select(o => new
                                        {
                                            id = o.Id,
                                            name = o.Name,
                                            priceModifier = o.PriceModifier,
                                            isDefault = o.IsDefault
                                        }).ToList()
                                }).ToList()
                        }).ToList()
                }
            }
        });
    }

    /// <summary>
    /// Generate URL-friendly slug from text
    /// </summary>
    private string GenerateSlug(string text)
    {
        if (string.IsNullOrEmpty(text))
            return Guid.NewGuid().ToString();

        // Convert to lowercase and replace spaces with hyphens
        text = text.ToLower().Trim();
        text = System.Text.RegularExpressions.Regex.Replace(text, @"\s+", "-");

        // Remove invalid characters
        text = System.Text.RegularExpressions.Regex.Replace(text, @"[^a-z0-9\-]", "");

        // Remove duplicate hyphens
        text = System.Text.RegularExpressions.Regex.Replace(text, @"-+", "-");

        // Trim hyphens from ends
        text = text.Trim('-');

        return text;
    }

    /// <summary>
    /// Get user's menus
    /// </summary>
    [HttpGet]
    [Authorize]
    [Route("/api/v1/menus/test")]
    public async Task<IActionResult> GetTestMenu()
    {
        try
        {
            MenuDto menu = new MenuDto
            {
                Categories = new List<MenuCategoryDto>
               {
                   new MenuCategoryDto
                   {
                          Name = "Pizzas",
                          Description = "Delicious pizzas",
                          Items = new List<MenuDishDto>
                          {
                              new MenuDishDto
                              {
                                  Name = "Hawaiian Pizza",
                                  Description = "Ham, pineapple, mozzarella, and tomato base.",
                                  Variants = new List<MenuItemVariantDto>
                                  {
                                      new MenuItemVariantDto
                                      {
                                          Title = "Small",
                                          Unit = new MenuItemVariantUnitDto
                                          {
                                              UnitType = MenuItemUnitType.Size,
                                              Label = "Small"
                                          },
                                          Prices = new List<MenuItemVariantPriceDto>
                                          {
                                              new MenuItemVariantPriceDto
                                              {
                                                  Amount = 69.99,
                                                  PriceType = MenuItemPriceType.Standard,

                                              }
                                          }
                                      },
                                      new MenuItemVariantDto
                                      {
                                          Title = "Medium",
                                          Unit = new MenuItemVariantUnitDto
                                          {
                                              UnitType = MenuItemUnitType.Size,
                                              Label = "Medium"
                                          },
                                          Prices = new List<MenuItemVariantPriceDto>
                                          {
                                              new MenuItemVariantPriceDto
                                              {
                                                  Amount = 89.99,
                                                  PriceType = MenuItemPriceType.Standard
                                              }
                                          }
                                      }
                                  },
                                  MenuDishTags = new Dictionary<MenuItemTagCategoryDto, List<MenuItemTagDto>>
                                  {
                                      {
                                          new MenuItemTagCategoryDto
                                          {
                                              Name = "Dietary",
                                              Description = "Dietary preferences"
                                          },
                                            new List<MenuItemTagDto>
                                            {
                                                new MenuItemTagDto
                                                {
                                                    Name = "Gluten-Free",
                                                    Description = "Suitable for gluten-free diets"
                                                },
                                                new MenuItemTagDto
                                                {
                                                    Name = "Vegan",
                                                    Description = "Suitable for vegan diets"
                                                }

                                            }
                                        }

                                    }

                                }
                            }
                        }
                    }
                };
    //        var Categories = new List<CatalogCategory>
    //{
                   
    //    new CatalogCategory
    //    {
    //        Id = 1,
    //        Name = "Pizzas",
    //        Description = "pizzas",
    //        Items = new List<CatalogItem>
    //        {
    //            new CatalogItem
    //            {
    //                Id = 1,
    //                Name = "Hawaiian Pizza",
    //                Description = "Ham, pineapple, mozzarella, and tomato base.",
                    
    //                Inventory = new CatalogItemInventory
    //                {
    //                    Id = 1,
    //                    QtyAvailable = 15,  // corrected property name
    //                    QtyReserved = 5     // corrected property name
    //                },

    //                Variants = new List<CatalogItemVariant>
    //                {
    //                    new CatalogItemVariant
    //                    {
    //                        Id = 1,
    //                        Title = "Small",
                            
    //                        Unit = new MenuItemVariantUnit
    //                        {
    //                            Id = 1,
    //                            UnitType = UnitType.Size,
    //                            Label = "Small"
    //                        },

    //                        Prices = new List<CatalogItemVariantPrice>
    //                        {
    //                            new CatalogItemVariantPrice {
    //                                Id = 1,
    //                                Amount = 69.99,
    //                                PriceType = PriceType.Standard
    //                            }
    //                        }
    //                    },
    //                    new ProductVariant
    //                    {
    //                        Id = 2,
    //                        Title = "Medium",

    //                        Unit = new VariantUnit
    //                        {
    //                            Id = 2,
    //                            UnitType = UnitType.Size,
    //                            Label = "Medium"
    //                        },

    //                        Prices = new List<CatalogItemVariantPrice>
    //                        {
    //                            new CatalogItemVariantPrice {
    //                                Id = 2,
    //                                Amount = 89.99,
    //                                PriceType = PriceType.Standard
    //                            }
    //                        }
    //                    },
    //                    new ProductVariant
    //                    {
    //                        Id = 3,
    //                        Title = "Large",

    //                        Unit = new VariantUnit
    //                        {
    //                            Id = 3,
    //                            UnitType = UnitType.Size,
    //                            Label = "Large"
    //                        },

    //                        Prices = new List<CatalogItemVariantPrice>
    //                        {
    //                            new CatalogItemVariantPrice {
    //                                Id = 3,
    //                                Amount = 109.99,
    //                                PriceType = PriceType.Standard
    //                            }
    //                        }
    //                    }
    //                }
    //            }
    //        }
    //    },

    //    new Category
    //    {
    //        Id = 2,
    //        Name = "Drinks",
    //        Slug = "drinks",
    //        Products = new List<Product>
    //        {
    //            new Product
    //            {
    //                Id = 5,
    //                Title = "Coca-Cola",
    //                Description = "Ice cold Coke.",

    //                Inventory = new Inventory
    //                {
    //                    Id = 10,
    //                    StockOnHand = 50,
    //                    ReorderLevel = 20
    //                },

    //                Variants = new List<ProductVariant>
    //                {
    //                    new ProductVariant
    //                    {
    //                        Id = 10,
    //                        Title = "300ml",

    //                        Unit = new VariantUnit
    //                        {
    //                            Id = 10,
    //                            UnitType = UnitType.Volume,
    //                            Value = 300,
    //                            Label = "300ml"
    //                        },

    //                        Prices = new List<CatalogItemVariantPrice>
    //                        {
    //                            new CatalogItemVariantPrice
    //                            {
    //                                Id = 10,
    //                                Amount = 18.99,
    //                                PriceType = PriceType.Standard
    //                            }
    //                        }
    //                    },
    //                    new ProductVariant
    //                    {
    //                        Id = 11,
    //                        Title = "500ml",

    //                        Unit = new VariantUnit
    //                        {
    //                            Id = 11,
    //                            UnitType = UnitType.Volume,
    //                            Value = 500,
    //                            Label = "500ml"
    //                        },

    //                        Prices = new List<CatalogItemVariantPrice>
    //                        {
    //                            new CatalogItemVariantPrice
    //                            {
    //                                Id = 11,
    //                                Amount = 22.99,
    //                                PriceType = PriceType.Standard
    //                            }
    //                        }
    //                    }
    //                }
    //            }
    //        }
    //    }
    //}
    //        };


    //        var menuData = new Catalog
    //        {
    //            Categories = new List<CatalogCategory>
    //            {
    //                new CatalogCategory
    //                {
    //                    Id = 1,
    //                    Name = "Pizzas",
    //                    Description = "pizzas",
    //                    Items = new List<CatalogItem>
    //                    {
    //                        new CatalogItem
    //                        {
    //                            Id = 1,
    //                            Name = "Hawaiian Pizza",
    //                            Description = "Ham, pineapple, mozzarella, and tomato base.",
    //                            Variants = new List<CatalogItemVariant>
    //                            {
    //                                new CatalogItemVariant { Id = 1, Title = "Small", Price = 69.99m, },
    //                                new CatalogItemVariant { Id = 2, Title = "Medium", Price = 89.99m },
    //                                new CatalogItemVariant { Id = 3, Title = "Large", Price = 109.99m }
    //                            }
    //                        },
    //                        new CatalogItem
    //                        {
    //                            Id = 2,
    //                            Name = "Pepperoni Pizza",
    //                            Description = "Classic pepperoni and cheese.",
    //                            Variants = new List<CatalogItemVariant>
    //                            {
    //                                new CatalogItemVariant { Id = 4, Title = "Medium", Price = 99.99m, },
    //                                new CatalogItemVariant { Id = 5, Title = "Large", Price = 119.99m }
    //                            }
    //                        }
    //                    }
    //                },

    //    new CatalogCategory
    //    {
    //        Id = 2,
    //        Name = "Burgers",
    //        Description = "burgers",
    //        Items = new List<CatalogItem>
    //        {
    //            new CatalogItem
    //            {
    //                Id = 3,
    //                Name = "Classic Beef Burger",
    //                Description = "Juicy beef patty with cheese.",
    //                Variants = new List<CatalogItemVariant>
    //                {
    //                    new CatalogItemVariant { Id = 6, Title = "Mild", Price = 79.99m },
    //                    new CatalogItemVariant { Id = 7, Title = "Hot", Price = 79.99m }
    //                }
    //            },
    //            new CatalogItem
    //            {
    //                Id = 4,
    //                Name = "Spicy Chicken Burger",
    //                Description = "Crispy hot chicken breast with mayo.",
    //                Variants = new List<CatalogItemVariant>
    //                {
    //                    new CatalogItemVariant { Id = 8, Title = "Mild", Price = 84.99m },
    //                    new CatalogItemVariant { Id = 9, Title = "Hot", Price = 84.99m }
    //                }
    //            }
    //        }
    //    },

    //    new CatalogCategory
    //    {
    //        Id = 3,
    //        Name = "Drinks",
    //        Description = "drinks",
    //        Items = new List<CatalogItem>
    //        {
    //            new CatalogItem
    //            {
    //                Id = 5,
    //                Name = "Coca-Cola",
    //                Description = "Ice cold Coke.",
    //                Variants = new List<CatalogItemVariant>
    //                {
    //                    new CatalogItemVariant { Id = 10, Title = "300ml", Price = 18.99m },
    //                    new CatalogItemVariant { Id = 11, Title = "500ml", Price = 22.99m }
    //                }
    //            },
    //            new CatalogItem
    //            {
    //                Id = 6,
    //                Name = "Orange Juice",
    //                Description = "Freshly squeezed.",
    //                Variants = new List<CatalogItemVariant>
    //                {
    //                    new CatalogItemVariant { Id = 12, Title = "300ml", Price = 24.99m },
    //                    new CatalogItemVariant { Id = 13, Title = "500ml", Price = 29.99m }
    //                }
    //            }
    //        }
    //    }
    //}
    //        };

            return Ok(new { success = true, data = menu });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                success = false,
                error = "Failed to retrieve menus",
                details = ex.Message
            });
        }
    }
}
