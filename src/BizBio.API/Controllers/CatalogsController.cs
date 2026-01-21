using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BizBio.Infrastructure.Data;
using BizBio.Core.Entities;
using BizBio.Core.DTOs;
using BizBio.Core.Interfaces;
using System.Security.Claims;

namespace BizBio.API.Controllers;

[Route("api/v1/catalogs")]
[ApiController]
[Authorize]
public class CatalogsController : ControllerBase
{
    private readonly ICatalogRepository _catalogRepo;
    private readonly ApplicationDbContext _context;
    private readonly ILogger<CatalogsController> _logger;

    public CatalogsController(
        ICatalogRepository catalogRepo,
        ApplicationDbContext context,
        ILogger<CatalogsController> logger)
    {
        _catalogRepo = catalogRepo;
        _context = context;
        _logger = logger;
    }

    private int GetUserId() => int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

    /// <summary>
    /// Get full catalog details for editing
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetCatalogDetail(int id)
    {
        try
        {
            var userId = GetUserId();
            var catalog = await _catalogRepo.GetDetailByIdAsync(id);

            if (catalog == null)
                return NotFound(new { success = false, error = "Catalog not found" });

            // Verify ownership via Entity
            if (catalog.Entity?.UserId != userId)
                return Forbid();

            // Map to DTO
            var dto = new CatalogDetailDto
            {
                Id = catalog.Id,
                Name = catalog.Name,
                Description = catalog.Description,
                // Note: catalog.Categories is now CatalogCategory junction records
                // Navigate to the actual Category entity
                Categories = catalog.Categories
                    .Where(cc => cc.IsActive)
                    .OrderBy(cc => cc.SortOrder)
                    .Select(cc => new CategoryDetailDto
                    {
                        Id = cc.Category.Id,
                        Name = cc.Category.Name,
                        Description = cc.Category.Description,
                        Icon = cc.Category.Icon,
                        SortOrder = cc.SortOrder, // Use sort order from junction table
                        ItemCount = cc.Category.CatalogItemCategories.Count(cic => cic.CatalogItem.IsActive)
                    })
                    .ToList(),
                Items = catalog.Items
                    .Where(i => i.IsActive)
                    .OrderBy(i => i.SortOrder)
                    .Select(i => new ItemDetailDto
                    {
                        Id = i.Id,
                        Name = i.Name,
                        Description = i.Description,
                        Price = i.Price,
                        Images = string.IsNullOrEmpty(i.Images) ? new List<string>() : System.Text.Json.JsonSerializer.Deserialize<List<string>>(i.Images) ?? new List<string>(),
                        CategoryIds = i.CatalogItemCategories.Select(cic => cic.CategoryId).ToList(),
                        SortOrder = i.SortOrder,
                        VariantCount = i.Variants.Count(v => v.IsActive),
                        HasOptions = i.OptionGroupLinks.Any(l => l.IsActive),
                        HasExtras = i.ExtraGroupLinks.Any(l => l.IsActive)
                    })
                    .ToList(),
                Bundles = catalog.Bundles
                    .Where(b => b.IsActive)
                    .OrderBy(b => b.SortOrder)
                    .Select(b => new BundleDetailDto
                    {
                        Id = b.Id,
                        Name = b.Name,
                        Description = b.Description,
                        BasePrice = b.BasePrice,
                        Images = string.IsNullOrEmpty(b.Images) ? new List<string>() : System.Text.Json.JsonSerializer.Deserialize<List<string>>(b.Images) ?? new List<string>(),
                        CategoryIds = new List<int>(), // TODO: CatalogBundle doesn't support categories yet
                        SortOrder = b.SortOrder
                    })
                    .ToList()
            };

            return Ok(new { success = true, data = dto });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching catalog detail for ID: {CatalogId}", id);
            return StatusCode(500, new { success = false, error = "Failed to fetch catalog details" });
        }
    }

