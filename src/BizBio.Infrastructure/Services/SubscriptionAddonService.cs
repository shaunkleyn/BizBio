using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using BizBio.Core.Entities;
using BizBio.Core.Interfaces;
using BizBio.Infrastructure.Data;

namespace BizBio.Infrastructure.Services;

public class SubscriptionAddonService : ISubscriptionAddonService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<SubscriptionAddonService> _logger;

    public SubscriptionAddonService(
        ApplicationDbContext context,
        ILogger<SubscriptionAddonService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<List<SubscriptionAddon>> GetAvailableAddonsForTierAsync(int tierId)
    {
        try
        {
            _logger.LogDebug("Getting available addons for tier {TierId}", tierId);

            var addons = await _context.SubscriptionTierAddons
                .Where(ta => ta.SubscriptionTierId == tierId && ta.IsActive)
                .Include(ta => ta.Addon)
                .Where(ta => ta.Addon.IsActive)
                .Select(ta => ta.Addon)
                .OrderBy(a => a.SortOrder)
                .ToListAsync();

            _logger.LogDebug("Found {Count} available addons for tier {TierId}", addons.Count, tierId);
            return addons;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting available addons for tier {TierId}", tierId);
            throw;
        }
    }

    public async Task<List<UserSubscriptionAddon>> GetUserActiveAddonsAsync(int userSubscriptionId)
    {
        try
        {
            _logger.LogDebug("Getting active addons for subscription {SubscriptionId}", userSubscriptionId);

            var addons = await _context.UserSubscriptionAddons
                .Where(usa => usa.UserSubscriptionId == userSubscriptionId && usa.IsActiveAddon && usa.IsActive)
                .Include(usa => usa.Addon)
                .OrderBy(usa => usa.Addon.SortOrder)
                .ToListAsync();

            _logger.LogDebug("Found {Count} active addons for subscription {SubscriptionId}", addons.Count, userSubscriptionId);
            return addons;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting active addons for subscription {SubscriptionId}", userSubscriptionId);
            throw;
        }
    }

    public async Task<UserSubscriptionAddon?> ActivateAddonAsync(int userSubscriptionId, int addonId, int userId)
    {
        try
        {
            _logger.LogInformation("User {UserId} activating addon {AddonId} for subscription {SubscriptionId}", 
                userId, addonId, userSubscriptionId);

            // 1. Verify subscription exists and belongs to user
            var subscription = await _context.UserSubscriptions
                .Include(s => s.Tier)
                .FirstOrDefaultAsync(s => s.Id == userSubscriptionId && s.UserId == userId);

            if (subscription == null)
            {
                _logger.LogWarning("Subscription {SubscriptionId} not found for user {UserId}", userSubscriptionId, userId);
                return null;
            }

            // 2. Check if addon is available for this tier
            var canActivate = await CanUserActivateAddonAsync(userSubscriptionId, addonId);
            if (!canActivate)
            {
                _logger.LogWarning("Addon {AddonId} not available for tier {TierId}", addonId, subscription.TierId);
                return null;
            }

            // 3. Check if user already has this addon active
            var existingAddon = await _context.UserSubscriptionAddons
                .FirstOrDefaultAsync(usa => usa.UserSubscriptionId == userSubscriptionId 
                    && usa.AddonId == addonId 
                    && usa.IsActiveAddon);

            if (existingAddon != null)
            {
                _logger.LogWarning("User already has addon {AddonId} active", addonId);
                return null;
            }

            // 4. Get addon pricing (with tier discount if applicable)
            var price = await GetAddonPriceForTierAsync(addonId, subscription.TierId);

            // 5. Create UserSubscriptionAddon record
            var userAddon = new UserSubscriptionAddon
            {
                UserSubscriptionId = userSubscriptionId,
                AddonId = addonId,
                AddedAt = DateTime.UtcNow,
                IsActiveAddon = true,
                MonthlyPrice = price,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                CreatedBy = userId.ToString(),
                UpdatedBy = userId.ToString()
            };

            _context.UserSubscriptionAddons.Add(userAddon);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Successfully activated addon {AddonId} for subscription {SubscriptionId}", 
                addonId, userSubscriptionId);

            // Reload with navigation properties
            await _context.Entry(userAddon).Reference(ua => ua.Addon).LoadAsync();
            return userAddon;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error activating addon {AddonId} for subscription {SubscriptionId}", 
                addonId, userSubscriptionId);
            throw;
        }
    }

    public async Task<bool> DeactivateAddonAsync(int userSubscriptionAddonId, int userId)
    {
        try
        {
            _logger.LogInformation("User {UserId} deactivating addon {AddonId}", userId, userSubscriptionAddonId);

            var userAddon = await _context.UserSubscriptionAddons
                .Include(usa => usa.UserSubscription)
                .FirstOrDefaultAsync(usa => usa.Id == userSubscriptionAddonId 
                    && usa.UserSubscription.UserId == userId
                    && usa.IsActiveAddon);

            if (userAddon == null)
            {
                _logger.LogWarning("UserSubscriptionAddon {Id} not found for user {UserId}", userSubscriptionAddonId, userId);
                return false;
            }

            userAddon.IsActiveAddon = false;
            userAddon.CancelledAt = DateTime.UtcNow;
            userAddon.UpdatedAt = DateTime.UtcNow;
            userAddon.UpdatedBy = userId.ToString();

            await _context.SaveChangesAsync();

            _logger.LogInformation("Successfully deactivated addon {AddonId}", userSubscriptionAddonId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deactivating addon {AddonId}", userSubscriptionAddonId);
            throw;
        }
    }

    public async Task<decimal> CalculateSubscriptionTotalAsync(int userSubscriptionId)
    {
        try
        {
            _logger.LogDebug("Calculating total for subscription {SubscriptionId}", userSubscriptionId);

            var subscription = await _context.UserSubscriptions
                .Include(s => s.Tier)
                .Include(s => s.Addons.Where(a => a.IsActiveAddon && a.IsActive))
                .FirstOrDefaultAsync(s => s.Id == userSubscriptionId);

            if (subscription == null)
            {
                _logger.LogWarning("Subscription {SubscriptionId} not found", userSubscriptionId);
                return 0;
            }

            var basePrice = subscription.Tier.MonthlyPrice;
            var addonsTotal = subscription.Addons.Sum(a => a.MonthlyPrice);
            var total = basePrice + addonsTotal;

            _logger.LogDebug("Subscription {SubscriptionId} total: Base={BasePrice}, Addons={AddonsTotal}, Total={Total}", 
                userSubscriptionId, basePrice, addonsTotal, total);

            return total;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calculating total for subscription {SubscriptionId}", userSubscriptionId);
            throw;
        }
    }

    public async Task<bool> CanUserActivateAddonAsync(int userSubscriptionId, int addonId)
    {
        try
        {
            _logger.LogDebug("Checking if subscription {SubscriptionId} can activate addon {AddonId}", 
                userSubscriptionId, addonId);

            var subscription = await _context.UserSubscriptions
                .FirstOrDefaultAsync(s => s.Id == userSubscriptionId);

            if (subscription == null)
            {
                return false;
            }

            // Check if addon is available for this tier
            var tierAddon = await _context.SubscriptionTierAddons
                .FirstOrDefaultAsync(ta => ta.SubscriptionTierId == subscription.TierId 
                    && ta.AddonId == addonId 
                    && ta.IsActive);

            return tierAddon != null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking addon eligibility for subscription {SubscriptionId}", userSubscriptionId);
            throw;
        }
    }

    public async Task<List<SubscriptionAddon>> GetAddonsByProductLineAsync(int productLineId)
    {
        try
        {
            _logger.LogDebug("Getting addons for product line {ProductLineId}", productLineId);

            var addons = await _context.SubscriptionAddons
                .Where(a => a.ProductLineId == productLineId && a.IsActive)
                .OrderBy(a => a.SortOrder)
                .ToListAsync();

            _logger.LogDebug("Found {Count} addons for product line {ProductLineId}", addons.Count, productLineId);
            return addons;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting addons for product line {ProductLineId}", productLineId);
            throw;
        }
    }

    public async Task<decimal> GetAddonPriceForTierAsync(int addonId, int tierId)
    {
        try
        {
            _logger.LogDebug("Getting price for addon {AddonId} at tier {TierId}", addonId, tierId);

            // Get the addon base price
            var addon = await _context.SubscriptionAddons
                .FirstOrDefaultAsync(a => a.Id == addonId);

            if (addon == null)
            {
                _logger.LogWarning("Addon {AddonId} not found", addonId);
                return 0;
            }

            var basePrice = addon.MonthlyPrice;

            // Check if there's a tier-specific discount
            var tierAddon = await _context.SubscriptionTierAddons
                .FirstOrDefaultAsync(ta => ta.SubscriptionTierId == tierId && ta.AddonId == addonId);

            if (tierAddon?.DiscountPercentage > 0)
            {
                var discount = basePrice * (tierAddon.DiscountPercentage.Value / 100);
                var discountedPrice = basePrice - discount;
                _logger.LogDebug("Tier {TierId} gets {Discount}% discount on addon {AddonId}: {BasePrice} -> {FinalPrice}", 
                    tierId, tierAddon.DiscountPercentage, addonId, basePrice, discountedPrice);
                return discountedPrice;
            }

            return basePrice;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting price for addon {AddonId} at tier {TierId}", addonId, tierId);
            throw;
        }
    }
}
