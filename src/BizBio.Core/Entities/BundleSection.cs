namespace BizBio.Core.Entities;

public class BundleSection : BaseEntity
{
    public int BundleId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int MinRequired { get; set; }
    public int MaxAllowed { get; set; }
    public int SortOrder { get; set; }

    // Navigation properties
    public virtual Bundle Bundle { get; set; } = null!;
    public virtual ICollection<BundleSectionItem> Items { get; set; } = new List<BundleSectionItem>();
}
