namespace BizBio.Core.Entities;

using System.ComponentModel.DataAnnotations;

/// <summary>
/// Represents a group of extras (e.g., "Add Extra Cheese", "Choose Toppings")
/// </summary>
public class CatalogItemExtraGroup : BaseEntity
{
    public int? UserId { get; set; } // For library groups
    public int? CatalogId { get; set; } // For catalog-specific groups

    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = null!;

    [MaxLength(1000)]
    public string? Description { get; set; }

    public int MinRequired { get; set; } = 0; // Minimum number of selections required
    public int MaxAllowed { get; set; } = 0; // Maximum number of selections allowed (0 = unlimited)

    public int DisplayOrder { get; set; }

    public bool AllowMultipleQuantities { get; set; } = true; // Allow customers to select quantity per extra

    // Navigation properties
    public virtual User? User { get; set; }
    public virtual Catalog? Catalog { get; set; }
    public virtual ICollection<CatalogItemExtraGroupItem> GroupItems { get; set; } = new List<CatalogItemExtraGroupItem>();
    public virtual ICollection<CatalogItemExtraGroupLink> ItemLinks { get; set; } = new List<CatalogItemExtraGroupLink>();
}
