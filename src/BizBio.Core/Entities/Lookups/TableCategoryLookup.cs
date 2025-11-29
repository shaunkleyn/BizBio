namespace BizBio.Core.Entities.Lookups;

public class TableCategoryLookup : EnumLookup
{
    public ICollection<RestaurantTable> RestaurantTables { get; set; } = new List<RestaurantTable>();
}
