namespace BizBio.Core.Entities;

using BizBio.Core.Entities.Lookups;
using BizBio.Core.Enums;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

public class Product
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

    [MaxLength(2000)]
    public string? LongDescription { get; set; }

    public int ProductTypeId { get; set; }

    public int ProductCategoryId { get; set; }

    public int? ProductLineId { get; set; }

    // Pricing
    public decimal Price { get; set; }

    public decimal? MonthlyPrice { get; set; }

    public decimal? AnnualPrice { get; set; }

    public decimal? AnnualDiscountPercent { get; set; }

    public decimal? SalePrice { get; set; }

    public decimal? CostPrice { get; set; }

    // Inventory (for physical products)
    public int? StockQuantity { get; set; }

    public int? LowStockThreshold { get; set; }

    public bool TrackInventory { get; set; }

    // Physical product details
    [MaxLength(50)]
    public string? Weight { get; set; }

    [MaxLength(100)]
    public string? Dimensions { get; set; }

    [MaxLength(50)]
    public string? Color { get; set; }

    [MaxLength(50)]
    public string? Material { get; set; }

    // Digital/Service details
    public int? DurationDays { get; set; }

    public bool IsRecurring { get; set; }

    public bool RequiresShipping { get; set; }

    public bool IsDigitalDelivery { get; set; }

    // Images and media
    [MaxLength(500)]
    public string? ImageUrl { get; set; }

    [MaxLength(500)]
    public string? ThumbnailUrl { get; set; }

    public string? ImageGallery { get; set; } // JSON array of image URLs

    // Display and metadata
    public int DisplayOrder { get; set; }

    public bool IsActive { get; set; }

    public bool IsFeatured { get; set; }

    public bool IsPopular { get; set; }

    [MaxLength(500)]
    public string? Tags { get; set; } // Comma-separated tags

    [MaxLength(200)]
    public string? MetaTitle { get; set; }

    [MaxLength(500)]
    public string? MetaDescription { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    // Navigation properties
    public virtual ProductTypeLookup ProductType { get; set; } = null!;
    public virtual ProductCategoryLookup ProductCategory { get; set; } = null!;
    public virtual ProductLineLookup? ProductLine { get; set; }

    [JsonIgnore]
    public virtual ICollection<ProductAddOn> AddOns { get; set; } = new List<ProductAddOn>();

    [JsonIgnore]
    public virtual ICollection<ProductAddOn> CompatibleAddOns { get; set; } = new List<ProductAddOn>();
}
