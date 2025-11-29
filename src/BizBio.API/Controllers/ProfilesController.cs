using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BizBio.Core.Interfaces;
using BizBio.Core.Entities;
using System.Security.Claims;

namespace BizBio.API.Controllers;

[Route("api/v1/profiles")]
[ApiController]
[Authorize]
public class ProfilesController : ControllerBase
{
    private readonly IProfileRepository _profileRepo;
    private readonly ICatalogRepository _catalogRepo;

    public ProfilesController(IProfileRepository profileRepo, ICatalogRepository catalogRepo)
    {
        _profileRepo = profileRepo;
        _catalogRepo = catalogRepo;
    }

    private int GetUserId() => int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

    /// <summary>
    /// Get all profiles for the authenticated user
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetMyProfiles()
    {
        var userId = GetUserId();
        var profiles = await _profileRepo.GetByUserIdAsync(userId);

        return Ok(new { data = profiles });
    }

    /// <summary>
    /// Get a specific profile by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetProfile(int id)
    {
        var userId = GetUserId();
        var profile = await _profileRepo.GetByIdAsync(id);

        if (profile == null || profile.UserId != userId)
            return NotFound();

        return Ok(new { data = profile });
    }

    /// <summary>
    /// Create a new profile for the authenticated user
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> CreateProfile([FromBody] CreateProfileDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var userId = GetUserId();

        // Validate slug uniqueness
        var existingProfile = await _profileRepo.GetBySlugAsync(dto.Slug);
        if (existingProfile != null)
            return BadRequest(new { message = "Profile slug already exists" });

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

        return CreatedAtAction(nameof(GetProfile), new { id = profile.Id }, new { data = profile });
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
