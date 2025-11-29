using Microsoft.EntityFrameworkCore;
using BizBio.Core.Entities;
using BizBio.Core.Interfaces;
using BizBio.Infrastructure.Data;

namespace BizBio.Infrastructure.Repositories;

public class CatalogRepository : ICatalogRepository
{
    private readonly ApplicationDbContext _context;
    private readonly ICacheService _cacheService;
    private const string CatalogCacheKeyPrefix = "catalog_";
    private const string CatalogProfileCacheKeyPrefix = "catalog_profile_";

    public CatalogRepository(ApplicationDbContext context, ICacheService cacheService)
    {
        _context = context;
        _cacheService = cacheService;
    }

    public async Task<Catalog?> GetByIdAsync(int id)
    {
        var cacheKey = $"{CatalogCacheKeyPrefix}{id}";
        return await _cacheService.GetOrSetAsync(cacheKey, async () =>
        {
            return await _context.Catalogs
                .Include(c => c.Profile)
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.Id == id);
        }, TimeSpan.FromMinutes(15));
    }

    public async Task<Catalog?> GetByProfileIdAsync(int profileId)
    {
        var cacheKey = $"{CatalogProfileCacheKeyPrefix}{profileId}";
        return await _cacheService.GetOrSetAsync(cacheKey, async () =>
        {
            return await _context.Catalogs
                .Include(c => c.Profile)
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.ProfileId == profileId);
        }, TimeSpan.FromMinutes(15));
    }

    public async Task<int> GetItemCountAsync(int catalogId)
    {
        return await _context.CatalogItems
            .Where(i => i.CatalogId == catalogId)
            .CountAsync();
    }

    public async Task AddAsync(Catalog catalog)
    {
        await _context.Catalogs.AddAsync(catalog);
    }

    public Task UpdateAsync(Catalog catalog)
    {
        _context.Catalogs.Update(catalog);
        // Invalidate cache when updating
        _cacheService.Remove($"{CatalogCacheKeyPrefix}{catalog.Id}");
        _cacheService.Remove($"{CatalogProfileCacheKeyPrefix}{catalog.ProfileId}");
        return Task.CompletedTask;
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }
}