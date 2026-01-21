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

    // DEPRECATED: Use CatalogItemCategories for many-to-many relationship instead
    // Keeping for backward compatibility during migration
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

    /// <summary>
    /// Reference to parent catalog item for item sharing within same entity.
    /// Null = master/template item. If set = reference to master with optional price override.
    /// </summary>
    public int? ParentCatalogItemId { get; set; }

    /// <summary>
    /// Price override for referenced items. Null = use parent price.
    /// Only applicable when ParentCatalogItemId is set.
    /// </summary>
    public decimal? PriceOverride { get; set; }

    //public DateTime CreatedAt { get; set; }

    //public DateTime UpdatedAt { get; set; }

    /// <summary>
    /// Computed property that returns the effective price based on override hierarchy.
    /// Priority: PriceOverride > ParentCatalogItem.EffectivePrice > Price
    /// </summary>
    public decimal EffectivePrice
    {
        get
        {
            if (PriceOverride.HasValue)
                return PriceOverride.Value;

            if (ParentCatalogItemId.HasValue && ParentCatalogItem != null)
                return ParentCatalogItem.EffectivePrice;

            return Price;
        }
    }

    // Navigation properties
    public virtual Catalog? Catalog { get; set; }

    /// <summary>
    /// Parent catalog item for item sharing (self-referencing)
    /// </summary>
    public virtual CatalogItem? ParentCatalogItem { get; set; }

    /// <summary>
    /// Child catalog items that reference this item as parent
    /// </summary>
    public virtual ICollection<CatalogItem> ChildCatalogItems { get; set; } = new List<CatalogItem>();
    public virtual CatalogBundle? Bundle { get; set; }
    public virtual User? User { get; set; }
    public virtual ICollection<CatalogItemVariant> Variants { get; set; } = new List<CatalogItemVariant>();
    public virtual ICollection<CatalogItemExtraGroupLink> ExtraGroupLinks { get; set; } = new List<CatalogItemExtraGroupLink>();
    public virtual ICollection<CatalogItemOptionGroupLink> OptionGroupLinks { get; set; } = new List<CatalogItemOptionGroupLink>();

    // Many-to-many with CatalogCategory through CatalogItemCategory
    // Supports items appearing in multiple categories (e.g., bundle in "Specials" and "Burgers")
    public virtual ICollection<CatalogItemCategory> CatalogItemCategories { get; set; } = new List<CatalogItemCategory>();
}
