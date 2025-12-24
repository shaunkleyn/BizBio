using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BizBio.Core.Entities
{
    // COMMENTED OUT - Part of the attribute system that was redundant
    // CatalogItemVariant already has all necessary properties (Title, SizeValue, SizeUnit, Price, etc.)
    // to represent different variants without needing a complex attribute value system

    /*
    public class CatalogItemVariantAttributeValue : BaseEntity
    {
        public int VariantId { get; set; }
        public CatalogItemVariant Variant { get; set; } = null!;

        public int AttributeValueId { get; set; }
        public CatalogItemAttributeValue AttributeValue { get; set; } = null!;
    }
    */
}
