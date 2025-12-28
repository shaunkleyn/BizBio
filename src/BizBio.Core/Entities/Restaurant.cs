using System.ComponentModel.DataAnnotations;

namespace BizBio.Core.Entities;

/// <summary>
/// Represents a restaurant or business location that can have multiple menus/profiles
/// </summary>
public class Restaurant : BaseEntity
{
    /// <summary>
    /// The user who owns this restaurant
    /// </summary>
    public int UserId { get; set; }

    /// <summary>
    /// Restaurant name (e.g., "The Cocktail Lounge", "Le Petit Bistro")
    /// </summary>
    [Required]
    [MaxLength(255)]
    public string Name { get; set; } = null!;

    /// <summary>
    /// Description of the restaurant
    /// </summary>
    [MaxLength(2000)]
    public string? Description { get; set; }

    /// <summary>
    /// Logo/brand image URL
    /// </summary>
    [MaxLength(500)]
    public string? Logo { get; set; }

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
    /// State/Province
    /// </summary>
    [MaxLength(50)]
    public string? State { get; set; }

    /// <summary>
    /// Postal/ZIP code
    /// </summary>
    [MaxLength(20)]
    public string? PostalCode { get; set; }

    /// <summary>
    /// Country code (ISO 3166-1 alpha-2)
    /// </summary>
    [MaxLength(2)]
    public string? Country { get; set; }

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

    /// <summary>
    /// Currency code (ISO 4217)
    /// </summary>
    [MaxLength(3)]
    public string Currency { get; set; } = "ZAR";

    /// <summary>
    /// Timezone identifier (IANA time zone database)
    /// </summary>
    [MaxLength(50)]
    public string? Timezone { get; set; }

    /// <summary>
    /// Display order for sorting
    /// </summary>
    public int SortOrder { get; set; } = 0;

    // Navigation properties

    /// <summary>
    /// The user who owns this restaurant
    /// </summary>
    public virtual User User { get; set; } = null!;

    /// <summary>
    /// Profiles/menus belonging to this restaurant
    /// </summary>
    public virtual ICollection<Profile> Profiles { get; set; } = new List<Profile>();
}
