using Microsoft.EntityFrameworkCore;
using BizBio.Core.Entities;
using BizBio.Core.Enums;
using BizBio.Core.Interfaces;
using BizBio.Infrastructure.Data;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace BizBio.Infrastructure.Repositories;

public class ProductAddOnRepository : IProductAddOnRepository
{
    private readonly ApplicationDbContext _context;
    private readonly ICacheService _cacheService;
    private readonly ILogger<ProductAddOnRepository> _logger;
    private readonly ITelemetryService _telemetry;

    private const string AddOnCacheKeyPrefix = "addon_";
    private const string AddOnSKUCacheKeyPrefix = "addon_sku_";
    private const string AllAddOnsCacheKey = "addons_all";
    private const string AllActiveAddOnsCacheKey = "addons_all_active";
    private const string AddOnProductCacheKeyPrefix = "addons_product_";
    private const string AddOnTypeCacheKeyPrefix = "addons_type_";
    private const string GlobalAddOnsCacheKey = "addons_global";
    private const string PhysicalAddOnsCacheKey = "addons_physical";
    private const string ServiceAddOnsCacheKey = "addons_service";

    public ProductAddOnRepository(
        ApplicationDbContext context,
        ICacheService cacheService,
        ILogger<ProductAddOnRepository> logger,
        ITelemetryService telemetry)
    {
        _context = context;
        _cacheService = cacheService;
        _logger = logger;
        _telemetry = telemetry;
    }

    public async Task<ProductAddOn?> GetByIdAsync(int id)
    {
        var stopwatch = Stopwatch.StartNew();
        var cacheKey = $"{AddOnCacheKeyPrefix}{id}";

        try
        {
            _logger.LogDebug("Fetching add-on by ID: {Id}", id);

            var result = await _cacheService.GetOrSetAsync(cacheKey, async () =>
            {
                var dbStopwatch = Stopwatch.StartNew();
                var addOn = await _context.ProductAddOns
                    .Include(a => a.AddOnType)
                    .Include(a => a.Product)
                    .FirstOrDefaultAsync(a => a.Id == id);
                dbStopwatch.Stop();

                _telemetry.TrackDependency(
                    "Database",
                    "ProductAddOns.GetById",
                    $"SELECT * FROM ProductAddOns WHERE Id = {id}",
                    DateTimeOffset.UtcNow.Subtract(dbStopwatch.Elapsed),
                    dbStopwatch.Elapsed,
                    addOn != null);

                return addOn;
            }, TimeSpan.FromHours(1));

            stopwatch.Stop();
            _logger.LogDebug("Add-on retrieved successfully. ID: {Id}, Duration: {Duration}ms", id, stopwatch.ElapsedMilliseconds);

            return result;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _logger.LogError(ex, "Error fetching add-on by ID: {Id}", id);
            _telemetry.TrackException(ex, new Dictionary<string, string>
            {
                { "Method", "GetByIdAsync" },
                { "AddOnId", id.ToString() }
            });
            throw;
        }
    }

    public async Task<ProductAddOn?> GetBySKUAsync(string sku)
    {
        var cacheKey = $"{AddOnSKUCacheKeyPrefix}{sku}";
        return await _cacheService.GetOrSetAsync(cacheKey, async () =>
        {
            return await _context.ProductAddOns
                .Include(a => a.AddOnType)
                .Include(a => a.Product)
                .FirstOrDefaultAsync(a => a.SKU == sku);
        }, TimeSpan.FromHours(1));
    }

    public async Task<IEnumerable<ProductAddOn>> GetAllAsync()
    {
        return await _cacheService.GetOrSetAsync(AllAddOnsCacheKey, async () =>
        {
            var addOns = await _context.ProductAddOns
                .Include(a => a.AddOnType)
                .Include(a => a.Product)
                .OrderBy(a => a.DisplayOrder)
                .ThenBy(a => a.Name)
                .ToListAsync();

            _logger.LogInformation("Retrieved {Count} add-ons from database", addOns.Count);
            return addOns;
        }, TimeSpan.FromHours(1)) ?? Enumerable.Empty<ProductAddOn>();
    }

    public async Task<IEnumerable<ProductAddOn>> GetAllActiveAsync()
    {
        return await _cacheService.GetOrSetAsync(AllActiveAddOnsCacheKey, async () =>
        {
            return await _context.ProductAddOns
                .Where(a => a.IsActive)
                .Include(a => a.AddOnType)
                .Include(a => a.Product)
                .OrderBy(a => a.DisplayOrder)
                .ThenBy(a => a.Name)
                .ToListAsync();
        }, TimeSpan.FromHours(1)) ?? Enumerable.Empty<ProductAddOn>();
    }

