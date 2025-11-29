# BizBio Menu Pro - Technical Specification

**Version:** 1.0  
**Date:** November 2025  
**Feature:** NFC Table Tags + Menu Pro Add-On

---

## Table of Contents

1. [Architecture Overview](#1-architecture-overview)
2. [Database Schema](#2-database-schema)
3. [API Endpoints](#3-api-endpoints)
4. [Data Models](#4-data-models)
5. [Business Logic](#5-business-logic)
6. [Security](#6-security)
7. [NFC Implementation](#7-nfc-implementation)
8. [Frontend Components](#8-frontend-components)
9. [Testing Requirements](#9-testing-requirements)
10. [Performance](#10-performance)

---

## 1. Architecture Overview

### 1.1 System Components

```
┌─────────────────────────────────────────────────────┐
│                  Customer's Phone                    │
│  ┌─────────────────────────────────────────────┐   │
│  │  NFC Chip Reader                             │   │
│  │  Reads: bizbio.co.za/c/restaurant?nfc=T5A3B9│   │
│  └──────────────────┬──────────────────────────┘   │
└─────────────────────┼────────────────────────────────┘
                      │
                      │ HTTPS Request
                      ▼
┌─────────────────────────────────────────────────────┐
│             Vue.js Frontend (SPA)                    │
│  ┌─────────────────────────────────────────────┐   │
│  │  Menu View Component                         │   │
│  │  - Parses ?nfc= parameter                   │   │
│  │  - Fetches menu + table data                │   │
│  │  - Renders table personality                │   │
│  │  - Applies event mode filter                │   │
│  └──────────────────┬──────────────────────────┘   │
└─────────────────────┼────────────────────────────────┘
                      │
                      │ API Call
                      ▼
┌─────────────────────────────────────────────────────┐
│          ASP.NET Core 6 API Backend                  │
│  ┌─────────────────────────────────────────────┐   │
│  │  MenuController                              │   │
│  │  GET /api/v1/c/{slug}?nfc={code}           │   │
│  │  1. Validate restaurant exists               │   │
│  │  2. Look up NFC code → table ID             │   │
│  │  3. Check event mode status                  │   │
│  │  4. Filter menu items                        │   │
│  │  5. Return menu + table data                 │   │
│  └──────────────────┬──────────────────────────┘   │
└─────────────────────┼────────────────────────────────┘
                      │
                      │ Database Queries
                      ▼
┌─────────────────────────────────────────────────────┐
│                 MySQL Database                       │
│  Tables:                                             │
│  - Profiles (restaurant data)                        │
│  - RestaurantTables (table info + NFC codes)        │
│  - CatalogItems (menu items)                        │
│  - NFCScans (analytics)                             │
└─────────────────────────────────────────────────────┘
```

### 1.2 Data Flow

**Customer Scans NFC:**
1. Phone reads NFC chip → URL with `?nfc=` parameter
2. Browser opens URL
3. Vue.js app extracts `nfc` query parameter
4. API call: `GET /api/v1/c/restaurant-slug?nfc=T5A3B9`
5. Backend processes:
   - Validates restaurant exists
   - Looks up NFC code → finds Table 5
   - Checks if Event Mode enabled
   - Filters menu items accordingly
   - Retrieves table fun fact & images
6. Returns JSON response
7. Frontend renders menu + table personality
8. Backend logs scan for analytics

---

## 2. Database Schema

### 2.1 New Tables

#### RestaurantTables

```sql
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
    Images JSON,  -- Array of image URLs: ["url1", "url2", ...]
    
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
    
    -- Indexes
    FOREIGN KEY (ProfileId) REFERENCES Profiles(Id) ON DELETE CASCADE,
    FOREIGN KEY (CreatedBy) REFERENCES Users(Id) ON DELETE SET NULL,
    UNIQUE KEY unique_table_number (ProfileId, TableNumber),
    INDEX idx_nfc_code (NFCTagCode),
    INDEX idx_profile_id (ProfileId),
    INDEX idx_active (IsActive)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
```

#### NFCScans (Analytics)

```sql
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
    
    -- Location (if available from IP)
    Country VARCHAR(2),
    City VARCHAR(100),
    
    -- Timing
    ScannedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    
    -- Session (for unique visitor tracking)
    SessionId VARCHAR(100),
    
    -- Indexes
    FOREIGN KEY (ProfileId) REFERENCES Profiles(Id) ON DELETE CASCADE,
    FOREIGN KEY (TableId) REFERENCES RestaurantTables(Id) ON DELETE SET NULL,
    INDEX idx_profile_scans (ProfileId, ScannedAt),
    INDEX idx_table_scans (TableId, ScannedAt),
    INDEX idx_nfc_code (NFCTagCode)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
```

#### MenuVersions (Event Configurations)

```sql
CREATE TABLE MenuVersions (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    
    -- Relationships
    ProfileId INT NOT NULL,
    
    -- Version Info
    VersionName VARCHAR(100) NOT NULL,
    Description TEXT,
    
    -- Configuration
    ItemsConfiguration JSON,  -- Array of item IDs and their availability
    
    -- Status
    IsActive BOOLEAN DEFAULT FALSE,
    IsTemplate BOOLEAN DEFAULT FALSE,
    
    -- Metadata
    CreatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    CreatedBy INT,
    LastUsedAt TIMESTAMP NULL,
    
    -- Indexes
    FOREIGN KEY (ProfileId) REFERENCES Profiles(Id) ON DELETE CASCADE,
    FOREIGN KEY (CreatedBy) REFERENCES Users(Id) ON DELETE SET NULL,
    INDEX idx_profile_versions (ProfileId),
    INDEX idx_active (IsActive)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
```

### 2.2 Modified Tables

#### Profiles (Add Event Mode Fields)

```sql
ALTER TABLE Profiles ADD COLUMN EventModeEnabled BOOLEAN DEFAULT FALSE;
ALTER TABLE Profiles ADD COLUMN EventModeName VARCHAR(100);
ALTER TABLE Profiles ADD COLUMN EventModeDescription TEXT;
ALTER TABLE Profiles ADD COLUMN ActiveMenuVersionId INT;
ALTER TABLE Profiles ADD COLUMN HasMenuProAddon BOOLEAN DEFAULT FALSE;

ALTER TABLE Profiles ADD FOREIGN KEY (ActiveMenuVersionId) 
    REFERENCES MenuVersions(Id) ON DELETE SET NULL;
ALTER TABLE Profiles ADD INDEX idx_event_mode (EventModeEnabled);
```

#### CatalogItems (Add Event Mode Fields)

```sql
ALTER TABLE CatalogItems ADD COLUMN AvailableInEventMode BOOLEAN DEFAULT TRUE;
ALTER TABLE CatalogItems ADD COLUMN EventModeOnly BOOLEAN DEFAULT FALSE;
ALTER TABLE CatalogItems ADD COLUMN MenuVersionAvailability JSON;  -- {versionId: available}

ALTER TABLE CatalogItems ADD INDEX idx_event_availability (AvailableInEventMode);
```

### 2.3 Sample Data

```sql
-- Sample Restaurant Table
INSERT INTO RestaurantTables (
    ProfileId, TableNumber, TableName, TableCategory,
    FunFact, Images, NFCTagCode, NFCTagType
) VALUES (
    1, 
    5, 
    'Window Seat', 
    'Window',
    'This table was where our first customer sat when we opened in 2015. She ordered our signature margherita pizza and gave us our first 5-star review!',
    '["https://cdn.bizbio.co.za/restaurant1/table5-history.jpg", "https://cdn.bizbio.co.za/restaurant1/table5-view.jpg"]',
    'T5A3B9',
    'Sticker'
);

-- Sample Menu Version
INSERT INTO MenuVersions (
    ProfileId, VersionName, Description, ItemsConfiguration
) VALUES (
    1,
    'Wedding Package',
    'Limited menu for wedding events - fast service',
    '{"1": true, "2": true, "5": true, "12": false, "15": false}'
);
```

---

## 3. API Endpoints

### 3.1 Public Endpoints (Customer-Facing)

#### Get Menu with NFC Context

```http
GET /api/v1/c/{restaurantSlug}?nfc={nfcCode}
```

**Parameters:**
- `restaurantSlug` (path): Restaurant's URL slug
- `nfc` (query, optional): NFC tag code

**Response:**
```json
{
  "success": true,
  "data": {
    "restaurant": {
      "id": 1,
      "name": "Joe's Pizza",
      "slug": "joes-pizza",
      "description": "Authentic wood-fired pizza",
      "logo": "https://cdn.bizbio.co.za/logos/joes.jpg",
      "coverImage": "https://cdn.bizbio.co.za/covers/joes.jpg",
      "contactInfo": {
        "phone": "+27 11 123 4567",
        "email": "info@joespizza.co.za",
        "address": "123 Main St, Sandton"
      },
      "hours": {
        "monday": "11:00-22:00",
        "tuesday": "11:00-22:00"
      }
    },
    "table": {
      "id": 5,
      "number": 5,
      "name": "Window Seat",
      "category": "Window",
      "funFact": "This table was where our first customer sat...",
      "images": [
        "https://cdn.bizbio.co.za/restaurant1/table5-history.jpg",
        "https://cdn.bizbio.co.za/restaurant1/table5-view.jpg"
      ]
    },
    "eventMode": {
      "enabled": true,
      "eventName": "Wedding Package",
      "description": "Limited menu for faster service"
    },
    "menu": {
      "categories": [
        {
          "id": 1,
          "name": "Pizzas",
          "sortOrder": 1,
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
              ],
              "addons": [
                { "name": "Extra cheese", "price": 15.00 },
                { "name": "Olives", "price": 10.00 }
              ],
              "allergens": ["dairy", "gluten"],
              "dietaryTags": ["vegetarian"],
              "availableInEventMode": true
            }
          ]
        }
      ]
    },
    "analytics": {
      "scanLogged": true
    }
  }
}
```

**Business Logic:**
1. Look up restaurant by slug
2. If `nfc` parameter provided:
   - Look up table by NFC code
   - Log scan to NFCScans table
   - Include table info in response
3. Check if event mode enabled
4. Filter menu items based on event mode
5. Return complete response

**Error Responses:**
```json
// Restaurant not found
{
  "success": false,
  "error": {
    "code": "RESTAURANT_NOT_FOUND",
    "message": "Restaurant not found"
  }
}

// Invalid NFC code
{
  "success": false,
  "error": {
    "code": "INVALID_NFC_CODE",
    "message": "This NFC tag is not recognized. Please contact restaurant staff."
  }
}
```

---

### 3.2 Restaurant Dashboard Endpoints

#### Table Management

**List Tables**
```http
GET /api/v1/dashboard/tables
Authorization: Bearer {jwt}
```

**Response:**
```json
{
  "success": true,
  "data": {
    "tables": [
      {
        "id": 1,
        "tableNumber": 5,
        "tableName": "Window Seat",
        "category": "Window",
        "funFact": "This table was...",
        "images": ["url1", "url2"],
        "nfcTagCode": "T5A3B9",
        "nfcTagType": "Sticker",
        "nfcTagStatus": "Active",
        "isActive": true,
        "analytics": {
          "totalScans": 245,
          "scansLast7Days": 18,
          "averageScansPerDay": 2.6
        }
      }
    ],
    "totalTables": 25,
    "nfcTagsAssigned": 25,
    "nfcTagsActive": 24
  }
}
```

---

**Create Table**
```http
POST /api/v1/dashboard/tables
Authorization: Bearer {jwt}
Content-Type: application/json

{
  "tableNumber": 12,
  "tableName": "Chef's Table",
  "category": "VIP",
  "funFact": "Experience the kitchen magic from our exclusive chef's table.",
  "images": [],
  "nfcTagCode": "T12X7Y2"
}
```

**Response:**
```json
{
  "success": true,
  "data": {
    "table": {
      "id": 12,
      "tableNumber": 12,
      "tableName": "Chef's Table",
      "nfcUrl": "https://bizbio.co.za/c/joes-pizza?nfc=T12X7Y2",
      "createdAt": "2025-11-14T10:30:00Z"
    }
  }
}
```

**Validation:**
- Table number must be unique per restaurant
- NFC tag code must be unique globally
- Must have Menu Pro add-on enabled

---

**Update Table**
```http
PUT /api/v1/dashboard/tables/{tableId}
Authorization: Bearer {jwt}
Content-Type: application/json

{
  "tableName": "Chef's Table - Premium",
  "funFact": "Updated fun fact...",
  "images": ["new-url1.jpg"]
}
```

---

**Delete Table**
```http
DELETE /api/v1/dashboard/tables/{tableId}
Authorization: Bearer {jwt}
```

**Response:**
```json
{
  "success": true,
  "message": "Table deleted successfully. NFC tag can be reassigned."
}
```

---

**Upload Table Image**
```http
POST /api/v1/dashboard/tables/{tableId}/images
Authorization: Bearer {jwt}
Content-Type: multipart/form-data

image: [binary file data]
```

**Response:**
```json
{
  "success": true,
  "data": {
    "imageUrl": "https://cdn.bizbio.co.za/uploads/table12-image1.jpg",
    "imageId": "img_abc123"
  }
}
```

**Limits:**
- Max 5 images per table
- Max 5MB per image
- Formats: JPG, PNG, WebP

---

#### Event Mode Management

**Get Event Mode Status**
```http
GET /api/v1/dashboard/event-mode
Authorization: Bearer {jwt}
```

**Response:**
```json
{
  "success": true,
  "data": {
    "enabled": false,
    "eventName": null,
    "description": null,
    "itemsHidden": 0,
    "itemsVisible": 150
  }
}
```

---

**Toggle Event Mode**
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

**Response:**
```json
{
  "success": true,
  "data": {
    "eventMode": {
      "enabled": true,
      "eventName": "Wedding Package",
      "activatedAt": "2025-11-14T18:00:00Z",
      "itemsAvailable": 45,
      "itemsHidden": 105
    }
  },
  "message": "Event mode activated. Customers will now see the Wedding Package menu."
}
```

---

**Configure Item Availability**
```http
PUT /api/v1/dashboard/event-mode/items
Authorization: Bearer {jwt}
Content-Type: application/json

{
  "items": [
    { "itemId": 1, "availableInEventMode": true },
    { "itemId": 2, "availableInEventMode": true },
    { "itemId": 3, "availableInEventMode": false }
  ]
}
```

---

#### Menu Versions

**List Saved Versions**
```http
GET /api/v1/dashboard/menu-versions
Authorization: Bearer {jwt}
```

**Response:**
```json
{
  "success": true,
  "data": {
    "versions": [
      {
        "id": 1,
        "versionName": "Wedding Package",
        "description": "Limited menu for weddings",
        "itemCount": 45,
        "isActive": false,
        "isTemplate": true,
        "lastUsedAt": "2025-11-10T14:00:00Z",
        "createdAt": "2025-10-01T10:00:00Z"
      },
      {
        "id": 2,
        "versionName": "Corporate Events",
        "description": "Business-friendly menu",
        "itemCount": 60,
        "isActive": false,
        "isTemplate": true,
        "lastUsedAt": null,
        "createdAt": "2025-10-15T11:00:00Z"
      }
    ]
  }
}
```

---

**Create Version**
```http
POST /api/v1/dashboard/menu-versions
Authorization: Bearer {jwt}
Content-Type: application/json

{
  "versionName": "Birthday Party",
  "description": "Kid-friendly menu with treats",
  "itemsConfiguration": {
    "1": true,
    "2": true,
    "5": false
  },
  "saveAsTemplate": true
}
```

---

**Activate Version**
```http
POST /api/v1/dashboard/menu-versions/{versionId}/activate
Authorization: Bearer {jwt}
```

**Response:**
```json
{
  "success": true,
  "data": {
    "eventMode": {
      "enabled": true,
      "activeVersion": "Wedding Package",
      "itemsAvailable": 45
    }
  },
  "message": "Wedding Package menu is now active"
}
```

---

#### Analytics

**Table Scan Analytics**
```http
GET /api/v1/dashboard/analytics/table-scans?startDate=2025-11-01&endDate=2025-11-14
Authorization: Bearer {jwt}
```

**Response:**
```json
{
  "success": true,
  "data": {
    "summary": {
      "totalScans": 1250,
      "uniqueVisitors": 890,
      "averageScansPerDay": 89,
      "mostPopularTable": {
        "tableNumber": 5,
        "tableName": "Window Seat",
        "scans": 145
      }
    },
    "tableBreakdown": [
      {
        "tableId": 5,
        "tableNumber": 5,
        "tableName": "Window Seat",
        "scans": 145,
        "uniqueVisitors": 98,
        "averageTimeOnMenu": 245
      }
    ],
    "timeDistribution": {
      "lunch": 450,
      "dinner": 680,
      "other": 120
    },
    "deviceTypes": {
      "mobile": 1100,
      "tablet": 80,
      "desktop": 70
    }
  }
}
```

---

## 4. Data Models

### 4.1 C# Entity Models

#### RestaurantTable Entity

```csharp
public class RestaurantTable
{
    public int Id { get; set; }
    
    // Relationships
    public int ProfileId { get; set; }
    public Profile Profile { get; set; }
    
    // Table Info
    public int TableNumber { get; set; }
    public string TableName { get; set; }
    public TableCategory Category { get; set; }
    
    // Table Personality
    public string FunFact { get; set; }
    public string Description { get; set; }
    public List<string> Images { get; set; }  // JSON stored as List
    
    // NFC Configuration
    public string NFCTagCode { get; set; }
    public NFCTagType NFCTagType { get; set; }
    public NFCTagStatus NFCTagStatus { get; set; }
    
    // Metadata
    public bool IsActive { get; set; } = true;
    public int SortOrder { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public int? CreatedBy { get; set; }
    
    // Navigation
    public ICollection<NFCScan> NFCScans { get; set; }
}

public enum TableCategory
{
    Regular,
    VIP,
    Window,
    Patio,
    Private,
    Bar,
    Outdoor
}

public enum NFCTagType
{
    Sticker,
    Disc
}

public enum NFCTagStatus
{
    Active,
    Inactive,
    Lost,
    Damaged
}
```

#### NFCScan Entity

```csharp
public class NFCScan
{
    public int Id { get; set; }
    
    // Relationships
    public int ProfileId { get; set; }
    public Profile Profile { get; set; }
    
    public int? TableId { get; set; }
    public RestaurantTable Table { get; set; }
    
    // NFC Info
    public string NFCTagCode { get; set; }
    
    // Request Info
    public string IPAddress { get; set; }
    public string UserAgent { get; set; }
    public DeviceType DeviceType { get; set; }
    
    // Location
    public string Country { get; set; }
    public string City { get; set; }
    
    // Timing
    public DateTime ScannedAt { get; set; }
    
    // Session
    public string SessionId { get; set; }
}

public enum DeviceType
{
    Mobile,
    Tablet,
    Desktop,
    Unknown
}
```

#### MenuVersion Entity

```csharp
public class MenuVersion
{
    public int Id { get; set; }
    
    // Relationships
    public int ProfileId { get; set; }
    public Profile Profile { get; set; }
    
    // Version Info
    public string VersionName { get; set; }
    public string Description { get; set; }
    
    // Configuration (JSON stored as Dictionary)
    public Dictionary<int, bool> ItemsConfiguration { get; set; }
    
    // Status
    public bool IsActive { get; set; }
    public bool IsTemplate { get; set; }
    
    // Metadata
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public int? CreatedBy { get; set; }
    public DateTime? LastUsedAt { get; set; }
}
```

---

### 4.2 DTOs (Data Transfer Objects)

#### MenuResponseDto

```csharp
public class MenuResponseDto
{
    public RestaurantDto Restaurant { get; set; }
    public TableDto Table { get; set; }
    public EventModeDto EventMode { get; set; }
    public MenuDto Menu { get; set; }
    public AnalyticsDto Analytics { get; set; }
}

public class TableDto
{
    public int Id { get; set; }
    public int Number { get; set; }
    public string Name { get; set; }
    public string Category { get; set; }
    public string FunFact { get; set; }
    public List<string> Images { get; set; }
}

public class EventModeDto
{
    public bool Enabled { get; set; }
    public string EventName { get; set; }
    public string Description { get; set; }
}
```

#### CreateTableDto

```csharp
public class CreateTableDto
{
    [Required]
    [Range(1, 9999)]
    public int TableNumber { get; set; }
    
    [MaxLength(100)]
    public string TableName { get; set; }
    
    [Required]
    public string Category { get; set; }
    
    [MaxLength(5000)]
    public string FunFact { get; set; }
    
    [MaxLength(1000)]
    public string Description { get; set; }
    
    [MaxLength(50)]
    [RegularExpression(@"^[A-Z0-9]{6,50}$")]
    public string NFCTagCode { get; set; }
}
```

---

## 5. Business Logic

### 5.1 MenuService

```csharp
public class MenuService : IMenuService
{
    private readonly IProfileRepository _profileRepo;
    private readonly IRestaurantTableRepository _tableRepo;
    private readonly ICatalogRepository _catalogRepo;
    private readonly INFCScanRepository _scanRepo;
    private readonly ILogger<MenuService> _logger;
    
    public async Task<MenuResponseDto> GetMenuBySlugAsync(
        string slug, 
        string nfcCode, 
        string ipAddress,
        string userAgent)
    {
        // 1. Get restaurant profile
        var profile = await _profileRepo.GetBySlugAsync(slug);
        if (profile == null)
        {
            throw new NotFoundException("Restaurant not found");
        }
        
        // 2. Handle NFC code if provided
        RestaurantTable table = null;
        if (!string.IsNullOrEmpty(nfcCode))
        {
            table = await _tableRepo.GetByNFCCodeAsync(nfcCode, profile.Id);
            
            if (table == null)
            {
                _logger.LogWarning($"Invalid NFC code attempted: {nfcCode}");
                // Don't throw - just show menu without table info
            }
            else
            {
                // Log the scan for analytics
                await LogNFCScanAsync(profile.Id, table.Id, nfcCode, ipAddress, userAgent);
            }
        }
        
        // 3. Get menu catalog
        var catalog = await _catalogRepo.GetByProfileIdAsync(profile.Id);
        var items = await _catalogRepo.GetItemsWithDetailsAsync(catalog.Id);
        
        // 4. Filter items based on event mode
        if (profile.EventModeEnabled)
        {
            items = items.Where(i => i.AvailableInEventMode).ToList();
        }
        
        // 5. Build response
        var response = new MenuResponseDto
        {
            Restaurant = MapToRestaurantDto(profile),
            Table = table != null ? MapToTableDto(table) : null,
            EventMode = new EventModeDto
            {
                Enabled = profile.EventModeEnabled,
                EventName = profile.EventModeName,
                Description = profile.EventModeDescription
            },
            Menu = MapToMenuDto(items),
            Analytics = new AnalyticsDto { ScanLogged = table != null }
        };
        
        return response;
    }
    
    private async Task LogNFCScanAsync(
        int profileId, 
        int tableId, 
        string nfcCode,
        string ipAddress,
        string userAgent)
    {
        var scan = new NFCScan
        {
            ProfileId = profileId,
            TableId = tableId,
            NFCTagCode = nfcCode,
            IPAddress = ipAddress,
            UserAgent = userAgent,
            DeviceType = DetermineDeviceType(userAgent),
            ScannedAt = DateTime.UtcNow,
            SessionId = GenerateSessionId()
        };
        
        // Optionally: Enrich with geo-location data
        await EnrichWithLocationDataAsync(scan, ipAddress);
        
        await _scanRepo.AddAsync(scan);
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
}
```

---

### 5.2 TableManagementService

```csharp
public class TableManagementService : ITableManagementService
{
    private readonly IRestaurantTableRepository _tableRepo;
    private readonly IProfileRepository _profileRepo;
    private readonly ISubscriptionService _subscriptionService;
    
    public async Task<RestaurantTable> CreateTableAsync(
        int userId, 
        int profileId, 
        CreateTableDto dto)
    {
        // 1. Verify user owns this profile
        var profile = await _profileRepo.GetByIdAsync(profileId);
        if (profile.UserId != userId)
        {
            throw new ForbiddenException("You don't have access to this restaurant");
        }
        
        // 2. Check if Menu Pro add-on is enabled
        if (!profile.HasMenuProAddon)
        {
            throw new BusinessException(
                "Menu Pro add-on required for advanced table features. " +
                "Upgrade to add table personalities and event mode."
            );
        }
        
        // 3. Check if table number already exists
        var existingTable = await _tableRepo
            .GetByTableNumberAsync(profileId, dto.TableNumber);
        if (existingTable != null)
        {
            throw new BusinessException(
                $"Table {dto.TableNumber} already exists. " +
                "Please choose a different table number."
            );
        }
        
        // 4. Validate NFC code is unique (if provided)
        if (!string.IsNullOrEmpty(dto.NFCTagCode))
        {
            var nfcExists = await _tableRepo
                .NFCCodeExistsAsync(dto.NFCTagCode);
            if (nfcExists)
            {
                throw new BusinessException(
                    $"NFC code {dto.NFCTagCode} is already in use. " +
                    "Please check the code or contact support."
                );
            }
        }
        
        // 5. Create table
        var table = new RestaurantTable
        {
            ProfileId = profileId,
            TableNumber = dto.TableNumber,
            TableName = dto.TableName,
            Category = Enum.Parse<TableCategory>(dto.Category),
            FunFact = dto.FunFact,
            Description = dto.Description,
            NFCTagCode = dto.NFCTagCode,
            NFCTagType = NFCTagType.Sticker,  // Default
            NFCTagStatus = NFCTagStatus.Active,
            IsActive = true,
            CreatedBy = userId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        
        await _tableRepo.AddAsync(table);
        await _tableRepo.SaveChangesAsync();
        
        return table;
    }
    
    public async Task ToggleEventModeAsync(
        int userId,
        int profileId,
        bool enabled,
        string eventName,
        string description)
    {
        var profile = await _profileRepo.GetByIdAsync(profileId);
        
        // Verify ownership
        if (profile.UserId != userId)
        {
            throw new ForbiddenException();
        }
        
        // Check Menu Pro
        if (!profile.HasMenuProAddon)
        {
            throw new BusinessException("Menu Pro required for Event Mode");
        }
        
        // Toggle
        profile.EventModeEnabled = enabled;
        profile.EventModeName = enabled ? eventName : null;
        profile.EventModeDescription = enabled ? description : null;
        profile.UpdatedAt = DateTime.UtcNow;
        
        await _profileRepo.UpdateAsync(profile);
        await _profileRepo.SaveChangesAsync();
        
        // Log event mode change
        _logger.LogInformation(
            $"Event mode {(enabled ? "enabled" : "disabled")} " +
            $"for profile {profileId} by user {userId}"
        );
    }
}
```

---

## 6. Security

### 6.1 Authentication & Authorization

**Menu Viewing (Public):**
- No authentication required
- Rate limiting: 100 requests/hour per IP

**Table Management (Protected):**
- JWT required
- User must own the profile
- Menu Pro add-on must be enabled

**NFC Code Security:**
- Codes are random, alphanumeric (e.g., T5A3B9)
- Globally unique
- Not sequential (prevents guessing)
- Can be deactivated if compromised

---

### 6.2 Input Validation

**NFC Code Format:**
```csharp
[RegularExpression(@"^[A-Z0-9]{6,50}$")]
```
- 6-50 characters
- Alphanumeric only
- Uppercase

**Table Number:**
```csharp
[Range(1, 9999)]
```

**Fun Fact Length:**
```csharp
[MaxLength(5000)]
```

**Images:**
- Max 5 per table
- Max 5MB per image
- Allowed types: JPG, PNG, WebP
- Virus scanning on upload

---

### 6.3 Data Privacy

**PII Handling:**
- IP addresses are hashed after 30 days
- User agents stored for analytics only
- No personally identifiable customer data collected

**POPIA Compliance:**
- Customers can request scan data deletion
- Restaurant owners can export all data
- Data retention: 12 months max

---

## 7. NFC Implementation

### 7.1 NFC Tag Programming

**Tag Type:** NTAG213/215/216 (ISO14443A)

**Data Written to Tag:**
```
URL: https://bizbio.co.za/c/restaurant-slug?nfc=T5A3B9
```

**Programming Process:**
1. Generate unique code (6-character alphanumeric)
2. Build URL with restaurant slug + NFC code
3. Write URL to NFC tag using NFC writer app
4. Print code on physical label/tag
5. Ship to customer
6. Customer links code to table in dashboard

---

### 7.2 NFC Tag Specifications

**Memory:**
- NTAG213: 144 bytes (sufficient for URLs)
- NTAG215: 504 bytes (for future features)

**Read Distance:**
- 1-4cm (secure, intentional taps only)

**Compatibility:**
- All NFC-enabled smartphones
- No app required (opens in browser)

**Durability:**
- Stickers: 3-5 years
- Discs: 5-7 years
- Waterproof ratings

---

## 8. Frontend Components

### 8.1 Customer-Facing Components

#### MenuView.vue

```vue
<template>
  <div class="menu-page">
    <!-- Table Personality Banner (if NFC scan) -->
    <TablePersonality 
      v-if="tableInfo"
      :table="tableInfo"
      :restaurant="restaurant"
    />
    
    <!-- Event Mode Banner -->
    <EventModeBanner 
      v-if="eventMode.enabled"
      :event-name="eventMode.eventName"
      :description="eventMode.description"
    />
    
    <!-- Menu Content -->
    <MenuCategories 
      :categories="menuCategories"
      :cart-enabled="restaurant.cartEnabled"
    />
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { useRoute } from 'vue-router'
import { useMenuStore } from '@/stores/menu'

const route = useRoute()
const menuStore = useMenuStore()

const restaurant = ref(null)
const tableInfo = ref(null)
const eventMode = ref({ enabled: false })
const menuCategories = ref([])

onMounted(async () => {
  const slug = route.params.slug
  const nfcCode = route.query.nfc
  
  try {
    const response = await menuStore.fetchMenu(slug, nfcCode)
    
    restaurant.value = response.restaurant
    tableInfo.value = response.table
    eventMode.value = response.eventMode
    menuCategories.value = response.menu.categories
    
    // Track view
    if (nfcCode) {
      trackNFCScan(slug, nfcCode)
    }
  } catch (error) {
    console.error('Failed to load menu:', error)
  }
})
</script>
```

#### TablePersonality.vue

```vue
<template>
  <div class="table-personality-banner">
    <div class="table-info">
      <h3>📍 You're at Table {{ table.number }}</h3>
      <p v-if="table.name" class="table-name">{{ table.name }}</p>
    </div>
    
    <div v-if="table.funFact" class="fun-fact">
      <h4>💡 Did You Know?</h4>
      <p>{{ table.funFact }}</p>
    </div>
    
    <div v-if="table.images?.length" class="table-images">
      <img 
        v-for="(image, index) in table.images" 
        :key="index"
        :src="image" 
        :alt="`Table ${table.number} - Image ${index + 1}`"
        class="table-image"
        @click="openLightbox(image)"
      />
    </div>
    
    <div class="view-menu-cta">
      <button @click="scrollToMenu" class="btn-primary">
        View Menu
      </button>
    </div>
  </div>
</template>

<script setup>
import { defineProps } from 'vue'

const props = defineProps({
  table: Object,
  restaurant: Object
})

const scrollToMenu = () => {
  document.querySelector('.menu-categories')?.scrollIntoView({ 
    behavior: 'smooth' 
  })
}

const openLightbox = (image) => {
  // Open image in lightbox/modal
}
</script>

<style scoped>
.table-personality-banner {
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  color: white;
  padding: 2rem;
  border-radius: 12px;
  margin-bottom: 2rem;
}

.table-info h3 {
  font-size: 1.5rem;
  margin-bottom: 0.5rem;
}

.table-name {
  font-size: 1.1rem;
  opacity: 0.9;
}

.fun-fact {
  background: rgba(255, 255, 255, 0.1);
  padding: 1.5rem;
  border-radius: 8px;
  margin: 1.5rem 0;
}

.fun-fact h4 {
  font-size: 1.2rem;
  margin-bottom: 0.75rem;
}

.table-images {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(150px, 1fr));
  gap: 1rem;
  margin: 1.5rem 0;
}

.table-image {
  width: 100%;
  height: 150px;
  object-fit: cover;
  border-radius: 8px;
  cursor: pointer;
  transition: transform 0.2s;
}

.table-image:hover {
  transform: scale(1.05);
}

.view-menu-cta {
  margin-top: 1.5rem;
  text-align: center;
}

.btn-primary {
  background: white;
  color: #667eea;
  padding: 1rem 2rem;
  border-radius: 8px;
  font-weight: 600;
  font-size: 1.1rem;
  border: none;
  cursor: pointer;
  transition: transform 0.2s;
}

.btn-primary:hover {
  transform: translateY(-2px);
}
</style>
```

---

### 8.2 Dashboard Components

#### TableManagement.vue

```vue
<template>
  <div class="table-management">
    <div class="header">
      <h2>Manage Tables</h2>
      <button @click="showCreateModal = true" class="btn-primary">
        + Add New Table
      </button>
    </div>
    
    <div class="tables-grid">
      <TableCard
        v-for="table in tables"
        :key="table.id"
        :table="table"
        @edit="editTable"
        @delete="deleteTable"
      />
    </div>
    
    <!-- Create/Edit Modal -->
    <TableModal
      v-if="showCreateModal || editingTable"
      :table="editingTable"
      @save="saveTable"
      @close="closeModal"
    />
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { useTableStore } from '@/stores/tables'

const tableStore = useTableStore()
const tables = ref([])
const showCreateModal = ref(false)
const editingTable = ref(null)

onMounted(async () => {
  await loadTables()
})

const loadTables = async () => {
  tables.value = await tableStore.fetchTables()
}

const editTable = (table) => {
  editingTable.value = table
}

const deleteTable = async (tableId) => {
  if (confirm('Delete this table? The NFC tag can be reassigned.')) {
    await tableStore.deleteTable(tableId)
    await loadTables()
  }
}

const saveTable = async (tableData) => {
  if (editingTable.value) {
    await tableStore.updateTable(editingTable.value.id, tableData)
  } else {
    await tableStore.createTable(tableData)
  }
  await loadTables()
  closeModal()
}

const closeModal = () => {
  showCreateModal.value = false
  editingTable.value = null
}
</script>
```

---

## 9. Testing Requirements

### 9.1 Unit Tests

**TableManagementService Tests:**
```csharp
[Fact]
public async Task CreateTable_WithValidData_CreatesSuccessfully()
{
    // Arrange
    var dto = new CreateTableDto
    {
        TableNumber = 5,
        TableName = "Window Seat",
        Category = "Window",
        FunFact = "Test fact",
        NFCTagCode = "T5A3B9"
    };
    
    // Act
    var result = await _service.CreateTableAsync(userId, profileId, dto);
    
    // Assert
    Assert.NotNull(result);
    Assert.Equal(5, result.TableNumber);
    Assert.Equal("T5A3B9", result.NFCTagCode);
}

[Fact]
public async Task CreateTable_DuplicateTableNumber_ThrowsException()
{
    // Arrange - create first table
    await CreateTableAsync(tableNumber: 5);
    
    // Act & Assert
    await Assert.ThrowsAsync<BusinessException>(
        () => CreateTableAsync(tableNumber: 5)
    );
}

[Fact]
public async Task CreateTable_WithoutMenuPro_ThrowsException()
{
    // Arrange
    _profile.HasMenuProAddon = false;
    
    // Act & Assert
    await Assert.ThrowsAsync<BusinessException>(
        () => _service.CreateTableAsync(userId, profileId, dto)
    );
}
```

---

### 9.2 Integration Tests

```csharp
[Fact]
public async Task GetMenu_WithNFCCode_ReturnsTableInfo()
{
    // Arrange
    var client = _factory.CreateClient();
    var table = await CreateTestTableAsync(nfcCode: "T5A3B9");
    
    // Act
    var response = await client.GetAsync(
        "/api/v1/c/test-restaurant?nfc=T5A3B9"
    );
    
    // Assert
    response.EnsureSuccessStatusCode();
    var menu = await response.Content.ReadAsAsync<MenuResponseDto>();
    
    Assert.NotNull(menu.Table);
    Assert.Equal(5, menu.Table.Number);
    Assert.Equal("Window Seat", menu.Table.Name);
    Assert.NotNull(menu.Table.FunFact);
}

[Fact]
public async Task GetMenu_WithEventMode_FiltersItems()
{
    // Arrange
    await EnableEventModeAsync(profileId);
    await SetItemAvailability(itemId: 1, available: true);
    await SetItemAvailability(itemId: 2, available: false);
    
    var client = _factory.CreateClient();
    
    // Act
    var response = await client.GetAsync("/api/v1/c/test-restaurant");
    var menu = await response.Content.ReadAsAsync<MenuResponseDto>();
    
    // Assert
    Assert.True(menu.EventMode.Enabled);
    Assert.Contains(menu.Menu.Categories.SelectMany(c => c.Items), 
        i => i.Id == 1);
    Assert.DoesNotContain(menu.Menu.Categories.SelectMany(c => c.Items), 
        i => i.Id == 2);
}
```

---

### 9.3 End-to-End Tests

**Scenario: Customer Scans NFC, Views Table Story**

```javascript
// Cypress test
describe('NFC Table Scan', () => {
  it('should show table personality when scanning NFC', () => {
    // Visit menu with NFC code
    cy.visit('/c/test-restaurant?nfc=T5A3B9')
    
    // Should show table banner
    cy.contains('You\'re at Table 5').should('be.visible')
    cy.contains('Window Seat').should('be.visible')
    
    // Should show fun fact
    cy.contains('Did You Know?').should('be.visible')
    cy.contains('This table was where').should('be.visible')
    
    // Should show table images
    cy.get('.table-image').should('have.length.greaterThan', 0)
    
    // Should be able to view menu
    cy.contains('View Menu').click()
    cy.get('.menu-categories').should('be.visible')
  })
  
  it('should filter menu in event mode', () => {
    // Enable event mode
    cy.login('restaurant@example.com', 'password')
    cy.visit('/dashboard/event-mode')
    cy.get('[data-testid="event-mode-toggle"]').click()
    cy.contains('Event mode activated').should('be.visible')
    
    // Visit menu
    cy.visit('/c/test-restaurant')
    
    // Should show event banner
    cy.contains('Wedding Package Menu').should('be.visible')
    
    // Should only show available items
    cy.contains('Margherita Pizza').should('be.visible')
    cy.contains('Hawaiian Pizza').should('not.exist')
  })
})
```

---

## 10. Performance

### 10.1 Optimization Strategies

**Database:**
- Index on NFCTagCode for fast lookups
- Index on (ProfileId, EventModeEnabled) for filtering
- Caching of restaurant profiles (Redis, 1-hour TTL)

**API:**
- Response caching for menus (30 seconds)
- CDN for images
- Compress JSON responses (gzip)

**Frontend:**
- Lazy load table images
- Prefetch menu data
- Service Worker for offline capability

---

### 10.2 Performance Targets

| Metric | Target | Notes |
|--------|--------|-------|
| Menu API response time | <200ms | p95 |
| Image load time | <500ms | First contentful paint |
| NFC scan to menu display | <2 seconds | End-to-end |
| Dashboard load time | <1 second | Initial load |

---

### 10.3 Scalability

**Expected Load:**
- 100 restaurants × 30 tables = 3,000 NFC tags
- 50 scans/table/day = 150,000 scans/day
- Peak: 10 scans/second

**Database Capacity:**
- RestaurantTables: ~5KB/row × 3,000 = 15MB
- NFCScans: ~1KB/row × 5M/year = 5GB/year
- Scaling strategy: Partition NFCScans by month

---

**End of Technical Specification**

**Version:** 1.0  
**Status:** Ready for Implementation  
**Complexity:** Medium  
**Estimated Development Time:** 3-4 weeks
