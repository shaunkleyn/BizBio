# Variants vs Options vs Extras - Complete Guide

This document explains the fundamental differences between **Variants**, **Options**, and **Extras** in the BizBio menu system and how they work together using the MRD pattern.

---

## Table of Contents

1. [Quick Overview](#quick-overview)
2. [Detailed Comparison](#detailed-comparison)
3. [Real-World Examples](#real-world-examples)
4. [Database Architecture](#database-architecture)
5. [API Response Format](#api-response-format)
6. [Customer Order Flow](#customer-order-flow)
7. [Business Rules](#business-rules)
8. [Implementation Guide](#implementation-guide)

---

## Quick Overview

### The Three-Layer Menu System

```
Menu Item (e.g., "Burger")
    ↓
1. VARIANTS - Different product versions (choose ONE)
    ↓
2. OPTIONS - Required customizations for the selected variant (MUST choose)
    ↓
3. EXTRAS - Optional add-ons for the selected variant (CAN choose)
```

### One-Sentence Definitions

- **Variants** = "What version of this item?" → Determines base price
- **Options** = "How do you want it configured?" → Modifies price (required)
- **Extras** = "Want to add anything?" → Adds to price (optional)

---

## Detailed Comparison

### Variants - Different Versions of a Product

**Purpose:** Represent different SKUs, sizes, or configurations of the SAME menu item

**Key Characteristics:**
- Each variant has its **own base price**
- Each variant has its **own SKU**
- Variants are **mutually exclusive** - customer picks ONE
- Variants are defined at **creation time** by the merchant
- **Not customizable** - they ARE the product choices

**Examples:**
- A "Burger" with variants:
  - "Single Burger" ($50)
  - "Double Burger" ($80)
  - "Combo Meal" ($120)

- A "Pizza" with variants:
  - "Small - 8 inch" ($80)
  - "Medium - 12 inch" ($120)
  - "Large - 16 inch" ($180)

- A "Coffee" with variants:
  - "Regular Cup" ($25)
  - "Large Cup" ($35)

**Database Storage:**
```
Table: CatalogItemVariant
Fields:
- Id
- CatalogItemId
- Title (e.g., "Large")
- Price (base price)
- Sku
- IsDefault
```

---

### Options - Required Customization Choices

**Purpose:** Allow customers to customize/configure their selected variant with REQUIRED selections

**Key Characteristics:**
- Options **modify the variant** with price adjustments
- Price adjustments can be: **+$5**, **-$2**, or **+$0**
- Options are **REQUIRED** (MinRequired >= 1)
- Customer **MUST pick** from each option group
- Can have **multiple option groups** per variant
- Different variants can have **different option groups**

**Examples:**
- For "Combo Meal" variant:
  - Option Group: **"Choose Your Drink"** (Required: must select 1)
    - Coke (+$0)
    - Sprite (+$0)
    - Fanta (+$3)

- For "Large Pizza" variant:
  - Option Group: **"Crust Type"** (Required: must select 1)
    - Thin Crust (+$0)
    - Thick Crust (+$5)
    - Stuffed Crust (+$15)

- For any burger:
  - Option Group: **"Remove Ingredients"** (Required: must select)
    - No Changes (+$0)
    - No Pickles (+$0)
    - No Onions (+$0)

**Database Storage:**
```
Tables:
- CatalogItemOptionGroup (the group definition)
- CatalogItemOption (individual options)
- CatalogItemOptionGroupItem (links options to groups)
- CatalogItemOptionGroupLink (links groups to items/variants)

OptionGroup Fields:
- Id
- Name (e.g., "Choose Your Drink")
- Description
- MinRequired (≥ 1 for options)
- MaxAllowed (1 = single choice, >1 = multi-select)
- IsRequired (true for options)

Option Fields:
- Id
- Name (e.g., "Coke")
- PriceModifier (+$5, -$2, $0)
- Description
```

---

### Extras - Optional Add-ons

**Purpose:** Optional upsells/additions that customers CAN add but don't have to

**Key Characteristics:**
- Extras are **optional** (MinRequired = 0)
- Customer can **skip them entirely**
- Can select **multiple quantities** (e.g., 2x Extra Cheese)
- Extras **add to the price** (never subtract)
- Same extras can be shared across variants

**Examples:**
- For any burger variant:
  - Extra Group: **"Upgrades"** (Optional)
    - Add Extra Patty (+$25)
    - Add Cheese (+$10)
    - Add Bacon (+$15)

- For combo meals:
  - Extra Group: **"Meal Upgrades"** (Optional)
    - Add Large Chips (+$35)
    - Add 1.5L Drink (+$20)
    - Add Dessert (+$25)

**Database Storage:**
```
Tables:
- CatalogItemExtraGroup (the group definition)
- CatalogItemExtra (individual extras)
- CatalogItemExtraGroupItem (links extras to groups)
- CatalogItemExtraGroupLink (links groups to items/variants)

ExtraGroup Fields:
- Id
- Name (e.g., "Upgrades")
- Description
- MinRequired (0 for extras)
- MaxAllowed (0 = unlimited)
- AllowMultipleQuantities (true/false)

Extra Fields:
- Id
- Name (e.g., "Extra Cheese")
- BasePrice (always positive or zero)
- Description
```

---

## Side-by-Side Comparison Table

| Feature | Variants | Options | Extras |
|---------|----------|---------|--------|
| **Purpose** | Different product versions | Required customizations | Optional add-ons |
| **Customer Action** | Choose ONE variant | MUST choose from each group | CAN choose (optional) |
| **Pricing Model** | Sets base price | Modifies price (+/- amount) | Adds to price (+ amount) |
| **Typical Examples** | Small/Medium/Large<br>Single/Double<br>Combo vs Solo | "Choose drink"<br>"Select crust"<br>"Remove ingredients" | "Add extra cheese"<br>"Add bacon"<br>"Upsell drink" |
| **Required?** | Yes (always choose 1) | Yes (MinRequired ≥ 1) | No (MinRequired = 0) |
| **Quantity** | Always 1 | Usually 1 per group | Can be multiple |
| **Price Field** | `Price` (absolute) | `PriceModifier` (delta) | `BasePrice` (absolute) |
| **Database Tables** | `CatalogItemVariant` | `CatalogItemOptionGroup`<br>`CatalogItemOption` | `CatalogItemExtraGroup`<br>`CatalogItemExtra` |
| **When Defined** | At item creation | Links to items/variants | Links to items/variants |
| **Variant-Specific?** | Yes (IS the variant) | Can be (via VariantId) | Can be (via VariantId) |

---

## Real-World Examples

### Example 1: McDonald's Big Mac

```
MenuItem: "Big Mac"
│
├─ VARIANTS (Choose ONE - determines base price)
│  ├─ "Big Mac Only" - $45.00
│  ├─ "Medium Meal" - $75.00  ← Customer selects this
│  └─ "Large Meal" - $85.00
│
├─ OPTIONS for "Medium Meal" variant (Required customizations)
│  │
│  ├─ Option Group: "Choose Your Drink" (Min: 1, Max: 1)
│  │  ├─ Coke (+$0)
│  │  ├─ Sprite (+$0)
│  │  └─ Orange Juice (+$5)  ← Customer selects this
│  │
│  └─ Option Group: "Condiments" (Min: 1, Max: 1)
│     ├─ With Sauce (+$0)  ← Customer selects this
│     └─ No Sauce (+$0)
│
└─ EXTRAS for "Medium Meal" variant (Optional add-ons)
   │
   └─ Extra Group: "Meal Upgrades" (Min: 0, Max: 99)
      ├─ Extra McFlurry (+$20)
      ├─ Extra Large Fries (+$15)  ← Customer selects 1
      └─ Apple Pie (+$10)

Final Price Calculation:
- Base (Variant "Medium Meal"): $75.00
- Option (Orange Juice): +$5.00
- Option (With Sauce): +$0.00
- Extra (Large Fries x1): +$15.00
= Total: $95.00
```

---

### Example 2: Pizza Restaurant

```
MenuItem: "Margherita Pizza"
│
├─ VARIANTS (Different sizes)
│  ├─ "Small - 8 inch" - $80.00
│  ├─ "Medium - 12 inch" - $120.00  ← Customer selects this
│  └─ "Large - 16 inch" - $180.00
│
├─ OPTIONS for "Medium" variant (Required)
│  │
│  ├─ Option Group: "Crust Type" (Min: 1, Max: 1)
│  │  ├─ Thin Crust (+$0)
│  │  ├─ Traditional (+$0)  ← Customer selects this
│  │  └─ Stuffed Crust (+$20)
│  │
│  └─ Option Group: "Sauce Base" (Min: 1, Max: 1)
│     ├─ Tomato Sauce (+$0)  ← Customer selects this
│     ├─ BBQ Sauce (+$5)
│     └─ Pesto (+$10)
│
└─ EXTRAS (Optional toppings)
   │
   └─ Extra Group: "Premium Toppings" (Min: 0, Max: 10)
      ├─ Extra Cheese (+$15)  ← Customer adds 2
      ├─ Pepperoni (+$20)
      ├─ Mushrooms (+$10)  ← Customer adds 1
      └─ Olives (+$10)

Final Price:
- Base (Medium variant): $120.00
- Option (Traditional crust): +$0.00
- Option (Tomato sauce): +$0.00
- Extra (Cheese x2): +$30.00
- Extra (Mushrooms x1): +$10.00
= Total: $160.00
```

---

### Example 3: Coffee Shop

```
MenuItem: "Latte"
│
├─ VARIANTS (Sizes)
│  ├─ "Regular" - $35.00  ← Customer selects this
│  └─ "Large" - $45.00
│
├─ OPTIONS for "Regular" variant (Required)
│  │
│  ├─ Option Group: "Milk Type" (Min: 1, Max: 1)
│  │  ├─ Full Cream (+$0)
│  │  ├─ Low Fat (+$0)
│  │  ├─ Almond Milk (+$5)  ← Customer selects this
│  │  └─ Oat Milk (+$5)
│  │
│  └─ Option Group: "Sweetness" (Min: 1, Max: 1)
│     ├─ No Sugar (+$0)
│     ├─ 1 Sugar (+$0)  ← Customer selects this
│     └─ 2 Sugars (+$0)
│
└─ EXTRAS (Optional add-ons)
   │
   └─ Extra Group: "Add-ons" (Min: 0, Max: 5)
      ├─ Extra Shot (+$8)  ← Customer adds 1
      ├─ Vanilla Syrup (+$5)
      ├─ Caramel Drizzle (+$5)
      └─ Whipped Cream (+$5)

Final Price:
- Base (Regular variant): $35.00
- Option (Almond Milk): +$5.00
- Option (1 Sugar): +$0.00
- Extra (Extra Shot x1): +$8.00
= Total: $48.00
```

---

## Database Architecture

### Entity Relationships

```
CatalogItem
├── CatalogItemVariant (1:Many)
│   ├── Id, Title, Price, Sku, IsDefault
│   └── Links to OptionGroups via CatalogItemOptionGroupLink
│
├── CatalogItemOptionGroupLink (Many:Many)
│   ├── Links Items/Variants to OptionGroups
│   └── VariantId (nullable - for variant-specific options)
│
├── CatalogItemExtraGroupLink (Many:Many)
│   ├── Links Items/Variants to ExtraGroups
│   └── VariantId (nullable - for variant-specific extras)

CatalogItemOptionGroup
├── Id, Name, Description
├── MinRequired (≥ 1), MaxAllowed, IsRequired
└── CatalogItemOptionGroupItem (Many:Many)
    └── Links to CatalogItemOption
        ├── Id, Name, Description
        └── PriceModifier (can be +/- or 0)

CatalogItemExtraGroup
├── Id, Name, Description
├── MinRequired (= 0), MaxAllowed, AllowMultipleQuantities
└── CatalogItemExtraGroupItem (Many:Many)
    └── Links to CatalogItemExtra
        ├── Id, Name, Description
        └── BasePrice (positive or 0)
```

### Key Fields Explained

**Variant Fields:**
- `Price`: The absolute base price for this variant
- `IsDefault`: Which variant is selected by default in the UI
- `Sku`: Unique stock-keeping unit for inventory

**Option Fields:**
- `PriceModifier`: Delta applied to variant price
  - Examples: `+10.00` (adds R10), `-5.00` (subtracts R5), `0.00` (no change)
- `IsDefault`: Which option is pre-selected
- `MinRequired >= 1`: Makes the group required
- `IsRequired = true`: Enforces selection

**Extra Fields:**
- `BasePrice`: Absolute price for the extra
  - Always positive or zero (extras never reduce price)
- `AllowMultipleQuantities`: Can customer order multiple?
- `MinRequired = 0`: Makes the group optional
- `PriceOverride`: Context-specific price (in ExtraGroupItem)

---

## API Response Format (MRD Pattern)

The MRD pattern uses index-based references to avoid data duplication.

### Traditional Nested Approach (❌ Inefficient)

```json
{
  "items": [{
    "name": "Burger",
    "variants": [{
      "name": "Combo Meal",
      "price": 95.00,
      "options": [
        {
          "group": "Choose Drink",
          "items": ["Coke", "Sprite", "Fanta"]
        }
      ]
    }]
  }]
}
```
**Problem:** Options duplicated across every variant → huge payload

---

### MRD Pattern (✅ Efficient)

```json
{
  "sections": [{
    "name": "Burgers",
    "items": [{
      "id": 1,
      "name": "Chicken Burger",
      "variants": [
        {
          "id": 101,
          "name": "Single Burger",
          "price": 50.00,
          "option_indices": [2],      // ← References options[2]
          "extra_indices": [0]         // ← References extras[0]
        },
        {
          "id": 102,
          "name": "Combo Meal",
          "price": 95.00,
          "option_indices": [0, 1],   // ← References options[0] and options[1]
          "extra_indices": [0]         // ← References extras[0]
        }
      ]
    }]
  }],

  "options": [
    {
      "id": 10,
      "name": "Choose Your Drink",
      "label": "Which drink would you like?",
      "minimumSelect": 1,
      "maximumSelect": 1,
      "items": [
        { "id": 101, "name": "Coke", "price": 0 },
        { "id": 102, "name": "Sprite", "price": 0 },
        { "id": 103, "name": "Fanta", "price": 3 }
      ]
    },
    {
      "id": 11,
      "name": "Choose Your Side",
      "label": "Which side would you like?",
      "minimumSelect": 1,
      "maximumSelect": 1,
      "items": [
        { "id": 111, "name": "Fries", "price": 0 },
        { "id": 112, "name": "Onion Rings", "price": 10 }
      ]
    },
    {
      "id": 12,
      "name": "Remove Ingredients",
      "label": "Any changes?",
      "minimumSelect": 1,
      "maximumSelect": 3,
      "items": [
        { "id": 121, "name": "No Changes", "price": 0 },
        { "id": 122, "name": "No Pickles", "price": 0 },
        { "id": 123, "name": "No Onions", "price": 0 }
      ]
    }
  ],

  "extras": [
    {
      "id": 20,
      "name": "Upgrades",
      "label": "Would you like to add more?",
      "minimumSelect": 0,
      "maximumSelect": 99,
      "items": [
        { "id": 201, "name": "Add Extra Patty", "price": 25 },
        { "id": 202, "name": "Add Cheese", "price": 10 },
        { "id": 203, "name": "Add Bacon", "price": 15 }
      ]
    }
  ]
}
```

**Benefits:**
- Options/extras defined **once** at menu level
- Variants reference by **index** (tiny arrays: `[0, 1]`)
- **50-70% smaller** payloads
- Direct array access: `menu.options[0]`
- Easy caching

---

## Customer Order Flow

### Step-by-Step Order Process

```
1. Customer views menu
   └─> Sees all menu items

2. Customer clicks "Chicken Burger"
   └─> Sees variants:
       - Single Burger ($50)
       - Combo Meal ($95)  ← Customer selects this

3. Customer selects "Combo Meal" variant
   └─> Base price set to $95.00
   └─> Required OPTIONS appear (MUST complete):

       Step 3a: "Choose Your Drink" (required)
       ├─ Coke (+$0)
       ├─ Sprite (+$0)
       └─ Fanta (+$3)  ← Customer selects this

       Step 3b: "Choose Your Side" (required)
       ├─ Fries (+$0)
       └─ Onion Rings (+$10)  ← Customer selects this

       Running total: $95 + $3 + $10 = $108

4. Optional EXTRAS offered (CAN skip):

   "Upgrades" group:
   ├─ Add Extra Patty (+$25)
   ├─ Add Cheese (+$10)  ← Customer selects 1
   └─ Add Bacon (+$15)   ← Customer selects 2

   Running total: $108 + $10 + ($15 × 2) = $148

5. Add to cart:
   {
     "variantId": 102,
     "variantName": "Combo Meal",
     "quantity": 1,
     "basePrice": 95.00,
     "selectedOptions": [
       { "optionId": 103, "name": "Fanta", "price": 3 },
       { "optionId": 112, "name": "Onion Rings", "price": 10 }
     ],
     "selectedExtras": [
       { "extraId": 202, "name": "Cheese", "quantity": 1, "price": 10 },
       { "extraId": 203, "name": "Bacon", "quantity": 2, "price": 30 }
     ],
     "itemTotal": 148.00
   }
```

---

## Business Rules

### Variant Rules

1. **At least one variant required** per menu item
2. **Exactly one variant selected** per cart item
3. **IsDefault** determines initial selection in UI
4. **Price is absolute** - not relative to anything
5. **Each variant can have different** option/extra groups

### Option Rules

1. **MinRequired ≥ 1** for all option groups
2. **IsRequired = true** enforces selection
3. **Customer must satisfy MinRequired** before adding to cart
4. **MaxAllowed controls** single vs multi-select
   - `MaxAllowed = 1`: Radio buttons (single choice)
   - `MaxAllowed > 1`: Checkboxes (multi-select)
5. **PriceModifier can be negative** (e.g., "No cheese" = -$5)
6. **Validation required** on frontend AND backend

### Extra Rules

1. **MinRequired = 0** for all extra groups
2. **Customer can skip entirely** without selecting anything
3. **AllowMultipleQuantities** enables quantity selector
4. **BasePrice always ≥ 0** (extras never reduce price)
5. **No validation required** (optional by definition)

### Linking Rules

1. **VariantId = null** in links → applies to ALL variants
2. **VariantId = specific ID** → applies to THAT variant only
3. **DisplayOrder** controls presentation order
4. **Same group can link to multiple items** (reusable)

---

## Implementation Guide

### For Merchants (Creating Products)

#### Step 1: Create Menu Item
```
Name: "Burger"
Description: "Juicy beef burger"
Category: "Mains"
```

#### Step 2: Add Variants
```
Variant 1:
- Title: "Single Burger"
- Price: $50.00
- Sku: "BURG-SINGLE"
- IsDefault: true

Variant 2:
- Title: "Double Burger"
- Price: $80.00
- Sku: "BURG-DOUBLE"

Variant 3:
- Title: "Combo Meal"
- Price: $120.00
- Sku: "BURG-COMBO"
```

#### Step 3: Create Option Groups (if needed)
```
Option Group 1: "Choose Your Drink"
- MinRequired: 1
- MaxAllowed: 1
- IsRequired: true
- Options:
  - Coke (+$0)
  - Sprite (+$0)
  - Fanta (+$3)

Option Group 2: "Remove Ingredients"
- MinRequired: 0
- MaxAllowed: 3
- IsRequired: false
- Options:
  - No Pickles (+$0)
  - No Onions (+$0)
  - No Tomato (+$0)
```

#### Step 4: Create Extra Groups (if needed)
```
Extra Group: "Upgrades"
- MinRequired: 0
- MaxAllowed: 99
- AllowMultipleQuantities: true
- Extras:
  - Extra Cheese (+$10)
  - Extra Bacon (+$15)
  - Extra Patty (+$25)
```

#### Step 5: Link Groups to Variants
```
Link "Choose Your Drink" → Combo Meal variant only
Link "Remove Ingredients" → All variants
Link "Upgrades" → All variants
```

---

### For Developers (Frontend Implementation)

#### Loading Menu Data

```typescript
// Fetch menu
const menu = await fetch('/api/v2/menu/by-slug/my-restaurant').then(r => r.json())

// Menu structure:
// menu.sections[].items[].variants[]
// menu.options[]  ← Global array
// menu.extras[]   ← Global array
```

#### Displaying Variants

```vue
<template>
  <div v-for="variant in menuItem.variants" :key="variant.id">
    <button @click="selectVariant(variant)">
      {{ variant.name }} - R{{ variant.price }}
    </button>
  </div>
</template>
```

#### Handling Options (Required)

```typescript
async function selectVariant(variant: Variant) {
  const selectedOptions = []

  // Get option groups for this variant
  for (const optionIndex of variant.option_indices) {
    const optionGroup = menu.options[optionIndex]  // Direct array access!

    // Show picker (modal/dialog)
    const selected = await showOptionPicker(optionGroup)

    if (!selected) {
      // Validation: Options are required!
      alert(`${optionGroup.label} is required`)
      return
    }

    selectedOptions.push({
      groupId: optionGroup.id,
      optionId: selected.id,
      price: selected.price
    })
  }

  // Continue to extras...
}
```

#### Handling Extras (Optional)

```typescript
async function selectExtras(variant: Variant) {
  const selectedExtras = []

  // Get extra groups for this variant
  for (const extraIndex of variant.extra_indices) {
    const extraGroup = menu.extras[extraIndex]  // Direct array access!

    // Show optional picker (can skip)
    const selected = await showExtrasPicker(extraGroup)

    // User can skip - no validation needed
    if (selected && selected.length > 0) {
      selected.forEach(extra => {
        selectedExtras.push({
          groupId: extraGroup.id,
          extraId: extra.id,
          quantity: extra.quantity,
          price: extra.price * extra.quantity
        })
      })
    }
  }

  return selectedExtras
}
```

#### Calculating Total

```typescript
function calculateCartItemTotal(cartItem: CartItem): number {
  let total = cartItem.basePrice  // Variant price

  // Add option costs
  cartItem.selectedOptions.forEach(option => {
    total += option.price
  })

  // Add extras costs
  cartItem.selectedExtras.forEach(extra => {
    total += extra.price  // Already multiplied by quantity
  })

  return total * cartItem.quantity
}
```

---

### For Developers (Backend Implementation)

#### MenuMappingService Pattern

```csharp
public class MenuMappingService
{
    public MenuResponseDto MapCatalogToMenu(Catalog catalog)
    {
        // 1. Build global arrays (no duplication!)
        var (options, extras, optionMap, extraMap) = BuildGlobalArrays(catalog);

        // 2. Map variants with indices
        var variants = catalog.Items
            .SelectMany(item => item.Variants)
            .Select(v => new VariantDto
            {
                Id = v.Id,
                Price = v.Price,
                OptionIndices = GetOptionIndices(v, optionMap),  // [0, 1]
                ExtraIndices = GetExtraIndices(v, extraMap)      // [0]
            });

        return new MenuResponseDto
        {
            Sections = mapSections,
            Options = options,   // Global array
            Extras = extras      // Global array
        };
    }
}
```

#### Cart Validation

```csharp
[HttpPost("api/v2/menu/calculate-cart")]
public async Task<ActionResult> CalculateCart(CartRequest request)
{
    decimal total = 0;

    foreach (var item in request.Items)
    {
        // 1. Get variant
        var variant = await _context.CatalogItemVariants
            .Include(v => v.CatalogItem)
                .ThenInclude(i => i.OptionGroupLinks)
            .FirstOrDefaultAsync(v => v.Id == item.VariantId);

        // 2. Start with variant base price
        total += variant.Price * item.Quantity;

        // 3. Validate and add option costs
        foreach (var optionSelection in item.SelectedOptions)
        {
            var option = await _context.CatalogItemOptions
                .FindAsync(optionSelection.OptionId);

            total += option.PriceModifier * item.Quantity;
        }

        // 4. Add extras costs
        foreach (var extraSelection in item.SelectedExtras)
        {
            var extra = await _context.CatalogItemExtras
                .FindAsync(extraSelection.ExtraId);

            total += extra.BasePrice * extraSelection.Quantity * item.Quantity;
        }
    }

    return Ok(new { subtotal = total });
}
```

---

## Summary

### When to Use Each

**Use VARIANTS when:**
- You have different sizes, configurations, or SKUs
- Each version has a different base price
- Customer chooses between versions (e.g., Small/Medium/Large)

**Use OPTIONS when:**
- Customer MUST make a selection to complete the order
- Choices modify the base product (e.g., "Choose your drink")
- Selection is required for the order to be valid

**Use EXTRAS when:**
- Customer CAN add something but doesn't have to
- Upselling additional items (e.g., "Add extra cheese")
- Optional enhancements to the base order

### The Complete Picture

```
Customer sees: "Chicken Burger"
  ↓
Selects VARIANT: "Combo Meal" ($95)
  ↓
Required OPTIONS:
  ✓ "Choose Drink" → Coke (+$0)
  ✓ "Choose Side" → Onion Rings (+$10)
  ↓
Optional EXTRAS:
  ✓ "Add Bacon" (+$15 × 2 = $30)
  ↓
Final Cart Item:
  Variant: $95
  Options: $10
  Extras: $30
  = Total: $135
```

---

**All three components work together to create a flexible, customizable menu system with efficient data transfer using the MRD pattern!** 🍔🍕☕
