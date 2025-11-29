namespace BizBio.Core.Entities;

using System.ComponentModel.DataAnnotations;

public class Catalog
{
    public int Id { get; set; }

    public int ProfileId { get; set; }

    [Required]
    [MaxLength(255)]
    public string Name { get; set; } = null!;

    [MaxLength(2000)]
    public string? Description { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    // Navigation properties
    public virtual Profile Profile { get; set; } = null!;

    public virtual ICollection<CatalogItem> Items { get; set; } = new List<CatalogItem>();
}
