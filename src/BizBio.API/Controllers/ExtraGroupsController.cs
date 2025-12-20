using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BizBio.Infrastructure.Data;
using BizBio.Core.Entities;
using System.Security.Claims;

namespace BizBio.API.Controllers;

[Route("api/v1/library/extra-groups")]
[ApiController]
[Authorize]
public class ExtraGroupsController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<ExtraGroupsController> _logger;

    public ExtraGroupsController(ApplicationDbContext context, ILogger<ExtraGroupsController> logger)
    {
        _context = context;
        _logger = logger;
    }

    private int GetUserId() => int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

    /// <summary>
    /// Get all extra groups for the current user
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetExtraGroups()
    {
        try
        {
            var userId = GetUserId();
            var groups = await _context.CatalogItemExtraGroups
                .Where(g => g.UserId == userId && g.CatalogId == null && g.IsActive)
                .Include(g => g.GroupItems)
                    .ThenInclude(gi => gi.Extra)
                .OrderBy(g => g.Id)
                .Select(g => new
                {
                    g.Id,
                    g.Name,
                    g.Description,
                    g.MinRequired,
                    g.MaxAllowed,
                    g.AllowMultipleQuantities,
                    g.DisplayOrder,
                    g.IsActive,
                    g.CreatedAt,
                    g.UpdatedAt,
                    Extras = g.GroupItems.Select(gi => new
                    {
                        gi.Id,
                        gi.ExtraId,
                        gi.DisplayOrder,
                        gi.PriceOverride,
                        Extra = new
                        {
                            gi.Extra.Id,
                            gi.Extra.Name,
                            gi.Extra.Description,
                            gi.Extra.Code,
                            gi.Extra.BasePrice,
                            gi.Extra.ImageUrl
                        }
                    }).OrderBy(e => e.DisplayOrder).ToList()
                })
                .ToListAsync();

            return Ok(new { success = true, data = groups });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching extra groups");
            return StatusCode(500, new { success = false, error = "Failed to fetch extra groups" });
        }
    }

    /// <summary>
    /// Get a single extra group by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetExtraGroup(int id)
    {
        try
        {
            var userId = GetUserId();
            var group = await _context.CatalogItemExtraGroups
                .Where(g => g.Id == id && g.UserId == userId && g.IsActive)
                .Include(g => g.GroupItems)
                    .ThenInclude(gi => gi.Extra)
                .Select(g => new
                {
                    g.Id,
                    g.Name,
                    g.Description,
                    g.MinRequired,
                    g.MaxAllowed,
                    g.AllowMultipleQuantities,
                    g.DisplayOrder,
                    g.IsActive,
                    g.CreatedAt,
                    g.UpdatedAt,
                    Extras = g.GroupItems.Select(gi => new
                    {
                        gi.Id,
                        gi.ExtraId,
                        gi.DisplayOrder,
                        gi.PriceOverride,
                        Extra = new
                        {
                            gi.Extra.Id,
                            gi.Extra.Name,
                            gi.Extra.Description,
                            gi.Extra.Code,
                            gi.Extra.BasePrice,
                            gi.Extra.ImageUrl
                        }
                    }).OrderBy(e => e.DisplayOrder).ToList()
                })
                .FirstOrDefaultAsync();

            if (group == null)
                return NotFound(new { success = false, error = "Extra group not found" });

            return Ok(new { success = true, data = group });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching extra group {Id}", id);
            return StatusCode(500, new { success = false, error = "Failed to fetch extra group" });
        }
    }

    /// <summary>
    /// Create a new extra group
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> CreateExtraGroup([FromBody] CreateExtraGroupDto dto)
    {
        try
        {
            var userId = GetUserId();
            var userIdString = userId.ToString();

            var group = new CatalogItemExtraGroup
            {
                UserId = userId,
                Name = dto.Name,
                Description = dto.Description,
                MinRequired = dto.MinRequired,
                MaxAllowed = dto.MaxAllowed,
                AllowMultipleQuantities = dto.AllowMultipleQuantities,
                DisplayOrder = dto.DisplayOrder,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                CreatedBy = userIdString,
                UpdatedBy = userIdString
            };

            _context.CatalogItemExtraGroups.Add(group);
            await _context.SaveChangesAsync();

            // Add extras to the group
            if (dto.ExtraIds != null && dto.ExtraIds.Any())
            {
                var displayOrder = 0;
                foreach (var extraId in dto.ExtraIds)
                {
                    var groupItem = new CatalogItemExtraGroupItem
                    {
                        ExtraGroupId = group.Id,
                        ExtraId = extraId,
                        DisplayOrder = displayOrder++,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow,
                        CreatedBy = userIdString,
                        UpdatedBy = userIdString
                    };
                    _context.CatalogItemExtraGroupItems.Add(groupItem);
                }
                await _context.SaveChangesAsync();
            }

            // Reload with extras
            var createdGroup = await _context.CatalogItemExtraGroups
                .Where(g => g.Id == group.Id)
                .Include(g => g.GroupItems)
                    .ThenInclude(gi => gi.Extra)
                .FirstOrDefaultAsync();

            return CreatedAtAction(nameof(GetExtraGroup), new { id = group.Id },
                new { success = true, data = createdGroup });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating extra group");
            return StatusCode(500, new { success = false, error = "Failed to create extra group" });
        }
    }

    /// <summary>
    /// Update an existing extra group
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateExtraGroup(int id, [FromBody] UpdateExtraGroupDto dto)
    {
        try
        {
            var userId = GetUserId();
            var userIdString = userId.ToString();
            var group = await _context.CatalogItemExtraGroups
                .Include(g => g.GroupItems)
                .FirstOrDefaultAsync(g => g.Id == id && g.UserId == userId);

            if (group == null)
                return NotFound(new { success = false, error = "Extra group not found" });

            group.Name = dto.Name;
            group.Description = dto.Description;
            group.MinRequired = dto.MinRequired;
            group.MaxAllowed = dto.MaxAllowed;
            group.AllowMultipleQuantities = dto.AllowMultipleQuantities;
            group.DisplayOrder = dto.DisplayOrder;
            group.UpdatedAt = DateTime.UtcNow;
            group.UpdatedBy = userIdString;

            // Update extras in the group
            if (dto.ExtraIds != null)
            {
                // Remove existing items
                _context.CatalogItemExtraGroupItems.RemoveRange(group.GroupItems);

                // Add new items
                var displayOrder = 0;
                foreach (var extraId in dto.ExtraIds)
                {
                    var groupItem = new CatalogItemExtraGroupItem
                    {
                        ExtraGroupId = group.Id,
                        ExtraId = extraId,
                        DisplayOrder = displayOrder++,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow,
                        CreatedBy = userIdString,
                        UpdatedBy = userIdString
                    };
                    _context.CatalogItemExtraGroupItems.Add(groupItem);
                }
            }

            await _context.SaveChangesAsync();

            // Reload with extras
            var updatedGroup = await _context.CatalogItemExtraGroups
                .Where(g => g.Id == group.Id)
                .Include(g => g.GroupItems)
                    .ThenInclude(gi => gi.Extra)
                .FirstOrDefaultAsync();

            return Ok(new { success = true, data = updatedGroup });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating extra group {Id}", id);
            return StatusCode(500, new { success = false, error = "Failed to update extra group" });
        }
    }

    /// <summary>
    /// Delete an extra group
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteExtraGroup(int id)
    {
        try
        {
            var userId = GetUserId();
            var userIdString = userId.ToString();
            var group = await _context.CatalogItemExtraGroups
                .FirstOrDefaultAsync(g => g.Id == id && g.UserId == userId);

            if (group == null)
                return NotFound(new { success = false, error = "Extra group not found" });

            group.IsActive = false;
            group.UpdatedAt = DateTime.UtcNow;
            group.UpdatedBy = userIdString;
            await _context.SaveChangesAsync();

            return Ok(new { success = true, message = "Extra group deleted successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting extra group {Id}", id);
            return StatusCode(500, new { success = false, error = "Failed to delete extra group" });
        }
    }
}

public class CreateExtraGroupDto
{
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public int MinRequired { get; set; }
    public int MaxAllowed { get; set; }
    public bool AllowMultipleQuantities { get; set; } = true;
    public int DisplayOrder { get; set; }
    public List<int>? ExtraIds { get; set; }
}

public class UpdateExtraGroupDto
{
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public int MinRequired { get; set; }
    public int MaxAllowed { get; set; }
    public bool AllowMultipleQuantities { get; set; } = true;
    public int DisplayOrder { get; set; }
    public List<int>? ExtraIds { get; set; }
}
