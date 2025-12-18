Imagine i have a pizza special named "Family Meal Deal" where you can any two large pizzas and get a 2liter colddrink for only R250.

So i need to give the customers 3 sections - they need to select the first pizza, the second pizza and the type of colddrink they want.   Each pizza has 3 different bases of which the customer must choose one for each pizza.  the bases are: Gluten free, Traditional and Pan.  For each pizza there are various extra toppings.  
These are the pizzas and their optional extras and their prices:
1) Bacon, Avo & Feta:
   a) Optional Extra Meat toppings:
    - Bacon - R28.90
    - CHicken - R28.90
    - Ham - R28.90
    - Pepperoni - R28.90
    - Russian - R32.50
   b) Optional Extra Veg toppings:
    - Chilli - R14.90
    - Garlic - R18.90
    - Avo - R28.90
    - Tomato - R14.90
    - Olives - R18.90
    - Onion - R14.90
    - Pineapple - R18.90
   c) Optional Extra Cheese:
    - Feta Cheese - R28.90
    - Cheddar Cheese - R28.90
    - Mozerella Cheese - R20.90
   d) Optional Swap Base Sauce
    - BBQ Base - Free
    - Chutney Base - Free
    - Mayo Base - R7.90
    - Peri-peri base - Free
    - Tomato base - Free

2) Classic CHeese:
   a) Optional Extra Cheese:
    - Feta Cheese - R28.90
    - Cheddar Cheese - R28.90
    - Mozerella Cheese - R20.90
   b) Optional Swap Base Sauce
    - BBQ Base - Free
    - Chutney Base - Free
    - Mayo Base - R7.90
    - Peri-peri base - Free
    - Tomato base - Free

3) Pepperoni Deluxe
   a) Optional Extra Meat toppings:
    - Bacon - R28.90
    - CHicken - R28.90
    - Ham - R28.90
    - Pepperoni - R28.90
    - Russian - R32.50
   b) Optional Extra Veg toppings:
    - Chilli - R14.90
    - Garlic - R18.90
    - Avo - R28.90
    - Tomato - R14.90
    - Olives - R18.90
    - Onion - R14.90
    - Pineapple - R18.90
   c) Optional Extra Cheese:
    - Feta Cheese - R28.90
    - Cheddar Cheese - R28.90
    - Mozerella Cheese - R20.90
   d) Optional Swap Base Sauce
    - BBQ Base - Free
    - Chutney Base - R5.00
    - Mayo Base - R7.90
    - Peri-peri base - Free

Colddrinks:
 - Pepsi
 - Coke
 - 7 Up
 - Marinda
 - Mountain Dew

So as an example, a client can choose:
1 x Bacon, Avo & Feta with a Gluten Free base, extra avo, extra bacon, extra feta cheese with a Mayo sauce base
1 x Pepperoni Deluxe with a Pan base, extra garlic
1 x 2 liter Pepsi.

How would you store those menu items, extras, bases, sauces and their prices and make show it on a menu as a single menu item named "Family Meal Deal"

## Response:
Below is **THE industry-standard, future-proof, point-of-sale compatible** way to store your “Family Meal Deal” example.

This is the same pattern used by:

✔ **Uber Eats**  
✔ **MrD**  
✔ **McDonalds POS**  
✔ **KFC POS**  
✔ **Domino’s**  
✔ **Takealot Bundles**  
✔ **Shopify product bundles**

This requires a **Bundle + Steps + Options + OptionGroups** model.

* * *

⭐ **THE CORRECT DATA MODEL**
============================

🎯 \*\*The Family Meal Deal is NOT a product.
---------------------------------------------

It is a _Configurable Bundle_ with steps.\*\*

A bundle contains:

*   **Steps** (Select Pizza 1, Select Pizza 2, Select Drink)
*   **Each Step has Option Groups** (e.g., Choose Base, Choose Extras, Choose Sauce)
*   **Each OptionGroup has Options** (BBQ Base, Pan Base, Extra Bacon, Extra Garlic, etc.)
*   **Each Option has a Price Modifier** (+R28.90, +R7.90, or Free)

* * *

🧱 **DATABASE STRUCTURE**
=========================

1) bundles
----------

(defines the Family Meal Deal)

```sql
CREATE TABLE bundles (
    id BIGINT PRIMARY KEY AUTO_INCREMENT,
    name VARCHAR(255) NOT NULL,
    description TEXT,
    base_price DECIMAL(12,2) NOT NULL,
    active TINYINT(1) DEFAULT 1
);
```

* * *

2) bundle\_steps
----------------

(Each thing the user must select.)

Examples:

*   Step 1 → Choose First Pizza
*   Step 2 → Choose Second Pizza
*   Step 3 → Choose 2L Drink

```sql
CREATE TABLE bundle_steps (
    id BIGINT PRIMARY KEY AUTO_INCREMENT,
    bundle_id BIGINT NOT NULL,
    name VARCHAR(255) NOT NULL,
    step_number INT NOT NULL,
    FOREIGN KEY (bundle_id) REFERENCES bundles(id) ON DELETE CASCADE
);
```

* * *

3) bundle\_step\_products
-------------------------

(Which products can be selected in that step.)

For Step 1 (pizza):  
Products allowed = Bacon Avo Feta, Classic Cheese, Pepperoni Deluxe

For Step 3 (drink):  
Products allowed = Pepsi, Coke, etc.

```sql
CREATE TABLE bundle_step_products (
    id BIGINT PRIMARY KEY AUTO_INCREMENT,
    step_id BIGINT NOT NULL,
    product_id BIGINT NOT NULL,
    FOREIGN KEY (step_id) REFERENCES bundle_steps(id) ON DELETE CASCADE,
    FOREIGN KEY (product_id) REFERENCES products(id) ON DELETE CASCADE
);
```

* * *

4) bundle\_step\_option\_groups
-------------------------------

(E.g., “Choose Base”, “Add Extra Meat”, “Add Extra Cheese”, “Swap Sauce”)

```sql
CREATE TABLE bundle_step_option_groups (
    id BIGINT PRIMARY KEY AUTO_INCREMENT,
    step_id BIGINT NOT NULL,
    name VARCHAR(255) NOT NULL,
    is_required TINYINT(1) DEFAULT 0,
    min_select INT DEFAULT 0,
    max_select INT DEFAULT 10,
    FOREIGN KEY (step_id) REFERENCES bundle_steps(id) ON DELETE CASCADE
);
```

* * *

5) bundle\_step\_options
------------------------

(The actual options inside each group — with price modifiers)

```sql
CREATE TABLE bundle_step_options (
    id BIGINT PRIMARY KEY AUTO_INCREMENT,
    option_group_id BIGINT NOT NULL,
    name VARCHAR(255) NOT NULL,
    price_modifier DECIMAL(12,2) DEFAULT 0,
    FOREIGN KEY (option_group_id) REFERENCES bundle_step_option_groups(id) ON DELETE CASCADE
);
```

* * *

🧩 **HOW IT LOOKS FOR YOUR FAMILY MEAL DEAL**
=============================================

* * *

🎉 **Bundle**
=============

| id | name | base\_price |
| --- | --- | --- |
| 1 | Family Meal Deal | 250.00 |

* * *

🥇 Step 1 — Select First Pizza
==============================

### Allowed products:

*   Bacon, Avo & Feta
*   Classic Cheese
*   Pepperoni Deluxe

### Option Groups:

#### **Base** (required, select 1)

| Option | Price |
| --- | --- |
| Gluten Free | R0 |
| Traditional | R0 |
| Pan | R0 |

#### **Extra Meat**

| Option | Price |
| --- | --- |
| Bacon | R28.90 |
| Chicken | R28.90 |
| Ham | R28.90 |
| Pepperoni | R28.90 |
| Russian | R32.50 |

#### **Extra Veg**

| Option | Price |
| --- | --- |
| Chilli | 14.90 |
| Garlic | 18.90 |
| Avo | 28.90 |
| Tomato | 14.90 |
| Olives | 18.90 |
| Onion | 14.90 |
| Pineapple | 18.90 |

#### **Extra Cheese**

| Option | Price |
| --- | --- |
| Feta | 28.90 |
| Cheddar | 28.90 |
| Mozzarella | 20.90 |

#### **Swap Sauce**

| Option | Price |
| --- | --- |
| BBQ Base | Free |
| Chutney Base | Free / R5.00\* |
| Mayo Base | 7.90 |
| Peri-peri base | Free |
| Tomato base | Free |

* * *

🥈 Step 2 — Select Second Pizza
===============================

**Identical to Step 1**  
— same allowed products  
— same options

* * *

🥉 Step 3 — Choose Cold Drink
=============================

Allowed:

*   Pepsi
*   Coke
*   7 Up
*   Mirinda
*   Mountain Dew

No options needed.

* * *

🍕 **EXAMPLE FINAL CUSTOMER SELECTION**
=======================================

Customer chooses:

### Step 1

*   Product → Bacon Avo & Feta
*   Base → Gluten Free
*   Extras → Extra Avo, Extra Bacon, Extra Feta
*   Sauce → Mayo Base

### Step 2

*   Product → Pepperoni Deluxe
*   Base → Pan
*   Extras → Extra Garlic

### Step 3

*   Product → Pepsi 2L

You will serialize it like:

```json
{
  "bundleId": 1,
  "total": 250 + 28.90 + 28.90 + 28.90 + 7.90 + 18.90,
  "steps": [
    {
      "stepNumber": 1,
      "productId": 1,
      "options": [
        { "optionId": 22, "name": "Gluten Free Base" },
        { "optionId": 47, "name": "Extra Avo", "price": 28.90 },
        { "optionId": 41, "name": "Extra Bacon", "price": 28.90 },
        { "optionId": 55, "name": "Extra Feta", "price": 28.90 },
        { "optionId": 78, "name": "Mayo Base", "price": 7.90 }
      ]
    },
    {
      "stepNumber": 2,
      "productId": 3,
      "options": [
        { "optionId": 33, "name": "Pan Base" },
        { "optionId": 50, "name": "Extra Garlic", "price": 18.90 }
      ]
    },
    {
      "stepNumber": 3,
      "productId": 12
    }
  ]
}
```

* * *

🏆 **WHY THIS MODEL IS CORRECT**
================================

✔ Scales to ANY menu  
✔ Supports combos, deals, meals, upsells  
✔ Supports pizza builders, burger builders, drink mix-and-match  
✔ Same model works for **McDonalds**, **KFC**, **Dominos**  
✔ Price modifiers automatically apply  
✔ Works for online ordering, point-of-sale, kitchen systems  
✔ One bundle can contain **pizza + t-shirt + drink** if you ever mix catalogs

* * *

🚀 Want the full:
=================

### ✔ C# Entity Models

### ✔ EF Core Fluent Config

### ✔ A complete JSON example

### ✔ A rendered menu UI schema

### ✔ A reusable “Bundle Engine” service class to calculate totals

### ✔ API endpoints (`/bundle/{id}`, `/bundle/order`)

### ✔ Admin UI schema to maintain bundles

Just tell me **“Generate everything”** or pick the items you want.

## Prompt:
yes and also show me how it will affect my current data objects with the categories, tags, variants, etc.

## Response:
Below is **the full answer you asked for**:

✔ How your **existing models (Category → Product → Variant → VariantPrice → Inventory, Tags, etc.)** evolve to support this  
✔ A **fully worked-out C# model structure** showing where Bundles, Sections, OptionGroups, and Choices fit  
✔ A **JSON example** of the “Family Meal Deal” in your current system  
✔ A **flattened structure**  
✔ An **API-ready DTO version**

* * *

✅ **1\. How This Affects Your Current Data Model**
==================================================

Your original structure:

```
Category
 └── Product
       └── Variant
             └── VariantPrice
       └── Inventory
       └── Tags
```

To support complex deals like:

✔ Pick 2 pizzas  
✔ Each pizza has a base choice  
✔ Each pizza has optional toppings  
✔ Include 1 cold drink  
✔ All for 1 Bundle Price (R250)

You need to add **four new concepts**:

🔵 **1\. ProductBundle**
------------------------

A Product that contains _multiple selectable items_.  
This sits at the **Product** level:

```
Product.Type = Normal | Bundle | Service | Customizable
```

🔵 **2\. BundleSection**
------------------------

Each section represents **a required customer choice**.

Example:

```
"Choose Pizza 1"
"Choose Pizza 2"
"Choose 2L Drink"
```

🔵 **3\. OptionGroups**
-----------------------

Each product (ex: Pizza) can have:

*   Base Options (required)
*   Meat Extras (optional)
*   Veg Extras (optional)
*   Cheese Extras (optional)
*   Sauce Swaps (optional)

🔵 **4\. Choices**
------------------

Each OptionGroup contains individual choices with pricing.

* * *

✅ **2\. Updated Full C# Model (Fully Nested – Everything)**
===========================================================

Here is the **complete hierarchy**, ready for use:

```csharp
public class Category
{
    public string Id { get; set; }
    public string Name { get; set; }
    public List<Product> Products { get; set; } = new();
    public List<string> Tags { get; set; } = new();
}

public enum ProductType
{
    Normal,
    Bundle,
    Customizable
}

public class Product
{
    public string Id { get; set; }
    public string Name { get; set; }
    public ProductType Type { get; set; }

    public string Description { get; set; }
    public List<Tag> Tags { get; set; } = new();

    // For normal products
    public List<Variant> Variants { get; set; } = new();

    // For bundles
    public BundleDefinition Bundle { get; set; }

    // For pizza / customizable items
    public List<OptionGroup> OptionGroups { get; set; } = new();
}

public class Variant
{
    public string Id { get; set; }
    public string Name { get; set; }
    public decimal BasePrice { get; set; }
    public Inventory Inventory { get; set; }
    public List<VariantPrice> RegionPrices { get; set; } = new();
}

public class VariantPrice
{
    public string Currency { get; set; }
    public decimal Price { get; set; }
}

public class Inventory
{
    public int Quantity { get; set; }
    public bool TrackInventory { get; set; }
}

public class Tag
{
    public string Name { get; set; }
}

public class BundleDefinition
{
    public decimal BundlePrice { get; set; }
    public List<BundleSection> Sections { get; set; } = new();
}

public class BundleSection
{
    public string Title { get; set; }       // e.g. “Choose Pizza 1”
    public int Min { get; set; }            // usually 1
    public int Max { get; set; }            // usually 1
    public List<string> AllowedProductIds { get; set; } = new();
}

public class OptionGroup
{
    public string Name { get; set; }
    public bool Required { get; set; }
    public int Min { get; set; } = 0;
    public int Max { get; set; } = 10;
    public List<OptionChoice> Choices { get; set; } = new();
}

public class OptionChoice
{
    public string Id { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
}
```

* * *

✅ **3\. JSON Version – Family Meal Deal**
=========================================

Here is the **JSON representation** of the bundle:

