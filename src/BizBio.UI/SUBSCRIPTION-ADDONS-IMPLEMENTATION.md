# SUBSCRIPTION ADDONS & PER-PRODUCT TRIALS - IMPLEMENTATION SUMMARY

## 📋 OVERVIEW

This document summarizes the database schema updates and backend implementation for:
1. **Subscription Addons System** - Structured addon management (Waiter Call, Order Sending, Menu Pro, etc.)
2. **Per-Product Trials** - Independent 14-day trials for each product line (Menu, Card, Retail)

---

## 🎯 PROFILE & CATALOG ARCHITECTURE CLARIFICATION

### **What is Profile?**
`Profile` is the **public-facing entity** that represents what the world sees:
- Think of it as the "storefront" or "landing page"
- Accessed via unique slug: `bizbio.com/lapineta` or `bizbio.com/john-smith`
- Contains public metadata: Name, Logo, Contact Info, ProfileType
- **One User can have multiple Profiles** (Personal, Business, Restaurant, etc.)

### **What is Catalog?**
`Catalog` is the **versioned content container** within a Profile:
- A Profile can have multiple Catalogs (Breakfast Menu, Dinner Menu, Winter Collection, etc.)
- Contains the actual content: Items, Categories, Bundles
- Supports versioning via `CatalogVersion` for time-based menus/catalogs

### **Architecture Flow:**
```
User (Authentication & Billing Layer)
  ├── UserSubscription (Menu Product Trial + Active)
  ├── UserSubscription (Card Product Trial)
  └── Profile ("La Pineta Restaurant")
      ├── Catalog ("Breakfast Menu")
      │   └── CatalogItems, Categories, Bundles
      ├── Catalog ("Dinner Menu")
      │   └── CatalogItems, Categories, Bundles
      └── Catalog ("Catering Menu")
          └── CatalogItems, Categories, Bundles
```

### **Why this works for all products:**
- **Menu Product**: Profile = Restaurant, Catalog = Menu (Breakfast/Lunch/Dinner/Event-specific)
- **Card Product**: Profile = Person/Company, Catalog = Business Card / Resume / Portfolio
- **Retail Product**: Profile = Store, Catalog = Product Catalog (Summer/Winter/Sale collections)

---

## 🗄️ DATABASE SCHEMA CHANGES

### **NEW ENTITIES CREATED**

#### 1. `SubscriptionAddon`
Represents an addon that can be purchased with subscription tiers.

**Columns:**
- `Id` (PK)
- `Name` VARCHAR(100) - "Waiter Call", "Order Sending", "Menu Pro"
- `Description` VARCHAR(2000)
- `ProductLineId` INT FK → ProductLineLookup
- `MonthlyPrice` DECIMAL(10,2)
- `ConfigurationJson` VARCHAR(4000) - Addon-specific settings
- `SortOrder` INT
- `IsActive`, `CreatedAt`, `UpdatedAt`, etc. (from BaseEntity)

**Purpose:** Master list of available addons per product line

---

#### 2. `SubscriptionTierAddon`
Junction table defining which addons are available for which tiers.

**Columns:**
- `Id` (PK)
- `SubscriptionTierId` INT FK → SubscriptionTier
- `AddonId` INT FK → SubscriptionAddon
- `IsRequired` BOOLEAN - Rarely used, for mandatory tier addons
- `DiscountPercentage` DECIMAL(5,2) - Tier-specific addon discount
- `IsActive`, `CreatedAt`, `UpdatedAt`, etc.

**Purpose:** 
- Controls addon availability (e.g., Menu Pro only for Entree & Banquet, not Starter)
- Allows tier-specific pricing (e.g., Banquet gets 20% off Menu Pro)

**Unique Index:** `(SubscriptionTierId, AddonId)`

---

#### 3. `UserSubscriptionAddon`
Tracks which addons a user has activated on their subscription.

