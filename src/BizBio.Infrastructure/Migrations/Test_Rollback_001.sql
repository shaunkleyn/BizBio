-- ============================================================================
-- Test Rollback 001: Multi-Product Subscription Architecture
-- ============================================================================
-- Description: Test script to validate rollback functionality
-- Author: System
-- Date: 2025-12-30
-- Version: 1.0.0
-- ============================================================================

-- IMPORTANT: Run this on a TEST DATABASE COPY before production migration!

-- This script performs the following:
-- 1. Creates a backup of current database state
-- 2. Runs the migration forward
-- 3. Validates migration success
-- 4. Runs the rollback script
-- 5. Validates rollback success
-- 6. Compares pre/post states

PRINT '====================================================================';
PRINT 'Rollback Test Suite for Migration 001';
PRINT 'Started: ' + CONVERT(NVARCHAR(50), GETUTCDATE(), 120);
PRINT '====================================================================';
PRINT '';

-- ============================================================================
-- PHASE 1: PRE-MIGRATION STATE CAPTURE
-- ============================================================================

PRINT 'Phase 1: Capturing pre-migration state...';

-- Create temp tables to store pre-migration counts
CREATE TABLE #PreMigrationState (
    TableName NVARCHAR(100),
    ColumnName NVARCHAR(100),
    RecordCount INT,
    StateType NVARCHAR(50)
);

-- Capture table existence
INSERT INTO #PreMigrationState (TableName, ColumnName, RecordCount, StateType)
VALUES
    ('Entities', NULL, CASE WHEN EXISTS(SELECT * FROM sys.tables WHERE name = 'Entities') THEN 1 ELSE 0 END, 'TableExists'),
    ('ProductSubscriptions', NULL, CASE WHEN EXISTS(SELECT * FROM sys.tables WHERE name = 'ProductSubscriptions') THEN 1 ELSE 0 END, 'TableExists'),
    ('CatalogCategories', NULL, CASE WHEN EXISTS(SELECT * FROM sys.tables WHERE name = 'CatalogCategories') THEN 1 ELSE 0 END, 'TableExists');

