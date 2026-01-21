using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BizBio.Infrastructure.Data;
using BizBio.Core.Entities;
using System.Security.Claims;

namespace BizBio.API.Controllers;

[Route("api/v1/categories")]
[ApiController]
[Authorize]
public class CategoriesController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<CategoriesController> _logger;

    public CategoriesController(
        ApplicationDbContext context,
        ILogger<CategoriesController> logger)
    {
        _context = context;
        _logger = logger;
    }

    private int GetUserId() => int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

    /// <summary>
    /// Get all categories for a specific entity
    /// </summary>
    [HttpGet("entity/{entityId}")]
    public async Task<IActionResult> GetEntityCategories(int entityId)
    {
        try
        {
            var userId = GetUserId();

            // Verify entity ownership
            var entity = await _context.Entities
                .FirstOrDefaultAsync(e => e.Id == entityId && e.UserId == userId);

            if (entity == null)
                return NotFound(new { success = false, error = "Entity not found" });

            var categories = await _context.Categories_New
                .Where(c => c.EntityId == entityId && c.IsActive)
                .OrderBy(c => c.SortOrder)
                .Select(c => new
                {
                    c.Id,
                    c.EntityId,
                    c.Name,
                    c.Slug,
                    c.Description,
                    c.Icon,
                    c.SortOrder,
                    c.IsActive,
                    c.CreatedAt,
                    c.UpdatedAt,
                    CatalogCount = c.CatalogCategories.Count(),
                    ItemCount = c.CatalogItemCategories.Count(cic => cic.CatalogItem.IsActive)
                })
                .ToListAsync();

            return Ok(new { success = true, categories });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting categories for entity {EntityId}", entityId);
            return StatusCode(500, new { success = false, error = "An error occurred while fetching categories" });
        }
    }

    /// <summary>
    /// Get a specific category by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetCategory(int id)
    {
        try
        {
            var userId = GetUserId();

            var category = await _context.Categories_New
                .Where(c => c.Id == id && c.Entity.UserId == userId)
                .Select(c => new
                {
                    c.Id,
                    c.EntityId,
                    c.Name,
                    c.Slug,
                    c.Description,
                    c.Icon,
                    c.SortOrder,
                    c.IsActive,
                    c.CreatedAt,
                    c.UpdatedAt,
                    Catalogs = c.CatalogCategories.Select(cc => new
                    {
                        cc.CatalogId,
                        cc.Catalog.Name,
                        cc.SortOrder
                    }).ToList()
                })
                .FirstOrDefaultAsync();

            if (category == null)
                return NotFound(new { success = false, error = "Category not found" });

            return Ok(new { success = true, category });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting category {CategoryId}", id);
            return StatusCode(500, new { success = false, error = "An error occurred while fetching the category" });
        }
    }

    /// <summary>
    /// Create a new category for an entity
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> CreateCategory([FromBody] CreateCategoryRequest request)
    {
        try
        {
            var userId = GetUserId();

            // Verify entity ownership
            var entity = await _context.Entities
                .FirstOrDefaultAsync(e => e.Id == request.EntityId && e.UserId == userId);

            if (entity == null)
                return NotFound(new { success = false, error = "Entity not found" });

            // TODO: Check MaxCategoriesPerCatalog limit based on subscription tier

            // Generate slug if not provided
            var slug = !string.IsNullOrEmpty(request.Slug)
                ? request.Slug
                : GenerateSlug(request.Name);

            // Check if slug is unique within this entity
            var slugExists = await _context.Categories_New
                .AnyAsync(c => c.EntityId == request.EntityId && c.Slug == slug && c.IsActive);

            if (slugExists)
            {
                return BadRequest(new { success = false, error = $"A category with the slug '{slug}' already exists for this business. Please use a different name." });
            }

            var userEmail = User.FindFirst(ClaimTypes.Email)!.Value;

            var category = new Category
            {
                EntityId = request.EntityId,
                Name = request.Name,
                Slug = slug,
                Description = request.Description,
                Icon = request.Icon,
                SortOrder = request.SortOrder ?? 0,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                CreatedBy = userEmail,
                UpdatedBy = userEmail
            };

            _context.Categories_New.Add(category);
            await _context.SaveChangesAsync();

            return Ok(new { success = true, category = new
            {
                category.Id,
                category.EntityId,
                category.Name,
                category.Slug,
                category.Description,
                category.Icon,
                category.SortOrder,
                category.IsActive,
                category.CreatedAt,
                category.UpdatedAt
            }});
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating category");
            return StatusCode(500, new { success = false, error = "An error occurred while creating the category" });
        }
    }

    /// <summary>
    /// Update an existing category
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateCategory(int id, [FromBody] UpdateCategoryRequest request)
    {
        try
        {
            var userId = GetUserId();

            var category = await _context.Categories_New
                .Include(c => c.Entity)
                .FirstOrDefaultAsync(c => c.Id == id && c.Entity.UserId == userId);

            if (category == null)
                return NotFound(new { success = false, error = "Category not found" });

            // Update fields
            var userEmail = User.FindFirst(ClaimTypes.Email)!.Value;

            if (!string.IsNullOrEmpty(request.Name))
                category.Name = request.Name;

            if (!string.IsNullOrEmpty(request.Slug) && request.Slug != category.Slug)
            {
                // Check if new slug is unique within this entity
                var slugExists = await _context.Categories_New
                    .AnyAsync(c => c.EntityId == category.EntityId && c.Slug == request.Slug && c.Id != id && c.IsActive);

                if (slugExists)
                {
                    return BadRequest(new { success = false, error = $"A category with the slug '{request.Slug}' already exists for this business." });
                }

                category.Slug = request.Slug;
            }

            if (request.Description != null)
                category.Description = request.Description;

            if (request.Icon != null)
                category.Icon = request.Icon;

            if (request.SortOrder.HasValue)
                category.SortOrder = request.SortOrder.Value;

            category.UpdatedAt = DateTime.UtcNow;
            category.UpdatedBy = userEmail;

            await _context.SaveChangesAsync();

            return Ok(new { success = true, category = new
            {
                category.Id,
                category.EntityId,
                category.Name,
                category.Slug,
                category.Description,
                category.Icon,
                category.SortOrder,
                category.IsActive,
                category.CreatedAt,
                category.UpdatedAt
            }});
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating category {CategoryId}", id);
            return StatusCode(500, new { success = false, error = "An error occurred while updating the category" });
        }
    }

    /// <summary>
    /// Delete a category (soft delete)
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCategory(int id)
    {
        try
        {
            var userId = GetUserId();

            var category = await _context.Categories_New
                .Include(c => c.Entity)
                .Include(c => c.CatalogCategories)
                .FirstOrDefaultAsync(c => c.Id == id && c.Entity.UserId == userId);

            if (category == null)
                return NotFound(new { success = false, error = "Category not found" });

            // Soft delete the category
            var userEmail = User.FindFirst(ClaimTypes.Email)!.Value;
            category.IsActive = false;
            category.UpdatedAt = DateTime.UtcNow;
            category.UpdatedBy = userEmail;

            // Remove from all catalog associations
            _context.CatalogCategories.RemoveRange(category.CatalogCategories);

            await _context.SaveChangesAsync();

            return Ok(new { success = true, message = "Category deleted successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting category {CategoryId}", id);
            return StatusCode(500, new { success = false, error = "An error occurred while deleting the category" });
        }
    }

    /// <summary>
    /// Add a category to a catalog (creates CatalogCategory association)
    /// </summary>
    [HttpPost("{categoryId}/catalogs/{catalogId}")]
    public async Task<IActionResult> AddCategoryToCatalog(int categoryId, int catalogId, [FromBody] AddCategoryToCatalogRequest? request = null)
    {
        try
        {
            var userId = GetUserId();

            // Verify category ownership
            var category = await _context.Categories_New
                .Include(c => c.Entity)
                .FirstOrDefaultAsync(c => c.Id == categoryId && c.Entity.UserId == userId);

            if (category == null)
                return NotFound(new { success = false, error = "Category not found" });

            // Verify catalog ownership and that it belongs to same entity
            var catalog = await _context.Catalogs
                .FirstOrDefaultAsync(c => c.Id == catalogId && c.EntityId == category.EntityId);

            if (catalog == null)
                return NotFound(new { success = false, error = "Catalog not found or does not belong to the same entity" });

            // Check if association already exists
            var existingAssociation = await _context.CatalogCategories
                .AnyAsync(cc => cc.CatalogId == catalogId && cc.CategoryId == categoryId);

            if (existingAssociation)
                return BadRequest(new { success = false, error = "Category is already associated with this catalog" });

            var userEmail = User.FindFirst(ClaimTypes.Email)!.Value;

            var catalogCategory = new CatalogCategory
            {
                CatalogId = catalogId,
                CategoryId = categoryId,
                SortOrder = request?.SortOrder ?? 0,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                CreatedBy = userEmail,
                UpdatedBy = userEmail
            };

            _context.CatalogCategories.Add(catalogCategory);
            await _context.SaveChangesAsync();

            return Ok(new { success = true, message = "Category added to catalog successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding category {CategoryId} to catalog {CatalogId}", categoryId, catalogId);
            return StatusCode(500, new { success = false, error = "An error occurred while adding the category to the catalog" });
        }
    }

    /// <summary>
    /// Remove a category from a catalog
    /// </summary>
    [HttpDelete("{categoryId}/catalogs/{catalogId}")]
    public async Task<IActionResult> RemoveCategoryFromCatalog(int categoryId, int catalogId)
    {
        try
        {
            var userId = GetUserId();

            // Verify ownership
            var category = await _context.Categories_New
                .Include(c => c.Entity)
                .FirstOrDefaultAsync(c => c.Id == categoryId && c.Entity.UserId == userId);

            if (category == null)
                return NotFound(new { success = false, error = "Category not found" });

            var catalogCategory = await _context.CatalogCategories
                .FirstOrDefaultAsync(cc => cc.CatalogId == catalogId && cc.CategoryId == categoryId);

            if (catalogCategory == null)
                return NotFound(new { success = false, error = "Category is not associated with this catalog" });

            _context.CatalogCategories.Remove(catalogCategory);
            await _context.SaveChangesAsync();

            return Ok(new { success = true, message = "Category removed from catalog successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing category {CategoryId} from catalog {CatalogId}", categoryId, catalogId);
            return StatusCode(500, new { success = false, error = "An error occurred while removing the category from the catalog" });
        }
    }

    private string GenerateSlug(string name)
    {
        return name
            .ToLowerInvariant()
            .Replace(" ", "-")
            .Replace("&", "and")
            // Remove special characters
            .Where(c => char.IsLetterOrDigit(c) || c == '-')
            .Aggregate("", (current, c) => current + c);
    }
}

// DTOs
public class CreateCategoryRequest
{
    public int EntityId { get; set; }
    public string Name { get; set; } = null!;
    public string? Slug { get; set; }
    public string? Description { get; set; }
    public string? Icon { get; set; }
    public int? SortOrder { get; set; }
}

public class UpdateCategoryRequest
{
    public string? Name { get; set; }
    public string? Slug { get; set; }
    public string? Description { get; set; }
    public string? Icon { get; set; }
    public int? SortOrder { get; set; }
}

public class AddCategoryToCatalogRequest
{
    public int? SortOrder { get; set; }
}
