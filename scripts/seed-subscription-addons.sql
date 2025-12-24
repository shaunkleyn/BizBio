-- =============================================
-- Subscription Addons Seed Data
-- Run this after applying the migration
-- =============================================

-- Note: Adjust TierIds based on your actual SubscriptionTiers table
-- Assuming:
-- ProductLineId 1 = Menu
-- TierId 1 = Starter (R149/m) - No addons available
-- TierId 2 = Entree (R299/m) - All addons available
-- TierId 3 = Banquet (R599/m) - All addons available with 20% discount

-- =============================================
-- 1. Insert Menu Product Addons
-- =============================================

INSERT INTO SubscriptionAddons 
    (Name, Description, ProductLineId, MonthlyPrice, ConfigurationJson, SortOrder, IsActive, IsValid, CreatedAt, UpdatedAt, CreatedBy, UpdatedBy)
VALUES
    (
        'Waiter Call',
        'Enable table-side waiter call buttons for your digital menu. Customers can request service directly from their phones.',
        1, -- Menu product
        49.00,
        '{"features": ["realtime_notifications", "custom_call_messages", "waiter_response_tracking"]}',
        1,
        1, -- IsActive
        1, -- IsValid
        NOW(),
        NOW(),
        'system',
        'system'
    ),
    (
        'Order Sending',
        'Allow customers to send orders directly to your WhatsApp number with itemized details and pricing.',
        1, -- Menu product
        79.00,
        '{"features": ["whatsapp_integration", "order_formatting", "customer_details_capture"]}',
        2,
        1,
        1,
        NOW(),
        NOW(),
        'system',
        'system'
    ),
    (
        'Menu Pro',
        'Advanced menu customization, detailed analytics, A/B testing, and premium design templates.',
        1, -- Menu product
        149.00,
        '{"features": ["advanced_analytics", "ab_testing", "premium_templates", "custom_css", "advanced_scheduling"]}',
        3,
        1,
        1,
        NOW(),
        NOW(),
        'system',
        'system'
    );

-- =============================================
-- 2. Link Addons to Tiers
-- =============================================

-- Get the addon IDs we just created (using variables for readability)
SET @waiterCallId = (SELECT Id FROM SubscriptionAddons WHERE Name = 'Waiter Call' LIMIT 1);
SET @orderSendingId = (SELECT Id FROM SubscriptionAddons WHERE Name = 'Order Sending' LIMIT 1);
SET @menuProId = (SELECT Id FROM SubscriptionAddons WHERE Name = 'Menu Pro' LIMIT 1);

-- =============================================
-- Link addons to Entree tier (TierId = 2)
-- All addons available at full price
-- =============================================

INSERT INTO SubscriptionTierAddons
    (SubscriptionTierId, AddonId, IsRequired, DiscountPercentage, IsActive, IsValid, CreatedAt, UpdatedAt, CreatedBy, UpdatedBy)
VALUES
    (2, @waiterCallId, 0, NULL, 1, 1, NOW(), NOW(), 'system', 'system'),
    (2, @orderSendingId, 0, NULL, 1, 1, NOW(), NOW(), 'system', 'system'),
    (2, @menuProId, 0, NULL, 1, 1, NOW(), NOW(), 'system', 'system');

-- =============================================
-- Link addons to Banquet tier (TierId = 3)
-- All addons available with 20% discount
-- =============================================

INSERT INTO SubscriptionTierAddons
    (SubscriptionTierId, AddonId, IsRequired, DiscountPercentage, IsActive, IsValid, CreatedAt, UpdatedAt, CreatedBy, UpdatedBy)
VALUES
    (3, @waiterCallId, 0, 20.00, 1, 1, NOW(), NOW(), 'system', 'system'), -- R49 - 20% = R39.20
    (3, @orderSendingId, 0, 20.00, 1, 1, NOW(), NOW(), 'system', 'system'), -- R79 - 20% = R63.20
    (3, @menuProId, 0, 20.00, 1, 1, NOW(), NOW(), 'system', 'system'); -- R149 - 20% = R119.20

-- =============================================
-- Verification Queries
-- =============================================

-- View all addons
SELECT 
    sa.Id,
    sa.Name,
    sa.MonthlyPrice,
    pl.Name AS ProductLine,
    sa.SortOrder
FROM SubscriptionAddons sa
JOIN ProductLineLookup pl ON sa.ProductLineId = pl.Id
WHERE sa.IsActive = 1
ORDER BY sa.ProductLineId, sa.SortOrder;

-- View addon availability per tier
SELECT 
    st.TierName,
    st.MonthlyPrice AS TierPrice,
    sa.Name AS AddonName,
    sa.MonthlyPrice AS BasePrice,
    sta.DiscountPercentage,
    CASE 
        WHEN sta.DiscountPercentage IS NOT NULL 
        THEN sa.MonthlyPrice - (sa.MonthlyPrice * sta.DiscountPercentage / 100)
        ELSE sa.MonthlyPrice
    END AS FinalPrice
FROM SubscriptionTiers st
LEFT JOIN SubscriptionTierAddons sta ON st.Id = sta.SubscriptionTierId AND sta.IsActive = 1
LEFT JOIN SubscriptionAddons sa ON sta.AddonId = sa.Id AND sa.IsActive = 1
WHERE st.ProductLineId = 1 -- Menu product
ORDER BY st.DisplayOrder, sa.SortOrder;

-- =============================================
-- FUTURE: Card Product Addons (Commented Out)
-- Uncomment when implementing Card product
-- =============================================

/*
INSERT INTO SubscriptionAddons 
    (Name, Description, ProductLineId, MonthlyPrice, ConfigurationJson, SortOrder, IsActive, IsValid, CreatedAt, UpdatedAt, CreatedBy, UpdatedBy)
VALUES
    (
        'QR Code Generator',
        'Generate unlimited custom QR codes for your business cards with tracking and analytics.',
        2, -- Card product
        29.00,
        '{"features": ["unlimited_qr_codes", "qr_analytics", "custom_designs"]}',
        1,
        1,
        1,
        NOW(),
        NOW(),
        'system',
        'system'
    ),
    (
        'Team Directory',
        'Create an organizational directory with unlimited employee profiles and hierarchy management.',
        2, -- Card product
        99.00,
        '{"features": ["unlimited_employees", "org_chart", "team_management", "role_based_access"]}',
        2,
        1,
        1,
        NOW(),
        NOW(),
        'system',
        'system'
    );
*/

-- =============================================
-- FUTURE: Retail Product Addons (Commented Out)
-- Uncomment when implementing Retail product
-- =============================================

/*
INSERT INTO SubscriptionAddons 
    (Name, Description, ProductLineId, MonthlyPrice, ConfigurationJson, SortOrder, IsActive, IsValid, CreatedAt, UpdatedAt, CreatedBy, UpdatedBy)
VALUES
    (
        'Inventory Management',
        'Track stock levels, low stock alerts, and multi-location inventory synchronization.',
        3, -- Retail product
        129.00,
        '{"features": ["stock_tracking", "low_stock_alerts", "multi_location_sync", "inventory_reports"]}',
        1,
        1,
        1,
        NOW(),
        NOW(),
        'system',
        'system'
    ),
    (
        'Advanced Analytics',
        'In-depth sales analytics, customer behavior tracking, and conversion optimization insights.',
        3, -- Retail product
        79.00,
        '{"features": ["sales_analytics", "customer_insights", "conversion_tracking", "custom_reports"]}',
        2,
        1,
        1,
        NOW(),
        NOW(),
        'system',
        'system'
    );
*/
