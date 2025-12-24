using System.ComponentModel.DataAnnotations;

namespace BizBio.Core.Entities;

/// <summary>
/// Tracks which addons a user has activated on their subscription
/// </summary>
public class UserSubscriptionAddon : BaseEntity
{
    public int UserSubscriptionId { get; set; }
    public int AddonId { get; set; }

    /// <summary>
    /// When the user activated this addon
    /// </summary>
    public DateTime AddedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Optional end date if user cancels addon
    /// </summary>
    public DateTime? CancelledAt { get; set; }

    /// <summary>
    /// Whether the addon is currently active
    /// </summary>
    public bool IsActiveAddon { get; set; } = true;

    /// <summary>
    /// Price user pays for this addon (stored for historical accuracy)
    /// May differ from current addon price due to discounts or price changes
    /// </summary>
    public decimal MonthlyPrice { get; set; }

    // Navigation properties
    public virtual UserSubscription UserSubscription { get; set; } = null!;
    public virtual SubscriptionAddon Addon { get; set; } = null!;
}
