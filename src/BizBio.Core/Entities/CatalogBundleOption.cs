namespace BizBio.Core.Entities;

using System.ComponentModel.DataAnnotations;

public class CatalogBundleOption : BaseEntity
{
    public int OptionGroupId { get; set; }

    [Required]
    [MaxLength(255)]
    public string Name { get; set; } = null!;

    public decimal PriceModifier { get; set; } = 0M;

    public bool IsDefault { get; set; } = false;

    // Navigation properties
    public virtual CatalogBundleOptionGroup OptionGroup { get; set; } = null!;
}
