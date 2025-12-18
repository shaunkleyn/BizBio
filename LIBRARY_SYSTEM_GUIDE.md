# Menu Item Library System - Complete Guide

## Overview

The Library System allows you to manage all your menu items, categories, variants, and allergen information **independently** from your menus. You can:

1. ✅ Add items without creating a menu
2. ✅ Manage variants (sizes/options) for each item
3. ✅ Track allergens and dietary restrictions
4. ✅ Organize items into categories
5. ✅ Reuse items across multiple menus
6. ✅ Update library items and sync changes to menus

---

## Database Changes

### CatalogItem Entity
Added support for library items:
- `CatalogId` (nullable) - null means it's a library item
- `UserId` (nullable) - owner of library item
- `Tags` (string, JSON) - allergens/dietary info
- `SourceLibraryItemId` (nullable) - tracks copied items

### CatalogCategory Entity
Added support for library categories:
- `CatalogId` (nullable) - null means it's a library category
- `UserId` (nullable) - owner of library category

---

## API Endpoints

### Library Items (`/api/v1/library/items`)

#### Get All Library Items
```http
GET /api/v1/library/items?categoryId={optional}
```
Returns all items in your library, optionally filtered by category.

**Response:**
```json
{
  "success": true,
  "data": {
    "items": [
      {
        "id": 1,
        "name": "Margherita Pizza",
        "description": "Classic tomato and mozzarella",
        "price": 89.99,
        "categoryId": 5,
        "itemType": 0,
        "images": ["url1", "url2"],
        "tags": ["Vegetarian", "Gluten-Free"],
        "variantCount": 3,
        "variants": [
          {
            "id": 1,
            "title": "Small",
            "price": 89.99,
            "isDefault": true
          }
        ]
      }
    ]
  }
}
```

#### Get Single Item
```http
GET /api/v1/library/items/{id}
```

#### Create New Item
```http
POST /api/v1/library/items
Content-Type: application/json

{
  "name": "Margherita Pizza",
  "description": "Classic tomato and mozzarella",
  "price": 89.99,
  "categoryId": 5,
  "tags": ["Vegetarian", "Dairy"],
  "variants": [
    {
      "title": "Small",
      "price": 89.99,
      "isDefault": true
    },
    {
      "title": "Medium",
      "price": 119.99,
      "isDefault": false
    },
    {
      "title": "Large",
      "price": 149.99,
      "isDefault": false
    }
  ]
}
```

#### Update Item
```http
PUT /api/v1/library/items/{id}
Content-Type: application/json

{
  "name": "Updated Name",
  "price": 99.99,
  "tags": ["Vegan", "Nut-Free"]
}
```

#### Delete Item
```http
DELETE /api/v1/library/items/{id}
```
Soft deletes the item (sets `IsActive = false`).

#### Add Item to Catalog/Menu
```http
POST /api/v1/library/items/{id}/add-to-catalog
Content-Type: application/json

{
  "catalogId": 10,
  "categoryId": 15,
  "sortOrder": 0
}
```
Creates a copy of the library item in the specified catalog/menu.

---

### Library Categories (`/api/v1/library/categories`)

#### Get All Categories
```http
GET /api/v1/library/categories
```

**Response:**
```json
{
  "success": true,
  "data": {
    "categories": [
      {
        "id": 1,
        "name": "Pizzas",
        "description": "All our pizza options",
        "icon": "fa-pizza-slice",
        "sortOrder": 0,
        "itemCount": 12
      }
    ]
  }
}
```

#### Create Category
```http
POST /api/v1/library/categories
Content-Type: application/json

{
  "name": "Pizzas",
  "description": "All our pizza options",
  "icon": "fa-pizza-slice",
  "sortOrder": 0
}
```

#### Update Category
```http
PUT /api/v1/library/categories/{id}
```

#### Delete Category
```http
DELETE /api/v1/library/categories/{id}
```
Cannot delete if category has items.

#### Add Category to Catalog
```http
POST /api/v1/library/categories/{id}/add-to-catalog
Content-Type: application/json

{
  "catalogId": 10,
  "sortOrder": 0
}
```

---

## Frontend Pages

### Library Items Page
**Location:** `/dashboard/library/items`

**Features:**
- View all library items in a grid
- Search items by name/description
- Filter by category
- Add new items
- Edit existing items
- Delete items
- Manage categories

**Components Used:**
- `ItemFormModal.vue` - Form for creating/editing items
- `CategoryModal.vue` - Modal for managing categories

