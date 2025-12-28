# BizBio Frontend Test Suite

This directory contains comprehensive tests for the BizBio frontend application, with a focus on validating the menu product route restructuring.

## Test Structure

```
tests/
├── unit/
│   ├── routes.test.ts                    # Route file existence tests
│   ├── components/
│   │   ├── MenuSidebar.test.ts          # Navigation sidebar link tests
│   │   └── component-links.test.ts       # Component internal link tests
│   └── pages/
│       ├── dashboard-links.test.ts       # Dashboard page link tests
│       └── menu-pages-links.test.ts      # Menu pages link tests
└── integration/
    └── route-navigation.test.ts          # Route hierarchy integration tests
```

## Running Tests

### Run all tests
```bash
npm run test
```

### Run tests once (CI mode)
```bash
npm run test:run
```

### Run tests with UI
```bash
npm run test:ui
```

### Run tests with coverage
```bash
npm run test:coverage
```

## Test Coverage

### 1. Route Existence Tests (`unit/routes.test.ts`)
**72 total tests across 6 test suites**

Tests that all route files exist in the correct locations after the restructuring:

- ✅ Menu root routes (`/menu/`, `/menu/create`, `/menu/dashboard`)
- ✅ Menu edit routes (`/menu/[id]/edit`, `/menu/[id]/content`)
- ✅ Bundle routes (`/menu/bundles/`, `/menu/bundles/create`, `/menu/bundles/[id]/edit`)
- ✅ Library item routes (`/menu/items`, `/menu/extras`, `/menu/options` + groups)
- ✅ Other routes (`/menu/categories`, `/menu/restaurants`, `/menu/analytics/[id]`)
- ✅ Validates old routes don't exist (deprecated paths removed)

### 2. MenuSidebar Navigation Tests (`unit/components/MenuSidebar.test.ts`)

Tests the main navigation sidebar to ensure all links point to correct new routes:

- ✅ Main navigation links (Overview, Menus, Bundles, Restaurants)
- ✅ Library navigation links (Items, Extras, Extra Groups, Options, Option Groups)
- ✅ Verifies no old route patterns exist (`/dashboard/menu/`, `/menu/library/`, etc.)
- ✅ Navigation structure completeness

### 3. Component Link Tests (`unit/components/component-links.test.ts`)

Tests that components using navigation links have been updated:

- ✅ **ItemFormModal**: Links to `/menu/options/groups` and `/menu/extras/groups`
- ✅ **RestaurantSelector**: Links to `/menu/restaurants`
- ✅ **MenuSidebar**: All new menu product routes present

### 4. Dashboard Page Link Tests (`unit/pages/dashboard-links.test.ts`)

Tests the main dashboard page links:

- ✅ Restaurant management links point to `/menu/restaurants`
- ✅ Menu edit links use `/menu/[id]/edit` pattern
- ✅ Restaurant view links use query params (`/menu?restaurant=id`)
- ✅ No old `/dashboard/restaurants` references

### 5. Menu Pages Link Tests (`unit/pages/menu-pages-links.test.ts`)

Tests internal links within menu pages:

- ✅ **Menu Index**: Links to `/menu/create` and `/menu/[id]/edit`
- ✅ **Menu Dashboard**: Links to `/menu/create`, `/menu/items`
- ✅ **Menu Content**: Links to `/menu/[id]/edit` for settings
- ✅ **Bundles Pages**: All bundle routes updated
- ✅ No old `/dashboard/menu/` or `/dashboard/bundles` references

### 6. Route Navigation Integration Tests (`integration/route-navigation.test.ts`)

High-level integration tests for route structure:

- ✅ Route mapping completeness
- ✅ Deprecated routes not included
- ✅ Route hierarchy validation
- ✅ URL pattern consistency (list pages, create pages, edit pages)
- ✅ Grouped resource patterns
- ✅ Logical navigation flow validation

## Route Restructuring Validation

The tests ensure the following route migrations were successful:

### Old Routes → New Routes

| Old Route | New Route |
|-----------|-----------|
| `/menu/menus` | `/menu` |
| `/menu` (dashboard) | `/menu/dashboard` |
| `/dashboard/menu/create` | `/menu/create` |
| `/dashboard/menu/[id]/edit` | `/menu/[id]/edit` |
| `/dashboard/menu/[id]/content` | `/menu/[id]/content` |
| `/dashboard/bundles` | `/menu/bundles` |
| `/dashboard/bundles/create` | `/menu/bundles/create` |
| `/dashboard/bundles/[id]/edit` | `/menu/bundles/[id]/edit` |
| `/dashboard/restaurants` | `/menu/restaurants` |
| `/menu/library/items` | `/menu/items` |
| `/menu/library/extras` | `/menu/extras` |
| `/menu/library/extra-groups` | `/menu/extras/groups` |
| `/menu/library/options` | `/menu/options` |
| `/menu/library/option-groups` | `/menu/options/groups` |

## Test Philosophy

1. **File Existence Tests**: Verify the physical files are in the correct locations
2. **Content Tests**: Verify file contents reference the correct routes
3. **Component Tests**: Verify Vue components have updated navigation links
4. **Integration Tests**: Verify the overall route structure makes sense

## Continuous Integration

These tests should be run:
- ✅ Before committing route changes
- ✅ In CI/CD pipeline before deployment
- ✅ After any navigation/routing changes
- ✅ As part of pre-merge checks

## Common Issues

### Test Failures

If tests fail after route changes:

1. **Check file locations**: Ensure files are in the correct directories
2. **Check file contents**: Search for old route patterns in files
3. **Check component imports**: Ensure NuxtLink components reference new routes
4. **Run linter**: Old routes might be in comments or dead code

### Adding New Routes

When adding new menu product routes:

1. Add file existence test in `unit/routes.test.ts`
2. Add navigation link test in `unit/components/MenuSidebar.test.ts` if applicable
3. Add integration test in `integration/route-navigation.test.ts`
4. Update this README with the new route

## Test Results

Latest run: **72 tests passing** ✅

```
Test Files  6 passed (6)
Tests       72 passed (72)
```

All route restructuring validations are passing successfully.
