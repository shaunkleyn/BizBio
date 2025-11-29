using Microsoft.EntityFrameworkCore;
using BizBio.Core.Entities;
using BizBio.Core.Enums;
using BizBio.Core.Interfaces;
using BizBio.Infrastructure.Data;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace BizBio.Infrastructure.Repositories;

public class SubscriptionTierRepository : ISubscriptionTierRepository
{
    private readonly ApplicationDbContext _context;
    private readonly ICacheService _cacheService;
    private readonly ILogger<SubscriptionTierRepository> _logger;
    private readonly ITelemetryService _telemetry;
    private const string TierCacheKeyPrefix = "tier_";
    private const string TierCodeCacheKeyPrefix = "tier_code_";
    private const string AllActiveTiersCacheKey = "tiers_all_active";
    private const string ProductLineTiersCacheKeyPrefix = "tiers_productline_";

    public SubscriptionTierRepository(
        ApplicationDbContext context,
        ICacheService cacheService,
        ILogger<SubscriptionTierRepository> logger,
        ITelemetryService telemetry)
    {
        _context = context;
        _cacheService = cacheService;
        _logger = logger;
        _telemetry = telemetry;
    }

    public async Task<SubscriptionTier?> GetByIdAsync(int id)
    {
        var stopwatch = Stopwatch.StartNew();
        var cacheKey = $"{TierCacheKeyPrefix}{id}";

        try
        {
            _logger.LogDebug("Fetching subscription tier by ID: {Id}", id);

            var result = await _cacheService.GetOrSetAsync(cacheKey, async () =>
            {
                var dbStopwatch = Stopwatch.StartNew();
                var tier = await _context.SubscriptionTiers
                    .Include(t => t.ProductLine)
                    .Include(t => t.Subscriptions)
                    .FirstOrDefaultAsync(t => t.Id == id);
                dbStopwatch.Stop();

                _telemetry.TrackDependency(
                    "Database",
                    "SubscriptionTiers.GetById",
                    $"SELECT * FROM SubscriptionTiers WHERE Id = {id}",
                    DateTimeOffset.UtcNow.Subtract(dbStopwatch.Elapsed),
                    dbStopwatch.Elapsed,
                    tier != null);

                return tier;
            }, TimeSpan.FromHours(1));

            stopwatch.Stop();
            _logger.LogDebug("Subscription tier retrieved successfully. ID: {Id}, Duration: {Duration}ms", id, stopwatch.ElapsedMilliseconds);

            return result;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _logger.LogError(ex, "Error fetching subscription tier by ID: {Id}", id);
            _telemetry.TrackException(ex, new Dictionary<string, string>
            {
                { "Method", "GetByIdAsync" },
                { "TierId", id.ToString() }
            });
            throw;
        }
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
        var stopwatch = Stopwatch.StartNew();

        try
        {
            _logger.LogDebug("Fetching all active subscription tiers");

            var result = await _cacheService.GetOrSetAsync(AllActiveTiersCacheKey, async () =>
            {
                var dbStopwatch = Stopwatch.StartNew();
                var tiers = await _context.SubscriptionTiers
                    .Where(t => t.IsActive)
                    .Include(t => t.ProductLine)
                    .Include(t => t.Subscriptions)
                    .ToListAsync();
                dbStopwatch.Stop();

                _telemetry.TrackDependency(
                    "Database",
                    "SubscriptionTiers.GetAllActive",
                    "SELECT * FROM SubscriptionTiers WHERE IsActive = true",
                    DateTimeOffset.UtcNow.Subtract(dbStopwatch.Elapsed),
                    dbStopwatch.Elapsed,
                    true);

                _logger.LogInformation("Retrieved {Count} active subscription tiers from database", tiers.Count);

                return tiers;
            }, TimeSpan.FromHours(1)) ?? Enumerable.Empty<SubscriptionTier>();

            stopwatch.Stop();
            _logger.LogDebug("GetAllActiveAsync completed. Duration: {Duration}ms, Count: {Count}",
                stopwatch.ElapsedMilliseconds, result.Count());

            return result;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _logger.LogError(ex, "Error fetching all active subscription tiers");
            _telemetry.TrackException(ex, new Dictionary<string, string>
            {
                { "Method", "GetAllActiveAsync" }
            });
            throw;
        }
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