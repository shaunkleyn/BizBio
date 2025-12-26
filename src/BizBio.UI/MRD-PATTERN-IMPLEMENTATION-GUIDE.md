# MRD-Style Menu System Implementation Guide

This guide explains how BizBio implements MRD's efficient menu pattern while maintaining a clean, normalized database.

## Table of Contents
1. [Architecture Overview](#architecture-overview)
2. [Database Schema](#database-schema)
3. [The MRD Pattern](#the-mrd-pattern)
4. [API Implementation](#api-implementation)
5. [Frontend Integration](#frontend-integration)
6. [Cart Logic](#cart-logic)

---

## Architecture Overview

### The Two-Layer Approach

```
┌─────────────────────┐
│   Frontend/API      │  ← MRD-style DTOs (efficient, indexed)
├─────────────────────┤
│  Mapping Service    │  ← Converts between layers
├─────────────────────┤
│  Database/EF Core   │  ← Normalized relational schema
└─────────────────────┘
```

**Why this works:**
- ✅ Database stays normalized (easy to query, update, maintain)
- ✅ API is efficient (single response, minimal data, fast parsing)
- ✅ Frontend is simple (no complex lookups, easy cart logic)

---

## Database Schema

### Your Current Entities (Normalized)

```csharp
CatalogItem
├── Variants[]
│   ├── Id, Title, Price
│   └── IsDefault
└── ExtraGroupLinks[]
    ├── ExtraGroup
    │   ├── Name, MinRequired, MaxAllowed
    │   └── GroupItems[]
    │       └── Extra (Id, Name, BasePrice)
    └── VariantId (nullable - applies to specific variant or all)
```

### Key Relationships

```sql
-- Items have variants (size/price options)
CatalogItemVariant
  - CatalogItemId → CatalogItem
  - Price, Title, IsDefault

-- Items are linked to extra groups
CatalogItemExtraGroupLink
  - CatalogItemId → CatalogItem
  - ExtraGroupId → CatalogItemExtraGroup
  - VariantId (optional - variant-specific extras)

-- Extra groups contain individual extras
CatalogItemExtraGroupItem
  - ExtraGroupId → CatalogItemExtraGroup
  - ExtraId → CatalogItemExtra
```

---

## The MRD Pattern

### What Makes It Special

Instead of this (traditional):
```json
{
  "variant": {
    "id": 123,
    "options": [
      { "id": 1, "name": "Coke", "price": 0 },
      { "id": 2, "name": "Sprite", "price": 0 }
    ],
    "extras": [
      { "id": 10, "name": "Extra Cheese", "price": 5 }
    ]
  }
}
```

MRD does this (indexed):
```json
{
  "options": [
    { "id": 1, "name": "Drinks", "items": [...] },
    { "id": 2, "name": "Condiments", "items": [...] }
  ],
  "extras": [
    { "id": 10, "name": "Upsells", "items": [...] }
  ],
  "variants": [
    {
      "id": 123,
      "option_indices": [0, 1],  // Uses options[0] and options[1]
      "extra_indices": [0]        // Uses extras[0]
    }
  ]
}
```

### Benefits

1. **No Data Duplication**
   - Options/extras defined once
   - Referenced by index across all variants
   - Smaller payload size

2. **Faster Frontend**
   - No nested lookups
   - Direct array access: `menu.options[index]`
   - Simple cart logic

3. **Better Caching**
   - Entire menu structure stable
   - Only prices/availability change
   - Easy to cache and invalidate

---

## API Implementation

### Step 1: Menu Response Structure

```csharp
public class MenuResponseDto
{
    public List<MenuSectionDto> Sections { get; set; }
    public List<ExtraGroupDto> Extras { get; set; }    // ← Global array
    public List<OptionGroupDto> Options { get; set; }  // ← Global array
}

public class VariantDto
{
    public int Id { get; set; }
    public decimal Price { get; set; }
    public List<int> OptionIndices { get; set; }  // ← References Options[]
    public List<int> ExtraIndices { get; set; }   // ← References Extras[]
}
```

### Step 2: Mapping Service

The `MenuMappingService` converts your normalized DB to MRD format:

```csharp
// 1. Build global options/extras arrays
var allExtraGroups = catalog.Items
    .SelectMany(item => item.ExtraGroupLinks)
    .Select(link => link.ExtraGroup)
    .DistinctBy(g => g.Id);

// 2. Separate into Options vs Extras
//    Options: MinRequired >= 1 (e.g., "Choose a drink")
//    Extras: MinRequired = 0 (e.g., "Add extra cheese?")
foreach (var group in allExtraGroups)
{
    if (group.MinRequired >= 1)
        optionGroups.Add(group);  // Index 0, 1, 2...
    else
        extraGroups.Add(group);   // Index 0, 1, 2...
}

// 3. Build variant with indices
variant.OptionIndices = item.ExtraGroupLinks
    .Where(link => link.ExtraGroup.MinRequired >= 1)
    .Select(link => optionIndexMap[link.ExtraGroupId])
    .ToList();
```

### Step 3: API Endpoint

```csharp
[HttpGet("api/v2/menu/{catalogId}")]
public async Task<ActionResult<MenuResponseDto>> GetMenu(int catalogId)
{
    var catalog = await LoadCatalogWithIncludes(catalogId);
    var menuDto = _menuMapper.MapCatalogToMenu(catalog);
    return Ok(menuDto);
}
```

---

## Frontend Integration

### Example: Vue/Nuxt Cart Store

```typescript
// stores/cart.ts
interface CartItem {
  variantId: number
  quantity: number
  selectedOptions: number[]  // Array of option IDs
  selectedExtras: Array<{ id: number, quantity: number }>
}

// When user adds item to cart
function addToCart(item: MenuItem, variant: Variant) {
  const selectedOptions: number[] = []

  // For each option group this variant uses
  variant.option_indices.forEach(index => {
    const optionGroup = menu.options[index]  // ← Direct array access!

    // Show UI for user to select from optionGroup.items
    const selectedOption = await showOptionPicker(optionGroup)
    selectedOptions.push(selectedOption.id)
  })

  // Same for extras
  variant.extra_indices.forEach(index => {
    const extraGroup = menu.extras[index]
    // Show optional extras picker
  })

  cart.items.push({
    variantId: variant.id,
    quantity: 1,
    selectedOptions,
    selectedExtras
  })
}
```

### Example: Calculating Price

```typescript
function calculateItemTotal(cartItem: CartItem): number {
  // 1. Get base variant price
  const variant = findVariantById(cartItem.variantId)
  let total = variant.price

  // 2. Add option costs (if any)
  cartItem.selectedOptions.forEach(optionId => {
    const option = findOptionById(optionId)  // Simple ID lookup
    total += option.price
  })

  // 3. Add extras costs
  cartItem.selectedExtras.forEach(({ id, quantity }) => {
    const extra = findExtraById(id)
    total += extra.price * quantity
  })

  return total * cartItem.quantity
}
```

---

## Cart Logic

### Validating Cart on Backend

```csharp
[HttpPost("api/v2/menu/calculate-cart")]
public async Task<ActionResult> CalculateCart(CartCalculationRequest request)
{
    decimal total = 0;

    foreach (var item in request.Items)
    {
        // 1. Validate variant exists
        var variant = await _context.CatalogItemVariants
            .Include(v => v.CatalogItem)
                .ThenInclude(i => i.ExtraGroupLinks)
            .FirstOrDefaultAsync(v => v.Id == item.VariantId);

        if (variant == null)
            return BadRequest("Invalid variant");

        // 2. Validate selected options
        foreach (var optionId in item.SelectedOptions)
        {
            var option = await _context.CatalogItemExtras
                .FindAsync(optionId);
            if (option == null)
                return BadRequest($"Invalid option: {optionId}");
            total += option.BasePrice;
        }

        // 3. Validate extras
        foreach (var extra in item.SelectedExtras)
        {
            var extraItem = await _context.CatalogItemExtras
                .FindAsync(extra.ExtraId);
            if (extraItem == null)
                return BadRequest($"Invalid extra: {extra.ExtraId}");
            total += extraItem.BasePrice * extra.Quantity;
        }

        total += variant.Price;
    }

    return Ok(new { subtotal = total });
}
```

---

## Example API Response

### GET /api/v2/menu/123

```json
{
  "id": 123,
  "name": "KFC Menu",
  "sections": [
    {
      "id": 1,
      "name": "Deals",
      "items": [
        {
          "id": 1001,
          "name": "All Star Box",
          "variants": [
            {
              "id": 5001,
              "name": "Box Only",
              "price": 99.00,
              "option_indices": [0, 1],  // ← Uses options[0] and options[1]
              "extra_indices": [0, 1]    // ← Uses extras[0] and extras[1]
            }
          ]
        }
      ]
    }
  ],
  "options": [
    {
      "id": 10,
      "name": "Drink Choice",
      "label": "Which cold drink would you like?",
      "minimum_select": 1,
      "maximum_select": 1,
      "items": [
        { "id": 101, "name": "Coke", "price": 0 },
        { "id": 102, "name": "Sprite", "price": 3.90 }
      ]
    },
    {
      "id": 11,
      "name": "Condiments",
      "label": "Would you like condiments?",
      "minimum_select": 1,
      "maximum_select": 1,
      "items": [
        { "id": 111, "name": "With Sauce", "price": 0 },
        { "id": 112, "name": "No Sauce", "price": 0 }
      ]
    }
  ],
  "extras": [
    {
      "id": 20,
      "name": "Bucket Upsells",
      "label": "Would you like to add more?",
      "minimum_select": 0,
      "maximum_select": 99,
      "items": [
        { "id": 201, "name": "Add Large Chips", "price": 34.90 },
        { "id": 202, "name": "Add 4 Mini Loaf", "price": 49.90 }
      ]
    },
    {
      "id": 21,
      "name": "Drinks Upsell",
      "label": "Add a drink?",
      "minimum_select": 0,
      "maximum_select": 5,
      "items": [
        { "id": 211, "name": "1.5L Coke", "price": 25.00 }
      ]
    }
  ]
}
```

### How Frontend Uses This:

```typescript
// User selects variant with id 5001
const variant = findVariant(5001)

// variant.option_indices = [0, 1]
// Show UI for options[0] → "Drink Choice"
// Show UI for options[1] → "Condiments"

// variant.extra_indices = [0, 1]
// Show UI for extras[0] → "Bucket Upsells" (optional)
// Show UI for extras[1] → "Drinks Upsell" (optional)

// Calculate price:
// Base: 99.00
// + Selected options (if they have prices)
// + Selected extras (user chose "Add Large Chips" = 34.90)
// = 133.90
```

---

## Key Advantages Over Traditional Approach

### Traditional (Nested)
- ❌ Each variant duplicates options/extras data
- ❌ Larger payload (10 variants × 5 options = 50 duplicates)
- ❌ Frontend needs nested lookups
- ❌ Harder to cache

### MRD Pattern (Indexed)
- ✅ Options/extras defined once
- ✅ Variants reference by index (tiny arrays: `[0, 1, 2]`)
- ✅ Smaller payload (~50% reduction)
- ✅ Direct array access: `menu.options[0]`
- ✅ Easy to cache

---

## Database Migration Notes

Your existing schema already supports this! You have:
- ✅ `CatalogItemExtraGroup` (option/extra groups)
- ✅ `CatalogItemExtra` (individual options/extras)
- ✅ `CatalogItemExtraGroupLink` (links items to groups)
- ✅ `VariantId` nullable field (variant-specific groups)

**No database changes needed!** Just:
1. Use `MinRequired >= 1` to identify "options" (required)
2. Use `MinRequired = 0` to identify "extras" (optional)
3. Map to MRD-style DTOs in the service layer

---

## Summary

You get the best of both worlds:
- **Backend**: Clean, normalized, relational database
- **API**: Efficient, indexed, minimal response
- **Frontend**: Simple, fast, easy cart logic

The mapping service bridges the gap, converting your normalized schema into MRD's efficient pattern on-the-fly.
