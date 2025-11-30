# Products and Add-Ons System

## Overview

The BizBio platform now has a comprehensive product catalog system that separates **Products** from **Subscriptions**. Products can be physical items (NFC tags, wristbands), services, digital products, or subscription plans. Each product can have specific **Add-Ons** which can also be physical items or additional services.

## Table of Contents

- [Product Types](#product-types)
- [Product Categories](#product-categories)
- [Add-On Types](#add-on-types)
- [Database Schema](#database-schema)
- [API Endpoints](#api-endpoints)
- [Caching Strategy](#caching-strategy)
- [Usage Examples](#usage-examples)

## Product Types

Products are categorized by type using the `ProductType` enum:

```csharp
public enum ProductType
{
    Subscription = 1,      // Recurring subscription plans
    PhysicalProduct = 2,   // Physical items (NFC tags, wristbands, etc.)
    Service = 3,           // One-time or recurring services
    DigitalProduct = 4,    // Digital downloads, assets
    AddOn = 5              // Product add-ons
}
```

## Product Categories

Products are further organized by category using the `ProductCategory` enum:

### Subscription Categories
- `SubscriptionPlan` - Subscription tier plans

### Physical Product Categories
- `NFCTag` - NFC tags
- `NFCCard` - NFC business cards
- `NFCWristband` - NFC wristbands
- `NFCSticker` - NFC stickers
- `QRCodeProduct` - QR code products
- `PrintedMaterial` - Printed materials
- `Merchandise` - Branded merchandise

### Service Categories
- `Setup` - Setup services
- `CustomDesign` - Custom design services
- `Training` - Training services
- `Support` - Support packages
- `Consulting` - Consulting services

### Digital Product Categories
- `DigitalAsset` - Digital assets
- `Template` - Templates
- `Theme` - Themes

### Add-On Categories
- `FeatureAddOn` - Feature enhancements
- `StorageAddOn` - Additional storage
- `UserAddOn` - Additional users
- `IntegrationAddOn` - Third-party integrations

## Add-On Types

Add-ons are categorized using the `AddOnType` enum:

### Service Add-Ons
- `AdditionalStorage` - Extra storage space
- `AdditionalUsers` - Extra user seats
- `PrioritySupport` - Priority support
- `CustomBranding` - Custom branding features
- `WhiteLabel` - White label solution

### Physical Add-Ons
- `ExtraNFCTags` - Additional NFC tags
- `ReplacementCards` - Replacement cards
- `AdditionalWristbands` - Additional wristbands

### Feature Add-Ons
- `AdvancedAnalytics` - Advanced analytics features
- `APIAccess` - API access
- `CustomDomain` - Custom domain
- `MultiLocation` - Multi-location support
- `TeamManagement` - Team management features
- `SSOIntegration` - Single Sign-On integration

## Database Schema

### Products Table

```sql
CREATE TABLE Products (
    Id INT PRIMARY KEY AUTO_INCREMENT,
    Name VARCHAR(200) NOT NULL,
    SKU VARCHAR(100) NOT NULL UNIQUE,
    Description VARCHAR(500),
    LongDescription VARCHAR(2000),
    ProductTypeId INT NOT NULL,
    ProductCategoryId INT NOT NULL,
    ProductLineId INT,

    -- Pricing
    Price DECIMAL(10,2),
    MonthlyPrice DECIMAL(10,2),
    AnnualPrice DECIMAL(10,2),
    AnnualDiscountPercent DECIMAL(5,2),
    SalePrice DECIMAL(10,2),
    CostPrice DECIMAL(10,2),

    -- Inventory
    StockQuantity INT,
    LowStockThreshold INT,
    TrackInventory BOOLEAN,

    -- Physical Details
    Weight VARCHAR(50),
    Dimensions VARCHAR(100),
    Color VARCHAR(50),
    Material VARCHAR(50),

    -- Digital/Service Details
    DurationDays INT,
    IsRecurring BOOLEAN,
    RequiresShipping BOOLEAN,
    IsDigitalDelivery BOOLEAN,

    -- Images
    ImageUrl VARCHAR(500),
    ThumbnailUrl VARCHAR(500),
    ImageGallery TEXT,

    -- Display
    DisplayOrder INT,
    IsActive BOOLEAN,
    IsFeatured BOOLEAN,
    IsPopular BOOLEAN,
    Tags VARCHAR(500),
    MetaTitle VARCHAR(200),
    MetaDescription VARCHAR(500),

    CreatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,

    FOREIGN KEY (ProductTypeId) REFERENCES ProductTypes(Id),
    FOREIGN KEY (ProductCategoryId) REFERENCES ProductCategories(Id),
    FOREIGN KEY (ProductLineId) REFERENCES ProductLines(Id)
);
```

### ProductAddOns Table

```sql
CREATE TABLE ProductAddOns (
    Id INT PRIMARY KEY AUTO_INCREMENT,
    Name VARCHAR(100) NOT NULL,
    SKU VARCHAR(50) NOT NULL UNIQUE,
    Description VARCHAR(500),
    AddOnTypeId INT NOT NULL,
    ProductId INT,  -- NULL for global add-ons

    -- Pricing
    Price DECIMAL(10,2),
    MonthlyPrice DECIMAL(10,2),
    AnnualPrice DECIMAL(10,2),
    IsRecurring BOOLEAN,

    -- Quantity
    DefaultQuantity INT,
    MinQuantity INT,
    MaxQuantity INT,
    Unit VARCHAR(50),

    -- Inventory
    StockQuantity INT,
    TrackInventory BOOLEAN,
    RequiresShipping BOOLEAN,

    -- Images
    ImageUrl VARCHAR(500),

    -- Display
    DisplayOrder INT,
    IsActive BOOLEAN,
    IsFeatured BOOLEAN,

    CreatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,

    FOREIGN KEY (AddOnTypeId) REFERENCES AddOnTypes(Id),
    FOREIGN KEY (ProductId) REFERENCES Products(Id) ON DELETE SET NULL
);
```

### ProductAddOnMappings Table (Many-to-Many)

```sql
CREATE TABLE ProductAddOnMappings (
    ProductId INT NOT NULL,
    ProductAddOnId INT NOT NULL,
    PRIMARY KEY (ProductId, ProductAddOnId),
    FOREIGN KEY (ProductId) REFERENCES Products(Id),
    FOREIGN KEY (ProductAddOnId) REFERENCES ProductAddOns(Id)
);
```

## API Endpoints

All endpoints return cached results for optimal performance.

### Product Endpoints

#### Get All Active Products
```http
GET /api/v1/products
```

**Response:**
```json
{
  "success": true,
  "data": {
    "products": [...],
    "count": 15
  }
}
```

#### Get Product by ID
```http
GET /api/v1/products/{id}
```

#### Get Product by SKU
```http
GET /api/v1/products/sku/{sku}
```

#### Get Products by Type
```http
GET /api/v1/products/type/{type}
```

**Type values:**
- `subscription`
- `physicalproduct`
- `service`
- `digitalproduct`
- `addon`

**Example:**
```http
GET /api/v1/products/type/physicalproduct
```

#### Get Products by Category
```http
GET /api/v1/products/category/{category}
```

**Category values:**
- `nfctag`
- `nfccard`
- `nfcwristband`
- `customdesign`
- `setup`
- etc.

**Example:**
```http
GET /api/v1/products/category/nfctag
```

#### Get Physical Products
```http
GET /api/v1/products/physical
```

Returns all physical products (NFC tags, wristbands, cards, etc.)

#### Get Subscription Products
```http
GET /api/v1/products/subscriptions
```

Returns all subscription plan products.

#### Get Featured Products
```http
GET /api/v1/products/featured
```

Returns all featured products.

### Add-On Endpoints

#### Get All Active Add-Ons
```http
GET /api/v1/products/add-ons
```

#### Get Add-On by ID
```http
GET /api/v1/products/add-ons/{id}
```

#### Get Add-Ons for Specific Product
```http
GET /api/v1/products/{productId}/add-ons
```

Returns all add-ons specific to a product.

#### Get Global Add-Ons
```http
GET /api/v1/products/add-ons/global
```

Returns add-ons available to all products.

#### Get Physical Add-Ons
```http
GET /api/v1/products/add-ons/physical
```

Returns physical add-ons (extra tags, replacement cards, etc.)

#### Get Service Add-Ons
```http
GET /api/v1/products/add-ons/services
```

Returns service add-ons (storage, users, features, etc.)

## Caching Strategy

All product and add-on queries are cached for **1 hour** to improve performance:

### Cache Keys

**Products:**
- `product_{id}` - Individual product
- `product_sku_{sku}` - Product by SKU
- `products_all` - All products
- `products_all_active` - All active products
- `products_type_{typeId}` - Products by type
- `products_category_{categoryId}` - Products by category
- `products_line_{lineId}` - Products by product line
- `products_featured` - Featured products
- `products_physical` - Physical products
- `products_subscription` - Subscription products

**Add-Ons:**
- `addon_{id}` - Individual add-on
- `addon_sku_{sku}` - Add-on by SKU
- `addons_all` - All add-ons
- `addons_all_active` - All active add-ons
- `addons_product_{productId}` - Add-ons for specific product
- `addons_type_{typeId}` - Add-ons by type
- `addons_global` - Global add-ons
- `addons_physical` - Physical add-ons
- `addons_service` - Service add-ons

### Cache Invalidation

Cache is automatically invalidated when:
- A product is created or updated
- An add-on is created or updated
- Related entities are modified

## Usage Examples

### Frontend: Display Physical Products

```javascript
// Fetch all physical products
const response = await fetch('/api/v1/products/physical');
const data = await response.json();

if (data.success) {
  const products = data.data.products;
  products.forEach(product => {
    console.log(`${product.name} - ${product.price}`);
    console.log(`Stock: ${product.stockQuantity}`);
    console.log(`Category: ${product.productCategory.name}`);
  });
}
```

### Frontend: Display Product with Add-Ons

```javascript
// Fetch product
const productResponse = await fetch('/api/v1/products/1');
const product = (await productResponse.json()).data;

// Fetch product-specific add-ons
const addOnsResponse = await fetch(`/api/v1/products/${product.id}/add-ons`);
const addOns = (await addOnsResponse.json()).data.addOns;

// Fetch global add-ons
const globalAddOnsResponse = await fetch('/api/v1/products/add-ons/global');
const globalAddOns = (await globalAddOnsResponse.json()).data.addOns;

console.log(`Product: ${product.name}`);
console.log('Available Add-Ons:');
[...addOns, ...globalAddOns].forEach(addon => {
  console.log(`  - ${addon.name}: $${addon.price}`);
});
```

### Backend: Create a Product

```csharp
var product = new Product
{
    Name = "Premium NFC Business Card",
    SKU = "NFC-CARD-001",
    Description = "Bamboo NFC business card with custom engraving",
    ProductTypeId = (int)ProductType.PhysicalProduct,
    ProductCategoryId = (int)ProductCategory.NFCCard,
    Price = 29.99m,
    StockQuantity = 100,
    TrackInventory = true,
    RequiresShipping = true,
    Weight = "10g",
    Material = "Bamboo",
    IsActive = true,
    DisplayOrder = 1
};

await _productRepo.AddAsync(product);
await _productRepo.SaveChangesAsync();
```

### Backend: Create an Add-On

```csharp
// Product-specific add-on
var productAddOn = new ProductAddOn
{
    Name = "Extra NFC Tags - Pack of 10",
    SKU = "ADDON-TAGS-10",
    Description = "10 additional NFC tags",
    AddOnTypeId = (int)AddOnType.ExtraNFCTags,
    ProductId = 1, // Specific to product ID 1
    Price = 19.99m,
    DefaultQuantity = 10,
    StockQuantity = 50,
    TrackInventory = true,
    RequiresShipping = true,
    IsActive = true
};

// Global add-on (available to all products)
var globalAddOn = new ProductAddOn
{
    Name = "Additional 10GB Storage",
    SKU = "ADDON-STORAGE-10GB",
    Description = "Add 10GB of storage to your account",
    AddOnTypeId = (int)AddOnType.AdditionalStorage,
    ProductId = null, // NULL = global add-on
    MonthlyPrice = 9.99m,
    IsRecurring = true,
    DefaultQuantity = 10,
    Unit = "GB",
    IsActive = true
};

await _addOnRepo.AddAsync(productAddOn);
await _addOnRepo.AddAsync(globalAddOn);
await _addOnRepo.SaveChangesAsync();
```

## Product vs Subscription Relationship

**SubscriptionTier** remains as the detailed configuration for subscription features and limits. A **Product** of type `Subscription` can reference a `SubscriptionTier` to get its pricing and features.

This separation allows:
1. Physical products (NFC tags, wristbands) to exist independently
2. Services (setup, design) to be sold separately
3. Add-ons to enhance any product type
4. Subscription plans to have detailed tier configurations
5. Unified product catalog for e-commerce

## Best Practices

1. **Use Product Categories Appropriately**: Choose the correct category for proper filtering
2. **Set SKUs Uniquely**: SKUs must be unique across all products
3. **Track Inventory for Physical Products**: Set `TrackInventory = true` and maintain `StockQuantity`
4. **Use Global Add-Ons for Common Features**: Storage, users, support can be global
5. **Cache Aggressively**: All read operations are cached for performance
6. **Include Images**: Use `ImageUrl` for thumbnails, `ImageGallery` for multiple images
7. **Set Display Order**: Control product listing order with `DisplayOrder`
8. **Mark Popular/Featured**: Use flags to highlight important products

## Migration Notes

When migrating to this system:

1. Run database migration to create tables
2. Seed lookup tables with enum values
3. Create products for existing items (NFC tags, etc.)
4. Link subscription tiers to products if needed
5. Create add-ons for customization options
6. Update frontend to use new product endpoints

## Performance Considerations

- All queries are cached for 1 hour
- Lookup tables are small and heavily cached
- Use indexes on SKU, ProductTypeId, ProductCategoryId
- Consider CDN for product images
- Paginate large product lists if needed

## Future Enhancements

- Product reviews and ratings
- Product bundles/kits
- Discount codes and promotions
- Product variants (sizes, colors)
- Related products
- Product recommendations
- Inventory alerts
- Back-order management
