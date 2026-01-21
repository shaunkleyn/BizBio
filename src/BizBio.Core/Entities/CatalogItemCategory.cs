using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BizBio.Core.Entities
{
    /// <summary>
    /// Junction table for many-to-many relationship between CatalogItem and Category
    /// Items can appear in multiple categories across different catalogs
    /// </summary>
    [Table("CatalogItemCategory")]
    public class CatalogItemCategory : BaseEntity
    {
        public int CatalogItemId { get; set; }
        public int CategoryId { get; set; }

        // Navigation properties
        public virtual CatalogItem CatalogItem { get; set; } = null!;
        /// <summary>
        /// Reference to the entity-level Category (not CatalogCategory which is now a junction table)
        /// </summary>
        public virtual Category Category { get; set; } = null!;
    }
}
