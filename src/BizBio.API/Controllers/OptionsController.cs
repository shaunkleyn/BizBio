using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BizBio.Infrastructure.Data;
using BizBio.Core.Entities;
using BizBio.Core.DTOs;
using System.Security.Claims;

namespace BizBio.API.Controllers;

[Route("api/v1/library/options")]
[ApiController]
[Authorize]
public class OptionsController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<OptionsController> _logger;

    public OptionsController(ApplicationDbContext context, ILogger<OptionsController> logger)
    {
        _context = context;
        _logger = logger;
    }

    private int GetUserId() => int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

    /// <summary>
    /// Get all options for the current user
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetOptions()
    {
        try
        {
            var userId = GetUserId();
            var options = await _context.CatalogItemOptions
                .Where(o => o.UserId == userId && o.CatalogId == null && o.IsActive)
                .OrderBy(o => o.DisplayOrder)
                .ToListAsync();

            return Ok(new { success = true, data = options });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching options");
            return StatusCode(500, new { success = false, error = "Failed to fetch options" });
        }
    }

    /// <summary>
    /// Get a single option by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetOption(int id)
    {
        try
        {
            var userId = GetUserId();
            var option = await _context.CatalogItemOptions
                .FirstOrDefaultAsync(o => o.Id == id && o.UserId == userId && o.IsActive);

            if (option == null)
                return NotFound(new { success = false, error = "Option not found" });

            return Ok(new { success = true, data = option });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching option {Id}", id);
            return StatusCode(500, new { success = false, error = "Failed to fetch option" });
        }
    }

    /// <summary>
    /// Create a new option
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> CreateOption([FromBody] CreateOptionDto dto)
    {
        try
        {
            var userId = GetUserId();
            var userIdString = userId.ToString();

            var option = new CatalogItemOption
            {
                UserId = userId,
                Name = dto.Name,
                Description = dto.Description,
                PriceModifier = dto.PriceModifier,
                ImageUrl = dto.ImageUrl,
                DisplayOrder = dto.DisplayOrder,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                CreatedBy = userIdString,
                UpdatedBy = userIdString
            };

            _context.CatalogItemOptions.Add(option);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetOption), new { id = option.Id },
                new { success = true, data = option });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating option");
            return StatusCode(500, new { success = false, error = "Failed to create option" });
        }
    }

    /// <summary>
    /// Update an existing option
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateOption(int id, [FromBody] UpdateOptionDto dto)
    {
        try
        {
            var userId = GetUserId();
            var userIdString = userId.ToString();
            var option = await _context.CatalogItemOptions
                .FirstOrDefaultAsync(o => o.Id == id && o.UserId == userId);

            if (option == null)
                return NotFound(new { success = false, error = "Option not found" });

            option.Name = dto.Name;
            option.Description = dto.Description;
            option.PriceModifier = dto.PriceModifier;
            option.ImageUrl = dto.ImageUrl;
            option.DisplayOrder = dto.DisplayOrder;
            option.UpdatedAt = DateTime.UtcNow;
            option.UpdatedBy = userIdString;

            await _context.SaveChangesAsync();

            return Ok(new { success = true, data = option });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating option {Id}", id);
            return StatusCode(500, new { success = false, error = "Failed to update option" });
        }
    }

    /// <summary>
    /// Delete an option (soft delete)
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteOption(int id)
    {
        try
        {
            var userId = GetUserId();
            var userIdString = userId.ToString();
            var option = await _context.CatalogItemOptions
                .FirstOrDefaultAsync(o => o.Id == id && o.UserId == userId);

            if (option == null)
                return NotFound(new { success = false, error = "Option not found" });

            option.IsActive = false;
            option.UpdatedAt = DateTime.UtcNow;
            option.UpdatedBy = userIdString;
            await _context.SaveChangesAsync();

            return Ok(new { success = true, message = "Option deleted successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting option {Id}", id);
            return StatusCode(500, new { success = false, error = "Failed to delete option" });
        }
    }
}
