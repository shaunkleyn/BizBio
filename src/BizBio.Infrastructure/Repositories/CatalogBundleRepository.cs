using Microsoft.ApplicationInsights;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using BizBio.Core.Entities;
using BizBio.Core.Interfaces;
using BizBio.Infrastructure.Data;

namespace BizBio.Infrastructure.Repositories;

public class CatalogBundleRepository : ICatalogBundleRepository
{
    private readonly ApplicationDbContext _context;
    private readonly ICacheService _cacheService;
    private readonly ILogger<CatalogBundleRepository> _logger;
    private readonly TelemetryClient _telemetryClient;
    private const string BundleCacheKeyPrefix = "bundle_";
    private const string BundleCatalogCacheKeyPrefix = "bundles_catalog_";

    public CatalogBundleRepository(
        ApplicationDbContext context,
        ICacheService cacheService,
        ILogger<CatalogBundleRepository> logger,
        TelemetryClient telemetryClient)
    {
        _context = context;
        _cacheService = cacheService;
        _logger = logger;
        _telemetryClient = telemetryClient;
    }

    public async Task<CatalogBundle?> GetByIdAsync(int id)
    {
        try
        {
            _logger.LogDebug("Fetching bundle by ID: {BundleId}", id);
            return await _context.CatalogBundles
                .FirstOrDefaultAsync(b => b.Id == id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching bundle by ID: {BundleId}", id);
            _telemetryClient.TrackException(ex, new Dictionary<string, string>
            {
                { "Repository", "CatalogBundleRepository" },
                { "Method", "GetByIdAsync" },
                { "BundleId", id.ToString() }
            });
            throw;
        }
    }

    public async Task<CatalogBundle?> GetByIdWithDetailsAsync(int id)
    {
        try
        {
            _logger.LogDebug("Fetching bundle with details by ID: {BundleId} (with cache)", id);
            var cacheKey = $"{BundleCacheKeyPrefix}{id}";
            return await _cacheService.GetOrSetAsync(cacheKey, async () =>
            {
                return await _context.CatalogBundles
                    .Include(b => b.Steps)
                        .ThenInclude(s => s.AllowedProducts)
                            .ThenInclude(p => p.Product)
                    .Include(b => b.Steps)
                        .ThenInclude(s => s.OptionGroups)
                            .ThenInclude(g => g.Options)
                    .FirstOrDefaultAsync(b => b.Id == id);
            }, TimeSpan.FromMinutes(15));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching bundle with details by ID: {BundleId}", id);
            _telemetryClient.TrackException(ex, new Dictionary<string, string>
            {
                { "Repository", "CatalogBundleRepository" },
                { "Method", "GetByIdWithDetailsAsync" },
                { "BundleId", id.ToString() }
            });
            throw;
        }
    }

    public async Task<IEnumerable<CatalogBundle>> GetByCatalogIdAsync(int catalogId)
    {
        try
        {
            _logger.LogDebug("Fetching bundles for catalog: {CatalogId} (with cache)", catalogId);
            var cacheKey = $"{BundleCatalogCacheKeyPrefix}{catalogId}";
            return await _cacheService.GetOrSetAsync(cacheKey, async () =>
            {
                return await _context.CatalogBundles
                    .Where(b => b.CatalogId == catalogId && b.IsActive)
                    .OrderBy(b => b.SortOrder)
                    .ThenBy(b => b.Name)
                    .ToListAsync();
            }, TimeSpan.FromMinutes(15));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching bundles for catalog: {CatalogId}", catalogId);
            _telemetryClient.TrackException(ex, new Dictionary<string, string>
            {
                { "Repository", "CatalogBundleRepository" },
                { "Method", "GetByCatalogIdAsync" },
                { "CatalogId", catalogId.ToString() }
            });
            throw;
        }
    }

    public async Task<int> GetBundleCountByCatalogIdAsync(int catalogId)
    {
        try
        {
            _logger.LogDebug("Counting bundles for catalog: {CatalogId}", catalogId);
            var count = await _context.CatalogBundles
                .Where(b => b.CatalogId == catalogId)
                .CountAsync();

            _logger.LogDebug("Catalog {CatalogId} has {Count} bundles", catalogId, count);
            return count;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error counting bundles for catalog: {CatalogId}", catalogId);
            _telemetryClient.TrackException(ex, new Dictionary<string, string>
            {
                { "Repository", "CatalogBundleRepository" },
                { "Method", "GetBundleCountByCatalogIdAsync" },
                { "CatalogId", catalogId.ToString() }
            });
            throw;
        }
    }

    public async Task AddAsync(CatalogBundle bundle)
    {
        try
        {
            _logger.LogInformation("Adding new bundle: {BundleName} to catalog: {CatalogId}", bundle.Name, bundle.CatalogId);
            await _context.CatalogBundles.AddAsync(bundle);
            InvalidateCatalogCache(bundle.CatalogId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding bundle: {BundleName}", bundle.Name);
            _telemetryClient.TrackException(ex, new Dictionary<string, string>
            {
                { "Repository", "CatalogBundleRepository" },
                { "Method", "AddAsync" },
                { "BundleName", bundle.Name }
            });
            throw;
        }
    }

    public Task UpdateAsync(CatalogBundle bundle)
    {
        try
        {
            _logger.LogInformation("Updating bundle: {BundleId}, invalidating cache", bundle.Id);
            _context.CatalogBundles.Update(bundle);
            InvalidateBundleCache(bundle.Id);
            InvalidateCatalogCache(bundle.CatalogId);
            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating bundle: {BundleId}", bundle.Id);
            _telemetryClient.TrackException(ex, new Dictionary<string, string>
            {
                { "Repository", "CatalogBundleRepository" },
                { "Method", "UpdateAsync" },
                { "BundleId", bundle.Id.ToString() }
            });
            throw;
        }
    }

    public async Task DeleteAsync(int id)
    {
        try
        {
            _logger.LogInformation("Deleting bundle: {BundleId}", id);
            var bundle = await GetByIdAsync(id);
            if (bundle != null)
            {
                _context.CatalogBundles.Remove(bundle);
                InvalidateBundleCache(id);
                InvalidateCatalogCache(bundle.CatalogId);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting bundle: {BundleId}", id);
            _telemetryClient.TrackException(ex, new Dictionary<string, string>
            {
                { "Repository", "CatalogBundleRepository" },
                { "Method", "DeleteAsync" },
                { "BundleId", id.ToString() }
            });
            throw;
        }
    }

    public async Task AddStepAsync(CatalogBundleStep step)
    {
        try
        {
            _logger.LogInformation("Adding step: {StepName} to bundle: {BundleId}", step.Name, step.BundleId);
            await _context.CatalogBundleSteps.AddAsync(step);
            InvalidateBundleCache(step.BundleId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding step: {StepName} to bundle: {BundleId}", step.Name, step.BundleId);
            _telemetryClient.TrackException(ex, new Dictionary<string, string>
            {
                { "Repository", "CatalogBundleRepository" },
                { "Method", "AddStepAsync" },
                { "StepName", step.Name },
                { "BundleId", step.BundleId.ToString() }
            });
            throw;
        }
    }

    public Task UpdateStepAsync(CatalogBundleStep step)
    {
        try
        {
            _logger.LogInformation("Updating step: {StepId}", step.Id);
            _context.CatalogBundleSteps.Update(step);
            InvalidateBundleCache(step.BundleId);
            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating step: {StepId}", step.Id);
            _telemetryClient.TrackException(ex, new Dictionary<string, string>
            {
                { "Repository", "CatalogBundleRepository" },
                { "Method", "UpdateStepAsync" },
                { "StepId", step.Id.ToString() }
            });
            throw;
        }
    }

    public async Task DeleteStepAsync(int stepId)
    {
        try
        {
            _logger.LogInformation("Deleting step: {StepId}", stepId);
            var step = await _context.CatalogBundleSteps.FindAsync(stepId);
            if (step != null)
            {
                _context.CatalogBundleSteps.Remove(step);
                InvalidateBundleCache(step.BundleId);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting step: {StepId}", stepId);
            _telemetryClient.TrackException(ex, new Dictionary<string, string>
            {
                { "Repository", "CatalogBundleRepository" },
                { "Method", "DeleteStepAsync" },
                { "StepId", stepId.ToString() }
            });
            throw;
        }
    }

    public async Task AddStepProductAsync(CatalogBundleStepProduct stepProduct)
    {
        try
        {
            _logger.LogInformation("Adding product: {ProductId} to step: {StepId}", stepProduct.ProductId, stepProduct.StepId);
            await _context.CatalogBundleStepProducts.AddAsync(stepProduct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding product: {ProductId} to step: {StepId}", stepProduct.ProductId, stepProduct.StepId);
            _telemetryClient.TrackException(ex, new Dictionary<string, string>
            {
                { "Repository", "CatalogBundleRepository" },
                { "Method", "AddStepProductAsync" },
                { "ProductId", stepProduct.ProductId.ToString() },
                { "StepId", stepProduct.StepId.ToString() }
            });
            throw;
        }
    }

    public async Task DeleteStepProductAsync(int stepId, int productId)
    {
        try
        {
            _logger.LogInformation("Deleting product: {ProductId} from step: {StepId}", productId, stepId);
            var stepProduct = await _context.CatalogBundleStepProducts
                .FirstOrDefaultAsync(sp => sp.StepId == stepId && sp.ProductId == productId);
            if (stepProduct != null)
            {
                _context.CatalogBundleStepProducts.Remove(stepProduct);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting product: {ProductId} from step: {StepId}", productId, stepId);
            _telemetryClient.TrackException(ex, new Dictionary<string, string>
            {
                { "Repository", "CatalogBundleRepository" },
                { "Method", "DeleteStepProductAsync" },
                { "ProductId", productId.ToString() },
                { "StepId", stepId.ToString() }
            });
            throw;
        }
    }

    public async Task AddOptionGroupAsync(CatalogBundleOptionGroup optionGroup)
    {
        try
        {
            _logger.LogInformation("Adding option group: {GroupName} to step: {StepId}", optionGroup.Name, optionGroup.StepId);
            await _context.CatalogBundleOptionGroups.AddAsync(optionGroup);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding option group: {GroupName} to step: {StepId}", optionGroup.Name, optionGroup.StepId);
            _telemetryClient.TrackException(ex, new Dictionary<string, string>
            {
                { "Repository", "CatalogBundleRepository" },
                { "Method", "AddOptionGroupAsync" },
                { "GroupName", optionGroup.Name },
                { "StepId", optionGroup.StepId.ToString() }
            });
            throw;
        }
    }

    public Task UpdateOptionGroupAsync(CatalogBundleOptionGroup optionGroup)
    {
        try
        {
            _logger.LogInformation("Updating option group: {GroupId}", optionGroup.Id);
            _context.CatalogBundleOptionGroups.Update(optionGroup);
            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating option group: {GroupId}", optionGroup.Id);
            _telemetryClient.TrackException(ex, new Dictionary<string, string>
            {
                { "Repository", "CatalogBundleRepository" },
                { "Method", "UpdateOptionGroupAsync" },
                { "GroupId", optionGroup.Id.ToString() }
            });
            throw;
        }
    }

    public async Task DeleteOptionGroupAsync(int optionGroupId)
    {
        try
        {
            _logger.LogInformation("Deleting option group: {GroupId}", optionGroupId);
            var optionGroup = await _context.CatalogBundleOptionGroups.FindAsync(optionGroupId);
            if (optionGroup != null)
            {
                _context.CatalogBundleOptionGroups.Remove(optionGroup);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting option group: {GroupId}", optionGroupId);
            _telemetryClient.TrackException(ex, new Dictionary<string, string>
            {
                { "Repository", "CatalogBundleRepository" },
                { "Method", "DeleteOptionGroupAsync" },
                { "GroupId", optionGroupId.ToString() }
            });
            throw;
        }
    }

    public async Task AddOptionAsync(CatalogBundleOption option)
    {
        try
        {
            _logger.LogInformation("Adding option: {OptionName} to group: {GroupId}", option.Name, option.OptionGroupId);
            await _context.CatalogBundleOptions.AddAsync(option);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding option: {OptionName} to group: {GroupId}", option.Name, option.OptionGroupId);
            _telemetryClient.TrackException(ex, new Dictionary<string, string>
            {
                { "Repository", "CatalogBundleRepository" },
                { "Method", "AddOptionAsync" },
                { "OptionName", option.Name },
                { "GroupId", option.OptionGroupId.ToString() }
            });
            throw;
        }
    }

    public Task UpdateOptionAsync(CatalogBundleOption option)
    {
        try
        {
            _logger.LogInformation("Updating option: {OptionId}", option.Id);
            _context.CatalogBundleOptions.Update(option);
            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating option: {OptionId}", option.Id);
            _telemetryClient.TrackException(ex, new Dictionary<string, string>
            {
                { "Repository", "CatalogBundleRepository" },
                { "Method", "UpdateOptionAsync" },
                { "OptionId", option.Id.ToString() }
            });
            throw;
        }
    }

    public async Task DeleteOptionAsync(int optionId)
    {
        try
        {
            _logger.LogInformation("Deleting option: {OptionId}", optionId);
            var option = await _context.CatalogBundleOptions.FindAsync(optionId);
            if (option != null)
            {
                _context.CatalogBundleOptions.Remove(option);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting option: {OptionId}", optionId);
            _telemetryClient.TrackException(ex, new Dictionary<string, string>
            {
                { "Repository", "CatalogBundleRepository" },
                { "Method", "DeleteOptionAsync" },
                { "OptionId", optionId.ToString() }
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
            _logger.LogError(ex, "Database update error in CatalogBundleRepository");
            _telemetryClient.TrackException(ex, new Dictionary<string, string>
            {
                { "Repository", "CatalogBundleRepository" },
                { "Method", "SaveChangesAsync" },
                { "ErrorType", "DbUpdateException" }
            });
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving changes in CatalogBundleRepository");
            _telemetryClient.TrackException(ex, new Dictionary<string, string>
            {
                { "Repository", "CatalogBundleRepository" },
                { "Method", "SaveChangesAsync" }
            });
            throw;
        }
    }

    private void InvalidateBundleCache(int bundleId)
    {
        _cacheService.Remove($"{BundleCacheKeyPrefix}{bundleId}");
    }

    private void InvalidateCatalogCache(int catalogId)
    {
        _cacheService.Remove($"{BundleCatalogCacheKeyPrefix}{catalogId}");
    }
}
