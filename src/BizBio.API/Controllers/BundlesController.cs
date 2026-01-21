using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BizBio.Core.Interfaces;
using BizBio.Core.Entities;
using BizBio.Infrastructure.Data;
using System.Security.Claims;

namespace BizBio.API.Controllers;

[Route("api/v1/dashboard/bundles")]
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
        var menuSubscription = subscriptions.FirstOrDefault(s => s.Tier.ProductLine.Name.Equals("menu", StringComparison.OrdinalIgnoreCase)); // 1 = Menu product line
        return menuSubscription?.Tier?.Bundles ?? false;
    }

    /// <summary>
    /// Get all bundles for the current user (from library items)
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetBundles()
    {
        var userId = GetUserId();

        // Get bundles from CatalogItems where ItemType = Bundle and UserId matches
        var bundleItems = await _context.CatalogItems
            .Where(i => i.UserId == userId && i.ItemType == CatalogItemType.Bundle && i.BundleId != null)
            .Select(i => i.BundleId!.Value)
            .Distinct()
            .ToListAsync();

        var bundles = new List<CatalogBundle>();
        foreach (var bundleId in bundleItems)
        {
            var bundle = await _bundleRepo.GetByIdAsync(bundleId);
            if (bundle != null)
            {
                bundles.Add(bundle);
            }
        }

        return Ok(new { success = true, data = new { bundles } });
    }

    /// <summary>
    /// Get a specific bundle by ID
    /// </summary>
    [HttpGet("{bundleId}")]
    public async Task<IActionResult> GetBundle(int bundleId)
    {
        var userId = GetUserId();

        var bundle = await _bundleRepo.GetByIdWithDetailsAsync(bundleId);

        if (bundle == null)
            return NotFound(new { success = false, error = "Bundle not found" });

        // Verify ownership through CatalogItems
        var hasAccess = await _context.CatalogItems
            .AnyAsync(i => i.BundleId == bundleId && i.UserId == userId);

        if (!hasAccess)
            return Forbid();

        return Ok(new { success = true, data = new { bundle } });
    }

    /// <summary>
    /// Create a new bundle
    /// Requires Bundles feature in subscription tier
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> CreateBundle([FromBody] CreateBundleDto dto)
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

        var bundle = new CatalogBundle
        {
            CatalogId = 0, // Not linked to a catalog directly
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
            UpdatedBy = userId.ToString(),
            Steps = new List<CatalogBundleStep>()
        };

        await _bundleRepo.AddAsync(bundle);
        await _bundleRepo.SaveChangesAsync();

        // Create a library item (CatalogItem) so the bundle appears in library items list
        var libraryItem = new CatalogItem
        {
            UserId = userId,
            CatalogId = null, // Library item, not assigned to catalog yet
            ItemType = CatalogItemType.Bundle,
            BundleId = bundle.Id,
            Name = bundle.Name,
            Description = bundle.Description,
            Price = bundle.BasePrice,
            Images = bundle.Images,
            SortOrder = bundle.SortOrder,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            CreatedBy = userId.ToString(),
            UpdatedBy = userId.ToString()
        };

        _context.CatalogItems.Add(libraryItem);
        await _context.SaveChangesAsync();

        return Ok(new { success = true, data = new { bundle, libraryItemId = libraryItem.Id } });
    }

    /// <summary>
    /// Update a bundle
    /// </summary>
    [HttpPut("{bundleId}")]
    public async Task<IActionResult> UpdateBundle(int bundleId, [FromBody] UpdateBundleDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var userId = GetUserId();

        var bundle = await _bundleRepo.GetByIdAsync(bundleId);

        if (bundle == null)
            return NotFound(new { success = false, error = "Bundle not found" });

        // Verify ownership through CatalogItems
        var hasAccess = await _context.CatalogItems
            .AnyAsync(i => i.BundleId == bundleId && i.UserId == userId);

        if (!hasAccess)
            return Forbid();

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
    public async Task<IActionResult> DeleteBundle(int bundleId)
    {
        var userId = GetUserId();

        var bundle = await _bundleRepo.GetByIdAsync(bundleId);

        if (bundle == null)
            return NotFound(new { success = false, error = "Bundle not found" });

        // Verify ownership through CatalogItems
        var hasAccess = await _context.CatalogItems
            .AnyAsync(i => i.BundleId == bundleId && i.UserId == userId);

        if (!hasAccess)
            return Forbid();

        await _bundleRepo.DeleteAsync(bundle.Id);
        await _bundleRepo.SaveChangesAsync();

        return Ok(new { success = true, message = "Bundle deleted successfully" });
    }

    /// <summary>
    /// Add a step to a bundle
    /// </summary>
    [HttpPost("{bundleId}/steps")]
    public async Task<IActionResult> AddStep(int bundleId, [FromBody] CreateBundleStepDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var userId = GetUserId();

        var bundle = await _bundleRepo.GetByIdAsync(bundleId);

        if (bundle == null)
            return NotFound(new { success = false, error = "Bundle not found" });

        // Verify ownership through CatalogItems
        var hasAccess = await _context.CatalogItems
            .AnyAsync(i => i.BundleId == bundleId && i.UserId == userId);

        if (!hasAccess)
            return Forbid();

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
    public async Task<IActionResult> AddProductToStep(int bundleId, int stepId, [FromBody] AddProductToStepDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var userId = GetUserId();

        var bundle = await _bundleRepo.GetByIdAsync(bundleId);

        if (bundle == null)
            return NotFound(new { success = false, error = "Bundle not found" });

        // Verify ownership through CatalogItems
        var hasAccess = await _context.CatalogItems
            .AnyAsync(i => i.BundleId == bundleId && i.UserId == userId);

        if (!hasAccess)
            return Forbid();

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
    public async Task<IActionResult> AddOptionGroup(int bundleId, int stepId, [FromBody] CreateBundleOptionGroupDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var userId = GetUserId();

        var bundle = await _bundleRepo.GetByIdAsync(bundleId);

        if (bundle == null)
            return NotFound(new { success = false, error = "Bundle not found" });

        // Verify ownership through CatalogItems
        var hasAccess = await _context.CatalogItems
            .AnyAsync(i => i.BundleId == bundleId && i.UserId == userId);

        if (!hasAccess)
            return Forbid();

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
    public async Task<IActionResult> AddBundleToCategory(int bundleId, [FromBody] AddBundleToCategoryDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var userId = GetUserId();

        // Verify the catalog exists and user has access
        var catalog = await _catalogRepo.GetByIdAsync(dto.CatalogId);

        if (catalog == null)
            return NotFound(new { success = false, error = "Catalog not found" });

        if (catalog.Entity == null || catalog.Entity.UserId != userId)
            return Forbid();

        var bundle = await _bundleRepo.GetByIdAsync(bundleId);
        if (bundle == null)
            return NotFound(new { success = false, error = "Bundle not found" });

        // Verify ownership of bundle through CatalogItems
        var hasAccess = await _context.CatalogItems
            .AnyAsync(i => i.BundleId == bundleId && i.UserId == userId);

        if (!hasAccess)
            return Forbid();

        // Create a CatalogItem that references the bundle
        var catalogItem = new CatalogItem
        {
            CatalogId = dto.CatalogId,
            CategoryId = dto.CategoryId,
            ItemType = CatalogItemType.Bundle,
            BundleId = bundleId,
            UserId = userId,
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
    /// Update a bundle step
    /// </summary>
    [HttpPut("{bundleId}/steps/{stepId}")]
    public async Task<IActionResult> UpdateStep(int bundleId, int stepId, [FromBody] UpdateBundleStepDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var userId = GetUserId();

        var bundle = await _bundleRepo.GetByIdAsync(bundleId);
        if (bundle == null)
            return NotFound(new { success = false, error = "Bundle not found" });

        // Verify ownership through CatalogItems
        var hasAccess = await _context.CatalogItems
            .AnyAsync(i => i.BundleId == bundleId && i.UserId == userId);

        if (!hasAccess)
            return Forbid();

        var step = await _context.CatalogBundleSteps.FindAsync(stepId);
        if (step == null || step.BundleId != bundleId)
            return NotFound(new { success = false, error = "Step not found" });

        if (!string.IsNullOrEmpty(dto.Name))
            step.Name = dto.Name;

        if (dto.MinSelect.HasValue)
            step.MinSelect = dto.MinSelect.Value;

        if (dto.MaxSelect.HasValue)
            step.MaxSelect = dto.MaxSelect.Value;

        if (dto.StepNumber.HasValue)
            step.StepNumber = dto.StepNumber.Value;

        step.UpdatedAt = DateTime.UtcNow;
        step.UpdatedBy = userId.ToString();

        await _bundleRepo.UpdateStepAsync(step);
        await _bundleRepo.SaveChangesAsync();

        return Ok(new { success = true, data = new { step } });
    }

    /// <summary>
    /// Delete a bundle step
    /// </summary>
    [HttpDelete("{bundleId}/steps/{stepId}")]
    public async Task<IActionResult> DeleteStep(int bundleId, int stepId)
    {
        var userId = GetUserId();

        var bundle = await _bundleRepo.GetByIdAsync(bundleId);
        if (bundle == null)
            return NotFound(new { success = false, error = "Bundle not found" });

        // Verify ownership through CatalogItems
        var hasAccess = await _context.CatalogItems
            .AnyAsync(i => i.BundleId == bundleId && i.UserId == userId);

        if (!hasAccess)
            return Forbid();

        await _bundleRepo.DeleteStepAsync(stepId);
        await _bundleRepo.SaveChangesAsync();

        return Ok(new { success = true, message = "Step deleted successfully" });
    }

    /// <summary>
    /// Remove a product from a bundle step
    /// </summary>
    [HttpDelete("{bundleId}/steps/{stepId}/products/{productId}")]
    public async Task<IActionResult> RemoveProductFromStep(int bundleId, int stepId, int productId)
    {
        var userId = GetUserId();

        var bundle = await _bundleRepo.GetByIdAsync(bundleId);
        if (bundle == null)
            return NotFound(new { success = false, error = "Bundle not found" });

        // Verify ownership through CatalogItems
        var hasAccess = await _context.CatalogItems
            .AnyAsync(i => i.BundleId == bundleId && i.UserId == userId);

        if (!hasAccess)
            return Forbid();

        await _bundleRepo.DeleteStepProductAsync(stepId, productId);
        await _bundleRepo.SaveChangesAsync();

        return Ok(new { success = true, message = "Product removed from step successfully" });
    }

    /// <summary>
    /// Reorder bundle steps
    /// </summary>
    [HttpPut("{bundleId}/steps/reorder")]
    public async Task<IActionResult> ReorderSteps(int bundleId, [FromBody] ReorderStepsDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var userId = GetUserId();

        var bundle = await _bundleRepo.GetByIdAsync(bundleId);
        if (bundle == null)
            return NotFound(new { success = false, error = "Bundle not found" });

        // Verify ownership through CatalogItems
        var hasAccess = await _context.CatalogItems
            .AnyAsync(i => i.BundleId == bundleId && i.UserId == userId);

        if (!hasAccess)
            return Forbid();

        // Update step numbers based on order
        foreach (var stepOrder in dto.Steps)
        {
            var step = await _context.CatalogBundleSteps.FindAsync(stepOrder.StepId);
            if (step != null && step.BundleId == bundleId)
            {
                step.StepNumber = stepOrder.StepNumber;
                step.UpdatedAt = DateTime.UtcNow;
                step.UpdatedBy = userId.ToString();
                await _bundleRepo.UpdateStepAsync(step);
            }
        }

        await _bundleRepo.SaveChangesAsync();

        return Ok(new { success = true, message = "Steps reordered successfully" });
    }

    /// <summary>
    /// Add an option to an option group
    /// </summary>
    [HttpPost("{bundleId}/option-groups/{optionGroupId}/options")]
    public async Task<IActionResult> AddOption(int bundleId, int optionGroupId, [FromBody] CreateBundleOptionDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var userId = GetUserId();

        var bundle = await _bundleRepo.GetByIdAsync(bundleId);

        if (bundle == null)
            return NotFound(new { success = false, error = "Bundle not found" });

        // Verify ownership through CatalogItems
        var hasAccess = await _context.CatalogItems
            .AnyAsync(i => i.BundleId == bundleId && i.UserId == userId);

        if (!hasAccess)
            return Forbid();

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
    public int CatalogId { get; set; }
    public int? CategoryId { get; set; }
    public int SortOrder { get; set; } = 0;
}

/// <summary>
/// Data transfer object for updating a bundle step
/// </summary>
public class UpdateBundleStepDto
{
    public string? Name { get; set; }
    public int? MinSelect { get; set; }
    public int? MaxSelect { get; set; }
    public int? StepNumber { get; set; }
}

/// <summary>
/// Data transfer object for reordering steps
/// </summary>
public class ReorderStepsDto
{
    public List<StepOrderDto> Steps { get; set; } = new List<StepOrderDto>();
}

/// <summary>
/// Data transfer object for step order
/// </summary>
public class StepOrderDto
{
    public int StepId { get; set; }
    public int StepNumber { get; set; }
}
