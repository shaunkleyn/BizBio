namespace BizBio.Core.Entities;

using System.ComponentModel.DataAnnotations;
using BizBio.Core.Enums;
using BizBio.Core.Entities.Lookups;

public class NFCScan
{
    public int Id { get; set; }

    public int ProfileId { get; set; }

    public int? TableId { get; set; }

    [Required]
    [MaxLength(50)]
    public string NFCTagCode { get; set; } = null!;

    [Required]
    [MaxLength(45)]
    public string IPAddress { get; set; } = null!;

    [MaxLength(1000)]
    public string? UserAgent { get; set; }

    public int DeviceTypeId { get; set; }

    [MaxLength(100)]
    public string? Country { get; set; }

    [MaxLength(2)]
    public string? City { get; set; }

    public DateTime ScannedAt { get; set; }

    [MaxLength(100)]
    public string? SessionId { get; set; }

    // Navigation properties
    public virtual Profile Profile { get; set; } = null!;
    public virtual RestaurantTable? Table { get; set; }
    public virtual DeviceTypeLookup DeviceType { get; set; } = null!;
}