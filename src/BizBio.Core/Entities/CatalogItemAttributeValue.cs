using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BizBio.Core.Entities
{
    public class AttributeValue : BaseEntity
    {
        //public long Id { get; set; }

        public long AttributeId { get; set; }
        public Attribute Attribute { get; set; } = null!;

        public string Value { get; set; } = null!; // e.g. 'Small', 'Medium', '250g'
        public int SortOrder { get; set; }

        public ICollection<CatalogItemVariantAttributeValue> VariantAttributeValues { get; set; } = new List<CatalogItemVariantAttributeValue>();
    }

}
