using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BizBio.Core.Entities
{
    public class CatalogItemInventory :BaseEntity
    {
        //public long Id { get; set; }

        public long VariantId { get; set; }
        public CatalogItemVariant Variant { get; set; } = null!;

        public long? LocationId { get; set; }

        public decimal QtyAvailable { get; set; }
        public decimal QtyReserved { get; set; }

        //public DateTime UpdatedAt { get; set; }
    }

}
