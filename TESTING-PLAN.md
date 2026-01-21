# BizBio Multi-Product Architecture - Testing Plan

**Date**: December 31, 2025
**Version**: 1.0
**Features to Test**: Phases 1, 3, 4, and 5 implementations

## Pre-Testing Setup

### 1. Environment Setup
```bash
# Ensure backend API is running
cd ../BizBio.API
dotnet run

# Start frontend development server
cd src/BizBio.UI
npm run dev
```

### 2. Test User Credentials
- Email: `testuser@test.com`
- Password: (your test password)

### 3. Test Data Requirements
- At least one entity with multiple catalogs
- Some catalog items in at least one catalog
- Active subscription to at least one product

---

## Test Suite 1: Stacked Modal System

### Test 1.1: Basic Modal Stacking
**Steps**:
1. Navigate to menu editor or catalog items page
2. Click "Add New Item" to open ItemFormModal
3. Verify modal opens with z-index 1000
4. Click "+ Create Category" button
5. Verify InlineCategoryCreateModal opens on top
6. Verify ItemFormModal is still visible but dimmed in background

**Expected Results**:
- ✅ InlineCategoryCreateModal has z-index 1001
- ✅ ItemFormModal is visible but dimmed (opacity 0.4)
- ✅ Both modals are centered on screen
- ✅ Clicking outside top modal doesn't close parent modal

### Test 1.2: ESC Key Handling
**Steps**:
1. Open ItemFormModal (z-index 1000)
2. Open InlineCategoryCreateModal (z-index 1001)
3. Press ESC key once
4. Verify only top modal (InlineCategoryCreateModal) closes
5. Press ESC key again
6. Verify ItemFormModal closes

**Expected Results**:
- ✅ ESC closes only the topmost modal
- ✅ Parent modal remains open until ESC pressed again
- ✅ No errors in console

### Test 1.3: Modal Stack with 3 Levels
**Steps**:
1. Open SubscriptionUpgradeModal from dashboard
2. Theoretically open another modal on top (if feature exists)
3. Verify proper stacking with incrementing z-index

**Expected Results**:
- ✅ Each modal has z-index incremented by 1
- ✅ Backdrop opacity increases slightly with each level
- ✅ All parent modals remain visible

---

## Test Suite 2: Inline Category Creation

### Test 2.1: Create Category from Item Form
**Steps**:
1. Navigate to catalog items
2. Click "Add New Item"
3. In the category dropdown, click "+ Create Category" button
4. Verify InlineCategoryCreateModal opens
5. Enter:
   - Name: "Test Category"
   - Description: "Testing inline creation"
   - Icon: "fa-utensils"
6. Click "Create" button
7. Verify modal closes
8. Verify new category appears in dropdown
9. Verify new category is auto-selected

**Expected Results**:
- ✅ Modal opens on top of item form
- ✅ Name field is auto-focused
- ✅ Category is created successfully
- ✅ Success toast notification appears
- ✅ Category appears in dropdown
- ✅ Category is auto-selected in form
- ✅ Item form remains open with all other data intact

### Test 2.2: Category Creation Validation
**Steps**:
1. Open inline category modal
2. Leave name field empty
3. Try to click "Create"
4. Verify button is disabled or validation error shows

**Expected Results**:
- ✅ Create button is disabled when name is empty
- ✅ Name field shows required validation

### Test 2.3: Cancel Category Creation
**Steps**:
1. Open inline category modal
2. Enter partial data
3. Click "Cancel" button
4. Verify modal closes
5. Verify item form is still open
6. Verify category dropdown unchanged

**Expected Results**:
- ✅ Modal closes without creating category
- ✅ No changes to category list
- ✅ Parent form remains intact

---

## Test Suite 3: Subscription Upgrade Modal

### Test 3.1: Open Upgrade Modal
**Steps**:
1. Navigate to `/dashboard/subscription`
2. Locate an active subscription card
3. Click "Upgrade Plan" button
4. Verify SubscriptionUpgradeModal opens