-- Capture column existence
INSERT INTO #PreMigrationState (TableName, ColumnName, RecordCount, StateType)
VALUES
    ('Categories', 'UserId', CASE WHEN EXISTS(SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Categories') AND name = 'UserId') THEN 1 ELSE 0 END, 'ColumnExists'),
    ('Categories', 'EntityId', CASE WHEN EXISTS(SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Categories') AND name = 'EntityId') THEN 1 ELSE 0 END, 'ColumnExists'),
    ('Catalogs', 'EntityId', CASE WHEN EXISTS(SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Catalogs') AND name = 'EntityId') THEN 1 ELSE 0 END, 'ColumnExists'),
    ('CatalogItems', 'ParentCatalogItemId', CASE WHEN EXISTS(SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('CatalogItems') AND name = 'ParentCatalogItemId') THEN 1 ELSE 0 END, 'ColumnExists'),
    ('CatalogItems', 'PriceOverride', CASE WHEN EXISTS(SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('CatalogItems') AND name = 'PriceOverride') THEN 1 ELSE 0 END, 'ColumnExists'),
    ('SubscriptionTiers', 'ProductType', CASE WHEN EXISTS(SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('SubscriptionTiers') AND name = 'ProductType') THEN 1 ELSE 0 END, 'ColumnExists'),
    ('SubscriptionTiers', 'MaxEntities', CASE WHEN EXISTS(SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('SubscriptionTiers') AND name = 'MaxEntities') THEN 1 ELSE 0 END, 'ColumnExists');

-- Capture record counts
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'Users')
    INSERT INTO #PreMigrationState VALUES ('Users', NULL, (SELECT COUNT(*) FROM Users), 'RecordCount');

IF EXISTS (SELECT * FROM sys.tables WHERE name = 'Categories')
    INSERT INTO #PreMigrationState VALUES ('Categories', NULL, (SELECT COUNT(*) FROM Categories), 'RecordCount');

IF EXISTS (SELECT * FROM sys.tables WHERE name = 'Catalogs')
    INSERT INTO #PreMigrationState VALUES ('Catalogs', NULL, (SELECT COUNT(*) FROM Catalogs), 'RecordCount');

IF EXISTS (SELECT * FROM sys.tables WHERE name = 'CatalogItems')
    INSERT INTO #PreMigrationState VALUES ('CatalogItems', NULL, (SELECT COUNT(*) FROM CatalogItems), 'RecordCount');

IF EXISTS (SELECT * FROM sys.tables WHERE name = 'Subscriptions')
    INSERT INTO #PreMigrationState VALUES ('Subscriptions', NULL, (SELECT COUNT(*) FROM Subscriptions), 'RecordCount');

PRINT '✓ Pre-migration state captured';
PRINT '';

-- Display pre-migration state
PRINT 'Pre-Migration State:';
SELECT
    TableName,
    ColumnName,
    RecordCount,
    StateType,
    CASE
        WHEN StateType IN ('TableExists', 'ColumnExists') AND RecordCount = 1 THEN 'EXISTS'
        WHEN StateType IN ('TableExists', 'ColumnExists') AND RecordCount = 0 THEN 'NOT EXISTS'
        ELSE CAST(RecordCount AS NVARCHAR(10)) + ' records'
    END as Status
FROM #PreMigrationState
ORDER BY StateType, TableName, ColumnName;

PRINT '';

-- ============================================================================
-- PHASE 2: RUN MIGRATION FORWARD
-- ============================================================================

PRINT 'Phase 2: Running migration forward...';
PRINT '(Migration script content would run here)';
PRINT '⚠ For actual testing, execute Migration_001_MultiProductArchitecture.sql at this point';
PRINT '';

-- ============================================================================
-- PHASE 3: VALIDATE MIGRATION SUCCESS
-- ============================================================================

PRINT 'Phase 3: Validating migration success...';

DECLARE @MigrationValidationErrors INT = 0;

-- Check new tables exist
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Entities')
BEGIN
    PRINT '  ✗ ERROR: Entities table does not exist';
    SET @MigrationValidationErrors = @MigrationValidationErrors + 1;
END
ELSE
    PRINT '  ✓ Entities table exists';

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'ProductSubscriptions')
BEGIN
    PRINT '  ✗ ERROR: ProductSubscriptions table does not exist';
    SET @MigrationValidationErrors = @MigrationValidationErrors + 1;
END
ELSE
    PRINT '  ✓ ProductSubscriptions table exists';

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'CatalogCategories')
BEGIN
    PRINT '  ✗ ERROR: CatalogCategories table does not exist';
    SET @MigrationValidationErrors = @MigrationValidationErrors + 1;
END
ELSE
    PRINT '  ✓ CatalogCategories table exists';

-- Check new columns exist
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Categories') AND name = 'EntityId')
BEGIN
    PRINT '  ✗ ERROR: Categories.EntityId column does not exist';
    SET @MigrationValidationErrors = @MigrationValidationErrors + 1;
END
ELSE
    PRINT '  ✓ Categories.EntityId column exists';

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Catalogs') AND name = 'EntityId')
BEGIN
    PRINT '  ✗ ERROR: Catalogs.EntityId column does not exist';
    SET @MigrationValidationErrors = @MigrationValidationErrors + 1;
END
ELSE
    PRINT '  ✓ Catalogs.EntityId column exists';

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('CatalogItems') AND name = 'ParentCatalogItemId')
BEGIN
    PRINT '  ✗ ERROR: CatalogItems.ParentCatalogItemId column does not exist';
    SET @MigrationValidationErrors = @MigrationValidationErrors + 1;
END
ELSE
    PRINT '  ✓ CatalogItems.ParentCatalogItemId column exists';

-- Check old columns removed
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Categories') AND name = 'UserId')
BEGIN
    PRINT '  ⚠ WARNING: Categories.UserId column still exists (should be removed)';
    -- This is expected during migration, not an error
END
ELSE
    PRINT '  ✓ Categories.UserId column removed as expected';

IF @MigrationValidationErrors > 0
BEGIN
    PRINT '';
    PRINT '  ✗ Migration validation FAILED with ' + CAST(@MigrationValidationErrors AS NVARCHAR(10)) + ' errors';
    PRINT '  ⚠ Do not proceed with rollback test - fix migration first';
    PRINT '';
    DROP TABLE #PreMigrationState;
    RETURN;
END

PRINT '';
PRINT '✓ Migration validation PASSED';
PRINT '';

-- ============================================================================
-- PHASE 4: RUN ROLLBACK
-- ============================================================================

PRINT 'Phase 4: Running rollback script...';
PRINT '(Rollback script content would run here)';
PRINT '⚠ For actual testing, execute Rollback_001_MultiProductArchitecture.sql at this point';
PRINT '';

-- ============================================================================
-- PHASE 5: VALIDATE ROLLBACK SUCCESS
-- ============================================================================

PRINT 'Phase 5: Validating rollback success...';

DECLARE @RollbackValidationErrors INT = 0;

-- Check new tables are gone
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'Entities')
BEGIN
    PRINT '  ✗ ERROR: Entities table still exists';
    SET @RollbackValidationErrors = @RollbackValidationErrors + 1;
END
ELSE
    PRINT '  ✓ Entities table removed';

IF EXISTS (SELECT * FROM sys.tables WHERE name = 'ProductSubscriptions')
BEGIN
    PRINT '  ✗ ERROR: ProductSubscriptions table still exists';
    SET @RollbackValidationErrors = @RollbackValidationErrors + 1;
END
ELSE
    PRINT '  ✓ ProductSubscriptions table removed';

IF EXISTS (SELECT * FROM sys.tables WHERE name = 'CatalogCategories')
BEGIN
    PRINT '  ✗ ERROR: CatalogCategories table still exists';
    SET @RollbackValidationErrors = @RollbackValidationErrors + 1;
END
ELSE
    PRINT '  ✓ CatalogCategories table removed';

-- Check new columns are gone
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Categories') AND name = 'EntityId')
BEGIN
    PRINT '  ✗ ERROR: Categories.EntityId column still exists';
    SET @RollbackValidationErrors = @RollbackValidationErrors + 1;
END
ELSE
    PRINT '  ✓ Categories.EntityId column removed';

IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Catalogs') AND name = 'EntityId')
BEGIN
    PRINT '  ✗ ERROR: Catalogs.EntityId column still exists';
    SET @RollbackValidationErrors = @RollbackValidationErrors + 1;
END
ELSE
    PRINT '  ✓ Catalogs.EntityId column removed';

IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('CatalogItems') AND name = 'ParentCatalogItemId')
BEGIN
    PRINT '  ✗ ERROR: CatalogItems.ParentCatalogItemId column still exists';
    SET @RollbackValidationErrors = @RollbackValidationErrors + 1;
