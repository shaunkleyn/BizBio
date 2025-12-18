using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BizBio.Core.Interfaces;
using BizBio.Core.Entities;
using System.Security.Claims;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BizBio.API.Controllers;

[Route("api/v1/profiles")]
[ApiController]
[Authorize]
public class ProfilesController : ControllerBase
{
    private readonly IProfileRepository _profileRepo;
    private readonly ICatalogRepository _catalogRepo;
    private readonly IUserSubscriptionRepository _subscriptionRepo;
    private readonly ILogger<ProfilesController> _logger;
    private readonly TelemetryClient _telemetryClient;

    public ProfilesController(
        IProfileRepository profileRepo,
        ICatalogRepository catalogRepo,
        IUserSubscriptionRepository subscriptionRepo,
        ILogger<ProfilesController> logger,
        TelemetryClient telemetryClient)
    {
        _profileRepo = profileRepo;
        _catalogRepo = catalogRepo;
        _subscriptionRepo = subscriptionRepo;
        _logger = logger;
        _telemetryClient = telemetryClient;
    }

    private int GetUserId() => int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

    /// <summary>
    /// Get all profiles for the authenticated user with subscription status
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetMyProfiles()
    {
        try
        {
            var userId = GetUserId();
            _logger.LogInformation("Fetching profiles for user {UserId}", userId);

            var profiles = await _profileRepo.GetByUserIdAsync(userId);
            var subscriptions = await _subscriptionRepo.GetActiveSubscriptionsAsync(userId);

            _logger.LogInformation("Found {Count} profiles and {SubCount} subscriptions for user {UserId}",
                profiles.Count(), subscriptions.Count(), userId);

            // Get the most recent active subscription (assuming one active subscription per user)
            var activeSubscription = subscriptions.FirstOrDefault();

            // Build response with subscription status
            var profilesWithStatus = profiles.Select(profile => new
            {
                profile.Id,
                profile.UserId,
                profile.Slug,
                profile.Name,
                profile.Description,
                profile.ProfileType,
                profile.Logo,
                profile.ContactPhone,
                profile.ContactEmail,
                profile.Website,
                profile.IsActive,
                profile.CreatedAt,
                profile.UpdatedAt,
                SubscriptionStatus = activeSubscription != null ? new
                {
                    IsInTrial = activeSubscription.StatusId == (int)Core.Enums.SubscriptionStatus.Trial,
                    TrialEndsAt = activeSubscription.TrialEndsAt,
                    TrialDaysRemaining = activeSubscription.TrialEndsAt.HasValue
                        ? Math.Max(0, (int)(activeSubscription.TrialEndsAt.Value - DateTime.UtcNow).TotalDays)
                        : (int?)null,
                    Status = ((Core.Enums.SubscriptionStatus)activeSubscription.StatusId).ToString(),
                    StatusId = activeSubscription.StatusId,
                    NextBillingDate = activeSubscription.NextBillingDate,
                    NeedsPayment = activeSubscription.StatusId == (int)Core.Enums.SubscriptionStatus.Expired ||
                                   (activeSubscription.StatusId == (int)Core.Enums.SubscriptionStatus.Trial &&
                                    activeSubscription.TrialEndsAt.HasValue &&
                                    activeSubscription.TrialEndsAt.Value <= DateTime.UtcNow),
                    TierName = activeSubscription.Tier?.DisplayName,
                    MaxProfiles = activeSubscription.CustomMaxProfiles ?? activeSubscription.Tier?.MaxProfiles,
                    MaxCatalogItems = activeSubscription.CustomMaxCatalogItems ?? activeSubscription.Tier?.MaxCatalogItems
                } : null
            }).ToList();

            // Return profiles with subscription status
            return Ok(new { data = profilesWithStatus });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching profiles");
            _telemetryClient.TrackException(ex, new Dictionary<string, string>
            {
                { "Controller", "ProfilesController" },
                { "Action", "GetMyProfiles" }
            });
            return StatusCode(500, new { message = "An error occurred while fetching profiles" });
        }
    }

