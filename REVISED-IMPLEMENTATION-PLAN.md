# BizBio Multi-Product Subscription Architecture - Implementation Plan

## Overview
Transform BizBio from single-subscription to multi-product subscription model with unified entity management.

## Core Architectural Principles

### 1. Products
Three distinct subscription products:
- **Cards** - Digital business card profiles
- **Menu** - Restaurant/venue menu management
- **Catalog** - Product catalog for stores/businesses

Each product has own subscription tier with independent trial periods, billed together monthly.

### 2. Entity Unification
Single `Entities` table replaces separate Restaurant/Company/Profile tables:
- Entity represents the business/organization/venue
- EntityType enum distinguishes Restaurant/Store/Venue/Organization
- Each entity can have multiple catalogs/menus (per tier limits)

### 3. Library/Menu/Catalog Terminology
**Important**: Library, Menu, and Catalog are the **same thing** - just different UI terminology:
- "Library" = user's private items (IsActive=false or no catalog assignment)
- "Menu" = published catalog for restaurants (IsActive=true)
- "Catalog" = published catalog for stores (IsActive=true)

All stored in single `Catalogs` table. No CatalogType needed.

### 4. Price Override Pattern
Menu items can be shared across catalogs with price overrides:
- CatalogItem has `ParentCatalogItemId` (nullable)
- If null = master/template item
- If set = reference to master with optional price override
- `PriceOverride` (nullable decimal) - null means use parent price

---

## Database Schema

### New Tables

#### Entities Table
Replaces separate Restaurant/Company/Profile tables.

```sql
CREATE TABLE Entities (
    Id INT PRIMARY KEY IDENTITY,
    UserId INT NOT NULL FOREIGN KEY REFERENCES Users(Id),
    EntityType INT NOT NULL,  -- 0=Restaurant, 1=Store, 2=Venue, 3=Organization
    Name NVARCHAR(255) NOT NULL,
    Slug NVARCHAR(255) NOT NULL UNIQUE,
    Description NVARCHAR(2000),
    Logo NVARCHAR(500),

    -- Contact Info
    Address NVARCHAR(255),
    City NVARCHAR(100),
    PostalCode NVARCHAR(20),
    Phone NVARCHAR(20),
    Email NVARCHAR(255),
    Website NVARCHAR(500),

    -- Settings
    Currency NVARCHAR(3) DEFAULT 'ZAR',
    Timezone NVARCHAR(50) DEFAULT 'Africa/Johannesburg',

    -- Metadata
    SortOrder INT DEFAULT 0,
    IsActive BIT DEFAULT 1,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CreatedBy NVARCHAR(50),
    UpdatedBy NVARCHAR(50),

    INDEX IX_Entities_UserId (UserId),
    INDEX IX_Entities_Slug (Slug),
    INDEX IX_Entities_EntityType (EntityType)
);
```

#### ProductSubscriptions Table
Replaces single Subscriptions table with per-product subscriptions.

```sql
CREATE TABLE ProductSubscriptions (
    Id INT PRIMARY KEY IDENTITY,
    UserId INT NOT NULL FOREIGN KEY REFERENCES Users(Id),
    ProductType INT NOT NULL,  -- 0=Cards, 1=Menu, 2=Catalog
    TierId INT NOT NULL FOREIGN KEY REFERENCES SubscriptionTiers(Id),

    -- Trial tracking per product
    IsTrialActive BIT DEFAULT 1,
    TrialStartDate DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    TrialEndDate DATETIME2 NOT NULL,
    TrialDaysRemaining AS DATEDIFF(DAY, GETUTCDATE(), TrialEndDate),

    -- Billing
    Status INT NOT NULL DEFAULT 0,  -- 0=Trial, 1=Active, 2=Cancelled, 3=Expired
    BillingCycle INT DEFAULT 0,  -- 0=Monthly, 1=Yearly
    CurrentPeriodStart DATETIME2,
    CurrentPeriodEnd DATETIME2,
    NextBillingDate DATETIME2,

    -- Metadata
    IsActive BIT DEFAULT 1,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CancelledAt DATETIME2,

    UNIQUE (UserId, ProductType),  -- One subscription per product per user
    INDEX IX_ProductSubscriptions_UserId (UserId),
    INDEX IX_ProductSubscriptions_Status (Status)
);
```

#### CatalogCategories Table
Junction table linking Categories to Catalogs (many-to-many).

```sql
CREATE TABLE CatalogCategories (
    Id INT PRIMARY KEY IDENTITY,
    CatalogId INT NOT NULL FOREIGN KEY REFERENCES Catalogs(Id) ON DELETE CASCADE,
    CategoryId INT NOT NULL FOREIGN KEY REFERENCES Categories(Id) ON DELETE CASCADE,
    SortOrder INT DEFAULT 0,

    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),

    UNIQUE (CatalogId, CategoryId),
    INDEX IX_CatalogCategories_CatalogId (CatalogId),
    INDEX IX_CatalogCategories_CategoryId (CategoryId)
);
```

