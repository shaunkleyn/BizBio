namespace BizBio.Core.Entities;

/// <summary>
/// Junction table linking Option Groups to Options (many-to-many)
/// Defines which options belong to which groups
/// </summary>
public class CatalogItemOptionGroupItem : BaseEntity
{
    public int OptionGroupId { get; set; }
    public int OptionId { get; set; }

    public int DisplayOrder { get; set; }

    /// <summary>
    /// Whether this option is selected by default
    /// e.g., "Medium" might be the default size
    /// </summary>
    public bool IsDefault { get; set; } = false;

    // Navigation properties
    public virtual CatalogItemOptionGroup OptionGroup { get; set; } = null!;
    public virtual CatalogItemOption Option { get; set; } = null!;
}
