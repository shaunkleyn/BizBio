using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BizBio.Core.Interfaces;
using BizBio.Core.Entities;
using BizBio.Infrastructure.Data;
using System.Security.Claims;

namespace BizBio.API.Controllers;

[Route("api/v1/dashboard/catalogs/{catalogId}/bundles")]
[ApiController]
[Authorize]
public class BundlesController : ControllerBase
{
    private readonly ICatalogBundleRepository _bundleRepo;
    private readonly ICatalogRepository _catalogRepo;
    private readonly IProfileRepository _profileRepo;
    private readonly IUserSubscriptionRepository _subscriptionRepo;
    private readonly ApplicationDbContext _context;

    public BundlesController(
        ICatalogBundleRepository bundleRepo,
        ICatalogRepository catalogRepo,
        IProfileRepository profileRepo,
        IUserSubscriptionRepository subscriptionRepo,
        ApplicationDbContext context)
    {
        _bundleRepo = bundleRepo;
        _catalogRepo = catalogRepo;
        _profileRepo = profileRepo;
        _subscriptionRepo = subscriptionRepo;
        _context = context;
    }

    private int GetUserId() => int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

    private async Task<bool> HasBundleFeature(int userId)
    {
        var subscriptions = await _subscriptionRepo.GetActiveSubscriptionsAsync(userId);
        var menuSubscription = subscriptions.FirstOrDefault(s => s.Tier?.ProductLineId == 1); // 1 = Menu product line
        return menuSubscription?.Tier?.Bundles ?? false;
    }

    /// <summary>
    /// Get all bundles for a catalog
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetBundles(int catalogId)
    {
        var userId = GetUserId();
        var catalog = await _catalogRepo.GetByIdAsync(catalogId);

        if (catalog == null)
            return NotFound(new { success = false, error = "Catalog not found" });

        var profile = await _profileRepo.GetByIdAsync(catalog.ProfileId);
        if (profile == null || profile.UserId != userId)
            return Forbid();

        var bundles = await _bundleRepo.GetByCatalogIdAsync(catalogId);

        return Ok(new { success = true, data = new { bundles } });
    }

    /// <summary>
    /// Get a specific bundle by ID
    /// </summary>
    [HttpGet("{bundleId}")]
    public async Task<IActionResult> GetBundle(int catalogId, int bundleId)
    {
        var userId = GetUserId();
        var catalog = await _catalogRepo.GetByIdAsync(catalogId);

        if (catalog == null)
            return NotFound(new { success = false, error = "Catalog not found" });

        var profile = await _profileRepo.GetByIdAsync(catalog.ProfileId);
        if (profile == null || profile.UserId != userId)
            return Forbid();

        var bundle = await _bundleRepo.GetByIdWithDetailsAsync(bundleId);

        if (bundle == null || bundle.CatalogId != catalogId)
            return NotFound(new { success = false, error = "Bundle not found" });

        return Ok(new { success = true, data = new { bundle } });
    }

    /// <summary>
    /// Create a new bundle
    /// Requires Bundles feature in subscription tier
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> CreateBundle(int catalogId, [FromBody] CreateBundleDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var userId = GetUserId();

        // Check if user has bundle feature
        if (!await HasBundleFeature(userId))
        {
            return BadRequest(new
            {
                success = false,
                error = "Bundles feature is not available in your subscription tier. Please upgrade to access this feature."
            });
        }

        var catalog = await _catalogRepo.GetByIdAsync(catalogId);
        if (catalog == null)
            return NotFound(new { success = false, error = "Catalog not found" });

        var profile = await _profileRepo.GetByIdAsync(catalog.ProfileId);
        if (profile == null || profile.UserId != userId)
            return Forbid();

        var bundle = new CatalogBundle
        {
            CatalogId = catalogId,
            Name = dto.Name,
            Slug = dto.Slug,
            Description = dto.Description,
            BasePrice = dto.BasePrice,
            Images = dto.Images,
            SortOrder = dto.SortOrder,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            CreatedBy = userId.ToString(),
            UpdatedBy = userId.ToString()
        };

        await _bundleRepo.AddAsync(bundle);
        await _bundleRepo.SaveChangesAsync();

        return Ok(new { success = true, data = new { bundle } });
    }

