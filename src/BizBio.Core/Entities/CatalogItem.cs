namespace BizBio.Core.Entities;

using System.ComponentModel.DataAnnotations;

public class CatalogItem
{
    public int Id { get; set; }

    public int CatalogId { get; set; }

    public int? CategoryId { get; set; }

    [Required]
    [MaxLength(255)]
    public string Name { get; set; } = null!;

    [MaxLength(2000)]
    public string? Description { get; set; }

    public decimal Price { get; set; }

    [MaxLength(5000)]
    public string? Images { get; set; } // JSON array as string

    public bool AvailableInEventMode { get; set; } = true;

    public bool EventModeOnly { get; set; } = false;

    public bool IsActive { get; set; } = true;

    public int SortOrder { get; set; } = 0;

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    // Navigation properties
    public virtual Catalog Catalog { get; set; } = null!;
}
