using Microsoft.EntityFrameworkCore;
using BizBio.Core.Entities;
using BizBio.Core.Enums;
using BizBio.Core.Interfaces;
using BizBio.Infrastructure.Data;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace BizBio.Infrastructure.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly ApplicationDbContext _context;
    private readonly ICacheService _cacheService;
    private readonly ILogger<ProductRepository> _logger;
    private readonly ITelemetryService _telemetry;

    private const string ProductCacheKeyPrefix = "product_";
    private const string ProductSKUCacheKeyPrefix = "product_sku_";
    private const string AllProductsCacheKey = "products_all";
    private const string AllActiveProductsCacheKey = "products_all_active";
    private const string ProductTypeCacheKeyPrefix = "products_type_";
    private const string ProductCategoryCacheKeyPrefix = "products_category_";
    private const string ProductLineCacheKeyPrefix = "products_line_";
    private const string FeaturedProductsCacheKey = "products_featured";
    private const string PhysicalProductsCacheKey = "products_physical";
    private const string SubscriptionProductsCacheKey = "products_subscription";

    public ProductRepository(
        ApplicationDbContext context,
        ICacheService cacheService,
        ILogger<ProductRepository> logger,
        ITelemetryService telemetry)
    {
        _context = context;
        _cacheService = cacheService;
        _logger = logger;
        _telemetry = telemetry;
    }

    public async Task<Product?> GetByIdAsync(int id)
    {
        var stopwatch = Stopwatch.StartNew();
        var cacheKey = $"{ProductCacheKeyPrefix}{id}";

        try
        {
            _logger.LogDebug("Fetching product by ID: {Id}", id);

            var result = await _cacheService.GetOrSetAsync(cacheKey, async () =>
            {
                var dbStopwatch = Stopwatch.StartNew();
                var product = await _context.Products
                    .Include(p => p.ProductType)
                    .Include(p => p.ProductCategory)
                    .Include(p => p.ProductLine)
                    .Include(p => p.AddOns)
                    .FirstOrDefaultAsync(p => p.Id == id);
                dbStopwatch.Stop();

                _telemetry.TrackDependency(
                    "Database",
                    "Products.GetById",
                    $"SELECT * FROM Products WHERE Id = {id}",
                    DateTimeOffset.UtcNow.Subtract(dbStopwatch.Elapsed),
                    dbStopwatch.Elapsed,
                    product != null);

                return product;
            }, TimeSpan.FromHours(1));

            stopwatch.Stop();
            _logger.LogDebug("Product retrieved successfully. ID: {Id}, Duration: {Duration}ms", id, stopwatch.ElapsedMilliseconds);

            return result;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _logger.LogError(ex, "Error fetching product by ID: {Id}", id);
            _telemetry.TrackException(ex, new Dictionary<string, string>
            {
                { "Method", "GetByIdAsync" },
                { "ProductId", id.ToString() }
            });
            throw;
        }
    }

    public async Task<Product?> GetBySKUAsync(string sku)
    {
        var stopwatch = Stopwatch.StartNew();
        var cacheKey = $"{ProductSKUCacheKeyPrefix}{sku}";

        try
        {
            _logger.LogDebug("Fetching product by SKU: {SKU}", sku);

            var result = await _cacheService.GetOrSetAsync(cacheKey, async () =>
            {
                var dbStopwatch = Stopwatch.StartNew();
                var product = await _context.Products
                    .Include(p => p.ProductType)
                    .Include(p => p.ProductCategory)
                    .Include(p => p.ProductLine)
                    .Include(p => p.AddOns)
                    .FirstOrDefaultAsync(p => p.SKU == sku);
                dbStopwatch.Stop();

                _telemetry.TrackDependency(
                    "Database",
                    "Products.GetBySKU",
                    $"SELECT * FROM Products WHERE SKU = {sku}",
                    DateTimeOffset.UtcNow.Subtract(dbStopwatch.Elapsed),
                    dbStopwatch.Elapsed,
                    product != null);

                return product;
            }, TimeSpan.FromHours(1));

            stopwatch.Stop();
            return result;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _logger.LogError(ex, "Error fetching product by SKU: {SKU}", sku);
            throw;
        }
    }

    public async Task<IEnumerable<Product>> GetAllAsync()
    {
        return await _cacheService.GetOrSetAsync(AllProductsCacheKey, async () =>
        {
            var dbStopwatch = Stopwatch.StartNew();
            var products = await _context.Products
                .Include(p => p.ProductType)
                .Include(p => p.ProductCategory)
                .Include(p => p.ProductLine)
                .OrderBy(p => p.DisplayOrder)
                .ThenBy(p => p.Name)
                .ToListAsync();
            dbStopwatch.Stop();

            _telemetry.TrackDependency(
                "Database",
                "Products.GetAll",
                "SELECT * FROM Products",
                DateTimeOffset.UtcNow.Subtract(dbStopwatch.Elapsed),
                dbStopwatch.Elapsed,
                true);

            _logger.LogInformation("Retrieved {Count} products from database", products.Count);
            return products;
        }, TimeSpan.FromHours(1)) ?? Enumerable.Empty<Product>();
    }

    public async Task<IEnumerable<Product>> GetAllActiveAsync()
    {
        return await _cacheService.GetOrSetAsync(AllActiveProductsCacheKey, async () =>
        {
            var products = await _context.Products
                .Where(p => p.IsActive)
                .Include(p => p.ProductType)
                .Include(p => p.ProductCategory)
                .Include(p => p.ProductLine)
                .OrderBy(p => p.DisplayOrder)
                .ThenBy(p => p.Name)
                .ToListAsync();

            _logger.LogInformation("Retrieved {Count} active products from database", products.Count);
            return products;
        }, TimeSpan.FromHours(1)) ?? Enumerable.Empty<Product>();
    }

    public async Task<IEnumerable<Product>> GetByTypeAsync(ProductType productType)
    {
        var cacheKey = $"{ProductTypeCacheKeyPrefix}{(int)productType}";
        return await _cacheService.GetOrSetAsync(cacheKey, async () =>
        {
            return await _context.Products
                .Where(p => p.IsActive && p.ProductTypeId == (int)productType)
                .Include(p => p.ProductType)
                .Include(p => p.ProductCategory)
                .Include(p => p.ProductLine)
                .OrderBy(p => p.DisplayOrder)
                .ThenBy(p => p.Name)
                .ToListAsync();
        }, TimeSpan.FromHours(1)) ?? Enumerable.Empty<Product>();
    }

    public async Task<IEnumerable<Product>> GetByCategoryAsync(ProductCategory category)
    {
        var cacheKey = $"{ProductCategoryCacheKeyPrefix}{(int)category}";
        return await _cacheService.GetOrSetAsync(cacheKey, async () =>
        {
            return await _context.Products
                .Where(p => p.IsActive && p.ProductCategoryId == (int)category)
                .Include(p => p.ProductType)
                .Include(p => p.ProductCategory)
                .Include(p => p.ProductLine)
                .OrderBy(p => p.DisplayOrder)
                .ThenBy(p => p.Name)
                .ToListAsync();
        }, TimeSpan.FromHours(1)) ?? Enumerable.Empty<Product>();
    }

    public async Task<IEnumerable<Product>> GetByProductLineAsync(int productLineId)
    {
        var cacheKey = $"{ProductLineCacheKeyPrefix}{productLineId}";
        return await _cacheService.GetOrSetAsync(cacheKey, async () =>
        {
            return await _context.Products
                .Where(p => p.IsActive && p.ProductLineId == productLineId)
                .Include(p => p.ProductType)
                .Include(p => p.ProductCategory)
                .Include(p => p.ProductLine)
                .OrderBy(p => p.DisplayOrder)
                .ThenBy(p => p.Name)
                .ToListAsync();
        }, TimeSpan.FromHours(1)) ?? Enumerable.Empty<Product>();
    }

    public async Task<IEnumerable<Product>> GetFeaturedAsync()
    {
        return await _cacheService.GetOrSetAsync(FeaturedProductsCacheKey, async () =>
        {
            return await _context.Products
                .Where(p => p.IsActive && p.IsFeatured)
                .Include(p => p.ProductType)
                .Include(p => p.ProductCategory)
                .Include(p => p.ProductLine)
                .OrderBy(p => p.DisplayOrder)
                .ThenBy(p => p.Name)
                .ToListAsync();
        }, TimeSpan.FromHours(1)) ?? Enumerable.Empty<Product>();
    }

    public async Task<IEnumerable<Product>> GetPhysicalProductsAsync()
    {
        return await _cacheService.GetOrSetAsync(PhysicalProductsCacheKey, async () =>
        {
            return await _context.Products
                .Where(p => p.IsActive && p.ProductTypeId == (int)ProductType.PhysicalProduct)
                .Include(p => p.ProductType)
                .Include(p => p.ProductCategory)
                .Include(p => p.ProductLine)
                .OrderBy(p => p.DisplayOrder)
                .ThenBy(p => p.Name)
                .ToListAsync();
        }, TimeSpan.FromHours(1)) ?? Enumerable.Empty<Product>();
    }

    public async Task<IEnumerable<Product>> GetSubscriptionProductsAsync()
    {
        return await _cacheService.GetOrSetAsync(SubscriptionProductsCacheKey, async () =>
        {
            return await _context.Products
                .Where(p => p.IsActive && p.ProductTypeId == (int)ProductType.Subscription)
                .Include(p => p.ProductType)
                .Include(p => p.ProductCategory)
                .Include(p => p.ProductLine)
                .OrderBy(p => p.DisplayOrder)
                .ThenBy(p => p.Name)
                .ToListAsync();
        }, TimeSpan.FromHours(1)) ?? Enumerable.Empty<Product>();
    }

    public async Task AddAsync(Product product)
    {
        product.CreatedAt = DateTime.UtcNow;
        product.UpdatedAt = DateTime.UtcNow;
        await _context.Products.AddAsync(product);
        InvalidateCache();
    }

    public Task UpdateAsync(Product product)
    {
        product.UpdatedAt = DateTime.UtcNow;
        _context.Products.Update(product);
        InvalidateCache();
        InvalidateCacheForProduct(product);
        return Task.CompletedTask;
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }

    private void InvalidateCache()
    {
        _cacheService.Remove(AllProductsCacheKey);
        _cacheService.Remove(AllActiveProductsCacheKey);
        _cacheService.Remove(FeaturedProductsCacheKey);
        _cacheService.Remove(PhysicalProductsCacheKey);
        _cacheService.Remove(SubscriptionProductsCacheKey);
        _cacheService.RemoveByPattern(ProductTypeCacheKeyPrefix);
        _cacheService.RemoveByPattern(ProductCategoryCacheKeyPrefix);
        _cacheService.RemoveByPattern(ProductLineCacheKeyPrefix);
    }

    private void InvalidateCacheForProduct(Product product)
    {
        _cacheService.Remove($"{ProductCacheKeyPrefix}{product.Id}");
        _cacheService.Remove($"{ProductSKUCacheKeyPrefix}{product.SKU}");
    }
}
