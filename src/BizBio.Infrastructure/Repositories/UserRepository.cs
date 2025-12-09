using Microsoft.ApplicationInsights;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using BizBio.Core.Entities;
using BizBio.Core.Interfaces;
using BizBio.Infrastructure.Data;

namespace BizBio.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<UserRepository> _logger;
    private readonly TelemetryClient _telemetryClient;

    public UserRepository(
        ApplicationDbContext context,
        ILogger<UserRepository> logger,
        TelemetryClient telemetryClient)
    {
        _context = context;
        _logger = logger;
        _telemetryClient = telemetryClient;
    }

    public async Task<User?> GetByIdAsync(int id)
    {
        try
        {
            _logger.LogDebug("Fetching user by ID: {UserId}", id);

            var user = await _context.Users
                .Include(u => u.Subscriptions)
                .ThenInclude(s => s.Tier)
                .Include(u => u.Profiles)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
            {
                _logger.LogDebug("User {UserId} not found", id);
            }

            return user;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching user by ID: {UserId}", id);
            _telemetryClient.TrackException(ex, new Dictionary<string, string>
            {
                { "Repository", "UserRepository" },
                { "Method", "GetByIdAsync" },
                { "UserId", id.ToString() }
            });
            throw;
        }
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        try
        {
            _logger.LogDebug("Fetching user by email: {Email}", email);

            var user = await _context.Users
                .Include(u => u.Subscriptions)
                .ThenInclude(s => s.Tier)
                .Include(u => u.Profiles)
                .FirstOrDefaultAsync(u => u.Email == email);

            if (user == null)
            {
                _logger.LogDebug("User with email {Email} not found", email);
            }

            return user;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching user by email: {Email}", email);
            _telemetryClient.TrackException(ex, new Dictionary<string, string>
            {
                { "Repository", "UserRepository" },
                { "Method", "GetByEmailAsync" },
                { "Email", email }
            });
            throw;
        }
    }

    public async Task<User?> GetByEmailVerificationTokenAsync(string token)
    {
        return await _context.Users
            .Include(u => u.Subscriptions)
            .ThenInclude(s => s.Tier)
            .FirstOrDefaultAsync(u => u.EmailVerificationToken == token);
    }

    public async Task<User?> GetByPasswordResetTokenAsync(string token)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.PasswordResetToken == token);
    }

    public async Task<IEnumerable<User>> GetAllAsync()
    {
        return await _context.Users
            .Include(u => u.Subscriptions)
            .ThenInclude(s => s.Tier)
            .Include(u => u.Profiles)
            .ToListAsync();
    }

    public async Task AddAsync(User user)
    {
        try
        {
            _logger.LogInformation("Adding new user: {Email}", user.Email);
            await _context.Users.AddAsync(user);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding user: {Email}", user.Email);
            _telemetryClient.TrackException(ex, new Dictionary<string, string>
            {
                { "Repository", "UserRepository" },
                { "Method", "AddAsync" },
                { "Email", user.Email }
            });
            throw;
        }
    }

    public Task UpdateAsync(User user)
    {
        try
        {
            _logger.LogInformation("Updating user: {UserId}", user.Id);
            _context.Users.Update(user);
            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating user: {UserId}", user.Id);
            _telemetryClient.TrackException(ex, new Dictionary<string, string>
            {
                { "Repository", "UserRepository" },
                { "Method", "UpdateAsync" },
                { "UserId", user.Id.ToString() }
            });
            throw;
        }
    }

    public async Task DeleteAsync(int id)
    {
        try
        {
            _logger.LogWarning("Deleting user: {UserId}", id);
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                _context.Users.Remove(user);
                _logger.LogInformation("User {UserId} marked for deletion", id);
            }
            else
            {
                _logger.LogWarning("User {UserId} not found for deletion", id);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting user: {UserId}", id);
            _telemetryClient.TrackException(ex, new Dictionary<string, string>
            {
                { "Repository", "UserRepository" },
                { "Method", "DeleteAsync" },
                { "UserId", id.ToString() }
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
            _logger.LogError(ex, "Database update error in UserRepository");
            _telemetryClient.TrackException(ex, new Dictionary<string, string>
            {
                { "Repository", "UserRepository" },
                { "Method", "SaveChangesAsync" },
                { "ErrorType", "DbUpdateException" }
            });
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving changes in UserRepository");
            _telemetryClient.TrackException(ex, new Dictionary<string, string>
            {
                { "Repository", "UserRepository" },
                { "Method", "SaveChangesAsync" }
            });
            throw;
        }
    }
}
