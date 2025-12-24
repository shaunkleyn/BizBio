namespace BizBio.Core.Entities;

public class Bundle : BaseEntity
{
    public int? CatalogId { get; set; }
    public int? UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal BasePrice { get; set; }
    public string? Images { get; set; }
    public int SortOrder { get; set; }

    // Navigation properties
    public virtual Catalog? Catalog { get; set; }
    public virtual User? User { get; set; }
    public virtual ICollection<BundleSection> Sections { get; set; } = new List<BundleSection>();
    public virtual ICollection<BundleCategory> Categories { get; set; } = new List<BundleCategory>();
}
