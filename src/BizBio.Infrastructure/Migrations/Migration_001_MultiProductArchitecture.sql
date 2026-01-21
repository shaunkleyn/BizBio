-- ============================================================================
-- Migration 001: Multi-Product Subscription Architecture
-- ============================================================================
-- Description: Transforms BizBio from single-subscription to multi-product
--              subscription model with unified entity management
-- Author: System
-- Date: 2025-12-30
-- Version: 1.0.0
-- ============================================================================

-- IMPORTANT: Create full database backup before running this migration!
-- BACKUP DATABASE BizBio TO DISK = 'C:\Backups\BizBio_PreMigration001.bak';

BEGIN TRANSACTION;

PRINT 'Starting Migration 001: Multi-Product Subscription Architecture';
PRINT '================================================================';

-- ============================================================================
-- PHASE 1: CREATE NEW TABLES
-- ============================================================================

PRINT 'Phase 1: Creating new tables...';

-- ----------------------------------------------------------------------------
-- 1.1 Create Entities Table
-- ----------------------------------------------------------------------------
PRINT '  Creating Entities table...';

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Entities')
BEGIN
    CREATE TABLE Entities (
        Id INT PRIMARY KEY IDENTITY(1,1),
        UserId INT NOT NULL,
        EntityType INT NOT NULL DEFAULT 0,  -- 0=Restaurant, 1=Store, 2=Venue, 3=Organization
        Name NVARCHAR(255) NOT NULL,
        Slug NVARCHAR(255) NOT NULL,
        Description NVARCHAR(2000) NULL,
        Logo NVARCHAR(500) NULL,

        -- Contact Information
        Address NVARCHAR(255) NULL,
        City NVARCHAR(100) NULL,
        PostalCode NVARCHAR(20) NULL,
        Phone NVARCHAR(20) NULL,
        Email NVARCHAR(255) NULL,
        Website NVARCHAR(500) NULL,

        -- Settings
        Currency NVARCHAR(3) NOT NULL DEFAULT 'ZAR',
        Timezone NVARCHAR(50) NOT NULL DEFAULT 'Africa/Johannesburg',

        -- Metadata
        SortOrder INT NOT NULL DEFAULT 0,
        IsActive BIT NOT NULL DEFAULT 1,
        CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        UpdatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        CreatedBy NVARCHAR(50) NULL,
        UpdatedBy NVARCHAR(50) NULL,

        CONSTRAINT FK_Entities_Users FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE CASCADE
    );

    CREATE INDEX IX_Entities_UserId ON Entities(UserId);
    CREATE UNIQUE INDEX IX_Entities_Slug ON Entities(Slug);
    CREATE INDEX IX_Entities_EntityType ON Entities(EntityType);
    CREATE INDEX IX_Entities_IsActive ON Entities(IsActive);

    PRINT '  ✓ Entities table created successfully';
END
ELSE
BEGIN
    PRINT '  ⚠ Entities table already exists, skipping creation';
END

-- ----------------------------------------------------------------------------
-- 1.2 Create ProductSubscriptions Table
-- ----------------------------------------------------------------------------
PRINT '  Creating ProductSubscriptions table...';

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'ProductSubscriptions')
BEGIN
    CREATE TABLE ProductSubscriptions (
        Id INT PRIMARY KEY IDENTITY(1,1),
        UserId INT NOT NULL,
        ProductType INT NOT NULL,  -- 0=Cards, 1=Menu, 2=Catalog
        TierId INT NOT NULL,

        -- Trial tracking per product
        IsTrialActive BIT NOT NULL DEFAULT 1,
        TrialStartDate DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        TrialEndDate DATETIME2 NOT NULL,

        -- Billing
        Status INT NOT NULL DEFAULT 0,  -- 0=Trial, 1=Active, 2=Cancelled, 3=Expired
        BillingCycle INT NOT NULL DEFAULT 0,  -- 0=Monthly, 1=Yearly
        CurrentPeriodStart DATETIME2 NULL,
        CurrentPeriodEnd DATETIME2 NULL,
        NextBillingDate DATETIME2 NULL,

        -- Metadata
        IsActive BIT NOT NULL DEFAULT 1,
        CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        UpdatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        CancelledAt DATETIME2 NULL,

        CONSTRAINT FK_ProductSubscriptions_Users FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE CASCADE,
        CONSTRAINT FK_ProductSubscriptions_Tiers FOREIGN KEY (TierId) REFERENCES SubscriptionTiers(Id),
        CONSTRAINT UQ_ProductSubscriptions_UserProduct UNIQUE (UserId, ProductType)
    );

    CREATE INDEX IX_ProductSubscriptions_UserId ON ProductSubscriptions(UserId);
    CREATE INDEX IX_ProductSubscriptions_Status ON ProductSubscriptions(Status);
    CREATE INDEX IX_ProductSubscriptions_ProductType ON ProductSubscriptions(ProductType);

    PRINT '  ✓ ProductSubscriptions table created successfully';
