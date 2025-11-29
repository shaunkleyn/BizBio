# BizBio Platform - Subscription System Technical Specification

**Version:** 2.1  
**Date:** November 2025  
**Status:** Ready for Implementation  
**Changes:** Vertical-specific subscription tiers

---

## Table of Contents

1. [Overview](#1-overview)
2. [Database Schema Updates](#2-database-schema-updates)
3. [Entity Models](#3-entity-models)
4. [API Endpoints](#4-api-endpoints)
5. [DTOs (Data Transfer Objects)](#5-dtos)
6. [Feature Flag System](#6-feature-flag-system)
7. [Business Logic](#7-business-logic)
8. [Migration Strategy](#8-migration-strategy)
9. [Testing Requirements](#9-testing-requirements)
10. [Implementation Guide](#10-implementation-guide)

---

## 1. Overview

### 1.1 Subscription Architecture

BizBio now implements a **vertical-specific subscription model** with three distinct product lines:

1. **BizBio Professional** - Digital business cards
2. **BizBio Menu** - Digital restaurant menus
3. **BizBio Retail** - Product catalogs

Each product line has its own tiers with specific features and limits. Users can subscribe to multiple product lines (bundles) with discounts.

### 1.2 Key Concepts

**Product Line:** The vertical market segment (Professional, Menu, Retail)  
**Subscription Tier:** The specific plan level within a product line  
**Bundle:** Combination of multiple product lines with discount  
**Feature Limits:** Quantitative restrictions based on tier  
**Feature Flags:** Binary feature availability based on tier

---

## 2. Database Schema Updates

### 2.1 SubscriptionTiers Table (NEW STRUCTURE)

```sql
CREATE TABLE SubscriptionTiers (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    ProductLine ENUM('Professional', 'Menu', 'Retail') NOT NULL,
    TierName VARCHAR(50) NOT NULL,
    TierCode VARCHAR(50) NOT NULL UNIQUE,
    DisplayName VARCHAR(100) NOT NULL,
    Description TEXT,
    MonthlyPrice DECIMAL(10,2) NOT NULL,
    AnnualPrice DECIMAL(10,2) NOT NULL,
    AnnualDiscountPercent DECIMAL(5,2) DEFAULT 15.00,
    
    -- Feature Limits
    MaxProfiles INT DEFAULT 0,
    MaxCatalogItems INT DEFAULT 0,
    MaxLocations INT DEFAULT 1,
    MaxTeamMembers INT DEFAULT 0,
    MaxDocuments INT DEFAULT 0,
    MaxDocumentSizeMB INT DEFAULT 5,
    MaxImagesPerItem INT DEFAULT 5,
    
    -- Feature Flags
    CustomBranding BOOLEAN DEFAULT FALSE,
    RemoveBranding BOOLEAN DEFAULT FALSE,
    Analytics BOOLEAN DEFAULT FALSE,
    AdvancedAnalytics BOOLEAN DEFAULT FALSE,
    ApiAccess BOOLEAN DEFAULT FALSE,
    WhiteLabel BOOLEAN DEFAULT FALSE,
    CustomDomain BOOLEAN DEFAULT FALSE,
    CustomSubdomain BOOLEAN DEFAULT FALSE,
    PrioritySupport BOOLEAN DEFAULT FALSE,
    PhoneSupport BOOLEAN DEFAULT FALSE,
    DedicatedManager BOOLEAN DEFAULT FALSE,
    
    -- Catalog-Specific Features
    ItemVariants BOOLEAN DEFAULT FALSE,
    ItemAddons BOOLEAN DEFAULT FALSE,
    AllergenInfo BOOLEAN DEFAULT FALSE,
    DietaryTags BOOLEAN DEFAULT FALSE,
    MenuScheduling BOOLEAN DEFAULT FALSE,
    MultiLanguage BOOLEAN DEFAULT FALSE,
    NutritionalInfo BOOLEAN DEFAULT FALSE,
    
    -- Retail-Specific Features
    InventoryTracking BOOLEAN DEFAULT FALSE,
    MultiLocationInventory BOOLEAN DEFAULT FALSE,
    BulkOperations BOOLEAN DEFAULT FALSE,
    BarcodeGeneration BOOLEAN DEFAULT FALSE,
    B2BFeatures BOOLEAN DEFAULT FALSE,
    
    -- Professional-Specific Features
    OrganizationalHierarchy BOOLEAN DEFAULT FALSE,
    TeamManagement BOOLEAN DEFAULT FALSE,
    SSOIntegration BOOLEAN DEFAULT FALSE,
    
    -- Metadata
    DisplayOrder INT DEFAULT 0,
    IsActive BOOLEAN DEFAULT TRUE,
    IsPopular BOOLEAN DEFAULT FALSE,
    RecommendedFor VARCHAR(255),
    CreatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    
    INDEX idx_product_line (ProductLine),
    INDEX idx_tier_code (TierCode),
    INDEX idx_active (IsActive)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
```

### 2.2 UserSubscriptions Table (UPDATED)

```sql
CREATE TABLE UserSubscriptions (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    UserId INT NOT NULL,
    TierId INT NOT NULL,
    
    -- Subscription Details
    Status ENUM('Active', 'Cancelled', 'Paused', 'Expired', 'Trial') DEFAULT 'Trial',
    BillingCycle ENUM('Monthly', 'Annual') NOT NULL,
    
    -- Dates
    StartDate DATETIME NOT NULL,
    EndDate DATETIME,
    TrialEndsAt DATETIME,
    NextBillingDate DATETIME,
    CancelledAt DATETIME,
    PausedAt DATETIME,
    
    -- Pricing (stored for history)
    Price DECIMAL(10,2) NOT NULL,
    Currency VARCHAR(3) DEFAULT 'ZAR',
    DiscountPercent DECIMAL(5,2) DEFAULT 0,
    DiscountReason VARCHAR(100),
    
    -- Limits Override (for custom deals)
    CustomMaxProfiles INT NULL,
    CustomMaxCatalogItems INT NULL,
    CustomMaxLocations INT NULL,
    
    -- Payment
    PaymentMethod VARCHAR(50),
    LastPaymentDate DATETIME,
    LastPaymentAmount DECIMAL(10,2),
    
    -- Auto-renewal
    AutoRenew BOOLEAN DEFAULT TRUE,
    
    -- Metadata
    CreatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    
    FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE CASCADE,
    FOREIGN KEY (TierId) REFERENCES SubscriptionTiers(Id),
    
    INDEX idx_user_id (UserId),
    INDEX idx_status (Status),
    INDEX idx_tier_id (TierId),
    INDEX idx_next_billing (NextBillingDate),
    INDEX idx_trial_ends (TrialEndsAt)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
```

### 2.3 SubscriptionBundles Table (NEW)

```sql
CREATE TABLE SubscriptionBundles (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    BundleCode VARCHAR(50) NOT NULL UNIQUE,
    BundleName VARCHAR(100) NOT NULL,
    Description TEXT,
    
    -- Component Tiers
    PrimaryTierId INT NOT NULL,
    SecondaryTierId INT NOT NULL,
    TertiaryTierId INT NULL,
    
    -- Pricing
    MonthlyPrice DECIMAL(10,2) NOT NULL,
    AnnualPrice DECIMAL(10,2) NOT NULL,
    DiscountPercent DECIMAL(5,2) NOT NULL,
    
    -- Display
    DisplayOrder INT DEFAULT 0,
    IsActive BOOLEAN DEFAULT TRUE,
    IsPopular BOOLEAN DEFAULT FALSE,
    
    CreatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    
    FOREIGN KEY (PrimaryTierId) REFERENCES SubscriptionTiers(Id),
    FOREIGN KEY (SecondaryTierId) REFERENCES SubscriptionTiers(Id),
    FOREIGN KEY (TertiaryTierId) REFERENCES SubscriptionTiers(Id),
    
    INDEX idx_bundle_code (BundleCode),
    INDEX idx_active (IsActive)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
```

### 2.4 UserBundleSubscriptions Table (NEW)

```sql
CREATE TABLE UserBundleSubscriptions (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    UserId INT NOT NULL,
    BundleId INT NOT NULL,
    
    -- This creates multiple UserSubscriptions (one per tier in bundle)
    -- This table tracks the bundle relationship
    
    Status ENUM('Active', 'Cancelled', 'Expired') DEFAULT 'Active',
    BillingCycle ENUM('Monthly', 'Annual') NOT NULL,
    
    Price DECIMAL(10,2) NOT NULL,
    DiscountAmount DECIMAL(10,2) NOT NULL,
    
    StartDate DATETIME NOT NULL,
    EndDate DATETIME,
    NextBillingDate DATETIME,
    
    AutoRenew BOOLEAN DEFAULT TRUE,
    
    CreatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    
    FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE CASCADE,
    FOREIGN KEY (BundleId) REFERENCES SubscriptionBundles(Id),
    
    INDEX idx_user_id (UserId),
    INDEX idx_bundle_id (BundleId),
    INDEX idx_status (Status)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
```

### 2.5 SubscriptionAddOns Table (NEW)

```sql
CREATE TABLE SubscriptionAddOns (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    AddOnCode VARCHAR(50) NOT NULL UNIQUE,
    Name VARCHAR(100) NOT NULL,
    Description TEXT,
    
    Type ENUM('PerUnit', 'Fixed') NOT NULL,
    UnitPrice DECIMAL(10,2),
    FixedPrice DECIMAL(10,2),
    
    -- Applicable to which product lines
    ApplicableToProductLine ENUM('Professional', 'Menu', 'Retail', 'All') DEFAULT 'All',
    MinimumTier VARCHAR(50),
    
    IsActive BOOLEAN DEFAULT TRUE,
    DisplayOrder INT DEFAULT 0,
    
    CreatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    
    INDEX idx_addon_code (AddOnCode),
    INDEX idx_product_line (ApplicableToProductLine)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
```

### 2.6 UserAddOns Table (NEW)

```sql
CREATE TABLE UserAddOns (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    UserId INT NOT NULL,
    SubscriptionId INT NOT NULL,
    AddOnId INT NOT NULL,
    
    Quantity INT DEFAULT 1,
    UnitPrice DECIMAL(10,2) NOT NULL,
    TotalPrice DECIMAL(10,2) NOT NULL,
    
    Status ENUM('Active', 'Cancelled') DEFAULT 'Active',
    StartDate DATETIME NOT NULL,
    EndDate DATETIME,
    
    CreatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    
    FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE CASCADE,
    FOREIGN KEY (SubscriptionId) REFERENCES UserSubscriptions(Id) ON DELETE CASCADE,
    FOREIGN KEY (AddOnId) REFERENCES SubscriptionAddOns(Id),
    
    INDEX idx_user_id (UserId),
    INDEX idx_subscription_id (SubscriptionId),
    INDEX idx_status (Status)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
```

### 2.7 SubscriptionHistory Table (NEW)

```sql
CREATE TABLE SubscriptionHistory (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    UserId INT NOT NULL,
    SubscriptionId INT,
    
    Action ENUM('Created', 'Upgraded', 'Downgraded', 'Cancelled', 'Renewed', 'Paused', 'Resumed', 'Expired') NOT NULL,
    
    FromTierId INT,
    ToTierId INT,
    
    FromPrice DECIMAL(10,2),
    ToPrice DECIMAL(10,2),
    
    Reason TEXT,
    Notes TEXT,
    
    PerformedBy INT,
    PerformedByType ENUM('User', 'Admin', 'System') DEFAULT 'User',
    
    CreatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    
    FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE CASCADE,
    FOREIGN KEY (SubscriptionId) REFERENCES UserSubscriptions(Id) ON DELETE SET NULL,
    FOREIGN KEY (FromTierId) REFERENCES SubscriptionTiers(Id),
    FOREIGN KEY (ToTierId) REFERENCES SubscriptionTiers(Id),
    
    INDEX idx_user_id (UserId),
    INDEX idx_subscription_id (SubscriptionId),
    INDEX idx_action (Action),
    INDEX idx_created_at (CreatedAt)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
```

---

## 3. Entity Models

### 3.1 SubscriptionTier Entity

```csharp
public class SubscriptionTier
{
    public int Id { get; set; }
    
    // Product Line
    public ProductLine ProductLine { get; set; }
    public string TierName { get; set; }
    public string TierCode { get; set; }
    public string DisplayName { get; set; }
    public string Description { get; set; }
    
    // Pricing
    public decimal MonthlyPrice { get; set; }
    public decimal AnnualPrice { get; set; }
    public decimal AnnualDiscountPercent { get; set; } = 15.00m;
    
    // Feature Limits
    public int MaxProfiles { get; set; }
    public int MaxCatalogItems { get; set; }
    public int MaxLocations { get; set; }
    public int MaxTeamMembers { get; set; }
    public int MaxDocuments { get; set; }
    public int MaxDocumentSizeMB { get; set; } = 5;
    public int MaxImagesPerItem { get; set; } = 5;
    
    // Feature Flags
    public bool CustomBranding { get; set; }
    public bool RemoveBranding { get; set; }
    public bool Analytics { get; set; }
    public bool AdvancedAnalytics { get; set; }
    public bool ApiAccess { get; set; }
    public bool WhiteLabel { get; set; }
    public bool CustomDomain { get; set; }
    public bool CustomSubdomain { get; set; }
    public bool PrioritySupport { get; set; }
    public bool PhoneSupport { get; set; }
    public bool DedicatedManager { get; set; }
    
    // Catalog-Specific Features
    public bool ItemVariants { get; set; }
    public bool ItemAddons { get; set; }
    public bool AllergenInfo { get; set; }
    public bool DietaryTags { get; set; }
    public bool MenuScheduling { get; set; }
    public bool MultiLanguage { get; set; }
    public bool NutritionalInfo { get; set; }
    
    // Retail-Specific Features
    public bool InventoryTracking { get; set; }
    public bool MultiLocationInventory { get; set; }
    public bool BulkOperations { get; set; }
    public bool BarcodeGeneration { get; set; }
    public bool B2BFeatures { get; set; }
    
    // Professional-Specific Features
    public bool OrganizationalHierarchy { get; set; }
    public bool TeamManagement { get; set; }
    public bool SSOIntegration { get; set; }
    
    // Metadata
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; } = true;
    public bool IsPopular { get; set; }
    public string RecommendedFor { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    // Navigation
    public ICollection<UserSubscription> UserSubscriptions { get; set; }
}

public enum ProductLine
{
    Professional,
    Menu,
    Retail
}
```

### 3.2 UserSubscription Entity

```csharp
public class UserSubscription
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int TierId { get; set; }
    
    // Status
    public SubscriptionStatus Status { get; set; } = SubscriptionStatus.Trial;
    public BillingCycle BillingCycle { get; set; }
    
    // Dates
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public DateTime? TrialEndsAt { get; set; }
    public DateTime? NextBillingDate { get; set; }
    public DateTime? CancelledAt { get; set; }
    public DateTime? PausedAt { get; set; }
    
    // Pricing
    public decimal Price { get; set; }
    public string Currency { get; set; } = "ZAR";
    public decimal DiscountPercent { get; set; }
    public string DiscountReason { get; set; }
    
    // Custom Limits (for special deals)
    public int? CustomMaxProfiles { get; set; }
    public int? CustomMaxCatalogItems { get; set; }
    public int? CustomMaxLocations { get; set; }
    
    // Payment
    public string PaymentMethod { get; set; }
    public DateTime? LastPaymentDate { get; set; }
    public decimal? LastPaymentAmount { get; set; }
    
    // Auto-renewal
    public bool AutoRenew { get; set; } = true;
    
    // Timestamps
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    // Navigation
    public User User { get; set; }
    public SubscriptionTier Tier { get; set; }
    public ICollection<UserAddOn> AddOns { get; set; }
}

public enum SubscriptionStatus
{
    Active,
    Cancelled,
    Paused,
    Expired,
    Trial
}

public enum BillingCycle
{
    Monthly,
    Annual
}
```

### 3.3 SubscriptionBundle Entity

```csharp
public class SubscriptionBundle
{
    public int Id { get; set; }
    public string BundleCode { get; set; }
    public string BundleName { get; set; }
    public string Description { get; set; }
    
    // Component Tiers
    public int PrimaryTierId { get; set; }
    public int SecondaryTierId { get; set; }
    public int? TertiaryTierId { get; set; }
    
    // Pricing
    public decimal MonthlyPrice { get; set; }
    public decimal AnnualPrice { get; set; }
    public decimal DiscountPercent { get; set; }
    
    // Display
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; } = true;
    public bool IsPopular { get; set; }
    
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    // Navigation
    public SubscriptionTier PrimaryTier { get; set; }
    public SubscriptionTier SecondaryTier { get; set; }
    public SubscriptionTier TertiaryTier { get; set; }
    public ICollection<UserBundleSubscription> UserBundleSubscriptions { get; set; }
}
```

---

## 4. API Endpoints

### 4.1 Public Subscription Endpoints

```
GET /api/v1/subscriptions/tiers
    - Get all available subscription tiers
    - Query params: productLine, isActive
    - Response: List of SubscriptionTierDto

GET /api/v1/subscriptions/tiers/{productLine}
    - Get tiers for specific product line
    - Response: List of SubscriptionTierDto for that product

GET /api/v1/subscriptions/bundles
    - Get all available bundles
    - Response: List of SubscriptionBundleDto

GET /api/v1/subscriptions/bundles/{bundleCode}
    - Get specific bundle details
    - Response: SubscriptionBundleDto

GET /api/v1/subscriptions/add-ons
    - Get available add-ons
    - Query params: productLine
    - Response: List of AddOnDto
```

### 4.2 User Subscription Management

```
GET /api/v1/users/me/subscriptions
    - Get user's current subscriptions
    - Auth: JWT Required
    - Response: List of UserSubscriptionDto

GET /api/v1/users/me/subscriptions/{id}
    - Get specific subscription details
    - Auth: JWT Required
    - Response: UserSubscriptionDto with full details

POST /api/v1/users/me/subscriptions
    - Create new subscription (start trial or paid)
    - Auth: JWT Required
    - Body: CreateSubscriptionDto
    - Response: UserSubscriptionDto

POST /api/v1/users/me/subscriptions/{id}/upgrade
    - Upgrade to higher tier
    - Auth: JWT Required
    - Body: UpgradeSubscriptionDto
    - Response: UserSubscriptionDto

POST /api/v1/users/me/subscriptions/{id}/downgrade
    - Schedule downgrade (takes effect at end of cycle)
    - Auth: JWT Required
    - Body: DowngradeSubscriptionDto
    - Response: UserSubscriptionDto

POST /api/v1/users/me/subscriptions/{id}/cancel
    - Cancel subscription
    - Auth: JWT Required
    - Body: CancelSubscriptionDto (reason, immediate)
    - Response: UserSubscriptionDto

POST /api/v1/users/me/subscriptions/{id}/pause
    - Pause subscription (up to 3 months)
    - Auth: JWT Required
    - Body: PauseSubscriptionDto
    - Response: UserSubscriptionDto

POST /api/v1/users/me/subscriptions/{id}/resume
    - Resume paused subscription
    - Auth: JWT Required
    - Response: UserSubscriptionDto

PUT /api/v1/users/me/subscriptions/{id}/auto-renew
    - Toggle auto-renewal
    - Auth: JWT Required
    - Body: { autoRenew: boolean }
    - Response: UserSubscriptionDto
```

### 4.3 Bundle Subscriptions

```
POST /api/v1/users/me/subscriptions/bundle
    - Subscribe to a bundle
    - Auth: JWT Required
    - Body: CreateBundleSubscriptionDto
    - Response: UserBundleSubscriptionDto

GET /api/v1/users/me/subscriptions/bundles
    - Get user's bundle subscriptions
    - Auth: JWT Required
    - Response: List of UserBundleSubscriptionDto
```

### 4.4 Add-Ons Management

```
GET /api/v1/users/me/subscriptions/{id}/add-ons
    - Get add-ons for specific subscription
    - Auth: JWT Required
    - Response: List of UserAddOnDto

POST /api/v1/users/me/subscriptions/{id}/add-ons
    - Add an add-on to subscription
    - Auth: JWT Required
    - Body: AddSubscriptionAddOnDto
    - Response: UserAddOnDto

DELETE /api/v1/users/me/subscriptions/{id}/add-ons/{addonId}
    - Remove add-on
    - Auth: JWT Required
    - Response: 204 No Content
```

### 4.5 Feature Checking Endpoints

```
GET /api/v1/users/me/features
    - Get all features available to user (combined from all subscriptions)
    - Auth: JWT Required
    - Response: UserFeaturesDto

GET /api/v1/users/me/features/check/{featureName}
    - Check if user has specific feature
    - Auth: JWT Required
    - Response: { hasFeature: boolean, limitRemaining: number }

GET /api/v1/users/me/usage
    - Get usage statistics against limits
    - Auth: JWT Required
    - Response: UsageStatisticsDto
```

### 4.6 Admin Endpoints

```
POST /api/v1/admin/subscriptions/tiers
    - Create new subscription tier
    - Auth: JWT Required (Admin)
    - Body: CreateSubscriptionTierDto
    - Response: SubscriptionTierDto

PUT /api/v1/admin/subscriptions/tiers/{id}
    - Update subscription tier
    - Auth: JWT Required (Admin)
    - Body: UpdateSubscriptionTierDto
    - Response: SubscriptionTierDto

POST /api/v1/admin/users/{userId}/subscriptions/{subscriptionId}/override
    - Override user's subscription limits (custom deal)
    - Auth: JWT Required (Admin)
    - Body: OverrideSubscriptionDto
    - Response: UserSubscriptionDto

GET /api/v1/admin/subscriptions/analytics
    - Get subscription analytics (MRR, churn, etc.)
    - Auth: JWT Required (Admin)
    - Query params: startDate, endDate, productLine
    - Response: SubscriptionAnalyticsDto
```

---

## 5. DTOs

### 5.1 SubscriptionTierDto

```csharp
public class SubscriptionTierDto
{
    public int Id { get; set; }
    public string ProductLine { get; set; }
    public string TierName { get; set; }
    public string TierCode { get; set; }
    public string DisplayName { get; set; }
    public string Description { get; set; }
    
    public PricingDto Pricing { get; set; }
    public FeatureLimitsDto Limits { get; set; }
    public FeatureFlagsDto Features { get; set; }
    
    public int DisplayOrder { get; set; }
    public bool IsPopular { get; set; }
    public string RecommendedFor { get; set; }
}

public class PricingDto
{
    public decimal MonthlyPrice { get; set; }
    public decimal AnnualPrice { get; set; }
    public decimal AnnualDiscountPercent { get; set; }
    public decimal MonthlySavings { get; set; } // Calculated
    public string Currency { get; set; }
    public string FormattedMonthlyPrice { get; set; } // "R99/month"
    public string FormattedAnnualPrice { get; set; } // "R999/year"
}

public class FeatureLimitsDto
{
    public int MaxProfiles { get; set; }
    public int MaxCatalogItems { get; set; }
    public int MaxLocations { get; set; }
    public int MaxTeamMembers { get; set; }
    public int MaxDocuments { get; set; }
    public int MaxDocumentSizeMB { get; set; }
    public int MaxImagesPerItem { get; set; }
    
    public string FormattedMaxProfiles { get; set; } // "5" or "Unlimited"
    public string FormattedMaxCatalogItems { get; set; }
}

public class FeatureFlagsDto
{
    // General Features
    public bool CustomBranding { get; set; }
    public bool RemoveBranding { get; set; }
    public bool Analytics { get; set; }
    public bool AdvancedAnalytics { get; set; }
    public bool ApiAccess { get; set; }
    public bool WhiteLabel { get; set; }
    public bool CustomDomain { get; set; }
    public bool CustomSubdomain { get; set; }
    public bool PrioritySupport { get; set; }
    public bool PhoneSupport { get; set; }
    public bool DedicatedManager { get; set; }
    
    // Catalog Features
    public bool ItemVariants { get; set; }
    public bool ItemAddons { get; set; }
    public bool AllergenInfo { get; set; }
    public bool DietaryTags { get; set; }
    public bool MenuScheduling { get; set; }
    public bool MultiLanguage { get; set; }
    public bool NutritionalInfo { get; set; }
    
    // Retail Features
    public bool InventoryTracking { get; set; }
    public bool MultiLocationInventory { get; set; }
    public bool BulkOperations { get; set; }
    public bool BarcodeGeneration { get; set; }
    public bool B2BFeatures { get; set; }
    
    // Professional Features
    public bool OrganizationalHierarchy { get; set; }
    public bool TeamManagement { get; set; }
    public bool SSOIntegration { get; set; }
}
```

### 5.2 UserSubscriptionDto

```csharp
public class UserSubscriptionDto
{
    public int Id { get; set; }
    public string Status { get; set; }
    public string BillingCycle { get; set; }
    
    public SubscriptionTierDto Tier { get; set; }
    
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public DateTime? TrialEndsAt { get; set; }
    public DateTime? NextBillingDate { get; set; }
    
    public decimal Price { get; set; }
    public string Currency { get; set; }
    public decimal DiscountPercent { get; set; }
    
    public bool AutoRenew { get; set; }
    public bool CanUpgrade { get; set; }
    public bool CanDowngrade { get; set; }
    public bool CanCancel { get; set; }
    
    public List<UserAddOnDto> AddOns { get; set; }
    public UsageDto CurrentUsage { get; set; }
}

public class UsageDto
{
    public int ProfilesUsed { get; set; }
    public int ProfilesLimit { get; set; }
    public int CatalogItemsUsed { get; set; }
    public int CatalogItemsLimit { get; set; }
    public int LocationsUsed { get; set; }
    public int LocationsLimit { get; set; }
    public int TeamMembersUsed { get; set; }
    public int TeamMembersLimit { get; set; }
    public int DocumentsUsed { get; set; }
    public int DocumentsLimit { get; set; }
    
    public bool IsAtProfileLimit { get; set; }
    public bool IsAtCatalogItemLimit { get; set; }
    public bool IsAtLocationLimit { get; set; }
}
```

### 5.3 CreateSubscriptionDto

```csharp
public class CreateSubscriptionDto
{
    [Required]
    public int TierId { get; set; }
    
    [Required]
    public BillingCycle BillingCycle { get; set; }
    
    public string PaymentMethodId { get; set; } // PayFast ID
    
    public string PromoCode { get; set; }
    
    public bool StartTrial { get; set; } = true;
}
```

### 5.4 UpgradeSubscriptionDto

```csharp
public class UpgradeSubscriptionDto
{
    [Required]
    public int NewTierId { get; set; }
    
    public bool ChangeImmediately { get; set; } = true;
    
    // If upgrading from monthly to annual
    public BillingCycle? NewBillingCycle { get; set; }
}
```

---

## 6. Feature Flag System

### 6.1 Feature Checking Service

```csharp
public interface IFeatureService
{
    Task<bool> HasFeatureAsync(int userId, string featureName);
    Task<bool> CanCreateProfileAsync(int userId);
    Task<bool> CanAddCatalogItemAsync(int userId, int catalogId);
    Task<bool> CanAddTeamMemberAsync(int userId, int organizationId);
    Task<FeatureLimits> GetUserLimitsAsync(int userId);
    Task<UsageStatistics> GetUserUsageAsync(int userId);
}

public class FeatureService : IFeatureService
{
    private readonly IUserSubscriptionRepository _subscriptionRepo;
    private readonly IProfileRepository _profileRepo;
    private readonly ICatalogRepository _catalogRepo;
    
    public async Task<bool> HasFeatureAsync(int userId, string featureName)
    {
        var subscriptions = await _subscriptionRepo
            .GetActiveSubscriptionsAsync(userId);
        
        // Check if any active subscription has this feature
        return subscriptions.Any(sub => 
            CheckFeatureFlag(sub.Tier, featureName));
    }
    
    public async Task<bool> CanCreateProfileAsync(int userId)
    {
        var currentCount = await _profileRepo
            .GetProfileCountAsync(userId);
        
        var limits = await GetUserLimitsAsync(userId);
        
        // Unlimited = -1
        if (limits.MaxProfiles == -1)
            return true;
            
        return currentCount < limits.MaxProfiles;
    }
    
    public async Task<bool> CanAddCatalogItemAsync(int userId, int catalogId)
    {
        var currentCount = await _catalogRepo
            .GetItemCountAsync(catalogId);
        
        var limits = await GetUserLimitsAsync(userId);
        
        if (limits.MaxCatalogItems == -1)
            return true;
            
        return currentCount < limits.MaxCatalogItems;
    }
    
    public async Task<FeatureLimits> GetUserLimitsAsync(int userId)
    {
        var subscriptions = await _subscriptionRepo
            .GetActiveSubscriptionsAsync(userId);
        
        // Combine limits from all active subscriptions
        // Take the maximum value for each limit
        var combinedLimits = new FeatureLimits
        {
            MaxProfiles = subscriptions.Max(s => 
                s.CustomMaxProfiles ?? s.Tier.MaxProfiles),
            MaxCatalogItems = subscriptions.Max(s => 
                s.CustomMaxCatalogItems ?? s.Tier.MaxCatalogItems),
            MaxLocations = subscriptions.Max(s => 
                s.CustomMaxLocations ?? s.Tier.MaxLocations),
            MaxTeamMembers = subscriptions.Max(s => 
                s.Tier.MaxTeamMembers),
            MaxDocuments = subscriptions.Max(s => 
                s.Tier.MaxDocuments)
        };
        
        return combinedLimits;
    }
    
    private bool CheckFeatureFlag(SubscriptionTier tier, string featureName)
    {
        return featureName switch
        {
            "CustomBranding" => tier.CustomBranding,
            "RemoveBranding" => tier.RemoveBranding,
            "Analytics" => tier.Analytics,
            "AdvancedAnalytics" => tier.AdvancedAnalytics,
            "ApiAccess" => tier.ApiAccess,
            "WhiteLabel" => tier.WhiteLabel,
            "ItemVariants" => tier.ItemVariants,
            "InventoryTracking" => tier.InventoryTracking,
            "TeamManagement" => tier.TeamManagement,
            _ => false
        };
    }
}
```

### 6.2 Feature Check Attribute

```csharp
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
public class RequireFeatureAttribute : Attribute, IAuthorizationFilter
{
    private readonly string _featureName;
    
    public RequireFeatureAttribute(string featureName)
    {
        _featureName = featureName;
    }
    
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var userId = context.HttpContext.User.GetUserId();
        if (userId == 0)
        {
            context.Result = new UnauthorizedResult();
            return;
        }
        
        var featureService = context.HttpContext
            .RequestServices
            .GetRequiredService<IFeatureService>();
        
        var hasFeature = featureService
            .HasFeatureAsync(userId, _featureName)
            .GetAwaiter()
            .GetResult();
        
        if (!hasFeature)
        {
            context.Result = new ForbidResult();
        }
    }
}

// Usage in controller:
[HttpPost]
[RequireFeature("CustomBranding")]
public async Task<IActionResult> UpdateBranding(...)
{
    // Only users with CustomBranding feature can access
}
```

### 6.3 Limit Check Helper

```csharp
public class SubscriptionLimitException : Exception
{
    public string LimitType { get; set; }
    public int CurrentValue { get; set; }
    public int LimitValue { get; set; }
    public string UpgradeMessage { get; set; }
    
    public SubscriptionLimitException(
        string limitType, 
        int currentValue, 
        int limitValue, 
        string upgradeMessage)
        : base($"Subscription limit reached for {limitType}")
    {
        LimitType = limitType;
        CurrentValue = currentValue;
        LimitValue = limitValue;
        UpgradeMessage = upgradeMessage;
    }
}

// Usage in service:
public async Task<Profile> CreateProfileAsync(int userId, ProfileRequestDto dto)
{
    var canCreate = await _featureService.CanCreateProfileAsync(userId);
    
    if (!canCreate)
    {
        var usage = await _featureService.GetUserUsageAsync(userId);
        
        throw new SubscriptionLimitException(
            "Profiles",
            usage.ProfilesUsed,
            usage.ProfilesLimit,
            "Upgrade to Pro for 5 profiles or Business for 20 profiles"
        );
    }
    
    // Continue with profile creation
}
```

---

## 7. Business Logic

### 7.1 Subscription Service

```csharp
public class SubscriptionService : ISubscriptionService
{
    private readonly IUserSubscriptionRepository _subscriptionRepo;
    private readonly ISubscriptionTierRepository _tierRepo;
    private readonly IPaymentService _paymentService;
    private readonly IEmailService _emailService;
    
    public async Task<UserSubscription> CreateSubscriptionAsync(
        int userId,
        CreateSubscriptionDto dto)
    {
        var tier = await _tierRepo.GetByIdAsync(dto.TierId);
        if (tier == null)
            throw new NotFoundException("Subscription tier not found");
        
        // Check if user already has subscription for this product line
        var existingSub = await _subscriptionRepo
            .GetActiveSubscriptionByProductLineAsync(userId, tier.ProductLine);
        
        if (existingSub != null)
            throw new BusinessException(
                $"You already have an active {tier.ProductLine} subscription");
        
        var subscription = new UserSubscription
        {
            UserId = userId,
            TierId = dto.TierId,
            Status = dto.StartTrial ? 
                SubscriptionStatus.Trial : 
                SubscriptionStatus.Active,
            BillingCycle = dto.BillingCycle,
            StartDate = DateTime.UtcNow,
            Price = dto.BillingCycle == BillingCycle.Monthly ?
                tier.MonthlyPrice : tier.AnnualPrice,
            Currency = "ZAR"
        };
        
        if (dto.StartTrial)
        {
            subscription.TrialEndsAt = DateTime.UtcNow.AddDays(14);
            subscription.NextBillingDate = subscription.TrialEndsAt;
        }
        else
        {
            // Process payment immediately
            var payment = await _paymentService.ProcessPaymentAsync(
                userId,
                subscription.Price,
                dto.PaymentMethodId);
            
            subscription.Status = SubscriptionStatus.Active;
            subscription.LastPaymentDate = DateTime.UtcNow;
            subscription.LastPaymentAmount = subscription.Price;
            subscription.NextBillingDate = CalculateNextBillingDate(
                DateTime.UtcNow,
                dto.BillingCycle);
        }
        
        await _subscriptionRepo.AddAsync(subscription);
        await _subscriptionRepo.SaveChangesAsync();
        
        // Send confirmation email
        await _emailService.SendSubscriptionConfirmationAsync(
            userId,
            subscription);
        
        return subscription;
    }
    
    public async Task<UserSubscription> UpgradeSubscriptionAsync(
        int userId,
        int subscriptionId,
        UpgradeSubscriptionDto dto)
    {
        var subscription = await _subscriptionRepo
            .GetByIdAsync(subscriptionId);
        
        if (subscription.UserId != userId)
            throw new ForbiddenException();
        
        var newTier = await _tierRepo.GetByIdAsync(dto.NewTierId);
        
        // Validate it's an upgrade (higher price)
        var currentMonthlyPrice = subscription.BillingCycle == BillingCycle.Monthly ?
            subscription.Price :
            subscription.Price / 12;
        
        if (newTier.MonthlyPrice <= currentMonthlyPrice)
            throw new BusinessException("New tier must be higher than current tier");
        
        // Calculate prorated amount
        var proratedAmount = CalculateProratedAmount(
            subscription,
            newTier,
            dto.NewBillingCycle ?? subscription.BillingCycle);
        
        // Process payment for difference
        if (proratedAmount > 0)
        {
            await _paymentService.ProcessPaymentAsync(
                userId,
                proratedAmount,
                null); // Use default payment method
        }
        
        // Update subscription
        subscription.TierId = dto.NewTierId;
        subscription.Price = dto.NewBillingCycle == BillingCycle.Annual ?
            newTier.AnnualPrice : newTier.MonthlyPrice;
        subscription.BillingCycle = dto.NewBillingCycle ?? subscription.BillingCycle;
        subscription.UpdatedAt = DateTime.UtcNow;
        
        await _subscriptionRepo.UpdateAsync(subscription);
        
        // Log history
        await LogSubscriptionHistoryAsync(
            userId,
            subscription.Id,
            SubscriptionAction.Upgraded,
            subscription.TierId,
            dto.NewTierId);
        
        await _emailService.SendSubscriptionUpgradedAsync(userId, subscription);
        
        return subscription;
    }
    
    private DateTime CalculateNextBillingDate(
        DateTime startDate,
        BillingCycle cycle)
    {
        return cycle == BillingCycle.Monthly ?
            startDate.AddMonths(1) :
            startDate.AddYears(1);
    }
    
    private decimal CalculateProratedAmount(
        UserSubscription currentSub,
        SubscriptionTier newTier,
        BillingCycle newCycle)
    {
        // Calculate days remaining in current cycle
        var daysRemaining = (currentSub.NextBillingDate.Value - DateTime.UtcNow).Days;
        var totalDaysInCycle = currentSub.BillingCycle == BillingCycle.Monthly ? 30 : 365;
        
        // Credit for unused days
        var credit = (currentSub.Price / totalDaysInCycle) * daysRemaining;
        
        // Cost of new tier
        var newCost = newCycle == BillingCycle.Monthly ?
            newTier.MonthlyPrice :
            newTier.AnnualPrice;
        
        // Difference (prorated)
        return Math.Max(0, newCost - credit);
    }
}
```

---

## 8. Migration Strategy

### 8.1 Seed Data for Subscription Tiers

```csharp
public class SeedSubscriptionTiers
{
    public static void Seed(ModelBuilder modelBuilder)
    {
        // PROFESSIONAL TIERS
        modelBuilder.Entity<SubscriptionTier>().HasData(
            // Free
            new SubscriptionTier
            {
                Id = 1,
                ProductLine = ProductLine.Professional,
                TierName = "Free",
                TierCode = "PROF_FREE",
                DisplayName = "Free",
                Description = "Perfect for trying out BizBio",
                MonthlyPrice = 0,
                AnnualPrice = 0,
                MaxProfiles = 1,
                MaxCatalogItems = 0,
                MaxLocations = 0,
                MaxTeamMembers = 0,
                MaxDocuments = 0,
                CustomBranding = false,
                RemoveBranding = false,
                Analytics = false,
                DisplayOrder = 1,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            },
            
            // Pro
            new SubscriptionTier
            {
                Id = 2,
                ProductLine = ProductLine.Professional,
                TierName = "Pro",
                TierCode = "PROF_PRO",
                DisplayName = "Pro",
                Description = "For individual professionals",
                MonthlyPrice = 99,
                AnnualPrice = 999,
                MaxProfiles = 5,
                MaxDocuments = 10,
                MaxDocumentSizeMB = 5,
                CustomBranding = true,
                RemoveBranding = true,
                Analytics = true,
                PrioritySupport = true,
                DisplayOrder = 2,
                IsActive = true,
                IsPopular = true,
                RecommendedFor = "Consultants, Freelancers, Professionals",
                CreatedAt = DateTime.UtcNow
            },
            
            // Business
            new SubscriptionTier
            {
                Id = 3,
                ProductLine = ProductLine.Professional,
                TierName = "Business",
                TierCode = "PROF_BUSINESS",
                DisplayName = "Business",
                Description = "For teams and growing businesses",
                MonthlyPrice = 249,
                AnnualPrice = 2490,
                MaxProfiles = 20,
                MaxTeamMembers = 20,
                MaxDocuments = 50,
                MaxDocumentSizeMB = 10,
                CustomBranding = true,
                RemoveBranding = true,
                Analytics = true,
                AdvancedAnalytics = true,
                CustomSubdomain = true,
                PrioritySupport = true,
                TeamManagement = true,
                OrganizationalHierarchy = true,
                DisplayOrder = 3,
                IsActive = true,
                RecommendedFor = "Small to medium businesses",
                CreatedAt = DateTime.UtcNow
            },
            
            // Enterprise
            new SubscriptionTier
            {
                Id = 4,
                ProductLine = ProductLine.Professional,
                TierName = "Enterprise",
                TierCode = "PROF_ENTERPRISE",
                DisplayName = "Enterprise",
                Description = "For large organizations",
                MonthlyPrice = 799,
                AnnualPrice = 7990,
                MaxProfiles = -1, // Unlimited
                MaxTeamMembers = -1,
                MaxDocuments = -1,
                MaxDocumentSizeMB = 25,
                CustomBranding = true,
                RemoveBranding = true,
                Analytics = true,
                AdvancedAnalytics = true,
                ApiAccess = true,
                WhiteLabel = true,
                CustomDomain = true,
                PrioritySupport = true,
                PhoneSupport = true,
                DedicatedManager = true,
                TeamManagement = true,
                OrganizationalHierarchy = true,
                SSOIntegration = true,
                DisplayOrder = 4,
                IsActive = true,
                RecommendedFor = "Large enterprises, Franchises",
                CreatedAt = DateTime.UtcNow
            }
        );
        
        // MENU TIERS
        modelBuilder.Entity<SubscriptionTier>().HasData(
            // Starter
            new SubscriptionTier
            {
                Id = 5,
                ProductLine = ProductLine.Menu,
                TierName = "Starter",
                TierCode = "MENU_STARTER",
                DisplayName = "Starter",
                Description = "For single-location restaurants",
                MonthlyPrice = 149,
                AnnualPrice = 1490,
                MaxProfiles = 1,
                MaxCatalogItems = 50,
                MaxLocations = 1,
                MaxImagesPerItem = 5,
                DisplayOrder = 1,
                IsActive = true,
                RecommendedFor = "Food trucks, Small cafes",
                CreatedAt = DateTime.UtcNow
            },
            
            // Restaurant
            new SubscriptionTier
            {
                Id = 6,
                ProductLine = ProductLine.Menu,
                TierName = "Restaurant",
                TierCode = "MENU_RESTAURANT",
                DisplayName = "Restaurant",
                Description = "For established restaurants",
                MonthlyPrice = 299,
                AnnualPrice = 2990,
                MaxProfiles = 3,
                MaxCatalogItems = 200,
                MaxLocations = 3,
                MaxImagesPerItem = -1, // Unlimited
                CustomBranding = true,
                RemoveBranding = true,
                Analytics = true,
                ItemVariants = true,
                ItemAddons = true,
                AllergenInfo = true,
                DietaryTags = true,
                MenuScheduling = true,
                PrioritySupport = true,
                DisplayOrder = 2,
                IsActive = true,
                IsPopular = true,
                RecommendedFor = "Full-service restaurants",
                CreatedAt = DateTime.UtcNow
            },
            
            // Multi-Location
            new SubscriptionTier
            {
                Id = 7,
                ProductLine = ProductLine.Menu,
                TierName = "Multi-Location",
                TierCode = "MENU_MULTI",
                DisplayName = "Multi-Location",
                Description = "For restaurant groups",
                MonthlyPrice = 599,
                AnnualPrice = 5990,
                MaxProfiles = -1,
                MaxCatalogItems = 500,
                MaxLocations = -1,
                MaxImagesPerItem = -1,
                CustomBranding = true,
                RemoveBranding = true,
                Analytics = true,
                AdvancedAnalytics = true,
                CustomSubdomain = true,
                ItemVariants = true,
                ItemAddons = true,
                AllergenInfo = true,
                DietaryTags = true,
                MenuScheduling = true,
                MultiLanguage = true,
                NutritionalInfo = true,
                PrioritySupport = true,
                DisplayOrder = 3,
                IsActive = true,
                RecommendedFor = "Restaurant chains (3-10 locations)",
                CreatedAt = DateTime.UtcNow
            },
            
            // Chain
            new SubscriptionTier
            {
                Id = 8,
                ProductLine = ProductLine.Menu,
                TierName = "Chain",
                TierCode = "MENU_CHAIN",
                DisplayName = "Chain",
                Description = "For large restaurant chains",
                MonthlyPrice = 1499,
                AnnualPrice = 14990,
                MaxProfiles = -1,
                MaxCatalogItems = -1,
                MaxLocations = -1,
                MaxImagesPerItem = -1,
                CustomBranding = true,
                RemoveBranding = true,
                Analytics = true,
                AdvancedAnalytics = true,
                ApiAccess = true,
                WhiteLabel = true,
                CustomDomain = true,
                ItemVariants = true,
                ItemAddons = true,
                AllergenInfo = true,
                DietaryTags = true,
                MenuScheduling = true,
                MultiLanguage = true,
                NutritionalInfo = true,
                PrioritySupport = true,
                PhoneSupport = true,
                DedicatedManager = true,
                DisplayOrder = 4,
                IsActive = true,
                RecommendedFor = "National franchises",
                CreatedAt = DateTime.UtcNow
            }
        );
        
        // RETAIL TIERS (similar pattern)
        // ... (continue with Retail tiers)
    }
}
```

### 8.2 Migration Commands

```bash
# Create migration
dotnet ef migrations add AddVerticalSubscriptionTiers --project BizBio.Infrastructure

# Review migration SQL
dotnet ef migrations script --project BizBio.Infrastructure

# Apply migration
dotnet ef database update --project BizBio.Infrastructure

# Seed data
dotnet run --project BizBio.API -- seed-subscriptions
```

---

## 9. Testing Requirements

### 9.1 Unit Tests

```csharp
public class FeatureServiceTests
{
    [Fact]
    public async Task CanCreateProfile_WhenUnderLimit_ReturnsTrue()
    {
        // Arrange
        var userId = 1;
        var currentCount = 2;
        var maxProfiles = 5;
        
        // Mock repositories
        var profileRepo = new Mock<IProfileRepository>();
        profileRepo.Setup(r => r.GetProfileCountAsync(userId))
            .ReturnsAsync(currentCount);
        
        var subscriptionRepo = new Mock<IUserSubscriptionRepository>();
        subscriptionRepo.Setup(r => r.GetActiveSubscriptionsAsync(userId))
            .ReturnsAsync(new List<UserSubscription>
            {
                new UserSubscription
                {
                    Tier = new SubscriptionTier { MaxProfiles = maxProfiles }
                }
            });
        
        var service = new FeatureService(subscriptionRepo.Object, profileRepo.Object);
        
        // Act
        var result = await service.CanCreateProfileAsync(userId);
        
        // Assert
        Assert.True(result);
    }
    
    [Fact]
    public async Task CanCreateProfile_WhenAtLimit_ReturnsFalse()
    {
        // Similar test for at limit scenario
    }
}
```

### 9.2 Integration Tests

```csharp
public class SubscriptionApiTests : IClassFixture<WebApplicationFactory<Program>>
{
    [Fact]
    public async Task GetSubscriptionTiers_ReturnsAllActiveTiers()
    {
        // Arrange
        var client = _factory.CreateClient();
        
        // Act
        var response = await client.GetAsync("/api/v1/subscriptions/tiers");
        
        // Assert
        response.EnsureSuccessStatusCode();
        var tiers = await response.Content
            .ReadAsAsync<List<SubscriptionTierDto>>();
        
        Assert.NotEmpty(tiers);
    }
    
    [Fact]
    public async Task CreateSubscription_WithValidData_ReturnsCreated()
    {
        // Test subscription creation
    }
}
```

---

## 10. Implementation Guide

### 10.1 Phase 1: Database (Week 1)

1. Create migration for new tables
2. Seed subscription tiers data
3. Test database schema
4. Create indexes for performance

### 10.2 Phase 2: Backend API (Week 2-3)

1. Implement entity models
2. Create repositories
3. Build subscription service
4. Implement feature service
5. Create API controllers
6. Add authorization filters
7. Write unit tests

### 10.3 Phase 3: Frontend Integration (Week 4)

1. Create pricing pages
2. Build subscription dashboard
3. Implement feature checks in UI
4. Add usage indicators
5. Create upgrade flows

### 10.4 Phase 4: Payment Integration (Week 5)

1. Update PayFast integration
2. Handle subscription webhooks
3. Implement prorated billing
4. Add invoice generation

### 10.5 Phase 5: Testing & Launch (Week 6)

1. End-to-end testing
2. User acceptance testing
3. Migration of existing users
4. Soft launch to beta users
5. Full production launch

---

**End of Subscription System Technical Specification**

**Version:** 2.1  
**Date:** November 2025  
**Status:** Ready for Implementation  
**Estimated Development Time:** 6 weeks