    public async Task<IEnumerable<ProductAddOn>> GetByProductIdAsync(int productId)
    {
        var cacheKey = $"{AddOnProductCacheKeyPrefix}{productId}";
        return await _cacheService.GetOrSetAsync(cacheKey, async () =>
        {
            return await _context.ProductAddOns
                .Where(a => a.IsActive && a.ProductId == productId)
                .Include(a => a.AddOnType)
                .OrderBy(a => a.DisplayOrder)
                .ThenBy(a => a.Name)
                .ToListAsync();
        }, TimeSpan.FromHours(1)) ?? Enumerable.Empty<ProductAddOn>();
    }

    public async Task<IEnumerable<ProductAddOn>> GetByTypeAsync(AddOnType addOnType)
    {
        var cacheKey = $"{AddOnTypeCacheKeyPrefix}{(int)addOnType}";
        return await _cacheService.GetOrSetAsync(cacheKey, async () =>
        {
            return await _context.ProductAddOns
                .Where(a => a.IsActive && a.AddOnTypeId == (int)addOnType)
                .Include(a => a.AddOnType)
                .Include(a => a.Product)
                .OrderBy(a => a.DisplayOrder)
                .ThenBy(a => a.Name)
                .ToListAsync();
        }, TimeSpan.FromHours(1)) ?? Enumerable.Empty<ProductAddOn>();
    }

    public async Task<IEnumerable<ProductAddOn>> GetGlobalAddOnsAsync()
    {
        return await _cacheService.GetOrSetAsync(GlobalAddOnsCacheKey, async () =>
        {
            return await _context.ProductAddOns
                .Where(a => a.IsActive && a.ProductId == null)
                .Include(a => a.AddOnType)
                .OrderBy(a => a.DisplayOrder)
                .ThenBy(a => a.Name)
                .ToListAsync();
        }, TimeSpan.FromHours(1)) ?? Enumerable.Empty<ProductAddOn>();
    }

    public async Task<IEnumerable<ProductAddOn>> GetPhysicalAddOnsAsync()
    {
        return await _cacheService.GetOrSetAsync(PhysicalAddOnsCacheKey, async () =>
        {
            return await _context.ProductAddOns
                .Where(a => a.IsActive && a.RequiresShipping)
                .Include(a => a.AddOnType)
                .Include(a => a.Product)
                .OrderBy(a => a.DisplayOrder)
                .ThenBy(a => a.Name)
                .ToListAsync();
        }, TimeSpan.FromHours(1)) ?? Enumerable.Empty<ProductAddOn>();
    }

    public async Task<IEnumerable<ProductAddOn>> GetServiceAddOnsAsync()
    {
        return await _cacheService.GetOrSetAsync(ServiceAddOnsCacheKey, async () =>
        {
            return await _context.ProductAddOns
                .Where(a => a.IsActive && !a.RequiresShipping && a.IsRecurring)
                .Include(a => a.AddOnType)
                .Include(a => a.Product)
                .OrderBy(a => a.DisplayOrder)
                .ThenBy(a => a.Name)
                .ToListAsync();
        }, TimeSpan.FromHours(1)) ?? Enumerable.Empty<ProductAddOn>();
    }

    public async Task AddAsync(ProductAddOn addOn)
    {
        addOn.CreatedAt = DateTime.UtcNow;
        addOn.UpdatedAt = DateTime.UtcNow;
        await _context.ProductAddOns.AddAsync(addOn);
        InvalidateCache();
    }

    public Task UpdateAsync(ProductAddOn addOn)
    {
        addOn.UpdatedAt = DateTime.UtcNow;
        _context.ProductAddOns.Update(addOn);
        InvalidateCache();
        InvalidateCacheForAddOn(addOn);
        return Task.CompletedTask;
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }

    private void InvalidateCache()
    {
        _cacheService.Remove(AllAddOnsCacheKey);
        _cacheService.Remove(AllActiveAddOnsCacheKey);
        _cacheService.Remove(GlobalAddOnsCacheKey);
        _cacheService.Remove(PhysicalAddOnsCacheKey);
        _cacheService.Remove(ServiceAddOnsCacheKey);
        _cacheService.RemoveByPattern(AddOnProductCacheKeyPrefix);
        _cacheService.RemoveByPattern(AddOnTypeCacheKeyPrefix);
    }

    private void InvalidateCacheForAddOn(ProductAddOn addOn)
    {
        _cacheService.Remove($"{AddOnCacheKeyPrefix}{addOn.Id}");
        _cacheService.Remove($"{AddOnSKUCacheKeyPrefix}{addOn.SKU}");
    }
}
