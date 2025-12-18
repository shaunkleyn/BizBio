namespace BizBio.Core.Entities;

using System.ComponentModel.DataAnnotations;

public enum CatalogItemType
{
    Regular = 0,
    Bundle = 1
}

public class CatalogItem : BaseEntity
{
    //public int Id { get; set; }

    // Nullable CatalogId - null means it's a library item not yet assigned to a catalog
    public int? CatalogId { get; set; }

    // UserId for library items (items owned by user but not in a specific catalog)
    public int? UserId { get; set; }

    public int? CategoryId { get; set; }

    public CatalogItemType ItemType { get; set; } = CatalogItemType.Regular;

    public int? BundleId { get; set; }

    [Required]
    [MaxLength(255)]
    public string Name { get; set; } = null!;

    [MaxLength(2000)]
    public string? Description { get; set; }

    public decimal Price { get; set; }

    [MaxLength(5000)]
    public string? Images { get; set; } // JSON array as string

    // Tags for allergens, dietary info (JSON array: ["Gluten-Free", "Vegan", "Nut Allergy", etc.])
    [MaxLength(2000)]
    public string? Tags { get; set; }

    public bool AvailableInEventMode { get; set; } = true;

    public bool EventModeOnly { get; set; } = false;

    //public bool IsActive { get; set; } = true;

    public int SortOrder { get; set; } = 0;

    // Reference to source library item (when item is copied from library to catalog)
    public int? SourceLibraryItemId { get; set; }

    //public DateTime CreatedAt { get; set; }

    //public DateTime UpdatedAt { get; set; }

    // Navigation properties
    public virtual Catalog? Catalog { get; set; }
    public virtual CatalogBundle? Bundle { get; set; }
    public virtual User? User { get; set; }
    public virtual ICollection<CatalogItemVariant> Variants { get; set; } = new List<CatalogItemVariant>();
    public virtual ICollection<CatalogItemExtraGroupLink> ExtraGroupLinks { get; set; } = new List<CatalogItemExtraGroupLink>();
}
