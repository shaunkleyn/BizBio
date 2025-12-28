using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BizBio.Core.Interfaces;
using BizBio.Core.Entities;
using System.Security.Claims;

namespace BizBio.API.Controllers;

[Route("api/v1/restaurants")]
[ApiController]
[Authorize]
public class RestaurantsController : ControllerBase
{
    private readonly IRestaurantRepository _restaurantRepo;
    private readonly IProfileRepository _profileRepo;
    private readonly IUserSubscriptionRepository _subscriptionRepo;
    private readonly ILogger<RestaurantsController> _logger;
    private readonly TelemetryClient _telemetryClient;

    public RestaurantsController(
        IRestaurantRepository restaurantRepo,
        IProfileRepository profileRepo,
        IUserSubscriptionRepository subscriptionRepo,
        ILogger<RestaurantsController> logger,
        TelemetryClient telemetryClient)
    {
        _restaurantRepo = restaurantRepo;
        _profileRepo = profileRepo;
        _subscriptionRepo = subscriptionRepo;
        _logger = logger;
        _telemetryClient = telemetryClient;
    }

    private int GetUserId() => int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

    /// <summary>
    /// Get all restaurants for the authenticated user
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetMyRestaurants()
    {
        try
        {
            var userId = GetUserId();
            _logger.LogInformation("Fetching restaurants for user {UserId}", userId);

            var restaurants = await _restaurantRepo.GetByUserIdAsync(userId);

            _logger.LogInformation("Found {Count} restaurants for user {UserId}",
                restaurants.Count(), userId);

            var restaurantsWithProfileCount = restaurants.Select(r => new
            {
                r.Id,
                r.UserId,
                r.Name,
                r.Description,
                r.Logo,
                r.Address,
                r.City,
                r.State,
                r.PostalCode,
                r.Country,
                r.Phone,
                r.Email,
                r.Website,
                r.Currency,
                r.Timezone,
                r.SortOrder,
                r.IsActive,
                r.CreatedAt,
                r.UpdatedAt,
                ProfileCount = r.Profiles.Count
            }).ToList();

            return Ok(new { data = restaurantsWithProfileCount });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching restaurants");
            _telemetryClient.TrackException(ex, new Dictionary<string, string>
            {
                { "Controller", "RestaurantsController" },
                { "Action", "GetMyRestaurants" }
            });
            return StatusCode(500, new { message = "An error occurred while fetching restaurants" });
        }
    }

    /// <summary>
    /// Get a specific restaurant by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetRestaurant(int id)
    {
        try
        {
            var userId = GetUserId();
            _logger.LogInformation("Fetching restaurant {RestaurantId} for user {UserId}", id, userId);

            var restaurant = await _restaurantRepo.GetByIdAsync(id);

            if (restaurant == null || restaurant.UserId != userId)
            {
                _logger.LogWarning("Restaurant {RestaurantId} not found or unauthorized for user {UserId}",
                    id, userId);
                return NotFound(new { message = "Restaurant not found" });
            }

            _logger.LogInformation("Successfully fetched restaurant {RestaurantId}", id);

            return Ok(new { data = restaurant });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching restaurant {RestaurantId}", id);
            _telemetryClient.TrackException(ex, new Dictionary<string, string>
            {
                { "Controller", "RestaurantsController" },
                { "Action", "GetRestaurant" },
                { "RestaurantId", id.ToString() }
            });
            return StatusCode(500, new { message = "An error occurred while fetching the restaurant" });
        }
    }

    /// <summary>
    /// Get all profiles/menus for a specific restaurant
    /// </summary>
    [HttpGet("{id}/profiles")]
    public async Task<IActionResult> GetRestaurantProfiles(int id)
    {
        try
        {
            var userId = GetUserId();
            _logger.LogInformation("Fetching profiles for restaurant {RestaurantId}, user {UserId}",
                id, userId);

            var restaurant = await _restaurantRepo.GetByIdWithProfilesAsync(id);

            if (restaurant == null || restaurant.UserId != userId)
            {
                _logger.LogWarning("Restaurant {RestaurantId} not found or unauthorized for user {UserId}",
                    id, userId);
                return NotFound(new { message = "Restaurant not found" });
            }

            _logger.LogInformation("Found {Count} profiles for restaurant {RestaurantId}",
                restaurant.Profiles.Count, id);

            return Ok(new { data = restaurant.Profiles });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching profiles for restaurant {RestaurantId}", id);
            _telemetryClient.TrackException(ex, new Dictionary<string, string>
            {
                { "Controller", "RestaurantsController" },
                { "Action", "GetRestaurantProfiles" },
                { "RestaurantId", id.ToString() }
            });
            return StatusCode(500, new { message = "An error occurred while fetching restaurant profiles" });
        }
    }