**Expected Results**:
- ✅ Modal displays with gradient header
- ✅ Current plan is highlighted
- ✅ "Current Plan" badge visible on active tier
- ✅ All available tiers displayed in grid (3 columns on desktop)

### Test 3.2: View Tier Information
**Steps**:
1. Open upgrade modal
2. Review each tier card
3. Verify all information is displayed correctly

**Expected Results**:
- ✅ Tier name and description visible
- ✅ Monthly price displayed (e.g., "R299")
- ✅ Annual price with savings % shown if available
- ✅ Feature limits displayed:
  - Max Entities (Businesses)
  - Max Catalogs per entity
  - Max Library Items
  - Max Categories per catalog
- ✅ Trial days shown if applicable
- ✅ Green checkmarks next to each feature

### Test 3.3: Select and Upgrade Tier
**Steps**:
1. Open upgrade modal for Menu product
2. Click on a higher tier card (e.g., Professional)
3. Verify tier is selected (highlighted border, "Selected" text)
4. Verify pro-rata billing notice appears
5. Click "Upgrade Now" button
6. Wait for API call to complete
7. Verify success toast notification
8. Verify modal closes
9. Verify dashboard refreshes with new tier

**Expected Results**:
- ✅ Selected tier has blue border and scale transform
- ✅ Pro-rata billing info box appears (yellow background)
- ✅ Upgrade API call succeeds
- ✅ Success notification: "Subscription upgraded successfully!"
- ✅ Dashboard shows new tier name and limits
- ✅ Usage stats remain accurate

### Test 3.4: Current Tier Selection
**Steps**:
1. Open upgrade modal
2. Try to click on current tier
3. Verify it cannot be selected

**Expected Results**:
- ✅ Current tier card shows "Current Plan" badge
- ✅ Select button shows "Current Plan" (disabled)
- ✅ Card has blue border and background
- ✅ Cannot click to select current tier

### Test 3.5: Loading and Error States
**Steps**:
1. Open upgrade modal
2. If slow network, verify loading state shows
3. Force an error (disconnect network or mock API failure)
4. Verify error state displays with "Try Again" button

**Expected Results**:
- ✅ Loading spinner appears while fetching tiers
- ✅ "Loading available plans..." message shown
- ✅ Error icon and message appear if fetch fails
- ✅ "Try Again" button retries the fetch

---

## Test Suite 4: Add Product Subscription Modal

### Test 4.1: Open Add Product Modal
**Steps**:
1. Navigate to `/dashboard/subscription`
2. If user has subscriptions, verify "Add Product" button in header
3. Click "Add Product" button
4. Verify AddProductSubscriptionModal opens

**Expected Results**:
- ✅ Modal opens with two-step wizard
- ✅ Step 1: Product selection is shown first
- ✅ Available products displayed as cards

### Test 4.2: Product Selection (Step 1)
**Steps**:
1. Open add product modal
2. Review available products
3. Verify each product card shows:
   - Icon
   - Name
   - Description
   - Key features list
4. Click on a product card (e.g., "Retail")
5. Verify card is highlighted
6. Click "Next" or "Continue" button

**Expected Results**:
- ✅ Three products shown: Cards, Menu, Retail
- ✅ Products user already subscribes to are filtered out
- ✅ Product selection highlights the card
- ✅ Can proceed to step 2

### Test 4.3: Plan Selection (Step 2)
**Steps**:
1. Select a product (e.g., Retail)
2. Proceed to step 2
3. Verify tier options are loaded for selected product
4. Review tier cards (similar to upgrade modal)
5. Select a tier
6. Click "Subscribe" button
7. Wait for API call
8. Verify success and modal closes

**Expected Results**:
- ✅ Tiers specific to selected product are shown
- ✅ Free tier and paid tiers displayed
- ✅ Trial period information visible
- ✅ Tier selection works correctly
- ✅ API call to POST `/api/v1/subscriptions` succeeds
- ✅ Success toast: "Successfully subscribed to [Product]!"
- ✅ Dashboard refreshes showing new subscription

