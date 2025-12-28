using System.ComponentModel.DataAnnotations;

namespace BizBio.Core.Entities;

/// <summary>
/// Represents a reference to a library item within a specific catalog/menu.
/// Allows for menu-specific overrides while maintaining live sync with the library item.
/// </summary>
public class CatalogItemInstance : BaseEntity
{
    /// <summary>
    /// The catalog/menu this instance belongs to
    /// </summary>
    public int CatalogId { get; set; }

    /// <summary>
    /// Reference to the library item (source of truth for core data)
    /// </summary>
    public int LibraryItemId { get; set; }

    /// <summary>
    /// Per-menu price override (null = use library item's price)
    /// </summary>
    public decimal? PriceOverride { get; set; }

    /// <summary>
    /// Per-menu name override (null = use library item's name)
    /// </summary>
    [MaxLength(255)]
    public string? NameOverride { get; set; }

    /// <summary>
    /// Per-menu description override (null = use library item's description)
    /// </summary>
    [MaxLength(2000)]
    public string? DescriptionOverride { get; set; }

    /// <summary>
    /// Per-menu availability override (null = use library item's IsActive status)
    /// </summary>
    public bool? AvailabilityOverride { get; set; }

    /// <summary>
    /// Sort order within the catalog
    /// </summary>
    public int SortOrder { get; set; } = 0;

    /// <summary>
    /// Whether this instance is visible in the menu (soft delete)
    /// </summary>
    public bool IsVisible { get; set; } = true;

    // Navigation properties
    public virtual Catalog Catalog { get; set; } = null!;
    public virtual CatalogItem LibraryItem { get; set; } = null!;
    public virtual ICollection<CatalogItemInstanceCategory> Categories { get; set; } = new List<CatalogItemInstanceCategory>();
}
