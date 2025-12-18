using Microsoft.ApplicationInsights;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using BizBio.Core.Entities;
using BizBio.Core.Interfaces;
using BizBio.Infrastructure.Data;

namespace BizBio.Infrastructure.Repositories;

public class ProfileRepository : IProfileRepository
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<ProfileRepository> _logger;
    private readonly TelemetryClient _telemetryClient;

    public ProfileRepository(
        ApplicationDbContext context,
        ILogger<ProfileRepository> logger,
        TelemetryClient telemetryClient)
    {
        _context = context;
        _logger = logger;
        _telemetryClient = telemetryClient;
    }

    public async Task<Profile?> GetByIdAsync(int id)
    {
        try
        {
            _logger.LogDebug("Fetching profile by ID: {ProfileId}", id);
            var profile = await _context.Profiles
                .Include(p => p.User)
                .Include(p => p.Catalogs)
                .Include(p => p.RestaurantTables)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (profile == null)
            {
                _logger.LogDebug("Profile {ProfileId} not found", id);
            }

            return profile;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching profile by ID: {ProfileId}", id);
            _telemetryClient.TrackException(ex, new Dictionary<string, string>
            {
                { "Repository", "ProfileRepository" },
                { "Method", "GetByIdAsync" },
                { "ProfileId", id.ToString() }
            });
            throw;
        }
    }

    public async Task<Profile?> GetBySlugAsync(string slug)
    {
        try
        {
            _logger.LogDebug("Fetching profile by slug: {Slug}", slug);
            var profile = await _context.Profiles
                .Include(p => p.User)
                .Include(p => p.Catalogs)
                .Include(p => p.RestaurantTables)
                .FirstOrDefaultAsync(p => p.Slug == slug);

            if (profile == null)
            {
                _logger.LogDebug("Profile with slug {Slug} not found", slug);
            }

            return profile;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching profile by slug: {Slug}", slug);
            _telemetryClient.TrackException(ex, new Dictionary<string, string>
            {
                { "Repository", "ProfileRepository" },
                { "Method", "GetBySlugAsync" },
                { "Slug", slug }
            });
            throw;
        }
    }

    public async Task<IEnumerable<Profile>> GetByUserIdAsync(int userId)
    {
        try
        {
            _logger.LogDebug("Fetching profiles for user: {UserId}", userId);
            var profiles = await _context.Profiles
                .Where(p => p.UserId == userId)
                .Include(p => p.Catalogs)
                .Include(p => p.RestaurantTables)
                .ToListAsync();

            _logger.LogDebug("Found {Count} profiles for user {UserId}", profiles.Count, userId);
            return profiles;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching profiles for user: {UserId}", userId);
            _telemetryClient.TrackException(ex, new Dictionary<string, string>
            {
                { "Repository", "ProfileRepository" },
                { "Method", "GetByUserIdAsync" },
                { "UserId", userId.ToString() }
            });
            throw;
        }
    }

    public async Task<int> GetProfileCountAsync(int userId)
    {
        try
        {
            _logger.LogDebug("Counting profiles for user: {UserId}", userId);
            var count = await _context.Profiles
                .Where(p => p.UserId == userId)
                .CountAsync();

            _logger.LogDebug("User {UserId} has {Count} profiles", userId, count);
            return count;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error counting profiles for user: {UserId}", userId);
            _telemetryClient.TrackException(ex, new Dictionary<string, string>
            {
                { "Repository", "ProfileRepository" },
                { "Method", "GetProfileCountAsync" },
                { "UserId", userId.ToString() }
            });
            throw;
        }
    }

    public async Task AddAsync(Profile profile)
    {
        try
        {
            _logger.LogInformation("Adding new profile: {Slug} for user {UserId}", profile.Slug, profile.UserId);
            await _context.Profiles.AddAsync(profile);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding profile: {Slug}", profile.Slug);
            _telemetryClient.TrackException(ex, new Dictionary<string, string>
            {
                { "Repository", "ProfileRepository" },
                { "Method", "AddAsync" },
                { "Slug", profile.Slug }
            });
            throw;
        }
    }

    public Task UpdateAsync(Profile profile)
    {
        try
        {
            _logger.LogInformation("Updating profile: {ProfileId}", profile.Id);
            _context.Profiles.Update(profile);
            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating profile: {ProfileId}", profile.Id);
            _telemetryClient.TrackException(ex, new Dictionary<string, string>
            {
                { "Repository", "ProfileRepository" },
                { "Method", "UpdateAsync" },
                { "ProfileId", profile.Id.ToString() }
            });
            throw;
        }
    }

    public async Task DeleteAsync(int id)
    {
        try
        {
            _logger.LogWarning("Deleting profile: {ProfileId}", id);
            var profile = await _context.Profiles.FindAsync(id);
            if (profile != null)
            {
                _context.Profiles.Remove(profile);
                _logger.LogInformation("Profile {ProfileId} marked for deletion", id);
            }
            else
            {
                _logger.LogWarning("Profile {ProfileId} not found for deletion", id);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting profile: {ProfileId}", id);
            _telemetryClient.TrackException(ex, new Dictionary<string, string>
            {
                { "Repository", "ProfileRepository" },
                { "Method", "DeleteAsync" },
                { "ProfileId", id.ToString() }
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
            _logger.LogError(ex, "Database update error in ProfileRepository");
            _telemetryClient.TrackException(ex, new Dictionary<string, string>
            {
                { "Repository", "ProfileRepository" },
                { "Method", "SaveChangesAsync" },
                { "ErrorType", "DbUpdateException" }
            });
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving changes in ProfileRepository");
            _telemetryClient.TrackException(ex, new Dictionary<string, string>
            {
                { "Repository", "ProfileRepository" },
                { "Method", "SaveChangesAsync" }
            });
            throw;
        }
    }
}