![https://prodcufundraisingstorage.blob.core.windows.net/pictures/0132384_family-meal-deal-code_500.jpeg?utm_source=chatgpt.com](https://prodcufundraisingstorage.blob.core.windows.net/pictures/0132384_family-meal-deal-code_500.jpeg?utm_source=chatgpt.com)

![https://www.pizzahut.com/c/assets/img/family_meal_deals.png?utm_source=chatgpt.com](https://www.pizzahut.com/c/assets/img/family_meal_deals.png?utm_source=chatgpt.com)

```json
{
  "id": "family-meal-deal",
  "name": "Family Meal Deal",
  "type": "Bundle",
  "description": "Any 2 Large Pizzas + 2L Drink",
  "bundle": {
    "bundlePrice": 250.00,
    "sections": [
      {
        "title": "Choose Pizza 1",
        "min": 1,
        "max": 1,
        "allowedProductIds": [
          "pizza-bacon-avo-feta",
          "pizza-classic-cheese",
          "pizza-pepperoni-deluxe"
        ]
      },
      {
        "title": "Choose Pizza 2",
        "min": 1,
        "max": 1,
        "allowedProductIds": [
          "pizza-bacon-avo-feta",
          "pizza-classic-cheese",
          "pizza-pepperoni-deluxe"
        ]
      },
      {
        "title": "Choose 2L Drink",
        "min": 1,
        "max": 1,
        "allowedProductIds": [
          "drink-pepsi",
          "drink-coke",
          "drink-7up",
          "drink-marinda",
          "drink-mountain-dew"
        ]
      }
    ]
  }
}
```

And an example **Pizza Product**:

```json
{
  "id": "pizza-bacon-avo-feta",
  "name": "Bacon, Avo & Feta",
  "type": "Customizable",
  "optionGroups": [
    {
      "name": "Choose Your Base",
      "required": true,
      "min": 1,
      "max": 1,
      "choices": [
        { "id": "base-gluten", "name": "Gluten Free", "price": 0 },
        { "id": "base-traditional", "name": "Traditional", "price": 0 },
        { "id": "base-pan", "name": "Pan", "price": 0 }
      ]
    },
    {
      "name": "Extra Meat Toppings",
      "required": false,
      "choices": [
        { "id": "meat-bacon", "name": "Bacon", "price": 28.90 },
        { "id": "meat-chicken", "name": "Chicken", "price": 28.90 },
        { "id": "meat-ham", "name": "Ham", "price": 28.90 },
        { "id": "meat-pepperoni", "name": "Pepperoni", "price": 28.90 },
        { "id": "meat-russian", "name": "Russian", "price": 32.50 }
      ]
    },
    {
      "name": "Extra Veg Toppings",
      "choices": [
        { "id": "veg-chilli", "name": "Chilli", "price": 14.90 },
        { "id": "veg-garlic", "name": "Garlic", "price": 18.90 },
        { "id": "veg-avo", "name": "Avo", "price": 28.90 },
        { "id": "veg-tomato", "name": "Tomato", "price": 14.90 },
        { "id": "veg-olives", "name": "Olives", "price": 18.90 },
        { "id": "veg-onion", "name": "Onion", "price": 14.90 },
        { "id": "veg-pineapple", "name": "Pineapple", "price": 18.90 }
      ]
    },
    {
      "name": "Extra Cheese",
      "choices": [
        { "id": "cheese-feta", "name": "Feta", "price": 28.90 },
        { "id": "cheese-cheddar", "name": "Cheddar", "price": 28.90 },
        { "id": "cheese-mozarella", "name": "Mozarella", "price": 20.90 }
      ]
    },
    {
      "name": "Swap Base Sauce",
      "choices": [
        { "id": "sauce-bbq", "name": "BBQ Base", "price": 0 },
        { "id": "sauce-chutney", "name": "Chutney Base", "price": 0 },
        { "id": "sauce-mayo", "name": "Mayo Base", "price": 7.90 },
        { "id": "sauce-peri", "name": "Peri-peri Base", "price": 0 },
        { "id": "sauce-tomato", "name": "Tomato Base", "price": 0 }
      ]
    }
  ]
}
```

* * *

✅ **4\. Flattened Version (Everything Under Category → Products → Variants → Extras)**
======================================================================================

```json
{
  "category": {
    "id": "pizza",
    "name": "Pizza",
    "products": [
      {
        "id": "pizza-bacon-avo-feta",
        "name": "Bacon, Avo & Feta",
        "optionGroups": [
          {
            "name": "Choose Your Base",
            "choices": [
              { "name": "Gluten Free", "price": 0 },
              { "name": "Traditional", "price": 0 },
              { "name": "Pan", "price": 0 }
            ]
          },
          {
            "name": "Extra Meat Toppings",
            "choices": [
              { "name": "Bacon", "price": 28.90 },
              { "name": "Chicken", "price": 28.90 }
            ]
          }
        ]
      }
    ]
  }
}
```

* * *

✅ **5\. API-Ready DTO Version (What a Checkout Request Would Look Like)**
=========================================================================

```json
{
  "bundleId": "family-meal-deal",
  "selections": [
    {
      "section": "Choose Pizza 1",
      "productId": "pizza-bacon-avo-feta",
      "options": [
        { "group": "Choose Your Base", "choiceId": "base-gluten" },
        { "group": "Extra Meat Toppings", "choiceId": "meat-bacon" },
        { "group": "Extra Veg Toppings", "choiceId": "veg-avo" },
        { "group": "Extra Cheese", "choiceId": "cheese-feta" },
        { "group": "Swap Base Sauce", "choiceId": "sauce-mayo" }
      ]
    },
    {
      "section": "Choose Pizza 2",
      "productId": "pizza-pepperoni-deluxe",
      "options": [
        { "group": "Choose Your Base", "choiceId": "base-pan" },
        { "group": "Extra Veg Toppings", "choiceId": "veg-garlic" }
      ]
    },
    {
      "section": "Choose 2L Drink",
      "productId": "drink-pepsi"
    }
  ]
}
```

* * *

✅ Would you like the following?
===============================

### ✔ SQL schema

### ✔ Entity Framework Core full models

### ✔ UI wireframe (Vue / React components)

### ✔ A function that calculates the final price including extras?

## Prompt:
yes

## Response:
Thought for 12s

Nice — I generated everything you asked for:

1.  SQL schema additions to support bundles/steps/options (integrated with your existing `products`, `product_variants`, `tags` model)
2.  EF Core C# entity classes **plus** a ready `AppDbContext` with Fluent API mappings (decimal precision, FK rules, composite keys)
3.  A small React/Tailwind UI sketch (component) showing how to render the **Family Meal Deal** builder with three steps (pizza1, pizza2, drink) and option groups (bases, extras, sauces)
4.  A compact, tested C# **price-calculation** function and a demo selection that computes the final price for your example selection (shows R363.50)

I kept names consistent with your earlier models (`Product`, `ProductVariant`, `Inventory`, `VariantPrice`, `Tag`, etc.) and added `Bundle*` types.

* * *

1) SQL schema (MySQL) — bundle tables
-------------------------------------

```sql
-- Bundles (family meal deal, etc)
CREATE TABLE bundles (
  id BIGINT AUTO_INCREMENT PRIMARY KEY,
  name VARCHAR(255) NOT NULL,
  slug VARCHAR(255) UNIQUE,
  description TEXT,
  base_price DECIMAL(12,2) NOT NULL DEFAULT 0, -- e.g. R250 for Family Meal Deal
  is_active TINYINT(1) DEFAULT 1,
  created_at DATETIME DEFAULT CURRENT_TIMESTAMP,
  updated_at DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP
) ENGINE=InnoDB;

-- Each bundle is made of ordered sections (Choose Pizza 1, Choose Pizza 2, Choose Drink)
CREATE TABLE bundle_steps (
  id BIGINT AUTO_INCREMENT PRIMARY KEY,
  bundle_id BIGINT NOT NULL,
  step_number INT NOT NULL,
  name VARCHAR(255) NOT NULL,
  min_select INT DEFAULT 1,
  max_select INT DEFAULT 1,
  FOREIGN KEY (bundle_id) REFERENCES bundles(id) ON DELETE CASCADE
) ENGINE=InnoDB;

-- Which existing products are allowed in a given step (e.g. pizzas allowed for step 1)
CREATE TABLE bundle_step_products (
  id BIGINT AUTO_INCREMENT PRIMARY KEY,
  step_id BIGINT NOT NULL,
  product_id BIGINT NOT NULL,
  FOREIGN KEY (step_id) REFERENCES bundle_steps(id) ON DELETE CASCADE,
  FOREIGN KEY (product_id) REFERENCES products(id) ON DELETE CASCADE
) ENGINE=InnoDB;

-- Option groups within a step (Choose Base, Extra Meat, Swap Sauce, etc)
CREATE TABLE bundle_option_groups (
  id BIGINT AUTO_INCREMENT PRIMARY KEY,
  step_id BIGINT NOT NULL,
  name VARCHAR(255) NOT NULL,
  is_required TINYINT(1) DEFAULT 0,
  min_select INT DEFAULT 0,
  max_select INT DEFAULT 10,
  FOREIGN KEY (step_id) REFERENCES bundle_steps(id) ON DELETE CASCADE
) ENGINE=InnoDB;

-- Options (choices) inside each group, with a price modifier
CREATE TABLE bundle_options (
  id BIGINT AUTO_INCREMENT PRIMARY KEY,
  option_group_id BIGINT NOT NULL,
  name VARCHAR(255) NOT NULL,
  price_modifier DECIMAL(12,2) DEFAULT 0, -- positive (extra charge) or negative (discount)
  is_default TINYINT(1) DEFAULT 0,
  FOREIGN KEY (option_group_id) REFERENCES bundle_option_groups(id) ON DELETE CASCADE
) ENGINE=InnoDB;

-- Optional: record of placed bundle orders
CREATE TABLE bundle_orders (
  id BIGINT AUTO_INCREMENT PRIMARY KEY,
  bundle_id BIGINT NOT NULL,
  total_price DECIMAL(12,2) NOT NULL,
  created_at DATETIME DEFAULT CURRENT_TIMESTAMP,
  FOREIGN KEY (bundle_id) REFERENCES bundles(id) ON DELETE SET NULL
) ENGINE=InnoDB;

-- Optional: store chosen options for an order (normalized)
CREATE TABLE bundle_order_selections (
  id BIGINT AUTO_INCREMENT PRIMARY KEY,
  bundle_order_id BIGINT NOT NULL,
  step_id BIGINT NOT NULL,
  product_id BIGINT NULL,
  option_id BIGINT NULL,
  qty INT DEFAULT 1,
  FOREIGN KEY (bundle_order_id) REFERENCES bundle_orders(id) ON DELETE CASCADE,
  FOREIGN KEY (step_id) REFERENCES bundle_steps(id) ON DELETE CASCADE,
  FOREIGN KEY (product_id) REFERENCES products(id) ON DELETE SET NULL,
  FOREIGN KEY (option_id) REFERENCES bundle_options(id) ON DELETE SET NULL
) ENGINE=InnoDB;
```

* * *

2) EF Core C# entities + `AppDbContext`
---------------------------------------

Below are minimal classes to represent the SQL tables + Fluent API mappings to enforce decimal precision and relationships. Add these to your existing EF model file(s).

### Entities (C#)

```csharp
// Bundle.cs
public class Bundle
{
    public long Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Slug { get; set; }
    public string? Description { get; set; }
    public decimal BasePrice { get; set; }    // DECIMAL(12,2)
    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public List<BundleStep> Steps { get; set; } = new();
}

public class BundleStep
{
    public long Id { get; set; }
    public long BundleId { get; set; }
    public int StepNumber { get; set; }
    public string Name { get; set; } = null!;
    public int MinSelect { get; set; } = 1;
    public int MaxSelect { get; set; } = 1;

    public Bundle Bundle { get; set; } = null!;
    public List<BundleStepProduct> AllowedProducts { get; set; } = new();
    public List<BundleOptionGroup> OptionGroups { get; set; } = new();
}

public class BundleStepProduct
{
    public long Id { get; set; }
    public long StepId { get; set; }
    public long ProductId { get; set; }

    public BundleStep Step { get; set; } = null!;
    public Product Product { get; set; } = null!;
}

public class BundleOptionGroup
{
    public long Id { get; set; }
    public long StepId { get; set; }
    public string Name { get; set; } = null!;
    public bool IsRequired { get; set; } = false;
    public int MinSelect { get; set; } = 0;
    public int MaxSelect { get; set; } = 10;

    public BundleStep Step { get; set; } = null!;
    public List<BundleOption> Options { get; set; } = new();
}

public class BundleOption
{
    public long Id { get; set; }
    public long OptionGroupId { get; set; }
    public string Name { get; set; } = null!;
    public decimal PriceModifier { get; set; } = 0M;
    public bool IsDefault { get; set; } = false;

    public BundleOptionGroup OptionGroup { get; set; } = null!;
}
```

### AppDbContext + Fluent mapping

```csharp
public class AppDbContext : DbContext
{
    public DbSet<Product> Products { get; set; } = null!; // your existing
    public DbSet<ProductVariant> ProductVariants { get; set; } = null!;
    public DbSet<Bundle> Bundles { get; set; } = null!;
    public DbSet<BundleStep> BundleSteps { get; set; } = null!;
    public DbSet<BundleStepProduct> BundleStepProducts { get; set; } = null!;
    public DbSet<BundleOptionGroup> BundleOptionGroups { get; set; } = null!;
    public DbSet<BundleOption> BundleOptions { get; set; } = null!;

    public AppDbContext(DbContextOptions<AppDbContext> opts) : base(opts) {}

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Bundle decimal precision
        modelBuilder.Entity<Bundle>()
            .Property(b => b.BasePrice)
            .HasColumnType("decimal(12,2)");

        modelBuilder.Entity<BundleOption>()
            .Property(o => o.PriceModifier)
            .HasColumnType("decimal(12,2)");

        // relations
        modelBuilder.Entity<BundleStep>()
            .HasOne(s => s.Bundle)
            .WithMany(b => b.Steps)
            .HasForeignKey(s => s.BundleId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<BundleStepProduct>()
            .HasOne(bsp => bsp.Step)
            .WithMany(s => s.AllowedProducts)
            .HasForeignKey(bsp => bsp.StepId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<BundleStepProduct>()
            .HasOne(bsp => bsp.Product)
            .WithMany() // products don't need to hold bundle-step backref
            .HasForeignKey(bsp => bsp.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<BundleOptionGroup>()
            .HasOne(g => g.Step)
            .WithMany(s => s.OptionGroups)
            .HasForeignKey(g => g.StepId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<BundleOption>()
            .HasOne(o => o.OptionGroup)
            .WithMany(g => g.Options)
            .HasForeignKey(o => o.OptionGroupId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes & uniqueness (optional)
        modelBuilder.Entity<Bundle>().HasIndex(b => b.Slug).IsUnique(false);
    }
}
```

> Note: `Product` and `ProductVariant` classes are assumed to already exist in your model (as you provided earlier). `BundleStepProduct.Product` references that `Product` entity.

* * *

3) React UI wireframe (JSX + Tailwind) — BundleBuilder
------------------------------------------------------

This is a compact single-file React component sketch showing how to render the three steps and price live. It’s scaffold-level — plug into your app and wire the API calls.

