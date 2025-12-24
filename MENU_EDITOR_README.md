# Menu Editor Implementation

This document describes the complete menu editor implementation with support for categories, items, and bundles.

## Overview

The menu editor provides a comprehensive interface for managing digital menus with the following features:

- **Categories**: Organize menu items into logical groups
- **Items**: Add catalog items to menus with multi-category support
- **Bundles**: Add combo deals/bundles to menus with multi-category support
- **Drag & Drop**: Reorder categories, items, and bundles
- **Library Integration**: Add items and bundles from your user library

## Database Schema

### New Entity: CatalogBundleCategory

A junction table that enables many-to-many relationships between bundles and categories, similar to the existing `CatalogItemCategory` and `BundleCategory` tables.

```csharp
public class CatalogBundleCategory : BaseEntity
{
    public int CatalogBundleId { get; set; }
    public int CategoryId { get; set; }
    
    // Navigation properties
    public virtual CatalogBundle CatalogBundle { get; set; }
    public virtual CatalogCategory Category { get; set; }
}
```

**Migration**: `20251224180013_AddCatalogBundleCategories`

## Backend API Endpoints

### MenuEditorController (`/api/v1/menu-editor`)

All endpoints require authentication via JWT Bearer token.

#### Categories

- `POST /catalogs/{catalogId}/categories` - Add a category
- `PUT /categories/{categoryId}` - Update category details
- `DELETE /categories/{categoryId}` - Delete/deactivate a category
- `PUT /catalogs/{catalogId}/categories/reorder` - Reorder categories

#### Items

- `POST /catalogs/{catalogId}/items` - Add library item to catalog
- `PUT /items/{itemId}/categories` - Update item's category assignments
- `PUT /catalogs/{catalogId}/items/reorder` - Reorder items
- `DELETE /items/{itemId}` - Remove item from catalog

#### Bundles

- `POST /catalogs/{catalogId}/bundles` - Add library bundle to catalog
- `PUT /bundles/{bundleId}/categories` - Update bundle's category assignments
- `PUT /catalogs/{catalogId}/bundles/reorder` - Reorder bundles
- `DELETE /bundles/{bundleId}` - Remove bundle from catalog

#### Bulk Operations

- `PUT /categories/{categoryId}/items/reorder` - Reorder all items in a category

### Updated MenuController

The `GET /api/v1/menus/{id}` endpoint now returns bundles with their category assignments:

```json
{
  "success": true,
  "data": {
    "categories": [...],
    "items": [...],
    "bundles": [
      {
        "id": 1,
        "name": "Family Combo",
        "categoryIds": [2, 5],  // Now includes category IDs
        ...
      }
    ]
  }
}
```

## DTOs

### Category Management

```csharp
public class CategoryCreateDto
{
    public string Name { get; set; }
    public string? Description { get; set; }
    public string? Icon { get; set; }
    public int SortOrder { get; set; }
}

public class CategoryUpdateDto
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? Icon { get; set; }
}
```

### Item/Bundle Management

```csharp
public class AddItemToCatalogDto
{
    public int LibraryItemId { get; set; }
    public List<int> CategoryIds { get; set; }
    public int SortOrder { get; set; }
}

public class AddBundleToCatalogDto
{
    public int BundleId { get; set; }
    public List<int> CategoryIds { get; set; }
    public int SortOrder { get; set; }
}

public class UpdateItemCategoriesDto
{
    public List<int> CategoryIds { get; set; }
}

public class UpdateBundleCategoriesDto
{
    public List<int> CategoryIds { get; set; }
}

public class ReorderDto
{
    public List<ReorderItemDto> Items { get; set; }
}

public class ReorderItemDto
{
    public int Id { get; set; }
    public int SortOrder { get; set; }
}
```

## Frontend UI

### Menu Content Editor Page

**Path**: `/dashboard/menu/[id]/content`

Features:
- View and manage all categories
- Add/edit/delete categories with name, description, and icon
- Add items from library with multi-category assignment
- Add bundles from library with multi-category assignment
- Edit category assignments for existing items/bundles
- Remove items/bundles from menu
- Visual category badges showing assignments

### Menu Settings Page

**Path**: `/dashboard/menu/[id]/edit` (existing)

For editing basic menu metadata (name, slug, description, active status).

## Usage Examples

### Adding a Category

```javascript
const response = await $fetch('/api/v1/menu-editor/catalogs/1/categories', {
  method: 'POST',
  headers: { Authorization: `Bearer ${token}` },
  body: {
    name: 'Appetizers',
    description: 'Start your meal right',
    icon: 'fas fa-cheese',
    sortOrder: 0
  }
})
```

### Adding an Item with Categories

```javascript
const response = await $fetch('/api/v1/menu-editor/catalogs/1/items', {
  method: 'POST',
  headers: { Authorization: `Bearer ${token}` },
  body: {
    libraryItemId: 42,
    categoryIds: [1, 3],  // Assign to multiple categories
    sortOrder: 0
  }
})
```

### Adding a Bundle with Categories

