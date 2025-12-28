namespace BizBio.Core.Entities;

/// <summary>
/// Junction table for many-to-many relationship between CatalogItemInstances and CatalogCategories.
/// Allows menu items to be organized into different categories per menu.
/// </summary>
public class CatalogItemInstanceCategory : BaseEntity
{
    /// <summary>
    /// The catalog item instance (menu item reference)
    /// </summary>
    public int InstanceId { get; set; }

    /// <summary>
    /// The category this instance belongs to in this specific menu
    /// </summary>
    public int CategoryId { get; set; }

    /// <summary>
    /// Sort order within the category
    /// </summary>
    public int SortOrder { get; set; } = 0;

    // Navigation properties
    public virtual CatalogItemInstance Instance { get; set; } = null!;
    public virtual CatalogCategory Category { get; set; } = null!;
}
