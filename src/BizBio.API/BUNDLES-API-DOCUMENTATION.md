# BizBio Bundles API Documentation

## Overview

The Bundles API allows you to create, manage, and configure complex meal deals and product bundles with multiple steps, products, and customization options.

**Base URL**: `/api/v1/dashboard/catalogs/{catalogId}/bundles`

**Authentication**: All endpoints require JWT authentication via `Authorization: Bearer {token}` header.

**Feature Requirement**: The "Bundles" feature must be enabled in the user's subscription tier (Menu product line, Tier 1+).

---

## Table of Contents

1. [Bundle Management](#bundle-management)
2. [Step Management](#step-management)
3. [Product Assignment](#product-assignment)
4. [Option Group Management](#option-group-management)
5. [Option Management](#option-management)
6. [Menu Integration](#menu-integration)
7. [Error Responses](#error-responses)
8. [Complete Workflow Example](#complete-workflow-example)

---

## Bundle Management

### Get All Bundles

Retrieve all bundles for a specific catalog.

**Endpoint**: `GET /api/v1/dashboard/catalogs/{catalogId}/bundles`

**Parameters**:
- `catalogId` (path, required): The catalog ID

**Response** (200 OK):
```json
{
  "success": true,
  "data": {
    "bundles": [
      {
        "id": 1,
        "catalogId": 1,
        "name": "Family Meal Deal",
        "slug": "family-meal-deal",
        "description": "Any 2 Large Pizzas + 2L Drink",
        "basePrice": 250.00,
        "images": "[\"https://...\"]",
        "sortOrder": 0,
        "isActive": true,
        "createdAt": "2025-01-01T10:00:00Z",
        "updatedAt": "2025-01-01T10:00:00Z"
      }
    ]
  }
}
```

---

### Get Bundle by ID

Retrieve a specific bundle with all details (steps, products, option groups, options).

**Endpoint**: `GET /api/v1/dashboard/catalogs/{catalogId}/bundles/{bundleId}`

**Parameters**:
- `catalogId` (path, required): The catalog ID
- `bundleId` (path, required): The bundle ID

**Response** (200 OK):
```json
{
  "success": true,
  "data": {
    "bundle": {
      "id": 1,
      "catalogId": 1,
      "name": "Family Meal Deal",
      "slug": "family-meal-deal",
      "description": "Any 2 Large Pizzas + 2L Drink",
      "basePrice": 250.00,
      "images": "[\"https://...\"]",
      "sortOrder": 0,
      "isActive": true,
      "steps": [
        {
          "id": 1,
          "bundleId": 1,
          "stepNumber": 1,
          "name": "Choose First Pizza",
          "minSelect": 1,
          "maxSelect": 1,
          "allowedProducts": [
            {
              "stepId": 1,
              "productId": 10,
              "product": {
                "id": 10,
                "name": "Bacon, Avo & Feta Pizza",
                "description": "...",
                "price": 150.00
              }
            }
          ],
          "optionGroups": [
            {
              "id": 1,
              "stepId": 1,
              "name": "Choose Your Base",
              "isRequired": true,
              "minSelect": 1,
              "maxSelect": 1,
              "options": [
                {
                  "id": 1,
                  "optionGroupId": 1,
                  "name": "Gluten Free",
                  "priceModifier": 0.00,
                  "isDefault": true
                },
                {
                  "id": 2,
                  "optionGroupId": 1,
                  "name": "Traditional",
                  "priceModifier": 0.00,
                  "isDefault": false
                }
              ]
            }
          ]
        }
      ]
    }
  }
}
```

---

### Create Bundle

Create a new bundle. Requires the "Bundles" feature in subscription.

**Endpoint**: `POST /api/v1/dashboard/catalogs/{catalogId}/bundles`

**Parameters**:
- `catalogId` (path, required): The catalog ID

**Request Body**:
```json
{
  "name": "Family Meal Deal",
  "slug": "family-meal-deal",
  "description": "Any 2 Large Pizzas + 2L Drink for only R250",
  "basePrice": 250.00,
  "images": "[\"https://example.com/family-deal.jpg\"]",
  "sortOrder": 0
}
```

**Field Descriptions**:
- `name` (string, required): Bundle name
- `slug` (string, optional): URL-friendly identifier (auto-generated from name if not provided)
- `description` (string, optional): Bundle description
- `basePrice` (decimal, required): Base price of the bundle
- `images` (string, optional): JSON array of image URLs
- `sortOrder` (integer, optional): Display order (default: 0)

**Response** (200 OK):
```json
{
  "success": true,
  "data": {
    "bundle": {
      "id": 1,
      "catalogId": 1,
      "name": "Family Meal Deal",
      "slug": "family-meal-deal",
      "description": "Any 2 Large Pizzas + 2L Drink for only R250",
      "basePrice": 250.00,
      "images": "[\"https://example.com/family-deal.jpg\"]",
      "sortOrder": 0,
      "isActive": true,
      "createdAt": "2025-01-01T10:00:00Z",
      "updatedAt": "2025-01-01T10:00:00Z"
    }
  }
}
```

**Error Response** (400 Bad Request - Feature Not Available):
```json
{
  "success": false,
  "error": "Bundles feature is not available in your subscription tier. Please upgrade to access this feature."
}
```

---

### Update Bundle

Update an existing bundle.

**Endpoint**: `PUT /api/v1/dashboard/catalogs/{catalogId}/bundles/{bundleId}`

**Parameters**:
- `catalogId` (path, required): The catalog ID
- `bundleId` (path, required): The bundle ID

**Request Body** (all fields optional):
```json
{
  "name": "Updated Family Meal Deal",
  "slug": "updated-family-meal-deal",
  "description": "New description",
  "basePrice": 275.00,
  "images": "[\"https://example.com/new-image.jpg\"]",
  "sortOrder": 1,
  "isActive": false
}
```

**Response** (200 OK):
```json
{
  "success": true,
  "data": {
    "bundle": { /* updated bundle object */ }
  }
}
```

---

### Delete Bundle

Delete a bundle (soft delete via IsActive flag).

**Endpoint**: `DELETE /api/v1/dashboard/catalogs/{catalogId}/bundles/{bundleId}`

**Parameters**:
- `catalogId` (path, required): The catalog ID
- `bundleId` (path, required): The bundle ID

**Response** (200 OK):
```json
{
  "success": true,
  "message": "Bundle deleted successfully"
}
```

---

## Step Management

### Add Step to Bundle

Add a step to a bundle (e.g., "Choose First Pizza", "Choose Drink").

**Endpoint**: `POST /api/v1/dashboard/catalogs/{catalogId}/bundles/{bundleId}/steps`

**Parameters**:
- `catalogId` (path, required): The catalog ID
- `bundleId` (path, required): The bundle ID

**Request Body**:
```json
{
  "stepNumber": 1,
  "name": "Choose Your First Pizza",
  "minSelect": 1,
  "maxSelect": 1
}
```

**Field Descriptions**:
- `stepNumber` (integer, required): Order of the step (1, 2, 3, etc.)
- `name` (string, required): Display name for the step
- `minSelect` (integer, optional): Minimum number of products to select (default: 1)
- `maxSelect` (integer, optional): Maximum number of products to select (default: 1)

**Response** (200 OK):
```json
{
  "success": true,
  "data": {
    "step": {
      "id": 1,
      "bundleId": 1,
      "stepNumber": 1,
      "name": "Choose Your First Pizza",
      "minSelect": 1,
      "maxSelect": 1,
      "isActive": true,
      "createdAt": "2025-01-01T10:00:00Z",
      "updatedAt": "2025-01-01T10:00:00Z"
    }
  }
}
```

---

## Product Assignment

### Add Product to Step

Add a product to a bundle step's allowed products list.

**Endpoint**: `POST /api/v1/dashboard/catalogs/{catalogId}/bundles/{bundleId}/steps/{stepId}/products`

**Parameters**:
- `catalogId` (path, required): The catalog ID
- `bundleId` (path, required): The bundle ID
- `stepId` (path, required): The step ID

**Request Body**:
```json
{
  "productId": 10
}
```

**Field Descriptions**:
- `productId` (integer, required): The ID of the catalog item to allow in this step

**Response** (200 OK):
```json
{
  "success": true,
  "data": {
    "stepProduct": {
      "stepId": 1,
      "productId": 10,
      "isActive": true,
      "createdAt": "2025-01-01T10:00:00Z",
      "updatedAt": "2025-01-01T10:00:00Z"
    }
  }
}
```

---

## Option Group Management

### Add Option Group to Step

Add an option group to a step (e.g., "Choose Base", "Extra Toppings").

**Endpoint**: `POST /api/v1/dashboard/catalogs/{catalogId}/bundles/{bundleId}/steps/{stepId}/option-groups`

**Parameters**:
- `catalogId` (path, required): The catalog ID
- `bundleId` (path, required): The bundle ID
- `stepId` (path, required): The step ID

**Request Body**:
```json
{
  "name": "Choose Your Base",
  "isRequired": true,
  "minSelect": 1,
  "maxSelect": 1
}
```

**Field Descriptions**:
- `name` (string, required): Display name for the option group
- `isRequired` (boolean, optional): Whether customer must make a selection (default: false)
- `minSelect` (integer, optional): Minimum options to select (default: 0)
- `maxSelect` (integer, optional): Maximum options to select (default: 10)

**Response** (200 OK):
```json
{
  "success": true,
  "data": {
    "optionGroup": {
      "id": 1,
      "stepId": 1,
      "name": "Choose Your Base",
      "isRequired": true,
      "minSelect": 1,
      "maxSelect": 1,
      "isActive": true,
      "createdAt": "2025-01-01T10:00:00Z",
      "updatedAt": "2025-01-01T10:00:00Z"
    }
  }
}
```

---

## Option Management

### Add Option to Option Group

Add an option to an option group (e.g., "Gluten Free", "Extra Bacon +R28.90").

**Endpoint**: `POST /api/v1/dashboard/catalogs/{catalogId}/bundles/{bundleId}/option-groups/{optionGroupId}/options`

**Parameters**:
- `catalogId` (path, required): The catalog ID
- `bundleId` (path, required): The bundle ID
- `optionGroupId` (path, required): The option group ID

**Request Body**:
```json
{
  "name": "Gluten Free",
  "priceModifier": 0.00,
  "isDefault": true
}
```

**Field Descriptions**:
- `name` (string, required): Display name for the option
- `priceModifier` (decimal, optional): Price to add/subtract (default: 0.00)
- `isDefault` (boolean, optional): Whether this option is selected by default (default: false)

**Response** (200 OK):
```json
{
  "success": true,
  "data": {
    "option": {
      "id": 1,
      "optionGroupId": 1,
      "name": "Gluten Free",
      "priceModifier": 0.00,
      "isDefault": true,
      "isActive": true,
      "createdAt": "2025-01-01T10:00:00Z",
      "updatedAt": "2025-01-01T10:00:00Z"
    }
  }
}
```

---

## Menu Integration

### Add Bundle to Category

Create a CatalogItem reference to make the bundle appear on a menu.

**Endpoint**: `POST /api/v1/dashboard/catalogs/{catalogId}/bundles/{bundleId}/add-to-category`

**Parameters**:
- `catalogId` (path, required): The catalog ID
- `bundleId` (path, required): The bundle ID

**Request Body**:
```json
{
  "categoryId": 5,
  "sortOrder": 0
}
```

**Field Descriptions**:
- `categoryId` (integer, optional): Category to add bundle to (can be null for no category)
- `sortOrder` (integer, optional): Display order within category (default: 0)

**Response** (200 OK):
```json
{
  "success": true,
  "data": {
    "catalogItem": {
      "id": 50,
      "catalogId": 1,
      "categoryId": 5,
      "itemType": 1,
      "bundleId": 1,
      "name": "Family Meal Deal",
      "description": "Any 2 Large Pizzas + 2L Drink",
      "price": 250.00,
      "images": "[\"https://...\"]",
      "sortOrder": 0,
      "isActive": true,
      "createdAt": "2025-01-01T10:00:00Z",
      "updatedAt": "2025-01-01T10:00:00Z"
    }
  }
}
```

**Notes**:
- `itemType: 1` indicates this is a Bundle type catalog item
- The bundle will now appear on menus that include this category
- Customers will see a "BUNDLE" badge on the menu item

---

## Error Responses

### 400 Bad Request

Returned when request validation fails or feature is not available.

```json
{
  "success": false,
  "error": "Error message describing the issue"
}
```

Common errors:
- "Bundles feature is not available in your subscription tier..."
- Invalid model state (missing required fields)

### 403 Forbidden

Returned when user doesn't own the catalog.

```json
{
  "success": false,
  "error": "Forbidden"
}
```

### 404 Not Found

Returned when catalog, bundle, step, or other resource is not found.

```json
{
  "success": false,
  "error": "Bundle not found"
}
```

---

## Complete Workflow Example

### Creating the "Family Meal Deal" Bundle

Here's a complete example of creating the Family Meal Deal with 3 steps, products, and options.

#### Step 1: Create the Bundle

```http
POST /api/v1/dashboard/catalogs/1/bundles
Content-Type: application/json
Authorization: Bearer {token}

{
  "name": "Family Meal Deal",
  "slug": "family-meal-deal",
  "description": "Any 2 Large Pizzas + 2L Drink for only R250",
  "basePrice": 250.00,
  "sortOrder": 0
}
```

**Response**: Bundle created with `id: 1`

---

#### Step 2: Create Steps

**Create Step 1: First Pizza**
```http
POST /api/v1/dashboard/catalogs/1/bundles/1/steps
Content-Type: application/json
Authorization: Bearer {token}

{
  "stepNumber": 1,
  "name": "Choose Your First Pizza",
  "minSelect": 1,
  "maxSelect": 1
}
```

**Response**: Step created with `id: 1`

**Create Step 2: Second Pizza**
```http
POST /api/v1/dashboard/catalogs/1/bundles/1/steps

{
  "stepNumber": 2,
  "name": "Choose Your Second Pizza",
  "minSelect": 1,
  "maxSelect": 1
}
```

**Response**: Step created with `id: 2`

**Create Step 3: Drink**
```http
POST /api/v1/dashboard/catalogs/1/bundles/1/steps

{
  "stepNumber": 3,
  "name": "Choose Your Drink",
  "minSelect": 1,
  "maxSelect": 1
}
```

**Response**: Step created with `id: 3`

---

#### Step 3: Assign Products to Steps

**Assign Pizzas to Step 1 and 2**
```http
POST /api/v1/dashboard/catalogs/1/bundles/1/steps/1/products

{"productId": 10}  // Bacon, Avo & Feta Pizza

POST /api/v1/dashboard/catalogs/1/bundles/1/steps/1/products

{"productId": 11}  // Classic Cheese Pizza

POST /api/v1/dashboard/catalogs/1/bundles/1/steps/1/products

{"productId": 12}  // Pepperoni Deluxe Pizza

// Repeat for step 2
POST /api/v1/dashboard/catalogs/1/bundles/1/steps/2/products

{"productId": 10}
// ... and so on
```

**Assign Drinks to Step 3**
```http
POST /api/v1/dashboard/catalogs/1/bundles/1/steps/3/products

{"productId": 20}  // Pepsi 2L

POST /api/v1/dashboard/catalogs/1/bundles/1/steps/3/products

{"productId": 21}  // Coke 2L

// ... etc
```

---

#### Step 4: Create Option Groups

**Create "Choose Base" Option Group for Step 1**
```http
POST /api/v1/dashboard/catalogs/1/bundles/1/steps/1/option-groups

{
  "name": "Choose Your Base",
  "isRequired": true,
  "minSelect": 1,
  "maxSelect": 1
}
```

**Response**: Option group created with `id: 1`

**Create "Extra Meat Toppings" Option Group**
```http
POST /api/v1/dashboard/catalogs/1/bundles/1/steps/1/option-groups

{
  "name": "Extra Meat Toppings",
  "isRequired": false,
  "minSelect": 0,
  "maxSelect": 10
}
```

**Response**: Option group created with `id: 2`

---

#### Step 5: Add Options to Option Groups

**Add Base Options**
```http
POST /api/v1/dashboard/catalogs/1/bundles/1/option-groups/1/options

{"name": "Gluten Free", "priceModifier": 0.00, "isDefault": true}

POST /api/v1/dashboard/catalogs/1/bundles/1/option-groups/1/options

{"name": "Traditional", "priceModifier": 0.00, "isDefault": false}

POST /api/v1/dashboard/catalogs/1/bundles/1/option-groups/1/options

{"name": "Pan", "priceModifier": 0.00, "isDefault": false}
```

**Add Extra Meat Toppings**
```http
POST /api/v1/dashboard/catalogs/1/bundles/1/option-groups/2/options

{"name": "Bacon", "priceModifier": 28.90, "isDefault": false}

POST /api/v1/dashboard/catalogs/1/bundles/1/option-groups/2/options

{"name": "Chicken", "priceModifier": 28.90, "isDefault": false}

POST /api/v1/dashboard/catalogs/1/bundles/1/option-groups/2/options

{"name": "Ham", "priceModifier": 28.90, "isDefault": false}

POST /api/v1/dashboard/catalogs/1/bundles/1/option-groups/2/options

{"name": "Pepperoni", "priceModifier": 28.90, "isDefault": false}

POST /api/v1/dashboard/catalogs/1/bundles/1/option-groups/2/options

{"name": "Russian", "priceModifier": 32.50, "isDefault": false}
```

---

#### Step 6: Add Bundle to Menu

```http
POST /api/v1/dashboard/catalogs/1/bundles/1/add-to-category

{
  "categoryId": 5,
  "sortOrder": 0
}
```

**Response**: Bundle is now visible on the menu in category 5!

---

## Rate Limiting

Bundle creation is subject to standard API rate limits:
- 100 requests per minute per user
- 1000 requests per hour per user

## Caching

Bundle data is cached for 15 minutes:
- Individual bundle details: 15 minutes
- Catalog bundles list: 15 minutes
- Cache is automatically invalidated on updates

## Best Practices

1. **Order of Operations**: Always create in this order:
   - Bundle → Steps → Products → Option Groups → Options → Add to Category

2. **Step Numbers**: Use sequential step numbers (1, 2, 3, ...) for best customer experience

3. **Required Options**: Mark essential choices (like "Choose Base") as required

4. **Price Modifiers**: Keep pricing consistent across similar items for customer clarity

5. **Defaults**: Set sensible default options to reduce customer clicks

6. **Validation**: Validate data on frontend before API calls to reduce errors

7. **Error Handling**: Always check for subscription feature availability before attempting bundle creation

8. **Testing**: Test complete bundle flow before making available to customers

---

## Support

For questions or issues:
- Check response error messages
- Verify subscription tier includes "Bundles" feature
- Review this documentation
- Contact support with request/response logs

---

## Changelog

### Version 1.0.0 (2025-01-23)
- Initial bundle API implementation
- Complete CRUD operations for bundles
- Step management endpoints
- Product assignment functionality
- Option group and option management
- Menu integration via add-to-category endpoint
