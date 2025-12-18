namespace BizBio.Core.Entities;

/// <summary>
/// Links extras to extra groups (many-to-many relationship)
/// </summary>
public class CatalogItemExtraGroupItem : BaseEntity
{
    public int ExtraGroupId { get; set; }
    public int ExtraId { get; set; }

    public int DisplayOrder { get; set; }

    public decimal? PriceOverride { get; set; } // Optional price override for this specific context

    // Navigation properties
    public virtual CatalogItemExtraGroup ExtraGroup { get; set; } = null!;
    public virtual CatalogItemExtra Extra { get; set; } = null!;
}
