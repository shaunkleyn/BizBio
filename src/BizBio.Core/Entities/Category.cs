using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace BizBio.Core.Entities;

/// <summary>
/// Entity-level category that can be used across multiple catalogs within the same entity.
/// Each entity (Restaurant/Store/Venue/Organization) has its own categories.
/// </summary>
public class Category : BaseEntity
{
    /// <summary>
    /// The entity (Restaurant/Store/Venue/Organization) this category belongs to
    /// </summary>
    public int EntityId { get; set; }

    /// <summary>
    /// Category name (e.g., "Appetizers", "Main Dishes", "Desserts")
    /// </summary>
    [Required]
    [MaxLength(255)]
    public string Name { get; set; } = null!;

    /// <summary>
    /// URL-friendly slug
    /// </summary>
    [MaxLength(255)]
    public string? Slug { get; set; }

    /// <summary>
    /// Category description
    /// </summary>
    [MaxLength(1000)]
    public string? Description { get; set; }

    /// <summary>
    /// Icon or image URL
    /// </summary>
    [MaxLength(500)]
    public string? Icon { get; set; }

    /// <summary>
    /// Display order for sorting
    /// </summary>
    public int SortOrder { get; set; } = 0;

    // Navigation properties

    /// <summary>
    /// The entity this category belongs to
    /// </summary>
    [JsonIgnore]
    public virtual Entity Entity { get; set; } = null!;

    /// <summary>
    /// CatalogCategories linking this category to catalogs (many-to-many)
    /// </summary>
    [JsonIgnore]
    public virtual ICollection<CatalogCategory> CatalogCategories { get; set; } = new List<CatalogCategory>();

    /// <summary>
    /// Items in this category (many-to-many through CatalogItemCategory)
    /// </summary>
    [JsonIgnore]
    public virtual ICollection<CatalogItemCategory> CatalogItemCategories { get; set; } = new List<CatalogItemCategory>();
}
