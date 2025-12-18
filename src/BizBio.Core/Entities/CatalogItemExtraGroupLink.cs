namespace BizBio.Core.Entities;

/// <summary>
/// Links extra groups to catalog items (which extras are available for which items)
/// </summary>
public class CatalogItemExtraGroupLink : BaseEntity
{
    public int CatalogItemId { get; set; }
    public int ExtraGroupId { get; set; }

    public int? VariantId { get; set; } // Optional: Link to specific variant

    public int DisplayOrder { get; set; }

    // Navigation properties
    public virtual CatalogItem CatalogItem { get; set; } = null!;
    public virtual CatalogItemExtraGroup ExtraGroup { get; set; } = null!;
    public virtual CatalogItemVariant? Variant { get; set; }
}
