namespace BizBio.Core.Entities.Lookups;

public class SubscriptionStatusLookup : EnumLookup
{
    public ICollection<UserSubscription> UserSubscriptions { get; set; } = new List<UserSubscription>();
}
