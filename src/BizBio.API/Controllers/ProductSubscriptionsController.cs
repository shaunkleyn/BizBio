using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BizBio.Infrastructure.Data;
using BizBio.Core.Entities;
using System.Security.Claims;

namespace BizBio.API.Controllers;

[Route("api/v1/subscriptions")]
[ApiController]
[Authorize]
public class ProductSubscriptionsController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<ProductSubscriptionsController> _logger;

    public ProductSubscriptionsController(
        ApplicationDbContext context,
        ILogger<ProductSubscriptionsController> logger)
    {
        _context = context;
        _logger = logger;
    }

    private int GetUserId() => int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

    /// <summary>
    /// Get all product subscriptions for the current user
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetMySubscriptions()
    {
        try
        {
            var userId = GetUserId();

            var subs = await _context.ProductSubscriptions
                .Where(s => s.UserId == userId && s.IsActive)
                .Include(s => s.Tier)
                .ToListAsync();

            var entityCount = await _context.Entities.CountAsync(e => e.UserId == userId && e.IsActive);
            var catalogCount = await _context.Catalogs.CountAsync(c => c.Entity.UserId == userId && c.IsActive);

            var subscriptions = subs.Select(s => new
            {
                s.Id,
                s.UserId,
                s.ProductType,
                s.TierId,
                TierName = s.Tier.TierName,
                s.IsTrialActive,
                s.TrialStartDate,
                s.TrialEndDate,
                TrialDaysRemaining = s.TrialEndDate > DateTime.UtcNow
                    ? (int)(s.TrialEndDate - DateTime.UtcNow).TotalDays
                    : 0,
                s.Status,
                s.BillingCycle,
                s.CurrentPeriodStart,
                s.CurrentPeriodEnd,
                s.NextBillingDate,
                s.CancelledAt,
                s.IsActive,
                s.CreatedAt,
                s.UpdatedAt,
                // Tier limits
                Limits = new
                {
                    s.Tier.MaxEntities,
                    s.Tier.MaxCatalogsPerEntity,
                    s.Tier.MaxLibraryItems,
                    s.Tier.MaxCategoriesPerCatalog,
                    s.Tier.MaxBundles
                },
                // Current usage
                Usage = new
                {
                    Entities = entityCount,
                    Catalogs = catalogCount
                }
            }).ToList();

            return Ok(new { success = true, subscriptions });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting subscriptions for user");
            return StatusCode(500, new { success = false, error = "An error occurred while fetching subscriptions" });
        }
    }

    /// <summary>
    /// Get a specific product subscription
    /// </summary>
    [HttpGet("{productType}")]
    public async Task<IActionResult> GetProductSubscription(int productType)
    {
        try
        {
            var userId = GetUserId();

            var sub = await _context.ProductSubscriptions
                .Where(s => s.UserId == userId && s.ProductType == (ProductType)productType && s.IsActive)
                .Include(s => s.Tier)
                .FirstOrDefaultAsync();

            if (sub == null)
                return NotFound(new { success = false, error = "Subscription not found for this product" });

            var subscription = new
            {
                sub.Id,
                sub.UserId,
                sub.ProductType,
                sub.TierId,
                TierName = sub.Tier.TierName,
                sub.IsTrialActive,
                sub.TrialStartDate,
                sub.TrialEndDate,
                TrialDaysRemaining = sub.TrialEndDate > DateTime.UtcNow
                    ? (int)(sub.TrialEndDate - DateTime.UtcNow).TotalDays
                    : 0,
                sub.Status,
                sub.BillingCycle,
                sub.CurrentPeriodStart,
                sub.CurrentPeriodEnd,
                sub.NextBillingDate,
                sub.CancelledAt,
                sub.IsActive,
                sub.CreatedAt,
                sub.UpdatedAt,
                // Tier limits
                Limits = new
                {
                    sub.Tier.MaxEntities,
                    sub.Tier.MaxCatalogsPerEntity,
                    sub.Tier.MaxLibraryItems,
                    sub.Tier.MaxCategoriesPerCatalog,
                    sub.Tier.MaxBundles
                }
            };

            return Ok(new { success = true, subscription });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting subscription for product type {ProductType}", productType);
            return StatusCode(500, new { success = false, error = "An error occurred while fetching the subscription" });
        }
    }

    /// <summary>
    /// Subscribe to a new product
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> SubscribeToProduct([FromBody] SubscribeToProductRequest request)
    {
        try
        {
            var userId = GetUserId();

            // Check if user already has a subscription for this product
            var existingSubscription = await _context.ProductSubscriptions
                .AnyAsync(s => s.UserId == userId && s.ProductType == (ProductType)request.ProductType && s.IsActive);

            if (existingSubscription)
            {
                return BadRequest(new { success = false, error = "You already have an active subscription for this product" });
            }

            // Get the tier
            var tier = await _context.SubscriptionTiers
                .FirstOrDefaultAsync(t => t.Id == request.TierId && t.IsActive);

            if (tier == null)
            {
                return NotFound(new { success = false, error = "Subscription tier not found" });
            }

            // Create the subscription
            var userEmail = User.FindFirst(ClaimTypes.Email)!.Value;

            var subscription = new ProductSubscription
            {
                UserId = userId,
                ProductType = (ProductType)request.ProductType,
                TierId = request.TierId,
                IsTrialActive = true,
                TrialStartDate = DateTime.UtcNow,
                TrialEndDate = DateTime.UtcNow.AddDays(tier.TrialDays),
                Status = SubscriptionStatus.Trial,
                BillingCycle = BillingCycle.Monthly,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                CreatedBy = userEmail,
                UpdatedBy = userEmail
            };

            _context.ProductSubscriptions.Add(subscription);
            await _context.SaveChangesAsync();

            // TODO: Calculate pro-rata billing if adding mid-cycle

            return Ok(new { success = true, subscription = new
            {
                subscription.Id,
                subscription.UserId,
                subscription.ProductType,
                subscription.TierId,
                subscription.IsTrialActive,
                subscription.TrialStartDate,
                subscription.TrialEndDate,
                subscription.Status,
                subscription.BillingCycle,
                subscription.CreatedAt
            }});
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error subscribing to product");
            return StatusCode(500, new { success = false, error = "An error occurred while creating the subscription" });
        }
    }

    /// <summary>
    /// Upgrade/downgrade subscription tier
    /// </summary>
    [HttpPut("{productType}/upgrade")]
    public async Task<IActionResult> UpgradeTier(int productType, [FromBody] UpgradeTierRequest request)
    {
        try
        {
            var userId = GetUserId();

            var subscription = await _context.ProductSubscriptions
                .FirstOrDefaultAsync(s => s.UserId == userId && s.ProductType == (ProductType)productType && s.IsActive);

            if (subscription == null)
                return NotFound(new { success = false, error = "Subscription not found" });

            // Get the new tier
            var newTier = await _context.SubscriptionTiers
                .FirstOrDefaultAsync(t => t.Id == request.NewTierId && t.IsActive);

            if (newTier == null)
                return NotFound(new { success = false, error = "Subscription tier not found" });

            // Update the tier
            var userEmail = User.FindFirst(ClaimTypes.Email)!.Value;
            subscription.TierId = request.NewTierId;
            subscription.UpdatedAt = DateTime.UtcNow;
            subscription.UpdatedBy = userEmail;

            // TODO: Calculate pro-rata billing adjustment

            await _context.SaveChangesAsync();

            return Ok(new { success = true, message = "Subscription tier updated successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error upgrading subscription tier");
            return StatusCode(500, new { success = false, error = "An error occurred while upgrading the subscription" });
        }
    }

    /// <summary>
    /// Cancel a product subscription
    /// </summary>
    [HttpPost("{productType}/cancel")]
    public async Task<IActionResult> CancelProductSubscription(int productType)
    {
        try
        {
            var userId = GetUserId();

            var subscription = await _context.ProductSubscriptions
                .FirstOrDefaultAsync(s => s.UserId == userId && s.ProductType == (ProductType)productType && s.IsActive);

            if (subscription == null)
                return NotFound(new { success = false, error = "Subscription not found" });

            // Cancel the subscription
            var userEmail = User.FindFirst(ClaimTypes.Email)!.Value;
            subscription.Status = SubscriptionStatus.Cancelled;
            subscription.CancelledAt = DateTime.UtcNow;
            subscription.UpdatedAt = DateTime.UtcNow;
            subscription.UpdatedBy = userEmail;

            await _context.SaveChangesAsync();

            return Ok(new { success = true, message = "Subscription cancelled successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error cancelling subscription");
            return StatusCode(500, new { success = false, error = "An error occurred while cancelling the subscription" });
        }
    }

    /// <summary>
    /// Get available subscription tiers for a product
    /// </summary>
    [HttpGet("tiers/{productType}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetProductTiers(int productType)
    {
        try
        {
            var tiers = await _context.SubscriptionTiers
                .Where(t => t.ProductLineId == productType && t.IsActive)
                .OrderBy(t => t.MonthlyPrice)
                .Select(t => new
                {
                    t.Id,
                    t.ProductLineId,
                    t.TierName,
                    t.Description,
                    t.MonthlyPrice,
                    t.AnnualPrice,
                    t.TrialDays,
                    t.MaxEntities,
                    t.MaxCatalogsPerEntity,
                    t.MaxLibraryItems,
                    t.MaxCategoriesPerCatalog,
                    t.MaxBundles,
                    t.IsActive
                })
                .ToListAsync();

            if (!tiers.Any())
            {
                return NotFound(new { success = false, error = "No tiers found for this product" });
            }

            return Ok(new { success = true, tiers });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting tiers for product type {ProductType}", productType);
            return StatusCode(500, new { success = false, error = "An error occurred while fetching subscription tiers" });
        }
    }

    /// <summary>
    /// Get combined invoice preview for all active products
    /// </summary>
    [HttpGet("invoice-preview")]
    public async Task<IActionResult> GetInvoicePreview()
    {
        try
        {
            var userId = GetUserId();

            var subscriptions = await _context.ProductSubscriptions
                .Where(s => s.UserId == userId && s.IsActive && s.Status == SubscriptionStatus.Active)
                .Include(s => s.Tier)
                .ToListAsync();

            var lineItems = subscriptions.Select(s => new
            {
                ProductType = s.ProductType.ToString(),
                TierName = s.Tier.TierName,
                Amount = s.BillingCycle == BillingCycle.Monthly
                    ? s.Tier.MonthlyPrice
                    : s.Tier.AnnualPrice,
                BillingCycle = s.BillingCycle.ToString()
            }).ToList();

            var subtotal = lineItems.Sum(i => i.Amount);
            var vat = subtotal * 0.15m; // 15% VAT
            var total = subtotal + vat;

            return Ok(new
            {
                success = true,
                invoice = new
                {
                    lineItems,
                    subtotal,
                    vat,
                    total,
                    currency = "ZAR"
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating invoice preview");
            return StatusCode(500, new { success = false, error = "An error occurred while generating the invoice preview" });
        }
    }
}

// DTOs
public class SubscribeToProductRequest
{
    public int ProductType { get; set; }
    public int TierId { get; set; }
}

public class UpgradeTierRequest
{
    public int NewTierId { get; set; }
}