END
ELSE
BEGIN
    PRINT '  ⚠ ProductSubscriptions table already exists, skipping creation';
END

-- ----------------------------------------------------------------------------
-- 1.3 Create CatalogCategories Table (Junction)
-- ----------------------------------------------------------------------------
PRINT '  Creating CatalogCategories table...';

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'CatalogCategories')
BEGIN
    CREATE TABLE CatalogCategories (
        Id INT PRIMARY KEY IDENTITY(1,1),
        CatalogId INT NOT NULL,
        CategoryId INT NOT NULL,
        SortOrder INT NOT NULL DEFAULT 0,
        CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),

        CONSTRAINT FK_CatalogCategories_Catalogs FOREIGN KEY (CatalogId) REFERENCES Catalogs(Id) ON DELETE CASCADE,
        CONSTRAINT FK_CatalogCategories_Categories FOREIGN KEY (CategoryId) REFERENCES Categories(Id) ON DELETE CASCADE,
        CONSTRAINT UQ_CatalogCategories_CatalogCategory UNIQUE (CatalogId, CategoryId)
    );

    CREATE INDEX IX_CatalogCategories_CatalogId ON CatalogCategories(CatalogId);
    CREATE INDEX IX_CatalogCategories_CategoryId ON CatalogCategories(CategoryId);

    PRINT '  ✓ CatalogCategories table created successfully';
END
ELSE
BEGIN
    PRINT '  ⚠ CatalogCategories table already exists, skipping creation';
END

-- ============================================================================
-- PHASE 2: UPDATE EXISTING TABLES
-- ============================================================================

PRINT 'Phase 2: Updating existing tables...';

-- ----------------------------------------------------------------------------
-- 2.1 Update SubscriptionTiers Table
-- ----------------------------------------------------------------------------
PRINT '  Updating SubscriptionTiers table...';

-- Add ProductType column
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('SubscriptionTiers') AND name = 'ProductType')
BEGIN
    ALTER TABLE SubscriptionTiers ADD ProductType INT NOT NULL DEFAULT 1;  -- Default to Menu product
    CREATE INDEX IX_SubscriptionTiers_ProductType ON SubscriptionTiers(ProductType);
    PRINT '    ✓ Added ProductType column';
END

-- Add MaxEntities column
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('SubscriptionTiers') AND name = 'MaxEntities')
BEGIN
    ALTER TABLE SubscriptionTiers ADD MaxEntities INT NOT NULL DEFAULT 1;
    PRINT '    ✓ Added MaxEntities column';
END

-- Add MaxCatalogsPerEntity column
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('SubscriptionTiers') AND name = 'MaxCatalogsPerEntity')
BEGIN
    ALTER TABLE SubscriptionTiers ADD MaxCatalogsPerEntity INT NOT NULL DEFAULT 3;
    PRINT '    ✓ Added MaxCatalogsPerEntity column';
END

-- Add MaxLibraryItems column
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('SubscriptionTiers') AND name = 'MaxLibraryItems')
BEGIN
    ALTER TABLE SubscriptionTiers ADD MaxLibraryItems INT NOT NULL DEFAULT 50;
    PRINT '    ✓ Added MaxLibraryItems column';
END

-- Add MaxCategoriesPerCatalog column
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('SubscriptionTiers') AND name = 'MaxCategoriesPerCatalog')
BEGIN
    ALTER TABLE SubscriptionTiers ADD MaxCategoriesPerCatalog INT NOT NULL DEFAULT 10;
    PRINT '    ✓ Added MaxCategoriesPerCatalog column';
