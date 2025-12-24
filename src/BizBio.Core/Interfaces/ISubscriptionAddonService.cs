using BizBio.Core.Entities;

namespace BizBio.Core.Interfaces;

/// <summary>
/// Service for managing subscription addons
/// </summary>
public interface ISubscriptionAddonService
{
    /// <summary>
    /// Get all addons available for a specific tier
    /// </summary>
    Task<List<SubscriptionAddon>> GetAvailableAddonsForTierAsync(int tierId);

    /// <summary>
    /// Get all active addons for a user's subscription
    /// </summary>
    Task<List<UserSubscriptionAddon>> GetUserActiveAddonsAsync(int userSubscriptionId);

    /// <summary>
    /// Activate an addon for a user's subscription
    /// </summary>
    /// <returns>UserSubscriptionAddon if successful, null if validation fails</returns>
    Task<UserSubscriptionAddon?> ActivateAddonAsync(int userSubscriptionId, int addonId, int userId);

    /// <summary>
    /// Deactivate an addon for a user's subscription
    /// </summary>
    Task<bool> DeactivateAddonAsync(int userSubscriptionAddonId, int userId);

    /// <summary>
    /// Calculate total monthly cost including base subscription and active addons
    /// </summary>
    Task<decimal> CalculateSubscriptionTotalAsync(int userSubscriptionId);

    /// <summary>
    /// Check if a user can activate a specific addon (tier eligibility check)
    /// </summary>
    Task<bool> CanUserActivateAddonAsync(int userSubscriptionId, int addonId);

    /// <summary>
    /// Get all addons for a product line
    /// </summary>
    Task<List<SubscriptionAddon>> GetAddonsByProductLineAsync(int productLineId);

    /// <summary>
    /// Get addon pricing for a specific tier (with tier discount applied)
    /// </summary>
    Task<decimal> GetAddonPriceForTierAsync(int addonId, int tierId);
}