### Updated Tables

#### Catalogs Table
Add EntityId reference, remove ProfileId.

```sql
ALTER TABLE Catalogs ADD EntityId INT NULL
    FOREIGN KEY REFERENCES Entities(Id) ON DELETE CASCADE;

-- Migration: Create default entity for each profile, update catalogs
-- (See migration section below)

ALTER TABLE Catalogs ALTER COLUMN EntityId INT NOT NULL;
ALTER TABLE Catalogs DROP CONSTRAINT FK_Catalogs_Profiles;
ALTER TABLE Catalogs DROP COLUMN ProfileId;

-- Add index
CREATE INDEX IX_Catalogs_EntityId ON Catalogs(EntityId);
```

#### CatalogItems Table
Add ParentCatalogItemId and PriceOverride for item sharing.

```sql
ALTER TABLE CatalogItems ADD ParentCatalogItemId INT NULL
    FOREIGN KEY REFERENCES CatalogItems(Id);
ALTER TABLE CatalogItems ADD PriceOverride DECIMAL(18,2) NULL;

CREATE INDEX IX_CatalogItems_ParentCatalogItemId ON CatalogItems(ParentCatalogItemId);
```

**Price Resolution Logic:**
```csharp
public decimal EffectivePrice
{
    get
    {
        if (PriceOverride.HasValue) return PriceOverride.Value;
        if (ParentCatalogItemId.HasValue && ParentCatalogItem != null)
            return ParentCatalogItem.EffectivePrice;
        return Price;
    }
}
```

#### Categories Table
Categories are now entity-level (each entity has its own categories).

```sql
-- Add EntityId to Categories table
ALTER TABLE Categories ADD EntityId INT NULL
    FOREIGN KEY REFERENCES Entities(Id) ON DELETE CASCADE;

-- Migration: Link existing categories to their entity via catalog
UPDATE c
SET c.EntityId = cat.EntityId
FROM Categories c
INNER JOIN CatalogCategories cc ON cc.CategoryId = c.Id
INNER JOIN Catalogs cat ON cat.Id = cc.CatalogId;

-- Make EntityId required
ALTER TABLE Categories ALTER COLUMN EntityId INT NOT NULL;

-- Remove UserId (categories now owned by entity, not user)
ALTER TABLE Categories DROP COLUMN UserId;

CREATE INDEX IX_Categories_EntityId ON Categories(EntityId);
```

#### SubscriptionTiers Table
Add product-specific limits.

```sql
ALTER TABLE SubscriptionTiers ADD ProductType INT NOT NULL DEFAULT 1;  -- 1=Menu initially
ALTER TABLE SubscriptionTiers ADD MaxEntities INT DEFAULT 1;
ALTER TABLE SubscriptionTiers ADD MaxCatalogsPerEntity INT DEFAULT 3;
ALTER TABLE SubscriptionTiers ADD MaxLibraryItems INT DEFAULT 50;
ALTER TABLE SubscriptionTiers ADD MaxCategoriesPerCatalog INT DEFAULT 10;
ALTER TABLE SubscriptionTiers ADD MaxBundles INT DEFAULT 0;

CREATE INDEX IX_SubscriptionTiers_ProductType ON SubscriptionTiers(ProductType);
```

**Tier Examples:**
```
Menu Product Tiers:
- Free: 1 entity, 1 catalog, 20 items, 5 categories (7-day trial)
- Starter: 1 entity, 3 catalogs, 50 items, 10 categories, 5 bundles (7-day trial, R99/month)
- Professional: 3 entities, 10 catalogs, 200 items, unlimited categories, 20 bundles (7-day trial, R299/month)
- Enterprise: Unlimited entities, unlimited catalogs, unlimited items, unlimited bundles (7-day trial, R599/month)

Cards Product Tiers:
- Free: 1 entity (profile), basic features (7-day trial)
- Pro: 5 entities (profiles), advanced features (7-day trial, R149/month)

Catalog Product Tiers:
- Free: 1 entity, 1 catalog, 50 products (7-day trial)
- Business: 3 entities, 10 catalogs, 500 products (7-day trial, R199/month)
```

---

## Entity Types Enum

```csharp
public enum EntityType
{
    Restaurant = 0,
    Store = 1,
    Venue = 2,
    Organization = 3
}
```

**Usage:**
- Menu product → typically Restaurant/Venue
- Catalog product → typically Store/Organization
- Cards product → any type (represents the business)

---

## URL Structure

### Frontend (Menu Viewing)

#### Single Catalog Per Entity
```
/{entity_slug}
Example: /the-cocktail-lounge
```

#### Multiple Catalogs Per Entity
**Both formats must be supported:**

