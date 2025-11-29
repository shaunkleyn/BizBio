using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BizBio.Core.Entities.Lookups;

public class BillingCycleLookup : EnumLookup
{
    public ICollection<UserSubscription> UserSubscriptions { get; set; } = new List<UserSubscription>();
}