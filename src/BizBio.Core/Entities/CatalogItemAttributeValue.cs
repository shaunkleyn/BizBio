using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BizBio.Core.Entities
{
    // COMMENTED OUT - Redundant with CatalogItemVariant system
    // CatalogItemVariant and CatalogItemVariantAttributeValue already handle this functionality

    /*
    public class CatalogItemAttributeValue : BaseEntity
    {
        //public long Id { get; set; }

        public int AttributeId { get; set; }
        public CatalogItemAttribute Attribute { get; set; } = null!;

        public string Value { get; set; } = null!; // e.g. 'Small', 'Medium', '250g'
        public int SortOrder { get; set; }
        public double? PriceDelta { get; set; }

        public ICollection<CatalogItemVariantAttributeValue> VariantAttributeValues { get; set; } = new List<CatalogItemVariantAttributeValue>();
    }
    */
}
