using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using BizBio.Core.Interfaces;
using BizBio.Core.Entities;
using BizBio.Core.Enums;
using BizBio.Core.DTOs;
using BizBio.Infrastructure.Data;
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
                        id = item.Id,
                        name = item.Name,
                        description = item.Description,
                        price = item.Price,
                        images = ParseJsonArray(item.Images)
                    }).ToList()
                }
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
                var category = new Category
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
}
