using BizBio.Core.Entities;
using BizBio.Core.Enums;

namespace BizBio.Core.Interfaces;

public interface ISubscriptionTierRepository
{
    Task<SubscriptionTier?> GetByIdAsync(int id);
    Task<SubscriptionTier?> GetByCodeAsync(string tierCode);
    Task<IEnumerable<SubscriptionTier>> GetAllActiveAsync();
    Task<IEnumerable<SubscriptionTier>> GetByProductLineAsync(ProductLine productLine);
    Task AddAsync(SubscriptionTier tier);
    Task UpdateAsync(SubscriptionTier tier);
    Task<int> SaveChangesAsync();
}