    /// <summary>
    /// Update a bundle
    /// </summary>
    [HttpPut("{bundleId}")]
    public async Task<IActionResult> UpdateBundle(int catalogId, int bundleId, [FromBody] UpdateBundleDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var userId = GetUserId();
        var catalog = await _catalogRepo.GetByIdAsync(catalogId);

        if (catalog == null)
            return NotFound(new { success = false, error = "Catalog not found" });

        var profile = await _profileRepo.GetByIdAsync(catalog.ProfileId);
        if (profile == null || profile.UserId != userId)
            return Forbid();

        var bundle = await _bundleRepo.GetByIdAsync(bundleId);

        if (bundle == null || bundle.CatalogId != catalogId)
            return NotFound(new { success = false, error = "Bundle not found" });

        if (!string.IsNullOrEmpty(dto.Name))
            bundle.Name = dto.Name;

        if (dto.Slug != null)
            bundle.Slug = dto.Slug;

        if (dto.Description != null)
            bundle.Description = dto.Description;

        if (dto.BasePrice.HasValue)
            bundle.BasePrice = dto.BasePrice.Value;

        if (dto.Images != null)
            bundle.Images = dto.Images;

        if (dto.SortOrder.HasValue)
            bundle.SortOrder = dto.SortOrder.Value;

        if (dto.IsActive.HasValue)
            bundle.IsActive = dto.IsActive.Value;

        bundle.UpdatedAt = DateTime.UtcNow;
        bundle.UpdatedBy = userId.ToString();

        await _bundleRepo.UpdateAsync(bundle);
        await _bundleRepo.SaveChangesAsync();

        return Ok(new { success = true, data = new { bundle } });
    }

    /// <summary>
    /// Delete a bundle
    /// </summary>
    [HttpDelete("{bundleId}")]
    public async Task<IActionResult> DeleteBundle(int catalogId, int bundleId)
    {
        var userId = GetUserId();
        var catalog = await _catalogRepo.GetByIdAsync(catalogId);

        if (catalog == null)
            return NotFound(new { success = false, error = "Catalog not found" });

        var profile = await _profileRepo.GetByIdAsync(catalog.ProfileId);
        if (profile == null || profile.UserId != userId)
            return Forbid();

        var bundle = await _bundleRepo.GetByIdAsync(bundleId);

        if (bundle == null || bundle.CatalogId != catalogId)
            return NotFound(new { success = false, error = "Bundle not found" });

        await _bundleRepo.DeleteAsync(bundle.Id);
        await _bundleRepo.SaveChangesAsync();

        return Ok(new { success = true, message = "Bundle deleted successfully" });
    }

    /// <summary>
    /// Add a step to a bundle
    /// </summary>
    [HttpPost("{bundleId}/steps")]
    public async Task<IActionResult> AddStep(int catalogId, int bundleId, [FromBody] CreateBundleStepDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var userId = GetUserId();
        var catalog = await _catalogRepo.GetByIdAsync(catalogId);

        if (catalog == null)
            return NotFound(new { success = false, error = "Catalog not found" });

        var profile = await _profileRepo.GetByIdAsync(catalog.ProfileId);
        if (profile == null || profile.UserId != userId)
            return Forbid();

        var bundle = await _bundleRepo.GetByIdAsync(bundleId);

        if (bundle == null || bundle.CatalogId != catalogId)
            return NotFound(new { success = false, error = "Bundle not found" });

        var step = new CatalogBundleStep
        {
            BundleId = bundleId,
            StepNumber = dto.StepNumber,
            Name = dto.Name,
            MinSelect = dto.MinSelect,
            MaxSelect = dto.MaxSelect,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            CreatedBy = userId.ToString(),
            UpdatedBy = userId.ToString()
        };

        await _bundleRepo.AddStepAsync(step);
        await _bundleRepo.SaveChangesAsync();

        return Ok(new { success = true, data = new { step } });
    }

