using BizBio.Core.Entities;

namespace BizBio.Core.Interfaces;

public interface IProfileRepository
{
    Task<Profile?> GetByIdAsync(int id);
    Task<Profile?> GetBySlugAsync(string slug);
    Task<IEnumerable<Profile>> GetByUserIdAsync(int userId);
    Task<int> GetProfileCountAsync(int userId);
    Task AddAsync(Profile profile);
    Task UpdateAsync(Profile profile);
    Task DeleteAsync(int id);
    Task<int> SaveChangesAsync();
}
