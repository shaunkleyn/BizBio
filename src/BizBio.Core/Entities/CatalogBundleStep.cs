namespace BizBio.Core.Entities;

using System.ComponentModel.DataAnnotations;

public class CatalogBundleStep : BaseEntity
{
    public int BundleId { get; set; }

    public int StepNumber { get; set; }

    [Required]
    [MaxLength(255)]
    public string Name { get; set; } = null!;

    public int MinSelect { get; set; } = 1;

    public int MaxSelect { get; set; } = 1;

    // Navigation properties
    public virtual CatalogBundle Bundle { get; set; } = null!;
    public virtual ICollection<CatalogBundleStepProduct> AllowedProducts { get; set; } = new List<CatalogBundleStepProduct>();
    public virtual ICollection<CatalogBundleOptionGroup> OptionGroups { get; set; } = new List<CatalogBundleOptionGroup>();
}