END

-- Add MaxBundles column
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('SubscriptionTiers') AND name = 'MaxBundles')
BEGIN
    ALTER TABLE SubscriptionTiers ADD MaxBundles INT NOT NULL DEFAULT 0;
    PRINT '    ✓ Added MaxBundles column';
END

PRINT '  ✓ SubscriptionTiers table updated successfully';

-- ----------------------------------------------------------------------------
-- 2.2 Update Catalogs Table
-- ----------------------------------------------------------------------------
PRINT '  Updating Catalogs table...';

-- Add EntityId column (nullable first for migration)
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Catalogs') AND name = 'EntityId')
BEGIN
    ALTER TABLE Catalogs ADD EntityId INT NULL;
    PRINT '    ✓ Added EntityId column (nullable)';
END

-- ----------------------------------------------------------------------------
-- 2.3 Update CatalogItems Table
-- ----------------------------------------------------------------------------
PRINT '  Updating CatalogItems table...';

-- Add ParentCatalogItemId for item sharing
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('CatalogItems') AND name = 'ParentCatalogItemId')
BEGIN
    ALTER TABLE CatalogItems ADD ParentCatalogItemId INT NULL;
    PRINT '    ✓ Added ParentCatalogItemId column';
END

-- Add PriceOverride for per-catalog pricing
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('CatalogItems') AND name = 'PriceOverride')
BEGIN
    ALTER TABLE CatalogItems ADD PriceOverride DECIMAL(18,2) NULL;
    PRINT '    ✓ Added PriceOverride column';
END

-- ----------------------------------------------------------------------------
-- 2.4 Update Categories Table
-- ----------------------------------------------------------------------------
PRINT '  Updating Categories table...';

-- Add EntityId column (nullable first for migration)
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Categories') AND name = 'EntityId')
BEGIN
    ALTER TABLE Categories ADD EntityId INT NULL;
    PRINT '    ✓ Added EntityId column (nullable)';
END

-- ============================================================================
-- PHASE 3: DATA MIGRATION
-- ============================================================================

PRINT 'Phase 3: Migrating existing data...';

-- ----------------------------------------------------------------------------
-- 3.1 Create Default Entities from Existing Profiles
-- ----------------------------------------------------------------------------
PRINT '  Creating default entities from profiles...';

-- Only create entities if Profiles table exists and has data
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'Profiles')
BEGIN
    INSERT INTO Entities (UserId, EntityType, Name, Slug, Description, Logo, Address, City, Phone, Email, Currency, SortOrder, IsActive, CreatedAt, UpdatedAt, CreatedBy)
    SELECT DISTINCT
        p.UserId,
        0 as EntityType,  -- Default to Restaurant
        COALESCE(p.BusinessName, 'Main Restaurant') as Name,
        COALESCE(p.Slug, LOWER(REPLACE(CAST(NEWID() AS NVARCHAR(50)), '-', ''))) as Slug,
        p.Bio as Description,
        p.ProfileImage as Logo,
        NULL as Address,  -- Profiles don't have address field
        NULL as City,
        NULL as Phone,
        NULL as Email,
        'ZAR' as Currency,
        0 as SortOrder,
        1 as IsActive,
        COALESCE(p.CreatedAt, GETUTCDATE()) as CreatedAt,
        GETUTCDATE() as UpdatedAt,
        p.CreatedBy
    FROM Profiles p
    WHERE p.UserId IS NOT NULL
    AND NOT EXISTS (
        SELECT 1 FROM Entities e WHERE e.UserId = p.UserId
    );

    PRINT '    ✓ Created ' + CAST(@@ROWCOUNT AS NVARCHAR(10)) + ' default entities';
END

-- For users without profiles, create a default entity
INSERT INTO Entities (UserId, EntityType, Name, Slug, Currency, IsActive, CreatedAt, UpdatedAt)
SELECT DISTINCT
    u.Id,
    0 as EntityType,
    'My Restaurant' as Name,
    'my-restaurant-' + LOWER(REPLACE(CAST(NEWID() AS NVARCHAR(36)), '-', '')) as Slug,
    'ZAR' as Currency,
    1 as IsActive,
    GETUTCDATE(),
    GETUTCDATE()
