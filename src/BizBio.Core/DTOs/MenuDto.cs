using BizBio.Core.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;

namespace BizBio.Core.DTOs
{
    public enum MenuItemUnitType
    {
        Size,
        Volume,
        Weight,
        Count,
        Length,
        Custom
    }

    public enum MenuItemPriceType
    {
        Standard,
        Discount,
        Special,
        HappyHour,
        Bulk
    }


    public class MenuDto
    {
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public string BusinessName { get; set; } = null!;
        public string? BusinessLogo { get; set; }
        public string Cuisine { get; set; } = null!;
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? Country { get; set; }
        public WorkingHoursDto? WorkingHours { get; set; }
        public List<MenuCategoryDto> Categories { get; set; } = new();
        
        public SubscriptionPlanDto? SubscriptionPlan { get; set; }
        public TrialDto? Trial { get; set; }
    }

    public class MenuCategoryDto
    {

        [MaxLength(255)]
        public string Name { get; set; } = null!;

        [MaxLength(2000)]
        public string? Description { get; set; }

        [MaxLength(100)]
        public string? Icon { get; set; }

        [MaxLength(5000)]
        public string? Images { get; set; } 

        public int SortOrder { get; set; } = 0;
        public List<MenuDishDto> Items { get; set; } = new();
    }

    public class MenuDishDto
    {
        public string CategoryId { get; set; } = null!; // Will be mapped to actual category ID

        // different sizes, weights, volumes, packs, variants
        public List<MenuItemVariantDto> Variants { get; set; } = new();
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public string? ImageUrl { get; set; }
        public List<string> Allergens { get; set; } = new();
        public List<string> Dietary { get; set; } = new();
        public bool Available { get; set; } = true;
        public bool Featured { get; set; } = false;
        //public DList<MenuItemProductTagDto> MenuDishTags { get; set; }
        public Dictionary<MenuItemTagCategoryDto, List<MenuItemTagDto>> MenuDishTags { get; set; }
    }

//    A product can have:

    //Small/Medium/Large

    //300ml / 500ml

    //250g / 500g

    //Single patty / double patty

    //Pack of 100 / pack of 1000

    //Per meter / per roll / per unit
    public class MenuItemVariantDto
    {
        public int Id { get; set; }

        // e.g. "Small", "Medium", "Large", "500ml", "Double Patty"
        public string Title { get; set; }

        // connection to standardized units/sizes
        public MenuItemVariantUnitDto Unit { get; set; }

        // multiple prices per variant (normal, special, discount, wholesale)
        public List<MenuItemVariantPriceDto> Prices { get; set; } = new();
    }

    public class MenuItemVariantUnitDto
    {
        public int Id { get; set; }

        public MenuItemUnitType UnitType { get; set; }   // enum: Size, Weight, Volume, Count, Length

        public double? Value { get; set; }       // 300ml → 300, 250g → 250, pack of 100 → 100

        public string Label { get; set; }        // "Small", "500ml", "250g", "Pack of 100"
    }

    public class MenuItemVariantPriceDto
    {
        public int Id { get; set; }

        public double Amount { get; set; }
        public string Currency { get; set; } = "ZAR";

        public MenuItemPriceType PriceType { get; set; } // Normal, HappyHour, Special, Bulk, etc.
    }

    //| id | name          | slug          |
    //| -- | ------------- | ------------- |
    //| 1  | Allergen      | allergen      |
    //| 2  | Dietary       | dietary       |
    //| 3  | Material      | material      |
    //| 4  | Certification | certification |

    public class MenuItemTagCategoryDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Slug { get; set; }

        public List<MenuItemTagDto> Tags { get; set; } = new();
    }

    //Allergens
    //| id | category_id | name      | slug      |
    //| -- | ----------- | --------- | --------- |
    //| 1  | 1           | Dairy     | dairy     |
    //| 2  | 1           | Gluten    | gluten    |
    //| 3  | 1           | Nuts      | nuts      |
    //| 4  | 1           | Shellfish | shellfish |

    //Dietary
    //| id | category_id | name       | slug       |
    //| -- | ----------- | ---------- | ---------- |
    //| 5  | 2           | Vegan      | vegan      |
    //| 6  | 2           | Halaal     | halaal     |
    //| 7  | 2           | Kosher     | kosher     |
    //| 8  | 2           | Vegetarian | vegetarian |

    //Material
    //| id | category_id | name      | slug      |
    //| -- | ----------- | --------- | --------- |
    //| 9  | 3           | Cotton    | cotton    |
    //| 10 | 3           | Plastic   | plastic   |
    //| 11 | 3           | Cardboard | cardboard |



    public class MenuItemTagDto
    {
        public long Id { get; set; }
        public long CategoryId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Slug { get; set; }

    }


    public class MenuItemProductTagDto
    {
        public long ProductId { get; set; }
        public long TagId { get; set; }

        public MenuItemDto Product { get; set; }
        public MenuItemTagDto Tag { get; set; }
    }



}