### Test 4.4: No Subscriptions State
**Steps**:
1. Delete all subscriptions (if possible in test env)
2. Navigate to subscription dashboard
3. Verify "No Active Subscriptions" message
4. Click "Browse Products" button
5. Verify add product modal opens

**Expected Results**:
- ✅ Empty state shows with icon and message
- ✅ "Browse Products" button opens add product modal
- ✅ User can subscribe to first product

---

## Test Suite 5: Price Override UI

### Test 5.1: View Available Parent Items
**Steps**:
1. Create at least 2 catalogs in same entity:
   - Catalog A: "Main Menu"
   - Catalog B: "Happy Hour Menu"
2. Add an item to Catalog A: "Mojito" - R95
3. Open Catalog B
4. Click "Add New Item"
5. Scroll to "Item Sharing & Price Override" section
6. Click on "Reference Existing Item" dropdown
7. Verify items from Catalog A are shown grouped by catalog name

**Expected Results**:
- ✅ Dropdown shows "Create New Item" as default
- ✅ Optgroup header shows "Main Menu"
- ✅ Items from Main Menu listed: "Mojito - R95"
- ✅ Current catalog (Happy Hour) items are NOT shown
- ✅ Only catalogs with items appear in dropdown

### Test 5.2: Create Shared Item with Price Override
**Steps**:
1. In Catalog B (Happy Hour), create new item
2. Select "Mojito" from Main Menu as parent item
3. Verify "Shared Item Configuration" box appears (blue background)
4. Enter price override: 65
5. Verify text shows "This catalog will show R65"
6. Fill in other required fields (category if needed)
7. Click "Create Item"
8. Verify item is created with override

**Expected Results**:
- ✅ Blue info box appears when parent selected
- ✅ Price override input is visible
- ✅ Placeholder text: "Leave empty to use parent price"
- ✅ Effective price calculation shown below input
- ✅ Item saves with parentCatalogItemId and priceOverride
- ✅ API call includes both fields in payload

### Test 5.3: Use Parent Price (No Override)
**Steps**:
1. Create shared item selecting parent
2. Leave price override input empty
3. Verify text shows "Will use the parent item's price"
4. Save item
5. View item in catalog
6. Verify it shows parent's price

**Expected Results**:
- ✅ Empty override input shows helper text
- ✅ Item saves with parentCatalogItemId
- ✅ priceOverride is null
- ✅ Effective price equals parent price (R95)

### Test 5.4: Reset Price Override
**Steps**:
1. Create shared item with parent
2. Enter price override: 75
3. Verify reset button (undo icon) appears
4. Click reset button
5. Verify price override is cleared
6. Verify text changes to "Will use parent item's price"

**Expected Results**:
- ✅ Reset button appears next to price override input when value entered
- ✅ Clicking reset clears the input
- ✅ Helper text updates accordingly

### Test 5.5: Edit Shared Item
**Steps**:
1. Open existing shared item for editing
2. Verify parent item is shown in dropdown (selected)
3. Verify price override value is loaded
4. Change price override to different value
5. Save changes
6. Verify changes persist

**Expected Results**:
- ✅ Parent item pre-selected in dropdown
- ✅ Price override value pre-filled if set
- ✅ Can modify price override
- ✅ Changes save correctly

---

## Test Suite 6: Copy Item Functionality

### Test 6.1: Open Copy Modal
**Steps**:
1. Navigate to catalog items page
2. Locate an item with image, tags, and variants
3. Click "Copy" or "..." menu → "Copy to Another Catalog"
4. Verify CopyItemModal opens

**Expected Results**:
- ✅ Modal opens with item preview at top
- ✅ Item image thumbnail shown (if image exists)
- ✅ Item name and price displayed
- ✅ Description shown (first line if truncated)

