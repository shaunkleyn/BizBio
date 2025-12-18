using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BizBio.Core.Entities;
using BizBio.Infrastructure.Data;
using System.Security.Claims;
using System.Text.Json;

namespace BizBio.API.Controllers;

/// <summary>
/// Controller for managing library items (items owned by user, not tied to a specific catalog/menu)
/// </summary>
[Route("api/v1/library/items")]
[ApiController]
[Authorize]
public class LibraryItemsController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public LibraryItemsController(ApplicationDbContext context)
    {
        _context = context;
    }

    private int GetUserId() => int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

    /// <summary>
    /// Get all library items for the current user
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetLibraryItems([FromQuery] int? categoryId = null)
    {
        var userId = GetUserId();

        var query = _context.CatalogItems
            .Include(i => i.Variants.Where(v => v.IsActive))
            .Include(i => i.ExtraGroupLinks.Where(l => l.IsActive))
                .ThenInclude(l => l.ExtraGroup)
                    .ThenInclude(g => g.GroupItems.Where(gi => gi.IsActive))
                        .ThenInclude(gi => gi.Extra)
            .Where(i => i.UserId == userId && i.CatalogId == null && i.IsActive);

        if (categoryId.HasValue)
        {
            query = query.Where(i => i.CategoryId == categoryId.Value);
        }

        var items = await query
            .OrderBy(i => i.SortOrder)
            .ThenBy(i => i.Name)
            .Select(i => new
            {
                i.Id,
                i.Name,
                i.Description,
                i.Price,
                i.CategoryId,
                i.ItemType,
                i.BundleId,
                images = ParseJsonArray(i.Images),
                tags = ParseJsonArray(i.Tags),
                i.SortOrder,
                variantCount = i.Variants.Count,
                variants = i.Variants.Select(v => new
                {
                    v.Id,
                    v.Title,
                    v.Price,
                    v.SizeValue,
                    v.SizeUnit,
                    v.IsDefault
                }).ToList(),
                extraGroups = i.ExtraGroupLinks
                    .OrderBy(l => l.DisplayOrder)
                    .Select(l => new
                    {
                        l.ExtraGroupId,
                        l.ExtraGroup.Name,
                        l.ExtraGroup.Description,
                        l.ExtraGroup.MinRequired,
                        l.ExtraGroup.MaxAllowed,
                        l.ExtraGroup.AllowMultipleQuantities,
                        Extras = l.ExtraGroup.GroupItems
                            .OrderBy(gi => gi.DisplayOrder)
                            .Select(gi => new
                            {
                                gi.Extra.Id,
                                gi.Extra.Name,
                                gi.Extra.BasePrice,
                                gi.PriceOverride
                            }).ToList()
                    }).ToList()
            })
            .ToListAsync();

        return Ok(new { success = true, data = new { items } });
    }

    /// <summary>
    /// Get a specific library item by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetLibraryItem(int id)
    {
        var userId = GetUserId();

        var item = await _context.CatalogItems
            .Include(i => i.Variants.Where(v => v.IsActive))
            .Include(i => i.ExtraGroupLinks.Where(l => l.IsActive))
                .ThenInclude(l => l.ExtraGroup)
                    .ThenInclude(g => g.GroupItems.Where(gi => gi.IsActive))
                        .ThenInclude(gi => gi.Extra)
            .FirstOrDefaultAsync(i => i.Id == id && i.UserId == userId && i.CatalogId == null);

        if (item == null)
            return NotFound(new { success = false, error = "Item not found" });

        return Ok(new
        {
            success = true,
            data = new
            {
                item = new
                {
                    item.Id,
                    item.Name,
                    item.Description,
                    item.Price,
                    item.CategoryId,
                    item.ItemType,
                    item.BundleId,
                    images = ParseJsonArray(item.Images),
                    tags = ParseJsonArray(item.Tags),
                    item.SortOrder,
                    item.AvailableInEventMode,
                    item.EventModeOnly,
                    variants = item.Variants.Select(v => new
                    {
                        v.Id,
                        v.Title,
                        v.Price,
                        v.Cost,
                        v.SizeValue,
                        v.SizeUnit,
                        v.UnitOfMeasure,
                        v.Sku,
                        v.Barcode,
                        v.IsDefault,
                        v.WeightG
                    }).ToList(),
                    extraGroups = item.ExtraGroupLinks
                        .OrderBy(l => l.DisplayOrder)
                        .Select(l => new
                        {
                            l.ExtraGroupId,
                            l.ExtraGroup.Name,
                            l.ExtraGroup.Description,
                            l.ExtraGroup.MinRequired,
                            l.ExtraGroup.MaxAllowed,
                            l.ExtraGroup.AllowMultipleQuantities,
                            Extras = l.ExtraGroup.GroupItems
                                .OrderBy(gi => gi.DisplayOrder)
                                .Select(gi => new
                                {
                                    gi.Extra.Id,
                                    gi.Extra.Name,
                                    gi.Extra.BasePrice,
                                    gi.PriceOverride
                                }).ToList()
                        }).ToList()
                }
            }
        });
    }

    /// <summary>
    /// Create a new library item
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> CreateLibraryItem([FromBody] CreateLibraryItemDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var userId = GetUserId();

        var item = new CatalogItem
        {
            UserId = userId,
            CatalogId = null, // Library item
            CategoryId = dto.CategoryId,
            ItemType = dto.ItemType,
            Name = dto.Name,
            Description = dto.Description,
            Price = dto.Price,
            Images = dto.Images != null && dto.Images.Any()
                ? JsonSerializer.Serialize(dto.Images)
                : null,
            Tags = dto.Tags != null && dto.Tags.Any()
                ? JsonSerializer.Serialize(dto.Tags)
                : null,
            SortOrder = dto.SortOrder,
            AvailableInEventMode = dto.AvailableInEventMode,
            EventModeOnly = dto.EventModeOnly,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            CreatedBy = userId.ToString(),
            UpdatedBy = userId.ToString()
        };

        _context.CatalogItems.Add(item);
        await _context.SaveChangesAsync();

        // Add variants if provided
        if (dto.Variants != null && dto.Variants.Any())
        {
            foreach (var variantDto in dto.Variants)
            {
                var variant = new CatalogItemVariant
                {
                    CatalogItemId = item.Id,
                    Title = variantDto.Title,
                    Price = variantDto.Price,
                    Cost = variantDto.Cost,
                    SizeValue = variantDto.SizeValue,
                    SizeUnit = variantDto.SizeUnit,
                    UnitOfMeasure = variantDto.UnitOfMeasure,
                    Sku = variantDto.Sku,
                    Barcode = variantDto.Barcode,
                    IsDefault = variantDto.IsDefault,
                    WeightG = variantDto.WeightG,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    CreatedBy = userId.ToString(),
                    UpdatedBy = userId.ToString()
                };
                _context.CatalogItemVariants.Add(variant);
            }
            await _context.SaveChangesAsync();
        }

        // Link extra groups if provided
        if (dto.ExtraGroupIds != null && dto.ExtraGroupIds.Any())
        {
            var displayOrder = 0;
            foreach (var extraGroupId in dto.ExtraGroupIds)
            {
                // Verify the extra group belongs to the user
                var extraGroup = await _context.CatalogItemExtraGroups
                    .FirstOrDefaultAsync(g => g.Id == extraGroupId && g.UserId == userId && g.IsActive);

                if (extraGroup != null)
                {
                    var link = new CatalogItemExtraGroupLink
                    {
                        CatalogItemId = item.Id,
                        ExtraGroupId = extraGroupId,
                        DisplayOrder = displayOrder++,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow,
                        CreatedBy = userId.ToString(),
                        UpdatedBy = userId.ToString()
                    };
                    _context.CatalogItemExtraGroupLinks.Add(link);
                }
            }
            await _context.SaveChangesAsync();
        }

        return Ok(new { success = true, data = new { item = new { item.Id, item.Name } } });
    }

    /// <summary>
    /// Update a library item
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateLibraryItem(int id, [FromBody] UpdateLibraryItemDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var userId = GetUserId();

        var item = await _context.CatalogItems
            .FirstOrDefaultAsync(i => i.Id == id && i.UserId == userId && i.CatalogId == null);

        if (item == null)
            return NotFound(new { success = false, error = "Item not found" });

        // Update fields
        if (!string.IsNullOrEmpty(dto.Name)) item.Name = dto.Name;
        if (dto.Description != null) item.Description = dto.Description;
        if (dto.Price.HasValue) item.Price = dto.Price.Value;
        if (dto.CategoryId != null) item.CategoryId = dto.CategoryId;
        if (dto.Images != null)
            item.Images = dto.Images.Any() ? JsonSerializer.Serialize(dto.Images) : null;
        if (dto.Tags != null)
            item.Tags = dto.Tags.Any() ? JsonSerializer.Serialize(dto.Tags) : null;
        if (dto.SortOrder.HasValue) item.SortOrder = dto.SortOrder.Value;
        if (dto.AvailableInEventMode.HasValue) item.AvailableInEventMode = dto.AvailableInEventMode.Value;
        if (dto.EventModeOnly.HasValue) item.EventModeOnly = dto.EventModeOnly.Value;

        item.UpdatedAt = DateTime.UtcNow;
        item.UpdatedBy = userId.ToString();

        // Update extra group links if provided
        if (dto.ExtraGroupIds != null)
        {
            // Remove existing links
            var existingLinks = await _context.CatalogItemExtraGroupLinks
                .Where(l => l.CatalogItemId == item.Id)
                .ToListAsync();
            _context.CatalogItemExtraGroupLinks.RemoveRange(existingLinks);

            // Add new links
            var displayOrder = 0;
            foreach (var extraGroupId in dto.ExtraGroupIds)
            {
                // Verify the extra group belongs to the user
                var extraGroup = await _context.CatalogItemExtraGroups
                    .FirstOrDefaultAsync(g => g.Id == extraGroupId && g.UserId == userId && g.IsActive);

                if (extraGroup != null)
                {
                    var link = new CatalogItemExtraGroupLink
                    {
                        CatalogItemId = item.Id,
                        ExtraGroupId = extraGroupId,
                        DisplayOrder = displayOrder++,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow,
                        CreatedBy = userId.ToString(),
                        UpdatedBy = userId.ToString()
                    };
                    _context.CatalogItemExtraGroupLinks.Add(link);
                }
            }
        }

        await _context.SaveChangesAsync();

        return Ok(new { success = true, data = new { item = new { item.Id, item.Name } } });
    }

    /// <summary>
    /// Delete a library item
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteLibraryItem(int id)
    {
        var userId = GetUserId();

        var item = await _context.CatalogItems
            .FirstOrDefaultAsync(i => i.Id == id && i.UserId == userId && i.CatalogId == null);

        if (item == null)
            return NotFound(new { success = false, error = "Item not found" });

        item.IsActive = false;
        item.UpdatedAt = DateTime.UtcNow;
        item.UpdatedBy = userId.ToString();

        await _context.SaveChangesAsync();

        return Ok(new { success = true, message = "Item deleted successfully" });
    }

    /// <summary>
    /// Add library item to a catalog/menu
    /// </summary>
    [HttpPost("{id}/add-to-catalog")]
    public async Task<IActionResult> AddToCatalog(int id, [FromBody] AddToCatalogDto dto)
    {
        var userId = GetUserId();

        var libraryItem = await _context.CatalogItems
            .Include(i => i.Variants)
            .Include(i => i.ExtraGroupLinks.Where(l => l.IsActive))
            .FirstOrDefaultAsync(i => i.Id == id && i.UserId == userId && i.CatalogId == null);

        if (libraryItem == null)
            return NotFound(new { success = false, error = "Library item not found" });

        // Verify user owns the catalog
        var catalog = await _context.Catalogs
            .Include(c => c.Profile)
            .FirstOrDefaultAsync(c => c.Id == dto.CatalogId);

        if (catalog == null || catalog.Profile.UserId != userId)
            return NotFound(new { success = false, error = "Catalog not found" });

        // Create a copy of the item for the catalog
        var catalogItem = new CatalogItem
        {
            CatalogId = dto.CatalogId,
            CategoryId = dto.CategoryId ?? libraryItem.CategoryId,
            ItemType = libraryItem.ItemType,
            BundleId = libraryItem.BundleId,
            Name = libraryItem.Name,
            Description = libraryItem.Description,
            Price = libraryItem.Price,
            Images = libraryItem.Images,
            Tags = libraryItem.Tags,
            AvailableInEventMode = libraryItem.AvailableInEventMode,
            EventModeOnly = libraryItem.EventModeOnly,
            SortOrder = dto.SortOrder,
            SourceLibraryItemId = libraryItem.Id,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            CreatedBy = userId.ToString(),
            UpdatedBy = userId.ToString()
        };

        _context.CatalogItems.Add(catalogItem);
        await _context.SaveChangesAsync();

        // Copy variants
        if (libraryItem.Variants.Any())
        {
            foreach (var sourceVariant in libraryItem.Variants.Where(v => v.IsActive))
            {
                var variant = new CatalogItemVariant
                {
                    CatalogItemId = catalogItem.Id,
                    Title = sourceVariant.Title,
                    Price = sourceVariant.Price,
                    Cost = sourceVariant.Cost,
                    SizeValue = sourceVariant.SizeValue,
                    SizeUnit = sourceVariant.SizeUnit,
                    UnitOfMeasure = sourceVariant.UnitOfMeasure,
                    Sku = sourceVariant.Sku,
                    Barcode = sourceVariant.Barcode,
                    IsDefault = sourceVariant.IsDefault,
                    WeightG = sourceVariant.WeightG,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    CreatedBy = userId.ToString(),
                    UpdatedBy = userId.ToString()
                };
                _context.CatalogItemVariants.Add(variant);
            }
            await _context.SaveChangesAsync();
        }

        // Copy extra group links
        if (libraryItem.ExtraGroupLinks.Any())
        {
            foreach (var sourceLink in libraryItem.ExtraGroupLinks.OrderBy(l => l.DisplayOrder))
            {
                var link = new CatalogItemExtraGroupLink
                {
                    CatalogItemId = catalogItem.Id,
                    ExtraGroupId = sourceLink.ExtraGroupId,
                    DisplayOrder = sourceLink.DisplayOrder,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    CreatedBy = userId.ToString(),
                    UpdatedBy = userId.ToString()
                };
                _context.CatalogItemExtraGroupLinks.Add(link);
            }
            await _context.SaveChangesAsync();
        }

        return Ok(new { success = true, data = new { catalogItemId = catalogItem.Id } });
    }

    private static string[] ParseJsonArray(string? json)
    {
        if (string.IsNullOrEmpty(json))
            return Array.Empty<string>();

        try
        {
            return JsonSerializer.Deserialize<string[]>(json) ?? Array.Empty<string>();
        }
        catch
        {
            return Array.Empty<string>();
        }
    }
}

