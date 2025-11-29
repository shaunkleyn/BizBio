using Microsoft.AspNetCore.Mvc;
using BizBio.Core.Interfaces;
using BizBio.Core.Entities;
using BizBio.Core.Enums;
using BizBio.Infrastructure.Data;

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
}
