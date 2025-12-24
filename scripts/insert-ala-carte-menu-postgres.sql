-- ================================================================
-- LA PINETA 2025 ALA CARTE MENU - POSTGRESQL VERSION
-- ================================================================
-- This script inserts the full ala carte menu from La Pineta Restaurant
-- Using PostgreSQL syntax with auto-incrementing IDs
-- ================================================================

-- Assuming:
-- CatalogId = 1 (La Pineta's menu catalog)
-- Current timestamp will be used for CreatedAt/UpdatedAt
-- 'System' will be used for CreatedBy/UpdatedBy

BEGIN;

-- ================================================================
-- STEP 1: CREATE CATEGORIES
-- ================================================================

-- Insert categories and store their IDs in variables
DO $$
DECLARE
    cat_breakfast_id INT;
    cat_kids_id INT;
    cat_starters_id INT;
    cat_salads_id INT;
    cat_burgers_id INT;
    cat_mains_id INT;
    cat_plant_based_id INT;
    cat_grills_id INT;
    cat_seafood_id INT;
    cat_pizza_id INT;
    cat_pasta_id INT;
    cat_desserts_id INT;
    cat_beverages_hot_id INT;
    cat_beverages_cold_id INT;
    cat_sides_id INT;
BEGIN

-- Create Categories
INSERT INTO CatalogCategory (CatalogId, UserId, ParentCategoryId, Name, Description, Icon, Images, SortOrder, IsActive, CreatedAt, UpdatedAt, CreatedBy, UpdatedBy)
VALUES
(1, NULL, NULL, 'Farm Fresh Breakfasts', 'Available 9am - 11am', NULL, NULL, 1, TRUE, NOW(), NOW(), 'System', 'System')
RETURNING Id INTO cat_breakfast_id;

INSERT INTO CatalogCategory (CatalogId, UserId, ParentCategoryId, Name, Description, Icon, Images, SortOrder, IsActive, CreatedAt, UpdatedAt, CreatedBy, UpdatedBy)
VALUES
(1, NULL, NULL, 'Kids Exclusive', '12 years and under', NULL, NULL, 2, TRUE, NOW(), NOW(), 'System', 'System')
RETURNING Id INTO cat_kids_id;

INSERT INTO CatalogCategory (CatalogId, UserId, ParentCategoryId, Name, Description, Icon, Images, SortOrder, IsActive, CreatedAt, UpdatedAt, CreatedBy, UpdatedBy)
VALUES
(1, NULL, NULL, 'Starters', NULL, NULL, NULL, 3, TRUE, NOW(), NOW(), 'System', 'System')
RETURNING Id INTO cat_starters_id;

INSERT INTO CatalogCategory (CatalogId, UserId, ParentCategoryId, Name, Description, Icon, Images, SortOrder, IsActive, CreatedAt, UpdatedAt, CreatedBy, UpdatedBy)
VALUES
(1, NULL, NULL, 'Salads', NULL, NULL, NULL, 4, TRUE, NOW(), NOW(), 'System', 'System')
RETURNING Id INTO cat_salads_id;

INSERT INTO CatalogCategory (CatalogId, UserId, ParentCategoryId, Name, Description, Icon, Images, SortOrder, IsActive, CreatedAt, UpdatedAt, CreatedBy, UpdatedBy)
VALUES
(1, NULL, NULL, 'Burgers', NULL, NULL, NULL, 5, TRUE, NOW(), NOW(), 'System', 'System')
RETURNING Id INTO cat_burgers_id;

INSERT INTO CatalogCategory (CatalogId, UserId, ParentCategoryId, Name, Description, Icon, Images, SortOrder, IsActive, CreatedAt, UpdatedAt, CreatedBy, UpdatedBy)
VALUES
(1, NULL, NULL, 'All Time Favourites', NULL, NULL, NULL, 6, TRUE, NOW(), NOW(), 'System', 'System')
RETURNING Id INTO cat_mains_id;

INSERT INTO CatalogCategory (CatalogId, UserId, ParentCategoryId, Name, Description, Icon, Images, SortOrder, IsActive, CreatedAt, UpdatedAt, CreatedBy, UpdatedBy)
VALUES
(1, NULL, NULL, 'Plant Based Meals', NULL, NULL, NULL, 7, TRUE, NOW(), NOW(), 'System', 'System')
RETURNING Id INTO cat_plant_based_id;

INSERT INTO CatalogCategory (CatalogId, UserId, ParentCategoryId, Name, Description, Icon, Images, SortOrder, IsActive, CreatedAt, UpdatedAt, CreatedBy, UpdatedBy)
VALUES
(1, NULL, NULL, 'Grills', NULL, NULL, NULL, 8, TRUE, NOW(), NOW(), 'System', 'System')
RETURNING Id INTO cat_grills_id;

INSERT INTO CatalogCategory (CatalogId, UserId, ParentCategoryId, Name, Description, Icon, Images, SortOrder, IsActive, CreatedAt, UpdatedAt, CreatedBy, UpdatedBy)
VALUES
(1, NULL, NULL, 'Seafood', NULL, NULL, NULL, 9, TRUE, NOW(), NOW(), 'System', 'System')
RETURNING Id INTO cat_seafood_id;

INSERT INTO CatalogCategory (CatalogId, UserId, ParentCategoryId, Name, Description, Icon, Images, SortOrder, IsActive, CreatedAt, UpdatedAt, CreatedBy, UpdatedBy)
VALUES
(1, NULL, NULL, 'Pizza', 'Hand-made, crispy, thin based pizzas prepared in our traditional wood fired oven', NULL, NULL, 10, TRUE, NOW(), NOW(), 'System', 'System')
RETURNING Id INTO cat_pizza_id;

INSERT INTO CatalogCategory (CatalogId, UserId, ParentCategoryId, Name, Description, Icon, Images, SortOrder, IsActive, CreatedAt, UpdatedAt, CreatedBy, UpdatedBy)
VALUES
(1, NULL, NULL, 'Pasta', 'Fresh hand-made La Pineta fettuccine', NULL, NULL, 11, TRUE, NOW(), NOW(), 'System', 'System')
RETURNING Id INTO cat_pasta_id;

INSERT INTO CatalogCategory (CatalogId, UserId, ParentCategoryId, Name, Description, Icon, Images, SortOrder, IsActive, CreatedAt, UpdatedAt, CreatedBy, UpdatedBy)
VALUES
(1, NULL, NULL, 'Desserts', NULL, NULL, NULL, 12, TRUE, NOW(), NOW(), 'System', 'System')
RETURNING Id INTO cat_desserts_id;

INSERT INTO CatalogCategory (CatalogId, UserId, ParentCategoryId, Name, Description, Icon, Images, SortOrder, IsActive, CreatedAt, UpdatedAt, CreatedBy, UpdatedBy)
VALUES
(1, NULL, NULL, 'Hot Beverages', NULL, NULL, NULL, 13, TRUE, NOW(), NOW(), 'System', 'System')
RETURNING Id INTO cat_beverages_hot_id;

INSERT INTO CatalogCategory (CatalogId, UserId, ParentCategoryId, Name, Description, Icon, Images, SortOrder, IsActive, CreatedAt, UpdatedAt, CreatedBy, UpdatedBy)
VALUES
(1, NULL, NULL, 'Cold Beverages', NULL, NULL, NULL, 14, TRUE, NOW(), NOW(), 'System', 'System')
RETURNING Id INTO cat_beverages_cold_id;

INSERT INTO CatalogCategory (CatalogId, UserId, ParentCategoryId, Name, Description, Icon, Images, SortOrder, IsActive, CreatedAt, UpdatedAt, CreatedBy, UpdatedBy)
VALUES
(1, NULL, NULL, 'Sides', NULL, NULL, NULL, 15, TRUE, NOW(), NOW(), 'System', 'System')
RETURNING Id INTO cat_sides_id;

-- ================================================================
-- STEP 2: INSERT CATALOG ITEMS - BREAKFASTS
-- ================================================================

INSERT INTO CatalogItem (CatalogId, UserId, CategoryId, ItemType, BundleId, Name, Description, Price, Images, Tags, AvailableInEventMode, EventModeOnly, SortOrder, SourceLibraryItemId, IsActive, CreatedAt, UpdatedAt, CreatedBy, UpdatedBy)
VALUES
(1, NULL, cat_breakfast_id, 0, NULL, 'The Cereal Killer', 'Two slices of toasted sourdough, two poached eggs, 2 rashers of bacon, tomatoes and mushrooms.', 105.00, NULL, '[Eggs,Bacon,Sourdough]', TRUE, FALSE, 1, NULL, TRUE, NOW(), NOW(), 'System', 'System'),
(1, NULL, cat_breakfast_id, 0, NULL, 'The Pope''s Pig', 'Two slices of toasted ciabatta topped with parma ham, mushrooms, two poached eggs and a béarnaise sauce.', 150.00, NULL, '[Eggs,Parma Ham,Ciabatta]', TRUE, FALSE, 2, NULL, TRUE, NOW(), NOW(), 'System', 'System'),
(1, NULL, cat_breakfast_id, 0, NULL, 'The Eggcited Royale', 'Two slices of toasted ciabatta topped with crème fraîche, smoked salmon, avocado, capers and two poached eggs.', 150.00, NULL, '[Eggs,Smoked Salmon,Avocado,Ciabatta]', TRUE, FALSE, 3, NULL, TRUE, NOW(), NOW(), 'System', 'System'),
(1, NULL, cat_breakfast_id, 0, NULL, 'The Mexican Eggsplorer', 'Toasted sourdough topped with chilli mince, avocado, two poached eggs and a bacon popper.', 140.00, NULL, '[Eggs,Spicy,Avocado,Bacon]', TRUE, FALSE, 4, NULL, TRUE, NOW(), NOW(), 'System', 'System'),
(1, NULL, cat_breakfast_id, 0, NULL, 'Let''s Avocuddle', 'Toasted sourdough, hummus, blistered cherry tomatoes, mushrooms, avocado, rocket and balsamic glaze.', 120.00, NULL, '[Vegan,Avocado,Sourdough]', TRUE, FALSE, 5, NULL, TRUE, NOW(), NOW(), 'System', 'System'),
(1, NULL, cat_breakfast_id, 0, NULL, 'Meet An'' Egg', 'Two slices of sourdough, two eggs, two rashers of bacon, 150g sirloin steak, tomatoes and mushrooms.', 195.00, NULL, '[Eggs,Bacon,Steak]', TRUE, FALSE, 6, NULL, TRUE, NOW(), NOW(), 'System', 'System'),
(1, NULL, cat_breakfast_id, 0, NULL, 'Livin'' On The Veg', 'Toasted sourdough, basil pesto, caramelized onions, feta, sundried tomatoes, and avocado topped with Asian salad.', 115.00, NULL, '[Vegetarian,Avocado,Feta]', TRUE, FALSE, 7, NULL, TRUE, NOW(), NOW(), 'System', 'System'),
(1, NULL, cat_breakfast_id, 0, NULL, 'Pancake My Eyes Off You (Single)', 'American-style pancake stack served with fresh berries, syrup and fresh cream.', 85.00, NULL, '[Vegetarian,Pancakes,Sweet]', TRUE, FALSE, 8, NULL, TRUE, NOW(), NOW(), 'System', 'System'),
(1, NULL, cat_breakfast_id, 0, NULL, 'Pancake My Eyes Off You (Double)', 'American-style pancake stack served with fresh berries, syrup and fresh cream.', 125.00, NULL, '[Vegetarian,Pancakes,Sweet]', TRUE, FALSE, 9, NULL, TRUE, NOW(), NOW(), 'System', 'System'),
(1, NULL, cat_breakfast_id, 0, NULL, 'Guac My World (Single)', 'Toasted sourdough, smashed avocado, poached egg and rocket.', 65.00, NULL, '[Vegetarian,Avocado,Eggs]', TRUE, FALSE, 10, NULL, TRUE, NOW(), NOW(), 'System', 'System'),
(1, NULL, cat_breakfast_id, 0, NULL, 'Guac My World (Double)', 'Toasted sourdough, smashed avocado, poached egg and rocket.', 95.00, NULL, '[Vegetarian,Avocado,Eggs]', TRUE, FALSE, 11, NULL, TRUE, NOW(), NOW(), 'System', 'System');

-- ================================================================
-- STEP 3: INSERT CATALOG ITEMS - KIDS MENU
-- ================================================================

INSERT INTO CatalogItem (CatalogId, UserId, CategoryId, ItemType, BundleId, Name, Description, Price, Images, Tags, AvailableInEventMode, EventModeOnly, SortOrder, SourceLibraryItemId, IsActive, CreatedAt, UpdatedAt, CreatedBy, UpdatedBy)
VALUES
(1, NULL, cat_kids_id, 0, NULL, 'Egg, Bacon and Chips', '12 years and under', 60.00, NULL, '[Kids,Eggs,Bacon]', TRUE, FALSE, 1, NULL, TRUE, NOW(), NOW(), 'System', 'System'),
(1, NULL, cat_kids_id, 0, NULL, 'French Toast', 'With syrup and 2 rashers of bacon. 12 years and under', 60.00, NULL, '[Kids,Sweet,Bacon]', TRUE, FALSE, 2, NULL, TRUE, NOW(), NOW(), 'System', 'System'),
(1, NULL, cat_kids_id, 0, NULL, 'Box Juice', 'Kids beverage', 26.00, NULL, '[Kids,Beverage]', TRUE, FALSE, 3, NULL, TRUE, NOW(), NOW(), 'System', 'System'),
(1, NULL, cat_kids_id, 0, NULL, 'Kids Milkshake', 'Chocolate, vanilla, or strawberry', 25.00, NULL, '[Kids,Beverage,Milkshake]', TRUE, FALSE, 4, NULL, TRUE, NOW(), NOW(), 'System', 'System'),
(1, NULL, cat_kids_id, 0, NULL, 'Babycino', 'Kids coffee alternative', 12.00, NULL, '[Kids,Beverage,Hot]', TRUE, FALSE, 5, NULL, TRUE, NOW(), NOW(), 'System', 'System');

-- ================================================================
-- STEP 4: INSERT CATALOG ITEMS - STARTERS
-- ================================================================

INSERT INTO CatalogItem (CatalogId, UserId, CategoryId, ItemType, BundleId, Name, Description, Price, Images, Tags, AvailableInEventMode, EventModeOnly, SortOrder, SourceLibraryItemId, IsActive, CreatedAt, UpdatedAt, CreatedBy, UpdatedBy)
VALUES
(1, NULL, cat_starters_id, 0, NULL, 'Soup of the Day', 'Served with toasted home-made bread.', 0.00, NULL, '[Soup]', TRUE, FALSE, 1, NULL, TRUE, NOW(), NOW(), 'System', 'System'),
(1, NULL, cat_starters_id, 0, NULL, 'Caprese Salad', 'Buffalo mozzarella, sliced tomatoes, fresh basil, pesto and balsamic reduction.', 145.00, NULL, '[Vegetarian,Mozzarella,Tomato]', TRUE, FALSE, 2, NULL, TRUE, NOW(), NOW(), 'System', 'System'),
(1, NULL, cat_starters_id, 0, NULL, 'Carpaccio of Beef', 'Thinly sliced beef, topped with deep-fried capers, rocket, parmesan cheese, olive oil and lemon dressing.', 130.00, NULL, '[Beef,Raw]', TRUE, FALSE, 3, NULL, TRUE, NOW(), NOW(), 'System', 'System'),
(1, NULL, cat_starters_id, 0, NULL, 'Calamari', 'Flash-fried, and served with lemon and garlic aioli, cucumber, pickled red onion, and a soya chilli dressing.', 105.00, NULL, '[Seafood,Calamari]', TRUE, FALSE, 4, NULL, TRUE, NOW(), NOW(), 'System', 'System'),
(1, NULL, cat_starters_id, 0, NULL, 'Prawn Cocktail', 'Prawns tossed in a classic Marie Rose sauce, on shredded ice-berg lettuce served with avocado.', 150.00, NULL, '[Seafood,Prawns]', TRUE, FALSE, 5, NULL, TRUE, NOW(), NOW(), 'System', 'System'),
(1, NULL, cat_starters_id, 0, NULL, 'Brandy Flambéed Chicken Livers', 'Pan fried with bacon in a Dijon mustard cream sauce, and served with toasted La Pineta bread. A house speciality for over 20 years!', 105.00, NULL, '[Chicken Livers,Bacon]', TRUE, FALSE, 6, NULL, TRUE, NOW(), NOW(), 'System', 'System'),
(1, NULL, cat_starters_id, 0, NULL, 'West Coast Mussels', 'Mussels steamed in white wine and tossed in lemongrass, chilli, garlic, and coconut cream, served with toasted La Pineta bread.', 135.00, NULL, '[Seafood,Mussels]', TRUE, FALSE, 7, NULL, TRUE, NOW(), NOW(), 'System', 'System'),
(1, NULL, cat_starters_id, 0, NULL, 'Deep Fried Camembert (Half)', 'Served on a bed of leaves with cranberry compote, berry coulis and melba toast.', 85.00, NULL, '[Vegetarian,Cheese,Fried]', TRUE, FALSE, 8, NULL, TRUE, NOW(), NOW(), 'System', 'System'),
(1, NULL, cat_starters_id, 0, NULL, 'Deep Fried Camembert (Full)', 'Served on a bed of leaves with cranberry compote, berry coulis and melba toast.', 125.00, NULL, '[Vegetarian,Cheese,Fried]', TRUE, FALSE, 9, NULL, TRUE, NOW(), NOW(), 'System', 'System'),
(1, NULL, cat_starters_id, 0, NULL, 'Pork Belly Spring Rolls', 'Diced pork belly, apple and black cherry spring rolls served with a hoisin dipping sauce.', 85.00, NULL, '[Pork,Asian]', TRUE, FALSE, 10, NULL, TRUE, NOW(), NOW(), 'System', 'System'),
(1, NULL, cat_starters_id, 0, NULL, 'Asian Rice Paper Rolls', 'Stuffed with red cabbage, cucumber, carrots, peppers, coriander and mint, served with a soy and ginger dipping sauce.', 75.00, NULL, '[Vegan,Asian,Rice Paper]', TRUE, FALSE, 11, NULL, TRUE, NOW(), NOW(), 'System', 'System'),
(1, NULL, cat_starters_id, 0, NULL, 'Home-Made Fish Cakes', 'Fish cakes, guacamole, herb salad, pickled red onion, soy ginger sauce and aioli.', 85.00, NULL, '[Fish,Fish Cakes]', TRUE, FALSE, 12, NULL, TRUE, NOW(), NOW(), 'System', 'System'),
(1, NULL, cat_starters_id, 0, NULL, 'Jalapeño Poppers', 'Stuffed with cream cheese, crumbed and deep fried served with sweet chilli mayonnaise.', 70.00, NULL, '[Vegetarian,Spicy,Fried]', TRUE, FALSE, 13, NULL, TRUE, NOW(), NOW(), 'System', 'System'),
(1, NULL, cat_starters_id, 0, NULL, 'Bacon Poppers', 'Jalapeño poppers stuffed with dates and cream cheese, wrapped in bacon, crumbed and deep fried, served with a sweet chilli mayonnaise.', 85.00, NULL, '[Bacon,Spicy,Fried]', TRUE, FALSE, 14, NULL, TRUE, NOW(), NOW(), 'System', 'System');

-- ================================================================
-- STEP 5: INSERT CATALOG ITEMS - SALADS
-- ================================================================

INSERT INTO CatalogItem (CatalogId, UserId, CategoryId, ItemType, BundleId, Name, Description, Price, Images, Tags, AvailableInEventMode, EventModeOnly, SortOrder, SourceLibraryItemId, IsActive, CreatedAt, UpdatedAt, CreatedBy, UpdatedBy)
VALUES
(1, NULL, cat_salads_id, 0, NULL, 'Greek Salad', 'Mixed leaves, tomatoes, olives, cucumber, peppers, feta and red onion.', 105.00, NULL, '[Vegetarian,Salad,Greek]', TRUE, FALSE, 1, NULL, TRUE, NOW(), NOW(), 'System', 'System'),
(1, NULL, cat_salads_id, 0, NULL, 'Chicken, Bacon and Avo', 'Grilled chicken, bacon, avo on a tossed salad, with a creamy mustard dressing.', 165.00, NULL, '[Chicken,Bacon,Avocado,Salad]', TRUE, FALSE, 2, NULL, TRUE, NOW(), NOW(), 'System', 'System'),
(1, NULL, cat_salads_id, 0, NULL, 'Parma Ham, Pear and Camembert', 'Parma Ham, camembert cheese, fresh pears, and berry coulis on a tossed salad.', 185.00, NULL, '[Parma Ham,Cheese,Pear,Salad]', TRUE, FALSE, 3, NULL, TRUE, NOW(), NOW(), 'System', 'System'),
(1, NULL, cat_salads_id, 0, NULL, 'Smoked Salmon Salad', 'Smoked Norwegian Salmon, new potatoes, and soy, mustard mayo dressing on a tossed salad.', 205.00, NULL, '[Salmon,Salad,Fish]', TRUE, FALSE, 4, NULL, TRUE, NOW(), NOW(), 'System', 'System');

-- ================================================================
-- STEP 6: INSERT CATALOG ITEMS - BURGERS
-- ================================================================

INSERT INTO CatalogItem (CatalogId, UserId, CategoryId, ItemType, BundleId, Name, Description, Price, Images, Tags, AvailableInEventMode, EventModeOnly, SortOrder, SourceLibraryItemId, IsActive, CreatedAt, UpdatedAt, CreatedBy, UpdatedBy)
VALUES
(1, NULL, cat_burgers_id, 0, NULL, 'American Beef Burger', 'Home ground beef patty, topped with mustard mayo, pickles, red onion, tomato chutney and lettuce, served with chips and onion rings.', 150.00, NULL, '[Burger,Beef]', TRUE, FALSE, 1, NULL, TRUE, NOW(), NOW(), 'System', 'System'),
(1, NULL, cat_burgers_id, 0, NULL, 'Chicken Burger', 'Crumbed chicken breast, peri-peri mayo, red onion, lettuce, grilled pineapple, served with chips and onion rings.', 140.00, NULL, '[Burger,Chicken]', TRUE, FALSE, 2, NULL, TRUE, NOW(), NOW(), 'System', 'System'),
(1, NULL, cat_burgers_id, 0, NULL, '''Shroom Burger', 'Crumbed portobello mushroom filled with mozzarella and cream cheese, topped with avocado and sweet-chili mayo, served with chips and onion rings.', 175.00, NULL, '[Burger,Vegetarian,Mushroom]', TRUE, FALSE, 3, NULL, TRUE, NOW(), NOW(), 'System', 'System');

-- ================================================================
-- STEP 7: INSERT CATALOG ITEMS - ALL TIME FAVOURITES
-- ================================================================

INSERT INTO CatalogItem (CatalogId, UserId, CategoryId, ItemType, BundleId, Name, Description, Price, Images, Tags, AvailableInEventMode, EventModeOnly, SortOrder, SourceLibraryItemId, IsActive, CreatedAt, UpdatedAt, CreatedBy, UpdatedBy)
VALUES
(1, NULL, cat_mains_id, 0, NULL, 'Lamb Shank', 'Braised and slow cooked in the pizza oven in red wine sauce, served with mashed potatoes.', 395.00, NULL, '[Lamb,Slow Cooked]', TRUE, FALSE, 1, NULL, TRUE, NOW(), NOW(), 'System', 'System'),
(1, NULL, cat_mains_id, 0, NULL, 'Pork Belly', 'Slow roasted pork belly, red cabbage, apple puree, tomato chutney, soya jus, served with mashed potato.', 210.00, NULL, '[Pork,Slow Cooked]', TRUE, FALSE, 2, NULL, TRUE, NOW(), NOW(), 'System', 'System'),
(1, NULL, cat_mains_id, 0, NULL, 'Traditional Oxtail', 'Braised and slow-cooked in our pizza oven in red wine sauce, served with mashed potato.', 295.00, NULL, '[Oxtail,Slow Cooked]', TRUE, FALSE, 3, NULL, TRUE, NOW(), NOW(), 'System', 'System'),
(1, NULL, cat_mains_id, 0, NULL, 'Eisbein', 'Corned pork hock crisped in our pizza oven served with a whole grain mustard & ginger sauce, mashed potato and red cabbage.', 275.00, NULL, '[Pork,German]', TRUE, FALSE, 4, NULL, TRUE, NOW(), NOW(), 'System', 'System'),
(1, NULL, cat_mains_id, 0, NULL, 'Pork Schnitzel', 'Panko crumbed pork fillet with your choice of mushroom, pepper or cheese sauce, served with chips and vegetables.', 140.00, NULL, '[Pork,Schnitzel]', TRUE, FALSE, 5, NULL, TRUE, NOW(), NOW(), 'System', 'System'),
(1, NULL, cat_mains_id, 0, NULL, 'Chicken Schnitzel', 'Panko crumbed chicken breast with your choice of mushroom, pepper or cheese sauce, served with chips and vegetables.', 140.00, NULL, '[Chicken,Schnitzel]', TRUE, FALSE, 6, NULL, TRUE, NOW(), NOW(), 'System', 'System'),
(1, NULL, cat_mains_id, 0, NULL, 'Chicken Roulade', 'Chicken roulade stuffed with spinach and cream cheese and wrapped in bacon served on carrot puree and drizzled with port jus, served with chips.', 155.00, NULL, '[Chicken,Bacon]', TRUE, FALSE, 7, NULL, TRUE, NOW(), NOW(), 'System', 'System'),
(1, NULL, cat_mains_id, 0, NULL, 'Chicken and Prawn Curry', 'Cooked in coconut cream, with peppers, red onions, cashew nuts and basmati rice.', 255.00, NULL, '[Chicken,Prawns,Curry]', TRUE, FALSE, 8, NULL, TRUE, NOW(), NOW(), 'System', 'System'),
(1, NULL, cat_mains_id, 0, NULL, 'Nachos (Plain)', 'Mexican tortilla chips, topped with melted mozzarella, tomato sauce, guacamole, sour cream and tomato salsa.', 195.00, NULL, '[Vegetarian,Nachos,Mexican]', TRUE, FALSE, 9, NULL, TRUE, NOW(), NOW(), 'System', 'System'),
(1, NULL, cat_mains_id, 0, NULL, 'Mince Nachos', 'Mexican tortilla chips, topped with melted mozzarella, tomato sauce, guacamole, sour cream and tomato salsa.', 245.00, NULL, '[Mince,Nachos,Mexican]', TRUE, FALSE, 10, NULL, TRUE, NOW(), NOW(), 'System', 'System'),
(1, NULL, cat_mains_id, 0, NULL, 'Chicken Nachos', 'Mexican tortilla chips, topped with melted mozzarella, tomato sauce, guacamole, sour cream and tomato salsa.', 235.00, NULL, '[Chicken,Nachos,Mexican]', TRUE, FALSE, 11, NULL, TRUE, NOW(), NOW(), 'System', 'System');

-- ================================================================
-- STEP 8: INSERT CATALOG ITEMS - PLANT BASED MEALS
-- ================================================================

INSERT INTO CatalogItem (CatalogId, UserId, CategoryId, ItemType, BundleId, Name, Description, Price, Images, Tags, AvailableInEventMode, EventModeOnly, SortOrder, SourceLibraryItemId, IsActive, CreatedAt, UpdatedAt, CreatedBy, UpdatedBy)
VALUES
(1, NULL, cat_plant_based_id, 0, NULL, 'Vegan Burger', 'Topped with mustard mayo, pickles, red onion, tomato chutney and lettuce, served with chips and onion rings.', 125.00, NULL, '[Vegan,Burger]', TRUE, FALSE, 1, NULL, TRUE, NOW(), NOW(), 'System', 'System'),
(1, NULL, cat_plant_based_id, 0, NULL, 'Skinny Mexican Nachos', 'Tortilla chips, topped with cheez sauce, served with chili beans, guacamole and tomato salsa.', 185.00, NULL, '[Vegan,Nachos,Mexican]', TRUE, FALSE, 2, NULL, TRUE, NOW(), NOW(), 'System', 'System'),
(1, NULL, cat_plant_based_id, 0, NULL, 'Vegetable Curry', 'Freshly sautéed vegetables in a medium curry sauce, served with basmati rice, and sambals.', 145.00, NULL, '[Vegan,Curry]', TRUE, FALSE, 3, NULL, TRUE, NOW(), NOW(), 'System', 'System'),
(1, NULL, cat_plant_based_id, 0, NULL, 'Falafel Bowl', 'Falafels, hummus, basmati rice, chili beans, tomato and onion salsa, red cabbage, avocado, and coriander.', 155.00, NULL, '[Vegan,Falafel,Bowl]', TRUE, FALSE, 4, NULL, TRUE, NOW(), NOW(), 'System', 'System'),
(1, NULL, cat_plant_based_id, 0, NULL, 'Aubergine Bolognese', 'With spaghetti, topped with nutritional yeast.', 145.00, NULL, '[Vegan,Pasta,Italian]', TRUE, FALSE, 5, NULL, TRUE, NOW(), NOW(), 'System', 'System'),
(1, NULL, cat_plant_based_id, 0, NULL, 'Spaghetti Di Bosco', 'Onions, garlic, chili, mushrooms, peppers, sundried tomatoes, parsley, and olive oil, with spaghetti, topped with pine kernels and nutritional yeast.', 145.00, NULL, '[Vegan,Pasta,Italian]', TRUE, FALSE, 6, NULL, TRUE, NOW(), NOW(), 'System', 'System'),
(1, NULL, cat_plant_based_id, 0, NULL, 'Garden of Eatin'' Pizza', 'Tomato base, hummus, courgettes, onions, peppers, mushrooms, rocket', 135.00, NULL, '[Vegan,Pizza]', TRUE, FALSE, 7, NULL, TRUE, NOW(), NOW(), 'System', 'System');

-- ================================================================
-- STEP 9: INSERT CATALOG ITEMS - GRILLS
-- ================================================================

INSERT INTO CatalogItem (CatalogId, UserId, CategoryId, ItemType, BundleId, Name, Description, Price, Images, Tags, AvailableInEventMode, EventModeOnly, SortOrder, SourceLibraryItemId, IsActive, CreatedAt, UpdatedAt, CreatedBy, UpdatedBy)
VALUES
(1, NULL, cat_grills_id, 0, NULL, 'Fillet Béarnaise 200g', 'On a grilled brown mushroom topped with caramelised onions, and whole grain Dijon béarnaise sauce. Served with your choice of chips or seasonal vegetables or garden salad.', 325.00, NULL, '[Beef,Fillet,Steak]', TRUE, FALSE, 1, NULL, TRUE, NOW(), NOW(), 'System', 'System'),
(1, NULL, cat_grills_id, 0, NULL, 'Fillet Béarnaise 300g', 'On a grilled brown mushroom topped with caramelised onions, and whole grain Dijon béarnaise sauce. Served with your choice of chips or seasonal vegetables or garden salad.', 385.00, NULL, '[Beef,Fillet,Steak]', TRUE, FALSE, 2, NULL, TRUE, NOW(), NOW(), 'System', 'System'),
(1, NULL, cat_grills_id, 0, NULL, 'Fillet La Pineta 200g', 'On pan-fried brown mushrooms with caramelised onions, garden peas and port jus under a nest of straw potatoes. Served with your choice of chips or seasonal vegetables or garden salad.', 325.00, NULL, '[Beef,Fillet,Steak]', TRUE, FALSE, 3, NULL, TRUE, NOW(), NOW(), 'System', 'System'),
(1, NULL, cat_grills_id, 0, NULL, 'Fillet La Pineta 300g', 'On pan-fried brown mushrooms with caramelised onions, garden peas and port jus under a nest of straw potatoes. Served with your choice of chips or seasonal vegetables or garden salad.', 385.00, NULL, '[Beef,Fillet,Steak]', TRUE, FALSE, 4, NULL, TRUE, NOW(), NOW(), 'System', 'System'),
(1, NULL, cat_grills_id, 0, NULL, 'Pepper Fillet 200g', 'Rolled in crushed pepper, grilled and pan-fried in a rich cream and brandy sauce flambéed at your table. Served with your choice of chips or seasonal vegetables or garden salad.', 325.00, NULL, '[Beef,Fillet,Steak,Pepper]', TRUE, FALSE, 5, NULL, TRUE, NOW(), NOW(), 'System', 'System'),
(1, NULL, cat_grills_id, 0, NULL, 'Pepper Fillet 300g', 'Rolled in crushed pepper, grilled and pan-fried in a rich cream and brandy sauce flambéed at your table. Served with your choice of chips or seasonal vegetables or garden salad.', 385.00, NULL, '[Beef,Fillet,Steak,Pepper]', TRUE, FALSE, 6, NULL, TRUE, NOW(), NOW(), 'System', 'System'),
(1, NULL, cat_grills_id, 0, NULL, 'Gorgonzola Fillet 200g', 'Grilled and topped with caramelised onions and creamy gorgonzola cheese. Served with your choice of chips or seasonal vegetables or garden salad.', 325.00, NULL, '[Beef,Fillet,Steak,Gorgonzola]', TRUE, FALSE, 7, NULL, TRUE, NOW(), NOW(), 'System', 'System'),
(1, NULL, cat_grills_id, 0, NULL, 'Gorgonzola Fillet 300g', 'Grilled and topped with caramelised onions and creamy gorgonzola cheese. Served with your choice of chips or seasonal vegetables or garden salad.', 385.00, NULL, '[Beef,Fillet,Steak,Gorgonzola]', TRUE, FALSE, 8, NULL, TRUE, NOW(), NOW(), 'System', 'System'),
(1, NULL, cat_grills_id, 0, NULL, 'Fillet 200g', 'Grilled and basted. Served with your choice of chips or seasonal vegetables or garden salad.', 260.00, NULL, '[Beef,Fillet,Steak]', TRUE, FALSE, 9, NULL, TRUE, NOW(), NOW(), 'System', 'System'),
(1, NULL, cat_grills_id, 0, NULL, 'Fillet 300g', 'Grilled and basted. Served with your choice of chips or seasonal vegetables or garden salad.', 350.00, NULL, '[Beef,Fillet,Steak]', TRUE, FALSE, 10, NULL, TRUE, NOW(), NOW(), 'System', 'System'),
(1, NULL, cat_grills_id, 0, NULL, 'Fillet 500g', 'Grilled and basted. Served with your choice of chips or seasonal vegetables or garden salad.', 495.00, NULL, '[Beef,Fillet,Steak]', TRUE, FALSE, 11, NULL, TRUE, NOW(), NOW(), 'System', 'System'),
(1, NULL, cat_grills_id, 0, NULL, 'Sirloin 200g', 'Grilled and basted. Served with your choice of chips or seasonal vegetables or garden salad.', 165.00, NULL, '[Beef,Sirloin,Steak]', TRUE, FALSE, 12, NULL, TRUE, NOW(), NOW(), 'System', 'System'),
(1, NULL, cat_grills_id, 0, NULL, 'Sirloin 300g', 'Grilled and basted. Served with your choice of chips or seasonal vegetables or garden salad.', 195.00, NULL, '[Beef,Sirloin,Steak]', TRUE, FALSE, 13, NULL, TRUE, NOW(), NOW(), 'System', 'System'),
(1, NULL, cat_grills_id, 0, NULL, 'Rump 200g', 'Grilled and basted. Served with your choice of chips or seasonal vegetables or garden salad.', 165.00, NULL, '[Beef,Rump,Steak]', TRUE, FALSE, 14, NULL, TRUE, NOW(), NOW(), 'System', 'System'),
(1, NULL, cat_grills_id, 0, NULL, 'Rump 300g', 'Grilled and basted. Served with your choice of chips or seasonal vegetables or garden salad.', 195.00, NULL, '[Beef,Rump,Steak]', TRUE, FALSE, 15, NULL, TRUE, NOW(), NOW(), 'System', 'System'),
(1, NULL, cat_grills_id, 0, NULL, 'Rump 500g', 'Grilled and basted. Served with your choice of chips or seasonal vegetables or garden salad.', 265.00, NULL, '[Beef,Rump,Steak]', TRUE, FALSE, 16, NULL, TRUE, NOW(), NOW(), 'System', 'System');

-- ================================================================
-- STEP 10: INSERT CATALOG ITEMS - SEAFOOD
-- ================================================================

INSERT INTO CatalogItem (CatalogId, UserId, CategoryId, ItemType, BundleId, Name, Description, Price, Images, Tags, AvailableInEventMode, EventModeOnly, SortOrder, SourceLibraryItemId, IsActive, CreatedAt, UpdatedAt, CreatedBy, UpdatedBy)
VALUES
(1, NULL, cat_seafood_id, 0, NULL, 'Calamari Two Ways', 'Calamari tubes and strips lightly crumbed and deep fried, served with tartare sauce, chips and salad.', 230.00, NULL, '[Seafood,Calamari]', TRUE, FALSE, 1, NULL, TRUE, NOW(), NOW(), 'System', 'System'),
(1, NULL, cat_seafood_id, 0, NULL, 'Prawn Platter', 'Eight queen size prawns served with chips, rice, salad and your choice of lemon butter, garlic butter or peri peri sauce.', 320.00, NULL, '[Seafood,Prawns]', TRUE, FALSE, 2, NULL, TRUE, NOW(), NOW(), 'System', 'System'),
(1, NULL, cat_seafood_id, 0, NULL, 'Seafood Platter for One', 'Four queen size prawns, line fish, flash-fried calamari, and mussels tossed in lemongrass, chilli, garlic, and coconut cream, served with chips, and rice, and a choice of lemon butter, garlic butter or peri peri sauce.', 415.00, NULL, '[Seafood,Mixed Platter]', TRUE, FALSE, 3, NULL, TRUE, NOW(), NOW(), 'System', 'System'),
(1, NULL, cat_seafood_id, 0, NULL, 'Catch of the Day', 'Prepared to compliment the fish.', 0.00, NULL, '[Seafood,Fish,Market Price]', TRUE, FALSE, 4, NULL, TRUE, NOW(), NOW(), 'System', 'System');

-- ================================================================
-- STEP 11: INSERT CATALOG ITEMS - PIZZA
-- ================================================================

INSERT INTO CatalogItem (CatalogId, UserId, CategoryId, ItemType, BundleId, Name, Description, Price, Images, Tags, AvailableInEventMode, EventModeOnly, SortOrder, SourceLibraryItemId, IsActive, CreatedAt, UpdatedAt, CreatedBy, UpdatedBy)
VALUES
(1, NULL, cat_pizza_id, 0, NULL, 'Focaccia', 'Herbs, garlic and mozzarella.', 55.00, NULL, '[Vegetarian,Pizza]', TRUE, FALSE, 1, NULL, TRUE, NOW(), NOW(), 'System', 'System'),
(1, NULL, cat_pizza_id, 0, NULL, 'Margherita', 'Tomato base, mozzarella.', 85.00, NULL, '[Vegetarian,Pizza]', TRUE, FALSE, 2, NULL, TRUE, NOW(), NOW(), 'System', 'System'),
(1, NULL, cat_pizza_id, 0, NULL, 'Garden of Eatin''', 'Tomato base, hummus, courgettes, onions, peppers, mushrooms, rocket', 135.00, NULL, '[Vegan,Pizza]', TRUE, FALSE, 3, NULL, TRUE, NOW(), NOW(), 'System', 'System'),
(1, NULL, cat_pizza_id, 0, NULL, 'Dominica', 'Chicken, bacon and onion.', 145.00, NULL, '[Pizza,Chicken,Bacon]', TRUE, FALSE, 4, NULL, TRUE, NOW(), NOW(), 'System', 'System'),
(1, NULL, cat_pizza_id, 0, NULL, 'Hawaiian', 'Ham and pineapple.', 115.00, NULL, '[Pizza,Ham,Pineapple]', TRUE, FALSE, 5, NULL, TRUE, NOW(), NOW(), 'System', 'System'),
(1, NULL, cat_pizza_id, 0, NULL, 'The Oxtail', 'Oxtail, caramelised onion and mushroom.', 235.00, NULL, '[Pizza,Oxtail]', TRUE, FALSE, 6, NULL, TRUE, NOW(), NOW(), 'System', 'System'),
(1, NULL, cat_pizza_id, 0, NULL, 'Parma', 'Parma ham, caramelised onion and rocket.', 165.00, NULL, '[Pizza,Parma Ham]', TRUE, FALSE, 7, NULL, TRUE, NOW(), NOW(), 'System', 'System'),
(1, NULL, cat_pizza_id, 0, NULL, 'Napolitano', 'Anchovies, olives, peppers.', 170.00, NULL, '[Pizza,Anchovies]', TRUE, FALSE, 8, NULL, TRUE, NOW(), NOW(), 'System', 'System'),
(1, NULL, cat_pizza_id, 0, NULL, 'La Pineta', 'Spinach, feta, onion, pine kernels and garlic.', 160.00, NULL, '[Vegetarian,Pizza]', TRUE, FALSE, 9, NULL, TRUE, NOW(), NOW(), 'System', 'System'),
(1, NULL, cat_pizza_id, 0, NULL, 'Mexican', 'Mince, chilli, avo and sour cream.', 205.00, NULL, '[Pizza,Mince,Spicy]', TRUE, FALSE, 10, NULL, TRUE, NOW(), NOW(), 'System', 'System'),
(1, NULL, cat_pizza_id, 0, NULL, 'Mediterranean', 'Chicken, peppadews, feta, and avo.', 185.00, NULL, '[Pizza,Chicken]', TRUE, FALSE, 11, NULL, TRUE, NOW(), NOW(), 'System', 'System'),
(1, NULL, cat_pizza_id, 0, NULL, 'Regina', 'Ham, mushroom.', 115.00, NULL, '[Pizza,Ham]', TRUE, FALSE, 12, NULL, TRUE, NOW(), NOW(), 'System', 'System'),
(1, NULL, cat_pizza_id, 0, NULL, 'Quatro', 'Olives, mushrooms, artichoke and salami.', 195.00, NULL, '[Pizza,Salami]', TRUE, FALSE, 13, NULL, TRUE, NOW(), NOW(), 'System', 'System'),
(1, NULL, cat_pizza_id, 0, NULL, 'Tropical', 'Bacon and avo.', 140.00, NULL, '[Pizza,Bacon,Avocado]', TRUE, FALSE, 14, NULL, TRUE, NOW(), NOW(), 'System', 'System'),
(1, NULL, cat_pizza_id, 0, NULL, 'Pesto Perfection', 'Feta, sun-dried tomatoes, avo and basil pesto.', 185.00, NULL, '[Vegetarian,Pizza]', TRUE, FALSE, 15, NULL, TRUE, NOW(), NOW(), 'System', 'System'),
(1, NULL, cat_pizza_id, 0, NULL, 'Sir Bashim', 'Bacon, caramelised onions, creamy gorgonzola and avo.', 195.00, NULL, '[Pizza,Bacon,Gorgonzola]', TRUE, FALSE, 16, NULL, TRUE, NOW(), NOW(), 'System', 'System'),
(1, NULL, cat_pizza_id, 0, NULL, 'Lady Lerina', 'Caramelized onions, gorgonzola, fresh tomato and rocket.', 160.00, NULL, '[Vegetarian,Pizza,Gorgonzola]', TRUE, FALSE, 17, NULL, TRUE, NOW(), NOW(), 'System', 'System');

-- ================================================================
-- STEP 12: INSERT CATALOG ITEMS - PASTA
-- ================================================================

INSERT INTO CatalogItem (CatalogId, UserId, CategoryId, ItemType, BundleId, Name, Description, Price, Images, Tags, AvailableInEventMode, EventModeOnly, SortOrder, SourceLibraryItemId, IsActive, CreatedAt, UpdatedAt, CreatedBy, UpdatedBy)
VALUES
(1, NULL, cat_pasta_id, 0, NULL, 'Carbonara', 'Bacon, egg and cream served with hand-made fettuccine.', 130.00, NULL, '[Pasta,Bacon,Cream]', TRUE, FALSE, 1, NULL, TRUE, NOW(), NOW(), 'System', 'System'),
(1, NULL, cat_pasta_id, 0, NULL, 'Bolognese', 'Traditional meat ragu served with hand-made fettuccine.', 135.00, NULL, '[Pasta,Meat]', TRUE, FALSE, 2, NULL, TRUE, NOW(), NOW(), 'System', 'System'),
(1, NULL, cat_pasta_id, 0, NULL, 'La Pineta Pasta', 'Thinly sliced fillet, mushroom, bacon, peas, roast tomato, courgette and cream served with hand-made fettuccine.', 255.00, NULL, '[Pasta,Fillet,Cream]', TRUE, FALSE, 3, NULL, TRUE, NOW(), NOW(), 'System', 'System'),
(1, NULL, cat_pasta_id, 0, NULL, 'Mediterranean Pasta', 'Cherry tomatoes, onion, feta, olives and pesto with hand-made fettuccine.', 165.00, NULL, '[Vegetarian,Pasta]', TRUE, FALSE, 4, NULL, TRUE, NOW(), NOW(), 'System', 'System'),
(1, NULL, cat_pasta_id, 0, NULL, 'Fettuccine Vegetali', 'Roasted vegetables, feta and sun dried tomatoes tossed in a sage béchamel sauce with hand-made fettuccine, on a rich marinara sauce.', 145.00, NULL, '[Vegetarian,Pasta]', TRUE, FALSE, 5, NULL, TRUE, NOW(), NOW(), 'System', 'System'),
(1, NULL, cat_pasta_id, 0, NULL, 'Fettuccine Di Pesce', 'Chef''s selection of calamari, crab sticks, mussels, and prawns with hand-made fettuccine, tossed in a creamy marinara sauce with fresh basil.', 265.00, NULL, '[Pasta,Seafood]', TRUE, FALSE, 6, NULL, TRUE, NOW(), NOW(), 'System', 'System'),
(1, NULL, cat_pasta_id, 0, NULL, 'Aubergine Bolognese', 'With spaghetti, topped with nutritional yeast.', 145.00, NULL, '[Vegan,Pasta]', TRUE, FALSE, 7, NULL, TRUE, NOW(), NOW(), 'System', 'System'),
(1, NULL, cat_pasta_id, 0, NULL, 'Spaghetti Di Bosco', 'Onions, garlic, chili, mushrooms, peppers, sundried tomato, parsley, and olive oil, with spaghetti, topped with pine kernels and nutritional yeast.', 145.00, NULL, '[Vegan,Pasta]', TRUE, FALSE, 8, NULL, TRUE, NOW(), NOW(), 'System', 'System');

-- ================================================================
-- STEP 13: INSERT CATALOG ITEMS - DESSERTS
-- ================================================================

INSERT INTO CatalogItem (CatalogId, UserId, CategoryId, ItemType, BundleId, Name, Description, Price, Images, Tags, AvailableInEventMode, EventModeOnly, SortOrder, SourceLibraryItemId, IsActive, CreatedAt, UpdatedAt, CreatedBy, UpdatedBy)
VALUES
(1, NULL, cat_desserts_id, 0, NULL, 'Lindt Chocolate Torte', 'Served with vanilla ice cream.', 95.00, NULL, '[Dessert,Chocolate]', TRUE, FALSE, 1, NULL, TRUE, NOW(), NOW(), 'System', 'System'),
(1, NULL, cat_desserts_id, 0, NULL, 'Baked Cheesecake', 'Passion fruit coulis, fresh berries, and vanilla ice cream.', 79.00, NULL, '[Dessert,Cheesecake]', TRUE, FALSE, 2, NULL, TRUE, NOW(), NOW(), 'System', 'System'),
(1, NULL, cat_desserts_id, 0, NULL, 'Crème Brûlée', 'Silky vanilla custard with caramelised sugar topping and shortbread.', 79.00, NULL, '[Dessert,French]', TRUE, FALSE, 3, NULL, TRUE, NOW(), NOW(), 'System', 'System'),
(1, NULL, cat_desserts_id, 0, NULL, 'Traditional Malva Pudding', 'Vanilla custard and vanilla ice cream.', 79.00, NULL, '[Dessert,South African]', TRUE, FALSE, 4, NULL, TRUE, NOW(), NOW(), 'System', 'System'),
(1, NULL, cat_desserts_id, 0, NULL, 'Panna Cotta', 'Traditional Italian vanilla cream, raspberry coulis, chocolate soil.', 79.00, NULL, '[Dessert,Italian]', TRUE, FALSE, 5, NULL, TRUE, NOW(), NOW(), 'System', 'System'),
(1, NULL, cat_desserts_id, 0, NULL, 'Eton Mess', 'Chantilly whipped cream, berries and crushed meringue.', 79.00, NULL, '[Dessert,British]', TRUE, FALSE, 6, NULL, TRUE, NOW(), NOW(), 'System', 'System'),
(1, NULL, cat_desserts_id, 0, NULL, 'Ice Cream and Chocolate Sauce', 'Three scoops of vanilla ice cream, with chocolate sauce.', 55.00, NULL, '[Dessert,Ice Cream]', TRUE, FALSE, 7, NULL, TRUE, NOW(), NOW(), 'System', 'System'),
(1, NULL, cat_desserts_id, 0, NULL, 'Dom Pedro', 'A favourite South African dessert beverage of vanilla ice cream blended with a shot of your preferred liquer.', 69.00, NULL, '[Dessert,Beverage,Alcoholic]', TRUE, FALSE, 8, NULL, TRUE, NOW(), NOW(), 'System', 'System'),
(1, NULL, cat_desserts_id, 0, NULL, 'Irish Coffee', 'A classic combination of sweetened coffee, Irish whiskey, and whipped cream.', 69.00, NULL, '[Dessert,Beverage,Alcoholic]', TRUE, FALSE, 9, NULL, TRUE, NOW(), NOW(), 'System', 'System');

-- ================================================================
-- STEP 14: INSERT CATALOG ITEMS - HOT BEVERAGES
-- ================================================================

INSERT INTO CatalogItem (CatalogId, UserId, CategoryId, ItemType, BundleId, Name, Description, Price, Images, Tags, AvailableInEventMode, EventModeOnly, SortOrder, SourceLibraryItemId, IsActive, CreatedAt, UpdatedAt, CreatedBy, UpdatedBy)
VALUES
(1, NULL, cat_beverages_hot_id, 0, NULL, 'Americano', NULL, 28.00, NULL, '[Coffee,Hot]', TRUE, FALSE, 1, NULL, TRUE, NOW(), NOW(), 'System', 'System'),
(1, NULL, cat_beverages_hot_id, 0, NULL, 'Cappuccino', NULL, 30.00, NULL, '[Coffee,Hot]', TRUE, FALSE, 2, NULL, TRUE, NOW(), NOW(), 'System', 'System'),
(1, NULL, cat_beverages_hot_id, 0, NULL, 'Refill Cappuccino', NULL, 15.00, NULL, '[Coffee,Hot]', TRUE, FALSE, 3, NULL, TRUE, NOW(), NOW(), 'System', 'System'),
(1, NULL, cat_beverages_hot_id, 0, NULL, 'Red Cappuccino', NULL, 32.00, NULL, '[Coffee,Hot,Rooibos]', TRUE, FALSE, 4, NULL, TRUE, NOW(), NOW(), 'System', 'System'),
(1, NULL, cat_beverages_hot_id, 0, NULL, 'Chai Latte', NULL, 32.00, NULL, '[Tea,Hot]', TRUE, FALSE, 5, NULL, TRUE, NOW(), NOW(), 'System', 'System'),
(1, NULL, cat_beverages_hot_id, 0, NULL, 'Decaf Coffee', NULL, 32.00, NULL, '[Coffee,Hot,Decaf]', TRUE, FALSE, 6, NULL, TRUE, NOW(), NOW(), 'System', 'System'),
(1, NULL, cat_beverages_hot_id, 0, NULL, 'Café Latte', NULL, 32.00, NULL, '[Coffee,Hot]', TRUE, FALSE, 7, NULL, TRUE, NOW(), NOW(), 'System', 'System'),
(1, NULL, cat_beverages_hot_id, 0, NULL, 'Tea Black / Rooibos', NULL, 22.00, NULL, '[Tea,Hot]', TRUE, FALSE, 8, NULL, TRUE, NOW(), NOW(), 'System', 'System'),
(1, NULL, cat_beverages_hot_id, 0, NULL, 'Single Espresso', NULL, 24.00, NULL, '[Coffee,Hot,Espresso]', TRUE, FALSE, 9, NULL, TRUE, NOW(), NOW(), 'System', 'System'),
(1, NULL, cat_beverages_hot_id, 0, NULL, 'Double Espresso', NULL, 26.00, NULL, '[Coffee,Hot,Espresso]', TRUE, FALSE, 10, NULL, TRUE, NOW(), NOW(), 'System', 'System'),
(1, NULL, cat_beverages_hot_id, 0, NULL, 'Macchiato', NULL, 26.00, NULL, '[Coffee,Hot]', TRUE, FALSE, 11, NULL, TRUE, NOW(), NOW(), 'System', 'System'),
(1, NULL, cat_beverages_hot_id, 0, NULL, 'Hot Chocolate', NULL, 35.00, NULL, '[Hot,Chocolate]', TRUE, FALSE, 12, NULL, TRUE, NOW(), NOW(), 'System', 'System'),
(1, NULL, cat_beverages_hot_id, 0, NULL, 'Soya Milk', 'Add on', 8.00, NULL, '[Add-on]', TRUE, FALSE, 13, NULL, TRUE, NOW(), NOW(), 'System', 'System');

-- ================================================================
-- STEP 15: INSERT CATALOG ITEMS - COLD BEVERAGES
-- ================================================================

INSERT INTO CatalogItem (CatalogId, UserId, CategoryId, ItemType, BundleId, Name, Description, Price, Images, Tags, AvailableInEventMode, EventModeOnly, SortOrder, SourceLibraryItemId, IsActive, CreatedAt, UpdatedAt, CreatedBy, UpdatedBy)
VALUES
(1, NULL, cat_beverages_cold_id, 0, NULL, 'Iced Coffee', 'Coffee, ice, milk', 35.00, NULL, '[Coffee,Cold,Iced]', TRUE, FALSE, 1, NULL, TRUE, NOW(), NOW(), 'System', 'System'),
(1, NULL, cat_beverages_cold_id, 0, NULL, 'Fruit Juice (250ml)', NULL, 26.00, NULL, '[Juice,Cold]', TRUE, FALSE, 2, NULL, TRUE, NOW(), NOW(), 'System', 'System'),
(1, NULL, cat_beverages_cold_id, 0, NULL, 'Regular Milkshakes', 'Chocolate, vanilla, or strawberry', 39.00, NULL, '[Milkshake,Cold]', TRUE, FALSE, 3, NULL, TRUE, NOW(), NOW(), 'System', 'System'),
(1, NULL, cat_beverages_cold_id, 0, NULL, 'Coffee Milkshake', NULL, 49.00, NULL, '[Milkshake,Cold,Coffee]', TRUE, FALSE, 4, NULL, TRUE, NOW(), NOW(), 'System', 'System'),
(1, NULL, cat_beverages_cold_id, 0, NULL, '500ml Still/Sparkling Water', NULL, 24.00, NULL, '[Water,Cold]', TRUE, FALSE, 5, NULL, TRUE, NOW(), NOW(), 'System', 'System'),
(1, NULL, cat_beverages_cold_id, 0, NULL, '750ml Still/Sparkling Water', NULL, 47.00, NULL, '[Water,Cold]', TRUE, FALSE, 6, NULL, TRUE, NOW(), NOW(), 'System', 'System'),
(1, NULL, cat_beverages_cold_id, 0, NULL, 'Regular Soft Drinks', 'Coke, Sprite, Fanta, Cream Soda', 34.00, NULL, '[Soft Drink,Cold]', TRUE, FALSE, 7, NULL, TRUE, NOW(), NOW(), 'System', 'System'),
(1, NULL, cat_beverages_cold_id, 0, NULL, 'Lemon/Peach Ice Tea', NULL, 34.00, NULL, '[Tea,Cold,Iced]', TRUE, FALSE, 8, NULL, TRUE, NOW(), NOW(), 'System', 'System'),
(1, NULL, cat_beverages_cold_id, 0, NULL, 'Appletiser/Grapetiser', NULL, 48.00, NULL, '[Soft Drink,Cold]', TRUE, FALSE, 9, NULL, TRUE, NOW(), NOW(), 'System', 'System');

-- ================================================================
-- STEP 16: INSERT CATALOG ITEMS - SIDES
-- ================================================================

INSERT INTO CatalogItem (CatalogId, UserId, CategoryId, ItemType, BundleId, Name, Description, Price, Images, Tags, AvailableInEventMode, EventModeOnly, SortOrder, SourceLibraryItemId, IsActive, CreatedAt, UpdatedAt, CreatedBy, UpdatedBy)
VALUES
(1, NULL, cat_sides_id, 0, NULL, 'Vegetables of the Day', NULL, 35.00, NULL, '[Side,Vegetables]', TRUE, FALSE, 1, NULL, TRUE, NOW(), NOW(), 'System', 'System'),
(1, NULL, cat_sides_id, 0, NULL, 'Hand Cut Chips', NULL, 35.00, NULL, '[Side,Chips]', TRUE, FALSE, 2, NULL, TRUE, NOW(), NOW(), 'System', 'System'),
(1, NULL, cat_sides_id, 0, NULL, 'Side Greek Salad', NULL, 45.00, NULL, '[Side,Salad]', TRUE, FALSE, 3, NULL, TRUE, NOW(), NOW(), 'System', 'System'),
(1, NULL, cat_sides_id, 0, NULL, 'Side Garden Salad', NULL, 35.00, NULL, '[Side,Salad]', TRUE, FALSE, 4, NULL, TRUE, NOW(), NOW(), 'System', 'System'),
(1, NULL, cat_sides_id, 0, NULL, 'Tomato & Onion Salad', NULL, 35.00, NULL, '[Side,Salad]', TRUE, FALSE, 5, NULL, TRUE, NOW(), NOW(), 'System', 'System'),
(1, NULL, cat_sides_id, 0, NULL, 'Crispy Onion Rings', NULL, 35.00, NULL, '[Side,Fried]', TRUE, FALSE, 6, NULL, TRUE, NOW(), NOW(), 'System', 'System'),
(1, NULL, cat_sides_id, 0, NULL, 'Rice', NULL, 35.00, NULL, '[Side,Rice]', TRUE, FALSE, 7, NULL, TRUE, NOW(), NOW(), 'System', 'System'),
(1, NULL, cat_sides_id, 0, NULL, 'Mashed Potato', NULL, 35.00, NULL, '[Side,Potato]', TRUE, FALSE, 8, NULL, TRUE, NOW(), NOW(), 'System', 'System'),
(1, NULL, cat_sides_id, 0, NULL, 'Baked Potato', 'Served with Sour Cream', 35.00, NULL, '[Side,Potato]', TRUE, FALSE, 9, NULL, TRUE, NOW(), NOW(), 'System', 'System');

-- ================================================================
-- STEP 17: INSERT SAUCES (Additional items)
-- ================================================================

INSERT INTO CatalogItem (CatalogId, UserId, CategoryId, ItemType, BundleId, Name, Description, Price, Images, Tags, AvailableInEventMode, EventModeOnly, SortOrder, SourceLibraryItemId, IsActive, CreatedAt, UpdatedAt, CreatedBy, UpdatedBy)
VALUES
(1, NULL, cat_sides_id, 0, NULL, 'Mushroom Sauce', NULL, 32.00, NULL, '[Sauce,Add-on]', TRUE, FALSE, 10, NULL, TRUE, NOW(), NOW(), 'System', 'System'),
(1, NULL, cat_sides_id, 0, NULL, 'Pepper Sauce', NULL, 32.00, NULL, '[Sauce,Add-on]', TRUE, FALSE, 11, NULL, TRUE, NOW(), NOW(), 'System', 'System'),
(1, NULL, cat_sides_id, 0, NULL, 'Cheese Sauce', NULL, 32.00, NULL, '[Sauce,Add-on]', TRUE, FALSE, 12, NULL, TRUE, NOW(), NOW(), 'System', 'System'),
(1, NULL, cat_sides_id, 0, NULL, 'Gorgonzola Sauce', NULL, 60.00, NULL, '[Sauce,Add-on]', TRUE, FALSE, 13, NULL, TRUE, NOW(), NOW(), 'System', 'System'),
(1, NULL, cat_sides_id, 0, NULL, 'Béarnaise Sauce', NULL, 45.00, NULL, '[Sauce,Add-on]', TRUE, FALSE, 14, NULL, TRUE, NOW(), NOW(), 'System', 'System'),
(1, NULL, cat_sides_id, 0, NULL, 'Chilli Garlic Butter', NULL, 45.00, NULL, '[Sauce,Add-on]', TRUE, FALSE, 15, NULL, TRUE, NOW(), NOW(), 'System', 'System'),
(1, NULL, cat_sides_id, 0, NULL, 'Whole Grain Mustard Sauce', NULL, 45.00, NULL, '[Sauce,Add-on]', TRUE, FALSE, 16, NULL, TRUE, NOW(), NOW(), 'System', 'System');

-- ================================================================
-- STEP 18: INSERT BURGER ADD-ONS
-- ================================================================

INSERT INTO CatalogItem (CatalogId, UserId, CategoryId, ItemType, BundleId, Name, Description, Price, Images, Tags, AvailableInEventMode, EventModeOnly, SortOrder, SourceLibraryItemId, IsActive, CreatedAt, UpdatedAt, CreatedBy, UpdatedBy)
VALUES
(1, NULL, cat_burgers_id, 0, NULL, 'Fried Egg (Add-on)', 'Add to burger', 12.00, NULL, '[Add-on,Egg]', TRUE, FALSE, 10, NULL, TRUE, NOW(), NOW(), 'System', 'System'),
(1, NULL, cat_burgers_id, 0, NULL, 'Bacon/Cheddar/Caramelized Onion (Add-on)', 'Add to burger', 18.00, NULL, '[Add-on]', TRUE, FALSE, 11, NULL, TRUE, NOW(), NOW(), 'System', 'System'),
(1, NULL, cat_burgers_id, 0, NULL, 'Avocado (Add-on)', 'Add to burger', 30.00, NULL, '[Add-on,Avocado]', TRUE, FALSE, 12, NULL, TRUE, NOW(), NOW(), 'System', 'System'),
(1, NULL, cat_burgers_id, 0, NULL, '200g Beef Patty (Add-on)', 'Add to burger', 55.00, NULL, '[Add-on,Beef]', TRUE, FALSE, 13, NULL, TRUE, NOW(), NOW(), 'System', 'System'),
(1, NULL, cat_burgers_id, 0, NULL, 'Crumbed Chicken Breast (Add-on)', 'Add to burger', 45.00, NULL, '[Add-on,Chicken]', TRUE, FALSE, 14, NULL, TRUE, NOW(), NOW(), 'System', 'System');

RAISE NOTICE 'La Pineta 2025 Ala Carte Menu imported successfully!';
RAISE NOTICE 'Total categories created: 15';
RAISE NOTICE 'Total items created: 150+';

END $$;

COMMIT;

-- ================================================================
-- VERIFICATION QUERIES
-- ================================================================

-- View all categories
SELECT Id, Name, Description, SortOrder
FROM CatalogCategory
WHERE CatalogId = 1
ORDER BY SortOrder;

-- View items by category
SELECT
    cc.Name AS CategoryName,
    COUNT(ci.Id) AS ItemCount
FROM CatalogCategory cc
LEFT JOIN CatalogItem ci ON ci.CategoryId = cc.Id
WHERE cc.CatalogId = 1
GROUP BY cc.Name
ORDER BY cc.SortOrder;

-- View all items with their categories
SELECT
    cc.Name AS Category,
    ci.Name AS Item,
    ci.Price,
    ci.Description
FROM CatalogItem ci
JOIN CatalogCategory cc ON ci.CategoryId = cc.Id
WHERE ci.CatalogId = 1
ORDER BY cc.SortOrder, ci.SortOrder;
