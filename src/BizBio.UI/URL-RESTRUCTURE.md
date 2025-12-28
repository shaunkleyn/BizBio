✅ Page Restructuring Complete

  New Menu Product Structure:

  pages/
  ├── dashboard/
  │   └── index.vue (Unified dashboard - unchanged)
  │
  └── menu/
      ├── index.vue (Menu list - was /menu/menus/index.vue)
      ├── create.vue (Create menu - was /dashboard/menu/create.vue)
      ├── [slug].vue (Public menu view - unchanged)
      │
      ├── dashboard/
      │   └── index.vue (Menu product dashboard - was /menu/index.vue)
      │
      ├── [id]/
      │   ├── content.vue (was /dashboard/menu/[id]/content.vue)
      │   └── edit.vue (was /dashboard/menu/[id]/edit.vue)
      │
      ├── analytics/
      │   └── [id].vue (was /dashboard/menu/analytics/[id].vue)
      │
      ├── bundles/
      │   ├── index.vue (was /dashboard/bundles/index.vue)
      │   ├── create.vue (was /dashboard/bundles/create.vue)
      │   └── [id]/
      │       └── edit.vue (was /dashboard/bundles/[id]/edit.vue)
      │
      ├── categories/
      │   └── index.vue (unchanged)
      │
      ├── items/
      │   └── index.vue (was /menu/library/items.vue)
      │
      ├── extras/
      │   ├── index.vue (was /menu/library/extras.vue)
      │   └── groups/
      │       └── index.vue (was /menu/library/extra-groups.vue)
      │
      ├── options/
      │   ├── index.vue (was /menu/library/options.vue)
      │   └── groups/
      │       └── index.vue (was /menu/library/option-groups.vue)
      │
      └── restaurants/
          └── index.vue (was /dashboard/restaurants/index.vue)

  Updated Route References:

  All route references have been updated throughout the application:

  Navigation (MenuSidebar.vue):
  - ✅ Overview: /menu/dashboard
  - ✅ Menus: /menu
  - ✅ Bundles: /menu/bundles
  - ✅ Restaurants: /menu/restaurants
  - ✅ Items: /menu/items
  - ✅ Extras: /menu/extras
  - ✅ Extra Groups: /menu/extras/groups
  - ✅ Options: /menu/options
  - ✅ Option Groups: /menu/options/groups

  Components Updated:
  - ✅ MenuSidebar.vue - All navigation links updated
  - ✅ ItemFormModal.vue - Library references updated
  - ✅ RestaurantSelector.vue - Route updated
  - ✅ MenuProfileSetup.vue - Already correct

  Pages Updated:
  - ✅ /dashboard/index.vue - Restaurant links updated
  - ✅ /menu/index.vue - Create and edit links updated
  - ✅ /menu/dashboard/index.vue - All quick action links updated
  - ✅ /menu/[id]/content.vue - Settings link updated
  - ✅ /menu/bundles/*.vue - All bundle route references updated

  New URL Structure:

  | Old URL                     | New URL              |
  |-----------------------------|----------------------|
  | /menu/menus                 | /menu                |
  | /menu (dashboard)           | /menu/dashboard      |
  | /dashboard/menu/create      | /menu/create         |
  | /dashboard/menu/[id]/edit   | /menu/[id]/edit      |
  | /dashboard/bundles          | /menu/bundles        |
  | /dashboard/restaurants      | /menu/restaurants    |
  | /menu/library/items         | /menu/items          |
  | /menu/library/extras        | /menu/extras         |
  | /menu/library/extra-groups  | /menu/extras/groups  |
  | /menu/library/options       | /menu/options        |
  | /menu/library/option-groups | /menu/options/groups |

  The restructuring is complete and all routes have been updated! The menu product now has a clean, organized structure that separates it from the unified dashboard and follows the pattern you specified.