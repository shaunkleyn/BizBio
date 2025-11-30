namespace BizBio.Core.Entities;

using BizBio.Core.Entities.Lookups;
using BizBio.Core.Enums;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

public class ProductAddOn
{
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = null!;

    [Required]
    [MaxLength(50)]
    public string SKU { get; set; } = null!;

    [MaxLength(500)]
    public string? Description { get; set; }

    public int AddOnTypeId { get; set; }

    public int? ProductId { get; set; } // If null, it's a global add-on available to all products

    // Pricing
    public decimal Price { get; set; }

    public decimal? MonthlyPrice { get; set; }

    public decimal? AnnualPrice { get; set; }

    public bool IsRecurring { get; set; }

    // For quantity-based add-ons (e.g., additional users, storage)
    public int? DefaultQuantity { get; set; }

    public int? MinQuantity { get; set; }

    public int? MaxQuantity { get; set; }

    [MaxLength(50)]
    public string? Unit { get; set; } // e.g., "GB", "users", "items"

    // Inventory (for physical add-ons)
    public int? StockQuantity { get; set; }

    public bool TrackInventory { get; set; }

    public bool RequiresShipping { get; set; }

    // Images
    [MaxLength(500)]
    public string? ImageUrl { get; set; }

    // Display and metadata
    public int DisplayOrder { get; set; }

    public bool IsActive { get; set; }

    public bool IsFeatured { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    // Navigation properties
    public virtual AddOnTypeLookup AddOnType { get; set; } = null!;

    [JsonIgnore]
    public virtual Product? Product { get; set; }

    // Many-to-many: Add-on can be applicable to multiple products
    [JsonIgnore]
    public virtual ICollection<Product> ApplicableProducts { get; set; } = new List<Product>();
}
