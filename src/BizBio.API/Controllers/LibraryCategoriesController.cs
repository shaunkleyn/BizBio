using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BizBio.Core.Entities;
using BizBio.Infrastructure.Data;
using System.Security.Claims;
using System.Text.Json;

namespace BizBio.API.Controllers;

/// <summary>
/// Controller for managing library categories (categories owned by user, not tied to a specific catalog/menu)
/// </summary>
[Route("api/v1/library/categories")]
[ApiController]
[Authorize]
public class LibraryCategoriesController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public LibraryCategoriesController(ApplicationDbContext context)
    {
        _context = context;
    }

    private int GetUserId() => int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

    /// <summary>
    /// Get all library categories for the current user
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetLibraryCategories()
    {
        var userId = GetUserId();

        var categories = await _context.Categories
            .Where(c => c.UserId == userId && c.IsActive)
            .OrderBy(c => c.SortOrder)
            .ThenBy(c => c.Name)
            .Select(c => new
            {
                c.Id,
                c.Name,
                c.Description,
                c.Icon,
                images = ParseJsonArray(c.Images),
                c.SortOrder,
                itemCount = c.CatalogItemCategories.Count(ic => ic.CatalogItem != null && ic.CatalogItem.IsActive)
            })
            .ToListAsync();

        return Ok(new { success = true, data = categories });
    }

    /// <summary>
    /// Get a specific library category by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetLibraryCategory(int id)
    {
        var userId = GetUserId();

        var category = await _context.Categories
            .FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId);

        if (category == null)
            return NotFound(new { success = false, error = "Category not found" });

        return Ok(new
        {
            success = true,
            data = new
            {
                category = new
                {
                    category.Id,
                    category.Name,
                    category.Description,
                    category.Icon,
                    images = ParseJsonArray(category.Images),
                    category.SortOrder
                }
            }
        });
    }

    /// <summary>
    /// Create a new library category
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> CreateLibraryCategory([FromBody] CreateLibraryCategoryDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var userId = GetUserId();

        var category = new CatalogCategory
        {
            UserId = userId,
            CatalogId = null, // Library category
            Name = dto.Name,
            Description = dto.Description,
            Icon = dto.Icon,
            Images = dto.Images != null && dto.Images.Any()
                ? JsonSerializer.Serialize(dto.Images)
                : null,
            SortOrder = dto.SortOrder,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            CreatedBy = userId.ToString(),
            UpdatedBy = userId.ToString()
        };

        _context.Categories.Add(category);
        await _context.SaveChangesAsync();

        return Ok(new { success = true, data = new { category = new { category.Id, category.Name } } });
    }

    /// <summary>
    /// Update a library category
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateLibraryCategory(int id, [FromBody] UpdateLibraryCategoryDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var userId = GetUserId();

        var category = await _context.Categories
            .FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId);

        if (category == null)
            return NotFound(new { success = false, error = "Category not found" });

        // Update fields
        if (dto.Name != null) category.Name = dto.Name;
        if (dto.Description != null) category.Description = dto.Description;
        if (dto.Icon != null) category.Icon = dto.Icon;
        if (dto.Images != null)
            category.Images = dto.Images.Any() ? JsonSerializer.Serialize(dto.Images) : null;
        if (dto.SortOrder.HasValue) category.SortOrder = dto.SortOrder.Value;

        category.UpdatedAt = DateTime.UtcNow;
        category.UpdatedBy = userId.ToString();

        await _context.SaveChangesAsync();

        return Ok(new { success = true, data = new { category = new { category.Id, category.Name } } });
    }

    /// <summary>
    /// Delete a library category
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteLibraryCategory(int id)
    {
        var userId = GetUserId();

        var category = await _context.Categories
            .FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId);

        if (category == null)
            return NotFound(new { success = false, error = "Category not found" });

        // Check if category has items
        var hasItems = await _context.CatalogItemCategories
            .AnyAsync(cic => cic.CategoryId == id && cic.CatalogItem != null && cic.CatalogItem.IsActive);

        if (hasItems)
            return BadRequest(new
            {
                success = false,
                error = "Cannot delete category that contains items. Please move or delete items first."
            });

        category.IsActive = false;
        category.UpdatedAt = DateTime.UtcNow;
        category.UpdatedBy = userId.ToString();

        await _context.SaveChangesAsync();

        return Ok(new { success = true, message = "Category deleted successfully" });
    }

    /// <summary>
    /// Add library category to a catalog/menu
    /// </summary>
    [HttpPost("{id}/add-to-catalog")]
    public async Task<IActionResult> AddToCatalog(int id, [FromBody] AddCategoryToCatalogDto dto)
    {
        var userId = GetUserId();

        var libraryCategory = await _context.Categories
            .FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId);

        if (libraryCategory == null)
            return NotFound(new { success = false, error = "Library category not found" });

        // Verify user owns the catalog
        var catalog = await _context.Catalogs
            .Include(c => c.Profile)
            .FirstOrDefaultAsync(c => c.Id == dto.CatalogId);

        if (catalog == null || catalog.Profile.UserId != userId)
            return NotFound(new { success = false, error = "Catalog not found" });

        // Create a copy of the category for the catalog
        var catalogCategory = new CatalogCategory
        {
            CatalogId = dto.CatalogId,
            Name = libraryCategory.Name,
            Description = libraryCategory.Description,
            Icon = libraryCategory.Icon,
            Images = libraryCategory.Images,
            SortOrder = dto.SortOrder,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            CreatedBy = userId.ToString(),
            UpdatedBy = userId.ToString()
        };

        _context.Categories.Add(catalogCategory);
        await _context.SaveChangesAsync();

        return Ok(new { success = true, data = new { catalogCategoryId = catalogCategory.Id } });
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

public class CreateLibraryCategoryDto
{
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public string? Icon { get; set; }
    public List<string>? Images { get; set; }
    public int SortOrder { get; set; } = 0;
}

public class UpdateLibraryCategoryDto
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? Icon { get; set; }
    public List<string>? Images { get; set; }
    public int? SortOrder { get; set; }
}

public class AddCategoryToCatalogDto
{
    public int CatalogId { get; set; }
    public int SortOrder { get; set; } = 0;
}
