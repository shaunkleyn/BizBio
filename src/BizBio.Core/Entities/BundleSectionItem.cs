namespace BizBio.Core.Entities;

public class BundleSectionItem : BaseEntity
{
    public int SectionId { get; set; }
    public int CatalogItemId { get; set; }
    public decimal Quantity { get; set; }
    public decimal PriceAdjustment { get; set; }
    public int SortOrder { get; set; }

    // Navigation properties
    public virtual BundleSection Section { get; set; } = null!;
    public virtual CatalogItem CatalogItem { get; set; } = null!;
}
