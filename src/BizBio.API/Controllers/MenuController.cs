using BizBio.Core.DTOs;
using BizBio.Core.Entities;
using BizBio.Core.Enums;
using BizBio.Core.Interfaces;
using BizBio.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
        var catalog = await _catalogRepo.GetByProfileIdAsync(profile.Id);
        var items = catalog?.Items.Where(i => i.IsActive).ToList() ?? new List<CatalogItem>();
 

        var categories = new Dictionary<CatalogCategory, CatalogItem>();
        foreach (var item in items)
        {   if (item.CategoryId == null) continue;
            var category = await _context.Categories.FindAsync(item.CategoryId);
            if (category != null && !categories.ContainsKey(category))
            {
                categories.Add(category, item);
            }
        }

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
                    }
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
                menu = new
                {
                    
                    items = items.Select(item => new
                    {
                        item.CategoryId,
                        id = item.Id,
                        name = item.Name,
                        description = item.Description,
                        price = item.Price,
                        images = ParseJsonArray(item.Images)
                    }).ToList()
                },
                categories = categories.Keys.Select(cat => new
                {
                    id = cat.Id,
                    name = cat.Name,
                    description = cat.Description,
                    items = items.Where(i => i.CategoryId == cat.Id).Select(i => i.Id).ToList(),
                    icon = cat.Icon
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

            // Create Catalog (Menu)
            var catalog = new Catalog
            {
                ProfileId = profile.Id,
                Name = dto.Name,
                Description = dto.Description,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Catalogs.Add(catalog);
            await _context.SaveChangesAsync();

            // Create Categories and map temporary IDs to real IDs
            var categoryIdMap = new Dictionary<string, int>();

            foreach (var categoryDto in dto.Categories.OrderBy(c => c.Order))
            {
                var category = new CatalogCategory
                {
                    CatalogId = catalog.Id,
                    Name = categoryDto.Name,
                    Description = categoryDto.Description,
                    Icon = categoryDto.Icon,
                    SortOrder = categoryDto.Order,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _context.Categories.Add(category);
                await _context.SaveChangesAsync();

                // Map the frontend temporary ID to the actual database ID
                var tempId = dto.Categories.IndexOf(categoryDto).ToString();
                categoryIdMap[categoryDto.Name] = category.Id;
            }

            // Create Menu Items
            foreach (var itemDto in dto.Items)
            {
                // Find the category ID by matching the category name
                var category = dto.Categories.FirstOrDefault(c =>
                    dto.Categories.IndexOf(c).ToString() == itemDto.CategoryId ||
                    c.Name == itemDto.CategoryId);

                if (category == null) continue;

                var categoryId = categoryIdMap[category.Name];

                var item = new CatalogItem
                {
                    CatalogId = catalog.Id,
                    CategoryId = categoryId,
                    Name = itemDto.Name,
                    Description = itemDto.Description,
                    Price = itemDto.Price,
                    Images = !string.IsNullOrEmpty(itemDto.ImageUrl)
                        ? JsonSerializer.Serialize(new[] { itemDto.ImageUrl })
                        : null,
                    IsActive = itemDto.Available,
                    SortOrder = 0,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _context.CatalogItems.Add(item);
            }

            await _context.SaveChangesAsync();

            return Ok(new
            {
                success = true,
                data = new
                {
                    profileId = profile.Id,
                    slug = profile.Slug,
                    catalogId = catalog.Id,
                    categoriesCount = dto.Categories.Count,
                    itemsCount = dto.Items.Count,
                    url = $"/menu/{profile.Slug}"
                }
            });
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

            var profiles = _context.Profiles
                .Where(p => p.UserId == userId && p.ProfileType == "Menu")
                .OrderByDescending(p => p.CreatedAt)
                .Select(p => new
                {
                    id = p.Id,
                    name = p.Name,
                    slug = p.Slug,
                    description = p.Description,
                    logo = p.Logo,
                    createdAt = p.CreatedAt,
                    isActive = p.IsActive
                })
                .ToList();

            return Ok(new { success = true, data = profiles });
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
