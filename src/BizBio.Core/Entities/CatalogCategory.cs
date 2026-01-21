using System.Text.Json.Serialization;

namespace BizBio.Core.Entities;

/// <summary>
/// Junction table linking Categories to Catalogs in a many-to-many relationship.
/// Categories are entity-level and can be used across multiple catalogs within the same entity.
/// </summary>
public class CatalogCategory : BaseEntity
{
    /// <summary>
    /// The catalog ID
    /// </summary>
    public int CatalogId { get; set; }

    /// <summary>
    /// The category ID
    /// </summary>
    public int CategoryId { get; set; }

    /// <summary>
    /// Display order for sorting categories within this catalog
    /// </summary>
    public int SortOrder { get; set; } = 0;

    // Navigation properties

    /// <summary>
    /// The catalog this category belongs to
    /// </summary>
    [JsonIgnore]
    public virtual Catalog Catalog { get; set; } = null!;

    /// <summary>
    /// The category
    /// </summary>
    [JsonIgnore]
    public virtual Category Category { get; set; } = null!;
}