    /// <summary>
    /// Get a specific profile by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetProfile(int id)
    {
        try
        {
            var userId = GetUserId();
            _logger.LogInformation("Fetching profile {ProfileId} for user {UserId}", id, userId);

            var profile = await _profileRepo.GetByIdAsync(id);

            if (profile == null || profile.UserId != userId)
            {
                _logger.LogWarning("Profile {ProfileId} not found or unauthorized for user {UserId}", id, userId);
                return NotFound();
            }

            _logger.LogInformation("Successfully fetched profile {ProfileId}", id);

            return Ok(new { data = profile });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching profile {ProfileId}", id);
            _telemetryClient.TrackException(ex, new Dictionary<string, string>
            {
                { "Controller", "ProfilesController" },
                { "Action", "GetProfile" },
                { "ProfileId", id.ToString() }
            });
            return StatusCode(500, new { message = "An error occurred while fetching the profile" });
        }
    }

    /// <summary>
    /// Create a new profile for the authenticated user
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> CreateProfile([FromBody] CreateProfileDto dto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Profile creation failed: Invalid model state");
                return BadRequest(ModelState);
            }

            var userId = GetUserId();
            _logger.LogInformation("Creating profile with slug {Slug} for user {UserId}", dto.Slug, userId);

            // Validate slug uniqueness
            var existingProfile = await _profileRepo.GetBySlugAsync(dto.Slug);
            if (existingProfile != null)
            {
                _logger.LogWarning("Profile creation failed: Slug {Slug} already exists", dto.Slug);
                return BadRequest(new { message = "Profile slug already exists" });
            }

            var profile = new Profile
            {
                UserId = userId,
                Slug = dto.Slug,
                Name = dto.Name,
                Description = dto.Description ?? string.Empty,
                ProfileType = dto.ProfileType,
                ContactPhone = dto.ContactPhone,
                ContactEmail = dto.ContactEmail,
                Website = dto.Website,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _profileRepo.AddAsync(profile);
            await _profileRepo.SaveChangesAsync();

            // Create default catalog
            var catalog = new Catalog
            {
                ProfileId = profile.Id,
                Name = "Main Catalog",
                Description = "Default catalog",
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _catalogRepo.AddAsync(catalog);
            await _catalogRepo.SaveChangesAsync();

            _logger.LogInformation("Profile {ProfileId} created successfully for user {UserId}", profile.Id, userId);
            _telemetryClient.TrackEvent("ProfileCreated", new Dictionary<string, string>
            {
                { "ProfileId", profile.Id.ToString() },
                { "UserId", userId.ToString() },
                { "ProfileType", dto.ProfileType }
            });

            return CreatedAtAction(nameof(GetProfile), new { id = profile.Id }, new { data = profile });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating profile");
            _telemetryClient.TrackException(ex, new Dictionary<string, string>
            {
                { "Controller", "ProfilesController" },
                { "Action", "CreateProfile" },
                { "Slug", dto.Slug }
            });
            return StatusCode(500, new { message = "An error occurred while creating the profile" });
        }
    }
}

/// <summary>
/// Data transfer object for creating a new profile
/// </summary>
public class CreateProfileDto
{
    /// <summary>
    /// Unique slug for the profile URL
    /// </summary>
    public string Slug { get; set; } = string.Empty;

    /// <summary>
    /// Display name of the profile
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Description of the profile
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Type of profile (e.g., "Professional", "Restaurant", "Business")
    /// </summary>
    public string ProfileType { get; set; } = "Connect";

    /// <summary>
    /// Contact phone number
    /// </summary>
    public string? ContactPhone { get; set; }

    /// <summary>
    /// Contact email address
    /// </summary>
    public string? ContactEmail { get; set; }

    /// <summary>
    /// Website URL
    /// </summary>
    public string? Website { get; set; }
}