Format 1 (Underscore):
```
/{entity_slug}_{catalog_slug}
Example: /the-cocktail-lounge_evening-menu
```

Format 2 (Slash):
```
/{entity_slug}/{catalog_slug}
Example: /the-cocktail-lounge/evening-menu
```

**Frontend Routing Logic:**
```csharp
// Try underscore format first: entity_catalog
var parts = slug.Split('_');
if (parts.Length == 2)
{
    var catalog = await _context.Catalogs
        .Include(c => c.Entity)
        .FirstOrDefaultAsync(c => c.Entity.Slug == parts[0] && c.Slug == parts[1] && c.IsActive);
    if (catalog != null) return catalog;
}

// Try slash format: entity/catalog
if (!string.IsNullOrEmpty(catalogSlug))
{
    var catalog = await _context.Catalogs
        .Include(c => c.Entity)
        .FirstOrDefaultAsync(c => c.Entity.Slug == entitySlug && c.Slug == catalogSlug && c.IsActive);
    if (catalog != null) return catalog;
}

// No catalog slug, get default (first active catalog for entity)
return await _context.Catalogs
    .Where(c => c.Entity.Slug == entitySlug && c.IsActive)
    .OrderBy(c => c.SortOrder)
    .FirstOrDefaultAsync();
```

### API Endpoints
API uses slash format exclusively:
```
GET /api/v1/entities/{entityId}/catalogs/{catalogId}
PUT /api/v1/entities/{entityId}/catalogs/{catalogId}
```

---

## Menu Creation Wizard Flow

### First Menu Creation (No Entities Exist)
**Step 1: Subscription Check**
- Check if user has Menu product subscription
- Show trial status or upgrade prompt

**Step 2: Entity Creation** (New Step)
```
Create Your Restaurant/Venue

Name: [The Cocktail Lounge]
Type: [Restaurant ▼] (dropdown: Restaurant/Venue/Store/Organization)
Slug: [the-cocktail-lounge] (auto-generated, editable)

Description: [Optional description]
Address: [Optional]
City: [Optional]
Phone: [Optional]
```

**Step 3: Menu Details**
```
Menu Details

Name: [Evening Menu]
Slug: [evening-menu] (auto-generated from entity + menu name)
Description: [Our signature cocktails and appetizers]
```

**Step 4: Categories**
- Create categories for this menu
- Inline creation: Click "Add Category" → modal opens → save → modal closes

**Step 5: Menu Items**
- Add items to categories
- Inline creation: Click "Add Item" → modal → save → close
- Nested inline creation:
  - Creating item → need category? → modal over modal
  - Creating item → need option group? → modal over modal
  - Creating option group → need options? → modal over modal over modal

### Subsequent Menu Creation (Entities Exist)
**Step 1: Subscription Check**
- Same as above

**Step 2: Select Entity** (Changed)
```
Select Restaurant/Venue

[ ] The Cocktail Lounge (Restaurant)
[ ] Le Petit Bistro (Restaurant)
[+] Create New Entity
```

If "Create New Entity" selected → show entity creation form (same as first-time flow)

**Step 3-5: Same as above**

---

## Inline Modal Creation Pattern

### Stacked Modals for Dependencies
When creating an item, user may need to create supporting entities inline:

```
[Item Form Modal - Z-index: 1000]
  ↓ User clicks "Create New Category"
  [Category Form Modal - Z-index: 1001]
    → User saves category
    → Category modal closes
    → Category appears in item form dropdown
  ↓ User clicks "Create Option Group"
  [Option Group Form Modal - Z-index: 1001]
    ↓ User clicks "Add Option"
    [Option Form Modal - Z-index: 1002]
      → User saves option
      → Option modal closes
      → Option appears in option group
    → User saves option group
    → Option group modal closes
    → Option group appears in item form
```

**Implementation Notes:**
- Each modal level increments z-index by 1
- Parent modal remains visible (dimmed) behind child modal
- Use Teleport to body for proper stacking
- Track modal stack in component state
- ESC key closes topmost modal only

---

## Library Item Sharing Pattern

### Scenario: Restaurant with Multiple Menus
```
Entity: "The Cocktail Lounge"
├─ Catalog: "Evening Menu"
│  ├─ Mojito (Price: R95) [master item]
│  └─ Caesar Salad (Price: R85) [master item]
│
└─ Catalog: "Happy Hour Menu"
   ├─ Mojito (Price Override: R65) [references master, different price]
   └─ Caesar Salad (Price: R85) [references master, same price]
```

