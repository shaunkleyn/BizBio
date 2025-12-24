using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BizBio.Infrastructure.Data.Seeding
{
    public sealed record CategorySeed(
    string Name,
    string? Description,
    int SortOrder
);

}
