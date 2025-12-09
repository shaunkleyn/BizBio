namespace BizBio.Core.Entities;

using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

public class Profile
{
    public int Id { get; set; }

    public int UserId { get; set; }

    [Required]
    [MaxLength(255)]
    public string Slug { get; set; } = null!;

    [Required]
    [MaxLength(255)]
    public string Name { get; set; } = null!;

    [MaxLength(2000)]
    public string? Description { get; set; }

    [Required]
    [MaxLength(50)]
    public string ProfileType { get; set; } = null!; // "Professional", "Menu", "Retail"

    [MaxLength(500)]
    public string? Logo { get; set; }

    [MaxLength(20)]
    public string? ContactPhone { get; set; }

    [MaxLength(255)]
    public string? ContactEmail { get; set; }

    [MaxLength(500)]
    public string? Website { get; set; }

    public bool EventModeEnabled { get; set; } = false;

    [MaxLength(255)]
    public string? EventModeName { get; set; }

    [MaxLength(2000)]
    public string? EventModeDescription { get; set; }

    public bool HasMenuProAddon { get; set; } = false;

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    // Navigation properties
    [JsonIgnore]
    public virtual User User { get; set; } = null!;
    [JsonIgnore]
    public virtual ICollection<Catalog> Catalogs { get; set; } = new List<Catalog>();
    [JsonIgnore]
    public virtual ICollection<RestaurantTable> RestaurantTables { get; set; } = new List<RestaurantTable>();
}
