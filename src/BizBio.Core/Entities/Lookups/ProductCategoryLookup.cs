using System.Text.Json.Serialization;

namespace BizBio.Core.Entities.Lookups;

public class ProductCategoryLookup : EnumLookup
{
    [JsonIgnore]
    public ICollection<Product> Products { get; set; } = new List<Product>();
}
