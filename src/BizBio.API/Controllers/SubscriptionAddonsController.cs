using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using BizBio.Core.Interfaces;

namespace BizBio.API.Controllers;

/// <summary>
/// Controller for managing subscription addons
/// </summary>
[Route("api/v1/subscription-addons")]
[ApiController]
[Authorize]
public class SubscriptionAddonsController : ControllerBase
{
    private readonly ISubscriptionAddonService _addonService;
    private readonly ILogger<SubscriptionAddonsController> _logger;

    public SubscriptionAddonsController(
        ISubscriptionAddonService addonService,
        ILogger<SubscriptionAddonsController> logger)
    {
        _addonService = addonService;
        _logger = logger;
    }

    private int GetUserId() => int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

    /// <summary>
    /// Get all addons available for a specific tier
    /// </summary>
    [HttpGet("tier/{tierId}")]
    public async Task<IActionResult> GetAvailableAddons(int tierId)
    {
        try
        {
            var addons = await _addonService.GetAvailableAddonsForTierAsync(tierId);

            return Ok(new
            {
                success = true,
                data = addons.Select(a => new
                {
                    a.Id,
                    a.Name,
                    a.Description,
                    a.MonthlyPrice,
                    a.ProductLineId,
                    a.SortOrder
                })
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting available addons for tier {TierId}", tierId);
            return StatusCode(500, new { success = false, error = "An error occurred while fetching addons" });
        }
    }

    /// <summary>
    /// Get all addons for a product line
    /// </summary>
    [HttpGet("product-line/{productLineId}")]
    public async Task<IActionResult> GetAddonsByProductLine(int productLineId)
    {
        try
        {
            var addons = await _addonService.GetAddonsByProductLineAsync(productLineId);

            return Ok(new
            {
                success = true,
                data = addons.Select(a => new
                {
                    a.Id,
                    a.Name,
                    a.Description,
                    a.MonthlyPrice,
                    a.SortOrder
                })
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting addons for product line {ProductLineId}", productLineId);
            return StatusCode(500, new { success = false, error = "An error occurred while fetching addons" });
        }
    }

    /// <summary>
    /// Get user's active addons for a subscription
    /// </summary>
    [HttpGet("subscription/{userSubscriptionId}")]
    public async Task<IActionResult> GetUserActiveAddons(int userSubscriptionId)
    {
        try
        {
            var addons = await _addonService.GetUserActiveAddonsAsync(userSubscriptionId);

            return Ok(new
            {
                success = true,
                data = addons.Select(ua => new
                {
                    ua.Id,
                    ua.AddonId,
                    AddonName = ua.Addon.Name,
                    AddonDescription = ua.Addon.Description,
                    ua.MonthlyPrice,
                    ua.AddedAt,
                    ua.IsActiveAddon
                })
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting active addons for subscription {SubscriptionId}", userSubscriptionId);
            return StatusCode(500, new { success = false, error = "An error occurred while fetching addons" });
        }
    }

    /// <summary>
    /// Activate an addon for a subscription
    /// </summary>
    [HttpPost("activate")]
    public async Task<IActionResult> ActivateAddon([FromBody] ActivateAddonDto dto)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = GetUserId();

            var userAddon = await _addonService.ActivateAddonAsync(dto.UserSubscriptionId, dto.AddonId, userId);

            if (userAddon == null)
            {
                return BadRequest(new
                {
                    success = false,
                    error = "Unable to activate addon. It may not be available for your tier, or you may already have it active."
                });
            }

            // Calculate new total
            var newTotal = await _addonService.CalculateSubscriptionTotalAsync(dto.UserSubscriptionId);

            return Ok(new
            {
                success = true,
                message = "Addon activated successfully",
                data = new
                {
                    userAddon.Id,
                    userAddon.AddonId,
                    AddonName = userAddon.Addon.Name,
                    userAddon.MonthlyPrice,
                    userAddon.AddedAt,
                    NewMonthlyTotal = newTotal
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error activating addon");
            return StatusCode(500, new { success = false, error = "An error occurred while activating addon" });
        }
    }

    /// <summary>
    /// Deactivate an addon
    /// </summary>
    [HttpDelete("{userSubscriptionAddonId}")]
    public async Task<IActionResult> DeactivateAddon(int userSubscriptionAddonId)
    {
        try
        {
            var userId = GetUserId();

            var success = await _addonService.DeactivateAddonAsync(userSubscriptionAddonId, userId);

            if (!success)
            {
                return NotFound(new
                {
                    success = false,
                    error = "Addon not found or already deactivated"
                });
            }

            return Ok(new
            {
                success = true,
                message = "Addon deactivated successfully"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deactivating addon {AddonId}", userSubscriptionAddonId);
            return StatusCode(500, new { success = false, error = "An error occurred while deactivating addon" });
        }
    }

    /// <summary>
    /// Get pricing for an addon at a specific tier (with tier discount applied)
    /// </summary>
    [HttpGet("pricing")]
    public async Task<IActionResult> GetAddonPricing([FromQuery] int addonId, [FromQuery] int tierId)
    {
        try
        {
            var price = await _addonService.GetAddonPriceForTierAsync(addonId, tierId);

            return Ok(new
            {
                success = true,
                data = new
                {
                    AddonId = addonId,
                    TierId = tierId,
                    MonthlyPrice = price
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting addon pricing for addon {AddonId} tier {TierId}", addonId, tierId);
            return StatusCode(500, new { success = false, error = "An error occurred while fetching pricing" });
        }
    }

    /// <summary>
    /// Calculate total monthly cost for a subscription including addons
    /// </summary>
    [HttpGet("subscription/{userSubscriptionId}/total")]
    public async Task<IActionResult> GetSubscriptionTotal(int userSubscriptionId)
    {
        try
        {
            var total = await _addonService.CalculateSubscriptionTotalAsync(userSubscriptionId);

            return Ok(new
            {
                success = true,
                data = new
                {
                    UserSubscriptionId = userSubscriptionId,
                    MonthlyTotal = total
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calculating subscription total for {SubscriptionId}", userSubscriptionId);
            return StatusCode(500, new { success = false, error = "An error occurred while calculating total" });
        }
    }

    /// <summary>
    /// Check if a user can activate a specific addon
    /// </summary>
    [HttpGet("can-activate")]
    public async Task<IActionResult> CanActivateAddon([FromQuery] int userSubscriptionId, [FromQuery] int addonId)
    {
        try
        {
            var canActivate = await _addonService.CanUserActivateAddonAsync(userSubscriptionId, addonId);

            return Ok(new
            {
                success = true,
                data = new
                {
                    UserSubscriptionId = userSubscriptionId,
                    AddonId = addonId,
                    CanActivate = canActivate
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking addon eligibility");
            return StatusCode(500, new { success = false, error = "An error occurred" });
        }
    }
}

public class ActivateAddonDto
{
    public int UserSubscriptionId { get; set; }
    public int AddonId { get; set; }
}