### Test 6.2: Select Target Catalog
**Steps**:
1. Open copy modal for an item
2. Review "Select Target Catalog" dropdown
3. Verify catalogs from same entity are shown
4. Verify current catalog is NOT in the list
5. Select a target catalog

**Expected Results**:
- ✅ Dropdown shows all entity's catalogs except current
- ✅ Only active catalogs shown
- ✅ Catalog names clearly displayed

### Test 6.3: Copy Options
**Steps**:
1. Open copy modal
2. Review copy options checkboxes
3. Verify all three options checked by default:
   - Copy images
   - Copy tags (allergens, dietary info)
   - Copy variants (sizes, options)
4. Uncheck "Copy variants"
5. Select target catalog
6. Click "Copy Item"

**Expected Results**:
- ✅ All checkboxes checked by default
- ✅ Can toggle each checkbox independently
- ✅ Info box explains difference between copy and reference
- ✅ API call includes selected options in payload

### Test 6.4: Execute Copy
**Steps**:
1. Open copy modal for item with full data:
   - Name: "Caesar Salad"
   - Price: R85
   - Images: 2 images
   - Tags: Vegetarian, Gluten-Free
   - Variants: Small (R75), Large (R95)
2. Select target catalog: "Dinner Menu"
3. Keep all copy options checked
4. Click "Copy Item"
5. Wait for API call
6. Verify success notification
7. Navigate to target catalog
8. Verify copied item exists

**Expected Results**:
- ✅ API POST to `/api/v1/catalog-items/{id}/copy-to-catalog`
- ✅ Success toast: "Item copied to 'Dinner Menu' successfully"
- ✅ Modal closes
- ✅ Original item unchanged
- ✅ Copied item in target catalog with:
  - Same name
  - Same price
  - Same images (if copy images checked)
  - Same tags (if copy tags checked)
  - Same variants (if copy variants checked)
  - NO parentCatalogItemId (independent item)

### Test 6.5: Verify Independence
**Steps**:
1. Copy an item from Catalog A to Catalog B
2. Edit original item in Catalog A (change price to R100)
3. View copied item in Catalog B
4. Verify copied item still has original price (not changed)

**Expected Results**:
- ✅ Original and copy are independent
- ✅ Changes to original don't affect copy
- ✅ Changes to copy don't affect original

### Test 6.6: Partial Copy
**Steps**:
1. Open copy modal
2. Uncheck "Copy images" and "Copy variants"
3. Keep "Copy tags" checked
4. Copy item
5. Verify copied item has:
   - Tags: ✅ copied
   - Images: ❌ not copied
   - Variants: ❌ not copied

**Expected Results**:
- ✅ Only selected options are copied
- ✅ Unchecked options result in empty/null values

### Test 6.7: Info Box Explanation
**Steps**:
1. Open copy modal
2. Read blue info box at bottom
3. Verify it explains:
   - Creates new independent item
   - Changes don't sync between original and copy
   - Use "Reference Item" for synchronized sharing

**Expected Results**:
- ✅ Info box clearly explains copy vs. reference
- ✅ Helps user understand the difference

---

## Test Suite 7: Integration Tests

### Test 7.1: Complete Item Creation Flow with Category
**Steps**:
1. Navigate to catalog items
2. Click "Add New Item"
3. Click "+ Create Category" (opens inline modal)
4. Create category: "Desserts"
5. Verify category modal closes and "Desserts" is selected
6. Fill item details:
   - Name: "Chocolate Cake"
   - Description: "Rich chocolate dessert"
   - Price: R45
   - Upload image
   - Add tags: Vegetarian
7. Click "Create Item"
8. Verify item created successfully

**Expected Results**:
- ✅ Inline category creation works seamlessly
- ✅ New category auto-selected
- ✅ Item creates with new category
- ✅ No data loss during modal stacking

### Test 7.2: Share Item Across Catalogs with Override
**Steps**:
1. **Catalog A** - Create master item:
   - Name: "Espresso"
   - Price: R30
   - Category: Beverages