**Database Representation:**
```sql
-- Master items (ParentCatalogItemId = NULL)
INSERT INTO CatalogItems (Name, Price, CatalogId, ParentCatalogItemId)
VALUES ('Mojito', 95.00, 1, NULL);  -- ID: 100

INSERT INTO CatalogItems (Name, Price, CatalogId, ParentCatalogItemId)
VALUES ('Caesar Salad', 85.00, 1, NULL);  -- ID: 101

-- Happy Hour references with override
INSERT INTO CatalogItems (Name, Price, CatalogId, ParentCatalogItemId, PriceOverride)
VALUES ('Mojito', 95.00, 2, 100, 65.00);  -- Override price to R65

INSERT INTO CatalogItems (Name, Price, CatalogId, ParentCatalogItemId, PriceOverride)
VALUES ('Caesar Salad', 85.00, 2, 101, NULL);  -- No override, use parent price
```

### Cross-Entity Sharing
**Decision**: Item sharing is limited to same entity only.

When user wants to use an item from Entity A in Entity B:
- **Copy only** - Creates new master item in Entity B catalog
- No ParentCatalogItemId link across entities
- Changes to Entity A item don't affect Entity B
- User can manually sync changes if needed

**Rationale**: Keeps data ownership clear, prevents confusion when different entities (e.g., different restaurants) want different pricing/descriptions for similar items.

---

## Subscription Billing Logic

### Trial Periods
Each product has independent 7-day trial:
```
User signs up on Jan 1:
- Menu product trial: Jan 1-7
- User adds Catalog product on Jan 3
- Catalog product trial: Jan 3-9

All trials must end before billing starts
First billing date: Jan 10 (after both trials end)
```

### Pro-Rata Billing
When adding product mid-billing cycle:
```
Billing cycle: Jan 1-31
Menu subscription: R299/month

User adds Catalog on Jan 15 (after trial ends):
- Catalog full price: R199/month
- Days remaining in cycle: 16 days
- Pro-rata amount: R199 × (16/31) = R102.71

Next bill (Jan 31):
- Menu: R299
- Catalog (pro-rata): R102.71
- Total: R401.71

Feb 1 onwards:
- Menu: R299
- Catalog: R199
- Total: R498/month
```

### Combined Billing
All active product subscriptions billed together on same date:
```
Invoice #123 - Jan 31, 2025
─────────────────────────────
Menu - Professional Plan       R299.00
Catalog - Business Plan        R199.00
Cards - Pro Plan               R149.00
                          ─────────────
Subtotal                       R647.00
VAT (15%)                       R97.05
                          ─────────────
Total Due                      R744.05
```

---

## API Changes

### New Controllers

#### EntitiesController
```csharp
[Route("api/v1/entities")]
[Authorize]
public class EntitiesController : ControllerBase
{
    [HttpGet]
    Task<IActionResult> GetMyEntities(int? productType = null)
    // Get all entities for current user, optionally filter by product type

    [HttpGet("{id}")]
    Task<IActionResult> GetEntity(int id)

    [HttpPost]
    Task<IActionResult> CreateEntity(CreateEntityDto dto)
    // Check MaxEntities limit for current product subscription

    [HttpPut("{id}")]
    Task<IActionResult> UpdateEntity(int id, UpdateEntityDto dto)

    [HttpDelete("{id}")]
    Task<IActionResult> DeleteEntity(int id)
    // Cascade delete all catalogs for this entity

    [HttpGet("{id}/catalogs")]
    Task<IActionResult> GetEntityCatalogs(int id)
}
```

#### ProductSubscriptionsController
```csharp
[Route("api/v1/subscriptions")]
[Authorize]
public class ProductSubscriptionsController : ControllerBase
{
    [HttpGet]
    Task<IActionResult> GetMySubscriptions()
    // Returns all product subscriptions for current user

    [HttpGet("{productType}")]
    Task<IActionResult> GetProductSubscription(ProductType productType)
    // Get specific product subscription (Cards/Menu/Catalog)

    [HttpPost]
    Task<IActionResult> SubscribeToProduct(SubscribeToProductDto dto)
    // Add new product subscription, calculate pro-rata if applicable

    [HttpPut("{productType}/upgrade")]
    Task<IActionResult> UpgradeTier(ProductType productType, int newTierId)

    [HttpPost("{productType}/cancel")]
    Task<IActionResult> CancelProductSubscription(ProductType productType)

    [HttpGet("invoice-preview")]
    Task<IActionResult> GetInvoicePreview()
    // Show combined billing for all active products
}
```

### Updated Controllers

#### CatalogsController
```csharp
// Update all methods to use EntityId instead of ProfileId

[HttpPost]
Task<IActionResult> CreateCatalog(CreateCatalogDto dto)
// Check MaxCatalogsPerEntity limit
// Verify user owns the entity

[HttpPut("{id}/categories")]
Task<IActionResult> UpdateCatalogCategories(int id, UpdateCategoriesDto dto)
// Update CatalogCategories join table
```

