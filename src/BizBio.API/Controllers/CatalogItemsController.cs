using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BizBio.Infrastructure.Data;
using BizBio.Core.Entities;
using System.Security.Claims;

namespace BizBio.API.Controllers;

[Route("api/v1/catalogs/{catalogId}/items")]
[ApiController]
[Authorize]
public class CatalogItemsController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<CatalogItemsController> _logger;

    public CatalogItemsController(
        ApplicationDbContext context,
        ILogger<CatalogItemsController> logger)
    {
        _context = context;
        _logger = logger;
    }

    private int GetUserId() => int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

    /// <summary>
    /// Get all items for a specific catalog
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetCatalogItems(int catalogId)
    {
        try
        {
            var userId = GetUserId();

            // Verify catalog ownership
            var catalog = await _context.Catalogs
                .Include(c => c.Entity)
                .FirstOrDefaultAsync(c => c.Id == catalogId && c.Entity.UserId == userId);

            if (catalog == null)
                return NotFound(new { success = false, error = "Catalog not found" });

            var items = await _context.CatalogItems
                .Where(i => i.CatalogId == catalogId && i.IsActive)
                .OrderBy(i => i.SortOrder)
                .Select(i => new
                {
                    i.Id,
                    i.CatalogId,
                    i.Name,
                    i.Description,
                    i.Price,
                    i.PriceOverride,
                    EffectivePrice = i.PriceOverride ?? (i.ParentCatalogItemId != null ? i.ParentCatalogItem!.Price : i.Price),
                    i.ParentCatalogItemId,
                    ParentItemName = i.ParentCatalogItemId != null ? i.ParentCatalogItem!.Name : null,
                    IsSharedItem = i.ParentCatalogItemId != null,
                    i.Images,
                    i.Tags,
                    i.SortOrder,
                    i.ItemType,
                    i.AvailableInEventMode,
                    i.EventModeOnly,
                    CategoryIds = i.CatalogItemCategories.Select(cic => cic.CategoryId).ToList(),
                    VariantCount = i.Variants.Count(v => v.IsActive),
                    HasOptions = i.OptionGroupLinks.Any(l => l.IsActive),
                    HasExtras = i.ExtraGroupLinks.Any(l => l.IsActive)
                })
                .ToListAsync();

            return Ok(new { success = true, items });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting items for catalog {CatalogId}", catalogId);
            return StatusCode(500, new { success = false, error = "An error occurred while fetching catalog items" });
        }
    }

    /// <summary>
    /// Add an item to a catalog (can be new item or reference to existing item with optional price override)
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> AddItemToCatalog(int catalogId, [FromBody] AddItemToCatalogRequest request)
    {
        try
        {
            var userId = GetUserId();

            // Verify catalog ownership
            var catalog = await _context.Catalogs
                .Include(c => c.Entity)
                .FirstOrDefaultAsync(c => c.Id == catalogId && c.Entity.UserId == userId);

            if (catalog == null)
                return NotFound(new { success = false, error = "Catalog not found" });

            CatalogItem item;

            var userEmail = User.FindFirst(ClaimTypes.Email)!.Value;

            if (request.ParentCatalogItemId.HasValue)
            {
                // Creating a reference to an existing item (item sharing with optional price override)
                var parentItem = await _context.CatalogItems
                    .Include(i => i.Catalog)
                        .ThenInclude(c => c.Entity)
                    .FirstOrDefaultAsync(i => i.Id == request.ParentCatalogItemId.Value);

                if (parentItem == null)
                    return NotFound(new { success = false, error = "Parent item not found" });

                // Verify parent item belongs to same entity
                if (parentItem.Catalog?.EntityId != catalog.EntityId)
                    return BadRequest(new { success = false, error = "Items can only be shared within the same entity" });

                // Create reference item
                item = new CatalogItem
                {
                    CatalogId = catalogId,
                    ParentCatalogItemId = request.ParentCatalogItemId.Value,
                    PriceOverride = request.PriceOverride,
                    // Copy data from parent
                    Name = parentItem.Name,
                    Description = parentItem.Description,
                    Price = parentItem.Price,
                    Images = parentItem.Images,
                    Tags = parentItem.Tags,
                    ItemType = parentItem.ItemType,
                    AvailableInEventMode = parentItem.AvailableInEventMode,
                    EventModeOnly = parentItem.EventModeOnly,
                    SortOrder = request.SortOrder ?? 0,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    CreatedBy = userEmail,
                    UpdatedBy = userEmail
                };
            }
            else
            {
                // Creating a new master item
                item = new CatalogItem
                {
                    CatalogId = catalogId,
                    Name = request.Name!,
                    Description = request.Description,
                    Price = request.Price ?? 0,
                    Images = request.Images,
                    Tags = request.Tags,
                    ItemType = request.ItemType ?? CatalogItemType.Regular,
                    AvailableInEventMode = request.AvailableInEventMode ?? true,
                    EventModeOnly = request.EventModeOnly ?? false,
                    SortOrder = request.SortOrder ?? 0,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    CreatedBy = userEmail,
                    UpdatedBy = userEmail
                };
            }

            _context.CatalogItems.Add(item);
            await _context.SaveChangesAsync();

            return Ok(new { success = true, item = new
            {
                item.Id,
                item.CatalogId,
                item.Name,
                item.Description,
                item.Price,
                item.PriceOverride,
                item.ParentCatalogItemId,
                item.Images,
                item.Tags,
                item.SortOrder,
                item.ItemType,
                item.CreatedAt
            }});
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding item to catalog {CatalogId}", catalogId);
            return StatusCode(500, new { success = false, error = "An error occurred while adding the item" });
        }
    }

    /// <summary>
    /// Update price override for a shared item
    /// </summary>
    [HttpPut("{itemId}/price-override")]
    public async Task<IActionResult> UpdatePriceOverride(int catalogId, int itemId, [FromBody] UpdatePriceOverrideRequest request)
    {
        try
        {
            var userId = GetUserId();

            var item = await _context.CatalogItems
                .Include(i => i.Catalog)
                    .ThenInclude(c => c.Entity)
                .Include(i => i.ParentCatalogItem)
                .FirstOrDefaultAsync(i => i.Id == itemId && i.CatalogId == catalogId && i.Catalog.Entity.UserId == userId);

            if (item == null)
                return NotFound(new { success = false, error = "Item not found" });

            if (!item.ParentCatalogItemId.HasValue)
                return BadRequest(new { success = false, error = "Price override can only be set on shared items (items with a parent)" });

            var userEmail = User.FindFirst(ClaimTypes.Email)!.Value;
            item.PriceOverride = request.PriceOverride;
            item.UpdatedAt = DateTime.UtcNow;
            item.UpdatedBy = userEmail;

            await _context.SaveChangesAsync();

            var effectivePrice = item.PriceOverride ?? item.ParentCatalogItem?.Price ?? item.Price;

            return Ok(new { success = true, message = "Price override updated", priceOverride = item.PriceOverride, effectivePrice });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating price override for item {ItemId}", itemId);
            return StatusCode(500, new { success = false, error = "An error occurred while updating the price override" });
        }
    }

    /// <summary>
    /// Copy an item to another catalog (creates independent copy, not a reference)
    /// </summary>
    [HttpPost("{itemId}/copy-to-catalog")]
    public async Task<IActionResult> CopyItemToAnotherCatalog(int catalogId, int itemId, [FromBody] CopyItemToCatalogRequest request)
    {
        try
        {
            var userId = GetUserId();

            // Get source item
            var sourceItem = await _context.CatalogItems
                .Include(i => i.Catalog)
                    .ThenInclude(c => c.Entity)
                .FirstOrDefaultAsync(i => i.Id == itemId && i.CatalogId == catalogId && i.Catalog.Entity.UserId == userId);

            if (sourceItem == null)
                return NotFound(new { success = false, error = "Source item not found" });

            // Get target catalog
            var targetCatalog = await _context.Catalogs
                .Include(c => c.Entity)
                .FirstOrDefaultAsync(c => c.Id == request.TargetCatalogId && c.Entity.UserId == userId);

            if (targetCatalog == null)
                return NotFound(new { success = false, error = "Target catalog not found" });

            // Determine if this is same-entity sharing or cross-entity copy
            bool sameEntity = sourceItem.Catalog.EntityId == targetCatalog.EntityId;

            var userEmail = User.FindFirst(ClaimTypes.Email)!.Value;
            CatalogItem newItem;

            if (sameEntity && request.CreateReference == true)
            {
                // Same entity - create reference with optional price override
                // Get the master item (if source is already a reference, use its parent)
                var masterItemId = sourceItem.ParentCatalogItemId ?? sourceItem.Id;

                newItem = new CatalogItem
                {
                    CatalogId = request.TargetCatalogId,
                    ParentCatalogItemId = masterItemId,
                    PriceOverride = request.PriceOverride,
                    Name = sourceItem.Name,
                    Description = sourceItem.Description,
                    Price = sourceItem.Price,
                    Images = sourceItem.Images,
                    Tags = sourceItem.Tags,
                    ItemType = sourceItem.ItemType,
                    AvailableInEventMode = sourceItem.AvailableInEventMode,
                    EventModeOnly = sourceItem.EventModeOnly,
                    SortOrder = 0,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    CreatedBy = userEmail,
                    UpdatedBy = userEmail
                };
            }
            else
            {
                // Cross-entity or explicit copy request - create independent copy
                newItem = new CatalogItem
                {
                    CatalogId = request.TargetCatalogId,
                    ParentCatalogItemId = null, // No parent reference
                    Name = sourceItem.Name,
                    Description = sourceItem.Description,
                    Price = sourceItem.Price,
                    Images = sourceItem.Images,
                    Tags = sourceItem.Tags,
                    ItemType = sourceItem.ItemType,
                    AvailableInEventMode = sourceItem.AvailableInEventMode,
                    EventModeOnly = sourceItem.EventModeOnly,
                    SortOrder = 0,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    CreatedBy = userEmail,
                    UpdatedBy = userEmail
                };
            }

            _context.CatalogItems.Add(newItem);
            await _context.SaveChangesAsync();

            return Ok(new { success = true, message = sameEntity && request.CreateReference == true ? "Item reference created" : "Item copied", item = new
            {
                newItem.Id,
                newItem.CatalogId,
                newItem.Name,
                newItem.ParentCatalogItemId,
                newItem.PriceOverride,
                IsReference = newItem.ParentCatalogItemId.HasValue
            }});
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error copying item {ItemId} to catalog {TargetCatalogId}", itemId, request.TargetCatalogId);
            return StatusCode(500, new { success = false, error = "An error occurred while copying the item" });
        }
    }

    /// <summary>
    /// Get all items that reference this item (child items)
    /// </summary>
    [HttpGet("{itemId}/references")]
    public async Task<IActionResult> GetItemReferences(int catalogId, int itemId)
    {
        try
        {
            var userId = GetUserId();

            // Verify item ownership
            var item = await _context.CatalogItems
                .Include(i => i.Catalog)
                    .ThenInclude(c => c.Entity)
                .FirstOrDefaultAsync(i => i.Id == itemId && i.CatalogId == catalogId && i.Catalog.Entity.UserId == userId);

            if (item == null)
                return NotFound(new { success = false, error = "Item not found" });

            // Get all child items that reference this item
            var references = await _context.CatalogItems
                .Where(i => i.ParentCatalogItemId == itemId && i.IsActive)
                .Select(i => new
                {
                    i.Id,
                    i.CatalogId,
                    CatalogName = i.Catalog!.Name,
                    i.PriceOverride,
                    EffectivePrice = i.PriceOverride ?? item.Price,
                    HasPriceOverride = i.PriceOverride.HasValue
                })
                .ToListAsync();

            return Ok(new { success = true, references, count = references.Count });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting references for item {ItemId}", itemId);
            return StatusCode(500, new { success = false, error = "An error occurred while fetching item references" });
        }
    }

    /// <summary>
    /// Delete an item from catalog (soft delete)
    /// </summary>
    [HttpDelete("{itemId}")]
    public async Task<IActionResult> DeleteItem(int catalogId, int itemId)
    {
        try
        {
            var userId = GetUserId();

            var item = await _context.CatalogItems
                .Include(i => i.Catalog)
                    .ThenInclude(c => c.Entity)
                .Include(i => i.ChildCatalogItems)
                .FirstOrDefaultAsync(i => i.Id == itemId && i.CatalogId == catalogId && i.Catalog.Entity.UserId == userId);

            if (item == null)
                return NotFound(new { success = false, error = "Item not found" });

            // Warn if this item is referenced by other items
            if (item.ChildCatalogItems.Any(c => c.IsActive))
            {
                return BadRequest(new { success = false, error = $"This item is referenced by {item.ChildCatalogItems.Count(c => c.IsActive)} other item(s). Delete those references first or they will lose their parent reference." });
            }

            var userEmail = User.FindFirst(ClaimTypes.Email)!.Value;
            item.IsActive = false;
            item.UpdatedAt = DateTime.UtcNow;
            item.UpdatedBy = userEmail;

            await _context.SaveChangesAsync();

            return Ok(new { success = true, message = "Item deleted successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting item {ItemId}", itemId);
            return StatusCode(500, new { success = false, error = "An error occurred while deleting the item" });
        }
    }
}

// DTOs
public class AddItemToCatalogRequest
{
    // For new master items
    public string? Name { get; set; }
    public string? Description { get; set; }
    public decimal? Price { get; set; }
    public string? Images { get; set; }
    public string? Tags { get; set; }
    public CatalogItemType? ItemType { get; set; }
    public bool? AvailableInEventMode { get; set; }
    public bool? EventModeOnly { get; set; }
    public int? SortOrder { get; set; }

    // For item sharing (references)
    public int? ParentCatalogItemId { get; set; }
    public decimal? PriceOverride { get; set; }
}

public class UpdatePriceOverrideRequest
{
    public decimal? PriceOverride { get; set; }
}

public class CopyItemToCatalogRequest
{
    public int TargetCatalogId { get; set; }
    public bool? CreateReference { get; set; } // Only works for same entity
    public decimal? PriceOverride { get; set; } // Only applies if CreateReference is true
}
