namespace BizBio.Core.Entities;

using System.ComponentModel.DataAnnotations;

public class CatalogBundle : BaseEntity
{
    public int CatalogId { get; set; }

    [Required]
    [MaxLength(255)]
    public string Name { get; set; } = null!;

    [MaxLength(255)]
    public string? Slug { get; set; }

    [MaxLength(2000)]
    public string? Description { get; set; }

    public decimal BasePrice { get; set; }

    [MaxLength(5000)]
    public string? Images { get; set; } // JSON array as string

    public int SortOrder { get; set; } = 0;

    // Navigation properties
    public virtual Catalog Catalog { get; set; } = null!;
    public virtual ICollection<CatalogBundleStep> Steps { get; set; } = new List<CatalogBundleStep>();
    public virtual ICollection<CatalogBundleCategory> CatalogBundleCategories { get; set; } = new List<CatalogBundleCategory>();
}
