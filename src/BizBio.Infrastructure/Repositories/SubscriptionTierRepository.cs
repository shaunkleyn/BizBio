using Microsoft.EntityFrameworkCore;
using BizBio.Core.Entities;
using BizBio.Core.Enums;
using BizBio.Core.Interfaces;
using BizBio.Infrastructure.Data;

namespace BizBio.Infrastructure.Repositories;

public class SubscriptionTierRepository : ISubscriptionTierRepository
{
    private readonly ApplicationDbContext _context;
    private readonly ICacheService _cacheService;
    private const string TierCacheKeyPrefix = "tier_";
    private const string TierCodeCacheKeyPrefix = "tier_code_";
    private const string AllActiveTiersCacheKey = "tiers_all_active";
    private const string ProductLineTiersCacheKeyPrefix = "tiers_productline_";

    public SubscriptionTierRepository(ApplicationDbContext context, ICacheService cacheService)
    {
        _context = context;
        _cacheService = cacheService;
    }

    public async Task<SubscriptionTier?> GetByIdAsync(int id)
    {
        var cacheKey = $"{TierCacheKeyPrefix}{id}";
        return await _cacheService.GetOrSetAsync(cacheKey, async () =>
        {
            return await _context.SubscriptionTiers
                .Include(t => t.ProductLine)
                .Include(t => t.Subscriptions)
                .FirstOrDefaultAsync(t => t.Id == id);
        }, TimeSpan.FromHours(1));
    }

    public async Task<SubscriptionTier?> GetByCodeAsync(string tierCode)
    {
        var cacheKey = $"{TierCodeCacheKeyPrefix}{tierCode}";
        return await _cacheService.GetOrSetAsync(cacheKey, async () =>
        {
            return await _context.SubscriptionTiers
                .Include(t => t.ProductLine)
                .Include(t => t.Subscriptions)
                .FirstOrDefaultAsync(t => t.TierCode == tierCode);
        }, TimeSpan.FromHours(1));
    }

    public async Task<IEnumerable<SubscriptionTier>> GetAllActiveAsync()
    {
        return await _cacheService.GetOrSetAsync(AllActiveTiersCacheKey, async () =>
        {
            return await _context.SubscriptionTiers
                .Where(t => t.IsActive)
                .Include(t => t.ProductLine)
                .Include(t => t.Subscriptions)
                .ToListAsync();
        }, TimeSpan.FromHours(1)) ?? Enumerable.Empty<SubscriptionTier>();
    }

    public async Task<IEnumerable<SubscriptionTier>> GetByProductLineAsync(ProductLine productLine)
    {
        var cacheKey = $"{ProductLineTiersCacheKeyPrefix}{productLine}";
        return await _cacheService.GetOrSetAsync(cacheKey, async () =>
        {
            // Get the product line lookup ID
            var productLineId = (await _context.ProductLines
                .FirstOrDefaultAsync(p => p.Name == productLine.ToString()))?.Id;

            if (productLineId == null)
                return new List<SubscriptionTier>();

            return await _context.SubscriptionTiers
                .Where(t => t.ProductLineId == productLineId && t.IsActive)
                .Include(t => t.ProductLine)
                .Include(t => t.Subscriptions)
                .ToListAsync();
        }, TimeSpan.FromHours(1)) ?? Enumerable.Empty<SubscriptionTier>();
    }

    public async Task AddAsync(SubscriptionTier tier)
    {
        await _context.SubscriptionTiers.AddAsync(tier);
        // Invalidate cache when adding
        _cacheService.Remove(AllActiveTiersCacheKey);
        _cacheService.RemoveByPattern(ProductLineTiersCacheKeyPrefix);
    }

    public Task UpdateAsync(SubscriptionTier tier)
    {
        _context.SubscriptionTiers.Update(tier);
        // Invalidate cache when updating
        _cacheService.Remove($"{TierCacheKeyPrefix}{tier.Id}");
        _cacheService.Remove($"{TierCodeCacheKeyPrefix}{tier.TierCode}");
        _cacheService.Remove(AllActiveTiersCacheKey);
        _cacheService.RemoveByPattern(ProductLineTiersCacheKeyPrefix);
        return Task.CompletedTask;
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }
}