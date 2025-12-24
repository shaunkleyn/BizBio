using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BizBio.Core.Entities
{
    // COMMENTED OUT - Redundant with CatalogItemVariant system
    // CatalogItemVariant already handles product attributes (size, weight, pack size, etc.)
    // with proper support for different prices per variant

    /*
    public class CatalogItemAttribute : BaseEntity
    {
        //public long Id { get; set; }
        public string Name { get; set; } = null!; // -- e.g. 'Size', 'Weight', 'PackSize' || -- Cheese, Size, Crust Type, Gluten
        public string Slug { get; set; } = null!; // -- cheese, size, crust_type, allergen_gluten

        public int AttributeGroupId { get; set; }

        //public DateTime CreatedAt { get; set; }

        public ICollection<CatalogItemAttributeValue> AttributeValues { get; set; }
            = new List<CatalogItemAttributeValue>();
    }
    */
}
