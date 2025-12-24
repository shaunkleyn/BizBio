using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BizBio.Infrastructure.Data.Seeding
{
    public sealed record ItemSeed(
    string CategoryName,
    string Name,
    string Description,
    decimal Price,
    int SortOrder,
    string Tags
);

}
