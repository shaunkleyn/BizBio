using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BizBio.Core.Entities
{
    // COMMENTED OUT - Redundant with CatalogCategory
    // CatalogCategory will be updated to support ParentCategoryId for hierarchical categories
    // Note: Typo in class name - should be "Category" not "Catagory"

    //Category
    // ├── parent category
    // └── child categories(infinite depth)

    //Catalog Item
    // └── can belong to MANY categories

    /*
    public class Catagory : BaseEntity
    {
        public string Name { get; set; }
        public string Slug { get; set; }
        public int? ParentCategoryId { get; set; }
        public int ProductId { get; set; }
    }
    */
}
