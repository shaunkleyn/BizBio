using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BizBio.Core.Entities.Lookups;

public class NFCTagStatusLookup : EnumLookup
{
    public ICollection<RestaurantTable> RestaurantTables { get; set; } = new List<RestaurantTable>();
}