namespace BizBio.Core.Entities;

public class CatalogBundleCategory : BaseEntity
{
    public int CatalogBundleId { get; set; }
    public int CategoryId { get; set; }

    // Navigation properties
    public virtual CatalogBundle CatalogBundle { get; set; } = null!;
    public virtual CatalogCategory Category { get; set; } = null!;
}