FROM Users u
WHERE NOT EXISTS (SELECT 1 FROM Entities e WHERE e.UserId = u.Id);

PRINT '    ✓ Created default entities for users without profiles: ' + CAST(@@ROWCOUNT AS NVARCHAR(10));

-- ----------------------------------------------------------------------------
-- 3.2 Link Catalogs to Entities
-- ----------------------------------------------------------------------------
PRINT '  Linking catalogs to entities...';

-- Link catalogs to their user's default entity (first entity for each user)
UPDATE c
SET c.EntityId = e.Id
FROM Catalogs c
INNER JOIN Users u ON c.CreatedBy = CAST(u.Id AS NVARCHAR(50))
INNER JOIN (
    SELECT UserId, MIN(Id) as Id
    FROM Entities
    GROUP BY UserId
) e ON e.UserId = u.Id
WHERE c.EntityId IS NULL;

PRINT '    ✓ Linked ' + CAST(@@ROWCOUNT AS NVARCHAR(10)) + ' catalogs to entities';

-- ----------------------------------------------------------------------------
-- 3.3 Link Categories to Entities via Catalogs
-- ----------------------------------------------------------------------------
PRINT '  Linking categories to entities...';

-- Link categories to entities based on their catalogs
UPDATE cat
SET cat.EntityId = c.EntityId
FROM Categories cat
INNER JOIN Catalogs c ON c.Id = (
    SELECT TOP 1 ci.CatalogId
    FROM CatalogItems ci
    INNER JOIN CatalogItemCategories cic ON cic.CatalogItemId = ci.Id
    WHERE cic.CategoryId = cat.Id
)
WHERE cat.EntityId IS NULL AND c.EntityId IS NOT NULL;

PRINT '    ✓ Linked categories to entities: ' + CAST(@@ROWCOUNT AS NVARCHAR(10));

-- For categories without catalog link, use user's first entity
UPDATE cat
SET cat.EntityId = e.Id
FROM Categories cat
INNER JOIN Users u ON cat.UserId = u.Id
INNER JOIN (
    SELECT UserId, MIN(Id) as Id
    FROM Entities
    GROUP BY UserId
) e ON e.UserId = u.Id
WHERE cat.EntityId IS NULL;

PRINT '    ✓ Linked orphaned categories: ' + CAST(@@ROWCOUNT AS NVARCHAR(10));

-- ----------------------------------------------------------------------------
-- 3.4 Populate CatalogCategories Junction Table
-- ----------------------------------------------------------------------------
PRINT '  Populating CatalogCategories junction table...';

-- Create CatalogCategories entries from existing CatalogItemCategories
INSERT INTO CatalogCategories (CatalogId, CategoryId, SortOrder, CreatedAt)
SELECT DISTINCT
    ci.CatalogId,
    cic.CategoryId,
    0 as SortOrder,
    GETUTCDATE()
FROM CatalogItems ci
INNER JOIN CatalogItemCategories cic ON cic.CatalogItemId = ci.Id
WHERE ci.CatalogId IS NOT NULL
AND NOT EXISTS (
    SELECT 1 FROM CatalogCategories cc
    WHERE cc.CatalogId = ci.CatalogId AND cc.CategoryId = cic.CategoryId
);

PRINT '    ✓ Created ' + CAST(@@ROWCOUNT AS NVARCHAR(10)) + ' catalog-category links';

-- ----------------------------------------------------------------------------
-- 3.5 Migrate Existing Subscriptions to ProductSubscriptions
-- ----------------------------------------------------------------------------
PRINT '  Migrating subscriptions to product subscriptions...';

