using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace BizBio.Core.Entities;

/// <summary>
/// Represents a user's subscription to a specific product (Cards, Menu, or Catalog).
/// Replaces single UserSubscription with per-product subscriptions that can be managed independently.
/// Each product has its own trial period but billing is combined monthly.
/// </summary>
public class ProductSubscription : BaseEntity
{
    /// <summary>
    /// The user who owns this subscription
    /// </summary>
    public int UserId { get; set; }

    /// <summary>
    /// Product type (Cards=0, Menu=1, Catalog=2)
    /// </summary>
    public ProductType ProductType { get; set; }

    /// <summary>
    /// Subscription tier ID
    /// </summary>
    public int TierId { get; set; }

    // Trial tracking per product

    /// <summary>
    /// Whether the trial is currently active
    /// </summary>
    public bool IsTrialActive { get; set; } = true;

    /// <summary>
    /// When the trial started
    /// </summary>
    public DateTime TrialStartDate { get; set; }

    /// <summary>
    /// When the trial ends
    /// </summary>
    public DateTime TrialEndDate { get; set; }

    // Billing

    /// <summary>
    /// Subscription status (Trial=0, Active=1, Cancelled=2, Expired=3)
    /// </summary>
    public SubscriptionStatus Status { get; set; } = SubscriptionStatus.Trial;

    /// <summary>
    /// Billing cycle (Monthly=0, Yearly=1)
    /// </summary>
    public BillingCycle BillingCycle { get; set; } = BillingCycle.Monthly;

    /// <summary>
    /// Start of current billing period
    /// </summary>
    public DateTime? CurrentPeriodStart { get; set; }

    /// <summary>
    /// End of current billing period
    /// </summary>
    public DateTime? CurrentPeriodEnd { get; set; }

    /// <summary>
    /// Next billing date
    /// </summary>
    public DateTime? NextBillingDate { get; set; }

    /// <summary>
    /// When the subscription was cancelled (if applicable)
    /// </summary>
    public DateTime? CancelledAt { get; set; }

    // Navigation properties

    /// <summary>
    /// The user who owns this subscription
    /// </summary>
    [JsonIgnore]
    public virtual User User { get; set; } = null!;

    /// <summary>
    /// The subscription tier
    /// </summary>
    [JsonIgnore]
    public virtual SubscriptionTier Tier { get; set; } = null!;
}

/// <summary>
/// Product types in the multi-product subscription model
/// </summary>
public enum ProductType
{
    /// <summary>
    /// Digital business cards product
    /// </summary>
    Cards = 0,

    /// <summary>
    /// Restaurant/venue menu product
    /// </summary>
    Menu = 1,

    /// <summary>
    /// Product catalog for stores
    /// </summary>
    Catalog = 2
}

/// <summary>
/// Subscription status enumeration
/// </summary>
public enum SubscriptionStatus
{
    /// <summary>
    /// Trial period active
    /// </summary>
    Trial = 0,

    /// <summary>
    /// Active paid subscription
    /// </summary>
    Active = 1,

    /// <summary>
    /// Subscription cancelled
    /// </summary>
    Cancelled = 2,

    /// <summary>
    /// Subscription expired
    /// </summary>
    Expired = 3
}

/// <summary>
/// Billing cycle enumeration
/// </summary>
public enum BillingCycle
{
    /// <summary>
    /// Monthly billing
    /// </summary>
    Monthly = 0,

    /// <summary>
    /// Yearly billing
    /// </summary>
    Yearly = 1
}