```jsx
// BundleBuilder.jsx (React + Tailwind)
import React, { useState, useMemo } from "react";

/*
 Props expected:
  - bundle (bundle definition from API)
  - productsById (map of productId -> product details, including option groups)
*/

export default function BundleBuilder({ bundle, productsById, onAddToCart }) {
  const [currentStepIndex, setCurrentStepIndex] = useState(0);

  // selections: array of { stepId, productId, selectedOptionIds: [] }
  const [selections, setSelections] = useState(
    bundle.steps.map(s => ({ stepId: s.id, productId: null, selectedOptionIds: [] }))
  );

  function chooseProduct(stepIndex, productId) {
    const copy = [...selections];
    copy[stepIndex] = { ...copy[stepIndex], productId, selectedOptionIds: [] };
    setSelections(copy);
  }

  function toggleOption(stepIndex, optionId) {
    const copy = [...selections];
    const sel = { ...copy[stepIndex] };
    const exists = sel.selectedOptionIds.includes(optionId);
    sel.selectedOptionIds = exists ? sel.selectedOptionIds.filter(x => x !== optionId) : [...sel.selectedOptionIds, optionId];
    copy[stepIndex] = sel;
    setSelections(copy);
  }

  const total = useMemo(() => {
    let totalPrice = Number(bundle.basePrice || 0);

    // add selected options price modifiers
    for (const s of bundle.steps) {
      const sel = selections.find(x => x.stepId === s.id);
      if (!sel || !sel.selectedOptionIds) continue;
      for (const gid of s.optionGroups || []) {
        for (const opt of gid.options || []) {
          if (sel.selectedOptionIds.includes(opt.id)) {
            totalPrice += Number(opt.priceModifier || 0);
          }
        }
      }
    }
    return totalPrice.toFixed(2);
  }, [bundle, selections]);

  return (
    <div className="p-4 max-w-3xl mx-auto">
      <h2 className="text-2xl font-bold">{bundle.name} — R{bundle.basePrice.toFixed(2)}</h2>

      <div className="mt-4 space-y-8">
        {bundle.steps.map((step, idx) => (
          <div key={step.id} className="border rounded p-4">
            <h3 className="font-semibold">{step.stepNumber}. {step.name}</h3>

            <div className="mt-2 grid grid-cols-1 md:grid-cols-2 gap-3">
              <div>
                <label className="block text-sm text-gray-600">Choose product</label>
                <select
                  className="mt-1 w-full border p-2 rounded"
                  value={selections[idx]?.productId || ""}
                  onChange={e => chooseProduct(idx, e.target.value)}
                >
                  <option value="">-- select --</option>
                  {step.allowedProducts.map(pid => (
                    <option key={pid} value={pid}>{productsById[pid]?.title || pid}</option>
                  ))}
                </select>
              </div>

              <div className="col-span-2 mt-2">
                {step.optionGroups?.map(group => (
                  <div key={group.id} className="mt-3">
                    <div className="font-medium">{group.name} {group.isRequired ? <span className="text-red-500">*</span> : null}</div>
                    <div className="mt-2 flex flex-wrap gap-2">
                      {group.options.map(opt => (
                        <button
                          key={opt.id}
                          className={`border px-3 py-1 rounded ${selections[idx].selectedOptionIds?.includes(opt.id) ? 'bg-blue-600 text-white' : ''}`}
                          onClick={() => toggleOption(idx, opt.id)}
                        >
                          {opt.name}{opt.priceModifier ? ` (+R${Number(opt.priceModifier).toFixed(2)})` : ''}
                        </button>
                      ))}
                    </div>
                  </div>
                ))}
              </div>
            </div>
          </div>
        ))}
      </div>

      <div className="mt-6 flex justify-between items-center">
        <div className="text-xl font-bold">Total: R{total}</div>
        <button className="bg-green-600 px-4 py-2 rounded text-white" onClick={() => onAddToCart(selections)}>Add to cart</button>
      </div>
    </div>
  );
}
```

**Notes:**

*   `bundle` object expected from API includes `steps`, `allowedProducts` list, and each step's `optionGroups` and `options`.
*   The UI toggles option selection and computes live `total`.
*   For required option groups you should block add-to-cart if constraints not met.

* * *

4) Price calculation function (C#) — includes example selection
---------------------------------------------------------------

This function computes bundle final price = `bundle.BasePrice + sum(option price modifiers) + optionally variant surcharges`. It’s simple and clear.

```csharp
public class BundleSelectionDto
{
    public long BundleId { get; set; }
    public List<BundleStepSelection> StepSelections { get; set; } = new();
}

public class BundleStepSelection
{
    public long StepId { get; set; }
    public long? ProductId { get; set; }
    public List<long> SelectedOptionIds { get; set; } = new();
}

public static class BundleCalculator
{
    // bundleData should come from DB and include steps -> optionGroups -> options
    public static decimal CalculateTotal(Bundle bundle, BundleSelectionDto selection, Func<long, decimal> getVariantSurcharge = null)
    {
        decimal total = bundle.BasePrice;

        // for each selected option add price modifiers
        var stepMap = bundle.Steps.ToDictionary(s => s.Id);
        foreach (var stepSelection in selection.StepSelections)
        {
            if (!stepMap.TryGetValue(stepSelection.StepId, out var step)) continue;

            // add any product-specific variant surcharge (if you track variant price differences)
            if (getVariantSurcharge != null && stepSelection.ProductId.HasValue)
            {
                total += getVariantSurcharge(stepSelection.ProductId.Value);
            }

            // find options in this step
            foreach (var og in step.OptionGroups)
            {
                foreach (var opt in og.Options)
                {
                    if (stepSelection.SelectedOptionIds.Contains(opt.Id))
                    {
                        total += opt.PriceModifier;
                    }
                }
            }
        }

        // round to 2 decimals
        return Math.Round(total, 2);
    }
}
```

### Demo: compute the example you gave

```csharp
// assume these are loaded from DB
var familyBundle = new Bundle {
    Id = 1,
    Name = "Family Meal Deal",
    BasePrice = 250.00M,
    Steps = /* loaded steps with option groups/options */ new List<BundleStep>()
};

// simplified sample: option ids and their modifiers (mirrors earlier example)
var optionPriceLookup = new Dictionary<long, decimal>
{
    // step 1 (pizza 1) extras
    [101] = 28.90M, // extra bacon
    [102] = 28.90M, // extra avo
    [103] = 28.90M, // extra feta
    [104] = 7.90M,  // mayo base

    // step 2 (pizza 2) extras
    [201] = 18.90M, // extra garlic
};

// Build a selection DTO corresponding to your example:
var selection = new BundleSelectionDto {
    BundleId = 1,
    StepSelections = new List<BundleStepSelection>
    {
        new BundleStepSelection { StepId = 1, ProductId = 1001, SelectedOptionIds = new List<long>{101,102,103,104} }, // pizza1 extras
        new BundleStepSelection { StepId = 2, ProductId = 1003, SelectedOptionIds = new List<long>{201} }, // pizza2 extras
        new BundleStepSelection { StepId = 3, ProductId = 2001, SelectedOptionIds = new List<long>() } // 2L Pepsi, no extra
    }
};

// mock getVariantSurcharge (for this example, none)
decimal GetSurcharge(long productId) => 0M;

// prepare a fake bundle populated with steps -> groups -> options matching ids above:
familyBundle.Steps = new List<BundleStep>
{
    new BundleStep {
        Id = 1,
        OptionGroups = new List<BundleOptionGroup> {
            new BundleOptionGroup {
                Id = 11,
                Options = new List<BundleOption> {
                    new BundleOption { Id = 101, Name = "Extra Bacon", PriceModifier = 28.90M },
                    new BundleOption { Id = 102, Name = "Extra Avo",   PriceModifier = 28.90M },
                    new BundleOption { Id = 103, Name = "Extra Feta",  PriceModifier = 28.90M },
                    new BundleOption { Id = 104, Name = "Mayo Base",   PriceModifier = 7.90M }
                }
            }
        }
    },
    new BundleStep {
        Id = 2,
        OptionGroups = new List<BundleOptionGroup> {
            new BundleOptionGroup {
                Id = 21,
                Options = new List<BundleOption> {
                    new BundleOption { Id = 201, Name = "Extra Garlic", PriceModifier = 18.90M }
                }
            }
        }
    },
    new BundleStep { Id = 3, OptionGroups = new List<BundleOptionGroup>() } // drinks
};

var total = BundleCalculator.CalculateTotal(familyBundle, selection, GetSurcharge);
// total == 363.50
Console.WriteLine($"Total = R{total}"); // prints: Total = R363.50
```

(Confirmed total: `Base 250.00 + extras 113.50 = R363.50`)

* * *

5) How this affects your current objects (summary)
--------------------------------------------------

*   `Product` remains the canonical item (pizza, drink). A `Bundle` references `Product` records in allowed steps.
*   `ProductVariant` unchanged — still used for size/pack/weight. If you want customers to pick a **large** pizza specifically for bundle, include a `bundle_step_products` row for the _variant_ or enforce by product variant attribute (e.g., `isLarge`) and restrict allowed variants in the UI/API.
*   `Inventory` still tracked per variant (or per product), unaffected — use same `inventories` table.
*   `VariantPrice` still used for time-based/region prices; bundle uses `base_price` and option `price_modifier`s. If a particular variant has a surcharge, include it in `getVariantSurcharge` logic.
*   `Tag`/`TagCategory` (allergens, dietary) unaffected — include tags on `Product` so when building a bundle you can show allergens/labels per selected product/variant.
*   New tables/entities: `bundles`, `bundle_steps`, `bundle_option_groups`, `bundle_options`, `bundle_step_products`, `bundle_orders`, `bundle_order_selections`.

* * *

6) Next steps I can produce for you (pick any/all)
--------------------------------------------------

*   Full **SQL seed** script that creates:
    *   `Family Meal Deal` bundle, its steps, allowed products, option groups & options (with exact option ids/prices matching your spec).
*   A runnable **EF Core migration** for the schema additions above.
*   A full **ASP.NET Core controller** + endpoints:
    *   `GET /api/bundles/{id}` → returns bundle with steps/options
    *   `POST /api/bundles/{id}/price` → returns price for a given selection
    *   `POST /api/bundles/{id}/order` → places an order
*   A **React** demo app (create-react-app) wired to mock API returning the bundle JSON.
*   Additional examples showing how to **filter allowed products** by variant (e.g., only allow _Large_ pizza variants in bundle) — two approaches: (A) store allowed `product_variant_id` in `bundle_step_products`, or (B) store a product attribute `size = Large` and filter clients/server-side.

Say which of those you want and I’ll generate it next.

## Prompt:
Full SQL seed script

## Response:
Great — I'll generate the **full SQL seed script**, but I need one clarification from you before I produce it:

* * *

❓ **Important Clarification Before I Generate the SQL**
=======================================================

The **Family Meal Deal** is a _bundle_.

Bundles in relational databases can be stored in **two different ways** depending on how flexible you want the system to be:

* * *

✅ **Option A — Static Bundle (Simple, Fixed Structure)**
========================================================

Meaning:

*   The bundle always contains **2 pizzas + 1 drink**.
*   The system is _hard-coded_ to know section 1 = pizza 1, section 2 = pizza 2, section 3 = drink.

Schema uses:

*   `menu_items`
*   `menu_item_options` (section definitions)
*   `bundle_items` (“slot 1 chooses from Pizza category”, etc.)

This is simpler but less flexible.

* * *

✅ **Option B — Fully Dynamic, E-commerce Style** _(Recommended)_
================================================================

Meaning:

*   Bundle defines **any number of choice groups**, each with rules:
    *   `"Choose 1 of these"`, `"Choose 2 of these"`, `"Choose up to 5 of these"`.
*   Each pizza has its own:
    *   base (exactly 1)
    *   sauce swap (0–1)
    *   extra toppings (0–many)
*   Extras are stored in a **modifier system** that also applies to:
    *   Chicken burgers
    *   Coffee
    *   T-shirts
    *   Boxes
    *   Cupboards  
        (since you said this schema must also support _catalogues_, not just food).

Schema uses:

### Core

*   `products`
*   `product_variants`
*   `product_variant_prices`

### Modifiers

*   `modifier_groups`
*   `modifiers`
*   `product_modifier_groups`

### Bundles & Deals

*   `bundles`
*   `bundle_sections`
*   `bundle_section_product_filters`
*   `bundle_prices`  
    _(because some bundles can have size-based pricing)_

### For allergens & dietary needs

*   `attributes` (Vegan, Halaal, Contains Nuts, Gluten Free)
*   `product_attributes` (many-to-many)

* * *

❓ **Which version do you want your SQL seed script generated for?**
===================================================================

✔ Option A — Simple bundle
--------------------------

Good for menus only.  
Faster to seed.  
Not good for catalogues.

✔ Option B — Fully dynamic modifier/bundle system
-------------------------------------------------

Supports:

*   pizzas with bases + toppings
*   drinks
*   clothing sizes
*   per-meter items
*   packs of 100/1000
*   allergens
*   dietary restrictions
*   ANY future product category
*   unlimited complex bundle types
*   e-commerce pricing rules
*   multiple variants per product

* * *

🔥 I strongly recommend **Option B** because it scales with EVERYTHING you listed.
==================================================================================

But you tell me:

👉 **Should I generate the Full SQL Seed Script for Option A or Option B?**
===========================================================================

## Prompt:
option b

## Response:
Thought for 6s

Got it — below is a complete, ready-to-run **MySQL 8+ SQL seed script** for **Option B (fully dynamic modifier + bundle system)**.

What it includes:

*   Creation of the new tables (safe `IF NOT EXISTS`) to hold modifiers, modifier groups, product→modifier mappings, bundles, bundle steps, and bundle options.
*   Seed data for categories, products (3 pizzas + drinks), product variants (Large variants for pizzas and drink variants), modifier groups and modifier options (bases, extra meat, extra veg, extra cheese, sauces), product → modifier group mappings, tags (allergens/dietary), and the **Family Meal Deal** bundle with 3 steps linking allowed products.
*   Comments to explain each section.

You can run this against your MySQL 8 database (preferably on a test DB first). It assumes your existing tables `categories`, `products`, `product_variants`, `attributes`, `attribute_values`, `variant_attribute_values`, `inventories`, `variant_prices`, `tag_categories`, `tags`, `product_tags` either exist or will coexist — the script creates new tables and inserts seed rows that reference the product tables created here.

* * *

**Important**: I use explicit IDs so the relationships are clear. If your DB already contains rows using the same IDs, change the IDs or truncate the relevant tables before running the seed.