2. **Catalog B** - Create shared reference:
   - Select "Espresso" as parent
   - Price override: R25
3. **Catalog C** - Create shared reference:
   - Select "Espresso" as parent
   - No price override (use parent price)
4. View all three catalogs
5. Verify:
   - Catalog A: Shows R30
   - Catalog B: Shows R25 (override)
   - Catalog C: Shows R30 (parent price)

**Expected Results**:
- ✅ Master item in Catalog A
- ✅ Catalog B shows overridden price
- ✅ Catalog C shows parent price
- ✅ All three items share same master data except price

### Test 7.3: Copy Then Modify
**Steps**:
1. Copy item "Mojito" from Catalog A to Catalog B
2. Modify copied item in Catalog B:
   - Change price to R70
   - Update description
3. View original in Catalog A
4. Verify original unchanged

**Expected Results**:
- ✅ Copy is independent
- ✅ Can modify without affecting original
- ✅ Use case: Similar items with different customization

### Test 7.4: Subscription Upgrade Impact
**Steps**:
1. Note current subscription limits
2. Upgrade to higher tier
3. Verify new limits appear in dashboard
4. Try to create entity/catalog beyond old limit
5. Verify now allowed due to upgrade

**Expected Results**:
- ✅ Limits update immediately after upgrade
- ✅ Can exceed old limits
- ✅ New limits enforced correctly

---

## Test Suite 8: Error Handling & Edge Cases

### Test 8.1: Network Errors
**Steps**:
1. Disconnect network
2. Try to open upgrade modal
3. Verify error state with retry button
4. Reconnect network
5. Click "Try Again"
6. Verify data loads

**Expected Results**:
- ✅ Error state displayed
- ✅ User-friendly error message
- ✅ Retry button works
- ✅ No console errors crash app

### Test 8.2: Invalid Price Override
**Steps**:
1. Create shared item
2. Enter negative price override: -10
3. Try to save

**Expected Results**:
- ✅ Validation prevents negative prices
- ✅ Error message shown
- ✅ Item doesn't save with invalid data

### Test 8.3: Copy to Same Catalog (Should Not Appear)
**Steps**:
1. Open copy modal for item in "Main Menu"
2. Check target catalog dropdown
3. Verify "Main Menu" is NOT in the list

**Expected Results**:
- ✅ Current catalog excluded from options
- ✅ Cannot accidentally copy item to same catalog

### Test 8.4: Empty Catalog Copy
**Steps**:
1. Create new empty catalog
2. Try to copy item
3. Verify new empty catalog appears in target list

**Expected Results**:
- ✅ Empty catalogs are valid copy targets
- ✅ Can copy to populate new catalog

### Test 8.5: Modal Backdrop Clicks
**Steps**:
1. Open ItemFormModal
2. Click on dimmed backdrop (outside modal)
3. Verify modal closes
4. Open ItemFormModal again
5. Open InlineCategoryCreateModal
6. Click on backdrop
7. Verify only top modal closes (not parent)

**Expected Results**:
- ✅ Single modal closes on backdrop click
- ✅ Stacked modals: only top closes
- ✅ Parent remains open

---

## Test Suite 9: Accessibility & UX

### Test 9.1: Keyboard Navigation
**Steps**:
1. Open any modal
2. Use Tab key to navigate through form fields
3. Use Shift+Tab to navigate backward
4. Press ESC to close modal

**Expected Results**:
- ✅ Tab order is logical
- ✅ All interactive elements focusable
- ✅ ESC closes modal
- ✅ Focus returns to trigger element

### Test 9.2: Form Auto-Focus
**Steps**:
1. Open inline category modal
2. Verify cursor is in Name field

**Expected Results**:
- ✅ Name field auto-focused
- ✅ User can immediately start typing

### Test 9.3: Loading States
**Steps**:
1. Open upgrade modal (slow network simulation)
2. Verify loading spinner appears
3. Verify loading message displayed

**Expected Results**:
- ✅ Visual feedback during loading
- ✅ User knows system is working

