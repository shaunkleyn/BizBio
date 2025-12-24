using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BizBio.Infrastructure.Data;
using BizBio.Core.Entities;
using BizBio.Core.DTOs;
using System.Security.Claims;

namespace BizBio.API.Controllers;

[Route("api/v1/library/option-groups")]
[ApiController]
[Authorize]
public class OptionGroupsController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<OptionGroupsController> _logger;

    public OptionGroupsController(ApplicationDbContext context, ILogger<OptionGroupsController> logger)
    {
        _context = context;
        _logger = logger;
    }

    private int GetUserId() => int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

    /// <summary>
    /// Get all option groups for the current user
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetOptionGroups()
    {
        try
        {
            var userId = GetUserId();
            var groups = await _context.CatalogItemOptionGroups
                .Where(g => g.UserId == userId && g.CatalogId == null && g.IsActive)
                .Include(g => g.GroupItems)
                    .ThenInclude(gi => gi.Option)
                .OrderBy(g => g.DisplayOrder)
                .Select(g => new
                {
                    g.Id,
                    g.Name,
                    g.Description,
                    g.MinRequired,
                    g.MaxAllowed,
                    g.IsRequired,
                    g.DisplayOrder,
                    g.IsActive,
                    g.CreatedAt,
                    g.UpdatedAt,
                    Options = g.GroupItems.Select(gi => new
                    {
                        gi.Id,
                        gi.OptionId,
                        gi.DisplayOrder,
                        gi.IsDefault,
                        Option = new
                        {
                            gi.Option.Id,
                            gi.Option.Name,
                            gi.Option.Description,
                            gi.Option.PriceModifier,
                            gi.Option.ImageUrl
                        }
                    }).OrderBy(o => o.DisplayOrder).ToList()
                })
                .ToListAsync();

            return Ok(new { success = true, data = groups });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching option groups");
            return StatusCode(500, new { success = false, error = "Failed to fetch option groups" });
        }
    }

    /// <summary>
    /// Get a single option group by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetOptionGroup(int id)
    {
        try
        {
            var userId = GetUserId();
            var group = await _context.CatalogItemOptionGroups
                .Where(g => g.Id == id && g.UserId == userId && g.IsActive)
                .Include(g => g.GroupItems)
                    .ThenInclude(gi => gi.Option)
                .Select(g => new
                {
                    g.Id,
                    g.Name,
                    g.Description,
                    g.MinRequired,
                    g.MaxAllowed,
                    g.IsRequired,
                    g.DisplayOrder,
                    g.IsActive,
                    g.CreatedAt,
                    g.UpdatedAt,
                    Options = g.GroupItems.Select(gi => new
                    {
                        gi.Id,
                        gi.OptionId,
                        gi.DisplayOrder,
                        gi.IsDefault,
                        Option = new
                        {
                            gi.Option.Id,
                            gi.Option.Name,
                            gi.Option.Description,
                            gi.Option.PriceModifier,
                            gi.Option.ImageUrl
                        }
                    }).OrderBy(o => o.DisplayOrder).ToList()
                })
                .FirstOrDefaultAsync();

            if (group == null)
                return NotFound(new { success = false, error = "Option group not found" });

            return Ok(new { success = true, data = group });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching option group {Id}", id);
            return StatusCode(500, new { success = false, error = "Failed to fetch option group" });
        }
    }

    /// <summary>
    /// Create a new option group
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> CreateOptionGroup([FromBody] CreateOptionGroupDto dto)
    {
        try
        {
            var userId = GetUserId();
            var userIdString = userId.ToString();

            var group = new CatalogItemOptionGroup
            {
                UserId = userId,
                Name = dto.Name,
                Description = dto.Description,
                MinRequired = dto.MinRequired,
                MaxAllowed = dto.MaxAllowed,
                IsRequired = dto.IsRequired,
                DisplayOrder = dto.DisplayOrder,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                CreatedBy = userIdString,
                UpdatedBy = userIdString
            };

            _context.CatalogItemOptionGroups.Add(group);
            await _context.SaveChangesAsync();

            // Add options to the group
            if (dto.OptionIds != null && dto.OptionIds.Any())
            {
                var displayOrder = 0;
                foreach (var optionId in dto.OptionIds)
                {
                    var groupItem = new CatalogItemOptionGroupItem
                    {
                        OptionGroupId = group.Id,
                        OptionId = optionId,
                        DisplayOrder = displayOrder++,
                        IsDefault = false, // First option could be default, but let user decide via update
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow,
                        CreatedBy = userIdString,
                        UpdatedBy = userIdString
                    };
                    _context.CatalogItemOptionGroupItems.Add(groupItem);
                }
                await _context.SaveChangesAsync();
            }

            // Reload with options
            var createdGroup = await _context.CatalogItemOptionGroups
                .Where(g => g.Id == group.Id)
                .Include(g => g.GroupItems)
                    .ThenInclude(gi => gi.Option)
                .FirstOrDefaultAsync();

            return CreatedAtAction(nameof(GetOptionGroup), new { id = group.Id },
                new { success = true, data = createdGroup });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating option group");
            return StatusCode(500, new { success = false, error = "Failed to create option group" });
        }
    }

    /// <summary>
    /// Update an existing option group
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateOptionGroup(int id, [FromBody] UpdateOptionGroupDto dto)
    {
        try
        {
            var userId = GetUserId();
            var userIdString = userId.ToString();
            var group = await _context.CatalogItemOptionGroups
                .Include(g => g.GroupItems)
                .FirstOrDefaultAsync(g => g.Id == id && g.UserId == userId);

            if (group == null)
                return NotFound(new { success = false, error = "Option group not found" });

            group.Name = dto.Name;
            group.Description = dto.Description;
            group.MinRequired = dto.MinRequired;
            group.MaxAllowed = dto.MaxAllowed;
            group.IsRequired = dto.IsRequired;
            group.DisplayOrder = dto.DisplayOrder;
            group.UpdatedAt = DateTime.UtcNow;
            group.UpdatedBy = userIdString;

            // Update options in the group
            if (dto.OptionIds != null)
            {
                // Remove existing items
                _context.CatalogItemOptionGroupItems.RemoveRange(group.GroupItems);

                // Add new items
                var displayOrder = 0;
                foreach (var optionId in dto.OptionIds)
                {
                    var groupItem = new CatalogItemOptionGroupItem
                    {
                        OptionGroupId = group.Id,
                        OptionId = optionId,
                        DisplayOrder = displayOrder++,
                        IsDefault = false,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow,
                        CreatedBy = userIdString,
                        UpdatedBy = userIdString
                    };
                    _context.CatalogItemOptionGroupItems.Add(groupItem);
                }
            }

            await _context.SaveChangesAsync();

            // Reload with options
            var updatedGroup = await _context.CatalogItemOptionGroups
                .Where(g => g.Id == group.Id)
                .Include(g => g.GroupItems)
                    .ThenInclude(gi => gi.Option)
                .FirstOrDefaultAsync();

            return Ok(new { success = true, data = updatedGroup });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating option group {Id}", id);
            return StatusCode(500, new { success = false, error = "Failed to update option group" });
        }
    }

    /// <summary>
    /// Delete an option group (soft delete)
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteOptionGroup(int id)
    {
        try
        {
            var userId = GetUserId();
            var userIdString = userId.ToString();
            var group = await _context.CatalogItemOptionGroups
                .FirstOrDefaultAsync(g => g.Id == id && g.UserId == userId);

            if (group == null)
                return NotFound(new { success = false, error = "Option group not found" });

            group.IsActive = false;
            group.UpdatedAt = DateTime.UtcNow;
            group.UpdatedBy = userIdString;
            await _context.SaveChangesAsync();

            return Ok(new { success = true, message = "Option group deleted successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting option group {Id}", id);
            return StatusCode(500, new { success = false, error = "Failed to delete option group" });
        }
    }
}
