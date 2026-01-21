using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace BizBio.Core.Entities;

/// <summary>
/// Unified entity representing a business, restaurant, store, venue, or organization.
/// Replaces separate Restaurant/Company/Profile tables to support multi-product architecture.
/// </summary>
public class Entity : BaseEntity
{
    /// <summary>
    /// The user who owns this entity
    /// </summary>
    public int UserId { get; set; }

    /// <summary>
    /// Type of entity (Restaurant, Store, Venue, Organization)
    /// </summary>
    public EntityType EntityType { get; set; } = EntityType.Restaurant;

    /// <summary>
    /// Entity name (e.g., "The Cocktail Lounge", "Tech Store", "Event Venue")
    /// </summary>
    [Required]
    [MaxLength(255)]
    public string Name { get; set; } = null!;

    /// <summary>
    /// URL-friendly slug for public pages
    /// </summary>
    [Required]
    [MaxLength(255)]
    public string Slug { get; set; } = null!;

    /// <summary>
    /// Description of the entity
    /// </summary>
    [MaxLength(2000)]
    public string? Description { get; set; }

    /// <summary>
    /// Logo/brand image URL
    /// </summary>
    [MaxLength(500)]
    public string? Logo { get; set; }

    // Contact Information

    /// <summary>
    /// Physical address
    /// </summary>
    [MaxLength(255)]
    public string? Address { get; set; }

    /// <summary>
    /// City
    /// </summary>
    [MaxLength(100)]
    public string? City { get; set; }

    /// <summary>
    /// Postal/ZIP code
    /// </summary>
    [MaxLength(20)]
    public string? PostalCode { get; set; }

    /// <summary>
    /// Contact phone number
    /// </summary>
    [MaxLength(20)]
    public string? Phone { get; set; }

    /// <summary>
    /// Contact email address
    /// </summary>
    [MaxLength(255)]
    public string? Email { get; set; }

    /// <summary>
    /// Website URL
    /// </summary>
    [MaxLength(500)]
    public string? Website { get; set; }

    // Settings

    /// <summary>
    /// Currency code (ISO 4217) - e.g., ZAR, USD, EUR
    /// </summary>
    [MaxLength(3)]
    public string Currency { get; set; } = "ZAR";

    /// <summary>
    /// Timezone identifier (IANA time zone database) - e.g., Africa/Johannesburg
    /// </summary>
    [MaxLength(50)]
    public string Timezone { get; set; } = "Africa/Johannesburg";

    /// <summary>
    /// Display order for sorting
    /// </summary>
    public int SortOrder { get; set; } = 0;

    // Navigation properties

    /// <summary>
    /// The user who owns this entity
    /// </summary>
    [JsonIgnore]
    public virtual User User { get; set; } = null!;

    /// <summary>
    /// Catalogs/menus belonging to this entity
    /// </summary>
    [JsonIgnore]
    public virtual ICollection<Catalog> Catalogs { get; set; } = new List<Catalog>();

    /// <summary>
    /// Categories belonging to this entity
    /// </summary>
    [JsonIgnore]
    public virtual ICollection<Category> Categories { get; set; } = new List<Category>();
}

/// <summary>
/// Entity type enumeration to distinguish different business types
/// </summary>
public enum EntityType
{
    /// <summary>
    /// Restaurant, cafe, bar, etc. (Menu product)
    /// </summary>
    Restaurant = 0,

    /// <summary>
    /// Retail store, shop, etc. (Catalog product)
    /// </summary>
    Store = 1,

    /// <summary>
    /// Event venue, conference center, etc. (Menu product)
    /// </summary>
    Venue = 2,

    /// <summary>
    /// General organization, company, etc. (Cards product)
    /// </summary>
    Organization = 3
}
