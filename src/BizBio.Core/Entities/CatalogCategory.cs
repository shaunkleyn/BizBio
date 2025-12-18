namespace BizBio.Core.Entities;

using System.ComponentModel.DataAnnotations;

public class CatalogCategory : BaseEntity
{
    //public int Id { get; set; }

    // Nullable CatalogId - null means it's a user library category
    public int? CatalogId { get; set; }

    // UserId for library categories (categories owned by user but not in a specific catalog)
    public int? UserId { get; set; }

    [Required]
    [MaxLength(255)]
    public string Name { get; set; } = null!;

    [MaxLength(2000)]
    public string? Description { get; set; }

    [MaxLength(100)]
    public string? Icon { get; set; }

    [MaxLength(5000)]
    public string? Images { get; set; } // JSON array as string

    public int SortOrder { get; set; } = 0;

    //public bool IsActive { get; set; } = true;

    //public DateTime CreatedAt { get; set; }

    //public DateTime UpdatedAt { get; set; }

    // Navigation properties
    public virtual Catalog? Catalog { get; set; }

    public virtual ICollection<CatalogItem> Items { get; set; } = new List<CatalogItem>();
}
