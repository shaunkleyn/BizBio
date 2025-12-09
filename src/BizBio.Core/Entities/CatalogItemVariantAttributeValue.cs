using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BizBio.Core.Entities
{
    public class CatalogItemVariantAttributeValue : BaseEntity
    {
        public long VariantId { get; set; }
        public CatalogItemVariant Variant { get; set; } = null!;

        public long AttributeValueId { get; set; }
        public AttributeValue AttributeValue { get; set; } = null!;
    }

}