public class CreateLibraryItemDto
{
    public int? CategoryId { get; set; }
    public CatalogItemType ItemType { get; set; } = CatalogItemType.Regular;
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public List<string>? Images { get; set; }
    public List<string>? Tags { get; set; } // Allergens, dietary info
    public int SortOrder { get; set; } = 0;
    public bool AvailableInEventMode { get; set; } = true;
    public bool EventModeOnly { get; set; } = false;
    public List<CreateVariantDto>? Variants { get; set; }
    public List<int>? ExtraGroupIds { get; set; }
}

public class UpdateLibraryItemDto
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public decimal? Price { get; set; }
    public int? CategoryId { get; set; }
    public List<string>? Images { get; set; }
    public List<string>? Tags { get; set; }
    public int? SortOrder { get; set; }
    public bool? AvailableInEventMode { get; set; }
    public bool? EventModeOnly { get; set; }
    public List<int>? ExtraGroupIds { get; set; }
}

public class CreateVariantDto
{
    public string Title { get; set; } = null!;
    public decimal Price { get; set; }
    public decimal? Cost { get; set; }
    public decimal? SizeValue { get; set; }
    public string? SizeUnit { get; set; }
    public string? UnitOfMeasure { get; set; }
    public string? Sku { get; set; }
    public string? Barcode { get; set; }
    public bool IsDefault { get; set; } = false;
    public int? WeightG { get; set; }
}

public class AddToCatalogDto
{
    public int CatalogId { get; set; }
    public int? CategoryId { get; set; }
    public int SortOrder { get; set; } = 0;
}
