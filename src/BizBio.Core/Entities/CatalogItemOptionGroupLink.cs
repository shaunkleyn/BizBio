namespace BizBio.Core.Entities;

/// <summary>
/// Junction table linking Catalog Items to Option Groups (many-to-many)
/// Determines which option groups are available for which catalog items
/// </summary>
public class CatalogItemOptionGroupLink : BaseEntity
{
    public int CatalogItemId { get; set; }
    public int OptionGroupId { get; set; }

    /// <summary>
    /// Optional: Link option group to specific variant
    /// Allows different variants to have different option groups
    /// e.g., "Large Pizza" might have different topping options than "Small Pizza"
    /// </summary>
    public int? VariantId { get; set; }

    public int DisplayOrder { get; set; }

    // Navigation properties
    public virtual CatalogItem CatalogItem { get; set; } = null!;
    public virtual CatalogItemOptionGroup OptionGroup { get; set; } = null!;
    public virtual CatalogItemVariant? Variant { get; set; }
}
