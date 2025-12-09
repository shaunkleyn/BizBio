using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BizBio.Core.Entities
{
    public class CatalogItemVariant : BaseEntity
    {
        //public long Id { get; set; }
        public long ProductId { get; set; }
        public CatalogItem CatalogItem { get; set; } = null!;

        public string? Sku { get; set; }
        public string Title { get; set; } = null!; //   "Small", "Double Patty", "250g"

        public decimal? SizeValue { get; set; } // optional numeric representation: 250, 1, 0.5
        public string? SizeUnit { get; set; } // optional unit for size: "g", "kg", "ml", "l", "oz", "lb"
        public string? UnitOfMeasure { get; set; } // canonical uom for inventory (e.g. 'ea', 'g', 'ml')

        public decimal Price { get; set; }
        public decimal? Cost { get; set; }

        public int? WeightG { get; set; } //optional physical weight for shipping

        public string? Barcode { get; set; }

        public bool IsDefault { get; set; }

        //public DateTime CreatedAt { get; set; }
        //public DateTime UpdatedAt { get; set; }

        public ICollection<CatalogItemVariantAttributeValue> VariantAttributeValues { get; set; } = new List<CatalogItemVariantAttributeValue>();

        public ICollection<CatalogItemInventory> Inventories { get; set; } = new List<CatalogItemInventory>();

        public ICollection<CatalogItemVariantPrice> Prices { get; set; }  = new List<CatalogItemVariantPrice>();
    }

}
