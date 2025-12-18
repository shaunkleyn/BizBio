namespace BizBio.Core.Entities;

public class CatalogBundleStepProduct : BaseEntity
{
    public int StepId { get; set; }

    public int ProductId { get; set; }

    // Navigation properties
    public virtual CatalogBundleStep Step { get; set; } = null!;
    public virtual CatalogItem Product { get; set; } = null!;
}
