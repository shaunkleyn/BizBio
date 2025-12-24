using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BizBio.Infrastructure.Data.Seeding
{
    public static class MenuData
    {
        public static readonly CategorySeed[] Categories =
        {
        new("Farm Fresh Breakfasts", "Available 9am - 11am", 1),
        new("Kids Exclusive", "12 years and under", 2),
        new("Starters", null, 3),
        new("Salads", null, 4),
        new("Burgers", null, 5),
        new("All Time Favourites", null, 6),
        new("Plant Based Meals", null, 7),
        new("Grills", null, 8),
        new("Seafood", null, 9),
        new("Pizza", "Hand-made, crispy, thin based pizzas prepared in our traditional wood fired oven", 10),
        new("Pasta", "Fresh hand-made La Pineta fettuccine", 11),
        new("Desserts", null, 12),
        new("Hot Beverages", null, 13),
        new("Cold Beverages", null, 14),
        new("Sides", null, 15)
    };

        public static readonly ItemSeed[] Items =
        {
        // BREAKFAST
 new("Farm Fresh Breakfasts", "The Cereal Killer", "'Two slices of toasted sourdough, two poached eggs, 2 rashers of bacon, tomatoes and mushrooms.'", 105.00m, 1, @"[""Eggs"",""Bacon"",""Sourdough""]"),

new("Farm Fresh Breakfasts", "The Popes Pig", "'Two slices of toasted ciabatta topped with parma ham, mushrooms, two poached eggs and a béarnaise sauce.'", 150.00m, 2, @"[""Eggs"",""Parma Ham"",""Ciabatta""]"),

new("Farm Fresh Breakfasts", "The Eggcited Royale", "'Two slices of toasted ciabatta topped with crème fraîche, smoked salmon, avocado, capers and two poached eggs.'", 150.00m, 3, @"[""Eggs"",""Smoked Salmon"",""Avocado"",""Ciabatta""]"),

new("Farm Fresh Breakfasts", "The Mexican Eggsplorer", "'Toasted sourdough topped with chilli mince, avocado, two poached eggs and a bacon popper.'", 140.00m, 4, @"[""Eggs"",""Spicy"",""Avocado"",""Bacon""]"),

new("Farm Fresh Breakfasts", "Lets Avocuddle", "'Toasted sourdough, hummus, blistered cherry tomatoes, mushrooms, avocado, rocket and balsamic glaze.'", 120.00m, 5, @"[""Vegan"",""Avocado"",""Sourdough""]"),

new("Farm Fresh Breakfasts", "Meet An Egg", "'Two slices of sourdough, two eggs, two rashers of bacon, 150g sirloin steak, tomatoes and mushrooms.'", 195.00m, 6, @"[""Eggs"",""Bacon"",""Steak""]"),

new("Farm Fresh Breakfasts", "Livin On The Veg", "'Toasted sourdough, basil pesto, caramelized onions, feta, sundried tomatoes, and avocado topped with Asian salad.'", 115.00m, 7, @"[""Vegetarian"",""Avocado"",""Feta""]"),

new("Farm Fresh Breakfasts", "Pancake My Eyes Off You (Single)", "'American-style pancake stack served with fresh berries, syrup and fresh cream.'", 85.00m, 8, @"[""Vegetarian"",""Pancakes"",""Sweet""]"),

new("Farm Fresh Breakfasts", "Pancake My Eyes Off You (Double)", "'American-style pancake stack served with fresh berries, syrup and fresh cream.'", 125.00m, 9, @"[""Vegetarian"",""Pancakes"",""Sweet""]"),

new("Farm Fresh Breakfasts", "Guac My World (Single)", "'Toasted sourdough, smashed avocado, poached egg and rocket.'", 65.00m, 10, @"[""Vegetarian"",""Avocado"",""Eggs""]"),

new("Farm Fresh Breakfasts", "Guac My World (Double)", "'Toasted sourdough, smashed avocado, poached egg and rocket.'", 95.00m, 11, @"[""Vegetarian"",""Avocado"",""Eggs""]"),



new("Kids Exclusive", "French Toast", "'With syrup and 2 rashers of bacon. 12 years and under'", 60.00m, 2, @"[""Kids"",""Sweet"",""Bacon""]"),

new("Kids Exclusive", "Box Juice", "'Kids beverage'", 26.00m, 3, @"[""Kids"",""Beverage""]"),

new("Kids Exclusive", "Kids Milkshake", "'Chocolate, vanilla, or strawberry'", 25.00m, 4, @"[""Kids"",""Beverage"",""Milkshake""]"),

new("Kids Exclusive", "Babycino", "'Kids coffee alternative'", 12.00m, 5, @"[""Kids"",""Beverage"",""Hot""]"),


new("Burgers", "Chicken Burger", "'Crumbed chicken breast, peri-peri mayo, red onion, lettuce, grilled pineapple, served with chips and onion rings.'", 140.00m, 2, @"[""Burger"",""Chicken""]"),



    };
    }

}