**Columns:**
- `Id` (PK)
- `UserSubscriptionId` INT FK → UserSubscription
- `AddonId` INT FK → SubscriptionAddon
- `AddedAt` DATETIME
- `CancelledAt` DATETIME (nullable)
- `IsActiveAddon` BOOLEAN
- `MonthlyPrice` DECIMAL(10,2) - Historical price (may differ from current price)
- `IsActive`, `CreatedAt`, `UpdatedAt`, etc.

**Purpose:** 
- Active addons for a user's subscription
- Historical pricing for billing accuracy
- Cancellation tracking

**Indexes:**
- `UserSubscriptionId`
- `AddonId`
- `(UserSubscriptionId, AddonId, IsActiveAddon)` - Composite for quick lookups

---

### **MODIFIED ENTITIES**

#### `UserSubscription` - Added ProductLineId
**NEW Column:**
- `ProductLineId` INT FK → ProductLineLookup

**Purpose:**
- Enables per-product trial tracking
- John can have:
  - UserSubscription #1: ProductLineId=1 (Menu), TrialEndsAt="2025-01-07"
  - UserSubscription #2: ProductLineId=2 (Card), TrialEndsAt="2025-01-14"

**NEW Navigation Property:**
```csharp
public virtual ProductLineLookup ProductLine { get; set; } = null!;
public virtual ICollection<UserSubscriptionAddon> Addons { get; set; } = new List<UserSubscriptionAddon>();
```

---

#### `SubscriptionTier` - Added TierAddons Navigation
**NEW Navigation Property:**
```csharp
public virtual ICollection<SubscriptionTierAddon> TierAddons { get; set; } = new List<SubscriptionTierAddon>();
```

---

## 🔗 ENTITY RELATIONSHIPS

```
ProductLineLookup (Menu, Card, Retail)
  ├── SubscriptionAddon (many addons per product)
  │   ├── SubscriptionTierAddon (which tiers can use this addon)
  │   └── UserSubscriptionAddon (which users have this addon active)
  └── UserSubscription (one subscription per user per product)
      └── UserSubscriptionAddon (user's active addons)

SubscriptionTier
  ├── SubscriptionTierAddon (available addons for this tier)
  └── UserSubscription
      └── UserSubscriptionAddon
```

---

## 🔨 IMPLEMENTATION FILES CREATED

### 1. **Core Entities** (`src/BizBio.Core/Entities/`)
- ✅ `SubscriptionAddon.cs`
- ✅ `SubscriptionTierAddon.cs`
- ✅ `UserSubscriptionAddon.cs`

### 2. **DbContext Updates** (`src/BizBio.Infrastructure/Data/ApplicationDbContext.cs`)
- ✅ Added `DbSet` properties for new entities
- ✅ Configured entity relationships in `OnModelCreating`:
  - SubscriptionAddon configuration with indexes
  - SubscriptionTierAddon with unique composite index
  - UserSubscriptionAddon with multiple indexes for performance
  - Updated UserSubscription to include ProductLine FK

---

## 📝 NEXT STEPS (TO COMPLETE AFTER STOPPING APP)

### 1. **Create Migration**
```bash
cd src/BizBio.Infrastructure
dotnet ef migrations add AddSubscriptionAddonsAndPerProductTrials --project . --startup-project ../BizBio.API/BizBio.API.csproj
```

### 2. **Apply Migration**
```bash
dotnet ef database update --project . --startup-project ../BizBio.API/BizBio.API.csproj
```