#### CatalogItemsController
```csharp
[HttpPost("{catalogId}/items")]
Task<IActionResult> AddItemToCatalog(int catalogId, AddItemDto dto)
// If dto.ParentCatalogItemId provided, verify parent belongs to same entity
// Create reference with optional price override
// If null, create master item

[HttpPut("{id}/price-override")]
Task<IActionResult> UpdatePriceOverride(int id, decimal? priceOverride)
// Update PriceOverride for referenced item

[HttpPost("{id}/copy-to-catalog")]
Task<IActionResult> CopyItemToAnotherCatalog(int id, CopyItemDto dto)
// Create master copy (no parent reference) in target catalog
// Works across entities (copy only, no reference)
```

#### CategoriesController
```csharp
[Route("api/v1/categories")]
[Authorize]
public class CategoriesController : ControllerBase
{
    [HttpGet("entity/{entityId}")]
    Task<IActionResult> GetEntityCategories(int entityId)
    // Get all categories for this entity

    [HttpPost]
    Task<IActionResult> CreateCategory(CreateCategoryDto dto)
    // Verify user owns the entity
    // Check MaxCategoriesPerCatalog limit

    [HttpPut("{id}")]
    Task<IActionResult> UpdateCategory(int id, UpdateCategoryDto dto)

    [HttpDelete("{id}")]
    Task<IActionResult> DeleteCategory(int id)
    // Remove from CatalogCategories join table entries first
}
```

---

## Frontend Changes

### New Composables

#### `useApi.ts` - Add Entity and Product Subscription APIs
```typescript
// Entities API
export const useEntitiesApi = () => {
  const api = useApi()

  return {
    getMyEntities: (productType?: number) => {
      const params = productType !== undefined ? `?productType=${productType}` : ''
      return api.get(`/entities${params}`)
    },
    getEntity: (id: number) => api.get(`/entities/${id}`),
    createEntity: (data: any) => api.post('/entities', data),
    updateEntity: (id: number, data: any) => api.put(`/entities/${id}`, data),
    deleteEntity: (id: number) => api.delete(`/entities/${id}`),
    getEntityCatalogs: (id: number) => api.get(`/entities/${id}/catalogs`)
  }
}

// Product Subscriptions API
export const useProductSubscriptionsApi = () => {
  const api = useApi()

  return {
    getMySubscriptions: () => api.get('/subscriptions'),
    getProductSubscription: (productType: number) =>
      api.get(`/subscriptions/${productType}`),
    subscribeToProduct: (data: any) => api.post('/subscriptions', data),
    upgradeTier: (productType: number, newTierId: number) =>
      api.put(`/subscriptions/${productType}/upgrade`, { newTierId }),
    cancelProductSubscription: (productType: number) =>
      api.post(`/subscriptions/${productType}/cancel`),
    getInvoicePreview: () => api.get('/subscriptions/invoice-preview')
  }
}

// Categories API (Entity-level)
export const useCategoriesApi = () => {
  const api = useApi()

  return {
    getEntityCategories: (entityId: number) =>
      api.get(`/categories/entity/${entityId}`),
    createCategory: (data: any) => api.post('/categories', data),
    updateCategory: (id: number, data: any) => api.put(`/categories/${id}`, data),
    deleteCategory: (id: number) => api.delete(`/categories/${id}`)
  }
}
```

### Updated Components

#### `MenuCreationWizard.vue`
Add Step 2 for entity selection/creation:
```vue
<template>
  <div v-if="currentStep === 2" class="wizard-step">
    <h3>Select or Create Entity</h3>

    <!-- Existing entities -->
    <div v-if="entities.length > 0" class="entity-list">
      <div
        v-for="entity in entities"
        :key="entity.id"
        class="entity-option"
        :class="{ selected: formData.entityId === entity.id }"
        @click="selectEntity(entity.id)"
      >
        <img v-if="entity.logo" :src="entity.logo" />
        <div>
          <h4>{{ entity.name }}</h4>
          <p>{{ entityTypeLabel(entity.entityType) }}</p>
        </div>
      </div>
    </div>

    <!-- Create new entity -->
    <button @click="showEntityForm = true" class="btn-create-entity">
      <i class="fas fa-plus"></i> Create New Entity
    </button>

    <!-- Entity creation form (inline) -->
    <div v-if="showEntityForm" class="entity-form">
      <input v-model="newEntity.name" placeholder="Entity name" />
      <select v-model="newEntity.entityType">
        <option value="0">Restaurant</option>
        <option value="1">Store</option>
        <option value="2">Venue</option>
        <option value="3">Organization</option>
      </select>
      <input v-model="newEntity.slug" placeholder="URL slug" />
      <!-- Additional fields... -->
      <button @click="createEntity">Save Entity</button>
    </div>
  </div>
</template>

<script setup lang="ts">
const entities = ref([])
const showEntityForm = ref(false)
const newEntity = ref({
  name: '',
  entityType: 0,
  slug: ''
})

onMounted(async () => {
  // Load user's entities for Menu product
  const response = await entitiesApi.getMyEntities(1) // 1 = Menu product
  entities.value = response.data?.entities || []

  // If no entities, auto-show creation form
  if (entities.value.length === 0) {
    showEntityForm.value = true
  }
})

// When entity is selected, load its categories
watch(() => formData.value.entityId, async (entityId) => {
  if (entityId) {
    const response = await categoriesApi.getEntityCategories(entityId)
    categories.value = response.data?.categories || []
  }
})

const createEntity = async () => {
  const response = await entitiesApi.createEntity(newEntity.value)
  if (response.success) {
    entities.value.push(response.data.entity)
    formData.value.entityId = response.data.entity.id
    showEntityForm.value = false
  }
}
</script>
```

