using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BizBio.Core.Entities
{
    //TagCategory
    //| id | name          | slug          |
    //| -- | ------------- | ------------- |
    //| 1  | Allergen      | allergen      |
    //| 2  | Dietary       | dietary       |
    //| 3  | Material      | material      |
    //| 4  | Certification | certification |
    //| 4  | Color         | color         |
    //| 4  | ClothingFit   | clothingfit   |
    //| 5  | FurnitureType | furnituretype |
    //| 6  | SafetyWarning | safetywarning |

    public class CatalogItemTagCategory : BaseEntity
    {
        //public long Id { get; set; }
        public string Name { get; set; }
        public string Slug { get; set; }

        public List<CatalogItemTag> Tags { get; set; } = new();
    }

}