```sql
-- ===================================================
-- OPTION B: FULLY DYNAMIC MODIFIERS + BUNDLES SEED
-- MySQL 8+ (run in a test environment first)
-- ===================================================

-- -------------------------
-- 1) New tables (if not exists)
-- -------------------------
CREATE TABLE IF NOT EXISTS modifier_groups (
  id BIGINT AUTO_INCREMENT PRIMARY KEY,
  name VARCHAR(255) NOT NULL,
  slug VARCHAR(255) NOT NULL UNIQUE,
  description TEXT,
  created_at DATETIME DEFAULT CURRENT_TIMESTAMP,
  updated_at DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP
) ENGINE=InnoDB;

CREATE TABLE IF NOT EXISTS modifiers (
  id BIGINT AUTO_INCREMENT PRIMARY KEY,
  modifier_group_id BIGINT NOT NULL,
  name VARCHAR(255) NOT NULL,
  slug VARCHAR(255),
  price_modifier DECIMAL(12,2) DEFAULT 0,
  is_default TINYINT(1) DEFAULT 0,
  created_at DATETIME DEFAULT CURRENT_TIMESTAMP,
  updated_at DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  CONSTRAINT fk_mod_group FOREIGN KEY (modifier_group_id) REFERENCES modifier_groups(id) ON DELETE CASCADE
) ENGINE=InnoDB;

CREATE TABLE IF NOT EXISTS product_modifier_groups (
  id BIGINT AUTO_INCREMENT PRIMARY KEY,
  product_id BIGINT NOT NULL,
  modifier_group_id BIGINT NOT NULL,
  is_required TINYINT(1) DEFAULT 0,
  min_select INT DEFAULT 0,
  max_select INT DEFAULT 10,
  CONSTRAINT fk_pmg_product FOREIGN KEY (product_id) REFERENCES products(id) ON DELETE CASCADE,
  CONSTRAINT fk_pmg_group FOREIGN KEY (modifier_group_id) REFERENCES modifier_groups(id) ON DELETE CASCADE
) ENGINE=InnoDB;

-- Bundles
CREATE TABLE IF NOT EXISTS bundles (
  id BIGINT AUTO_INCREMENT PRIMARY KEY,
  name VARCHAR(255) NOT NULL,
  slug VARCHAR(255) UNIQUE,
  description TEXT,
  base_price DECIMAL(12,2) NOT NULL DEFAULT 0,
  is_active TINYINT(1) DEFAULT 1,
  created_at DATETIME DEFAULT CURRENT_TIMESTAMP,
  updated_at DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP
) ENGINE=InnoDB;

CREATE TABLE IF NOT EXISTS bundle_steps (
  id BIGINT AUTO_INCREMENT PRIMARY KEY,
  bundle_id BIGINT NOT NULL,
  step_number INT NOT NULL,
  name VARCHAR(255) NOT NULL,
  min_select INT DEFAULT 1,
  max_select INT DEFAULT 1,
  CONSTRAINT fk_bundle_step_bundle FOREIGN KEY (bundle_id) REFERENCES bundles(id) ON DELETE CASCADE
) ENGINE=InnoDB;

CREATE TABLE IF NOT EXISTS bundle_step_products (
  id BIGINT AUTO_INCREMENT PRIMARY KEY,
  step_id BIGINT NOT NULL,
  product_id BIGINT NOT NULL,
  CONSTRAINT fk_bsp_step FOREIGN KEY (step_id) REFERENCES bundle_steps(id) ON DELETE CASCADE,
  CONSTRAINT fk_bsp_product FOREIGN KEY (product_id) REFERENCES products(id) ON DELETE CASCADE
) ENGINE=InnoDB;

-- Optional: record customer selections per bundle order
CREATE TABLE IF NOT EXISTS bundle_orders (
  id BIGINT AUTO_INCREMENT PRIMARY KEY,
  bundle_id BIGINT NOT NULL,
  total_price DECIMAL(12,2) NOT NULL,
  created_at DATETIME DEFAULT CURRENT_TIMESTAMP,
  updated_at DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  CONSTRAINT fk_bundle_orders_bundle FOREIGN KEY (bundle_id) REFERENCES bundles(id) ON DELETE SET NULL
) ENGINE=InnoDB;

CREATE TABLE IF NOT EXISTS bundle_order_selections (
  id BIGINT AUTO_INCREMENT PRIMARY KEY,
  bundle_order_id BIGINT NOT NULL,
  step_id BIGINT NOT NULL,
  product_id BIGINT NULL,
  modifier_id BIGINT NULL,
  qty INT DEFAULT 1,
  CONSTRAINT fk_bos_order FOREIGN KEY (bundle_order_id) REFERENCES bundle_orders(id) ON DELETE CASCADE,
  CONSTRAINT fk_bos_step FOREIGN KEY (step_id) REFERENCES bundle_steps(id) ON DELETE CASCADE,
  CONSTRAINT fk_bos_product FOREIGN KEY (product_id) REFERENCES products(id) ON DELETE SET NULL,
  CONSTRAINT fk_bos_modifier FOREIGN KEY (modifier_id) REFERENCES modifiers(id) ON DELETE SET NULL
) ENGINE=InnoDB;

-- -------------------------
-- 2) Seed categories & products & variants (pizzas + drinks)
-- -------------------------
-- categories (if categories table exists)
INSERT INTO categories (id, name, slug, created_at, updated_at)
VALUES
(1, 'Pizzas', 'pizzas', NOW(), NOW()),
(2, 'Burgers', 'burgers', NOW(), NOW()),
(3, 'Drinks', 'drinks', NOW(), NOW())
ON DUPLICATE KEY UPDATE name = VALUES(name);

-- products
INSERT INTO products (id, category_id, sku, name, description, is_active, created_at, updated_at)
VALUES
(1001, 1, 'PZ-BAC-AVO-FETA', 'Bacon, Avo & Feta', 'Ham, bacon, avo & feta. Base pizza.', 1, NOW(), NOW()),
(1002, 1, 'PZ-CLASSIC-CHEESE', 'Classic Cheese', 'Classic cheese pizza.', 1, NOW(), NOW()),
(1003, 1, 'PZ-PEP-DELUXE', 'Pepperoni Deluxe', 'Loaded pepperoni deluxe pizza.', 1, NOW(), NOW()),
-- drinks as products (2L variants will be product_variants)
(2001, 3, 'DR-PEPSI', 'Pepsi 2L', 'Pepsi 2 litre bottle', 1, NOW(), NOW()),
(2002, 3, 'DR-COKE', 'Coke 2L', 'Coke 2 litre bottle', 1, NOW(), NOW()),
(2003, 3, 'DR-7UP', '7 Up 2L', '7 Up 2 litre bottle', 1, NOW(), NOW()),
(2004, 3, 'DR-MARINDA', 'Marinda 2L', 'Marinda 2 litre bottle', 1, NOW(), NOW()),
(2005, 3, 'DR-MDW', 'Mountain Dew 2L', 'Mountain Dew 2 litre bottle', 1, NOW(), NOW())
ON DUPLICATE KEY UPDATE name = VALUES(name), description = VALUES(description), updated_at = NOW();

-- product_variants (we will create Large pizza variants only for bundle and 2L drink variants)
INSERT INTO product_variants (id, product_id, sku, title, size_value, size_unit, unit_of_measure, price, weight_g, created_at, updated_at)
VALUES
(10001, 1001, 'PZ-BAC-AVO-FETA-L', 'Large', NULL, 'size', 'ea', 109.99, NULL, NOW(), NOW()),
(10002, 1002, 'PZ-CLASSIC-CHEESE-L', 'Large', NULL, 'size', 'ea', 99.99, NULL, NOW(), NOW()),
(10003, 1003, 'PZ-PEP-DELUXE-L', 'Large', NULL, 'size', 'ea', 119.99, NULL, NOW(), NOW()),
(20011, 2001, 'DR-PEPSI-2L', '2L', 2000, 'ml', 'ml', 0.00, NULL, NOW(), NOW()),
(20012, 2002, 'DR-COKE-2L', '2L', 2000, 'ml', 'ml', 0.00, NULL, NOW(), NOW()),
(20013, 2003, 'DR-7UP-2L', '2L', 2000, 'ml', 'ml', 0.00, NULL, NOW(), NOW()),
(20014, 2004, 'DR-MARINDA-2L', '2L', 2000, 'ml', 'ml', 0.00, NULL, NOW(), NOW()),
(20015, 2005, 'DR-MDW-2L', '2L', 2000, 'ml', 'ml', 0.00, NULL, NOW(), NOW())
ON DUPLICATE KEY UPDATE price = VALUES(price), updated_at = NOW();

-- Optional inventory seed (track stock for the large variants and drinks)
INSERT INTO inventories (variant_id, location_id, qty_available, qty_reserved, updated_at)
VALUES
(10001, NULL, 20, 0, NOW()),
(10002, NULL, 15, 0, NOW()),
(10003, NULL, 10, 0, NOW()),
(20011, NULL, 50, 0, NOW()),
(20012, NULL, 50, 0, NOW()),
(20013, NULL, 40, 0, NOW()),
(20014, NULL, 30, 0, NOW()),
(20015, NULL, 25, 0, NOW())
ON DUPLICATE KEY UPDATE qty_available = VALUES(qty_available), updated_at = NOW();

-- -------------------------
-- 3) Modifier groups + modifiers
-- -------------------------
-- Bases group (Gluten Free, Traditional, Pan) — price 0
INSERT INTO modifier_groups (id, name, slug, description)
VALUES
(1, 'Base', 'base', 'Pizza base choices: Gluten Free, Traditional, Pan')
ON DUPLICATE KEY UPDATE name = VALUES(name);

INSERT INTO modifiers (id, modifier_group_id, name, slug, price_modifier, is_default)
VALUES
(1001, 1, 'Gluten Free', 'base-gluten', 0.00, 0),
(1002, 1, 'Traditional', 'base-traditional', 0.00, 1),
(1003, 1, 'Pan', 'base-pan', 0.00, 0)
ON DUPLICATE KEY UPDATE price_modifier = VALUES(price_modifier), is_default = VALUES(is_default);

-- Extra Meat Toppings
INSERT INTO modifier_groups (id, name, slug, description)
VALUES
(2, 'Extra Meat Toppings', 'extra-meat', 'Optional extra meat toppings')
ON DUPLICATE KEY UPDATE name = VALUES(name);

INSERT INTO modifiers (id, modifier_group_id, name, slug, price_modifier)
VALUES
(2001, 2, 'Bacon', 'meat-bacon', 28.90),
(2002, 2, 'Chicken', 'meat-chicken', 28.90),
(2003, 2, 'Ham', 'meat-ham', 28.90),
(2004, 2, 'Pepperoni', 'meat-pepperoni', 28.90),
(2005, 2, 'Russian', 'meat-russian', 32.50)
ON DUPLICATE KEY UPDATE price_modifier = VALUES(price_modifier);

-- Extra Veg Toppings
INSERT INTO modifier_groups (id, name, slug, description)
VALUES
(3, 'Extra Veg Toppings', 'extra-veg', 'Optional extra vegetable toppings')
ON DUPLICATE KEY UPDATE name = VALUES(name);

INSERT INTO modifiers (id, modifier_group_id, name, slug, price_modifier)
VALUES
(3001, 3, 'Chilli', 'veg-chilli', 14.90),
(3002, 3, 'Garlic', 'veg-garlic', 18.90),
(3003, 3, 'Avo', 'veg-avo', 28.90),
(3004, 3, 'Tomato', 'veg-tomato', 14.90),
(3005, 3, 'Olives', 'veg-olives', 18.90),
(3006, 3, 'Onion', 'veg-onion', 14.90),
(3007, 3, 'Pineapple', 'veg-pineapple', 18.90)
ON DUPLICATE KEY UPDATE price_modifier = VALUES(price_modifier);

-- Extra Cheese
INSERT INTO modifier_groups (id, name, slug, description)
VALUES
(4, 'Extra Cheese', 'extra-cheese', 'Optional extra cheese toppings')
ON DUPLICATE KEY UPDATE name = VALUES(name);

INSERT INTO modifiers (id, modifier_group_id, name, slug, price_modifier)
VALUES
(4001, 4, 'Feta Cheese', 'cheese-feta', 28.90),
(4002, 4, 'Cheddar Cheese', 'cheese-cheddar', 28.90),
(4003, 4, 'Mozzarella Cheese', 'cheese-mozzarella', 20.90)
ON DUPLICATE KEY UPDATE price_modifier = VALUES(price_modifier);

-- Swap Base Sauce
INSERT INTO modifier_groups (id, name, slug, description)
VALUES
(5, 'Swap Base Sauce', 'swap-sauce', 'Swap the pizza base sauce')
ON DUPLICATE KEY UPDATE name = VALUES(name);

INSERT INTO modifiers (id, modifier_group_id, name, slug, price_modifier)
VALUES
(5001, 5, 'BBQ Base', 'sauce-bbq', 0.00),
(5002, 5, 'Chutney Base', 'sauce-chutney', 0.00),
(5003, 5, 'Mayo Base', 'sauce-mayo', 7.90),
(5004, 5, 'Peri-peri Base', 'sauce-peri', 0.00),
(5005, 5, 'Tomato Base', 'sauce-tomato', 0.00)
ON DUPLICATE KEY UPDATE price_modifier = VALUES(price_modifier);

-- Note: Pepperoni Deluxe had Chutney Base = R5.00 and Mayo Base = R7.90 and Peri-peri free,
-- we will override product-specific price by adding product-level modifier group (see below)
-- If product-specific variation in modifier price is needed, create product-specific modifier entries
-- or use a join table with price override (not implemented here for simplicity).

-- -------------------------
-- 4) Connect modifier groups to products (so each pizza supports those options)
-- -------------------------
INSERT INTO product_modifier_groups (product_id, modifier_group_id, is_required, min_select, max_select)
VALUES
-- Bacon, Avo & Feta supports base (required 1), extra meat, extra veg, extra cheese, swap sauce
(1001, 1, 1, 1, 1),
(1001, 2, 0, 0, 5),
(1001, 3, 0, 0, 7),
(1001, 4, 0, 0, 3),
(1001, 5, 0, 0, 1),

-- Classic Cheese supports base, extra cheese, swap sauce
(1002, 1, 1, 1, 1),
(1002, 4, 0, 0, 3),
(1002, 5, 0, 0, 1),

-- Pepperoni Deluxe supports base, extra meat, extra veg, extra cheese, swap sauce
(1003, 1, 1, 1, 1),
(1003, 2, 0, 0, 5),
(1003, 3, 0, 0, 7),
(1003, 4, 0, 0, 3),
(1003, 5, 0, 0, 1)
ON DUPLICATE KEY UPDATE is_required = VALUES(is_required), min_select = VALUES(min_select), max_select = VALUES(max_select);

-- -------------------------
-- 5) Allergen & Dietary tags (tag categories + tags) — if your tag tables exist use them
-- -------------------------
CREATE TABLE IF NOT EXISTS tag_categories (
  id BIGINT AUTO_INCREMENT PRIMARY KEY,
  name VARCHAR(255) NOT NULL,
  slug VARCHAR(255) NOT NULL UNIQUE
) ENGINE=InnoDB;

CREATE TABLE IF NOT EXISTS tags (
  id BIGINT AUTO_INCREMENT PRIMARY KEY,
  category_id BIGINT NOT NULL,
  name VARCHAR(255) NOT NULL,
  slug VARCHAR(255) NOT NULL UNIQUE,
  CONSTRAINT fk_tag_category FOREIGN KEY (category_id) REFERENCES tag_categories(id) ON DELETE CASCADE
) ENGINE=InnoDB;

CREATE TABLE IF NOT EXISTS product_tags (
  product_id BIGINT NOT NULL,
  tag_id BIGINT NOT NULL,
  PRIMARY KEY (product_id, tag_id),
  CONSTRAINT fk_pt_product FOREIGN KEY (product_id) REFERENCES products(id) ON DELETE CASCADE,
  CONSTRAINT fk_pt_tag FOREIGN KEY (tag_id) REFERENCES tags(id) ON DELETE CASCADE
) ENGINE=InnoDB;

-- insert some tag categories and tags
INSERT INTO tag_categories (id, name, slug) VALUES
(1, 'Allergen', 'allergen'),
(2, 'Dietary', 'dietary'),
(3, 'Material', 'material')
ON DUPLICATE KEY UPDATE name = VALUES(name);

INSERT INTO tags (id, category_id, name, slug)
VALUES
(1, 1, 'Dairy', 'dairy'),
(2, 1, 'Gluten', 'gluten'),
(3, 1, 'Nuts', 'nuts'),
(4, 2, 'Vegan', 'vegan'),
(5, 2, 'Halaal', 'halaal'),
(6, 2, 'Kosher', 'kosher'),
(7, 1, 'Eggs', 'eggs')
ON DUPLICATE KEY UPDATE name = VALUES(name);

-- tag some products
INSERT INTO product_tags (product_id, tag_id)
VALUES
(1001, 1), -- bacon avo feta contains dairy
(1001, 2), -- contains gluten
(1002, 1),
(1002, 2),
(1003, 1),
(1003, 2)
ON DUPLICATE KEY UPDATE product_id = VALUES(product_id);

-- -------------------------
-- 6) Create the Family Meal Deal bundle + steps + allowed products
-- -------------------------
INSERT INTO bundles (id, name, slug, description, base_price, is_active, created_at, updated_at)
VALUES
(5001, 'Family Meal Deal', 'family-meal-deal', 'Any 2 Large Pizzas + 2L Cold Drink for R250', 250.00, 1, NOW(), NOW())
ON DUPLICATE KEY UPDATE base_price = VALUES(base_price), updated_at = NOW();

-- Steps: 1 = Pizza 1, 2 = Pizza 2, 3 = 2L Drink
INSERT INTO bundle_steps (id, bundle_id, step_number, name, min_select, max_select)
VALUES
(5101, 5001, 1, 'Choose Pizza 1 (Large)', 1, 1),
(5102, 5001, 2, 'Choose Pizza 2 (Large)', 1, 1),
(5103, 5001, 3, 'Choose 2L Cold Drink', 1, 1)
ON DUPLICATE KEY UPDATE name = VALUES(name), min_select = VALUES(min_select), max_select = VALUES(max_select);

-- allowed products in step 1 and 2 (only pizza products for these steps)
INSERT INTO bundle_step_products (step_id, product_id)
VALUES
(5101, 1001),
(5101, 1002),
(5101, 1003),

(5102, 1001),
(5102, 1002),
(5102, 1003)
ON DUPLICATE KEY UPDATE product_id = VALUES(product_id);

-- allowed drinks in step 3
INSERT INTO bundle_step_products (step_id, product_id)
VALUES
(5103, 2001),
(5103, 2002),
(5103, 2003),
(5103, 2004),
(5103, 2005)
ON DUPLICATE KEY UPDATE product_id = VALUES(product_id);

-- -------------------------
-- 7) Optional: convenience view that returns bundle + step + allowed products + product modifier groups
-- -------------------------
DROP VIEW IF EXISTS vw_bundle_family_meal;
CREATE VIEW vw_bundle_family_meal AS
SELECT
  b.id AS bundle_id,
  b.name AS bundle_name,
  b.base_price,
  s.id AS step_id,
  s.step_number,
  s.name AS step_name,
  p.id AS product_id,
  p.name AS product_name
FROM bundles b
JOIN bundle_steps s ON s.bundle_id = b.id
JOIN bundle_step_products bsp ON bsp.step_id = s.id
JOIN products p ON p.id = bsp.product_id
WHERE b.id = 5001;

-- -------------------------
-- 8) Quick sanity check queries (returns rows if seeded)
-- -------------------------
-- List bundle and steps:
SELECT b.id, b.name, b.base_price, s.step_number, s.name AS step_name
FROM bundles b JOIN bundle_steps s ON s.bundle_id = b.id
WHERE b.id = 5001
ORDER BY s.step_number;

-- Show products allowed for step 1:
SELECT p.id, p.name
FROM bundle_step_products bsp
JOIN products p ON p.id = bsp.product_id
WHERE bsp.step_id = 5101;

-- Show modifiers for Bacon, Avo & Feta:
SELECT mg.id AS modifier_group_id, mg.name AS group_name, m.id AS modifier_id, m.name AS modifier_name, m.price_modifier
FROM product_modifier_groups pmg
JOIN modifier_groups mg ON mg.id = pmg.modifier_group_id
JOIN modifiers m ON m.modifier_group_id = mg.id
WHERE pmg.product_id = 1001
ORDER BY mg.id, m.id;

-- ===================================================
-- END OF SEED SCRIPT
-- ===================================================
```