#### `ItemFormModal.vue`
Add inline category/option group creation:
```vue
<template>
  <BaseModal :show="show" @close="$emit('close')">
    <form @submit.prevent="saveItem">
      <!-- Category selection with inline create -->
      <div class="form-group">
        <label>Category</label>
        <select v-model="itemForm.categoryId">
          <option v-for="cat in categories" :key="cat.id" :value="cat.id">
            {{ cat.name }}
          </option>
        </select>
        <button type="button" @click="showCategoryModal = true">
          <i class="fas fa-plus"></i> Create Category
        </button>
      </div>

      <!-- Option Groups -->
      <div class="form-group">
        <label>Option Groups</label>
        <div v-for="group in itemForm.optionGroups" :key="group.id">
          {{ group.name }}
        </div>
        <button type="button" @click="showOptionGroupModal = true">
          <i class="fas fa-plus"></i> Add Option Group
        </button>
      </div>

      <button type="submit">Save Item</button>
    </form>

    <!-- Nested modal: Category creation -->
    <CategoryFormModal
      :show="showCategoryModal"
      @close="showCategoryModal = false"
      @created="handleCategoryCreated"
      style="z-index: 1001"
    />

    <!-- Nested modal: Option Group creation -->
    <OptionGroupFormModal
      :show="showOptionGroupModal"
      @close="showOptionGroupModal = false"
      @created="handleOptionGroupCreated"
      style="z-index: 1001"
    />
  </BaseModal>
</template>

<script setup lang="ts">
const showCategoryModal = ref(false)
const showOptionGroupModal = ref(false)

// Props should include entityId
const props = defineProps<{
  show: boolean
  entityId: number
  item?: any
}>()

// Load categories for this entity
onMounted(async () => {
  if (props.entityId) {
    const response = await categoriesApi.getEntityCategories(props.entityId)
    categories.value = response.data?.categories || []
  }
})

const handleCategoryCreated = (category: any) => {
  categories.value.push(category)
  itemForm.value.categoryId = category.id
  showCategoryModal.value = false
}

const handleOptionGroupCreated = (group: any) => {
  itemForm.value.optionGroups.push(group)
  showOptionGroupModal.value = false
}
</script>
```

#### `pages/dashboard/index.vue`
Show all product subscriptions:
```vue
<template>
  <div class="subscription-overview">
    <div v-for="sub in subscriptions" :key="sub.productType" class="product-card">
      <h3>{{ productName(sub.productType) }}</h3>
      <p>{{ tierName(sub.tierId) }}</p>

      <div v-if="sub.isTrialActive" class="trial-banner">
        Trial: {{ sub.trialDaysRemaining }} days remaining
      </div>

      <div class="usage-stats">
        <p>Entities: {{ sub.usage.entities }} / {{ sub.limits.maxEntities }}</p>
        <p>Catalogs: {{ sub.usage.catalogs }} / {{ sub.limits.maxCatalogs }}</p>
      </div>

      <button @click="managePlan(sub.productType)">Manage Plan</button>
    </div>

    <!-- Add product subscription -->
    <div class="add-product-card">
      <h3>Add Product</h3>
      <button @click="subscribeToCards">Add Cards</button>
      <button @click="subscribeToCatalog">Add Catalog</button>
    </div>
  </div>
</template>
```

---

## Data Migration Strategy

### Phase 1: Create New Tables
```sql
-- Run all CREATE TABLE statements from schema section
-- All new tables created without breaking existing functionality
```

### Phase 2: Create Default Entities
```sql
-- For each distinct UserId in Profiles, create a default entity
INSERT INTO Entities (UserId, EntityType, Name, Slug, CreatedAt, UpdatedAt)
SELECT DISTINCT
    p.UserId,
    0 as EntityType,  -- Default to Restaurant
    'Main Restaurant' as Name,
    LOWER(REPLACE(u.Email, '@', '-')) + '-default' as Slug,
    GETUTCDATE(),
    GETUTCDATE()
FROM Profiles p
INNER JOIN Users u ON p.UserId = u.Id
WHERE p.UserId IS NOT NULL;
```