### 3. **Seed Initial Data** (Recommended)
Create seed data for Menu product addons:
```sql
-- Seed Menu Product Addons
INSERT INTO SubscriptionAddons (Name, Description, ProductLineId, MonthlyPrice, SortOrder, IsActive, CreatedAt, UpdatedAt, CreatedBy, UpdatedBy, IsValid)
VALUES 
('Waiter Call', 'Enable table-side waiter call buttons for your digital menu', 1, 49.00, 1, 1, NOW(), NOW(), 'system', 'system', 1),
('Order Sending', 'Allow customers to send orders directly to WhatsApp', 1, 79.00, 2, 1, NOW(), NOW(), 'system', 'system', 1),
('Menu Pro', 'Advanced menu customization and analytics features', 1, 149.00, 3, 1, NOW(), NOW(), 'system', 'system', 1);

-- Link addons to tiers (assuming Entree=TierId:2, Banquet=TierId:3)
INSERT INTO SubscriptionTierAddons (SubscriptionTierId, AddonId, IsRequired, DiscountPercentage, IsActive, CreatedAt, UpdatedAt, CreatedBy, UpdatedBy, IsValid)
SELECT 2, Id, 0, NULL, 1, NOW(), NOW(), 'system', 'system', 1 FROM SubscriptionAddons WHERE ProductLineId = 1;

INSERT INTO SubscriptionTierAddons (SubscriptionTierId, AddonId, IsRequired, DiscountPercentage, IsActive, CreatedAt, UpdatedAt, CreatedBy, UpdatedBy, IsValid)
SELECT 3, Id, 0, 20.00, 1, NOW(), NOW(), 'system', 'system', 1 FROM SubscriptionAddons WHERE ProductLineId = 1;
-- Banquet tier gets 20% discount on all addons
```

### 4. **Create Backend Service** (`src/BizBio.Infrastructure/Services/SubscriptionAddonService.cs`)
```csharp
public interface ISubscriptionAddonService
{
    Task<List<SubscriptionAddon>> GetAvailableAddonsForTier(int tierId);
    Task<bool> AddAddonToSubscription(int userSubscriptionId, int addonId);
    Task<bool> RemoveAddonFromSubscription(int userSubscriptionAddonId);
    Task<decimal> CalculateSubscriptionTotalWithAddons(int userSubscriptionId);
    Task<bool> CanUserAccessAddon(int userId, int addonId);
}
```

### 5. **Create API Controller** (`src/BizBio.API/Controllers/SubscriptionAddonsController.cs`)
```csharp
[ApiController]
[Route("api/v1/subscription-addons")]
[Authorize]
public class SubscriptionAddonsController : ControllerBase
{
    [HttpGet("available/{tierId}")]
    public async Task<IActionResult> GetAvailableAddons(int tierId) { }

    [HttpPost("activate")]
    public async Task<IActionResult> ActivateAddon([FromBody] ActivateAddonDto dto) { }

    [HttpDelete("{userSubscriptionAddonId}")]
    public async Task<IActionResult> DeactivateAddon(int userSubscriptionAddonId) { }

    [HttpGet("my-addons")]
    public async Task<IActionResult> GetMyActiveAddons() { }
}
```

### 6. **Update Subscription Logic**
Modify subscription creation to:
- Check if user already has an active subscription for this product line
- If not, create with 14-day trial
- If yes (different product), create new subscription with separate 14-day trial

---

## 💡 USAGE EXAMPLES

### **Scenario 1: John signs up for Menu product**
```
1. User registers → UserId=123
2. Selects "Menu - Starter Plan" → Creates Profile with ProfileType="Menu"
3. System creates UserSubscription:
   - UserId=123
   - TierId=1 (Starter)
   - ProductLineId=1 (Menu)
   - TrialEndsAt=NOW() + 14 days
   - Status="Trial"
```

### **Scenario 2: John upgrades to Entree and adds addons**
```
1. User upgrades → Update UserSubscription TierId=2 (Entree)
2. Available addons shown: Waiter Call (R49), Order Sending (R79), Menu Pro (R149)
3. User activates "Order Sending"
4. System creates UserSubscriptionAddon:
   - UserSubscriptionId=1
   - AddonId=2 (Order Sending)
   - MonthlyPrice=79.00
   - IsActiveAddon=true
5. Monthly bill = R299 (Entree) + R79 (Order Sending) = R378
```

### **Scenario 3: Week later, John wants business card**
```
1. User navigates to "Add Product" → Selects "Card - Professional"
2. System creates NEW UserSubscription:
   - UserId=123
   - TierId=4 (Card Professional tier)
   - ProductLineId=2 (Card)
   - TrialEndsAt=NOW() + 14 days (NEW TRIAL!)
   - Status="Trial"
3. John now has:
   - Menu subscription (active, paying)
   - Card subscription (trial, 14 days free)
```

