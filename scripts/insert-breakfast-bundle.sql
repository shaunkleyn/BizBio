-- ================================================================
-- LA PINETA BREAKFAST SET MENU - R220 PER PERSON
-- ================================================================
-- Bundle Structure:
-- Fixed: Mini Pastries (2 per person)
-- Fixed: Yogurt, Fruit and Granola Cups
-- Choice: Main Course (4 options)
-- Choice: Hot Beverage (2 options)
-- Fixed: Fruit Juice (1 glass)
-- ================================================================

-- Assuming:
-- CatalogId = 1 (La Pineta's menu catalog)
-- This script uses identity insert starting from IDs 1000+ to avoid conflicts
-- Adjust IDs based on your existing data

SET IDENTITY_INSERT CatalogItem ON;

-- ================================================================
-- STEP 1: CREATE INDIVIDUAL CATALOG ITEMS (Products)
-- ================================================================

-- Fixed Items (Auto-included in bundle)
INSERT INTO CatalogItem (Id, CatalogId, UserId, CategoryId, ItemType, BundleId, Name, Description, Price, Images, Tags, AvailableInEventMode, EventModeOnly, SortOrder, SourceLibraryItemId, IsActive, CreatedAt, UpdatedAt, CreatedBy, UpdatedBy)
VALUES
(1001, 1, NULL, NULL, 0, NULL, 'Mini Pastries', '2 per person, served on platters to the table', 0.00, NULL, NULL, 1, 0, 0, NULL, 1, GETDATE(), GETDATE(), 'System', 'System'),
(1002, 1, NULL, NULL, 0, NULL, 'Yogurt, Fruit and Granola Cups', 'Served to each guest', 0.00, NULL, NULL, 1, 0, 0, NULL, 1, GETDATE(), GETDATE(), 'System', 'System'),
(1003, 1, NULL, NULL, 0, NULL, 'Fruit Juice', '1 Glass of Fruit Juice per person', 0.00, NULL, NULL, 1, 0, 0, NULL, 1, GETDATE(), GETDATE(), 'System', 'System');

-- Main Course Options (Step 3 - Choose Your Main)
INSERT INTO CatalogItem (Id, CatalogId, UserId, CategoryId, ItemType, BundleId, Name, Description, Price, Images, Tags, AvailableInEventMode, EventModeOnly, SortOrder, SourceLibraryItemId, IsActive, CreatedAt, UpdatedAt, CreatedBy, UpdatedBy)
VALUES
(1004, 1, NULL, NULL, 0, NULL, 'The Cereal Killer', 'Two slices of toasted sourdough, two poached eggs, bacon, tomatoes and mushrooms. (For halaal guests, bacon will be replaced with avocado)', 0.00, NULL, '["Eggs","Bacon","Sourdough"]', 1, 0, 1, NULL, 1, GETDATE(), GETDATE(), 'System', 'System'),
(1005, 1, NULL, NULL, 0, NULL, 'Double Guac My World', 'Toasted sourdough, smashed avocado, poached egg and rocket', 0.00, NULL, '["Vegetarian","Eggs","Avocado","Sourdough"]', 1, 0, 2, NULL, 1, GETDATE(), GETDATE(), 'System', 'System'),
(1006, 1, NULL, NULL, 0, NULL, 'Livin'' on the Veg', 'Toasted sourdough, basil pesto, caramelised onions, feta, sun dried tomatoes, and avocado topped with Asian salad', 0.00, NULL, '["Vegetarian","Avocado","Sourdough","Feta"]', 1, 0, 3, NULL, 1, GETDATE(), GETDATE(), 'System', 'System'),
(1007, 1, NULL, NULL, 0, NULL, 'Let''s Avocuddle', 'Toasted sourdough, hummus, blistered cherry tomatoes, mushrooms, avocado, rocket and balsamic glaze', 0.00, NULL, '["Vegan","Avocado","Sourdough"]', 1, 0, 4, NULL, 1, GETDATE(), GETDATE(), 'System', 'System');

-- Beverage Options (Step 4 - Choose Your Coffee)
INSERT INTO CatalogItem (Id, CatalogId, UserId, CategoryId, ItemType, BundleId, Name, Description, Price, Images, Tags, AvailableInEventMode, EventModeOnly, SortOrder, SourceLibraryItemId, IsActive, CreatedAt, UpdatedAt, CreatedBy, UpdatedBy)
VALUES
(1008, 1, NULL, NULL, 0, NULL, 'Cappuccino', '1 Cappuccino per person', 0.00, NULL, '["Coffee","Hot Beverage"]', 1, 0, 1, NULL, 1, GETDATE(), GETDATE(), 'System', 'System'),
(1009, 1, NULL, NULL, 0, NULL, 'Americano', '1 Americano per person', 0.00, NULL, '["Coffee","Hot Beverage"]', 1, 0, 2, NULL, 1, GETDATE(), GETDATE(), 'System', 'System');

SET IDENTITY_INSERT CatalogItem OFF;

-- ================================================================
-- STEP 2: CREATE THE BUNDLE
-- ================================================================

SET IDENTITY_INSERT CatalogBundle ON;

INSERT INTO CatalogBundle (Id, CatalogId, Name, Slug, Description, BasePrice, Images, SortOrder, IsActive, CreatedAt, UpdatedAt, CreatedBy, UpdatedBy)
VALUES (
    2001,
    1,
    'Breakfast Set Menu',
    'breakfast-set-menu',
    'R220 per person. Includes mini pastries, yogurt cups, your choice of main course, hot beverage, and fruit juice. A customary service charge of 12.5% is added to tables of 8 or more.',
    220.00,
    NULL,
    1,
    1,
    GETDATE(),
    GETDATE(),
    'System',
    'System'
);

SET IDENTITY_INSERT CatalogBundle OFF;

-- ================================================================
-- STEP 3: CREATE BUNDLE STEPS (Selection Stages)
-- ================================================================

SET IDENTITY_INSERT CatalogBundleStep ON;

INSERT INTO CatalogBundleStep (Id, BundleId, StepNumber, Name, MinSelect, MaxSelect, IsActive, CreatedAt, UpdatedAt, CreatedBy, UpdatedBy)
VALUES
(3001, 2001, 1, 'Mini Pastries', 1, 1, 1, GETDATE(), GETDATE(), 'System', 'System'),
(3002, 2001, 2, 'Yogurt, Fruit & Granola', 1, 1, 1, GETDATE(), GETDATE(), 'System', 'System'),
(3003, 2001, 3, 'Choose Your Main Course', 1, 1, 1, GETDATE(), GETDATE(), 'System', 'System'),
(3004, 2001, 4, 'Choose Your Coffee', 1, 1, 1, GETDATE(), GETDATE(), 'System', 'System'),
(3005, 2001, 5, 'Fruit Juice', 1, 1, 1, GETDATE(), GETDATE(), 'System', 'System');

SET IDENTITY_INSERT CatalogBundleStep OFF;

-- ================================================================
-- STEP 4: LINK PRODUCTS TO BUNDLE STEPS
-- ================================================================

SET IDENTITY_INSERT CatalogBundleStepProduct ON;

-- Step 1: Mini Pastries (Fixed - only one option)
INSERT INTO CatalogBundleStepProduct (Id, StepId, ProductId, IsActive, CreatedAt, UpdatedAt, CreatedBy, UpdatedBy)
VALUES (4001, 3001, 1001, 1, GETDATE(), GETDATE(), 'System', 'System');

-- Step 2: Yogurt Cup (Fixed - only one option)
INSERT INTO CatalogBundleStepProduct (Id, StepId, ProductId, IsActive, CreatedAt, UpdatedAt, CreatedBy, UpdatedBy)
VALUES (4002, 3002, 1002, 1, GETDATE(), GETDATE(), 'System', 'System');

-- Step 3: Main Course (4 options to choose from)
INSERT INTO CatalogBundleStepProduct (Id, StepId, ProductId, IsActive, CreatedAt, UpdatedAt, CreatedBy, UpdatedBy)
VALUES
(4003, 3003, 1004, 1, GETDATE(), GETDATE(), 'System', 'System'), -- The Cereal Killer
(4004, 3003, 1005, 1, GETDATE(), GETDATE(), 'System', 'System'), -- Double Guac My World
(4005, 3003, 1006, 1, GETDATE(), GETDATE(), 'System', 'System'), -- Livin' on the Veg
(4006, 3003, 1007, 1, GETDATE(), GETDATE(), 'System', 'System'); -- Let's Avocuddle

-- Step 4: Coffee (2 options to choose from)
INSERT INTO CatalogBundleStepProduct (Id, StepId, ProductId, IsActive, CreatedAt, UpdatedAt, CreatedBy, UpdatedBy)
VALUES
(4007, 3004, 1008, 1, GETDATE(), GETDATE(), 'System', 'System'), -- Cappuccino
(4008, 3004, 1009, 1, GETDATE(), GETDATE(), 'System', 'System'); -- Americano

-- Step 5: Fruit Juice (Fixed - only one option)
INSERT INTO CatalogBundleStepProduct (Id, StepId, ProductId, IsActive, CreatedAt, UpdatedAt, CreatedBy, UpdatedBy)
VALUES (4009, 3005, 1003, 1, GETDATE(), GETDATE(), 'System', 'System');

SET IDENTITY_INSERT CatalogBundleStepProduct OFF;

-- ================================================================
-- STEP 5: CREATE OPTION GROUP FOR HALAAL PREFERENCE
-- (For "The Cereal Killer" - bacon replacement option)
-- ================================================================

SET IDENTITY_INSERT CatalogBundleOptionGroup ON;

INSERT INTO CatalogBundleOptionGroup (Id, StepId, Name, IsRequired, MinSelect, MaxSelect, IsActive, CreatedAt, UpdatedAt, CreatedBy, UpdatedBy)
VALUES (
    5001,
    3003,
    'Dietary Preference (The Cereal Killer only)',
    0, -- Not required - defaults to regular version
    0,
    1,
    1,
    GETDATE(),
    GETDATE(),
    'System',
    'System'
);

SET IDENTITY_INSERT CatalogBundleOptionGroup OFF;

-- ================================================================
-- STEP 6: CREATE OPTIONS FOR HALAAL PREFERENCE
-- ================================================================

SET IDENTITY_INSERT CatalogBundleOption ON;

INSERT INTO CatalogBundleOption (Id, OptionGroupId, Name, PriceModifier, IsDefault, IsActive, CreatedAt, UpdatedAt, CreatedBy, UpdatedBy)
VALUES
(6001, 5001, 'Regular (with bacon)', 0.00, 1, 1, GETDATE(), GETDATE(), 'System', 'System'),
(6002, 5001, 'Halaal (bacon replaced with avocado)', 0.00, 0, 1, GETDATE(), GETDATE(), 'System', 'System');

SET IDENTITY_INSERT CatalogBundleOption OFF;

-- ================================================================
-- OPTIONAL: CREATE A CATALOG VERSION FOR BREAKFAST HOURS
-- (Uncomment if you want this bundle only available during breakfast hours)
-- ================================================================

/*
SET IDENTITY_INSERT CatalogVersion ON;

INSERT INTO CatalogVersion (Id, CatalogId, Name, Description, ValidFrom, ValidTo, Priority, TimeOfDayConstraint, DaysOfWeek, IsActive, CreatedAt, UpdatedAt, CreatedBy, UpdatedBy)
VALUES (
    7001,
    1,
    'Breakfast Service',
    'Breakfast menu available during morning hours',
    '2024-01-01 00:00:00',
    '2025-12-31 23:59:59',
    5,
    '06:00-11:00', -- Only available 6 AM to 11 AM
    '1,2,3,4,5,6,7', -- All days of the week (Mon-Sun)
    1,
    GETDATE(),
    GETDATE(),
    'System',
    'System'
);

SET IDENTITY_INSERT CatalogVersion OFF;
*/

-- ================================================================
-- VERIFICATION QUERIES
-- ================================================================

-- View the complete bundle structure
SELECT
    'Bundle' AS EntityType,
    cb.Id,
    cb.Name,
    cb.BasePrice,
    cb.Description
FROM CatalogBundle cb
WHERE cb.Id = 2001;

-- View all bundle steps
SELECT
    'Steps' AS EntityType,
    cbs.StepNumber,
    cbs.Name,
    cbs.MinSelect,
    cbs.MaxSelect
FROM CatalogBundleStep cbs
WHERE cbs.BundleId = 2001
ORDER BY cbs.StepNumber;

-- View products for each step
SELECT
    'Step Products' AS EntityType,
    cbs.StepNumber,
    cbs.Name AS StepName,
    ci.Name AS ProductName,
    ci.Description AS ProductDescription
FROM CatalogBundleStep cbs
JOIN CatalogBundleStepProduct cbsp ON cbs.Id = cbsp.StepId
JOIN CatalogItem ci ON cbsp.ProductId = ci.Id
WHERE cbs.BundleId = 2001
ORDER BY cbs.StepNumber, ci.SortOrder;

-- View option groups and options
SELECT
    'Options' AS EntityType,
    cbs.Name AS StepName,
    cbog.Name AS OptionGroupName,
    cbo.Name AS OptionName,
    cbo.PriceModifier,
    cbo.IsDefault
FROM CatalogBundleStep cbs
JOIN CatalogBundleOptionGroup cbog ON cbs.Id = cbog.StepId
JOIN CatalogBundleOption cbo ON cbog.Id = cbo.OptionGroupId
WHERE cbs.BundleId = 2001
ORDER BY cbs.StepNumber, cbog.Id, cbo.Id;

PRINT 'Breakfast Set Menu bundle created successfully!';
PRINT 'Bundle ID: 2001';
PRINT 'Base Price: R220 per person';
PRINT 'Total Items Created:';
PRINT '  - 9 Catalog Items';
PRINT '  - 1 Bundle';
PRINT '  - 5 Bundle Steps';
PRINT '  - 9 Step-Product Links';
PRINT '  - 1 Option Group (Halaal preference)';
PRINT '  - 2 Options';
