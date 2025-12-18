namespace BizBio.Core.Entities;

using System.ComponentModel.DataAnnotations;

public class CatalogBundleOptionGroup : BaseEntity
{
    public int StepId { get; set; }

    [Required]
    [MaxLength(255)]
    public string Name { get; set; } = null!;

    public bool IsRequired { get; set; } = false;

    public int MinSelect { get; set; } = 0;

    public int MaxSelect { get; set; } = 10;

    // Navigation properties
    public virtual CatalogBundleStep Step { get; set; } = null!;
    public virtual ICollection<CatalogBundleOption> Options { get; set; } = new List<CatalogBundleOption>();
}