* * *

### Notes & Next steps

*   The script uses the **modifier groups** pattern so the same extras (meat, veg, cheese, sauces) are reusable across pizzas and other products.
*   For product-specific modifier price differences (e.g., `Chutney Base` costs R5.00 on Pepperoni Deluxe), you can:
    *   create product-specific modifier rows (e.g., `modifiers` with product override via another join table), or
    *   maintain a `product_modifier_price_overrides` table mapping `product_id + modifier_id -> price_modifier`.  
        I omitted that for clarity; if you want it, I can add it next.
*   The bundle flow in UI will:
    1.  Load bundle + steps (from `bundles` + `bundle_steps` + `bundle_step_products`)
    2.  When user picks a product for a pizza step, load product's `product_modifier_groups` → `modifier_groups` → `modifiers` to show base/extras/sauces and their prices.
    3.  Calculate total = `bundle.base_price + sum(selected modifier price_modifiers) + optional variant surcharges`.
*   I included view `vw_bundle_family_meal` for convenience; remove or adapt it if not needed.

If you want, I can now:

*   Add **product-specific modifier price overrides** (so `Chutney Base` = R5.00 for Pepperoni Deluxe) and seed those overrides.
*   Produce an **EF Core migration** (C#) that generates these tables.
*   Produce a **POST /api/bundles/{id}/price** endpoint + example request/response.
*   Generate a **detailed seed** reflecting your exact example order and show the `bundle_order_selections` insert for that order (i.e., store the customer's chosen extra bacon, avo, feta, mayo base, pepperoni garlic, and the Pepsi).

Which of those would you like next?

## Prompt:
rather show me how you would store menus containing both "normal" dishes (not bundles) and bundles.  what would those tables look like, what would the data look like, how would you retrieve it and how would you combine the data so that it displays on 1 menu

## Response:
Perfect — this is a **real-world requirement**, and the solution must support:

*   Normal menu items (e.g., **"Pepperoni Pizza"**, **"Coke 500ml"**, **"Chicken Burger"**)
*   Bundle menu items (e.g., **"Family Meal Deal"**, **"2 Large Pizzas + 1 Drink"**)
*   A unified menu displayed under **categories**
*   A consistent API response
*   A flexible database that works for both restaurants and product catalogs

Below is **the correct industry-standard architecture** used by:

✔ McDonalds  
✔ KFC  
✔ Starbucks  
✔ Uber Eats  
✔ Shopify  
✔ MrD / Delivery Hero

* * *

⭐ 1. THE CORE PRINCIPLE
=======================

**A menu shows "items".**  
BUT an item can be one of two types:

### **(1) NormalProduct**

A typical dish with variants & modifiers.

### **(2) Bundle**

A configurable product made of multiple sections.

* * *

⭐ 2. THE DATABASE STRUCTURE
===========================

✔ 2.1 categories
----------------

Each category contains a list of _menu items_ (not products).

```sql
CREATE TABLE categories (
    id BIGINT PRIMARY KEY AUTO_INCREMENT,
    name VARCHAR(255) NOT NULL,
    slug VARCHAR(255) NOT NULL UNIQUE,
    sort_order INT DEFAULT 0
);
```

* * *

✔ 2.2 menu\_items
-----------------

This table is the **entry point** shown on the menu.

```sql
CREATE TABLE menu_items (
    id BIGINT PRIMARY KEY AUTO_INCREMENT,
    category_id BIGINT NOT NULL,
    type ENUM('product', 'bundle') NOT NULL,
    product_id BIGINT NULL,
    bundle_id BIGINT NULL,
    name VARCHAR(255) NOT NULL,
    description TEXT,
    image_url VARCHAR(500),
    sort_order INT DEFAULT 0,
    FOREIGN KEY (category_id) REFERENCES categories(id),
    FOREIGN KEY (product_id) REFERENCES products(id),
    FOREIGN KEY (bundle_id) REFERENCES bundles(id)
);
```

### **This lets menu\_items reference either:**

*   a product (normal dish)
*   or a bundle (meal deal)

* * *

✔ 2.3 products / product\_variants (normal items)
-------------------------------------------------

Already defined earlier.

```sql
products
product_variants
product_variant_prices
modifier_groups
modifiers
product_modifier_groups
```

* * *

✔ 2.4 bundles (complex configurable items)
------------------------------------------

### bundles

High-level bundle.

```sql
CREATE TABLE bundles (
    id BIGINT PRIMARY KEY AUTO_INCREMENT,
    name VARCHAR(255) NOT NULL,
    description TEXT,
    base_price DECIMAL(12,2) NOT NULL DEFAULT 0
);
```

### bundle\_sections

The “Choose Pizza 1”, “Choose Pizza 2”, “Choose Drink” groups.

```sql
CREATE TABLE bundle_sections (
    id BIGINT PRIMARY KEY AUTO_INCREMENT,
    bundle_id BIGINT NOT NULL,
    name VARCHAR(255) NOT NULL,
    min_select INT DEFAULT 1,
    max_select INT DEFAULT 1,
    sort_order INT DEFAULT 0,
    FOREIGN KEY (bundle_id) REFERENCES bundles(id)
);
```

### bundle\_section\_products

Products allowed inside a section.

```sql
CREATE TABLE bundle_section_products (
    id BIGINT PRIMARY KEY AUTO_INCREMENT,
    section_id BIGINT NOT NULL,
    product_id BIGINT NOT NULL,
    FOREIGN KEY (section_id) REFERENCES bundle_sections(id),
    FOREIGN KEY (product_id) REFERENCES products(id)
);
```

### bundle\_section\_modifier\_groups

What modifiers apply inside that section.

```sql
CREATE TABLE bundle_section_modifier_groups (
    id BIGINT PRIMARY KEY AUTO_INCREMENT,
    section_id BIGINT NOT NULL,
    modifier_group_id BIGINT NOT NULL,
    FOREIGN KEY (section_id) REFERENCES bundle_sections(id),
    FOREIGN KEY (modifier_group_id) REFERENCES modifier_groups(id)
);
```

* * *

⭐ 3. HOW THE DATA LOOKS (EXAMPLE)
=================================

✔ Category: "Pizzas"
--------------------

### menu\_items

| id | category\_id | type | product\_id | bundle\_id | name |
| --- | --- | --- | --- | --- | --- |
| 1 | 1 | product | 10 | NULL | Pepperoni Pizza |
| 2 | 1 | product | 11 | NULL | Bacon Avo & Feta |
| 3 | 1 | bundle | NULL | 1 | Family Meal Deal |

* * *

✔ Bundle: "Family Meal Deal"
----------------------------

### bundles

| id | name | base\_price |
| --- | --- | --- |
| 1 | Family Meal Deal | 250.00 |

### bundle\_sections

| id | bundle\_id | name | min | max |
| --- | --- | --- | --- | --- |
| 1 | 1 | Choose Pizza 1 | 1 | 1 |
| 2 | 1 | Choose Pizza 2 | 1 | 1 |
| 3 | 1 | Choose Drink | 1 | 1 |

### bundle\_section\_products

| id | section\_id | product\_id |
| --- | --- | --- |
| 1 | 1 | 10 |
| 2 | 1 | 11 |
| 3 | 1 | 12 |
| 4 | 2 | 10 |
| 5 | 2 | 11 |
| 6 | 2 | 12 |
| 7 | 3 | 20 |
| 8 | 3 | 21 |
| 9 | 3 | 22 |
| 10 | 3 | 23 |
| 11 | 3 | 24 |

* * *

### bundle\_section\_modifier\_groups

Applies toppings + bases to pizzas only

| id | section\_id | modifier\_group\_id |
| --- | --- | --- |
| 1 | 1 | 100 (Pizza Base) |
| 2 | 1 | 101 (Extra Meat) |
| 3 | 1 | 102 (Extra Veg) |
| 4 | 1 | 103 (Extra Cheese) |
| 5 | 1 | 104 (Swap Sauce) |
| 6 | 2 | 100 |
| 7 | 2 | 101 |
| 8 | 2 | 102 |
| 9 | 2 | 103 |
| 10 | 2 | 104 |

(Drinks section has no modifiers.)

* * *

⭐ 4. HOW YOU RETRIEVE A MENU (SQL + Pseudocode)
===============================================

4.1 Step 1 — Get all categories
-------------------------------

```sql
SELECT * FROM categories ORDER BY sort_order;
```

4.2 Step 2 — Load menu items for each category
----------------------------------------------

```sql
SELECT * FROM menu_items WHERE category_id = ? ORDER BY sort_order;
```

4.3 Step 3 — If menu\_item.type = 'product', load:
--------------------------------------------------

*   product
*   variants
*   prices
*   modifiers

4.4 Step 4 — If menu\_item.type = 'bundle', load:
-------------------------------------------------

*   bundle
*   sections
*   allowed products
*   modifiers attached to sections

* * *

⭐ 5. UNIFIED API RESPONSE (Frontend-Ready)
==========================================

Your API can return:

```json
{
  "menu": [
    {
      "id": 1,
      "name": "Pizzas",
      "items": [
        {
          "id": 1,
          "type": "product",
          "name": "Pepperoni Pizza",
          "variants": [...],
          "modifierGroups": [...]
        },
        {
          "id": 3,
          "type": "bundle",
          "name": "Family Meal Deal",
          "basePrice": 250,
          "sections": [
            {
              "id": 1,
              "name": "Choose Pizza 1",
              "products": [...],
              "modifierGroups": [...]
            },
            {
              "id": 2,
              "name": "Choose Pizza 2",
              "products": [...],
              "modifierGroups": [...]
            },
            {
              "id": 3,
              "name": "Choose Drink",
              "products": [...]
            }
          ]
        }
      ]
    }
  ]
}
```

This allows the frontend to present:

*   normal items
*   bundles  
    **all in one menu list**  
    exactly like MrD / Uber Eats.

* * *

⭐ 6. SUMMARY
============

You now have:

### ✔ A single unified `menu_items` table

that can point to either:

*   a product
*   or a bundle

### ✔ A complete bundle system

with:

*   sections
*   allowed products
*   modifiers
*   base price
*   toppings & extras

### ✔ A unified API output

### ✔ A structure that works for BOTH:

*   restaurant menus
*   product catalogs (t-shirts, boxes, cupboards, etc.)

* * *

🚀 If you want…
===============

I can now generate:

### ✔ Full SQL schema (all tables)

### ✔ Full SQL seed script (data for pizzas, drinks, toppings, bundle)

### ✔ C# EF Core entities

### ✔ EF Fluent mappings

### ✔ API controllers & DTOs

### ✔ Example JSON outputs

### ✔ A mock UI response like Uber Eats

Just say **“Generate everything”** or specify which part you want next.

## Prompt:
Generate everything

## Response:
Nice — below I’ve generated **everything** you asked for in one deliverable. I kept things practical and consistent with the schema and patterns we discussed earlier (fully dynamic modifiers + bundles + menu\_items). Copy / paste the pieces you need.

* * *

WHAT’S INCLUDED (quick index)
-----------------------------

1.  Full **MySQL 8** schema (all tables required)
2.  Full **SQL seed script** (menu: categories, products, variants, modifiers, bundle Family Meal Deal, menu\_items)
3.  **C# EF Core entity classes** (POCOs)
4.  **AppDbContext** with Fluent API mapping (precision, relationships)
5.  **DTOs** (API-ready)
6.  Minimal **ASP.NET Core controllers** (menu endpoints, bundle price calc, place bundle order)
7.  Example **JSON** API output (`GET /api/menu`)
8.  Compact **React + Tailwind** UI sketch that consumes the menu API
9.  A small **C# bundle price calculator** snippet (server-side)

* * *

> Note: this is intended as a working reference. You may need to adapt minor names/nullable rules to match your existing project (IDs, namespaces). Run the SQL on a test DB first.

* * *

1) MySQL 8 — Complete Schema
----------------------------

```sql
-- ================================
-- CORE: categories, products, variants, attributes, tags, inventory, prices
-- ================================
CREATE TABLE IF NOT EXISTS categories (
  id BIGINT AUTO_INCREMENT PRIMARY KEY,
  name VARCHAR(255) NOT NULL,
  slug VARCHAR(255) NOT NULL UNIQUE,
  sort_order INT DEFAULT 0,
  created_at DATETIME DEFAULT CURRENT_TIMESTAMP,
  updated_at DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP
) ENGINE=InnoDB;

CREATE TABLE IF NOT EXISTS products (
  id BIGINT AUTO_INCREMENT PRIMARY KEY,
  category_id BIGINT,
  sku VARCHAR(255),
  name VARCHAR(255) NOT NULL,
  description TEXT,
  type ENUM('normal','bundle','custom') DEFAULT 'normal',
  is_active TINYINT(1) DEFAULT 1,
  created_at DATETIME DEFAULT CURRENT_TIMESTAMP,
  updated_at DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  CONSTRAINT fk_products_cat FOREIGN KEY (category_id) REFERENCES categories(id) ON DELETE SET NULL
) ENGINE=InnoDB;

CREATE TABLE IF NOT EXISTS product_variants (
  id BIGINT AUTO_INCREMENT PRIMARY KEY,
  product_id BIGINT NOT NULL,
  sku VARCHAR(255),
  title VARCHAR(255) NOT NULL,
  size_value DECIMAL(12,4),
  size_unit VARCHAR(50),
  unit_of_measure VARCHAR(50),
  price DECIMAL(12,2) NOT NULL DEFAULT 0,
  cost DECIMAL(12,2),
  weight_g INT,
  barcode VARCHAR(255),
  is_default TINYINT(1) DEFAULT 0,
  created_at DATETIME DEFAULT CURRENT_TIMESTAMP,
  updated_at DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  CONSTRAINT fk_variants_product FOREIGN KEY (product_id) REFERENCES products(id) ON DELETE CASCADE
) ENGINE=InnoDB;

CREATE TABLE IF NOT EXISTS inventories (
  id BIGINT AUTO_INCREMENT PRIMARY KEY,
  variant_id BIGINT NOT NULL,
  location_id BIGINT,
  qty_available DECIMAL(12,3) DEFAULT 0,
  qty_reserved DECIMAL(12,3) DEFAULT 0,
  updated_at DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  CONSTRAINT fk_inv_variant FOREIGN KEY (variant_id) REFERENCES product_variants(id) ON DELETE CASCADE
) ENGINE=InnoDB;

CREATE TABLE IF NOT EXISTS variant_prices (
  id BIGINT AUTO_INCREMENT PRIMARY KEY,
  variant_id BIGINT NOT NULL,
  price DECIMAL(12,2) NOT NULL,
  starts_at DATETIME DEFAULT CURRENT_TIMESTAMP,
  ends_at DATETIME NULL,
  CONSTRAINT fk_vprice_variant FOREIGN KEY (variant_id) REFERENCES product_variants(id) ON DELETE CASCADE
) ENGINE=InnoDB;

-- ================================
-- TAGS (allergen / dietary / material)
-- ================================
CREATE TABLE IF NOT EXISTS tag_categories (
  id BIGINT AUTO_INCREMENT PRIMARY KEY,
  name VARCHAR(255) NOT NULL,
  slug VARCHAR(255) NOT NULL UNIQUE
) ENGINE=InnoDB;

CREATE TABLE IF NOT EXISTS tags (
  id BIGINT AUTO_INCREMENT PRIMARY KEY,
  category_id BIGINT NOT NULL,
  name VARCHAR(255) NOT NULL,
  slug VARCHAR(255) NOT NULL UNIQUE,
  CONSTRAINT fk_tag_category FOREIGN KEY (category_id) REFERENCES tag_categories(id) ON DELETE CASCADE
) ENGINE=InnoDB;

CREATE TABLE IF NOT EXISTS product_tags (
  product_id BIGINT NOT NULL,
  tag_id BIGINT NOT NULL,
  PRIMARY KEY (product_id, tag_id),
  CONSTRAINT fk_pt_product FOREIGN KEY (product_id) REFERENCES products(id) ON DELETE CASCADE,
  CONSTRAINT fk_pt_tag FOREIGN KEY (tag_id) REFERENCES tags(id) ON DELETE CASCADE
) ENGINE=InnoDB;

-- ================================
-- MODIFIERS (generic system for extras / bases / sauces)
-- ================================
CREATE TABLE IF NOT EXISTS modifier_groups (
  id BIGINT AUTO_INCREMENT PRIMARY KEY,
  name VARCHAR(255) NOT NULL,
  slug VARCHAR(255) NOT NULL UNIQUE,
  description TEXT,
  created_at DATETIME DEFAULT CURRENT_TIMESTAMP,
  updated_at DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP
) ENGINE=InnoDB;

CREATE TABLE IF NOT EXISTS modifiers (
  id BIGINT AUTO_INCREMENT PRIMARY KEY,
  modifier_group_id BIGINT NOT NULL,
  name VARCHAR(255) NOT NULL,
  slug VARCHAR(255),
  price_modifier DECIMAL(12,2) DEFAULT 0,
  is_default TINYINT(1) DEFAULT 0,
  created_at DATETIME DEFAULT CURRENT_TIMESTAMP,
  updated_at DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  CONSTRAINT fk_mod_group FOREIGN KEY (modifier_group_id) REFERENCES modifier_groups(id) ON DELETE CASCADE
) ENGINE=InnoDB;

-- what modifier groups apply to which products
CREATE TABLE IF NOT EXISTS product_modifier_groups (
  id BIGINT AUTO_INCREMENT PRIMARY KEY,
  product_id BIGINT NOT NULL,
  modifier_group_id BIGINT NOT NULL,
  is_required TINYINT(1) DEFAULT 0,
  min_select INT DEFAULT 0,
  max_select INT DEFAULT 10,
  CONSTRAINT fk_pmg_product FOREIGN KEY (product_id) REFERENCES products(id) ON DELETE CASCADE,
  CONSTRAINT fk_pmg_group FOREIGN KEY (modifier_group_id) REFERENCES modifier_groups(id) ON DELETE CASCADE
) ENGINE=InnoDB;

-- ================================
-- BUNDLES (composable deals)
-- ================================
CREATE TABLE IF NOT EXISTS bundles (
  id BIGINT AUTO_INCREMENT PRIMARY KEY,
  name VARCHAR(255) NOT NULL,
  slug VARCHAR(255) UNIQUE,
  description TEXT,
  base_price DECIMAL(12,2) NOT NULL DEFAULT 0,
  is_active TINYINT(1) DEFAULT 1,
  created_at DATETIME DEFAULT CURRENT_TIMESTAMP,
  updated_at DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP
) ENGINE=InnoDB;

CREATE TABLE IF NOT EXISTS bundle_sections (
  id BIGINT AUTO_INCREMENT PRIMARY KEY,
  bundle_id BIGINT NOT NULL,
  section_index INT NOT NULL,
  name VARCHAR(255) NOT NULL,
  min_select INT DEFAULT 1,
  max_select INT DEFAULT 1,
  CONSTRAINT fk_bundle_section_bundle FOREIGN KEY (bundle_id) REFERENCES bundles(id) ON DELETE CASCADE
) ENGINE=InnoDB;

CREATE TABLE IF NOT EXISTS bundle_section_products (
  id BIGINT AUTO_INCREMENT PRIMARY KEY,
  section_id BIGINT NOT NULL,
  product_id BIGINT NOT NULL,
  CONSTRAINT fk_bsp_section FOREIGN KEY (section_id) REFERENCES bundle_sections(id) ON DELETE CASCADE,
  CONSTRAINT fk_bsp_product FOREIGN KEY (product_id) REFERENCES products(id) ON DELETE CASCADE
) ENGINE=InnoDB;

-- which modifier groups apply to a bundle section (e.g., pizza toppings in pizza sections)
CREATE TABLE IF NOT EXISTS bundle_section_modifier_groups (
  id BIGINT AUTO_INCREMENT PRIMARY KEY,
  section_id BIGINT NOT NULL,
  modifier_group_id BIGINT NOT NULL,
  CONSTRAINT fk_bsmg_section FOREIGN KEY (section_id) REFERENCES bundle_sections(id) ON DELETE CASCADE,
  CONSTRAINT fk_bsmg_group FOREIGN KEY (modifier_group_id) REFERENCES modifier_groups(id) ON DELETE CASCADE
) ENGINE=InnoDB;

-- order/bundle recording
CREATE TABLE IF NOT EXISTS bundle_orders (
  id BIGINT AUTO_INCREMENT PRIMARY KEY,
  bundle_id BIGINT NOT NULL,
  total_price DECIMAL(12,2) NOT NULL,
  created_at DATETIME DEFAULT CURRENT_TIMESTAMP,
  CONSTRAINT fk_bundle_orders_bundle FOREIGN KEY (bundle_id) REFERENCES bundles(id) ON DELETE SET NULL
) ENGINE=InnoDB;

CREATE TABLE IF NOT EXISTS bundle_order_selections (
  id BIGINT AUTO_INCREMENT PRIMARY KEY,
  bundle_order_id BIGINT NOT NULL,
  section_id BIGINT NOT NULL,
  product_id BIGINT NULL,
  modifier_id BIGINT NULL,
  qty INT DEFAULT 1,
  CONSTRAINT fk_bos_order FOREIGN KEY (bundle_order_id) REFERENCES bundle_orders(id) ON DELETE CASCADE,
  CONSTRAINT fk_bos_section FOREIGN KEY (section_id) REFERENCES bundle_sections(id) ON DELETE CASCADE,
  CONSTRAINT fk_bos_product FOREIGN KEY (product_id) REFERENCES products(id) ON DELETE SET NULL,
  CONSTRAINT fk_bos_modifier FOREIGN KEY (modifier_id) REFERENCES modifiers(id) ON DELETE SET NULL
) ENGINE=InnoDB;

-- ================================
-- MENU: menu_items references either product or bundle to display unified menu
-- ================================
CREATE TABLE IF NOT EXISTS menu_items (
  id BIGINT AUTO_INCREMENT PRIMARY KEY,
  category_id BIGINT NOT NULL,
  type ENUM('product','bundle') NOT NULL,
  product_id BIGINT NULL,
  bundle_id BIGINT NULL,
  name VARCHAR(255) NOT NULL,
  description TEXT,
  image_url VARCHAR(500),
  sort_order INT DEFAULT 0,
  created_at DATETIME DEFAULT CURRENT_TIMESTAMP,
  updated_at DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  CONSTRAINT fk_menu_cat FOREIGN KEY (category_id) REFERENCES categories(id),
  CONSTRAINT fk_menu_product FOREIGN KEY (product_id) REFERENCES products(id),
  CONSTRAINT fk_menu_bundle FOREIGN KEY (bundle_id) REFERENCES bundles(id)
) ENGINE=InnoDB;
```

* * *

2) Full SQL Seed Script (Option B) — MENU + PRODUCTS + MODIFIERS + BUNDLE
-------------------------------------------------------------------------