### Test 9.4: Success Feedback
**Steps**:
1. Complete any successful action:
   - Create category
   - Upgrade subscription
   - Copy item
2. Verify toast notification appears
3. Verify notification is readable and informative

**Expected Results**:
- ✅ Toast appears in consistent location
- ✅ Message is clear and specific
- ✅ Auto-dismisses after ~3 seconds

---

## Test Suite 10: Cross-Browser Testing

### Test 10.1: Browser Compatibility
**Browsers to Test**:
- ✅ Chrome (latest)
- ✅ Firefox (latest)
- ✅ Safari (latest)
- ✅ Edge (latest)

**Tests**:
1. Modal rendering and stacking
2. Form submissions
3. API calls
4. Animations and transitions

### Test 10.2: Responsive Design
**Viewports**:
- Mobile: 375px width
- Tablet: 768px width
- Desktop: 1920px width

**Tests**:
1. Modal fit on screen
2. Grid layouts (tier cards)
3. Button sizes and touch targets
4. Form field responsiveness

---

## Regression Testing

### Areas to Check
1. ✅ Existing item creation (without new features)
2. ✅ Category management (original CategoryModal)
3. ✅ Dashboard display
4. ✅ Navigation between pages
5. ✅ User authentication
6. ✅ Catalog switching

**Verify**:
- No breaking changes to existing functionality
- All old features still work
- No console errors
- No visual regressions

---

## Performance Testing

### Test P1: Modal Open Speed
**Steps**:
1. Click to open modal
2. Measure time to fully render

**Expected**: < 300ms

### Test P2: API Response Times
**Steps**:
1. Open upgrade modal (fetches tiers)
2. Check network tab for response time

**Expected**: < 500ms

### Test P3: Large Lists
**Steps**:
1. Entity with 10+ catalogs
2. Each catalog with 50+ items
3. Open parent item dropdown
4. Verify no lag or freezing

**Expected**: Smooth performance, < 1s load

---

## Bug Tracking Template

```markdown
### Bug #[Number]
**Title**: [Brief description]
**Severity**: Critical / High / Medium / Low
**Component**: [Modal Stack / Price Override / etc.]

**Steps to Reproduce**:
1.
2.
3.

**Expected Result**:


**Actual Result**:


**Screenshots**:
[Attach if applicable]

**Browser/OS**:


**Console Errors**:
```
[Error logs]
```

**Status**: Open / In Progress / Fixed / Won't Fix
```

---

## Test Completion Checklist

### Pre-Deployment
- [ ] All test suites executed
- [ ] No critical bugs remaining
- [ ] All medium/low bugs documented
- [ ] Performance acceptable
- [ ] Cross-browser testing complete
- [ ] Accessibility review done
- [ ] Code review completed
- [ ] Documentation updated

### Post-Deployment
- [ ] Smoke test in production
- [ ] Monitor error logs
- [ ] User feedback collected
- [ ] Analytics tracking verified

---

## Notes for Testers

1. **Test Data**: Use separate test database or clearly marked test data
2. **API Errors**: Check browser console and Network tab for details
3. **Screenshots**: Capture for any bugs found
4. **Edge Cases**: Try unexpected inputs and workflows
5. **Real World**: Test with realistic data volumes
6. **Mobile**: Don't forget mobile testing
7. **Permissions**: Test with different user roles if applicable

---

## Success Criteria

**Required for Release**:
- ✅ All Test Suite 1-7 tests passing
- ✅ No critical or high severity bugs
- ✅ Cross-browser compatibility verified
- ✅ Responsive design working on all viewports
- ✅ No console errors in production build
- ✅ All new features integrated smoothly with existing features

**Nice to Have**:
- Performance optimizations implemented
- User feedback incorporated
- Additional edge cases tested

---

**Testing Started**: [Date]
**Testing Completed**: [Date]
**Tested By**: [Name]
**Sign-Off**: [Name] - [Date]
