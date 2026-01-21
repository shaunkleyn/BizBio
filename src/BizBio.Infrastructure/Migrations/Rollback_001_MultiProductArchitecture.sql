-- ============================================================================
-- Rollback 001: Multi-Product Subscription Architecture
-- ============================================================================
-- Description: Rollback script for Migration_001_MultiProductArchitecture.sql
-- Author: System
-- Date: 2025-12-30
-- Version: 1.0.0
-- ============================================================================

-- IMPORTANT: This script will REVERSE the multi-product architecture changes
-- Only run this if you need to rollback to the previous single-subscription model

BEGIN TRANSACTION;

PRINT 'Starting Rollback 001: Multi-Product Subscription Architecture';
PRINT '==============================================================';

-- ============================================================================
-- PHASE 1: REMOVE FOREIGN KEY CONSTRAINTS
-- ============================================================================

PRINT 'Phase 1: Removing foreign key constraints...';

-- Remove FK from CatalogItems ParentCatalogItemId
IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_CatalogItems_ParentCatalogItem')
BEGIN
    ALTER TABLE CatalogItems DROP CONSTRAINT FK_CatalogItems_ParentCatalogItem;
    PRINT '  ✓ Dropped FK_CatalogItems_ParentCatalogItem';
END

-- Remove FK from Categories EntityId
IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Categories_Entities')
BEGIN
    ALTER TABLE Categories DROP CONSTRAINT FK_Categories_Entities;
    PRINT '  ✓ Dropped FK_Categories_Entities';
END

-- Remove FK from Catalogs EntityId
IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Catalogs_Entities')
BEGIN
    ALTER TABLE Catalogs DROP CONSTRAINT FK_Catalogs_Entities;
    PRINT '  ✓ Dropped FK_Catalogs_Entities';
END

-- ============================================================================
-- PHASE 2: RESTORE USERID TO CATEGORIES
-- ============================================================================

PRINT 'Phase 2: Restoring UserId to Categories...';

-- Add UserId column back to Categories
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Categories') AND name = 'UserId')
BEGIN
    ALTER TABLE Categories ADD UserId INT NULL;
    PRINT '  ✓ Added UserId column to Categories';

    -- Restore UserId from EntityId relationship
    UPDATE c
    SET c.UserId = e.UserId
    FROM Categories c
    INNER JOIN Entities e ON e.Id = c.EntityId
    WHERE c.UserId IS NULL;

    PRINT '  ✓ Restored UserId values: ' + CAST(@@ROWCOUNT AS NVARCHAR(10));

    -- Make UserId NOT NULL
    ALTER TABLE Categories ALTER COLUMN UserId INT NOT NULL;

    -- Re-add FK constraint
    ALTER TABLE Categories ADD CONSTRAINT FK_Categories_Users
        FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE CASCADE;

    PRINT '  ✓ Added FK_Categories_Users constraint';
END

-- ============================================================================
-- PHASE 3: REMOVE COLUMNS FROM EXISTING TABLES
-- ============================================================================

PRINT 'Phase 3: Removing columns from existing tables...';

-- ----------------------------------------------------------------------------
-- 3.1 Remove Columns from Categories
-- ----------------------------------------------------------------------------
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Categories') AND name = 'EntityId')
BEGIN
    -- Drop index first
    IF EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Categories_EntityId')
        DROP INDEX IX_Categories_EntityId ON Categories;

    ALTER TABLE Categories DROP COLUMN EntityId;
    PRINT '  ✓ Removed EntityId from Categories';
END

-- ----------------------------------------------------------------------------
-- 3.2 Remove Columns from CatalogItems
-- ----------------------------------------------------------------------------
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('CatalogItems') AND name = 'ParentCatalogItemId')
BEGIN
    -- Drop index first
    IF EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_CatalogItems_ParentCatalogItemId')
        DROP INDEX IX_CatalogItems_ParentCatalogItemId ON CatalogItems;

    ALTER TABLE CatalogItems DROP COLUMN ParentCatalogItemId;
    PRINT '  ✓ Removed ParentCatalogItemId from CatalogItems';
END

IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('CatalogItems') AND name = 'PriceOverride')
BEGIN
    ALTER TABLE CatalogItems DROP COLUMN PriceOverride;
    PRINT '  ✓ Removed PriceOverride from CatalogItems';
END

-- ----------------------------------------------------------------------------
-- 3.3 Remove Columns from Catalogs
-- ----------------------------------------------------------------------------
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Catalogs') AND name = 'EntityId')
BEGIN
    -- Drop index first
    IF EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Catalogs_EntityId')
        DROP INDEX IX_Catalogs_EntityId ON Catalogs;

    ALTER TABLE Catalogs DROP COLUMN EntityId;
    PRINT '  ✓ Removed EntityId from Catalogs';
END

-- ----------------------------------------------------------------------------
-- 3.4 Remove Columns from SubscriptionTiers
-- ----------------------------------------------------------------------------
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('SubscriptionTiers') AND name = 'ProductType')
BEGIN
    -- Drop index first
    IF EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_SubscriptionTiers_ProductType')
        DROP INDEX IX_SubscriptionTiers_ProductType ON SubscriptionTiers;

    ALTER TABLE SubscriptionTiers DROP COLUMN ProductType;
    PRINT '  ✓ Removed ProductType from SubscriptionTiers';
