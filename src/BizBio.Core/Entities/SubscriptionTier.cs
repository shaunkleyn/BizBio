namespace BizBio.Core.Entities;

using BizBio.Core.Entities.Lookups;
using BizBio.Core.Enums;
using System.ComponentModel.DataAnnotations;

public class SubscriptionTier : BaseEntity
{
    //public int Id { get; set; }

    public int ProductLineId { get; set; }

    [Required]
    [MaxLength(100)]
    public string TierName { get; set; } = null!;

    [Required]
    [MaxLength(50)]
    public string TierCode { get; set; } = null!;

    [Required]
    [MaxLength(100)]
    public string DisplayName { get; set; } = null!;

    [MaxLength(1000)]
    public string? Description { get; set; }

    public decimal MonthlyPrice { get; set; }

    public decimal AnnualPrice { get; set; }

    public decimal AnnualDiscountPercent { get; set; } = 15.00m;

    // Feature limits
    public int MaxProfiles { get; set; }

    public int MaxCatalogItems { get; set; }

    public int MaxLocations { get; set; }

    public int MaxTeamMembers { get; set; }

    public int MaxDocuments { get; set; }

    public int MaxDocumentSizeMB { get; set; }

    public int MaxImagesPerItem { get; set; }

    // Feature flags
    public bool CustomBranding { get; set; }

    public bool RemoveBranding { get; set; }

    public bool Analytics { get; set; }

    public bool AdvancedAnalytics { get; set; }

    public bool ApiAccess { get; set; }

    public bool WhiteLabel { get; set; }

    public bool CustomDomain { get; set; }

    public bool CustomSubdomain { get; set; }

    public bool PrioritySupport { get; set; }

    public bool PhoneSupport { get; set; }

    public bool DedicatedManager { get; set; }

    // Catalog features
    public bool ItemVariants { get; set; }

    public bool ItemAddons { get; set; }

    public bool AllergenInfo { get; set; }

    public bool DietaryTags { get; set; }

    public bool MenuScheduling { get; set; }

    public bool MultiLanguage { get; set; }

    public bool NutritionalInfo { get; set; }

    // Retail features
    public bool InventoryTracking { get; set; }

    public bool MultiLocationInventory { get; set; }

    public bool BulkOperations { get; set; }

    public bool BarcodeGeneration { get; set; }

    public bool B2BFeatures { get; set; }

    // Professional features
    public bool OrganizationalHierarchy { get; set; }

    public bool TeamManagement { get; set; }

    public bool SSOIntegration { get; set; }

    // Display and metadata
    public int DisplayOrder { get; set; }

    //public bool IsActive { get; set; }

    public bool IsPopular { get; set; }

    [MaxLength(500)]
    public string? RecommendedFor { get; set; }

    //public DateTime CreatedAt { get; set; }

    //public DateTime UpdatedAt { get; set; }

    // Navigation properties
    public virtual ICollection<UserSubscription> Subscriptions { get; set; } = new List<UserSubscription>();
    public virtual ProductLineLookup ProductLine { get; set; } = null!;
}
