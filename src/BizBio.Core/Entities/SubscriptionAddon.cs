using System.ComponentModel.DataAnnotations;

namespace BizBio.Core.Entities;

/// <summary>
/// Represents an addon that can be purchased with a subscription tier
/// Examples: "Waiter Call", "Order Sending", "Menu Pro"
/// </summary>
public class SubscriptionAddon : BaseEntity
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = null!;

    [MaxLength(2000)]
    public string? Description { get; set; }

    /// <summary>
    /// Which product line this addon belongs to
    /// Links to ProductLineLookup (Menu, Card, Retail)
    /// </summary>
    public int ProductLineId { get; set; }

    /// <summary>
    /// Monthly price for this addon
    /// </summary>
    public decimal MonthlyPrice { get; set; }

    /// <summary>
    /// JSON containing addon-specific configuration/features
    /// Example: {"maxTables": 10, "features": ["realtime_notifications"]}
    /// </summary>
    [MaxLength(4000)]
    public string? ConfigurationJson { get; set; }

    public int SortOrder { get; set; } = 0;

    // Navigation properties
    public virtual ICollection<SubscriptionTierAddon> TierAddons { get; set; } = new List<SubscriptionTierAddon>();
    public virtual ICollection<UserSubscriptionAddon> UserSubscriptionAddons { get; set; } = new List<UserSubscriptionAddon>();
}
