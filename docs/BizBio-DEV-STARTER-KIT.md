# BizBio - Development Starter Kit (Phase 1 MVP)

**Version:** 1.0  
**Date:** November 2025  
**Scope:** Phase 1 MVP - NFC Tables + Event Mode  
**Timeline:** 4 weeks (60-80 hours)

---

## Table of Contents

1. [MVP Scope](#1-mvp-scope)
2. [Database Setup](#2-database-setup)
3. [API Endpoints (Phase 1)](#3-api-endpoints-phase-1)
4. [Implementation Order](#4-implementation-order)
5. [Code Samples](#5-code-samples)
6. [Testing Checklist](#6-testing-checklist)

---

## 1. MVP Scope

### What We're Building (Phase 1)

**Core Features:**
- ✅ NFC table tags that link to specific tables
- ✅ Table info display (fun fact, images)
- ✅ Event Mode toggle (show/hide menu items)
- ✅ Basic table management (CRUD)
- ✅ Basic analytics (scan tracking)

**NOT in Phase 1:**
- ❌ Menu versioning/templates (Phase 2)
- ❌ Advanced analytics dashboard (Phase 2)
- ❌ WhatsApp integration (Phase 3)
- ❌ Multi-language (Phase 3)

---

## 2. Database Setup

### Step 1: Run These Migrations

Copy and run these SQL scripts in order:

#### Migration 1: Create RestaurantTables Table

```sql
-- File: 001_create_restaurant_tables.sql
CREATE TABLE RestaurantTables (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    
    -- Relationships
    ProfileId INT NOT NULL,
    
    -- Table Info
    TableNumber INT NOT NULL,
    TableName VARCHAR(100),
    TableCategory ENUM('Regular', 'VIP', 'Window', 'Patio', 'Private', 'Bar', 'Outdoor') DEFAULT 'Regular',
    
    -- Table Personality
    FunFact TEXT,
    Description TEXT,
    Images JSON,  -- Array of image URLs: ["url1", "url2"]
    
    -- NFC Configuration
    NFCTagCode VARCHAR(50) UNIQUE,
    NFCTagType ENUM('Sticker', 'Disc') DEFAULT 'Sticker',
    NFCTagStatus ENUM('Active', 'Inactive', 'Lost', 'Damaged') DEFAULT 'Active',
    
    -- Metadata
    IsActive BOOLEAN DEFAULT TRUE,
    SortOrder INT DEFAULT 0,
    CreatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    CreatedBy INT,
    
    -- Foreign Keys
    FOREIGN KEY (ProfileId) REFERENCES Profiles(Id) ON DELETE CASCADE,
    FOREIGN KEY (CreatedBy) REFERENCES Users(Id) ON DELETE SET NULL,
    
    -- Indexes
    UNIQUE KEY unique_table_number (ProfileId, TableNumber),
    INDEX idx_nfc_code (NFCTagCode),
    INDEX idx_profile_id (ProfileId),
    INDEX idx_active (IsActive)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
```

#### Migration 2: Add Event Mode to Profiles

```sql
-- File: 002_add_event_mode_to_profiles.sql
ALTER TABLE Profiles 
    ADD COLUMN EventModeEnabled BOOLEAN DEFAULT FALSE,
    ADD COLUMN EventModeName VARCHAR(100),
    ADD COLUMN EventModeDescription TEXT,
    ADD COLUMN HasMenuProAddon BOOLEAN DEFAULT FALSE,
    ADD INDEX idx_event_mode (EventModeEnabled);
```

#### Migration 3: Add Event Mode to CatalogItems

```sql
-- File: 003_add_event_mode_to_catalog_items.sql
ALTER TABLE CatalogItems 
    ADD COLUMN AvailableInEventMode BOOLEAN DEFAULT TRUE,
    ADD COLUMN EventModeOnly BOOLEAN DEFAULT FALSE,
    ADD INDEX idx_event_availability (AvailableInEventMode);
```

#### Migration 4: Create NFCScans Table (Analytics)

```sql
-- File: 004_create_nfc_scans.sql
CREATE TABLE NFCScans (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    
    -- Relationships
    ProfileId INT NOT NULL,
    TableId INT,
    
    -- NFC Info
    NFCTagCode VARCHAR(50) NOT NULL,
    
    -- Request Info
    IPAddress VARCHAR(45),
    UserAgent TEXT,
    DeviceType ENUM('Mobile', 'Tablet', 'Desktop', 'Unknown') DEFAULT 'Unknown',
    
    -- Location (optional, from IP lookup)
    Country VARCHAR(2),
    City VARCHAR(100),
    
    -- Timing
    ScannedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    
    -- Session (for unique visitor tracking)
    SessionId VARCHAR(100),
    
    -- Foreign Keys
    FOREIGN KEY (ProfileId) REFERENCES Profiles(Id) ON DELETE CASCADE,
    FOREIGN KEY (TableId) REFERENCES RestaurantTables(Id) ON DELETE SET NULL,
    
    -- Indexes
    INDEX idx_profile_scans (ProfileId, ScannedAt),
    INDEX idx_table_scans (TableId, ScannedAt),
    INDEX idx_nfc_code (NFCTagCode),
    INDEX idx_scanned_at (ScannedAt)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
```

---

### Step 2: Seed Sample Data (For Testing)

```sql
-- File: seed_nfc_sample_data.sql

-- Sample: Enable Menu Pro for test restaurant
UPDATE Profiles 
SET HasMenuProAddon = TRUE 
WHERE Id = 1;

-- Sample: Create test tables
INSERT INTO RestaurantTables (
    ProfileId, TableNumber, TableName, TableCategory,
    FunFact, Images, NFCTagCode, NFCTagType
) VALUES 
(
    1,  -- Your test restaurant ProfileId
    5, 
    'Window Seat', 
    'Window',
    'This table was where our first customer sat when we opened in 2015. She ordered our signature margherita pizza and gave us our first 5-star review on Google!',
    '["https://picsum.photos/400/300", "https://picsum.photos/400/301"]',
    'T5A3B9',
    'Sticker'
),
(
    1,
    12,
    'Chef''s Table',
    'VIP',
    'Watch the magic happen! This table offers a front-row seat to our open kitchen. Chef Marco personally greets every guest sitting here.',
    '["https://picsum.photos/400/302"]',
    'T12X7Y2',
    'Disc'
);

-- Sample: Mark some menu items for Event Mode testing
UPDATE CatalogItems 
SET AvailableInEventMode = TRUE 
WHERE Id IN (1, 2, 3, 5, 8);  -- Adjust IDs as needed

UPDATE CatalogItems 
SET AvailableInEventMode = FALSE 
WHERE Id IN (4, 6, 7, 9, 10);  -- These won't show in Event Mode
```

---

## 3. API Endpoints (Phase 1)

### Endpoint 1: Get Menu with NFC Support

**The most important endpoint - this is what customers see**

```http
GET /api/v1/c/{restaurantSlug}?nfc={nfcCode}
```

**Example:**
```http
GET /api/v1/c/joes-pizza?nfc=T5A3B9
```

**Response:**
```json
{
  "success": true,
  "data": {
    "restaurant": {
      "id": 1,
      "name": "Joe's Pizza",
      "slug": "joes-pizza",
      "description": "Authentic wood-fired pizza since 2015",
      "logo": "https://cdn.bizbio.co.za/logos/joes.jpg",
      "contactInfo": {
        "phone": "+27 11 123 4567",
        "email": "info@joespizza.co.za"
      }
    },
    "table": {
      "id": 5,
      "number": 5,
      "name": "Window Seat",
      "category": "Window",
      "funFact": "This table was where our first customer sat...",
      "images": [
        "https://picsum.photos/400/300",
        "https://picsum.photos/400/301"
      ]
    },
    "eventMode": {
      "enabled": false,
      "eventName": null,
      "description": null
    },
    "menu": {
      "categories": [
        {
          "id": 1,
          "name": "Pizzas",
          "items": [
            {
              "id": 1,
              "name": "Margherita",
              "description": "Classic tomato and mozzarella",
              "price": 85.00,
              "images": ["https://..."],
              "portions": [
                { "name": "Small", "price": 65.00 },
                { "name": "Medium", "price": 85.00 },
                { "name": "Large", "price": 105.00 }
              ]
            }
          ]
        }
      ]
    }
  }
}
```

**Implementation:**
```csharp
[HttpGet("c/{slug}")]
public async Task<IActionResult> GetMenuBySlug(
    string slug,
    [FromQuery] string nfc = null)
{
    // 1. Get restaurant profile
    var profile = await _context.Profiles
        .FirstOrDefaultAsync(p => p.Slug == slug && p.IsActive);
    
    if (profile == null)
        return NotFound(new { success = false, error = "Restaurant not found" });
    
    // 2. Get table info if NFC code provided
    RestaurantTable table = null;
    if (!string.IsNullOrEmpty(nfc))
    {
        table = await _context.RestaurantTables
            .FirstOrDefaultAsync(t => 
                t.NFCTagCode == nfc && 
                t.ProfileId == profile.Id && 
                t.IsActive);
        
        // Log the scan for analytics
        if (table != null)
        {
            var scan = new NFCScan
            {
                ProfileId = profile.Id,
                TableId = table.Id,
                NFCTagCode = nfc,
                IPAddress = HttpContext.Connection.RemoteIpAddress?.ToString(),
                UserAgent = HttpContext.Request.Headers["User-Agent"].ToString(),
                DeviceType = DetermineDeviceType(HttpContext.Request.Headers["User-Agent"]),
                ScannedAt = DateTime.UtcNow,
                SessionId = HttpContext.Session.Id
            };
            _context.NFCScans.Add(scan);
            await _context.SaveChangesAsync();
        }
    }
    
    // 3. Get menu items
    var catalog = await _context.Catalogs
        .FirstOrDefaultAsync(c => c.ProfileId == profile.Id);
    
    var items = await _context.CatalogItems
        .Where(i => i.CatalogId == catalog.Id && i.IsActive)
        .ToListAsync();
    
    // 4. Filter by event mode if enabled
    if (profile.EventModeEnabled)
    {
        items = items.Where(i => i.AvailableInEventMode).ToList();
    }
    
    // 5. Build response
    var response = new
    {
        success = true,
        data = new
        {
            restaurant = MapRestaurant(profile),
            table = table != null ? MapTable(table) : null,
            eventMode = new
            {
                enabled = profile.EventModeEnabled,
                eventName = profile.EventModeName,
                description = profile.EventModeDescription
            },
            menu = MapMenu(items)
        }
    };
    
    return Ok(response);
}

private DeviceType DetermineDeviceType(string userAgent)
{
    if (string.IsNullOrEmpty(userAgent))
        return DeviceType.Unknown;
    
    userAgent = userAgent.ToLower();
    
    if (userAgent.Contains("mobile") || userAgent.Contains("android"))
        return DeviceType.Mobile;
    if (userAgent.Contains("tablet") || userAgent.Contains("ipad"))
        return DeviceType.Tablet;
    if (userAgent.Contains("windows") || userAgent.Contains("mac"))
        return DeviceType.Desktop;
    
    return DeviceType.Unknown;
}
```

---

### Endpoint 2: Create Table

```http
POST /api/v1/dashboard/tables
Authorization: Bearer {jwt}
Content-Type: application/json

{
  "tableNumber": 5,
  "tableName": "Window Seat",
  "category": "Window",
  "funFact": "This table was where our first customer sat...",
  "nfcTagCode": "T5A3B9"
}
```

**Implementation:**
```csharp
[HttpPost("dashboard/tables")]
[Authorize]
public async Task<IActionResult> CreateTable([FromBody] CreateTableDto dto)
{
    var userId = GetCurrentUserId();
    var profileId = GetCurrentProfileId(); // From JWT or session
    
    // 1. Get profile and verify ownership
    var profile = await _context.Profiles
        .FirstOrDefaultAsync(p => p.Id == profileId && p.UserId == userId);
    
    if (profile == null)
        return Forbid();
    
    // 2. Check Menu Pro subscription
    if (!profile.HasMenuProAddon)
    {
        return BadRequest(new 
        { 
            success = false, 
            error = "Menu Pro add-on required. Please upgrade to use table features." 
        });
    }
    
    // 3. Check if table number already exists
    var existingTable = await _context.RestaurantTables
        .AnyAsync(t => t.ProfileId == profileId && t.TableNumber == dto.TableNumber);
    
    if (existingTable)
    {
        return BadRequest(new 
        { 
            success = false, 
            error = $"Table {dto.TableNumber} already exists" 
        });
    }
    
    // 4. Check if NFC code already used
    if (!string.IsNullOrEmpty(dto.NFCTagCode))
    {
        var nfcExists = await _context.RestaurantTables
            .AnyAsync(t => t.NFCTagCode == dto.NFCTagCode);
        
        if (nfcExists)
        {
            return BadRequest(new 
            { 
                success = false, 
                error = $"NFC code {dto.NFCTagCode} is already in use" 
            });
        }
    }
    
    // 5. Create table
    var table = new RestaurantTable
    {
        ProfileId = profileId,
        TableNumber = dto.TableNumber,
        TableName = dto.TableName,
        TableCategory = Enum.Parse<TableCategory>(dto.Category),
        FunFact = dto.FunFact,
        NFCTagCode = dto.NFCTagCode,
        NFCTagType = NFCTagType.Sticker,
        NFCTagStatus = NFCTagStatus.Active,
        IsActive = true,
        CreatedBy = userId,
        CreatedAt = DateTime.UtcNow
    };
    
    _context.RestaurantTables.Add(table);
    await _context.SaveChangesAsync();
    
    return Ok(new 
    { 
        success = true, 
        data = new 
        {
            table = MapTable(table),
            nfcUrl = $"https://bizbio.co.za/c/{profile.Slug}?nfc={table.NFCTagCode}"
        }
    });
}
```

---

### Endpoint 3: Toggle Event Mode

```http
POST /api/v1/dashboard/event-mode/toggle
Authorization: Bearer {jwt}
Content-Type: application/json

{
  "enabled": true,
  "eventName": "Wedding Package",
  "description": "Limited menu for faster service"
}
```

**Implementation:**
```csharp
[HttpPost("dashboard/event-mode/toggle")]
[Authorize]
public async Task<IActionResult> ToggleEventMode([FromBody] EventModeDto dto)
{
    var userId = GetCurrentUserId();
    var profileId = GetCurrentProfileId();
    
    var profile = await _context.Profiles
        .FirstOrDefaultAsync(p => p.Id == profileId && p.UserId == userId);
    
    if (profile == null)
        return Forbid();
    
    // Check Menu Pro
    if (!profile.HasMenuProAddon)
    {
        return BadRequest(new 
        { 
            success = false, 
            error = "Menu Pro required for Event Mode" 
        });
    }
    
    // Toggle event mode
    profile.EventModeEnabled = dto.Enabled;
    profile.EventModeName = dto.Enabled ? dto.EventName : null;
    profile.EventModeDescription = dto.Enabled ? dto.Description : null;
    profile.UpdatedAt = DateTime.UtcNow;
    
    await _context.SaveChangesAsync();
    
    // Count items affected
    var totalItems = await _context.CatalogItems
        .CountAsync(i => i.Catalog.ProfileId == profileId && i.IsActive);
    
    var availableItems = await _context.CatalogItems
        .CountAsync(i => 
            i.Catalog.ProfileId == profileId && 
            i.IsActive && 
            i.AvailableInEventMode);
    
    return Ok(new 
    { 
        success = true,
        data = new
        {
            eventMode = new
            {
                enabled = profile.EventModeEnabled,
                eventName = profile.EventModeName,
                description = profile.EventModeDescription
            },
            itemsAvailable = availableItems,
            itemsHidden = totalItems - availableItems
        },
        message = dto.Enabled 
            ? $"Event mode activated. Showing {availableItems} of {totalItems} items."
            : "Event mode deactivated. Full menu restored."
    });
}
```

---

### Endpoint 4: List Tables

```http
GET /api/v1/dashboard/tables
Authorization: Bearer {jwt}
```

**Implementation:**
```csharp
[HttpGet("dashboard/tables")]
[Authorize]
public async Task<IActionResult> GetTables()
{
    var profileId = GetCurrentProfileId();
    
    var tables = await _context.RestaurantTables
        .Where(t => t.ProfileId == profileId && t.IsActive)
        .OrderBy(t => t.TableNumber)
        .Select(t => new
        {
            id = t.Id,
            tableNumber = t.TableNumber,
            tableName = t.TableName,
            category = t.TableCategory.ToString(),
            funFact = t.FunFact,
            images = t.Images,
            nfcTagCode = t.NFCTagCode,
            nfcTagStatus = t.NFCTagStatus.ToString(),
            // Basic analytics
            totalScans = _context.NFCScans.Count(s => s.TableId == t.Id),
            scansLast7Days = _context.NFCScans.Count(s => 
                s.TableId == t.Id && 
                s.ScannedAt >= DateTime.UtcNow.AddDays(-7))
        })
        .ToListAsync();
    
    return Ok(new 
    { 
        success = true, 
        data = new { tables } 
    });
}
```

---

### Endpoint 5: Update Item Event Availability

```http
PUT /api/v1/dashboard/items/{itemId}/event-availability
Authorization: Bearer {jwt}
Content-Type: application/json

{
  "availableInEventMode": true
}
```

**Implementation:**
```csharp
[HttpPut("dashboard/items/{itemId}/event-availability")]
[Authorize]
public async Task<IActionResult> UpdateEventAvailability(
    int itemId, 
    [FromBody] UpdateEventAvailabilityDto dto)
{
    var profileId = GetCurrentProfileId();
    
    var item = await _context.CatalogItems
        .Include(i => i.Catalog)
        .FirstOrDefaultAsync(i => 
            i.Id == itemId && 
            i.Catalog.ProfileId == profileId);
    
    if (item == null)
        return NotFound();
    
    item.AvailableInEventMode = dto.AvailableInEventMode;
    item.UpdatedAt = DateTime.UtcNow;
    
    await _context.SaveChangesAsync();
    
    return Ok(new 
    { 
        success = true,
        message = dto.AvailableInEventMode 
            ? "Item will be shown in Event Mode"
            : "Item will be hidden in Event Mode"
    });
}
```

---

## 4. Implementation Order

### Week 1: Database + Core API

**Day 1-2: Database Setup**
- [ ] Run all 4 migrations
- [ ] Seed sample data
- [ ] Verify tables created correctly
- [ ] Test foreign key relationships

**Day 3-5: Menu API with NFC**
- [ ] Implement `GET /api/v1/c/{slug}?nfc={code}`
- [ ] Add NFC scan logging
- [ ] Test with sample NFC codes
- [ ] Handle edge cases (invalid codes, etc.)

**Day 6-8: Table Management**
- [ ] Implement `POST /api/v1/dashboard/tables`
- [ ] Implement `GET /api/v1/dashboard/tables`
- [ ] Add validation (unique table numbers, NFC codes)
- [ ] Test CRUD operations

**Day 9-10: Event Mode**
- [ ] Implement `POST /api/v1/dashboard/event-mode/toggle`
- [ ] Implement `PUT /api/v1/dashboard/items/{id}/event-availability`
- [ ] Test event mode filtering
- [ ] Verify menu changes instantly

---

### Week 2: Frontend (Customer-Facing)

**Day 11-13: Menu View**
```vue
<!-- MenuView.vue -->
<template>
  <div class="menu-page">
    <!-- Table Personality Banner (shows if ?nfc= parameter) -->
    <div v-if="tableInfo" class="table-banner">
      <h2>📍 You're at Table {{ tableInfo.number }}</h2>
      <p v-if="tableInfo.name" class="table-name">{{ tableInfo.name }}</p>
      
      <div v-if="tableInfo.funFact" class="fun-fact">
        <h3>💡 Did You Know?</h3>
        <p>{{ tableInfo.funFact }}</p>
      </div>
      
      <div v-if="tableInfo.images?.length" class="images">
        <img v-for="img in tableInfo.images" :key="img" :src="img" />
      </div>
    </div>
    
    <!-- Event Mode Banner -->
    <div v-if="eventMode.enabled" class="event-banner">
      🎉 {{ eventMode.eventName }}
    </div>
    
    <!-- Menu -->
    <MenuCategories :categories="menuCategories" />
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { useRoute } from 'vue-router'
import axios from 'axios'

const route = useRoute()
const tableInfo = ref(null)
const eventMode = ref({ enabled: false })
const menuCategories = ref([])

onMounted(async () => {
  const slug = route.params.slug
  const nfc = route.query.nfc
  
  const { data } = await axios.get(`/api/v1/c/${slug}`, {
    params: { nfc }
  })
  
  tableInfo.value = data.data.table
  eventMode.value = data.data.eventMode
  menuCategories.value = data.data.menu.categories
})
</script>

<style scoped>
.table-banner {
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  color: white;
  padding: 2rem;
  border-radius: 12px;
  margin-bottom: 2rem;
}

.fun-fact {
  background: rgba(255, 255, 255, 0.1);
  padding: 1.5rem;
  border-radius: 8px;
  margin-top: 1rem;
}

.images {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(150px, 1fr));
  gap: 1rem;
  margin-top: 1rem;
}

.images img {
  width: 100%;
  height: 150px;
  object-fit: cover;
  border-radius: 8px;
}

.event-banner {
  background: #f59e0b;
  color: white;
  padding: 1rem;
  text-align: center;
  font-weight: 600;
  border-radius: 8px;
  margin-bottom: 2rem;
}
</style>
```

**Day 14: Test on Real Phone**
- [ ] Test NFC scanning on iPhone/Android
- [ ] Verify table info displays correctly
- [ ] Test event mode banner
- [ ] Check mobile responsiveness

---

### Week 3: Dashboard UI

**Day 15-17: Table Management**
```vue
<!-- TableManagement.vue -->
<template>
  <div class="table-management">
    <div class="header">
      <h1>Manage Tables</h1>
      <button @click="showCreateModal = true">+ Add Table</button>
    </div>
    
    <div class="tables-grid">
      <div v-for="table in tables" :key="table.id" class="table-card">
        <div class="table-header">
          <h3>Table {{ table.tableNumber }}</h3>
          <span class="category">{{ table.category }}</span>
        </div>
        
        <p class="table-name">{{ table.tableName }}</p>
        
        <div class="nfc-info">
          <span class="nfc-code">{{ table.nfcTagCode }}</span>
          <span :class="'status-' + table.nfcTagStatus">
            {{ table.nfcTagStatus }}
          </span>
        </div>
        
        <div class="analytics">
          <div>Total Scans: {{ table.totalScans }}</div>
          <div>Last 7 Days: {{ table.scansLast7Days }}</div>
        </div>
        
        <button @click="editTable(table)">Edit</button>
      </div>
    </div>
    
    <!-- Create/Edit Modal -->
    <Modal v-if="showCreateModal" @close="showCreateModal = false">
      <h2>{{ editingTable ? 'Edit' : 'Add' }} Table</h2>
      
      <form @submit.prevent="saveTable">
        <input 
          v-model="form.tableNumber" 
          type="number" 
          placeholder="Table Number"
          required
        />
        
        <input 
          v-model="form.tableName" 
          placeholder="Table Name (optional)"
        />
        
        <select v-model="form.category">
          <option value="Regular">Regular</option>
          <option value="VIP">VIP</option>
          <option value="Window">Window</option>
          <option value="Patio">Patio</option>
        </select>
        
        <textarea 
          v-model="form.funFact" 
          placeholder="Fun fact about this table..."
          rows="4"
        />
        
        <input 
          v-model="form.nfcTagCode" 
          placeholder="NFC Tag Code (e.g., T5A3B9)"
          required
        />
        
        <button type="submit">Save Table</button>
      </form>
    </Modal>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import axios from 'axios'

const tables = ref([])
const showCreateModal = ref(false)
const editingTable = ref(null)
const form = ref({
  tableNumber: '',
  tableName: '',
  category: 'Regular',
  funFact: '',
  nfcTagCode: ''
})

onMounted(async () => {
  await loadTables()
})

const loadTables = async () => {
  const { data } = await axios.get('/api/v1/dashboard/tables')
  tables.value = data.data.tables
}

const saveTable = async () => {
  if (editingTable.value) {
    // Update (not implemented in Phase 1)
  } else {
    await axios.post('/api/v1/dashboard/tables', form.value)
  }
  
  await loadTables()
  showCreateModal.value = false
  resetForm()
}

const editTable = (table) => {
  editingTable.value = table
  form.value = { ...table }
  showCreateModal.value = true
}

const resetForm = () => {
  form.value = {
    tableNumber: '',
    tableName: '',
    category: 'Regular',
    funFact: '',
    nfcTagCode: ''
  }
  editingTable.value = null
}
</script>
```

**Day 18-19: Event Mode UI**
```vue
<!-- EventModeToggle.vue -->
<template>
  <div class="event-mode-panel">
    <h1>Event Mode</h1>
    
    <div class="toggle-section">
      <label class="toggle">
        <input 
          v-model="eventMode.enabled" 
          type="checkbox"
          @change="toggleEventMode"
        />
        <span class="slider"></span>
        <span class="label">
          {{ eventMode.enabled ? 'ON' : 'OFF' }}
        </span>
      </label>
    </div>
    
    <div v-if="eventMode.enabled" class="event-config">
      <input 
        v-model="eventMode.eventName"
        placeholder="Event Name (e.g., Wedding Package)"
        @blur="updateEventMode"
      />
      
      <textarea 
        v-model="eventMode.description"
        placeholder="Description..."
        @blur="updateEventMode"
      />
      
      <div class="stats">
        <div>Items Available: {{ itemsAvailable }}</div>
        <div>Items Hidden: {{ itemsHidden }}</div>
      </div>
    </div>
    
    <div class="menu-items">
      <h2>Menu Items Availability</h2>
      <div v-for="item in menuItems" :key="item.id" class="item-row">
        <span>{{ item.name }}</span>
        <label>
          <input 
            v-model="item.availableInEventMode"
            type="checkbox"
            @change="updateItemAvailability(item)"
          />
          Available in Event Mode
        </label>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import axios from 'axios'

const eventMode = ref({
  enabled: false,
  eventName: '',
  description: ''
})
const menuItems = ref([])
const itemsAvailable = ref(0)
const itemsHidden = ref(0)

onMounted(async () => {
  await loadEventMode()
  await loadMenuItems()
})

const toggleEventMode = async () => {
  const { data } = await axios.post('/api/v1/dashboard/event-mode/toggle', {
    enabled: eventMode.value.enabled,
    eventName: eventMode.value.eventName,
    description: eventMode.value.description
  })
  
  itemsAvailable.value = data.data.itemsAvailable
  itemsHidden.value = data.data.itemsHidden
}

const updateEventMode = async () => {
  if (eventMode.value.enabled) {
    await toggleEventMode()
  }
}

const updateItemAvailability = async (item) => {
  await axios.put(`/api/v1/dashboard/items/${item.id}/event-availability`, {
    availableInEventMode: item.availableInEventMode
  })
}

const loadEventMode = async () => {
  const { data } = await axios.get('/api/v1/dashboard/event-mode')
  eventMode.value = data.data
}

const loadMenuItems = async () => {
  const { data } = await axios.get('/api/v1/dashboard/items')
  menuItems.value = data.data.items
}
</script>
```

**Day 20: Integration Testing**
- [ ] Test complete flow: Create table → NFC scan → Event mode toggle
- [ ] Test on multiple devices
- [ ] Fix any bugs

---

### Week 4: Testing & Polish

**Day 21-22: End-to-End Testing**
- [ ] Test all happy paths
- [ ] Test error cases
- [ ] Test edge cases
- [ ] Cross-browser testing

**Day 23-24: Bug Fixes**
- [ ] Fix critical bugs
- [ ] Improve error messages
- [ ] Add loading states
- [ ] Polish UI

**Day 25: Documentation**
- [ ] API documentation (Swagger)
- [ ] User guide (screenshots)
- [ ] Setup guide for restaurants

---

## 5. Code Samples

### C# Entity Models

```csharp
// Models/RestaurantTable.cs
public class RestaurantTable
{
    public int Id { get; set; }
    public int ProfileId { get; set; }
    
    public int TableNumber { get; set; }
    public string TableName { get; set; }
    public TableCategory TableCategory { get; set; }
    
    public string FunFact { get; set; }
    public string Description { get; set; }
    public List<string> Images { get; set; }  // Stored as JSON
    
    public string NFCTagCode { get; set; }
    public NFCTagType NFCTagType { get; set; }
    public NFCTagStatus NFCTagStatus { get; set; }
    
    public bool IsActive { get; set; }
    public int SortOrder { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public int? CreatedBy { get; set; }
    
    // Navigation
    public Profile Profile { get; set; }
    public ICollection<NFCScan> NFCScans { get; set; }
}

public enum TableCategory
{
    Regular, VIP, Window, Patio, Private, Bar, Outdoor
}

public enum NFCTagType
{
    Sticker, Disc
}

public enum NFCTagStatus
{
    Active, Inactive, Lost, Damaged
}
```

```csharp
// Models/NFCScan.cs
public class NFCScan
{
    public int Id { get; set; }
    public int ProfileId { get; set; }
    public int? TableId { get; set; }
    
    public string NFCTagCode { get; set; }
    public string IPAddress { get; set; }
    public string UserAgent { get; set; }
    public DeviceType DeviceType { get; set; }
    
    public string Country { get; set; }
    public string City { get; set; }
    
    public DateTime ScannedAt { get; set; }
    public string SessionId { get; set; }
    
    // Navigation
    public Profile Profile { get; set; }
    public RestaurantTable Table { get; set; }
}

public enum DeviceType
{
    Mobile, Tablet, Desktop, Unknown
}
```

### DTOs

```csharp
// DTOs/CreateTableDto.cs
public class CreateTableDto
{
    [Required]
    [Range(1, 9999)]
    public int TableNumber { get; set; }
    
    [MaxLength(100)]
    public string TableName { get; set; }
    
    [Required]
    public string Category { get; set; }  // Regular, VIP, etc.
    
    [MaxLength(5000)]
    public string FunFact { get; set; }
    
    [Required]
    [MaxLength(50)]
    [RegularExpression(@"^[A-Z0-9]{6,50}$")]
    public string NFCTagCode { get; set; }
}
```

```csharp
// DTOs/EventModeDto.cs
public class EventModeDto
{
    [Required]
    public bool Enabled { get; set; }
    
    [MaxLength(100)]
    public string EventName { get; set; }
    
    [MaxLength(1000)]
    public string Description { get; set; }
}
```

---

## 6. Testing Checklist

### Unit Tests

```csharp
// Tests/MenuControllerTests.cs
[Fact]
public async Task GetMenuBySlug_WithNFC_ReturnsTableInfo()
{
    // Arrange
    var slug = "test-restaurant";
    var nfcCode = "T5A3B9";
    
    // Act
    var result = await _controller.GetMenuBySlug(slug, nfcCode);
    
    // Assert
    var okResult = Assert.IsType<OkObjectResult>(result);
    var response = Assert.IsType<MenuResponse>(okResult.Value);
    Assert.NotNull(response.Table);
    Assert.Equal(5, response.Table.Number);
}

[Fact]
public async Task GetMenuBySlug_WithEventMode_FiltersItems()
{
    // Arrange
    var slug = "test-restaurant";
    await EnableEventMode(slug);
    
    // Act
    var result = await _controller.GetMenuBySlug(slug, null);
    
    // Assert
    var response = GetResponse(result);
    Assert.True(response.EventMode.Enabled);
    Assert.All(response.Menu.Items, item => 
        Assert.True(item.AvailableInEventMode)
    );
}
```

### Integration Tests

```csharp
[Fact]
public async Task CreateTable_WithValidData_Succeeds()
{
    // Arrange
    var client = _factory.CreateAuthenticatedClient();
    var dto = new CreateTableDto
    {
        TableNumber = 5,
        TableName = "Window Seat",
        Category = "Window",
        FunFact = "Test fact",
        NFCTagCode = "T5A3B9"
    };
    
    // Act
    var response = await client.PostAsJsonAsync("/api/v1/dashboard/tables", dto);
    
    // Assert
    response.EnsureSuccessStatusCode();
    var table = await response.Content.ReadAsAsync<TableResponse>();
    Assert.Equal(5, table.TableNumber);
}
```

### Manual Testing Checklist

**Customer Flow:**
- [ ] Scan NFC tag with phone (iPhone & Android)
- [ ] Verify table info displays
- [ ] Verify fun fact displays
- [ ] Verify images load
- [ ] Toggle event mode ON in dashboard
- [ ] Scan again - verify event banner shows
- [ ] Verify reduced menu items
- [ ] Toggle event mode OFF
- [ ] Verify full menu restored

**Dashboard Flow:**
- [ ] Create new table
- [ ] Verify table appears in list
- [ ] Verify NFC URL is correct
- [ ] Test duplicate table number (should fail)
- [ ] Test duplicate NFC code (should fail)
- [ ] Edit table fun fact
- [ ] Upload table image
- [ ] Delete table

---

## 7. Environment Setup

### appsettings.json

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=bizbio;User=root;Password=yourpassword;"
  },
  "Jwt": {
    "SecretKey": "your-secret-key-min-32-chars",
    "Issuer": "bizbio.co.za",
    "Audience": "bizbio.co.za",
    "ExpiryMinutes": 1440
  },
  "MenuPro": {
    "MonthlyPrice": 149.00,
    "TrialDays": 14
  }
}
```

### Program.cs Configuration

```csharp
// Add services
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DefaultConnection"))
    )
);

// Add repositories
builder.Services.AddScoped<IRestaurantTableRepository, RestaurantTableRepository>();
builder.Services.AddScoped<INFCScanRepository, NFCScanRepository>();

// Add services
builder.Services.AddScoped<IMenuService, MenuService>();
builder.Services.AddScoped<ITableManagementService, TableManagementService>();
```

---

## Next Steps

### After Week 4 (MVP Complete)

1. **Test with Real Restaurants**
   - Recruit 5 beta testers
   - Give them free access
   - Collect feedback

2. **Order NFC Tags**
   - AliExpress: NTAG213 stickers
   - Order 100 units (~R1,500)
   - Test programming and durability

3. **Set up Marketing**
   - Create demo video
   - Write case study template
   - Build landing page

4. **Prepare for Launch**
   - Set up PayFast
   - Create pricing packages
   - Write Terms of Service

---

## Resources

**NFC Tag Suppliers:**
- AliExpress: Search "NTAG213 NFC sticker"
- Price: R15-25 per tag
- MOQ: 10-50 units

**NFC Programming:**
- iOS: Use "NFC Tools" app (free)
- Android: Use "NFC TagWriter" app (free)
- Write URL: `https://bizbio.co.za/c/restaurant?nfc=T5A3B9`

**Testing Tools:**
- Postman: API testing
- Swagger: API documentation
- Chrome DevTools: Frontend debugging
- ngrok: Test on real phone

---

## Quick Reference

### Database Tables
- `RestaurantTables` - Table info + NFC codes
- `NFCScans` - Analytics
- `Profiles` - Added event mode fields
- `CatalogItems` - Added event availability

### API Endpoints (Phase 1)
- `GET /api/v1/c/{slug}?nfc={code}` - Customer menu view
- `POST /api/v1/dashboard/tables` - Create table
- `GET /api/v1/dashboard/tables` - List tables
- `POST /api/v1/dashboard/event-mode/toggle` - Toggle event mode
- `PUT /api/v1/dashboard/items/{id}/event-availability` - Set item availability

### Frontend Routes
- `/c/:slug` - Customer menu view
- `/dashboard/tables` - Table management
- `/dashboard/event-mode` - Event mode control

---

**Ready to start coding? Begin with Week 1, Day 1: Database Setup! 🚀**

**Questions?** Refer back to the [Complete Technical Spec](./BizBio-NFC-Menu-Pro-Technical-Spec.md) for detailed examples.