    /// <summary>
    /// Reorder categories
    /// </summary>
    [HttpPut("{id}/categories/reorder")]
    public async Task<IActionResult> ReorderCategories(int id, [FromBody] ReorderDto dto)
    {
        try
        {
            var userId = GetUserId();
            var catalog = await _context.Catalogs
                .Include(c => c.Entity)
                .Include(c => c.Categories)
                .FirstOrDefaultAsync(c => c.Id == id && c.IsActive);

            if (catalog == null)
                return NotFound(new { success = false, error = "Catalog not found" });

            if (catalog.Entity?.UserId != userId)
                return Forbid();

            // Update sort orders
            foreach (var item in dto.Items)
            {
                var category = catalog.Categories.FirstOrDefault(c => c.Id == item.Id);
                if (category != null)
                {
                    category.SortOrder = item.SortOrder;
                    category.UpdatedAt = DateTime.UtcNow;
                    category.UpdatedBy = userId.ToString();
                }
            }

            await _context.SaveChangesAsync();

            // Invalidate cache
            catalog.UpdatedAt = DateTime.UtcNow;
            catalog.UpdatedBy = userId.ToString();
            await _catalogRepo.UpdateAsync(catalog);
            await _catalogRepo.SaveChangesAsync();

            return Ok(new { success = true, message = "Categories reordered successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error reordering categories for catalog: {CatalogId}", id);
            return StatusCode(500, new { success = false, error = "Failed to reorder categories" });
        }
    }

    /// <summary>
    /// Reorder items with optional category changes
    /// </summary>
    [HttpPut("{id}/items/reorder")]
    public async Task<IActionResult> ReorderItems(int id, [FromBody] ReorderItemsDto dto)
    {
        try
        {
            var userId = GetUserId();
            var userIdString = userId.ToString();
            var catalog = await _context.Catalogs
                .Include(c => c.Entity)
                .Include(c => c.Items)
                    .ThenInclude(i => i.CatalogItemCategories)
                .FirstOrDefaultAsync(c => c.Id == id && c.IsActive);

            if (catalog == null)
                return NotFound(new { success = false, error = "Catalog not found" });

            if (catalog.Entity?.UserId != userId)
                return Forbid();

            // Update sort orders and handle category changes
            foreach (var itemDto in dto.Items)
            {
                var item = catalog.Items.FirstOrDefault(i => i.Id == itemDto.Id);
                if (item != null)
                {
                    item.SortOrder = itemDto.SortOrder;
                    item.UpdatedAt = DateTime.UtcNow;
                    item.UpdatedBy = userIdString;

                    // Handle category change if specified
                    if (itemDto.CategoryId.HasValue)
                    {
                        // Check if item already has this category
                        var hasCategory = item.CatalogItemCategories
                            .Any(cic => cic.CategoryId == itemDto.CategoryId.Value);

                        if (!hasCategory)
                        {
                            // Add new category relationship
                            var catalogItemCategory = new CatalogItemCategory
                            {
                                CatalogItemId = item.Id,
                                CategoryId = itemDto.CategoryId.Value,
                                IsActive = true,
                                CreatedAt = DateTime.UtcNow,
                                UpdatedAt = DateTime.UtcNow,
                                CreatedBy = userIdString,
                                UpdatedBy = userIdString
                            };
                            _context.CatalogItemCategories.Add(catalogItemCategory);
                        }
                    }
                }
            }

            await _context.SaveChangesAsync();

            // Invalidate cache
            catalog.UpdatedAt = DateTime.UtcNow;
            catalog.UpdatedBy = userIdString;
            await _catalogRepo.UpdateAsync(catalog);
            await _catalogRepo.SaveChangesAsync();

            return Ok(new { success = true, message = "Items reordered successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error reordering items for catalog: {CatalogId}", id);
            return StatusCode(500, new { success = false, error = "Failed to reorder items" });
        }
    }

    /// <summary>
    /// Add library item to catalog
    /// </summary>
    [HttpPost("{id}/library-items")]
    public async Task<IActionResult> AddLibraryItemToCatalog(int id, [FromBody] AddItemToCatalogDto dto)
    {
        try
        {
            var userId = GetUserId();
            var userIdString = userId.ToString();

            var catalog = await _context.Catalogs
                .Include(c => c.Entity)
                .FirstOrDefaultAsync(c => c.Id == id && c.IsActive);

            if (catalog == null)
                return NotFound(new { success = false, error = "Catalog not found" });

            if (catalog.Entity?.UserId != userId)
                return Forbid();

            // Get the library item
            var libraryItem = await _context.CatalogItems
                .Include(i => i.Variants)
                .Include(i => i.OptionGroupLinks)
                    .ThenInclude(l => l.OptionGroup)
                        .ThenInclude(g => g.GroupItems)
                .Include(i => i.ExtraGroupLinks)
                    .ThenInclude(l => l.ExtraGroup)
                        .ThenInclude(g => g.GroupItems)
                .FirstOrDefaultAsync(i => i.Id == dto.LibraryItemId && i.UserId == userId && i.IsActive);

            if (libraryItem == null)
                return NotFound(new { success = false, error = "Library item not found" });

            // Copy item to catalog
            var catalogItem = new CatalogItem
            {
                CatalogId = catalog.Id,
                UserId = null, // Belongs to catalog now
                Name = libraryItem.Name,
                Description = libraryItem.Description,
                Price = libraryItem.Price,
                Images = libraryItem.Images,
                SortOrder = dto.SortOrder,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                CreatedBy = userIdString,
                UpdatedBy = userIdString
            };

            _context.CatalogItems.Add(catalogItem);
            await _context.SaveChangesAsync();

            // Copy variants
            foreach (var variant in libraryItem.Variants.Where(v => v.IsActive))
            {
                var catalogVariant = new CatalogItemVariant
                {
                    CatalogItemId = catalogItem.Id,
                    Title = variant.Title,
                    Price = variant.Price,
                    SortOrder = variant.SortOrder,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    CreatedBy = userIdString,
                    UpdatedBy = userIdString
                };
                _context.CatalogItemVariants.Add(catalogVariant);
            }

            // Copy option group links
            foreach (var link in libraryItem.OptionGroupLinks.Where(l => l.IsActive))
            {
                var catalogLink = new CatalogItemOptionGroupLink
                {
                    CatalogItemId = catalogItem.Id,
                    OptionGroupId = link.OptionGroupId,
                    VariantId = null, // Variants will have different IDs, handle separately if needed
                    DisplayOrder = link.DisplayOrder,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    CreatedBy = userIdString,
                    UpdatedBy = userIdString
                };
                _context.CatalogItemOptionGroupLinks.Add(catalogLink);
            }

            // Copy extra group links
            foreach (var link in libraryItem.ExtraGroupLinks.Where(l => l.IsActive))
            {
                var catalogLink = new CatalogItemExtraGroupLink
                {
                    CatalogItemId = catalogItem.Id,
                    ExtraGroupId = link.ExtraGroupId,
                    VariantId = null,
                    DisplayOrder = link.DisplayOrder,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    CreatedBy = userIdString,
                    UpdatedBy = userIdString
                };
                _context.CatalogItemExtraGroupLinks.Add(catalogLink);
            }

            // Create category relationships
            foreach (var categoryId in dto.CategoryIds)
            {
                var catalogItemCategory = new CatalogItemCategory
                {
                    CatalogItemId = catalogItem.Id,
                    CategoryId = categoryId,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    CreatedBy = userIdString,
                    UpdatedBy = userIdString
                };
                _context.CatalogItemCategories.Add(catalogItemCategory);
            }

            await _context.SaveChangesAsync();

            // Invalidate cache
            catalog.UpdatedAt = DateTime.UtcNow;
            catalog.UpdatedBy = userIdString;
            await _catalogRepo.UpdateAsync(catalog);
            await _catalogRepo.SaveChangesAsync();

            return Ok(new { success = true, data = new { id = catalogItem.Id }, message = "Item added to catalog successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding item to catalog: {CatalogId}", id);
            return StatusCode(500, new { success = false, error = "Failed to add item to catalog" });
        }
    }

    /// <summary>
    /// Add bundle to catalog
    /// </summary>
    [HttpPost("{id}/bundles")]
    public async Task<IActionResult> AddBundleToCatalog(int id, [FromBody] AddBundleToCatalogDto dto)
    {
        try
        {
            var userId = GetUserId();
            var userIdString = userId.ToString();

            var catalog = await _context.Catalogs
                .Include(c => c.Entity)
                .FirstOrDefaultAsync(c => c.Id == id && c.IsActive);

            if (catalog == null)
                return NotFound(new { success = false, error = "Catalog not found" });

            if (catalog.Entity?.UserId != userId)
                return Forbid();

            // Get the bundle
            var bundle = await _context.Bundles
                .Include(b => b.Sections)
                    .ThenInclude(s => s.Items)
                .FirstOrDefaultAsync(b => b.Id == dto.BundleId && b.UserId == userId && b.IsActive);

            if (bundle == null)
                return NotFound(new { success = false, error = "Bundle not found" });

            // Copy bundle to catalog
            var catalogBundle = new CatalogBundle
            {
                CatalogId = catalog.Id,
                Name = bundle.Name,
                Description = bundle.Description,
                BasePrice = bundle.BasePrice,
                Images = bundle.Images,
                SortOrder = dto.SortOrder,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                CreatedBy = userIdString,
                UpdatedBy = userIdString
            };

            _context.CatalogBundles.Add(catalogBundle);
            await _context.SaveChangesAsync();

            // TODO: Copy bundle steps and products from library Bundle to CatalogBundle
            // The entity structure is different (BundleSection vs CatalogBundleStep)
            // For now, just create the basic bundle without steps

            await _context.SaveChangesAsync();

            // Invalidate cache
            catalog.UpdatedAt = DateTime.UtcNow;
            catalog.UpdatedBy = userIdString;
            await _catalogRepo.UpdateAsync(catalog);
            await _catalogRepo.SaveChangesAsync();

            return Ok(new { success = true, data = new { id = catalogBundle.Id }, message = "Bundle added to catalog successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding bundle to catalog: {CatalogId}", id);
            return StatusCode(500, new { success = false, error = "Failed to add bundle to catalog" });
        }
    }

    /// <summary>
    /// Remove item from catalog (soft delete) - DEPRECATED: Use CatalogItemsController.DeleteItem instead
    /// </summary>
    [HttpDelete("{id}/catalog-items/{itemId}")]
    public async Task<IActionResult> RemoveItemFromCatalog(int id, int itemId)
    {
        try
        {
            var userId = GetUserId();
            var userIdString = userId.ToString();

            var catalog = await _context.Catalogs
                .Include(c => c.Entity)
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.Id == id && c.IsActive);

            if (catalog == null)
                return NotFound(new { success = false, error = "Catalog not found" });

            if (catalog.Entity?.UserId != userId)
                return Forbid();

            var item = catalog.Items.FirstOrDefault(i => i.Id == itemId);
            if (item == null)
                return NotFound(new { success = false, error = "Item not found in catalog" });

            // Soft delete
            item.IsActive = false;
            item.UpdatedAt = DateTime.UtcNow;
            item.UpdatedBy = userIdString;

            await _context.SaveChangesAsync();

            // Invalidate cache
            catalog.UpdatedAt = DateTime.UtcNow;
            catalog.UpdatedBy = userIdString;
            await _catalogRepo.UpdateAsync(catalog);
            await _catalogRepo.SaveChangesAsync();

            return Ok(new { success = true, message = "Item removed from catalog successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing item from catalog: {CatalogId}, ItemId: {ItemId}", id, itemId);
            return StatusCode(500, new { success = false, error = "Failed to remove item from catalog" });
        }
    }

    /// <summary>
    /// Remove bundle from catalog (soft delete)
    /// </summary>
    [HttpDelete("{id}/catalog-bundles/{bundleId}")]
    public async Task<IActionResult> RemoveBundleFromCatalog(int id, int bundleId)
    {
        try
        {
            var userId = GetUserId();
            var userIdString = userId.ToString();

            var catalog = await _context.Catalogs
                .Include(c => c.Entity)
                .Include(c => c.Bundles)
                .FirstOrDefaultAsync(c => c.Id == id && c.IsActive);

            if (catalog == null)
                return NotFound(new { success = false, error = "Catalog not found" });

            if (catalog.Entity?.UserId != userId)
                return Forbid();

            var bundle = catalog.Bundles.FirstOrDefault(b => b.Id == bundleId);
            if (bundle == null)
                return NotFound(new { success = false, error = "Bundle not found in catalog" });

            // Soft delete
            bundle.IsActive = false;
            bundle.UpdatedAt = DateTime.UtcNow;
            bundle.UpdatedBy = userIdString;

            await _context.SaveChangesAsync();

            // Invalidate cache
            catalog.UpdatedAt = DateTime.UtcNow;
            catalog.UpdatedBy = userIdString;
            await _catalogRepo.UpdateAsync(catalog);
            await _catalogRepo.SaveChangesAsync();

            return Ok(new { success = true, message = "Bundle removed from catalog successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing bundle from catalog: {CatalogId}, BundleId: {BundleId}", id, bundleId);
            return StatusCode(500, new { success = false, error = "Failed to remove bundle from catalog" });
        }
    }

    /// <summary>
    /// Update item categories (multi-category assignment)
    /// </summary>
    [HttpPut("{id}/items/{itemId}/categories")]
    public async Task<IActionResult> UpdateItemCategories(int id, int itemId, [FromBody] UpdateItemCategoriesDto dto)
    {
        try
        {
            var userId = GetUserId();
            var userIdString = userId.ToString();

            var catalog = await _context.Catalogs
                .Include(c => c.Entity)
                .Include(c => c.Items)
                    .ThenInclude(i => i.CatalogItemCategories)
                .FirstOrDefaultAsync(c => c.Id == id && c.IsActive);

            if (catalog == null)
                return NotFound(new { success = false, error = "Catalog not found" });

            if (catalog.Entity?.UserId != userId)
                return Forbid();

            var item = catalog.Items.FirstOrDefault(i => i.Id == itemId);
            if (item == null)
                return NotFound(new { success = false, error = "Item not found in catalog" });

            // Remove existing category relationships
            _context.CatalogItemCategories.RemoveRange(item.CatalogItemCategories);

            // Create new category relationships
            foreach (var categoryId in dto.CategoryIds)
            {
                var catalogItemCategory = new CatalogItemCategory
                {
                    CatalogItemId = item.Id,
                    CategoryId = categoryId,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    CreatedBy = userIdString,
                    UpdatedBy = userIdString
                };
                _context.CatalogItemCategories.Add(catalogItemCategory);
            }

            item.UpdatedAt = DateTime.UtcNow;
            item.UpdatedBy = userIdString;

            await _context.SaveChangesAsync();

            // Invalidate cache
            catalog.UpdatedAt = DateTime.UtcNow;
            catalog.UpdatedBy = userIdString;
            await _catalogRepo.UpdateAsync(catalog);
            await _catalogRepo.SaveChangesAsync();

            return Ok(new { success = true, message = "Item categories updated successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating item categories: {CatalogId}, ItemId: {ItemId}", id, itemId);
            return StatusCode(500, new { success = false, error = "Failed to update item categories" });
        }
    }
}