### Phase 3: Migrate Catalogs to Entities
```sql
-- Add EntityId column (nullable)
ALTER TABLE Catalogs ADD EntityId INT NULL;

-- Link catalogs to their user's default entity
UPDATE c
SET c.EntityId = e.Id
FROM Catalogs c
INNER JOIN Profiles p ON c.ProfileId = p.Id
INNER JOIN Entities e ON e.UserId = p.UserId AND e.Name = 'Main Restaurant';

-- Make EntityId required
ALTER TABLE Catalogs ALTER COLUMN EntityId INT NOT NULL;

-- Add foreign key
ALTER TABLE Catalogs ADD CONSTRAINT FK_Catalogs_Entities
    FOREIGN KEY (EntityId) REFERENCES Entities(Id) ON DELETE CASCADE;

-- Drop old ProfileId column
ALTER TABLE Catalogs DROP CONSTRAINT FK_Catalogs_Profiles;
ALTER TABLE Catalogs DROP COLUMN ProfileId;
```

### Phase 4: Create Product Subscriptions
```sql
-- For each user with active subscription, create Menu product subscription
INSERT INTO ProductSubscriptions (
    UserId,
    ProductType,
    TierId,
    IsTrialActive,
    TrialStartDate,
    TrialEndDate,
    Status,
    CreatedAt,
    UpdatedAt
)
SELECT
    s.UserId,
    1 as ProductType,  -- Menu product
    s.TierId,
    s.IsTrialActive,
    s.TrialStartDate,
    s.TrialEndDate,
    s.Status,
    s.CreatedAt,
    GETUTCDATE()
FROM Subscriptions s
WHERE s.IsActive = 1;
```

### Phase 5: Create Subscription Tiers for Each Product
```sql
-- Copy existing tiers for Menu product
UPDATE SubscriptionTiers SET ProductType = 1 WHERE ProductType = 0;

-- Create Cards product tiers
INSERT INTO SubscriptionTiers (Name, ProductType, Price, BillingCycle, Features, MaxEntities, TrialDays)
VALUES
('Cards Free', 0, 0, 0, 'Basic digital business card', 1, 7),
('Cards Pro', 0, 149, 0, 'Advanced features, 5 profiles', 5, 7);

-- Create Catalog product tiers
INSERT INTO SubscriptionTiers (Name, ProductType, Price, BillingCycle, Features, MaxEntities, MaxCatalogsPerEntity, MaxLibraryItems, TrialDays)
VALUES
('Catalog Free', 2, 0, 0, 'Basic catalog, 50 products', 1, 1, 50, 7),
('Catalog Business', 2, 199, 0, 'Advanced catalog, 500 products', 3, 10, 500, 7);
```

### Phase 6: Migrate Categories to Entity-Level
```sql
-- Add EntityId to Categories (nullable first)
ALTER TABLE Categories ADD EntityId INT NULL;

-- Link categories to entities via their catalogs
UPDATE c
SET c.EntityId = cat.EntityId
FROM Categories c
INNER JOIN CatalogCategories cc ON cc.CategoryId = c.Id
INNER JOIN Catalogs cat ON cat.Id = cc.CatalogId;

-- For categories not yet in CatalogCategories, link via old CatalogId if exists
UPDATE c
SET c.EntityId = cat.EntityId
FROM Categories c
INNER JOIN Catalogs cat ON cat.Id = c.CatalogId
WHERE c.EntityId IS NULL AND c.CatalogId IS NOT NULL;

-- Make EntityId required
ALTER TABLE Categories ALTER COLUMN EntityId INT NOT NULL;

-- Add foreign key and index
ALTER TABLE Categories ADD CONSTRAINT FK_Categories_Entities
    FOREIGN KEY (EntityId) REFERENCES Entities(Id) ON DELETE CASCADE;
CREATE INDEX IX_Categories_EntityId ON Categories(EntityId);

-- Remove UserId (categories now owned by entity, not user)
ALTER TABLE Categories DROP COLUMN UserId;

-- Create CatalogCategories join table entries if not already done
INSERT INTO CatalogCategories (CatalogId, CategoryId, SortOrder, CreatedAt)
SELECT DISTINCT
    c.CatalogId,
    cat.Id as CategoryId,
    cat.SortOrder,
    GETUTCDATE()
FROM Categories cat
INNER JOIN Catalogs c ON c.EntityId = cat.EntityId
WHERE NOT EXISTS (
    SELECT 1 FROM CatalogCategories cc
    WHERE cc.CatalogId = c.Id AND cc.CategoryId = cat.Id
);

-- Remove old CatalogId from Categories if it exists
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Categories') AND name = 'CatalogId')
BEGIN
    ALTER TABLE Categories DROP COLUMN CatalogId;
END
```

---

## Implementation Phases

