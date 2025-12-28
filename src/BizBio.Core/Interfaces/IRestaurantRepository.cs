using BizBio.Core.Entities;

namespace BizBio.Core.Interfaces;

public interface IRestaurantRepository
{
    Task<Restaurant?> GetByIdAsync(int id);
    Task<Restaurant?> GetByIdWithProfilesAsync(int id);
    Task<IEnumerable<Restaurant>> GetByUserIdAsync(int userId);
    Task<int> GetRestaurantCountAsync(int userId);
    Task AddAsync(Restaurant restaurant);
    Task UpdateAsync(Restaurant restaurant);
    Task DeleteAsync(int id);
    Task<int> SaveChangesAsync();
}