-- Migrate existing subscriptions to Menu product subscriptions
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'Subscriptions')
BEGIN
    INSERT INTO ProductSubscriptions (
        UserId,
        ProductType,
        TierId,
        IsTrialActive,
        TrialStartDate,
        TrialEndDate,
        Status,
        BillingCycle,
        CurrentPeriodStart,
        CurrentPeriodEnd,
        NextBillingDate,
        IsActive,
        CreatedAt,
        UpdatedAt,
        CancelledAt
    )
    SELECT
        s.UserId,
        1 as ProductType,  -- Menu product
        s.TierId,
        s.IsTrialActive,
        s.TrialStartDate,
        s.TrialEndDate,
        s.Status,
        COALESCE(s.BillingCycle, 0) as BillingCycle,
        s.CurrentPeriodStart,
        s.CurrentPeriodEnd,
        s.NextBillingDate,
        s.IsActive,
        COALESCE(s.CreatedAt, GETUTCDATE()) as CreatedAt,
        GETUTCDATE() as UpdatedAt,
        s.CancelledAt
    FROM Subscriptions s
    WHERE s.IsActive = 1
    AND NOT EXISTS (
        SELECT 1 FROM ProductSubscriptions ps
        WHERE ps.UserId = s.UserId AND ps.ProductType = 1
    );

    PRINT '    ✓ Migrated ' + CAST(@@ROWCOUNT AS NVARCHAR(10)) + ' subscriptions to ProductSubscriptions';
END

-- ============================================================================
-- PHASE 4: ADD CONSTRAINTS AND FINALIZE
-- ============================================================================

PRINT 'Phase 4: Adding constraints and finalizing...';

-- ----------------------------------------------------------------------------
-- 4.1 Make EntityId Required in Catalogs
-- ----------------------------------------------------------------------------
PRINT '  Making EntityId required in Catalogs...';

-- Check if any catalogs still don't have EntityId
DECLARE @CatalogsWithoutEntity INT;
SELECT @CatalogsWithoutEntity = COUNT(*) FROM Catalogs WHERE EntityId IS NULL;

IF @CatalogsWithoutEntity > 0
BEGIN
    PRINT '    ⚠ WARNING: ' + CAST(@CatalogsWithoutEntity AS NVARCHAR(10)) + ' catalogs without EntityId found!';
    PRINT '    Creating fallback entities...';

    -- Create fallback entities for orphaned catalogs
    INSERT INTO Entities (UserId, EntityType, Name, Slug, Currency, IsActive, CreatedAt, UpdatedAt)
    SELECT DISTINCT
        1 as UserId,  -- System user
        0 as EntityType,
        'Orphaned Catalog Entity',
        'orphaned-' + LOWER(REPLACE(CAST(NEWID() AS NVARCHAR(36)), '-', '')) as Slug,
        'ZAR',
        1,
        GETUTCDATE(),
        GETUTCDATE();

    -- Link orphaned catalogs to fallback entity
    UPDATE c
    SET c.EntityId = e.Id
    FROM Catalogs c
    CROSS JOIN (SELECT TOP 1 Id FROM Entities WHERE Name = 'Orphaned Catalog Entity' ORDER BY Id DESC) e
    WHERE c.EntityId IS NULL;
END

-- Make EntityId NOT NULL
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Catalogs') AND name = 'EntityId' AND is_nullable = 1)
BEGIN
    ALTER TABLE Catalogs ALTER COLUMN EntityId INT NOT NULL;
    PRINT '    ✓ Made EntityId required in Catalogs';
END

-- Add foreign key constraint
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Catalogs_Entities')
BEGIN
    ALTER TABLE Catalogs ADD CONSTRAINT FK_Catalogs_Entities
        FOREIGN KEY (EntityId) REFERENCES Entities(Id) ON DELETE CASCADE;
    CREATE INDEX IX_Catalogs_EntityId ON Catalogs(EntityId);
    PRINT '    ✓ Added FK_Catalogs_Entities constraint';
END

-- ----------------------------------------------------------------------------
-- 4.2 Make EntityId Required in Categories
-- ----------------------------------------------------------------------------
PRINT '  Making EntityId required in Categories...';

-- Check for categories without EntityId
DECLARE @CategoriesWithoutEntity INT;
SELECT @CategoriesWithoutEntity = COUNT(*) FROM Categories WHERE EntityId IS NULL;

IF @CategoriesWithoutEntity > 0
BEGIN
    PRINT '    ⚠ WARNING: ' + CAST(@CategoriesWithoutEntity AS NVARCHAR(10)) + ' categories without EntityId found!';

    -- Link to first available entity as fallback
    UPDATE c
    SET c.EntityId = e.Id
    FROM Categories c
    CROSS JOIN (SELECT TOP 1 Id FROM Entities ORDER BY Id) e
    WHERE c.EntityId IS NULL;