    /// <summary>
    /// Add a product to a bundle step
    /// </summary>
    [HttpPost("{bundleId}/steps/{stepId}/products")]
    public async Task<IActionResult> AddProductToStep(int catalogId, int bundleId, int stepId, [FromBody] AddProductToStepDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var userId = GetUserId();
        var catalog = await _catalogRepo.GetByIdAsync(catalogId);

        if (catalog == null)
            return NotFound(new { success = false, error = "Catalog not found" });

        var profile = await _profileRepo.GetByIdAsync(catalog.ProfileId);
        if (profile == null || profile.UserId != userId)
            return Forbid();

        var bundle = await _bundleRepo.GetByIdAsync(bundleId);

        if (bundle == null || bundle.CatalogId != catalogId)
            return NotFound(new { success = false, error = "Bundle not found" });

        var stepProduct = new CatalogBundleStepProduct
        {
            StepId = stepId,
            ProductId = dto.ProductId,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            CreatedBy = userId.ToString(),
            UpdatedBy = userId.ToString()
        };

        await _bundleRepo.AddStepProductAsync(stepProduct);
        await _bundleRepo.SaveChangesAsync();

        return Ok(new { success = true, data = new { stepProduct } });
    }

    /// <summary>
    /// Add an option group to a bundle step
    /// </summary>
    [HttpPost("{bundleId}/steps/{stepId}/option-groups")]
    public async Task<IActionResult> AddOptionGroup(int catalogId, int bundleId, int stepId, [FromBody] CreateBundleOptionGroupDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var userId = GetUserId();
        var catalog = await _catalogRepo.GetByIdAsync(catalogId);

        if (catalog == null)
            return NotFound(new { success = false, error = "Catalog not found" });

        var profile = await _profileRepo.GetByIdAsync(catalog.ProfileId);
        if (profile == null || profile.UserId != userId)
            return Forbid();

        var bundle = await _bundleRepo.GetByIdAsync(bundleId);

        if (bundle == null || bundle.CatalogId != catalogId)
            return NotFound(new { success = false, error = "Bundle not found" });

        var optionGroup = new CatalogBundleOptionGroup
        {
            StepId = stepId,
            Name = dto.Name,
            IsRequired = dto.IsRequired,
            MinSelect = dto.MinSelect,
            MaxSelect = dto.MaxSelect,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            CreatedBy = userId.ToString(),
            UpdatedBy = userId.ToString()
        };

        await _bundleRepo.AddOptionGroupAsync(optionGroup);
        await _bundleRepo.SaveChangesAsync();

        return Ok(new { success = true, data = new { optionGroup } });
    }

    /// <summary>
    /// Add bundle to a menu category
    /// Creates a CatalogItem reference to the bundle
    /// </summary>
    [HttpPost("{bundleId}/add-to-category")]
    public async Task<IActionResult> AddBundleToCategory(int catalogId, int bundleId, [FromBody] AddBundleToCategoryDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var userId = GetUserId();
        var catalog = await _catalogRepo.GetByIdAsync(catalogId);

        if (catalog == null)
            return NotFound(new { success = false, error = "Catalog not found" });

        var profile = await _profileRepo.GetByIdAsync(catalog.ProfileId);
        if (profile == null || profile.UserId != userId)
            return Forbid();

        var bundle = await _bundleRepo.GetByIdAsync(bundleId);
        if (bundle == null || bundle.CatalogId != catalogId)
            return NotFound(new { success = false, error = "Bundle not found" });

        // Create a CatalogItem that references the bundle
        var catalogItem = new CatalogItem
        {
            CatalogId = catalogId,
            CategoryId = dto.CategoryId,
            ItemType = CatalogItemType.Bundle,
            BundleId = bundleId,
            Name = bundle.Name,
            Description = bundle.Description,
            Price = bundle.BasePrice,
            Images = bundle.Images,
            SortOrder = dto.SortOrder,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            CreatedBy = userId.ToString(),
            UpdatedBy = userId.ToString()
        };

        _context.CatalogItems.Add(catalogItem);
        await _context.SaveChangesAsync();

        return Ok(new { success = true, data = new { catalogItem } });
    }