### Phase 1: Database & Core Backend (Week 1-2)
- [ ] Create Entities table and migration
- [ ] Create ProductSubscriptions table
- [ ] Create CatalogCategories table
- [ ] Update Catalogs table (add EntityId)
- [ ] Update CatalogItems table (add ParentCatalogItemId, PriceOverride)
- [ ] Update SubscriptionTiers table (add product limits)
- [ ] Run data migrations
- [ ] Create EntitiesController
- [ ] Create ProductSubscriptionsController
- [ ] Update CatalogsController
- [ ] Add price override logic to CatalogItemsController

### Phase 2: Frontend Foundation (Week 2-3)
- [ ] Add useEntitiesApi composable
- [ ] Add useProductSubscriptionsApi composable
- [ ] Update MenuCreationWizard with entity step
- [ ] Create EntitySelector component
- [ ] Create EntityFormModal component
- [ ] Update subscription check logic for multiple products

### Phase 3: Inline Creation UX (Week 3-4)
- [ ] Implement stacked modal system (z-index management)
- [ ] Add inline category creation to ItemFormModal
- [ ] Add inline option group creation to ItemFormModal
- [ ] Add inline option creation to OptionGroupFormModal
- [ ] Add inline extra group/extra creation

### Phase 4: Multi-Product Dashboard (Week 4-5)
- [ ] Update dashboard to show all product subscriptions
- [ ] Add product subscription cards with usage stats
- [ ] Create combined billing invoice preview
- [ ] Add upgrade/downgrade flows per product
- [ ] Add "Add Product" subscription flow

### Phase 5: Library Sharing & Overrides (Week 5-6)
- [ ] Implement price override UI in catalog item editor
- [ ] Add "Copy to another catalog" functionality
- [ ] Show parent item indicator for referenced items
- [ ] Add "Reset to default price" button for overrides
- [ ] Create "Item usage" view (shows which catalogs use this item)

### Phase 6: URL Routing & Polish (Week 6-7)
- [ ] Implement dynamic routing for /{entity_slug}/{catalog_slug}
- [ ] Add fallback for single-catalog entities
- [ ] Update all navigation links
- [ ] Add SEO metadata for entity/catalog pages
- [ ] Testing and bug fixes

---

## Risk Mitigation

### Risk 1: Data Migration Complexity
**Impact:** High - Could corrupt existing data
**Mitigation:**
- Full database backup before migration
- Run migrations on staging environment first
- Write rollback scripts for each migration step
- Validate data integrity with queries after each phase

### Risk 2: Breaking Changes to Existing Frontend
**Impact:** Medium - Users may experience downtime
**Mitigation:**
- Maintain backward compatibility in API during transition
- Feature flags for new entity-based flows
- Gradual rollout to beta users first
- Keep old ProfileId temporarily as deprecated field

### Risk 3: User Confusion with Multi-Product Subscriptions
**Impact:** Medium - Users may not understand billing
**Mitigation:**
- Clear onboarding flow explaining product separation
- Detailed invoice preview before charging
- In-app help tooltips and documentation
- Email notifications before first combined billing

### Risk 4: Performance Degradation with Price Overrides
**Impact:** Low - Recursive queries for parent prices
**Mitigation:**
- Add database index on ParentCatalogItemId
- Implement caching for frequently accessed items
- Limit parent chain depth (max 1 level deep)
- Use computed EffectivePrice property

---

## Success Criteria

### Technical
- [ ] All existing functionality works after migration
- [ ] No data loss or corruption
- [ ] API response times under 500ms for catalog loads
- [ ] Zero downtime deployment

### User Experience
- [ ] Users can create multiple entities
- [ ] Users can subscribe to multiple products
- [ ] Inline creation flows work for categories/option groups
- [ ] Price overrides work correctly
- [ ] Combined billing shows correct amounts

### Business
- [ ] Users upgrade to multi-product subscriptions
- [ ] Reduced support tickets about entity/menu confusion
- [ ] Successful pro-rata billing for mid-cycle additions

---

## Decisions Made

1. **Cross-entity item sharing**: Same entity only - ParentCatalogItemId references only work within same entity

2. **Entity limit enforcement**: Free tier allows 1 entity (no forced upgrade)

3. **Category sharing scope**: Entity-level - each entity has its own categories (Categories table has EntityId FK)

4. **URL format**:
   - **Frontend viewing**: Support both `/{entity_slug}_{catalog_slug}` AND `/{entity_slug}/{catalog_slug}` for multiple catalogs
   - **Single catalog**: `/{entity_slug}` only
   - **API**: `/{entity}/{catalog}` format exclusively

5. **Bundle assignment**: Catalog-level - bundles belong to specific catalog (not shared across entity's catalogs)

---

## Next Steps

1. **Review this plan** - Confirm all clarifications are correctly incorporated
2. **Discuss open questions** - Get decisions on ambiguous points
3. **Create detailed task breakdown** - Break each phase into specific tickets
4. **Set up development branch** - Create `feature/multi-product-architecture` branch
5. **Begin Phase 1 implementation** - Start with database schema and migrations
