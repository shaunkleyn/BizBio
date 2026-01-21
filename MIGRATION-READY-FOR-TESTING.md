# Migration 001: Multi-Product Architecture - Ready for Testing

## Status: ✓ Scripts Created - Awaiting Rollback Testing

**Date**: 2025-12-30
**Author**: System
**Risk Level**: HIGH (Major architectural change)

---

## What's Been Created

### 1. Migration Script (Forward)
**File**: `src/BizBio.Infrastructure/Migrations/Migration_001_MultiProductArchitecture.sql`
**Size**: 637 lines
**Purpose**: Migrates database from single-subscription to multi-product architecture

**Changes**:
- Creates 3 new tables: Entities, ProductSubscriptions, CatalogCategories
- Adds EntityId to Catalogs and Categories tables
- Adds ParentCatalogItemId and PriceOverride to CatalogItems
- Adds product limits to SubscriptionTiers
- Migrates existing Profiles → Entities
- Migrates existing Subscriptions → ProductSubscriptions
- Links all existing data to new structure

### 2. Rollback Script (Reverse)
**File**: `src/BizBio.Infrastructure/Migrations/Rollback_001_MultiProductArchitecture.sql`
**Size**: 334 lines
**Purpose**: Safely reverses all migration changes

**Changes**:
- Drops all new tables
- Removes all new columns
- Restores Categories.UserId column
- Validates rollback success

### 3. Test/Validation Script
**File**: `src/BizBio.Infrastructure/Migrations/Test_Rollback_001.sql`
**Size**: 383 lines
**Purpose**: Automated testing of rollback functionality

**Features**:
- Captures database state before migration
- Validates migration success
- Validates rollback success
- Compares pre/post states
- Detects data loss
- Comprehensive test report

---

## CRITICAL: Next Steps Before Running Migration

### ⚠ Step 1: Backup Your Database

**REQUIRED** before any testing:

```sql
-- Full backup
BACKUP DATABASE BizBio
TO DISK = 'C:\Backups\BizBio_PreMigration001_' + CONVERT(VARCHAR(8), GETDATE(), 112) + '.bak'
WITH FORMAT, COMPRESSION;
```

Verify backup succeeded:
```sql
RESTORE VERIFYONLY
FROM DISK = 'C:\Backups\BizBio_PreMigration001_YYYYMMDD.bak';
```

### ⚠ Step 2: Test Rollback on Development Database

**DO NOT SKIP THIS STEP!**

#### Option A: Automated Test (Recommended)
1. Connect to your development database (NOT production)
2. Execute: `Test_Rollback_001.sql`
3. When prompted, execute: `Migration_001_MultiProductArchitecture.sql`
4. When prompted, execute: `Rollback_001_MultiProductArchitecture.sql`
5. Review test report

#### Option B: Manual Test
```sql
-- 1. Capture current database state
SELECT
    TABLE_NAME,
    (SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES t WHERE t.TABLE_NAME = s.TABLE_NAME) as RecordCount
FROM INFORMATION_SCHEMA.TABLES s
WHERE TABLE_TYPE = 'BASE TABLE'
ORDER BY TABLE_NAME;

-- 2. Execute migration
-- Run: Migration_001_MultiProductArchitecture.sql

-- 3. Verify migration succeeded
SELECT 'Entities' as TableName, COUNT(*) as Records FROM Entities
UNION ALL
SELECT 'ProductSubscriptions', COUNT(*) FROM ProductSubscriptions
UNION ALL
SELECT 'CatalogCategories', COUNT(*) FROM CatalogCategories;

-- 4. Execute rollback
-- Run: Rollback_001_MultiProductArchitecture.sql

-- 5. Verify rollback succeeded
SELECT
    'Entities table removed' as Check,
    CASE WHEN NOT EXISTS(SELECT * FROM sys.tables WHERE name = 'Entities')
        THEN '✓ PASS' ELSE '✗ FAIL' END as Result
UNION ALL
SELECT
    'ProductSubscriptions table removed',
    CASE WHEN NOT EXISTS(SELECT * FROM sys.tables WHERE name = 'ProductSubscriptions')
        THEN '✓ PASS' ELSE '✗ FAIL' END
UNION ALL
SELECT
    'CatalogCategories table removed',
    CASE WHEN NOT EXISTS(SELECT * FROM sys.tables WHERE name = 'CatalogCategories')
        THEN '✓ PASS' ELSE '✗ FAIL' END
UNION ALL
SELECT
    'Categories.UserId restored',
    CASE WHEN EXISTS(SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Categories') AND name = 'UserId')
        THEN '✓ PASS' ELSE '✗ FAIL' END
UNION ALL
SELECT
    'Categories.EntityId removed',
    CASE WHEN NOT EXISTS(SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Categories') AND name = 'EntityId')
        THEN '✓ PASS' ELSE '✗ FAIL' END
UNION ALL
SELECT
    'Catalogs.EntityId removed',
    CASE WHEN NOT EXISTS(SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Catalogs') AND name = 'EntityId')
        THEN '✓ PASS' ELSE '✗ FAIL' END;
```