END

IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('SubscriptionTiers') AND name = 'MaxEntities')
BEGIN
    ALTER TABLE SubscriptionTiers DROP COLUMN MaxEntities;
    PRINT '  ✓ Removed MaxEntities from SubscriptionTiers';
END

IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('SubscriptionTiers') AND name = 'MaxCatalogsPerEntity')
BEGIN
    ALTER TABLE SubscriptionTiers DROP COLUMN MaxCatalogsPerEntity;
    PRINT '  ✓ Removed MaxCatalogsPerEntity from SubscriptionTiers';
END

IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('SubscriptionTiers') AND name = 'MaxLibraryItems')
BEGIN
    ALTER TABLE SubscriptionTiers DROP COLUMN MaxLibraryItems;
    PRINT '  ✓ Removed MaxLibraryItems from SubscriptionTiers';
END

IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('SubscriptionTiers') AND name = 'MaxCategoriesPerCatalog')
BEGIN
    ALTER TABLE SubscriptionTiers DROP COLUMN MaxCategoriesPerCatalog;
    PRINT '  ✓ Removed MaxCategoriesPerCatalog from SubscriptionTiers';
END

IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('SubscriptionTiers') AND name = 'MaxBundles')
BEGIN
    ALTER TABLE SubscriptionTiers DROP COLUMN MaxBundles;
    PRINT '  ✓ Removed MaxBundles from SubscriptionTiers';
END

-- ============================================================================
-- PHASE 4: DROP NEW TABLES
-- ============================================================================

PRINT 'Phase 4: Dropping new tables...';

-- Drop CatalogCategories table
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'CatalogCategories')
BEGIN
    DROP TABLE CatalogCategories;
    PRINT '  ✓ Dropped CatalogCategories table';
END

-- Drop ProductSubscriptions table
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'ProductSubscriptions')
BEGIN
    DROP TABLE ProductSubscriptions;
    PRINT '  ✓ Dropped ProductSubscriptions table';
END

-- Drop Entities table
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'Entities')
BEGIN
    DROP TABLE Entities;
    PRINT '  ✓ Dropped Entities table';
END

-- ============================================================================
-- PHASE 5: VALIDATION
-- ============================================================================

PRINT 'Phase 5: Validating rollback...';

-- Check that new tables are gone
DECLARE @RemainingTables NVARCHAR(500) = '';

IF EXISTS (SELECT * FROM sys.tables WHERE name = 'Entities')
    SET @RemainingTables = @RemainingTables + 'Entities, ';

IF EXISTS (SELECT * FROM sys.tables WHERE name = 'ProductSubscriptions')
    SET @RemainingTables = @RemainingTables + 'ProductSubscriptions, ';

IF EXISTS (SELECT * FROM sys.tables WHERE name = 'CatalogCategories')
    SET @RemainingTables = @RemainingTables + 'CatalogCategories, ';

IF LEN(@RemainingTables) > 0
BEGIN
    PRINT '  ⚠ WARNING: Some tables still exist: ' + @RemainingTables;
    ROLLBACK TRANSACTION;
    RAISERROR('Rollback validation failed - tables still exist', 16, 1);
    RETURN;
END
ELSE
BEGIN
    PRINT '  ✓ All new tables successfully removed';
END

-- Check that old columns are gone
DECLARE @RemainingColumns NVARCHAR(500) = '';

IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Categories') AND name = 'EntityId')
    SET @RemainingColumns = @RemainingColumns + 'Categories.EntityId, ';

IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Catalogs') AND name = 'EntityId')
    SET @RemainingColumns = @RemainingColumns + 'Catalogs.EntityId, ';

IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('CatalogItems') AND name = 'ParentCatalogItemId')
    SET @RemainingColumns = @RemainingColumns + 'CatalogItems.ParentCatalogItemId, ';

IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('CatalogItems') AND name = 'PriceOverride')
    SET @RemainingColumns = @RemainingColumns + 'CatalogItems.PriceOverride, ';

IF LEN(@RemainingColumns) > 0
BEGIN
    PRINT '  ⚠ WARNING: Some columns still exist: ' + @RemainingColumns;
    ROLLBACK TRANSACTION;
    RAISERROR('Rollback validation failed - columns still exist', 16, 1);
    RETURN;
END
ELSE
BEGIN
    PRINT '  ✓ All new columns successfully removed';
END

-- Check that UserId is restored to Categories
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Categories') AND name = 'UserId')
BEGIN
    PRINT '  ⚠ ERROR: UserId column not restored to Categories';
    ROLLBACK TRANSACTION;
    RAISERROR('Rollback validation failed - UserId not restored', 16, 1);
    RETURN;
END
ELSE
BEGIN
    PRINT '  ✓ UserId successfully restored to Categories';
END

COMMIT TRANSACTION;

PRINT '==============================================================';
PRINT 'Rollback 001 completed successfully!';
PRINT 'Database restored to pre-migration state.';
PRINT '==============================================================';
