namespace BizBio.Core.Entities;

using System.ComponentModel.DataAnnotations;

/// <summary>
/// Represents an extra/modifier that can be added to catalog items (e.g., "Feta Cheese", "Extra Bacon")
/// </summary>
public class CatalogItemExtra : BaseEntity
{
    public int? UserId { get; set; } // For library extras
    public int? CatalogId { get; set; } // For catalog-specific extras

    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = null!;

    [MaxLength(1000)]
    public string? Description { get; set; }

    [MaxLength(100)]
    public string? Code { get; set; } // SKU or product code

    public decimal BasePrice { get; set; }

    [MaxLength(500)]
    public string? ImageUrl { get; set; }

    public int DisplayOrder { get; set; }

    // Navigation properties
    public virtual User? User { get; set; }
    public virtual Catalog? Catalog { get; set; }
    public virtual ICollection<CatalogItemExtraGroupItem> ExtraGroupItems { get; set; } = new List<CatalogItemExtraGroupItem>();
}
