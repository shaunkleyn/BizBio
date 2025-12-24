using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BizBio.Core.Entities
{
    // NOTE: This junction table is actually NEEDED for many-to-many relationship
    // between CatalogItem and CatalogCategory (items can appear in multiple categories)
    // Will implement this properly
    [Table("CatalogItemCategory")]
    public class CatalogItemCategory : BaseEntity
    {
        public int CatalogItemId { get; set; }
        public int CategoryId { get; set; }

        // Navigation properties
        public virtual CatalogItem CatalogItem { get; set; } = null!;
        public virtual CatalogCategory Category { get; set; } = null!;
    }
}