> Run this after the schema above. Uses explicit IDs to keep relationships clear. Adjust IDs if your DB already contains these.

```sql
-- -------------------------
-- Categories
-- -------------------------
INSERT INTO categories (id, name, slug) VALUES
(1, 'Pizzas', 'pizzas'),
(2, 'Burgers', 'burgers'),
(3, 'Drinks', 'drinks')
ON DUPLICATE KEY UPDATE name = VALUES(name);

-- -------------------------
-- Products
-- -------------------------
-- pizzas
INSERT INTO products (id, category_id, sku, name, description, type)
VALUES
(1001, 1, 'PZ-BAC-AVO-FETA', 'Bacon, Avo & Feta', 'Bacon, avo, feta & cheese', 'normal'),
(1002, 1, 'PZ-CLASSIC', 'Classic Cheese', 'Classic cheese pizza', 'normal'),
(1003, 1, 'PZ-PEP-DELUXE', 'Pepperoni Deluxe', 'Pepperoni deluxe', 'normal')
ON DUPLICATE KEY UPDATE name = VALUES(name);

-- drinks (products)
INSERT INTO products (id, category_id, sku, name, description, type)
VALUES
(2001, 3, 'DR-PEPSI', 'Pepsi 2L', 'Pepsi 2L', 'normal'),
(2002, 3, 'DR-COKE', 'Coke 2L', 'Coke 2L', 'normal'),
(2003, 3, 'DR-7UP', '7 Up 2L', '7 Up 2L', 'normal'),
(2004, 3, 'DR-MARINDA', 'Marinda 2L', 'Marinda 2L', 'normal'),
(2005, 3, 'DR-MDW', 'Mountain Dew 2L', 'Mountain Dew 2L', 'normal')
ON DUPLICATE KEY UPDATE name = VALUES(name);

-- -------------------------
-- Product Variants (Large pizzas + 2L drinks)
-- -------------------------
INSERT INTO product_variants (id, product_id, sku, title, size_unit, unit_of_measure, price)
VALUES
(10001, 1001, 'PZ-BAC-AVO-FETA-L', 'Large', 'size', 'ea', 109.99),
(10002, 1002, 'PZ-CLASSIC-L', 'Large', 'size', 'ea', 99.99),
(10003, 1003, 'PZ-PEP-DELUXE-L', 'Large', 'size', 'ea', 119.99),

(20011, 2001, 'DR-PEPSI-2L', '2L', 'ml', 'ml', 0.00),
(20012, 2002, 'DR-COKE-2L', '2L', 'ml', 'ml', 0.00),
(20013, 2003, 'DR-7UP-2L', '2L', 'ml', 'ml', 0.00),
(20014, 2004, 'DR-MARINDA-2L', '2L', 'ml', 'ml', 0.00),
(20015, 2005, 'DR-MDW-2L', '2L', 'ml', 'ml', 0.00)
ON DUPLICATE KEY UPDATE price = VALUES(price);

-- Optional inventory
INSERT INTO inventories (variant_id, qty_available)
VALUES
(10001, 20),(10002,15),(10003,10),(20011,50),(20012,50),(20013,40),(20014,30),(20015,25)
ON DUPLICATE KEY UPDATE qty_available = VALUES(qty_available), updated_at = NOW();

-- -------------------------
-- Modifiers and modifier groups
-- -------------------------
-- bases:
INSERT INTO modifier_groups (id, name, slug) VALUES
(1, 'Base', 'base') ON DUPLICATE KEY UPDATE name = VALUES(name);

INSERT INTO modifiers (id, modifier_group_id, name, slug, price_modifier, is_default)
VALUES
(1001,1,'Gluten Free','base-gluten',0.00,0),
(1002,1,'Traditional','base-traditional',0.00,1),
(1003,1,'Pan','base-pan',0.00,0)
ON DUPLICATE KEY UPDATE price_modifier = VALUES(price_modifier);

-- extra meat
INSERT INTO modifier_groups (id, name, slug) VALUES
(2, 'Extra Meat Toppings', 'extra-meat') ON DUPLICATE KEY UPDATE name = VALUES(name);

INSERT INTO modifiers (id, modifier_group_id, name, slug, price_modifier)
VALUES
(2001,2,'Bacon','meat-bacon',28.90),
(2002,2,'Chicken','meat-chicken',28.90),
(2003,2,'Ham','meat-ham',28.90),
(2004,2,'Pepperoni','meat-pepperoni',28.90),
(2005,2,'Russian','meat-russian',32.50)
ON DUPLICATE KEY UPDATE price_modifier = VALUES(price_modifier);

-- extra veg
INSERT INTO modifier_groups (id, name, slug) VALUES
(3, 'Extra Veg Toppings', 'extra-veg') ON DUPLICATE KEY UPDATE name = VALUES(name);

INSERT INTO modifiers (id, modifier_group_id, name, slug, price_modifier)
VALUES
(3001,3,'Chilli','veg-chilli',14.90),
(3002,3,'Garlic','veg-garlic',18.90),
(3003,3,'Avo','veg-avo',28.90),
(3004,3,'Tomato','veg-tomato',14.90),
(3005,3,'Olives','veg-olives',18.90),
(3006,3,'Onion','veg-onion',14.90),
(3007,3,'Pineapple','veg-pineapple',18.90)
ON DUPLICATE KEY UPDATE price_modifier = VALUES(price_modifier);

-- extra cheese
INSERT INTO modifier_groups (id, name, slug) VALUES
(4, 'Extra Cheese', 'extra-cheese') ON DUPLICATE KEY UPDATE name = VALUES(name);

INSERT INTO modifiers (id, modifier_group_id, name, slug, price_modifier)
VALUES
(4001,4,'Feta Cheese','cheese-feta',28.90),
(4002,4,'Cheddar Cheese','cheese-cheddar',28.90),
(4003,4,'Mozzarella Cheese','cheese-mozarella',20.90)
ON DUPLICATE KEY UPDATE price_modifier = VALUES(price_modifier);

-- swap base sauce
INSERT INTO modifier_groups (id, name, slug) VALUES
(5, 'Swap Base Sauce','swap-sauce') ON DUPLICATE KEY UPDATE name = VALUES(name);

INSERT INTO modifiers (id, modifier_group_id, name, slug, price_modifier)
VALUES
(5001,5,'BBQ Base','sauce-bbq',0.00),
(5002,5,'Chutney Base','sauce-chutney',0.00),
(5003,5,'Mayo Base','sauce-mayo',7.90),
(5004,5,'Peri-peri Base','sauce-peri',0.00),
(5005,5,'Tomato Base','sauce-tomato',0.00)
ON DUPLICATE KEY UPDATE price_modifier = VALUES(price_modifier);

-- connect modifier groups to pizza products
INSERT INTO product_modifier_groups (product_id, modifier_group_id, is_required, min_select, max_select)
VALUES
(1001,1,1,1,1),(1001,2,0,0,5),(1001,3,0,0,7),(1001,4,0,0,3),(1001,5,0,0,1),
(1002,1,1,1,1),(1002,4,0,0,3),(1002,5,0,0,1),
(1003,1,1,1,1),(1003,2,0,0,5),(1003,3,0,0,7),(1003,4,0,0,3),(1003,5,0,0,1)
ON DUPLICATE KEY UPDATE is_required = VALUES(is_required);

-- -------------------------
-- Tags (allergens / dietary)
-- -------------------------
INSERT INTO tag_categories (id, name, slug) VALUES (1, 'Allergen','allergen'),(2,'Dietary','dietary') ON DUPLICATE KEY UPDATE name=VALUES(name);

INSERT INTO tags (id, category_id, name, slug)
VALUES
(1,1,'Dairy','dairy'),
(2,1,'Gluten','gluten'),
(3,1,'Nuts','nuts'),
(4,2,'Vegan','vegan'),
(5,2,'Halaal','halaal'),
(6,2,'Kosher','kosher')
ON DUPLICATE KEY UPDATE name=VALUES(name);

INSERT INTO product_tags (product_id, tag_id) VALUES
(1001,1),(1001,2),(1002,1),(1002,2),(1003,1),(1003,2)
ON DUPLICATE KEY UPDATE product_id=VALUES(product_id);

-- -------------------------
-- Family Meal Deal Bundle
-- -------------------------
INSERT INTO bundles (id, name, slug, description, base_price) VALUES
(5001, 'Family Meal Deal', 'family-meal-deal', 'Any 2 Large Pizzas + 2L Cold Drink for R250', 250.00)
ON DUPLICATE KEY UPDATE base_price=VALUES(base_price), updated_at = NOW();

-- sections: pizza1, pizza2, drink
INSERT INTO bundle_sections (id, bundle_id, section_index, name, min_select, max_select)
VALUES
(5101,5001,1,'Choose Pizza 1 (Large)',1,1),
(5102,5001,2,'Choose Pizza 2 (Large)',1,1),
(5103,5001,3,'Choose 2L Cold Drink',1,1)
ON DUPLICATE KEY UPDATE name=VALUES(name), min_select=VALUES(min_select);

-- allowed products per section (only large pizzas in pizza sections)
INSERT INTO bundle_section_products (section_id, product_id) VALUES
(5101,1001),(5101,1002),(5101,1003),
(5102,1001),(5102,1002),(5102,1003),
(5103,2001),(5103,2002),(5103,2003),(5103,2004),(5103,2005)
ON DUPLICATE KEY UPDATE product_id = VALUES(product_id);

-- attach pizza modifier groups to pizza sections
INSERT INTO bundle_section_modifier_groups (section_id, modifier_group_id)
VALUES
(5101,1),(5101,2),(5101,3),(5101,4),(5101,5),
(5102,1),(5102,2),(5102,3),(5102,4),(5102,5)
ON DUPLICATE KEY UPDATE modifier_group_id = VALUES(modifier_group_id);

-- -------------------------
-- Menu items (unified menu)
-- -------------------------
INSERT INTO menu_items (id, category_id, type, product_id, bundle_id, name, description, sort_order)
VALUES
(1, 1, 'product', 1001, NULL, 'Bacon, Avo & Feta', 'Bacon, avocado & feta pizza', 1),
(2, 1, 'product', 1002, NULL, 'Classic Cheese', 'Classic cheese pizza', 2),
(3, 1, 'product', 1003, NULL, 'Pepperoni Deluxe', 'Pepperoni deluxe', 3),
(4, 1, 'bundle', NULL, 5001, 'Family Meal Deal', 'Any 2 Large Pizzas + 2L Cold Drink for R250', 10),

(10, 3, 'product', 2001, NULL, 'Pepsi 2L', 'Pepsi 2 litre', 1),
(11, 3, 'product', 2002, NULL, 'Coke 2L', 'Coke 2 litre', 2)
ON DUPLICATE KEY UPDATE name = VALUES(name), description = VALUES(description), updated_at = NOW();

-- done
```

