-- =============================================
-- Migration Cleanup Script
-- Run this if migration failed partway through
-- =============================================

-- 1. Drop foreign key if exists
SET @fk_exists = (
    SELECT COUNT(*) 
    FROM information_schema.TABLE_CONSTRAINTS 
    WHERE CONSTRAINT_SCHEMA = DATABASE()
    AND TABLE_NAME = 'UserSubscriptions'
    AND CONSTRAINT_NAME = 'FK_UserSubscriptions_ProductLines_ProductLineId'
);
SET @sql = IF(@fk_exists > 0, 
    'ALTER TABLE `UserSubscriptions` DROP FOREIGN KEY `FK_UserSubscriptions_ProductLines_ProductLineId`', 
    'SELECT ''Foreign key does not exist, skipping...''');
PREPARE stmt FROM @sql;
EXECUTE stmt;
DEALLOCATE PREPARE stmt;

-- 2. Drop ProductLineId column if exists
SET @col_exists = (
    SELECT COUNT(*) 
    FROM information_schema.COLUMNS 
    WHERE TABLE_SCHEMA = DATABASE()
    AND TABLE_NAME = 'UserSubscriptions'
    AND COLUMN_NAME = 'ProductLineId'
);
SET @sql = IF(@col_exists > 0, 
    'ALTER TABLE `UserSubscriptions` DROP COLUMN `ProductLineId`', 
    'SELECT ''Column does not exist, skipping...''');
PREPARE stmt FROM @sql;
EXECUTE stmt;