END
ELSE
    PRINT '  ✓ CatalogItems.ParentCatalogItemId column removed';

IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('CatalogItems') AND name = 'PriceOverride')
BEGIN
    PRINT '  ✗ ERROR: CatalogItems.PriceOverride column still exists';
    SET @RollbackValidationErrors = @RollbackValidationErrors + 1;
END
ELSE
    PRINT '  ✓ CatalogItems.PriceOverride column removed';

-- Check old columns restored
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Categories') AND name = 'UserId')
BEGIN
    PRINT '  ✗ ERROR: Categories.UserId column not restored';
    SET @RollbackValidationErrors = @RollbackValidationErrors + 1;
END
ELSE
    PRINT '  ✓ Categories.UserId column restored';

IF @RollbackValidationErrors > 0
BEGIN
    PRINT '';
    PRINT '  ✗ Rollback validation FAILED with ' + CAST(@RollbackValidationErrors AS NVARCHAR(10)) + ' errors';
END
ELSE
BEGIN
    PRINT '';
    PRINT '✓ Rollback validation PASSED';
END

PRINT '';

-- ============================================================================
-- PHASE 6: COMPARE PRE/POST STATE
-- ============================================================================

PRINT 'Phase 6: Comparing pre-migration vs post-rollback state...';

-- Create post-rollback state table
CREATE TABLE #PostRollbackState (
    TableName NVARCHAR(100),
    ColumnName NVARCHAR(100),
    RecordCount INT,
    StateType NVARCHAR(50)
);

-- Capture table existence
INSERT INTO #PostRollbackState (TableName, ColumnName, RecordCount, StateType)
VALUES
    ('Entities', NULL, CASE WHEN EXISTS(SELECT * FROM sys.tables WHERE name = 'Entities') THEN 1 ELSE 0 END, 'TableExists'),
    ('ProductSubscriptions', NULL, CASE WHEN EXISTS(SELECT * FROM sys.tables WHERE name = 'ProductSubscriptions') THEN 1 ELSE 0 END, 'TableExists'),
    ('CatalogCategories', NULL, CASE WHEN EXISTS(SELECT * FROM sys.tables WHERE name = 'CatalogCategories') THEN 1 ELSE 0 END, 'TableExists');