    /// <summary>
    /// Add an option to an option group
    /// </summary>
    [HttpPost("{bundleId}/option-groups/{optionGroupId}/options")]
    public async Task<IActionResult> AddOption(int catalogId, int bundleId, int optionGroupId, [FromBody] CreateBundleOptionDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var userId = GetUserId();
        var catalog = await _catalogRepo.GetByIdAsync(catalogId);

        if (catalog == null)
            return NotFound(new { success = false, error = "Catalog not found" });

        var profile = await _profileRepo.GetByIdAsync(catalog.ProfileId);
        if (profile == null || profile.UserId != userId)
            return Forbid();

        var bundle = await _bundleRepo.GetByIdAsync(bundleId);

        if (bundle == null || bundle.CatalogId != catalogId)
            return NotFound(new { success = false, error = "Bundle not found" });

        var option = new CatalogBundleOption
        {
            OptionGroupId = optionGroupId,
            Name = dto.Name,
            PriceModifier = dto.PriceModifier,
            IsDefault = dto.IsDefault,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            CreatedBy = userId.ToString(),
            UpdatedBy = userId.ToString()
        };

        await _bundleRepo.AddOptionAsync(option);
        await _bundleRepo.SaveChangesAsync();

        return Ok(new { success = true, data = new { option } });
    }
}

/// <summary>
/// Data transfer object for creating a new bundle
/// </summary>
public class CreateBundleDto
{
    public string Name { get; set; } = null!;
    public string? Slug { get; set; }
    public string? Description { get; set; }
    public decimal BasePrice { get; set; }
    public string? Images { get; set; }
    public int SortOrder { get; set; } = 0;
}

/// <summary>
/// Data transfer object for updating a bundle
/// </summary>
public class UpdateBundleDto
{
    public string? Name { get; set; }
    public string? Slug { get; set; }
    public string? Description { get; set; }
    public decimal? BasePrice { get; set; }
    public string? Images { get; set; }
    public int? SortOrder { get; set; }
    public bool? IsActive { get; set; }
}

/// <summary>
/// Data transfer object for creating a bundle step
/// </summary>
public class CreateBundleStepDto
{
    public int StepNumber { get; set; }
    public string Name { get; set; } = null!;
    public int MinSelect { get; set; } = 1;
    public int MaxSelect { get; set; } = 1;
}

/// <summary>
/// Data transfer object for adding a product to a step
/// </summary>
public class AddProductToStepDto
{
    public int ProductId { get; set; }
}

/// <summary>
/// Data transfer object for creating an option group
/// </summary>
public class CreateBundleOptionGroupDto
{
    public string Name { get; set; } = null!;
    public bool IsRequired { get; set; } = false;
    public int MinSelect { get; set; } = 0;
    public int MaxSelect { get; set; } = 10;
}

/// <summary>
/// Data transfer object for creating an option
/// </summary>
public class CreateBundleOptionDto
{
    public string Name { get; set; } = null!;
    public decimal PriceModifier { get; set; } = 0M;
    public bool IsDefault { get; set; } = false;
}

/// <summary>
/// Data transfer object for adding a bundle to a category
/// </summary>
public class AddBundleToCategoryDto
{
    public int? CategoryId { get; set; }
    public int SortOrder { get; set; } = 0;
}