END

-- Make EntityId NOT NULL
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Categories') AND name = 'EntityId' AND is_nullable = 1)
BEGIN
    ALTER TABLE Categories ALTER COLUMN EntityId INT NOT NULL;
    PRINT '    ✓ Made EntityId required in Categories';
END

-- Add foreign key constraint
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Categories_Entities')
BEGIN
    ALTER TABLE Categories ADD CONSTRAINT FK_Categories_Entities
        FOREIGN KEY (EntityId) REFERENCES Entities(Id) ON DELETE CASCADE;
    CREATE INDEX IX_Categories_EntityId ON Categories(EntityId);
    PRINT '    ✓ Added FK_Categories_Entities constraint';
END

-- Remove UserId from Categories (now entity-owned, not user-owned)
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Categories') AND name = 'UserId')
BEGIN
    -- Drop FK constraint first if it exists
    IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Categories_Users')
    BEGIN
        ALTER TABLE Categories DROP CONSTRAINT FK_Categories_Users;
    END

    ALTER TABLE Categories DROP COLUMN UserId;
    PRINT '    ✓ Removed UserId from Categories';
END

-- ----------------------------------------------------------------------------
-- 4.3 Add Foreign Key for ParentCatalogItemId
-- ----------------------------------------------------------------------------
PRINT '  Adding ParentCatalogItemId foreign key...';

IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_CatalogItems_ParentCatalogItem')
BEGIN
    ALTER TABLE CatalogItems ADD CONSTRAINT FK_CatalogItems_ParentCatalogItem
        FOREIGN KEY (ParentCatalogItemId) REFERENCES CatalogItems(Id);
    CREATE INDEX IX_CatalogItems_ParentCatalogItemId ON CatalogItems(ParentCatalogItemId);
    PRINT '    ✓ Added FK_CatalogItems_ParentCatalogItem constraint';
END

-- ============================================================================
-- PHASE 5: VALIDATION
-- ============================================================================

PRINT 'Phase 5: Validating migration...';

-- Count records
DECLARE @EntityCount INT, @ProductSubCount INT, @CatalogCatCount INT;
SELECT @EntityCount = COUNT(*) FROM Entities;
SELECT @ProductSubCount = COUNT(*) FROM ProductSubscriptions;
SELECT @CatalogCatCount = COUNT(*) FROM CatalogCategories;

PRINT '  Validation Results:';
PRINT '    Entities: ' + CAST(@EntityCount AS NVARCHAR(10));
PRINT '    ProductSubscriptions: ' + CAST(@ProductSubCount AS NVARCHAR(10));
PRINT '    CatalogCategories: ' + CAST(@CatalogCatCount AS NVARCHAR(10));

-- Check for orphaned records
DECLARE @OrphanedCatalogs INT, @OrphanedCategories INT;
SELECT @OrphanedCatalogs = COUNT(*) FROM Catalogs WHERE EntityId IS NULL;
SELECT @OrphanedCategories = COUNT(*) FROM Categories WHERE EntityId IS NULL;

IF @OrphanedCatalogs > 0 OR @OrphanedCategories > 0
BEGIN
    PRINT '  ⚠ WARNING: Found orphaned records:';
    IF @OrphanedCatalogs > 0 PRINT '    - Catalogs without EntityId: ' + CAST(@OrphanedCatalogs AS NVARCHAR(10));
    IF @OrphanedCategories > 0 PRINT '    - Categories without EntityId: ' + CAST(@OrphanedCategories AS NVARCHAR(10));
END
ELSE
BEGIN
    PRINT '  ✓ No orphaned records found';
END

COMMIT TRANSACTION;

PRINT '================================================================';
PRINT 'Migration 001 completed successfully!';
PRINT 'Total Entities created: ' + CAST(@EntityCount AS NVARCHAR(10));
PRINT 'Total ProductSubscriptions migrated: ' + CAST(@ProductSubCount AS NVARCHAR(10));
PRINT '================================================================';