-- Capture column existence
INSERT INTO #PostRollbackState (TableName, ColumnName, RecordCount, StateType)
VALUES
    ('Categories', 'UserId', CASE WHEN EXISTS(SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Categories') AND name = 'UserId') THEN 1 ELSE 0 END, 'ColumnExists'),
    ('Categories', 'EntityId', CASE WHEN EXISTS(SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Categories') AND name = 'EntityId') THEN 1 ELSE 0 END, 'ColumnExists'),
    ('Catalogs', 'EntityId', CASE WHEN EXISTS(SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Catalogs') AND name = 'EntityId') THEN 1 ELSE 0 END, 'ColumnExists'),
    ('CatalogItems', 'ParentCatalogItemId', CASE WHEN EXISTS(SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('CatalogItems') AND name = 'ParentCatalogItemId') THEN 1 ELSE 0 END, 'ColumnExists'),
    ('CatalogItems', 'PriceOverride', CASE WHEN EXISTS(SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('CatalogItems') AND name = 'PriceOverride') THEN 1 ELSE 0 END, 'ColumnExists'),
    ('SubscriptionTiers', 'ProductType', CASE WHEN EXISTS(SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('SubscriptionTiers') AND name = 'ProductType') THEN 1 ELSE 0 END, 'ColumnExists'),
    ('SubscriptionTiers', 'MaxEntities', CASE WHEN EXISTS(SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('SubscriptionTiers') AND name = 'MaxEntities') THEN 1 ELSE 0 END, 'ColumnExists');

-- Capture record counts
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'Users')
    INSERT INTO #PostRollbackState VALUES ('Users', NULL, (SELECT COUNT(*) FROM Users), 'RecordCount');

IF EXISTS (SELECT * FROM sys.tables WHERE name = 'Categories')
    INSERT INTO #PostRollbackState VALUES ('Categories', NULL, (SELECT COUNT(*) FROM Categories), 'RecordCount');

IF EXISTS (SELECT * FROM sys.tables WHERE name = 'Catalogs')
    INSERT INTO #PostRollbackState VALUES ('Catalogs', NULL, (SELECT COUNT(*) FROM Catalogs), 'RecordCount');

IF EXISTS (SELECT * FROM sys.tables WHERE name = 'CatalogItems')
    INSERT INTO #PostRollbackState VALUES ('CatalogItems', NULL, (SELECT COUNT(*) FROM CatalogItems), 'RecordCount');

-- Compare states
SELECT
    COALESCE(pre.TableName, post.TableName) as TableName,
    COALESCE(pre.ColumnName, post.ColumnName) as ColumnName,
    COALESCE(pre.StateType, post.StateType) as StateType,
    pre.RecordCount as PreMigration,
    post.RecordCount as PostRollback,
    CASE
        WHEN pre.RecordCount = post.RecordCount THEN '✓ MATCH'
        WHEN pre.RecordCount IS NULL THEN '⚠ ADDED DURING MIGRATION'
        WHEN post.RecordCount IS NULL THEN '⚠ REMOVED DURING MIGRATION'
        ELSE '✗ MISMATCH'
    END as Comparison
FROM #PreMigrationState pre
FULL OUTER JOIN #PostRollbackState post
    ON pre.TableName = post.TableName
    AND COALESCE(pre.ColumnName, '') = COALESCE(post.ColumnName, '')
    AND pre.StateType = post.StateType
ORDER BY StateType, TableName, ColumnName;

-- Check for record count differences
DECLARE @RecordCountDifferences INT;
SELECT @RecordCountDifferences = COUNT(*)
FROM #PreMigrationState pre
FULL OUTER JOIN #PostRollbackState post
    ON pre.TableName = post.TableName AND pre.StateType = post.StateType
WHERE pre.StateType = 'RecordCount'
AND pre.RecordCount != post.RecordCount;

IF @RecordCountDifferences > 0
BEGIN
    PRINT '';
    PRINT '⚠ WARNING: ' + CAST(@RecordCountDifferences AS NVARCHAR(10)) + ' table(s) have different record counts!';
    PRINT 'This may indicate data loss during migration/rollback.';
END
ELSE
BEGIN
    PRINT '';
    PRINT '✓ All record counts match - no data loss detected';
END

-- Cleanup
DROP TABLE #PreMigrationState;
DROP TABLE #PostRollbackState;

-- ============================================================================
-- SUMMARY
-- ============================================================================

PRINT '';
PRINT '====================================================================';
PRINT 'Rollback Test Suite Summary';
PRINT '====================================================================';

IF @MigrationValidationErrors = 0 AND @RollbackValidationErrors = 0 AND @RecordCountDifferences = 0
BEGIN
    PRINT '✓ ALL TESTS PASSED';
    PRINT '';
    PRINT 'The rollback script successfully reverses the migration.';
    PRINT 'It is safe to proceed with production migration.';
END
ELSE
BEGIN
    PRINT '✗ TESTS FAILED';
    PRINT '';
    PRINT 'Issues detected:';
    IF @MigrationValidationErrors > 0
        PRINT '  - Migration validation errors: ' + CAST(@MigrationValidationErrors AS NVARCHAR(10));
    IF @RollbackValidationErrors > 0
        PRINT '  - Rollback validation errors: ' + CAST(@RollbackValidationErrors AS NVARCHAR(10));
    IF @RecordCountDifferences > 0
        PRINT '  - Record count differences: ' + CAST(@RecordCountDifferences AS NVARCHAR(10));
    PRINT '';
    PRINT '⚠ DO NOT proceed with production migration until issues are resolved.';
END

PRINT '';
PRINT 'Completed: ' + CONVERT(NVARCHAR(50), GETUTCDATE(), 120);
PRINT '====================================================================';
