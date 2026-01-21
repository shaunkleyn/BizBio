using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BizBio.Core.Entities;
using BizBio.Infrastructure.Data;
using System.Security.Claims;
using System.Text.Json;

namespace BizBio.API.Controllers;

/// <summary>
/// DEPRECATED: Controller for managing library categories (categories owned by user, not tied to a specific catalog/menu)
/// This controller is deprecated as of the multi-product architecture migration.
/// Categories now belong to Entities, not to users directly.
/// CatalogCategory is now a junction table, not a category entity.
/// Use CategoriesController for entity-level category management instead.
/// </summary>
[Route("api/v1/library/categories")]
[ApiController]
[Authorize]
[Obsolete("This controller is deprecated. Use CategoriesController for entity-level category management.")]
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
        // This endpoint is deprecated - Categories now belong to Entities
        return StatusCode(501, new
        {
            success = false,
            error = "Endpoint deprecated",
            message = "Use CategoriesController to get entity-level categories"
        });
    }

    /// <summary>
    /// Get a specific library category by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetLibraryCategory(int id)
    {
        return StatusCode(501, new
        {
            success = false,
            error = "Endpoint deprecated",
            message = "Use CategoriesController to get entity-level categories"
        });
    }

    /// <summary>
    /// Create a new library category
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> CreateLibraryCategory([FromBody] CreateLibraryCategoryDto dto)
    {
        return StatusCode(501, new
        {
            success = false,
            error = "Endpoint deprecated",
            message = "Use CategoriesController to create entity-level categories"
        });
    }

    /// <summary>
    /// Update a library category
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateLibraryCategory(int id, [FromBody] UpdateLibraryCategoryDto dto)
    {
        return StatusCode(501, new
        {
            success = false,
            error = "Endpoint deprecated",
            message = "Use CategoriesController to update entity-level categories"
        });
    }

    /// <summary>
    /// Delete a library category
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteLibraryCategory(int id)
    {
        return StatusCode(501, new
        {
            success = false,
            error = "Endpoint deprecated",
            message = "Use CategoriesController to delete entity-level categories"
        });
    }

    /// <summary>
    /// Add library category to a catalog/menu
    /// </summary>
    [HttpPost("{id}/add-to-catalog")]
    public async Task<IActionResult> AddToCatalog(int id, [FromBody] AddCategoryToCatalogDto dto)
    {
        return StatusCode(501, new
        {
            success = false,
            error = "Endpoint deprecated",
            message = "Use CategoriesController.AddCategoryToCatalog to link entity-level categories to catalogs"
        });
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
