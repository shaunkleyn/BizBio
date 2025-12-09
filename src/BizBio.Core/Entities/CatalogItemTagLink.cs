using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BizBio.Core.Entities
{
    public class CatalogItemTagLink : BaseEntity
    {
        public int CatalogItemId { get; set; }
        public int CatalogItemTagId { get; set; }

        public CatalogItem CatalogItem { get; set; }
        public CatalogItemTag Tag { get; set; }
    }
}
