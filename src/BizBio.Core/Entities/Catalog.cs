namespace BizBio.Core.Entities;

using System.ComponentModel.DataAnnotations;

public class Catalog : BaseEntity
{
    /// <summary>
    /// The entity (Restaurant/Store/Venue/Organization) this catalog belongs to
    /// </summary>
    public int EntityId { get; set; }

    [Required]
    [MaxLength(255)]
    public string Name { get; set; } = null!;

    /// <summary>
    /// URL-friendly slug for catalog routing
    /// </summary>
    [MaxLength(255)]
    public string? Slug { get; set; }

    [MaxLength(2000)]
    public string? Description { get; set; }

    /// <summary>
    /// Sort order for displaying multiple catalogs
    /// </summary>
    public int SortOrder { get; set; } = 0;

    public DateTime ValidFrom { get; set; }
    public DateTime ValidTo { get; set; }
    public bool IsPublic { get; set; }

    // Navigation properties

    /// <summary>
    /// The entity (Restaurant/Store/Venue/Organization) this catalog belongs to
    /// </summary>
    public virtual Entity Entity { get; set; } = null!;

    public virtual ICollection<CatalogItem> Items { get; set; } = new List<CatalogItem>();
    public virtual ICollection<CatalogCategory> Categories { get; set; } = new List<CatalogCategory>();
    public virtual ICollection<CatalogVersion> Versions { get; set; } = new List<CatalogVersion>();
    public virtual ICollection<CatalogBundle> Bundles { get; set; } = new List<CatalogBundle>();
}
