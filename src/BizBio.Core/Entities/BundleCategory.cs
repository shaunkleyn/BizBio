namespace BizBio.Core.Entities;

public class BundleCategory : BaseEntity
{
    public int BundleId { get; set; }
    public int CategoryId { get; set; }

    // Navigation properties
    public virtual Bundle Bundle { get; set; } = null!;
    public virtual CatalogCategory Category { get; set; } = null!;
}
