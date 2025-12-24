# BizBio Bundles System - Complete Guide

## Overview

The BizBio bundles system allows you to create complex meal deals and product bundles with multiple configuration options. This guide explains how to create and manage bundles like the "Family Meal Deal" example.

## Table of Contents

1. [Understanding Bundle Structure](#understanding-bundle-structure)
2. [Creating a Bundle](#creating-a-bundle)
3. [Family Meal Deal Example](#family-meal-deal-example)
4. [Customer Experience](#customer-experience)
5. [Technical Implementation](#technical-implementation)

---

## Understanding Bundle Structure

A bundle consists of the following hierarchy:

```
Bundle (e.g., "Family Meal Deal" - R250)
├── Step 1 (e.g., "Choose First Pizza")
│   ├── Allowed Products
│   │   └── Bacon, Avo & Feta Pizza
│   │   └── Classic Cheese Pizza
│   │   └── Pepperoni Deluxe Pizza
│   └── Option Groups
│       ├── Choose Base (Required: Min 1, Max 1)
│       │   ├── Gluten Free (Free)
│       │   ├── Traditional (Free)
│       │   └── Pan (Free)
│       ├── Extra Meat Toppings (Optional: Min 0, Max 10)
│       │   ├── Bacon (+R28.90)
│       │   ├── Chicken (+R28.90)
│       │   └── ...
│       └── ...
├── Step 2 (e.g., "Choose Second Pizza")
│   └── [Same structure as Step 1]
└── Step 3 (e.g., "Choose Cold Drink")
    ├── Allowed Products
    │   └── Pepsi 2L
    │   └── Coke 2L
    │   └── ...
    └── [No option groups needed]
```

### Key Concepts:

- **Bundle**: The main product with a base price (e.g., R250 for Family Meal Deal)
- **Steps**: Sequential choices the customer makes (e.g., "Choose Pizza 1", "Choose Pizza 2", "Choose Drink")
- **Allowed Products**: Which items can be selected in each step
- **Option Groups**: Additional customization within a step (e.g., "Choose Base", "Extra Toppings")
- **Options**: Individual choices within an option group (e.g., "Gluten Free", "Extra Bacon +R28.90")

---

## Creating a Bundle

### Navigation

Go to: **Dashboard** → **Bundles** → **Create Bundle**

### Step-by-Step Process

#### Step 1: Basic Information

1. **Bundle Name**: "Family Meal Deal"
2. **Description**: "Any 2 Large Pizzas + 2L Drink for only R250"
3. **Base Price**: R250.00
4. **URL Slug**: "family-meal-deal" (auto-generated from name)
5. **Bundle Image**: Upload an appealing image of the deal

Click **Next: Add Steps**

#### Step 2: Bundle Steps

Create 3 steps:

**Step 1:**
- Step Name: "Choose Your First Pizza"
- Min Select: 1
- Max Select: 1

**Step 2:**
- Step Name: "Choose Your Second Pizza"
- Min Select: 1
- Max Select: 1

**Step 3:**
- Step Name: "Choose Your Drink"
- Min Select: 1
- Max Select: 1

Click **Next: Assign Products**

#### Step 3: Assign Products to Steps

**For Step 1 (First Pizza):**
Select from dropdown:
- Bacon, Avo & Feta Pizza
- Classic Cheese Pizza
- Pepperoni Deluxe Pizza

**For Step 2 (Second Pizza):**
Select same pizzas:
- Bacon, Avo & Feta Pizza
- Classic Cheese Pizza
- Pepperoni Deluxe Pizza

**For Step 3 (Drink):**
Select cold drinks:
- Pepsi 2L
- Coke 2L
- 7 Up 2L
- Mirinda 2L
- Mountain Dew 2L

Click **Next: Add Options**

#### Step 4: Add Option Groups & Options

This is where you add customization options for each step.

**For Step 1 (First Pizza):**

1. Click **Add Option Group**
   - Name: "Choose Your Base"
   - Required: ✓ (checked)
   - Min Select: 1
   - Max Select: 1

   Add Options:
   - Gluten Free | Price: R0.00 | Default: ✓
   - Traditional | Price: R0.00
   - Pan | Price: R0.00

2. Click **Add Option Group**
   - Name: "Extra Meat Toppings"
   - Required: ☐ (unchecked)
   - Min Select: 0
   - Max Select: 10

   Add Options:
   - Bacon | Price: R28.90
   - Chicken | Price: R28.90
   - Ham | Price: R28.90
   - Pepperoni | Price: R28.90
   - Russian | Price: R32.50

3. Click **Add Option Group**
   - Name: "Extra Veg Toppings"
   - Required: ☐ (unchecked)
   - Min Select: 0
   - Max Select: 10

   Add Options:
   - Chilli | Price: R14.90
   - Garlic | Price: R18.90
   - Avo | Price: R28.90
   - Tomato | Price: R14.90
   - Olives | Price: R18.90
   - Onion | Price: R14.90
   - Pineapple | Price: R18.90

4. Click **Add Option Group**
   - Name: "Extra Cheese"
   - Required: ☐ (unchecked)
   - Min Select: 0
   - Max Select: 10

   Add Options:
   - Feta Cheese | Price: R28.90
   - Cheddar Cheese | Price: R28.90
   - Mozzarella Cheese | Price: R20.90

5. Click **Add Option Group**
   - Name: "Swap Base Sauce"
   - Required: ☐ (unchecked)
   - Min Select: 0
   - Max Select: 1

   Add Options:
   - BBQ Base | Price: R0.00
   - Chutney Base | Price: R0.00
   - Mayo Base | Price: R7.90
   - Peri-Peri Base | Price: R0.00
   - Tomato Base | Price: R0.00 | Default: ✓

**For Step 2 (Second Pizza):**
Repeat the same option groups as Step 1

**For Step 3 (Drink):**
No option groups needed (just product selection)

Click **Save Bundle**

---

## Family Meal Deal Example

### Example Configuration

A customer orders:

**Step 1: First Pizza**
- Product: Bacon, Avo & Feta
- Base: Gluten Free
- Extra Meat: Bacon (+R28.90), Extra: Avo (+R28.90)
- Extra Cheese: Feta Cheese (+R28.90)
- Sauce: Mayo Base (+R7.90)

**Step 2: Second Pizza**
- Product: Pepperoni Deluxe
- Base: Pan
- Extra Veg: Garlic (+R18.90)

**Step 3: Drink**
- Product: Pepsi 2L

### Price Calculation

```
Base Bundle Price:        R250.00
Pizza 1 Extras:
  - Extra Bacon:         +R 28.90
  - Extra Avo:           +R 28.90
  - Extra Feta:          +R 28.90
  - Mayo Base:           +R  7.90
Pizza 2 Extras:
  - Extra Garlic:        +R 18.90
                         --------
TOTAL:                    R363.50
```

---

## Customer Experience

### On the Menu Page

1. Customer browses menu and sees "Family Meal Deal" with a **BUNDLE** badge
2. Item shows: "From R250.00"
3. Customer clicks on the bundle

### Bundle Configuration Modal

**Step 1: Choose First Pizza**
- Progress bar shows: 1 of 3 steps
- "Select 1 item" instruction
- Customer sees 3 pizza options with images
- Customer clicks "Bacon, Avo & Feta"
- Option groups expand showing:
  - Choose Base (required - radio buttons)
  - Extra Meat (optional - checkboxes)
  - Extra Veg (optional - checkboxes)
  - Extra Cheese (optional - checkboxes)
  - Swap Sauce (optional - radio buttons)
- Each option shows price modifier (e.g., "+R28.90" or "Free")
- Customer selects options
- Running total updates in footer: "R278.90"
- Customer clicks "Next Step"

**Step 2: Choose Second Pizza**
- Progress bar shows: 2 of 3 steps
- Same interface as Step 1
- Customer configures second pizza
- Total updates: "R363.50"
- Customer clicks "Next Step"

**Step 3: Choose Drink**
- Progress bar shows: 3 of 3 steps
- Customer selects drink
- Total remains: "R363.50"
- Button changes to "Add to Cart"
- Customer clicks "Add to Cart"

### After Adding to Cart

- Toast notification: "Family Meal Deal added to cart!"
- Cart drawer opens automatically
- Shows configured bundle with summary
- Customer can proceed to checkout

---

## Technical Implementation

### Files Modified/Created

#### 1. Bundle Builder UI
- **File**: `pages/dashboard/bundles/create.vue`
- **Purpose**: 4-step wizard for creating bundles
- **Features**:
  - Step 1: Basic bundle info
  - Step 2: Add steps with min/max selection rules
  - Step 3: Assign products to steps
  - Step 4: Add option groups and options with prices

#### 2. Bundle Edit Page
- **File**: `pages/dashboard/bundles/[id]/edit.vue`
- **Purpose**: Edit existing bundles
- **Features**: Loads bundle data and allows updates

#### 3. Customer Configuration Modal
- **File**: `components/BundleConfigModal.vue`
- **Purpose**: Customer-facing bundle configuration
- **Features**:
  - Step-by-step product selection
  - Option group rendering
  - Price calculation
  - Validation (required options)
  - Progress tracking

#### 4. Cart Integration
- **File**: `stores/cart.ts`
- **Function**: `addBundleItem()`
- **Purpose**: Handle bundle items in cart
- **Data Stored**:
  ```typescript
  {
    id: "cart-xxx",
    catalogItemId: 123,
    name: "Family Meal Deal",
    basePrice: 363.50, // Including all extras
    quantity: 1,
    isBundle: true,
    bundleSelections: {
      bundleId: 123,
      steps: [
        {
          stepId: 1,
          stepName: "Choose First Pizza",
          selectedProducts: [...],
          productOptions: {
            productId: {
              groupId: [options]
            }
          }
        },
        ...
      ]
    }
  }
  ```

#### 5. Menu Page Integration
- **File**: `pages/menu/[slug].vue`
- **Changes**:
  - Detects bundle items (itemType === 1)
  - Opens BundleConfigModal instead of ItemDetailModal
  - Handles bundle add-to-cart events

### API Endpoints Used

```
POST   /api/v1/dashboard/catalogs/{catalogId}/bundles
GET    /api/v1/dashboard/catalogs/{catalogId}/bundles
GET    /api/v1/dashboard/catalogs/{catalogId}/bundles/{bundleId}
PUT    /api/v1/dashboard/catalogs/{catalogId}/bundles/{bundleId}
DELETE /api/v1/dashboard/catalogs/{catalogId}/bundles/{bundleId}

POST   /api/v1/dashboard/catalogs/{catalogId}/bundles/{bundleId}/steps
POST   /api/v1/dashboard/catalogs/{catalogId}/bundles/{bundleId}/steps/{stepId}/products
POST   /api/v1/dashboard/catalogs/{catalogId}/bundles/{bundleId}/steps/{stepId}/option-groups
POST   /api/v1/dashboard/catalogs/{catalogId}/bundles/{bundleId}/option-groups/{groupId}/options

POST   /api/v1/dashboard/catalogs/{catalogId}/bundles/{bundleId}/add-to-category
```

### Database Schema

```
CatalogBundle
├── id, name, slug, description, basePrice, images, sortOrder
└── CatalogBundleStep[]
    ├── id, stepNumber, name, minSelect, maxSelect
    ├── CatalogBundleStepProduct[] (allowed products)
    └── CatalogBundleOptionGroup[]
        ├── id, name, isRequired, minSelect, maxSelect
        └── CatalogBundleOption[]
            └── id, name, priceModifier, isDefault
```

---

## Best Practices

### Pricing Strategy

1. **Base Price**: Set the bundle price at the total value without extras
   - Example: 2 pizzas (R150 each) + drink (R50) = R250 base

2. **Option Pricing**: Charge for extras the same as standalone items
   - Keep pricing consistent across regular items and bundles
   - Makes it easier to understand value

3. **Free Options**: Use "Free" (R0.00) for included choices
   - Different bases, standard sauces, etc.

### User Experience

1. **Clear Instructions**: Use descriptive step names
   - ✓ "Choose Your First Pizza"
   - ✗ "Step 1"

2. **Required vs Optional**: Make base choices required, extras optional
   - Required: Base, Size (if applicable)
   - Optional: Extra toppings, sauces, sides

3. **Default Selections**: Set sensible defaults
   - Default base to most popular option
   - Default sauce to standard sauce
   - Reduces clicks for customers

4. **Validation Messages**: Help users complete configuration
   - "Please select a base for your pizza"
   - "You can only select up to 3 toppings"

### Bundle Design

1. **Logical Grouping**: Group similar items in steps
   - All pizzas in one step
   - All drinks in another step
   - Don't mix categories unnecessarily

2. **Reasonable Limits**: Set realistic min/max selections
   - Don't allow 50 toppings
   - Require essential choices

3. **Price Modifiers**: Keep extras consistent
   - Same topping prices across all pizzas in bundle
   - Predictable pricing builds trust

---

## Troubleshooting

### Bundle Not Showing on Menu

**Issue**: Created bundle doesn't appear on menu

**Solutions**:
1. Check bundle is **Active** (isActive = true)
2. Verify bundle was added to a category (use "Add to Category" button)
3. Ensure category is assigned to the menu
4. Check menu is published and active

### Configuration Modal Not Opening

**Issue**: Clicking bundle doesn't open configuration modal

**Solutions**:
1. Verify item has `itemType = 1` (Bundle)
2. Check bundle has associated steps and products
3. Look for console errors in browser developer tools
4. Ensure `BundleConfigModal` component is imported

### Price Not Calculating Correctly

**Issue**: Total price doesn't match expected amount

**Solutions**:
1. Check `priceModifier` values on options
2. Verify base price is set correctly on bundle
3. Review price calculation in `BundleConfigModal.vue:totalPrice` computed
4. Check cart store `addBundleItem` uses correct total

### Validation Errors

**Issue**: Can't proceed to next step or add to cart

**Solutions**:
1. Check all **Required** option groups have selections
2. Verify min/max selection rules are met
3. Ensure at least minimum products are selected in each step
4. Review `isCurrentStepValid()` function logic

---

## Future Enhancements

Potential improvements to the bundle system:

1. **Bundle Templates**: Save common configurations as templates
2. **Bulk Pricing**: Different prices for different quantity tiers
3. **Time-Based Bundles**: Lunch specials, happy hour deals
4. **Bundle Categories**: Group similar bundles (Breakfast, Family, Individual)
5. **Image Gallery**: Multiple images per bundle
6. **Nutrition Info**: Show combined nutritional information
7. **Allergen Warnings**: Display allergens for bundle items
8. **Recommended Bundles**: Suggest bundles based on cart contents
9. **Bundle Analytics**: Track which bundles sell best
10. **Conditional Options**: Show/hide options based on product selection

---

## Support

For issues or questions:
1. Check browser console for errors
2. Review API response in Network tab
3. Verify database has bundle data correctly stored
4. Check that user subscription includes "Bundles" feature

## Summary

The BizBio bundles system provides a complete solution for creating complex product bundles with multiple configuration options. By following this guide, you can create engaging meal deals like the "Family Meal Deal" that give customers flexibility while maintaining pricing control.

Key takeaways:
- Bundles = Base Price + Steps + Products + Option Groups + Options
- Each step can have multiple products to choose from
- Option groups control customization (bases, extras, etc.)
- Pricing is transparent with real-time updates
- System handles validation and cart integration automatically
