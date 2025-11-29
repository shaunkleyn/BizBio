using Microsoft.EntityFrameworkCore;
using BizBio.Core.Entities;
using BizBio.Core.Enums;
using BizBio.Core.Interfaces;
using BizBio.Infrastructure.Data;

namespace BizBio.Infrastructure.Repositories;

public class UserSubscriptionRepository : IUserSubscriptionRepository
{
    private readonly ApplicationDbContext _context;

    public UserSubscriptionRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<UserSubscription?> GetByIdAsync(int id)
    {
        return await _context.UserSubscriptions
            .Include(s => s.User)
            .Include(s => s.Tier)
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<IEnumerable<UserSubscription>> GetByUserIdAsync(int userId)
    {
        return await _context.UserSubscriptions
            .Where(s => s.UserId == userId)
            .Include(s => s.Tier)
            .Include(s => s.User)
            .ToListAsync();
    }

    public async Task<IEnumerable<UserSubscription>> GetActiveSubscriptionsAsync(int userId)
    {
        return await _context.UserSubscriptions
            .Where(s => s.UserId == userId && s.StatusId == (int)SubscriptionStatus.Active)
            .Include(s => s.Tier)
            .Include(s => s.User)
            .ToListAsync();
    }

    public async Task<UserSubscription?> GetActiveSubscriptionByProductLineAsync(int userId, ProductLine productLine)
    {
        return await _context.UserSubscriptions
            .Where(s => s.UserId == userId && s.StatusId == (int)SubscriptionStatus.Active)
            .Include(s => s.Tier)
            .FirstOrDefaultAsync(s => s.Tier.ProductLine.Id == (int)productLine);
    }

    public async Task<IEnumerable<UserSubscription>> GetTrialsEndingBetweenAsync(DateTime start, DateTime end)
    {
        return await _context.UserSubscriptions
            .Where(s => s.StatusId == (int)SubscriptionStatus.Trial &&
                        s.TrialEndsAt.HasValue &&
                        s.TrialEndsAt >= start &&
                        s.TrialEndsAt <= end)
            .Include(s => s.User)
            .Include(s => s.Tier)
            .ToListAsync();
    }

    public async Task AddAsync(UserSubscription subscription)
    {
        await _context.UserSubscriptions.AddAsync(subscription);
    }

    public Task UpdateAsync(UserSubscription subscription)
    {
        _context.UserSubscriptions.Update(subscription);
        return Task.CompletedTask;
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }
}