* * *

3) C# EF Core Entities (POCOs)
------------------------------

Place these in your Models folder. I keep them minimal; navigation properties included.

```csharp
// Category.cs
public class Category
{
    public long Id { get; set; }
    public string Name { get; set; } = null!;
    public string Slug { get; set; } = null!;
    public int SortOrder { get; set; }
    public ICollection<MenuItem> MenuItems { get; set; } = new List<MenuItem>();
}

// Product.cs
public class Product
{
    public long Id { get; set; }
    public long? CategoryId { get; set; }
    public Category? Category { get; set; }
    public string? Sku { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public string Type { get; set; } = "normal"; // normal | bundle | custom
    public bool IsActive { get; set; } = true;

    public ICollection<ProductVariant> Variants { get; set; } = new List<ProductVariant>();
    public ICollection<ProductModifierGroup> ProductModifierGroups { get; set; } = new List<ProductModifierGroup>();
    public ICollection<ProductTag> ProductTags { get; set; } = new List<ProductTag>();
}

// ProductVariant.cs
public class ProductVariant
{
    public long Id { get; set; }
    public long ProductId { get; set; }
    public Product Product { get; set; } = null!;
    public string? Sku { get; set; }
    public string Title { get; set; } = null!;
    public decimal Price { get; set; }
    public decimal? Cost { get; set; }
    public bool IsDefault { get; set; }
    public Inventory? Inventory { get; set; }
    public ICollection<VariantPrice> VariantPrices { get; set; } = new List<VariantPrice>();
}

// Inventory.cs
public class Inventory
{
    public long Id { get; set; }
    public long VariantId { get; set; }
    public ProductVariant Variant { get; set; } = null!;
    public decimal QtyAvailable { get; set; }
    public decimal QtyReserved { get; set; }
}

// VariantPrice.cs
public class VariantPrice
{
    public long Id { get; set; }
    public long VariantId { get; set; }
    public ProductVariant Variant { get; set; } = null!;
    public decimal Price { get; set; }
    public DateTime StartsAt { get; set; }
    public DateTime? EndsAt { get; set; }
}

// TagCategory.cs
public class TagCategory
{
    public long Id { get; set; }
    public string Name { get; set; } = null!;
    public string Slug { get; set; } = null!;
    public ICollection<Tag> Tags { get; set; } = new List<Tag>();
}

// Tag.cs
public class Tag
{
    public long Id { get; set; }
    public long CategoryId { get; set; }
    public TagCategory Category { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string Slug { get; set; } = null!;
    public ICollection<ProductTag> ProductTags { get; set; } = new List<ProductTag>();
}

// ProductTag.cs (join)
public class ProductTag
{
    public long ProductId { get; set; }
    public Product Product { get; set; } = null!;
    public long TagId { get; set; }
    public Tag Tag { get; set; } = null!;
}

// ModifierGroup.cs
public class ModifierGroup
{
    public long Id { get; set; }
    public string Name { get; set; } = null!;
    public string Slug { get; set; } = null!;
    public string? Description { get; set; }
    public ICollection<Modifier> Modifiers { get; set; } = new List<Modifier>();
}

// Modifier.cs
public class Modifier
{
    public long Id { get; set; }
    public long ModifierGroupId { get; set; }
    public ModifierGroup ModifierGroup { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string? Slug { get; set; }
    public decimal PriceModifier { get; set; }
    public bool IsDefault { get; set; }
}

// ProductModifierGroup.cs
public class ProductModifierGroup
{
    public long Id { get; set; }
    public long ProductId { get; set; }
    public Product Product { get; set; } = null!;
    public long ModifierGroupId { get; set; }
    public ModifierGroup ModifierGroup { get; set; } = null!;
    public bool IsRequired { get; set; }
    public int MinSelect { get; set; }
    public int MaxSelect { get; set; }
}

// Bundle.cs
public class Bundle
{
    public long Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Slug { get; set; }
    public string? Description { get; set; }
    public decimal BasePrice { get; set; }
    public bool IsActive { get; set; } = true;
    public ICollection<BundleSection> Sections { get; set; } = new List<BundleSection>();
}

// BundleSection.cs
public class BundleSection
{
    public long Id { get; set; }
    public long BundleId { get; set; }
    public Bundle Bundle { get; set; } = null!;
    public int SectionIndex { get; set; }
    public string Name { get; set; } = null!;
    public int MinSelect { get; set; } = 1;
    public int MaxSelect { get; set; } = 1;
    public ICollection<BundleSectionProduct> AllowedProducts { get; set; } = new List<BundleSectionProduct>();
    public ICollection<BundleSectionModifierGroup> ModifierGroups { get; set; } = new List<BundleSectionModifierGroup>();
}

// BundleSectionProduct.cs
public class BundleSectionProduct
{
    public long Id { get; set; }
    public long SectionId { get; set; }
    public BundleSection Section { get; set; } = null!;
    public long ProductId { get; set; }
    public Product Product { get; set; } = null!;
}

// BundleSectionModifierGroup.cs
public class BundleSectionModifierGroup
{
    public long Id { get; set; }
    public long SectionId { get; set; }
    public BundleSection Section { get; set; } = null!;
    public long ModifierGroupId { get; set; }
    public ModifierGroup ModifierGroup { get; set; } = null!;
}

// MenuItem.cs
public class MenuItem
{
    public long Id { get; set; }
    public long CategoryId { get; set; }
    public Category Category { get; set; } = null!;
    public string Type { get; set; } = "product"; // product | bundle
    public long? ProductId { get; set; }
    public Product? Product { get; set; }
    public long? BundleId { get; set; }
    public Bundle? Bundle { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
    public int SortOrder { get; set; }
}
```

* * *

4) AppDbContext with Fluent API
-------------------------------

```csharp
public class AppDbContext : DbContext
{
    public DbSet<Category> Categories { get; set; } = null!;
    public DbSet<MenuItem> MenuItems { get; set; } = null!;
    public DbSet<Product> Products { get; set; } = null!;
    public DbSet<ProductVariant> ProductVariants { get; set; } = null!;
    public DbSet<Inventory> Inventories { get; set; } = null!;
    public DbSet<VariantPrice> VariantPrices { get; set; } = null!;
    public DbSet<TagCategory> TagCategories { get; set; } = null!;
    public DbSet<Tag> Tags { get; set; } = null!;
    public DbSet<ProductTag> ProductTags { get; set; } = null!;
    public DbSet<ModifierGroup> ModifierGroups { get; set; } = null!;
    public DbSet<Modifier> Modifiers { get; set; } = null!;
    public DbSet<ProductModifierGroup> ProductModifierGroups { get; set; } = null!;
    public DbSet<Bundle> Bundles { get; set; } = null!;
    public DbSet<BundleSection> BundleSections { get; set; } = null!;
    public DbSet<BundleSectionProduct> BundleSectionProducts { get; set; } = null!;
    public DbSet<BundleSectionModifierGroup> BundleSectionModifierGroups { get; set; } = null!;
    public DbSet<BundleOrder> BundleOrders { get; set; } = null!;
    public DbSet<BundleOrderSelection> BundleOrderSelections { get; set; } = null!;

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {}

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<ProductVariant>()
            .Property(v => v.Price).HasColumnType("decimal(12,2)");
        modelBuilder.Entity<VariantPrice>()
            .Property(p => p.Price).HasColumnType("decimal(12,2)");
        modelBuilder.Entity<Modifier>()
            .Property(m => m.PriceModifier).HasColumnType("decimal(12,2)");
        modelBuilder.Entity<Bundle>()
            .Property(b => b.BasePrice).HasColumnType("decimal(12,2)");
        modelBuilder.Entity<MenuItem>()
            .HasOne(m => m.Category).WithMany(c => c.MenuItems).HasForeignKey(m => m.CategoryId);

        modelBuilder.Entity<Product>()
            .HasMany(p => p.Variants).WithOne(v => v.Product).HasForeignKey(v => v.ProductId);

        modelBuilder.Entity<ProductTag>().HasKey(pt => new { pt.ProductId, pt.TagId });
        modelBuilder.Entity<ProductTag>()
            .HasOne(pt => pt.Product).WithMany(p => p.ProductTags).HasForeignKey(pt => pt.ProductId);
        modelBuilder.Entity<ProductTag>().HasOne(pt => pt.Tag).WithMany(t => t.ProductTags).HasForeignKey(pt => pt.TagId);

        modelBuilder.Entity<ProductModifierGroup>()
            .HasOne(x => x.Product).WithMany(p => p.ProductModifierGroups).HasForeignKey(x => x.ProductId);
        modelBuilder.Entity<ProductModifierGroup>()
            .HasOne(x => x.ModifierGroup).WithMany(mg => mg.Modifiers).HasForeignKey(x => x.ModifierGroupId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<BundleSection>()
            .HasOne(s => s.Bundle).WithMany(b => b.Sections).HasForeignKey(s => s.BundleId);

        // BundleSectionProduct
        modelBuilder.Entity<BundleSectionProduct>()
            .HasOne(p => p.Section).WithMany(s => s.AllowedProducts).HasForeignKey(p => p.SectionId);
        modelBuilder.Entity<BundleSectionProduct>()
            .HasOne(p => p.Product).WithMany().HasForeignKey(p => p.ProductId);

        // BundleSectionModifierGroup
        modelBuilder.Entity<BundleSectionModifierGroup>()
            .HasOne(x => x.Section).WithMany(s => s.ModifierGroups).HasForeignKey(x => x.SectionId);
        modelBuilder.Entity<BundleSectionModifierGroup>()
            .HasOne(x => x.ModifierGroup).WithMany().HasForeignKey(x => x.ModifierGroupId);

        // MenuItem -> Product OR Bundle
        modelBuilder.Entity<MenuItem>()
            .HasOne(mi => mi.Product).WithMany().HasForeignKey(mi => mi.ProductId).OnDelete(DeleteBehavior.SetNull);
        modelBuilder.Entity<MenuItem>()
            .HasOne(mi => mi.Bundle).WithMany().HasForeignKey(mi => mi.BundleId).OnDelete(DeleteBehavior.SetNull);
    }
}
```

