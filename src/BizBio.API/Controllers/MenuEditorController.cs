using BizBio.Core.DTOs;
using BizBio.Core.Entities;
using BizBio.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BizBio.API.Controllers;

/// <summary>
/// Menu Editor API - Comprehensive menu management functionality
/// </summary>
[Route("api/v1/menu-editor")]
[ApiController]
[Authorize]
public class MenuEditorController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public MenuEditorController(ApplicationDbContext context)
    {
        _context = context;
    }

    private int GetUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
            throw new UnauthorizedAccessException("User not authenticated");
        return userId;
    }

    #region Categories

    /// <summary>
    /// Add a new category to a catalog
    /// </summary>
    [HttpPost("catalogs/{catalogId}/categories")]
    public async Task<IActionResult> AddCategory(int catalogId, [FromBody] CategoryCreateDto dto)
    {
        try
        {
            var userId = GetUserId();

            // Verify catalog ownership
            var catalog = await _context.Catalogs
                .Include(c => c.Profile)
                .FirstOrDefaultAsync(c => c.Id == catalogId && c.Profile.UserId == userId);

            if (catalog == null)
                return NotFound(new { success = false, error = "Catalog not found" });

            var category = new CatalogCategory
            {
                CatalogId = catalogId,
                Name = dto.Name,
                Description = dto.Description,
                Icon = dto.Icon,
                SortOrder = dto.SortOrder,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                success = true,
                data = new
                {
                    id = category.Id,
                    name = category.Name,
                    description = category.Description,
                    icon = category.Icon,
                    sortOrder = category.SortOrder
                }
            });
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized(new { success = false, error = "User not authenticated" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, error = "Failed to add category", details = ex.Message });
        }
    }

    /// <summary>
    /// Update category details
    /// </summary>
    [HttpPut("categories/{categoryId}")]
    public async Task<IActionResult> UpdateCategory(int categoryId, [FromBody] CategoryUpdateDto dto)
    {
        try
        {
            var userId = GetUserId();

            var category = await _context.Categories
                .Include(c => c.Catalog)
                    .ThenInclude(cat => cat.Profile)
                .FirstOrDefaultAsync(c => c.Id == categoryId && c.Catalog.Profile.UserId == userId);

            if (category == null)
                return NotFound(new { success = false, error = "Category not found" });

            category.Name = dto.Name ?? category.Name;
            category.Description = dto.Description ?? category.Description;
            category.Icon = dto.Icon ?? category.Icon;
            category.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Ok(new { success = true, message = "Category updated successfully" });
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized(new { success = false, error = "User not authenticated" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, error = "Failed to update category", details = ex.Message });
        }
    }

    /// <summary>
    /// Delete/deactivate a category
    /// </summary>
    [HttpDelete("categories/{categoryId}")]
    public async Task<IActionResult> DeleteCategory(int categoryId)
    {
        try
        {
            var userId = GetUserId();

            var category = await _context.Categories
                .Include(c => c.Catalog)
                    .ThenInclude(cat => cat.Profile)
                .FirstOrDefaultAsync(c => c.Id == categoryId && c.Catalog.Profile.UserId == userId);

            if (category == null)
                return NotFound(new { success = false, error = "Category not found" });

            category.IsActive = false;
            category.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Ok(new { success = true, message = "Category deleted successfully" });
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized(new { success = false, error = "User not authenticated" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, error = "Failed to delete category", details = ex.Message });
        }
    }

    /// <summary>
    /// Reorder categories
    /// </summary>
    [HttpPut("catalogs/{catalogId}/categories/reorder")]
    public async Task<IActionResult> ReorderCategories(int catalogId, [FromBody] ReorderDto dto)
    {
        try
        {
            var userId = GetUserId();

            // Verify catalog ownership
            var catalog = await _context.Catalogs
                .Include(c => c.Profile)
                .FirstOrDefaultAsync(c => c.Id == catalogId && c.Profile.UserId == userId);

            if (catalog == null)
                return NotFound(new { success = false, error = "Catalog not found" });

            foreach (var item in dto.Items)
            {
                var category = await _context.Categories.FindAsync(item.Id);
                if (category != null && category.CatalogId == catalogId)
                {
                    category.SortOrder = item.SortOrder;
                    category.UpdatedAt = DateTime.UtcNow;
                }
            }

            await _context.SaveChangesAsync();

            return Ok(new { success = true, message = "Categories reordered successfully" });
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized(new { success = false, error = "User not authenticated" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, error = "Failed to reorder categories", details = ex.Message });
        }
    }

    #endregion

    #region Items

    /// <summary>
    /// Add library item to catalog with categories
    /// </summary>
    [HttpPost("catalogs/{catalogId}/items")]
    public async Task<IActionResult> AddItemToCatalog(int catalogId, [FromBody] AddItemToCatalogDto dto)
    {
        try
        {
            var userId = GetUserId();

            // Verify catalog ownership
            var catalog = await _context.Catalogs
                .Include(c => c.Profile)
                .FirstOrDefaultAsync(c => c.Id == catalogId && c.Profile.UserId == userId);

            if (catalog == null)
                return NotFound(new { success = false, error = "Catalog not found" });

            // Get the library item (items owned by user, not assigned to a catalog)
            var libraryItem = await _context.CatalogItems
                .Include(i => i.Variants)
                .Include(i => i.OptionGroupLinks)
                    .ThenInclude(l => l.OptionGroup)
                .Include(i => i.ExtraGroupLinks)
                    .ThenInclude(l => l.ExtraGroup)
                .FirstOrDefaultAsync(i => i.Id == dto.LibraryItemId && i.UserId == userId && i.CatalogId == null);

            if (libraryItem == null)
                return NotFound(new { success = false, error = "Library item not found or not owned by user" });

            // Create catalog item (copy from library)
            var catalogItem = new CatalogItem
            {
                CatalogId = catalogId,
                Name = libraryItem.Name,
                Description = libraryItem.Description,
                Price = libraryItem.Price,
                Images = libraryItem.Images,
                Tags = libraryItem.Tags,
                ItemType = libraryItem.ItemType,
                SortOrder = dto.SortOrder,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.CatalogItems.Add(catalogItem);
            await _context.SaveChangesAsync();

            // Copy variants from library item
            foreach (var variant in libraryItem.Variants.Where(v => v.IsActive))
            {
                var catalogVariant = new CatalogItemVariant
                {
                    CatalogItemId = catalogItem.Id,
                    Title = variant.Title,
                    Price = variant.Price,
                    SizeValue = variant.SizeValue,
                    SizeUnit = variant.SizeUnit,
                    IsDefault = variant.IsDefault,
                    SortOrder = variant.SortOrder,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
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
                    DisplayOrder = link.DisplayOrder,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
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
                    DisplayOrder = link.DisplayOrder,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                _context.CatalogItemExtraGroupLinks.Add(catalogLink);
            }

            // Add to categories
            foreach (var categoryId in dto.CategoryIds)
            {
                var itemCategory = new CatalogItemCategory
                {
                    CatalogItemId = catalogItem.Id,
                    CategoryId = categoryId,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                _context.CatalogItemCategories.Add(itemCategory);
            }

            await _context.SaveChangesAsync();

            return Ok(new { success = true, data = new { id = catalogItem.Id } });
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized(new { success = false, error = "User not authenticated" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, error = "Failed to add item to catalog", details = ex.Message });
        }
    }

    /// <summary>
    /// Update item categories
    /// </summary>
    [HttpPut("items/{itemId}/categories")]
    public async Task<IActionResult> UpdateItemCategories(int itemId, [FromBody] UpdateItemCategoriesDto dto)
    {
        try
        {
            var userId = GetUserId();

            var item = await _context.CatalogItems
                .Include(i => i.Catalog)
                    .ThenInclude(c => c.Profile)
                .Include(i => i.CatalogItemCategories)
                .FirstOrDefaultAsync(i => i.Id == itemId && i.Catalog.Profile.UserId == userId);

            if (item == null)
                return NotFound(new { success = false, error = "Item not found" });

            // Remove existing categories
            _context.CatalogItemCategories.RemoveRange(item.CatalogItemCategories);

            // Add new categories
            foreach (var categoryId in dto.CategoryIds)
            {
                var itemCategory = new CatalogItemCategory
                {
                    CatalogItemId = itemId,
                    CategoryId = categoryId,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                _context.CatalogItemCategories.Add(itemCategory);
            }

            item.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return Ok(new { success = true, message = "Item categories updated successfully" });
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized(new { success = false, error = "User not authenticated" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, error = "Failed to update item categories", details = ex.Message });
        }
    }

    /// <summary>
    /// Reorder items
    /// </summary>
    [HttpPut("catalogs/{catalogId}/items/reorder")]
    public async Task<IActionResult> ReorderItems(int catalogId, [FromBody] ReorderDto dto)
    {
        try
        {
            var userId = GetUserId();

            // Verify catalog ownership
            var catalog = await _context.Catalogs
                .Include(c => c.Profile)
                .FirstOrDefaultAsync(c => c.Id == catalogId && c.Profile.UserId == userId);

            if (catalog == null)
                return NotFound(new { success = false, error = "Catalog not found" });

            foreach (var item in dto.Items)
            {
                var catalogItem = await _context.CatalogItems.FindAsync(item.Id);
                if (catalogItem != null && catalogItem.CatalogId == catalogId)
                {
                    catalogItem.SortOrder = item.SortOrder;
                    catalogItem.UpdatedAt = DateTime.UtcNow;
                }
            }

            await _context.SaveChangesAsync();

            return Ok(new { success = true, message = "Items reordered successfully" });
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized(new { success = false, error = "User not authenticated" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, error = "Failed to reorder items", details = ex.Message });
        }
    }

    /// <summary>
    /// Remove item from catalog
    /// </summary>
    [HttpDelete("items/{itemId}")]
    public async Task<IActionResult> RemoveItem(int itemId)
    {
        try
        {
            var userId = GetUserId();

            var item = await _context.CatalogItems
                .Include(i => i.Catalog)
                    .ThenInclude(c => c.Profile)
                .FirstOrDefaultAsync(i => i.Id == itemId && i.Catalog.Profile.UserId == userId);

            if (item == null)
                return NotFound(new { success = false, error = "Item not found" });

            item.IsActive = false;
            item.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Ok(new { success = true, message = "Item removed from catalog" });
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized(new { success = false, error = "User not authenticated" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, error = "Failed to remove item", details = ex.Message });
        }
    }

    #endregion

    #region Bundles

    /// <summary>
    /// Add bundle to catalog with categories
    /// </summary>
    [HttpPost("catalogs/{catalogId}/bundles")]
    public async Task<IActionResult> AddBundleToCatalog(int catalogId, [FromBody] AddBundleToCatalogDto dto)
    {
        try
        {
            var userId = GetUserId();

            // Verify catalog ownership
            var catalog = await _context.Catalogs
                .Include(c => c.Profile)
                .FirstOrDefaultAsync(c => c.Id == catalogId && c.Profile.UserId == userId);

            if (catalog == null)
                return NotFound(new { success = false, error = "Catalog not found" });

            // Get the bundle from library
            var libraryBundle = await _context.Bundles
                .Include(b => b.Sections)
                    .ThenInclude(s => s.Items)
                .FirstOrDefaultAsync(b => b.Id == dto.BundleId && b.UserId == userId);

            if (libraryBundle == null)
                return NotFound(new { success = false, error = "Bundle not found in library" });

            // Create catalog bundle
            var catalogBundle = new CatalogBundle
            {
                CatalogId = catalogId,
                Name = libraryBundle.Name,
                Description = libraryBundle.Description,
                BasePrice = libraryBundle.BasePrice,
                Images = libraryBundle.Images,
                SortOrder = dto.SortOrder,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.CatalogBundles.Add(catalogBundle);
            await _context.SaveChangesAsync();

            // Add to categories
            foreach (var categoryId in dto.CategoryIds)
            {
                var bundleCategory = new CatalogBundleCategory
                {
                    CatalogBundleId = catalogBundle.Id,
                    CategoryId = categoryId,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                _context.CatalogBundleCategories.Add(bundleCategory);
            }

            await _context.SaveChangesAsync();

            return Ok(new { success = true, data = new { id = catalogBundle.Id } });
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized(new { success = false, error = "User not authenticated" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, error = "Failed to add bundle to catalog", details = ex.Message });
        }
    }

    /// <summary>
    /// Update bundle categories
    /// </summary>
    [HttpPut("bundles/{bundleId}/categories")]
    public async Task<IActionResult> UpdateBundleCategories(int bundleId, [FromBody] UpdateBundleCategoriesDto dto)
    {
        try
        {
            var userId = GetUserId();

            var bundle = await _context.CatalogBundles
                .Include(b => b.Catalog)
                    .ThenInclude(c => c.Profile)
                .Include(b => b.CatalogBundleCategories)
                .FirstOrDefaultAsync(b => b.Id == bundleId && b.Catalog.Profile.UserId == userId);

            if (bundle == null)
                return NotFound(new { success = false, error = "Bundle not found" });

            // Remove existing categories
            _context.CatalogBundleCategories.RemoveRange(bundle.CatalogBundleCategories);

            // Add new categories
            foreach (var categoryId in dto.CategoryIds)
            {
                var bundleCategory = new CatalogBundleCategory
                {
                    CatalogBundleId = bundleId,
                    CategoryId = categoryId,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                _context.CatalogBundleCategories.Add(bundleCategory);
            }

            bundle.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return Ok(new { success = true, message = "Bundle categories updated successfully" });
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized(new { success = false, error = "User not authenticated" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, error = "Failed to update bundle categories", details = ex.Message });
        }
    }

    /// <summary>
    /// Reorder bundles
    /// </summary>
    [HttpPut("catalogs/{catalogId}/bundles/reorder")]
    public async Task<IActionResult> ReorderBundles(int catalogId, [FromBody] ReorderDto dto)
    {
        try
        {
            var userId = GetUserId();

            // Verify catalog ownership
            var catalog = await _context.Catalogs
                .Include(c => c.Profile)
                .FirstOrDefaultAsync(c => c.Id == catalogId && c.Profile.UserId == userId);

            if (catalog == null)
                return NotFound(new { success = false, error = "Catalog not found" });

            foreach (var item in dto.Items)
            {
                var bundle = await _context.CatalogBundles.FindAsync(item.Id);
                if (bundle != null && bundle.CatalogId == catalogId)
                {
                    bundle.SortOrder = item.SortOrder;
                    bundle.UpdatedAt = DateTime.UtcNow;
                }
            }

            await _context.SaveChangesAsync();

            return Ok(new { success = true, message = "Bundles reordered successfully" });
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized(new { success = false, error = "User not authenticated" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, error = "Failed to reorder bundles", details = ex.Message });
        }
    }

    /// <summary>
    /// Remove bundle from catalog
    /// </summary>
    [HttpDelete("bundles/{bundleId}")]
    public async Task<IActionResult> RemoveBundle(int bundleId)
    {
        try
        {
            var userId = GetUserId();

            var bundle = await _context.CatalogBundles
                .Include(b => b.Catalog)
                    .ThenInclude(c => c.Profile)
                .FirstOrDefaultAsync(b => b.Id == bundleId && b.Catalog.Profile.UserId == userId);

            if (bundle == null)
                return NotFound(new { success = false, error = "Bundle not found" });

            bundle.IsActive = false;
            bundle.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Ok(new { success = true, message = "Bundle removed from catalog" });
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized(new { success = false, error = "User not authenticated" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, error = "Failed to remove bundle", details = ex.Message });
        }
    }

    #endregion

    #region Bulk Operations

    /// <summary>
    /// Bulk update - reorder all items in a category
    /// </summary>
    [HttpPut("categories/{categoryId}/items/reorder")]
    public async Task<IActionResult> ReorderCategoryItems(int categoryId, [FromBody] ReorderDto dto)
    {
        try
        {
            var userId = GetUserId();

            var category = await _context.Categories
                .Include(c => c.Catalog)
                    .ThenInclude(cat => cat.Profile)
                .FirstOrDefaultAsync(c => c.Id == categoryId && c.Catalog.Profile.UserId == userId);

            if (category == null)
                return NotFound(new { success = false, error = "Category not found" });

            foreach (var item in dto.Items)
            {
                var catalogItem = await _context.CatalogItems.FindAsync(item.Id);
                if (catalogItem != null)
                {
                    catalogItem.SortOrder = item.SortOrder;
                    catalogItem.UpdatedAt = DateTime.UtcNow;
                }
            }

            await _context.SaveChangesAsync();

            return Ok(new { success = true, message = "Category items reordered successfully" });
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized(new { success = false, error = "User not authenticated" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, error = "Failed to reorder category items", details = ex.Message });
        }
    }

    #endregion
}
