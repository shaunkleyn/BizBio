namespace BizBio.Core.Entities;

using System.ComponentModel.DataAnnotations;

/// <summary>
/// Individual option for product customization (e.g., "Small", "Medium", "Large", "No Tomato")
/// Represents product options/choices, separate from extras which are add-ons/upsells
/// </summary>
public class CatalogItemOption : BaseEntity
{
    // Nullable for library options (UserId set) or catalog-specific options (CatalogId set)
    public int? UserId { get; set; }
    public int? CatalogId { get; set; }

    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = null!;

    [MaxLength(1000)]
    public string? Description { get; set; }

    /// <summary>
    /// Price adjustment for this option (can be positive, negative, or zero)
    /// e.g., "Large" might add +R10, "No Sugar" might be R0
    /// </summary>
    public decimal PriceModifier { get; set; }

    [MaxLength(500)]
    public string? ImageUrl { get; set; }

    public int DisplayOrder { get; set; }

    // Navigation properties
    public virtual User? User { get; set; }
    public virtual Catalog? Catalog { get; set; }
    public virtual ICollection<CatalogItemOptionGroupItem> OptionGroupItems { get; set; } = new List<CatalogItemOptionGroupItem>();
}
