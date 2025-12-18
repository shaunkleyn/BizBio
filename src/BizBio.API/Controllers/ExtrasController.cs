using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BizBio.Infrastructure.Data;
using BizBio.Core.Entities;
using System.Security.Claims;

namespace BizBio.API.Controllers;

[Route("api/v1/library/extras")]
[ApiController]
[Authorize]
public class ExtrasController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<ExtrasController> _logger;

    public ExtrasController(ApplicationDbContext context, ILogger<ExtrasController> logger)
    {
        _context = context;
        _logger = logger;
    }

    private int GetUserId() => int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

    /// <summary>
    /// Get all extras for the current user
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetExtras()
    {
        try
        {
            var userId = GetUserId();
            var extras = await _context.CatalogItemExtras
                .Where(e => e.UserId == userId && e.CatalogId == null && e.IsActive)
                .OrderBy(e => e.DisplayOrder)
                .ThenBy(e => e.Name)
                .ToListAsync();

            return Ok(new { success = true, data = extras });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching extras");
            return StatusCode(500, new { success = false, error = "Failed to fetch extras" });
        }
    }

    /// <summary>
    /// Get a single extra by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetExtra(int id)
    {
        try
        {
            var userId = GetUserId();
            var extra = await _context.CatalogItemExtras
                .FirstOrDefaultAsync(e => e.Id == id && e.UserId == userId && e.IsActive);

            if (extra == null)
                return NotFound(new { success = false, error = "Extra not found" });

            return Ok(new { success = true, data = extra });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching extra {Id}", id);
            return StatusCode(500, new { success = false, error = "Failed to fetch extra" });
        }
    }

    /// <summary>
    /// Create a new extra
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> CreateExtra([FromBody] CreateExtraDto dto)
    {
        try
        {
            var userId = GetUserId();
            var userIdString = userId.ToString();

            var extra = new CatalogItemExtra
            {
                UserId = userId,
                Name = dto.Name,
                Description = dto.Description,
                Code = dto.Code,
                BasePrice = dto.BasePrice,
                ImageUrl = dto.ImageUrl,
                DisplayOrder = dto.DisplayOrder,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                CreatedBy = userIdString,
                UpdatedBy = userIdString
            };

            _context.CatalogItemExtras.Add(extra);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetExtra), new { id = extra.Id },
                new { success = true, data = extra });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating extra");
            return StatusCode(500, new { success = false, error = "Failed to create extra" });
        }
    }

    /// <summary>
    /// Update an existing extra
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateExtra(int id, [FromBody] UpdateExtraDto dto)
    {
        try
        {
            var userId = GetUserId();
            var userIdString = userId.ToString();
            var extra = await _context.CatalogItemExtras
                .FirstOrDefaultAsync(e => e.Id == id && e.UserId == userId);

            if (extra == null)
                return NotFound(new { success = false, error = "Extra not found" });

            extra.Name = dto.Name;
            extra.Description = dto.Description;
            extra.Code = dto.Code;
            extra.BasePrice = dto.BasePrice;
            extra.ImageUrl = dto.ImageUrl;
            extra.DisplayOrder = dto.DisplayOrder;
            extra.UpdatedAt = DateTime.UtcNow;
            extra.UpdatedBy = userIdString;

            await _context.SaveChangesAsync();

            return Ok(new { success = true, data = extra });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating extra {Id}", id);
            return StatusCode(500, new { success = false, error = "Failed to update extra" });
        }
    }

    /// <summary>
    /// Delete an extra
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteExtra(int id)
    {
        try
        {
            var userId = GetUserId();
            var userIdString = userId.ToString();
            var extra = await _context.CatalogItemExtras
                .FirstOrDefaultAsync(e => e.Id == id && e.UserId == userId);

            if (extra == null)
                return NotFound(new { success = false, error = "Extra not found" });

            extra.IsActive = false;
            extra.UpdatedAt = DateTime.UtcNow;
            extra.UpdatedBy = userIdString;
            await _context.SaveChangesAsync();

            return Ok(new { success = true, message = "Extra deleted successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting extra {Id}", id);
            return StatusCode(500, new { success = false, error = "Failed to delete extra" });
        }
    }
}

public class CreateExtraDto
{
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public string? Code { get; set; }
    public decimal BasePrice { get; set; }
    public string? ImageUrl { get; set; }
    public int DisplayOrder { get; set; }
}

public class UpdateExtraDto
{
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public string? Code { get; set; }
    public decimal BasePrice { get; set; }
    public string? ImageUrl { get; set; }
    public int DisplayOrder { get; set; }
}
