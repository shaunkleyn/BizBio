namespace BizBio.Core.Entities;

using BizBio.Core.Entities.Lookups;
using BizBio.Core.Enums;
using System.ComponentModel.DataAnnotations;

public class RestaurantTable
{
    public int Id { get; set; }

    public int ProfileId { get; set; }

    public int TableNumber { get; set; }

    [Required]
    [MaxLength(100)]
    public string TableName { get; set; } = null!;

    public int TableCategoryId { get; set; }

    [MaxLength(2000)]
    public string? FunFact { get; set; }

    [MaxLength(2000)]
    public string? Description { get; set; }

    [MaxLength(5000)]
    public string? Images { get; set; } // JSON array as string

    [Required]
    [MaxLength(50)]
    public string NFCTagCode { get; set; } = null!;

    public int NFCTagTypeId { get; set; }

    public int NFCTagStatusId { get; set; }

    public bool IsActive { get; set; } = true;

    public int SortOrder { get; set; } = 0;

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public int? CreatedBy { get; set; }

    // Navigation properties
    public virtual Profile Profile { get; set; } = null!;

    public virtual ICollection<NFCScan> NFCScans { get; set; } = new List<NFCScan>();
    public virtual TableCategoryLookup TableCategory { get; set; } = null!;
    public virtual NFCTagTypeLookup NFCTagType { get; set; } = null!;
    public virtual NFCTagStatusLookup NFCTagStatus { get; set; } = null!;
}
