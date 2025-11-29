using BizBio.Core.Entities;

namespace BizBio.Core.Interfaces;

public interface IRestaurantTableRepository
{
    Task<RestaurantTable?> GetByIdAsync(int id);
    Task<RestaurantTable?> GetByNFCCodeAsync(string nfcCode);
    Task<IEnumerable<RestaurantTable>> GetByProfileIdAsync(int profileId);
    Task AddAsync(RestaurantTable table);
    Task UpdateAsync(RestaurantTable table);
    Task DeleteAsync(int id);
    Task<int> SaveChangesAsync();
}
