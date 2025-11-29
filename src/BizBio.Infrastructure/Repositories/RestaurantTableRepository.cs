using Microsoft.EntityFrameworkCore;
using BizBio.Core.Entities;
using BizBio.Core.Interfaces;
using BizBio.Infrastructure.Data;

namespace BizBio.Infrastructure.Repositories;

public class RestaurantTableRepository : IRestaurantTableRepository
{
    private readonly ApplicationDbContext _context;

    public RestaurantTableRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<RestaurantTable?> GetByIdAsync(int id)
    {
        return await _context.RestaurantTables
            .Include(t => t.Profile)
            .FirstOrDefaultAsync(t => t.Id == id);
    }

    public async Task<RestaurantTable?> GetByNFCCodeAsync(string nfcCode)
    {
        return await _context.RestaurantTables
            .Include(t => t.Profile)
            .FirstOrDefaultAsync(t => t.NFCTagCode == nfcCode);
    }

    public async Task<IEnumerable<RestaurantTable>> GetByProfileIdAsync(int profileId)
    {
        return await _context.RestaurantTables
            .Where(t => t.ProfileId == profileId)
            .Include(t => t.Profile)
            .ToListAsync();
    }

    public async Task AddAsync(RestaurantTable table)
    {
        await _context.RestaurantTables.AddAsync(table);
    }

    public Task UpdateAsync(RestaurantTable table)
    {
        _context.RestaurantTables.Update(table);
        return Task.CompletedTask;
    }

    public async Task DeleteAsync(int id)
    {
        var table = await _context.RestaurantTables.FindAsync(id);
        if (table != null)
        {
            _context.RestaurantTables.Remove(table);
        }
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }
}
