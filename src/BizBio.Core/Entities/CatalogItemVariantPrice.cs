using BizBio.Core.Entities.Lookups;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BizBio.Core.Entities
{
    public class CatalogItemVariantPrice : BaseEntity
    {
        //public long Id { get; set; }

        public int VariantId { get; set; }
        public CatalogItemVariant Variant { get; set; } = null!;

        public decimal Price { get; set; }

        public int? PriceTypeId { get; set; }
        public CatalogItemPriceType? PriceType { get; set; } // Normal, HappyHour, Special, Bulk, etc.


        public DateTime StartsAt { get; set; }
        public DateTime? EndsAt { get; set; }
    }

}