### Expected Rollback Test Results

**All must be PASS before proceeding**:
- ✓ Entities table removed
- ✓ ProductSubscriptions table removed
- ✓ CatalogCategories table removed
- ✓ Categories.UserId restored
- ✓ Categories.EntityId removed
- ✓ Catalogs.EntityId removed
- ✓ CatalogItems.ParentCatalogItemId removed
- ✓ CatalogItems.PriceOverride removed
- ✓ All SubscriptionTiers new columns removed
- ✓ No data loss (record counts match pre-migration)

---

## Migration Safety Features

### Transaction Wrapped
- Entire migration runs in single transaction
- Any error triggers automatic rollback
- Database remains in consistent state

### Validation at Each Phase
- Checks for existing tables/columns before creating
- Validates data migration success
- Reports orphaned records
- Final validation before commit

### Fallback Handling
- Creates default entities for users without profiles
- Links orphaned catalogs to fallback entities
- Preserves all existing data

### Detailed Logging
- PRINT statements at each step
- Record counts after each operation
- Error messages with context
- Success confirmation

---

## Post-Rollback Test: Next Steps

### If All Tests Pass ✓
1. Document rollback test results in `IMPLEMENTATION-LOG.md`
2. Review migration plan one final time
3. Schedule migration execution window
4. Run migration on development database
5. Test application functionality
6. Create C# entity classes
7. Update API controllers
8. Test API endpoints

### If Any Tests Fail ✗
1. **DO NOT RUN MIGRATION ON PRODUCTION**
2. Review failed test output
3. Identify root cause
4. Fix migration/rollback scripts
5. Re-test rollback until all tests pass

---

## Migration Execution Checklist

When ready to run migration (after rollback tested):

- [ ] Full database backup completed and verified
- [ ] Rollback script tested successfully
- [ ] All rollback tests passed
- [ ] Application stopped (no active connections)
- [ ] Maintenance window scheduled
- [ ] Team notified of migration
- [ ] Rollback procedure documented
- [ ] Execute migration script
- [ ] Verify migration success
- [ ] Test application startup
- [ ] Monitor for errors
- [ ] Document migration completion time

---

## Rollback Procedure (If Migration Fails)

1. **Immediately stop application**
2. **Execute rollback script**:
   ```sql
   -- Run: Rollback_001_MultiProductArchitecture.sql
   ```
3. **Verify rollback succeeded** using validation queries above
4. **Restart application** on old schema
5. **Document failure** in `IMPLEMENTATION-LOG.md`
6. **Review migration script** for errors
7. **Fix and re-test** before retry

---

## Contact & Support

**Implementation Log**: `IMPLEMENTATION-LOG.md`
**Implementation Plan**: `REVISED-IMPLEMENTATION-PLAN.md`
**Migration Scripts**: `src/BizBio.Infrastructure/Migrations/`

**Questions Before Proceeding**:
1. Do you have a recent database backup?
2. Is this a development/test database?
3. Are you comfortable executing SQL scripts?
4. Do you want me to guide you through rollback testing?

---

## Summary

✓ **Migration script created** - Comprehensive, transaction-safe
✓ **Rollback script created** - Fully reverses all changes
✓ **Test script created** - Validates rollback works
⚠ **Rollback testing required** - Do not skip this step!
⏳ **Awaiting user confirmation** - Ready to test when you are

**Recommendation**: Test rollback on development database copy first, then proceed with full migration only after successful rollback test.
