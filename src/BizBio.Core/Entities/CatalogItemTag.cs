using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BizBio.Core.Entities
{

//Tags:
//| id | category_id | name      | slug      |
//| -- | ----------- | --------- | --------- |
//| 1  | 1           | Dairy     | dairy     |
//| 2  | 1           | Gluten    | gluten    |
//| 3  | 1           | Nuts      | nuts      |
//| 4  | 1           | Shellfish | shellfish |

//| id | category_id | name       | slug       |
//| -- | ----------- | ---------- | ---------- |
//| 5  | 2           | Vegan      | vegan      |
//| 6  | 2           | Halaal     | halaal     |
//| 7  | 2           | Kosher     | kosher     |
//| 8  | 2           | Vegetarian | vegetarian |

//| id | category_id | name      | slug      |
//| -- | ----------- | --------- | --------- |
//| 9  | 3           | Cotton    | cotton    |
//| 10 | 3           | Plastic   | plastic   |
//| 11 | 3           | Cardboard | cardboard |


    public class CatalogItemTag : BaseEntity
    {
        //public long Id { get; set; }
        public long CategoryId { get; set; }
        public string Name { get; set; }
        public string Slug { get; set; }

        public CatalogItemTagCategory Category { get; set; }

        public List<CatalogItemTag> Tags { get; set; } = new();
    }
}