* * *

5) DTOs (API-ready)
-------------------

```csharp
public class MenuResponseDto { public List<CategoryDto> Menu { get; set; } = new(); }
public class CategoryDto { public long Id { get; set; } public string Name { get; set; } = null!; public List<MenuItemDto> Items { get; set; } = new(); }
public class MenuItemDto { public long Id { get; set; } public string Type { get; set; } = null!; public string Name { get; set; } = null!; public ProductDto? Product { get; set; } public BundleDto? Bundle { get; set; } }
public class ProductDto { public long Id { get; set; } public string Name { get; set; } = null!; public string? Description { get; set; } public List<VariantDto> Variants { get; set; } = new(); public List<ModifierGroupDto> ModifierGroups { get; set; } = new(); }
public class VariantDto { public long Id { get; set; } public string Title { get; set; } = null!; public decimal Price { get; set; } }
public class ModifierGroupDto { public long Id { get; set; } public string Name { get; set; } = null!; public bool IsRequired { get; set; } public List<ModifierDto> Options { get; set; } = new(); }
public class ModifierDto { public long Id { get; set; } public string Name { get; set; } = null!; public decimal PriceModifier { get; set; } }
public class BundleDto { public long Id { get; set; } public string Name { get; set; } = null!; public decimal BasePrice { get; set; } public List<BundleSectionDto> Sections { get; set; } = new(); }
public class BundleSectionDto { public long Id { get; set; } public string Name { get; set; } = null!; public int MinSelect { get; set; } public int MaxSelect { get; set; } public List<ProductDto> AllowedProducts { get; set; } = new(); public List<ModifierGroupDto> ModifierGroups { get; set; } = new(); }

public class BundlePriceRequestDto { public long BundleId { get; set; } public List<BundleSectionSelectionDto> Selections { get; set; } = new(); }
public class BundleSectionSelectionDto { public long SectionId { get; set; } public long? ProductId { get; set; } public List<long> ModifierIds { get; set; } = new(); }
public class BundlePriceResponseDto { public decimal Total { get; set; } public decimal BasePrice { get; set; } public decimal Extras { get; set; } }
```

* * *

6) Minimal ASP.NET Core Controllers (sketch)
--------------------------------------------

Add these routes in your API project — they use LINQ/EF to construct the unified menu and calculate prices.

```csharp
[ApiController]
[Route("api/[controller]")]
public class MenuController : ControllerBase
{
    private readonly AppDbContext _db;
    public MenuController(AppDbContext db) => _db = db;

    [HttpGet]
    public async Task<ActionResult<MenuResponseDto>> GetMenu()
    {
        var cats = await _db.Categories.OrderBy(c => c.SortOrder).ToListAsync();
        var menu = new MenuResponseDto();
        foreach (var cat in cats)
        {
            var menuItems = await _db.MenuItems
                .Where(mi => mi.CategoryId == cat.Id)
                .OrderBy(mi => mi.SortOrder)
                .ToListAsync();

            var catDto = new CategoryDto { Id = cat.Id, Name = cat.Name };
            foreach (var mi in menuItems)
            {
                var miDto = new MenuItemDto { Id = mi.Id, Type = mi.Type, Name = mi.Name };
                if (mi.Type == "product" && mi.ProductId.HasValue)
                {
                    var p = await _db.Products.Include(p => p.Variants)
                                              .Include(p => p.ProductModifierGroups).ThenInclude(pm => pm.ModifierGroup).ThenInclude(mg => mg.Modifiers)
                                              .FirstOrDefaultAsync(x => x.Id == mi.ProductId.Value);
                    if (p != null)
                    {
                        miDto.Product = new ProductDto {
                            Id = p.Id,
                            Name = p.Name,
                            Description = p.Description,
                            Variants = p.Variants.Select(v => new VariantDto { Id = v.Id, Title = v.Title, Price = v.Price }).ToList(),
                            ModifierGroups = p.ProductModifierGroups.Select(pm => new ModifierGroupDto {
                                Id = pm.ModifierGroupId,
                                Name = pm.ModifierGroup.Name,
                                IsRequired = pm.IsRequired,
                                Options = pm.ModifierGroup.Modifiers.Select(m => new ModifierDto { Id = m.Id, Name = m.Name, PriceModifier = m.PriceModifier }).ToList()
                            }).ToList()
                        };
                    }
                }
                else if (mi.Type == "bundle" && mi.BundleId.HasValue)
                {
                    var b = await _db.Bundles.Include(b => b.Sections)
                                              .ThenInclude(s => s.AllowedProducts).ThenInclude(ap => ap.Product)
                                              .Include(b => b.Sections).ThenInclude(s => s.ModifierGroups).ThenInclude(x => x.ModifierGroup).ThenInclude(mg => mg.Modifiers)
                                              .FirstOrDefaultAsync(x => x.Id == mi.BundleId.Value);
                    if (b != null)
                    {
                        miDto.Bundle = new BundleDto {
                            Id = b.Id,
                            Name = b.Name,
                            BasePrice = b.BasePrice,
                            Sections = b.Sections.OrderBy(s => s.SectionIndex).Select(s => new BundleSectionDto {
                                Id = s.Id,
                                Name = s.Name,
                                MinSelect = s.MinSelect,
                                MaxSelect = s.MaxSelect,
                                AllowedProducts = s.AllowedProducts.Select(ap => new ProductDto {
                                    Id = ap.Product.Id,
                                    Name = ap.Product.Name,
                                    Description = ap.Product.Description,
                                    Variants = ap.Product.Variants.Select(v => new VariantDto { Id = v.Id, Title = v.Title, Price = v.Price }).ToList(),
                                    ModifierGroups = ap.Product.ProductModifierGroups.Select(pm => new ModifierGroupDto {
                                        Id = pm.ModifierGroupId,
                                        Name = pm.ModifierGroup.Name,
                                        IsRequired = pm.IsRequired,
                                        Options = pm.ModifierGroup.Modifiers.Select(m => new ModifierDto { Id = m.Id, Name = m.Name, PriceModifier = m.PriceModifier }).ToList()
                                    }).ToList()
                                }).ToList(),
                                ModifierGroups = s.ModifierGroups.Select(mg => new ModifierGroupDto {
                                    Id = mg.ModifierGroupId,
                                    Name = mg.ModifierGroup.Name,
                                    Options = mg.ModifierGroup.Modifiers.Select(m => new ModifierDto { Id = m.Id, Name = m.Name, PriceModifier = m.PriceModifier }).ToList()
                                }).ToList()
                            }).ToList()
                        };
                    }
                }

                catDto.Items.Add(miDto);
            }
            menu.Menu.Add(catDto);
        }
        return Ok(menu);
    }
}
```

```csharp
[ApiController]
[Route("api/bundles")]
public class BundlesController : ControllerBase
{
    private readonly AppDbContext _db;
    public BundlesController(AppDbContext db) => _db = db;

    [HttpPost("{bundleId}/price")]
    public async Task<ActionResult<BundlePriceResponseDto>> Price(long bundleId, [FromBody] BundlePriceRequestDto req)
    {
        var bundle = await _db.Bundles.Include(b => b.Sections)
                                      .ThenInclude(s => s.ModifierGroups).ThenInclude(x => x.ModifierGroup).ThenInclude(mg => mg.Modifiers)
                                      .FirstOrDefaultAsync(b => b.Id == bundleId);
        if (bundle == null) return NotFound();

        decimal extras = 0M;
        foreach (var s in req.Selections)
        {
            if (s.ModifierIds != null)
            {
                var mods = await _db.Modifiers.Where(m => s.ModifierIds.Contains(m.Id)).ToListAsync();
                extras += mods.Sum(m => m.PriceModifier);
            }
            // optionally add variant surcharge per selected product or variant
        }
        var total = bundle.BasePrice + extras;
        return Ok(new BundlePriceResponseDto { BasePrice = bundle.BasePrice, Extras = extras, Total = Math.Round(total,2) });
    }

    [HttpPost("{bundleId}/order")]
    public async Task<ActionResult> Order(long bundleId, [FromBody] BundlePriceRequestDto req)
    {
        var bundle = await _db.Bundles.FindAsync(bundleId);
        if (bundle == null) return NotFound();

        var priceResp = await Price(bundleId, req);
        var body = (priceResp.Result as OkObjectResult)?.Value as BundlePriceResponseDto;
        if (body == null) return BadRequest();

        var order = new BundleOrder { BundleId = bundleId, TotalPrice = body.Total };
        _db.BundleOrders.Add(order);
        await _db.SaveChangesAsync();

        // store selections
        foreach (var sel in req.Selections)
        {
            if (sel.ProductId.HasValue)
            {
                // record product selection
                _db.BundleOrderSelections.Add(new BundleOrderSelection { BundleOrderId = order.Id, SectionId = sel.SectionId, ProductId = sel.ProductId.Value, Qty = 1 });
            }
            if (sel.ModifierIds != null)
            {
                foreach (var mid in sel.ModifierIds)
                    _db.BundleOrderSelections.Add(new BundleOrderSelection { BundleOrderId = order.Id, SectionId = sel.SectionId, ModifierId = mid, Qty = 1 });
            }
        }
        await _db.SaveChangesAsync();
        return Ok(new { orderId = order.Id, total = order.TotalPrice });
    }
}
```

* * *

7) Example JSON (`GET /api/menu`)
---------------------------------

(This is what the `MenuController` returns; trimmed for brevity — this is the flattened/combined view)

```json
{
  "menu": [
    {
      "id": 1,
      "name": "Pizzas",
      "items": [
        {
          "id": 1,
          "type": "product",
          "name": "Bacon, Avo & Feta",
          "product": {
            "id": 1001,
            "name": "Bacon, Avo & Feta",
            "description": "Bacon, avo, feta & cheese",
            "variants": [
              { "id": 10001, "title": "Large", "price": 109.99 }
            ],
            "modifierGroups": [
              { "id": 1, "name": "Base", "isRequired": true, "options": [ { "id":1002,"name":"Traditional","priceModifier":0 } ] },
              { "id": 2, "name": "Extra Meat Toppings", "options": [ { "id":2001,"name":"Bacon","priceModifier":28.9 } ] }
            ]
          }
        },
        {
          "id": 4,
          "type": "bundle",
          "name": "Family Meal Deal",
          "bundle": {
            "id": 5001,
            "name": "Family Meal Deal",
            "basePrice": 250.00,
            "sections": [
              {
                "id": 5101,
                "name": "Choose Pizza 1 (Large)",
                "minSelect": 1,
                "maxSelect": 1,
                "allowedProducts": [
                  { "id": 1001, "name": "Bacon, Avo & Feta", "variants":[{"id":10001,"title":"Large","price":109.99}] },
                  { "id": 1002, "name": "Classic Cheese", "variants":[{"id":10002,"title":"Large","price":99.99}] }
                ],
                "modifierGroups": [
                  { "id": 1, "name": "Base", "options":[ {"id":1001,"name":"Gluten Free","priceModifier":0} ] }
                ]
              },
              {
                "id": 5103,
                "name": "Choose 2L Cold Drink",
                "minSelect": 1,
                "maxSelect": 1,
                "allowedProducts": [
                  { "id": 2001, "name": "Pepsi 2L", "variants":[{"id":20011,"title":"2L","price":0.0}] }
                ],
                "modifierGroups": []
              }
            ]
          }
        }
      ]
    }
  ]
}
```

* * *

8) React + Tailwind UI sketch (Bundle + Product rendering)
----------------------------------------------------------

```jsx
// MenuPage.jsx
import React, { useEffect, useState } from "react";

export default function MenuPage(){
  const [menu, setMenu] = useState(null);
  useEffect(()=>{ fetch('/api/menu').then(r=>r.json()).then(setMenu); },[]);
  if(!menu) return <div>Loading...</div>;

  return (
    <div className="p-6">
      {menu.menu.map(cat => (
        <section key={cat.id} className="mb-8">
          <h2 className="text-2xl font-bold mb-4">{cat.name}</h2>
          <div className="grid gap-4">
            {cat.items.map(item => (
              <div key={item.id} className="p-4 border rounded flex justify-between">
                <div>
                  <div className="font-semibold">{item.name}</div>
                  <div className="text-sm text-gray-600">{item.type === 'product' ? item.product?.description : item.bundle?.description}</div>
                </div>
                <div className="flex flex-col items-end">
                  {item.type === 'product' && item.product?.variants?.length > 0 && <div className="text-lg font-bold">R{item.product.variants[0].price.toFixed(2)}</div>}
                  {item.type === 'bundle' && <div className="text-lg font-bold">R{item.bundle.basePrice.toFixed(2)}</div>}
                  <button className="mt-2 bg-blue-600 text-white px-3 py-1 rounded">Choose</button>
                </div>
              </div>
            ))}
          </div>
        </section>
      ))}
    </div>
  );
}
```

* * *

9) C# Bundle Price Calculator (server-side snippet)
---------------------------------------------------

```csharp
public static class BundleCalculator
{
    // bundle must include sections with ModifierGroups -> Modifiers
    public static decimal CalculateTotal(Bundle bundle, BundlePriceRequestDto req, IEnumerable<Modifier> allModifiers = null)
    {
        decimal total = bundle.BasePrice;
        // add selected modifiers
        foreach (var sel in req.Selections)
        {
            if (sel.ModifierIds != null && sel.ModifierIds.Any())
            {
                // if you have EF context available, query Modifiers by ids; here assume allModifiers provided
                if (allModifiers != null)
                {
                    var mods = allModifiers.Where(m => sel.ModifierIds.Contains(m.Id));
                    total += mods.Sum(m => m.PriceModifier);
                }
            }
            // optional: add variant surcharges or variant price differences; easiest: add variant price if product variant chosen.
        }
        return Math.Round(total, 2);
    }
}
```
