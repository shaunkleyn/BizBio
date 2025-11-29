using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BizBio.Core.Entities.Lookups;

public class ProductLineLookup : EnumLookup
{
    [JsonIgnore]
    public ICollection<SubscriptionTier> SubscriptionTiers { get; set; } = new List<SubscriptionTier>();
}
