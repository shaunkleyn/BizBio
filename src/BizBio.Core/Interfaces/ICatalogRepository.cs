using BizBio.Core.Entities;

namespace BizBio.Core.Interfaces;

public interface ICatalogRepository
{
    Task<Catalog?> GetByIdAsync(int id);
    Task<Catalog?> GetByProfileIdAsync(int profileId);
    Task<Catalog?> GetDetailByIdAsync(int id);
    Task<int> GetItemCountAsync(int catalogId);
    Task AddAsync(Catalog catalog);
    Task UpdateAsync(Catalog catalog);
    Task<int> SaveChangesAsync();
}
