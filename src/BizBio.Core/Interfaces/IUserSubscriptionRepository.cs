using BizBio.Core.Entities;
using BizBio.Core.Enums;

namespace BizBio.Core.Interfaces;

public interface IUserSubscriptionRepository
{
    Task<UserSubscription?> GetByIdAsync(int id);
    Task<IEnumerable<UserSubscription>> GetByUserIdAsync(int userId);
    Task<IEnumerable<UserSubscription>> GetActiveSubscriptionsAsync(int userId);
    Task<UserSubscription?> GetActiveSubscriptionByProductLineAsync(int userId, ProductLine productLine);
    Task<IEnumerable<UserSubscription>> GetTrialsEndingBetweenAsync(DateTime start, DateTime end);
    Task AddAsync(UserSubscription subscription);
    Task UpdateAsync(UserSubscription subscription);
    Task<int> SaveChangesAsync();
}
