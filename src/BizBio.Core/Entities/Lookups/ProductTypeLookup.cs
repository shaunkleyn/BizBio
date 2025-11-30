using System.Text.Json.Serialization;

namespace BizBio.Core.Entities.Lookups;

public class ProductTypeLookup : EnumLookup
{
    [JsonIgnore]
    public ICollection<Product> Products { get; set; } = new List<Product>();
}
