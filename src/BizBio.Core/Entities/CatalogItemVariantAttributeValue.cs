using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BizBio.Core.Entities
{
    public class CatalogItemVariantAttributeValue : BaseEntity
    {
        public int VariantId { get; set; }
        public CatalogItemVariant Variant { get; set; } = null!;

        public int AttributeValueId { get; set; }
        public CatalogItemAttributeValue AttributeValue { get; set; } = null!;
    }

}