---

## ✅ BENEFITS OF THIS IMPLEMENTATION

### **1. Per-Product Trials**
- ✅ Each product gets its own 14-day trial period
- ✅ Users can trial Menu, then Card, then Retail independently
- ✅ Simple query: `SELECT * FROM UserSubscriptions WHERE UserId=X AND ProductLineId=Y`

### **2. Structured Addons**
- ✅ No more JSON blob for addons
- ✅ Queryable: "Show me all users with Waiter Call addon"
- ✅ Historical pricing preserved
- ✅ Tier-specific discounts supported
- ✅ Easy to add/remove addons dynamically

### **3. Flexible & Scalable**
- ✅ Add new addons without schema changes
- ✅ Different addons for different products (Card addons, Retail addons)
- ✅ Tier-based availability control
- ✅ Promotional pricing per tier

---

## 🔍 QUERIES YOU CAN NOW RUN

```sql
-- Get all users with "Waiter Call" addon
SELECT u.Email, usa.AddedAt, usa.MonthlyPrice
FROM UserSubscriptionAddons usa
JOIN UserSubscriptions us ON usa.UserSubscriptionId = us.Id
JOIN Users u ON us.UserId = u.Id
JOIN SubscriptionAddons sa ON usa.AddonId = sa.Id
WHERE sa.Name = 'Waiter Call' AND usa.IsActiveAddon = 1;

-- Get user's total monthly bill including addons
SELECT 
    us.UserId,
    st.MonthlyPrice AS BasePlan,
    COALESCE(SUM(usa.MonthlyPrice), 0) AS AddonsTotal,
    st.MonthlyPrice + COALESCE(SUM(usa.MonthlyPrice), 0) AS TotalMonthly
FROM UserSubscriptions us
JOIN SubscriptionTiers st ON us.TierId = st.Id
LEFT JOIN UserSubscriptionAddons usa ON us.Id = usa.UserSubscriptionId AND usa.IsActiveAddon = 1
WHERE us.UserId = 123
GROUP BY us.Id;

-- Find users in trial for Menu product
SELECT u.Email, us.TrialEndsAt, DATEDIFF(us.TrialEndsAt, NOW()) AS DaysRemaining
FROM UserSubscriptions us
JOIN Users u ON us.UserId = u.Id
WHERE us.ProductLineId = 1 
  AND us.TrialEndsAt > NOW()
  AND us.StatusId = (SELECT Id FROM SubscriptionStatusLookup WHERE Name = 'Trial');
```

---

## 📊 VALIDATION RULES TO IMPLEMENT

### **Addon Activation:**
1. ✅ Check user has active subscription
2. ✅ Check addon is available for user's tier (`SubscriptionTierAddon` exists)
3. ✅ Check user doesn't already have addon active
4. ✅ Calculate price (with tier discount if applicable)
5. ✅ Create `UserSubscriptionAddon` record

### **Trial Management:**
1. ✅ Check if user already has subscription for this product line
2. ✅ If yes, don't grant trial
3. ✅ If no, create with `TrialEndsAt = NOW() + INTERVAL 14 DAY`
4. ✅ Set status to "Trial"

---

## 🚀 DEPLOYMENT CHECKLIST

- [ ] Stop running application
- [ ] Create migration
- [ ] Review migration SQL
- [ ] Apply migration to development database
- [ ] Seed initial addon data
- [ ] Create SubscriptionAddonService
- [ ] Create API endpoints
- [ ] Add validation logic
- [ ] Update frontend to display available addons
- [ ] Test trial creation for multiple products
- [ ] Test addon activation/deactivation
- [ ] Deploy to production

---

## 📚 ADDITIONAL NOTES

- **Catalog Naming**: Keep "Catalog" - it works well conceptually as a "container" for all product types
- **Profile Purpose**: Think of it as the "public presence" or "brand" that can contain multiple catalogs
- **Future Card/Retail**: When implementing, just create appropriate catalogs under profiles with Card/Retail-specific items