    /// <summary>
    /// Create a new restaurant for the authenticated user
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> CreateRestaurant([FromBody] CreateRestaurantDto dto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Restaurant creation failed: Invalid model state");
                return BadRequest(ModelState);
            }

            var userId = GetUserId();
            _logger.LogInformation("Creating restaurant {Name} for user {UserId}", dto.Name, userId);

            // Check subscription limits
            var activeCount = await _restaurantRepo.GetRestaurantCountAsync(userId);
            var subscriptions = await _subscriptionRepo.GetActiveSubscriptionsAsync(userId);
            var activeSubscription = subscriptions.FirstOrDefault();

            if (activeSubscription != null)
            {
                var maxRestaurants = activeSubscription.Tier?.MaxRestaurants ?? 1;
                if (activeCount >= maxRestaurants)
                {
                    _logger.LogWarning("User {UserId} has reached restaurant limit ({Current}/{Max})",
                        userId, activeCount, maxRestaurants);
                    return BadRequest(new {
                        message = $"You have reached your restaurant limit ({maxRestaurants}). Please upgrade your subscription to add more restaurants."
                    });
                }
            }

            var restaurant = new Restaurant
            {
                UserId = userId,
                Name = dto.Name,
                Description = dto.Description,
                Logo = dto.Logo,
                Address = dto.Address,
                City = dto.City,
                State = dto.State,
                PostalCode = dto.PostalCode,
                Country = dto.Country,
                Phone = dto.Phone,
                Email = dto.Email,
                Website = dto.Website,
                Currency = dto.Currency ?? "ZAR",
                Timezone = dto.Timezone,
                SortOrder = dto.SortOrder ?? 0,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                CreatedBy = userId.ToString(),
                UpdatedBy = userId.ToString()
            };

            await _restaurantRepo.AddAsync(restaurant);
            await _restaurantRepo.SaveChangesAsync();

            _logger.LogInformation("Restaurant {RestaurantId} created successfully for user {UserId}",
                restaurant.Id, userId);
            _telemetryClient.TrackEvent("RestaurantCreated", new Dictionary<string, string>
            {
                { "RestaurantId", restaurant.Id.ToString() },
                { "UserId", userId.ToString() },
                { "Name", dto.Name }
            });

            return CreatedAtAction(nameof(GetRestaurant), new { id = restaurant.Id },
                new { data = restaurant });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating restaurant");
            _telemetryClient.TrackException(ex, new Dictionary<string, string>
            {
                { "Controller", "RestaurantsController" },
                { "Action", "CreateRestaurant" },
                { "Name", dto.Name }
            });
            return StatusCode(500, new { message = "An error occurred while creating the restaurant" });
        }
    }

    /// <summary>
    /// Update an existing restaurant
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateRestaurant(int id, [FromBody] UpdateRestaurantDto dto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Restaurant update failed: Invalid model state");
                return BadRequest(ModelState);
            }

            var userId = GetUserId();
            _logger.LogInformation("Updating restaurant {RestaurantId} for user {UserId}", id, userId);

            var restaurant = await _restaurantRepo.GetByIdAsync(id);

            if (restaurant == null || restaurant.UserId != userId)
            {
                _logger.LogWarning("Restaurant {RestaurantId} not found or unauthorized for user {UserId}",
                    id, userId);
                return NotFound(new { message = "Restaurant not found" });
            }

            // Update fields
            restaurant.Name = dto.Name;
            restaurant.Description = dto.Description;
            restaurant.Logo = dto.Logo;
            restaurant.Address = dto.Address;
            restaurant.City = dto.City;
            restaurant.State = dto.State;
            restaurant.PostalCode = dto.PostalCode;
            restaurant.Country = dto.Country;
            restaurant.Phone = dto.Phone;
            restaurant.Email = dto.Email;
            restaurant.Website = dto.Website;
            restaurant.Currency = dto.Currency ?? restaurant.Currency;
            restaurant.Timezone = dto.Timezone;
            restaurant.SortOrder = dto.SortOrder ?? restaurant.SortOrder;
            restaurant.IsActive = dto.IsActive ?? restaurant.IsActive;
            restaurant.UpdatedAt = DateTime.UtcNow;
            restaurant.UpdatedBy = userId.ToString();

            await _restaurantRepo.UpdateAsync(restaurant);
            await _restaurantRepo.SaveChangesAsync();

            _logger.LogInformation("Restaurant {RestaurantId} updated successfully", id);
            _telemetryClient.TrackEvent("RestaurantUpdated", new Dictionary<string, string>
            {
                { "RestaurantId", id.ToString() },
                { "UserId", userId.ToString() }
            });

            return Ok(new { data = restaurant });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating restaurant {RestaurantId}", id);
            _telemetryClient.TrackException(ex, new Dictionary<string, string>
            {
                { "Controller", "RestaurantsController" },
                { "Action", "UpdateRestaurant" },
                { "RestaurantId", id.ToString() }
            });
            return StatusCode(500, new { message = "An error occurred while updating the restaurant" });
        }
    }

    /// <summary>
    /// Delete a restaurant
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteRestaurant(int id)
    {
        try
        {
            var userId = GetUserId();
            _logger.LogInformation("Deleting restaurant {RestaurantId} for user {UserId}", id, userId);

            var restaurant = await _restaurantRepo.GetByIdAsync(id);

            if (restaurant == null || restaurant.UserId != userId)
            {
                _logger.LogWarning("Restaurant {RestaurantId} not found or unauthorized for user {UserId}",
                    id, userId);
                return NotFound(new { message = "Restaurant not found" });
            }

            await _restaurantRepo.DeleteAsync(id);
            await _restaurantRepo.SaveChangesAsync();

            _logger.LogInformation("Restaurant {RestaurantId} deleted successfully", id);
            _telemetryClient.TrackEvent("RestaurantDeleted", new Dictionary<string, string>
            {
                { "RestaurantId", id.ToString() },
                { "UserId", userId.ToString() }
            });

            return Ok(new { message = "Restaurant deleted successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting restaurant {RestaurantId}", id);
            _telemetryClient.TrackException(ex, new Dictionary<string, string>
            {
                { "Controller", "RestaurantsController" },
                { "Action", "DeleteRestaurant" },
                { "RestaurantId", id.ToString() }
            });
            return StatusCode(500, new { message = "An error occurred while deleting the restaurant" });
        }
    }
}

