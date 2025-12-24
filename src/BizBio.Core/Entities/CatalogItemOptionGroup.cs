namespace BizBio.Core.Entities;

using System.ComponentModel.DataAnnotations;

/// <summary>
/// Group of options that belong together (e.g., "Choose Size", "Remove Ingredients", "Cooking Preference")
/// Defines selection rules (min/max required) for product customization
/// </summary>
public class CatalogItemOptionGroup : BaseEntity
{
    // Nullable for library groups (UserId set) or catalog-specific groups (CatalogId set)
    public int? UserId { get; set; }
    public int? CatalogId { get; set; }

    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = null!;

    [MaxLength(1000)]
    public string? Description { get; set; }

    /// <summary>
    /// Minimum number of options required (1 = required, 0 = optional)
    /// e.g., "Choose Size" would have MinRequired = 1
    /// </summary>
    public int MinRequired { get; set; } = 1;

    /// <summary>
    /// Maximum number of options allowed (1 = single choice, 0 = unlimited)
    /// e.g., "Choose Size" would have MaxAllowed = 1
    /// "Remove Toppings" might have MaxAllowed = 0 (unlimited)
    /// </summary>
    public int MaxAllowed { get; set; } = 1;

    /// <summary>
    /// Whether this option group must be configured (enforces MinRequired > 0)
    /// </summary>
    public bool IsRequired { get; set; } = true;

    public int DisplayOrder { get; set; }

    // Navigation properties
    public virtual User? User { get; set; }
    public virtual Catalog? Catalog { get; set; }
    public virtual ICollection<CatalogItemOptionGroupItem> GroupItems { get; set; } = new List<CatalogItemOptionGroupItem>();
    public virtual ICollection<CatalogItemOptionGroupLink> ItemLinks { get; set; } = new List<CatalogItemOptionGroupLink>();
}
