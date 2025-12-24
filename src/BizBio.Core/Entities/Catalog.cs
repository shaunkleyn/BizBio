namespace BizBio.Core.Entities;

using System.ComponentModel.DataAnnotations;

public class Catalog : BaseEntity
{
    public int OwnerType { get; set; }
    public int OwnerId { get; set; }
    public int ProductId { get; set; }
    public int ProfileId { get; set; }

    [Required]
    [MaxLength(255)]
    public string Name { get; set; } = null!;

    [MaxLength(2000)]
    public string? Description { get; set; }

    public DateTime ValidFrom { get; set; }
    public DateTime ValidTo { get; set; }
    public bool IsPublic { get; set; }

    // Navigation properties
    public virtual Profile Profile { get; set; } = null!;

    public virtual ICollection<CatalogItem> Items { get; set; } = new List<CatalogItem>();
    public virtual ICollection<CatalogCategory> Categories { get; set; } = new List<CatalogCategory>();
    public virtual ICollection<CatalogVersion> Versions { get; set; } = new List<CatalogVersion>();
    public virtual ICollection<CatalogBundle> Bundles { get; set; } = new List<CatalogBundle>();
}