/// <summary>
/// Data transfer object for creating a new restaurant
/// </summary>
public class CreateRestaurantDto
{
    /// <summary>
    /// Name of the restaurant
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Description of the restaurant
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Logo URL for the restaurant
    /// </summary>
    public string? Logo { get; set; }

    /// <summary>
    /// Street address
    /// </summary>
    public string? Address { get; set; }

    /// <summary>
    /// City
    /// </summary>
    public string? City { get; set; }

    /// <summary>
    /// State/Province
    /// </summary>
    public string? State { get; set; }

    /// <summary>
    /// Postal/ZIP code
    /// </summary>
    public string? PostalCode { get; set; }

    /// <summary>
    /// Country code (e.g., "ZA", "US")
    /// </summary>
    public string? Country { get; set; }

    /// <summary>
    /// Contact phone number
    /// </summary>
    public string? Phone { get; set; }

    /// <summary>
    /// Contact email
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// Website URL
    /// </summary>
    public string? Website { get; set; }

    /// <summary>
    /// Currency code (defaults to "ZAR")
    /// </summary>
    public string? Currency { get; set; }

    /// <summary>
    /// Timezone (e.g., "Africa/Johannesburg")
    /// </summary>
    public string? Timezone { get; set; }

    /// <summary>
    /// Sort order for display
    /// </summary>
    public int? SortOrder { get; set; }
}

/// <summary>
/// Data transfer object for updating an existing restaurant
/// </summary>
public class UpdateRestaurantDto
{
    /// <summary>
    /// Name of the restaurant
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Description of the restaurant
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Logo URL for the restaurant
    /// </summary>
    public string? Logo { get; set; }

    /// <summary>
    /// Street address
    /// </summary>
    public string? Address { get; set; }

    /// <summary>
    /// City
    /// </summary>
    public string? City { get; set; }

    /// <summary>
    /// State/Province
    /// </summary>
    public string? State { get; set; }

    /// <summary>
    /// Postal/ZIP code
    /// </summary>
    public string? PostalCode { get; set; }

    /// <summary>
    /// Country code (e.g., "ZA", "US")
    /// </summary>
    public string? Country { get; set; }

    /// <summary>
    /// Contact phone number
    /// </summary>
    public string? Phone { get; set; }

    /// <summary>
    /// Contact email
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// Website URL
    /// </summary>
    public string? Website { get; set; }

    /// <summary>
    /// Currency code
    /// </summary>
    public string? Currency { get; set; }

    /// <summary>
    /// Timezone
    /// </summary>
    public string? Timezone { get; set; }

    /// <summary>
    /// Sort order for display
    /// </summary>
    public int? SortOrder { get; set; }

    /// <summary>
    /// Active status
    /// </summary>
    public bool? IsActive { get; set; }
}
