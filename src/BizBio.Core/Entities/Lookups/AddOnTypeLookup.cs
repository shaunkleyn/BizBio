using System.Text.Json.Serialization;

namespace BizBio.Core.Entities.Lookups;

public class AddOnTypeLookup : EnumLookup
{
    [JsonIgnore]
    public ICollection<ProductAddOn> AddOns { get; set; } = new List<ProductAddOn>();
}
