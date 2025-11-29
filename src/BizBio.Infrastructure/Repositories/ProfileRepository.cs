using Microsoft.EntityFrameworkCore;
using BizBio.Core.Entities;
using BizBio.Core.Interfaces;
using BizBio.Infrastructure.Data;

namespace BizBio.Infrastructure.Repositories;

public class ProfileRepository : IProfileRepository
{
    private readonly ApplicationDbContext _context;

    public ProfileRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Profile?> GetByIdAsync(int id)
    {
        return await _context.Profiles
            .Include(p => p.User)
            .Include(p => p.Catalogs)
            .Include(p => p.RestaurantTables)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<Profile?> GetBySlugAsync(string slug)
    {
        return await _context.Profiles
            .Include(p => p.User)
            .Include(p => p.Catalogs)
            .Include(p => p.RestaurantTables)
            .FirstOrDefaultAsync(p => p.Slug == slug);
    }

    public async Task<IEnumerable<Profile>> GetByUserIdAsync(int userId)
    {
        return await _context.Profiles
            .Where(p => p.UserId == userId)
            .Include(p => p.Catalogs)
            .Include(p => p.RestaurantTables)
            .ToListAsync();
    }

    public async Task<int> GetProfileCountAsync(int userId)
    {
        return await _context.Profiles
            .Where(p => p.UserId == userId)
            .CountAsync();
    }

    public async Task AddAsync(Profile profile)
    {
        await _context.Profiles.AddAsync(profile);
    }

    public Task UpdateAsync(Profile profile)
    {
        _context.Profiles.Update(profile);
        return Task.CompletedTask;
    }

    public async Task DeleteAsync(int id)
    {
        var profile = await _context.Profiles.FindAsync(id);
        if (profile != null)
        {
            _context.Profiles.Remove(profile);
        }
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }
}