```javascript
const response = await $fetch('/api/v1/menu-editor/catalogs/1/bundles', {
  method: 'POST',
  headers: { Authorization: `Bearer ${token}` },
  body: {
    bundleId: 7,
    categoryIds: [2, 5],  // Combos and Family Deals
    sortOrder: 0
  }
})
```

### Updating Bundle Categories

```javascript
const response = await $fetch('/api/v1/menu-editor/bundles/7/categories', {
  method: 'PUT',
  headers: { Authorization: `Bearer ${token}` },
  body: {
    categoryIds: [2, 5, 8]  // Add to "Specials" category
  }
})
```

### Reordering Categories

```javascript
const response = await $fetch('/api/v1/menu-editor/catalogs/1/categories/reorder', {
  method: 'PUT',
  headers: { Authorization: `Bearer ${token}` },
  body: {
    items: [
      { id: 1, sortOrder: 0 },
      { id: 3, sortOrder: 1 },
      { id: 2, sortOrder: 2 }
    ]
  }
})
```

## Key Features Implemented

✅ **CatalogBundleCategory Entity** - Enables bundle-to-category relationships  
✅ **Database Migration** - Applied successfully  
✅ **MenuEditorController** - Complete CRUD operations for all entities  
✅ **Category Management** - Add, edit, delete, and reorder  
✅ **Item Management** - Add from library, assign categories, remove  
✅ **Bundle Management** - Add from library, assign categories, remove  
✅ **Multi-Category Support** - Items and bundles can belong to multiple categories  
✅ **Reordering** - Drag and drop support for all entities  
✅ **Authorization** - All operations require authenticated user and verify ownership  
✅ **Frontend UI** - Clean, intuitive interface with dialogs and visual feedback  

## Next Steps (Optional Enhancements)

- [ ] Add drag-and-drop library with visual feedback
- [ ] Implement search/filter for library items and bundles
- [ ] Add bulk operations (assign multiple items to category at once)
- [ ] Category hierarchy support (subcategories)
- [ ] Item/bundle preview in dialogs
- [ ] Undo/redo functionality
- [ ] Auto-save functionality
- [ ] Export/import menu structure

## Testing the Implementation

1. **Start the API**:
   ```bash
   cd src/BizBio.API
   dotnet run
   ```

2. **Start the UI**:
   ```bash
   cd src/BizBio.UI
   npm run dev
   ```

3. **Navigate to**:
   - View menus: `/menu/menus`
   - Edit menu content: `/dashboard/menu/{id}/content`
   - Edit menu settings: `/dashboard/menu/{id}/edit`

4. **Test workflows**:
   - Create categories
   - Add items from library to categories
   - Add bundles from library to categories
   - Assign items/bundles to multiple categories
   - Reorder categories, items, and bundles
   - Remove items/bundles from menu

## Database Changes

The migration creates the `CatalogBundleCategories` table:

```sql
CREATE TABLE CatalogBundleCategories (
    Id INT PRIMARY KEY AUTO_INCREMENT,
    CatalogBundleId INT NOT NULL,
    CategoryId INT NOT NULL,
    IsActive TINYINT(1) NOT NULL,
    IsValid TINYINT(1) NOT NULL,
    CreatedAt DATETIME(6) NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CreatedBy LONGTEXT NOT NULL,
    UpdatedAt DATETIME(6) NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    UpdatedBy LONGTEXT NOT NULL,
    CONSTRAINT FK_CatalogBundleCategories_CatalogBundles FOREIGN KEY (CatalogBundleId) 
        REFERENCES CatalogBundles(Id) ON DELETE CASCADE,
    CONSTRAINT FK_CatalogBundleCategories_Categories FOREIGN KEY (CategoryId) 
        REFERENCES Categories(Id) ON DELETE CASCADE,
    UNIQUE INDEX IX_CatalogBundleCategories_CatalogBundleId_CategoryId (CatalogBundleId, CategoryId),
    INDEX IX_CatalogBundleCategories_CategoryId (CategoryId),
    INDEX IX_CatalogBundleCategories_IsActive (IsActive)
);
```

## Architecture Decisions

1. **Separate Controller**: MenuEditorController is separate from MenuController to maintain clean separation between public menu viewing and authenticated menu editing.

2. **Library-Based**: Items and bundles are added from the user's library rather than created inline, promoting reusability across multiple menus.

3. **Soft Deletes**: Items and bundles are marked as inactive rather than hard deleted, preserving data integrity.

4. **Multi-Category Support**: Both items and bundles can belong to multiple categories, providing flexibility in menu organization.

5. **Sort Order**: Explicit sort order fields allow fine-grained control over display order, independent of creation order.

6. **Authorization**: All operations verify catalog ownership through the Profile→User relationship.

## Troubleshooting

### Bundle categories not showing
- Ensure migration `20251224180013_AddCatalogBundleCategories` is applied
- Check that `CatalogBundle` entity includes the navigation property
- Verify the Include statement in GetDetailByIdAsync includes the new relationship

### Items not appearing in library
- Ensure items have `CatalogId = null` (library items)
- Check `UserId` matches the authenticated user

### Reordering not persisting
- Verify sortOrder is being sent in the request
- Check that the catalog ownership is verified before updates
