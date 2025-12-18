using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BizBio.Core.Entities
{
    public class CatalogItemAttribute : BaseEntity
    {
        //public long Id { get; set; }
        public string Name { get; set; } = null!; // -- e.g. 'Size', 'Weight', 'PackSize'
        public string Slug { get; set; } = null!;

        //public DateTime CreatedAt { get; set; }

        public ICollection<CatalogItemAttributeValue> AttributeValues { get; set; }
            = new List<CatalogItemAttributeValue>();
    }

}
