# How to Link Options to Menu Items

This guide explains how to attach option groups (required customizations) and extra groups (optional add-ons) to your menu items.

---

## Table of Contents

1. [Understanding the Workflow](#understanding-the-workflow)
2. [Step-by-Step Guide](#step-by-step-guide)
3. [Visual Walkthrough](#visual-walkthrough)
4. [Backend Implementation](#backend-implementation)
5. [Troubleshooting](#troubleshooting)

---

## Understanding the Workflow

Before you can link options to items, you need to understand the hierarchy:

```
1. Create Individual Options/Extras
   ↓
2. Group Them into Option Groups/Extra Groups
   ↓
3. Link Groups to Menu Items
   ↓
4. Items can now be customized by customers
```

### Key Difference: Options vs Extras

- **Options** = Required customizations (customer MUST choose)
  - Example: "Choose your drink" for a combo meal
  - MinRequired ≥ 1

- **Extras** = Optional add-ons (customer CAN choose)
  - Example: "Add extra cheese"
  - MinRequired = 0

---

## Step-by-Step Guide

### Phase 1: Create Options

#### 1a. Create Individual Options

Navigate to: **Menu Dashboard → Library → Options**

1. Click **"Add Option"**
2. Fill in the details:
   - **Name**: e.g., "Small", "Medium", "Large"
   - **Description**: Optional description
   - **Price Modifier**: Amount to add/subtract from base price
     - `+10.00` = Adds R10
     - `-5.00` = Subtracts R5
     - `0.00` = No change
   - **Image URL**: Optional image
3. Click **"Create"**

**Example Options:**
```
Size Options:
├─ Small (R0)
├─ Medium (+R5)
└─ Large (+R10)

Drink Options:
├─ Coke (R0)
├─ Sprite (R0)
└─ Orange Juice (+R3)
```

#### 1b. Create an Option Group

Navigate to: **Menu Dashboard → Library → Option Groups**

1. Click **"Add Option Group"**
2. Fill in the details:
   - **Name**: e.g., "Choose Your Size"
   - **Description**: e.g., "Select a size for your item"
   - **Minimum Required**: `1` (customer must select at least 1)
   - **Maximum Allowed**: `1` (single choice) or `>1` (multi-select)
   - **Is Required**: ✅ Checked
   - **Display Order**: Order in which group appears
3. **Select Options**: Check the options to include in this group
4. Click **"Create"**

**Example Option Groups:**
```
Group: "Choose Your Size"
├─ MinRequired: 1
├─ MaxAllowed: 1
└─ Options:
    ├─ Small (R0)
    ├─ Medium (+R5)
    └─ Large (+R10)

Group: "Choose Your Drink"
├─ MinRequired: 1
├─ MaxAllowed: 1
└─ Options:
    ├─ Coke (R0)
    ├─ Sprite (R0)
    └─ Orange Juice (+R3)
```

---

### Phase 2: Create Extras (Optional)

#### 2a. Create Individual Extras

Navigate to: **Menu Dashboard → Library → Extras**

1. Click **"Add Extra"**
2. Fill in the details:
   - **Name**: e.g., "Extra Cheese"
   - **Description**: Optional description
   - **Base Price**: Always positive (extras add to price)
   - **Code**: Optional SKU/code
3. Click **"Create"**

**Example Extras:**
```
├─ Extra Cheese (R10)
├─ Extra Bacon (R15)
└─ Extra Patty (R25)
```

#### 2b. Create an Extra Group

Navigate to: **Menu Dashboard → Library → Extra Groups**

1. Click **"Add Extra Group"**
2. Fill in the details:
   - **Name**: e.g., "Upgrades"
   - **Description**: e.g., "Add extras to your burger"
   - **Minimum Required**: `0` (optional)
   - **Maximum Allowed**: `0` (unlimited) or specific number
   - **Allow Multiple Quantities**: ✅ If customers can add multiple
3. **Select Extras**: Check the extras to include
4. Click **"Create"**

**Example Extra Group:**
```
Group: "Burger Upgrades"
├─ MinRequired: 0
├─ MaxAllowed: 99
├─ AllowMultipleQuantities: Yes
└─ Extras:
    ├─ Extra Cheese (R10)
    ├─ Extra Bacon (R15)
    └─ Extra Patty (R25)
```

---

### Phase 3: Link Groups to Menu Items

Navigate to: **Menu Dashboard → Library → Items**

#### Option 1: When Creating a New Item

1. Click **"Add Item"**
2. Fill in basic information (name, description, price, images)
3. Add variants if needed
4. Scroll to **"Options (Required Choices)"** section:
   - ✅ Check option groups to attach (e.g., "Choose Your Size")
   - Selected groups show a **"Required"** badge
5. Scroll to **"Extras (Optional Add-ons)"** section:
   - ✅ Check extra groups to attach (e.g., "Burger Upgrades")
6. Click **"Create Item"**

#### Option 2: When Editing an Existing Item

1. Click the **"Edit"** button on an item
2. Scroll to the **"Options (Required Choices)"** section
3. Check/uncheck option groups
4. Scroll to the **"Extras (Optional Add-ons)"** section
5. Check/uncheck extra groups
6. Click **"Update Item"**

---

## Visual Walkthrough

### UI Flow: Creating & Linking

```
┌─────────────────────────────────────────────────────┐
│  ItemFormModal.vue                                  │
│                                                     │
│  ┌──────────────────────────────────────────────┐  │
│  │ Basic Information                             │  │
│  │ - Name: Burger Combo                          │  │
│  │ - Price: R95                                  │  │
│  └──────────────────────────────────────────────┘  │
│                                                     │
│  ┌──────────────────────────────────────────────┐  │
│  │ Variants (Sizes/Options)                      │  │
│  │ - Combo Meal (R95)                            │  │
│  └──────────────────────────────────────────────┘  │
│                                                     │
│  ┌──────────────────────────────────────────────┐  │
│  │ Options (Required Choices)  [Manage Options] │  │
│  │ ─────────────────────────────────────────────│  │
│  │                                               │  │
│  │ ☑ Choose Your Drink          [Required]      │  │
│  │   Choose which drink for the combo            │  │
│  │   (3 options)                                 │  │
│  │                                               │  │
│  │ ☑ Choose Your Side           [Required]      │  │
│  │   Select your side item                       │  │
│  │   (2 options)                                 │  │
│  │                                               │  │
│  └──────────────────────────────────────────────┘  │
│                                                     │
│  ┌──────────────────────────────────────────────┐  │
│  │ Extras (Optional Add-ons)   [Manage Extras]  │  │
│  │ ─────────────────────────────────────────────│  │
│  │                                               │  │
│  │ ☑ Burger Upgrades                             │  │
│  │   Add extras to your burger                   │  │
│  │   (3 extras)                                  │  │
│  │                                               │  │
│  │ ☐ Meal Upgrades                               │  │
│  │   Upgrade your meal size                      │  │
│  │   (2 extras)                                  │  │
│  │                                               │  │
│  └──────────────────────────────────────────────┘  │
│                                                     │
│  [Cancel]                          [Create Item]   │
└─────────────────────────────────────────────────────┘
```

### Customer Experience

When a customer adds "Burger Combo" to cart:

```
┌─────────────────────────────────────────────────────┐
│  Burger Combo - R95.00                              │
├─────────────────────────────────────────────────────┤
│                                                     │
│  🔴 REQUIRED: Choose Your Drink                     │
│  ◯ Coke           (+R0)                             │
│  ◉ Sprite         (+R0)     ← Selected              │
│  ◯ Orange Juice   (+R3)                             │
│                                                     │
│  🔴 REQUIRED: Choose Your Side                      │
│  ◯ Fries          (+R0)                             │
│  ◉ Onion Rings    (+R10)    ← Selected              │
│                                                     │
│  ─────────────────────────────────────────────────  │
│                                                     │
│  💡 OPTIONAL: Burger Upgrades                       │
│  ☐ Extra Cheese   (+R10)                            │
│  ☑ Extra Bacon    (+R15)    ← Added                 │
│  ☐ Extra Patty    (+R25)                            │
│                                                     │
│  ─────────────────────────────────────────────────  │
│                                                     │
│  Total: R120.00                                     │
│  (Base R95 + Onion Rings R10 + Bacon R15)          │
│                                                     │
│  [Add to Cart]                                      │
└─────────────────────────────────────────────────────┘
```

---

## Backend Implementation

### Database Structure

When you check an option group in the UI, the system creates a link:

```
CatalogItem: "Burger Combo" (ID: 100)
    ↓
CatalogItemOptionGroupLink
├─ CatalogItemId: 100
├─ OptionGroupId: 5
├─ DisplayOrder: 0
└─ IsActive: true

CatalogItemExtraGroupLink
├─ CatalogItemId: 100
├─ ExtraGroupId: 3
├─ DisplayOrder: 0
└─ IsActive: true
```

### API Endpoints

**Creating an item with groups:**
```http
POST /api/v1/library/items
Content-Type: application/json

{
  "name": "Burger Combo",
  "price": 95.00,
  "optionGroupIds": [5, 6],    // Links to option groups
  "extraGroupIds": [3],         // Links to extra groups
  "variants": [...]
}
```

**Response includes linked groups:**
```json
{
  "success": true,
  "data": {
    "item": {
      "id": 100,
      "name": "Burger Combo",
      "optionGroups": [
        {
          "optionGroupId": 5,
          "name": "Choose Your Drink",
          "minRequired": 1,
          "maxAllowed": 1,
          "options": [...]
        }
      ],
      "extraGroups": [
        {
          "extraGroupId": 3,
          "name": "Burger Upgrades",
          "minRequired": 0,
          "extras": [...]
        }
      ]
    }
  }
}
```

---

## Troubleshooting

### Issue: "No option groups available"

**Solution:**
1. Navigate to **Library → Option Groups**
2. Create at least one option group
3. Return to item creation - groups should now appear

### Issue: Option group doesn't show in the list

**Possible causes:**
- Group belongs to different user
- Group is marked as inactive (`IsActive = false`)
- Options haven't been added to the group yet

**Solution:**
- Verify you created the group while logged in
- Edit the group and ensure options are selected

### Issue: Changes not saving

**Solution:**
- Check browser console for errors
- Ensure all required fields are filled
- Verify backend API is running
- Check that option/extra group IDs are valid numbers

### Issue: Option group shows but no options inside

**Solution:**
- Edit the option group
- Select individual options in the "Select Options" section
- Save the group

---

## Advanced Topics

### Linking to Specific Variants

By default, option/extra groups apply to **ALL variants** of an item.

To link to a specific variant:
- This requires backend customization
- Set `VariantId` in the link table instead of leaving it `null`
- Currently not exposed in the UI

### Display Order

Groups are displayed in the order they're added. To reorder:
- Currently requires database update
- Future enhancement: drag-and-drop reordering in UI

### Copying Items Between Catalogs

When copying library items to catalogs:
- Option and extra links are preserved
- Ensures consistent customization options across menus

---

## Summary

### Quick Reference

| Task | Location |
|------|----------|
| Create Options | **Library → Options** |
| Create Option Groups | **Library → Option Groups** |
| Create Extras | **Library → Extras** |
| Create Extra Groups | **Library → Extra Groups** |
| Link to Items | **Library → Items → Edit Item** |

### Workflow Checklist

- [ ] Create individual options
- [ ] Group options into option groups
- [ ] Create individual extras
- [ ] Group extras into extra groups
- [ ] Open/create menu item
- [ ] Check desired option groups (Required)
- [ ] Check desired extra groups (Optional)
- [ ] Save item
- [ ] Test in menu preview

---

**You're now ready to create fully customizable menu items with both required options and optional extras!** 🎉
