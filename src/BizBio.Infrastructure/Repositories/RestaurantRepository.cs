using Microsoft.ApplicationInsights;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using BizBio.Core.Entities;
using BizBio.Core.Interfaces;
using BizBio.Infrastructure.Data;

namespace BizBio.Infrastructure.Repositories;

public class RestaurantRepository : IRestaurantRepository
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<RestaurantRepository> _logger;
    private readonly TelemetryClient _telemetryClient;

    public RestaurantRepository(
        ApplicationDbContext context,
        ILogger<RestaurantRepository> logger,
        TelemetryClient telemetryClient)
    {
        _context = context;
        _logger = logger;
        _telemetryClient = telemetryClient;
    }

    public async Task<Restaurant?> GetByIdAsync(int id)
    {
        try
        {
            _logger.LogDebug("Fetching restaurant by ID: {RestaurantId}", id);
            var restaurant = await _context.Restaurants
                .Include(r => r.User)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (restaurant == null)
            {
                _logger.LogDebug("Restaurant {RestaurantId} not found", id);
            }

            return restaurant;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching restaurant by ID: {RestaurantId}", id);
            _telemetryClient.TrackException(ex, new Dictionary<string, string>
            {
                { "Repository", "RestaurantRepository" },
                { "Method", "GetByIdAsync" },
                { "RestaurantId", id.ToString() }
            });
            throw;
        }
    }

    public async Task<Restaurant?> GetByIdWithProfilesAsync(int id)
    {
        try
        {
            _logger.LogDebug("Fetching restaurant by ID with profiles: {RestaurantId}", id);
            var restaurant = await _context.Restaurants
                .Include(r => r.User)
                .Include(r => r.Profiles)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (restaurant == null)
            {
                _logger.LogDebug("Restaurant {RestaurantId} not found", id);
            }

            return restaurant;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching restaurant by ID with profiles: {RestaurantId}", id);
            _telemetryClient.TrackException(ex, new Dictionary<string, string>
            {
                { "Repository", "RestaurantRepository" },
                { "Method", "GetByIdWithProfilesAsync" },
                { "RestaurantId", id.ToString() }
            });
            throw;
        }
    }

    public async Task<IEnumerable<Restaurant>> GetByUserIdAsync(int userId)
    {
        try
        {
            _logger.LogDebug("Fetching restaurants for user: {UserId}", userId);
            var restaurants = await _context.Restaurants
                .Where(r => r.UserId == userId)
                .Include(r => r.Profiles)
                .OrderBy(r => r.SortOrder)
                .ThenBy(r => r.Name)
                .ToListAsync();

            _logger.LogDebug("Found {Count} restaurants for user {UserId}", restaurants.Count, userId);
            return restaurants;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching restaurants for user: {UserId}", userId);
            _telemetryClient.TrackException(ex, new Dictionary<string, string>
            {
                { "Repository", "RestaurantRepository" },
                { "Method", "GetByUserIdAsync" },
                { "UserId", userId.ToString() }
            });
            throw;
        }
    }

    public async Task<int> GetRestaurantCountAsync(int userId)
    {
        try
        {
            _logger.LogDebug("Counting restaurants for user: {UserId}", userId);
            var count = await _context.Restaurants
                .Where(r => r.UserId == userId && r.IsActive)
                .CountAsync();

            _logger.LogDebug("User {UserId} has {Count} restaurants", userId, count);
            return count;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error counting restaurants for user: {UserId}", userId);
            _telemetryClient.TrackException(ex, new Dictionary<string, string>
            {
                { "Repository", "RestaurantRepository" },
                { "Method", "GetRestaurantCountAsync" },
                { "UserId", userId.ToString() }
            });
            throw;
        }
    }

    public async Task AddAsync(Restaurant restaurant)
    {
        try
        {
            _logger.LogInformation("Adding new restaurant: {Name} for user {UserId}", restaurant.Name, restaurant.UserId);
            await _context.Restaurants.AddAsync(restaurant);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding restaurant: {Name}", restaurant.Name);
            _telemetryClient.TrackException(ex, new Dictionary<string, string>
            {
                { "Repository", "RestaurantRepository" },
                { "Method", "AddAsync" },
                { "Name", restaurant.Name }
            });
            throw;
        }
    }

    public Task UpdateAsync(Restaurant restaurant)
    {
        try
        {
            _logger.LogInformation("Updating restaurant: {RestaurantId}", restaurant.Id);
            _context.Restaurants.Update(restaurant);
            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating restaurant: {RestaurantId}", restaurant.Id);
            _telemetryClient.TrackException(ex, new Dictionary<string, string>
            {
                { "Repository", "RestaurantRepository" },
                { "Method", "UpdateAsync" },
                { "RestaurantId", restaurant.Id.ToString() }
            });
            throw;
        }
    }

    public async Task DeleteAsync(int id)
    {
        try
        {
            _logger.LogWarning("Deleting restaurant: {RestaurantId}", id);
            var restaurant = await _context.Restaurants.FindAsync(id);
            if (restaurant != null)
            {
                _context.Restaurants.Remove(restaurant);
                _logger.LogInformation("Restaurant {RestaurantId} marked for deletion", id);
            }
            else
            {
                _logger.LogWarning("Restaurant {RestaurantId} not found for deletion", id);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting restaurant: {RestaurantId}", id);
            _telemetryClient.TrackException(ex, new Dictionary<string, string>
            {
                { "Repository", "RestaurantRepository" },
                { "Method", "DeleteAsync" },
                { "RestaurantId", id.ToString() }
            });
            throw;
        }
    }

    public async Task<int> SaveChangesAsync()
    {
        try
        {
            var changes = await _context.SaveChangesAsync();
            if (changes > 0)
            {
                _logger.LogDebug("Saved {Count} changes to database", changes);
            }
            return changes;
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Database update error in RestaurantRepository");
            _telemetryClient.TrackException(ex, new Dictionary<string, string>
            {
                { "Repository", "RestaurantRepository" },
                { "Method", "SaveChangesAsync" },
                { "ErrorType", "DbUpdateException" }
            });
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving changes in RestaurantRepository");
            _telemetryClient.TrackException(ex, new Dictionary<string, string>
            {
                { "Repository", "RestaurantRepository" },
                { "Method", "SaveChangesAsync" }
            });
            throw;
        }
    }
}