---

## Item Form Features

### Basic Information
- **Name** (required) - Item name
- **Description** - Detailed description
- **Category** - Assign to category
- **Base Price** (required) - Starting price

### Allergens & Dietary Tags
Pre-defined tags:
- Gluten-Free
- Vegan
- Vegetarian
- Dairy-Free
- Nut-Free
- Halal
- Kosher
- Spicy
- Organic
- Sugar-Free

Plus ability to add custom tags.

### Variants (Sizes/Options)
For each variant, specify:
- **Title** (required) - e.g., "Small", "Medium", "Large"
- **Price** (required) - Price for this variant
- **IsDefault** - Mark as default selection

---

## Usage Workflow

### 1. Set Up Library
```
Dashboard → Library → Add Categories
Dashboard → Library → Add Items with variants and allergens
```

### 2. Create Menu from Library
```
Dashboard → Menu → Create New Menu
Select items from library → Add to menu
Customize per menu if needed
```

### 3. Update Library Items
When you update a library item:
- Existing menu items **won't** auto-update (they're copies)
- Future additions will use the updated version
- You can manually update menu items if needed

---

## Common Allergen Tags

Use these standard tags for consistency:

**Allergens:**
- Gluten
- Dairy
- Eggs
- Nuts (or specific: Peanuts, Tree Nuts)
- Soy
- Fish
- Shellfish
- Sesame

**Dietary:**
- Vegan
- Vegetarian
- Halal
- Kosher
- Gluten-Free
- Dairy-Free
- Sugar-Free
- Organic
- Keto
- Paleo
- Low-Carb
- Spicy (Mild/Medium/Hot)

---

## Best Practices

### Organizing Items
1. **Use Categories** - Group similar items (Pizzas, Burgers, Drinks)
2. **Consistent Naming** - Use clear, descriptive names
3. **Add Descriptions** - Help customers understand what they're ordering
4. **Tag Allergens** - Always mark allergens for safety

### Managing Variants
1. **Size Variants** - Small, Medium, Large
2. **Weight Variants** - 250g, 500g, 1kg
3. **Portion Variants** - Single, Double, Family
4. **Mark Default** - Always set one variant as default

### Pricing Strategy
1. Set base price to lowest variant price
2. Variants should have incremental pricing
3. Update prices in library for consistency

---

## Frontend API Usage

```typescript
// Import the API
import { useLibraryItemsApi, useLibraryCategoriesApi } from '~/composables/useApi'

const libraryItemsApi = useLibraryItemsApi()
const categoriesApi = useLibraryCategoriesApi()

// Get all items
const response = await libraryItemsApi.getItems()
const items = response.data.data.items

// Create new item
await libraryItemsApi.createItem({
  name: "Pizza",
  price: 89.99,
  tags: ["Vegetarian"],
  variants: [
    { title: "Small", price: 89.99, isDefault: true }
  ]
})

// Add item to catalog
await libraryItemsApi.addToCatalog(itemId, {
  catalogId: 10,
  categoryId: 5,
  sortOrder: 0
})
```

---

## Troubleshooting

### "Category has items" Error
You cannot delete a category that contains items. First:
1. Move items to another category, OR
2. Delete items, THEN
3. Delete category

### Item Not Showing in Menu
Check:
1. Item is active (`IsActive = true`)
2. Item has been added to the catalog
3. Category is active in the menu

### Variants Not Loading
Make sure:
1. Variants have `IsActive = true`
2. Item has been fetched with variants included
3. At least one variant is marked as default

---

## Future Enhancements

Possible additions:
1. **Bulk Import** - CSV/Excel import of items
2. **Image Upload** - Direct image upload for items
3. **Sync Changes** - Auto-update menu items when library changes
4. **Templates** - Save item templates for quick creation
5. **Pricing Rules** - Apply pricing rules across variants
6. **Inventory Tracking** - Track stock levels per variant

---

## Support

For issues or questions:
1. Check this guide first
2. Review API endpoint documentation
3. Check browser console for errors
4. Verify migration has been run successfully

---

## Summary

The Library System gives you complete control over your menu items:

✅ **Centralized Management** - One place for all items
✅ **Reusability** - Use items across multiple menus
✅ **Flexibility** - Variants, allergens, categories
✅ **Safety** - Track allergen information
✅ **Efficiency** - Quick menu creation from library

Navigate to **Dashboard → Library** to get started!
