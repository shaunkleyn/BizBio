using Microsoft.ApplicationInsights;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using BizBio.Core.Entities;
using BizBio.Core.Interfaces;
using BizBio.Infrastructure.Data;

namespace BizBio.Infrastructure.Repositories;

public class CatalogRepository : ICatalogRepository
{
    private readonly ApplicationDbContext _context;
    private readonly ICacheService _cacheService;
    private readonly ILogger<CatalogRepository> _logger;
    private readonly TelemetryClient _telemetryClient;
    private const string CatalogCacheKeyPrefix = "catalog_";
    private const string CatalogProfileCacheKeyPrefix = "catalog_profile_";

    public CatalogRepository(
        ApplicationDbContext context,
        ICacheService cacheService,
        ILogger<CatalogRepository> logger,
        TelemetryClient telemetryClient)
    {
        _context = context;
        _cacheService = cacheService;
        _logger = logger;
        _telemetryClient = telemetryClient;
    }

    public async Task<Catalog?> GetByIdAsync(int id)
    {
        try
        {
            _logger.LogDebug("Fetching catalog by ID: {CatalogId} (with cache)", id);
            var cacheKey = $"{CatalogCacheKeyPrefix}{id}";
            return await _cacheService.GetOrSetAsync(cacheKey, async () =>
            {
                return await _context.Catalogs
                    .Include(c => c.Profile)
                    .Include(c => c.Items)
                    .FirstOrDefaultAsync(c => c.Id == id);
            }, TimeSpan.FromMinutes(15));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching catalog by ID: {CatalogId}", id);
            _telemetryClient.TrackException(ex, new Dictionary<string, string>
            {
                { "Repository", "CatalogRepository" },
                { "Method", "GetByIdAsync" },
                { "CatalogId", id.ToString() }
            });
            throw;
        }
    }

    public async Task<Catalog?> GetByProfileIdAsync(int profileId)
    {
        try
        {
            _logger.LogDebug("Fetching catalog by profile ID: {ProfileId} (with cache)", profileId);
            var cacheKey = $"{CatalogProfileCacheKeyPrefix}{profileId}";
            return await _cacheService.GetOrSetAsync(cacheKey, async () =>
            {
                return await _context.Catalogs
                    .Include(c => c.Profile)
                    .Include(c => c.Items)
                    .FirstOrDefaultAsync(c => c.ProfileId == profileId);
            }, TimeSpan.FromMinutes(15));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching catalog by profile ID: {ProfileId}", profileId);
            _telemetryClient.TrackException(ex, new Dictionary<string, string>
            {
                { "Repository", "CatalogRepository" },
                { "Method", "GetByProfileIdAsync" },
                { "ProfileId", profileId.ToString() }
            });
            throw;
        }
    }

    public async Task<Catalog?> GetDetailByIdAsync(int id)
    {
        try
        {
            _logger.LogDebug("Fetching catalog detail by ID: {CatalogId} (no cache for editing)", id);
            // No cache for editing - need fresh data
            return await _context.Catalogs
                .Include(c => c.Profile)
                .Include(c => c.Categories.Where(cat => cat.IsActive).OrderBy(cat => cat.SortOrder))
                .Include(c => c.Items.Where(i => i.IsActive).OrderBy(i => i.SortOrder))
                    .ThenInclude(i => i.Variants.Where(v => v.IsActive))
                .Include(c => c.Items)
                    .ThenInclude(i => i.CatalogItemCategories)
                        .ThenInclude(cic => cic.Category)
                .Include(c => c.Items)
                    .ThenInclude(i => i.OptionGroupLinks.Where(l => l.IsActive))
                        .ThenInclude(l => l.OptionGroup)
                .Include(c => c.Items)
                    .ThenInclude(i => i.ExtraGroupLinks.Where(l => l.IsActive))
                        .ThenInclude(l => l.ExtraGroup)
                .Include(c => c.Bundles.Where(b => b.IsActive).OrderBy(b => b.SortOrder))
                    .ThenInclude(b => b.CatalogBundleCategories.Where(cbc => cbc.IsActive))
                        .ThenInclude(cbc => cbc.Category)
                .AsSplitQuery() // Use split query for better performance with multiple includes
                .FirstOrDefaultAsync(c => c.Id == id && c.IsActive);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching catalog detail by ID: {CatalogId}", id);
            _telemetryClient.TrackException(ex, new Dictionary<string, string>
            {
                { "Repository", "CatalogRepository" },
                { "Method", "GetDetailByIdAsync" },
                { "CatalogId", id.ToString() }
            });
            throw;
        }
    }

    public async Task<int> GetItemCountAsync(int catalogId)
    {
        try
        {
            _logger.LogDebug("Counting items for catalog: {CatalogId}", catalogId);
            var count = await _context.CatalogItems
                .Where(i => i.CatalogId == catalogId)
                .CountAsync();

            _logger.LogDebug("Catalog {CatalogId} has {Count} items", catalogId, count);
            return count;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error counting items for catalog: {CatalogId}", catalogId);
            _telemetryClient.TrackException(ex, new Dictionary<string, string>
            {
                { "Repository", "CatalogRepository" },
                { "Method", "GetItemCountAsync" },
                { "CatalogId", catalogId.ToString() }
            });
            throw;
        }
    }

    public async Task AddAsync(Catalog catalog)
    {
        try
        {
            _logger.LogInformation("Adding new catalog for profile: {ProfileId}", catalog.ProfileId);
            await _context.Catalogs.AddAsync(catalog);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding catalog for profile: {ProfileId}", catalog.ProfileId);
            _telemetryClient.TrackException(ex, new Dictionary<string, string>
            {
                { "Repository", "CatalogRepository" },
                { "Method", "AddAsync" },
                { "ProfileId", catalog.ProfileId.ToString() }
            });
            throw;
        }
    }

    public Task UpdateAsync(Catalog catalog)
    {
        try
        {
            _logger.LogInformation("Updating catalog: {CatalogId}, invalidating cache", catalog.Id);
            _context.Catalogs.Update(catalog);
            // Invalidate cache when updating
            _cacheService.Remove($"{CatalogCacheKeyPrefix}{catalog.Id}");
            _cacheService.Remove($"{CatalogProfileCacheKeyPrefix}{catalog.ProfileId}");
            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating catalog: {CatalogId}", catalog.Id);
            _telemetryClient.TrackException(ex, new Dictionary<string, string>
            {
                { "Repository", "CatalogRepository" },
                { "Method", "UpdateAsync" },
                { "CatalogId", catalog.Id.ToString() }
            });
            throw;
        }
    }

    public async Task<int> SaveChangesAsync()
    {
        try
        {
            var changes = await _context.SaveChangesAsync();
            if (changes > 0)
            {
                _logger.LogDebug("Saved {Count} changes to database", changes);
            }
            return changes;
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Database update error in CatalogRepository");
            _telemetryClient.TrackException(ex, new Dictionary<string, string>
            {
                { "Repository", "CatalogRepository" },
                { "Method", "SaveChangesAsync" },
                { "ErrorType", "DbUpdateException" }
            });
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving changes in CatalogRepository");
            _telemetryClient.TrackException(ex, new Dictionary<string, string>
            {
                { "Repository", "CatalogRepository" },
                { "Method", "SaveChangesAsync" }
            });
            throw;
        }
    }
}