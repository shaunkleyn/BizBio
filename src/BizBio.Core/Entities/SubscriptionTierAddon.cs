namespace BizBio.Core.Entities;

/// <summary>
/// Junction table defining which addons are available for which subscription tiers
/// Example: "Menu Pro" addon is only available for Entree and Banquet tiers, not Starter
/// </summary>
public class SubscriptionTierAddon : BaseEntity
{
    public int SubscriptionTierId { get; set; }
    public int AddonId { get; set; }

    /// <summary>
    /// Whether this addon is required for this tier (usually false)
    /// </summary>
    public bool IsRequired { get; set; } = false;

    /// <summary>
    /// Optional tier-specific discount on the addon
    /// Example: Banquet tier gets 20% off Menu Pro addon
    /// </summary>
    public decimal? DiscountPercentage { get; set; }

    // Navigation properties
    public virtual SubscriptionTier SubscriptionTier { get; set; } = null!;
    public virtual SubscriptionAddon Addon { get; set; } = null!;
}
