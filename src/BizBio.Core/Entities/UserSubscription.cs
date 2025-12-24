namespace BizBio.Core.Entities;

using BizBio.Core.Entities.Lookups;
using BizBio.Core.Enums;
using System.ComponentModel.DataAnnotations;

public class UserSubscription : BaseEntity
{
    //public int Id { get; set; }

    public int UserId { get; set; }

    public int TierId { get; set; }

    /// <summary>
    /// Which product line this subscription is for (Menu, Card, Retail)
    /// Allows tracking separate 14-day trials per product
    /// </summary>
    public int ProductLineId { get; set; }

    public int StatusId { get; set; }

    public int BillingCycleId { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public DateTime? TrialEndsAt { get; set; }

    public DateTime? NextBillingDate { get; set; }

    public DateTime? CancelledAt { get; set; }

    public DateTime? PausedAt { get; set; }

    public decimal Price { get; set; }

    [MaxLength(10)]
    public string Currency { get; set; } = "ZAR";

    public decimal DiscountPercent { get; set; }

    [MaxLength(500)]
    public string? DiscountReason { get; set; }

    // Custom limits (nullable for overrides)
    public int? CustomMaxProfiles { get; set; }

    public int? CustomMaxCatalogItems { get; set; }

    public int? CustomMaxLocations { get; set; }

    [MaxLength(100)]
    public string? PaymentMethod { get; set; }

    public DateTime? LastPaymentDate { get; set; }

    public decimal? LastPaymentAmount { get; set; }

    public bool AutoRenew { get; set; } = true;

    //public DateTime CreatedAt { get; set; }

    //public DateTime UpdatedAt { get; set; }

    // Navigation properties
    public virtual User User { get; set; } = null!;

    public virtual SubscriptionTier Tier { get; set; } = null!;
    public virtual SubscriptionStatusLookup Status { get; set; } = null!;
    public virtual BillingCycleLookup BillingCycle { get; set; } = null!;
    public virtual ProductLineLookup ProductLine { get; set; } = null!;
    public virtual ICollection<UserSubscriptionAddon> Addons { get; set; } = new List<UserSubscriptionAddon>();
}
