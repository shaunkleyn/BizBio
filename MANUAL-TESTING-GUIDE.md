# Manual Testing Guide
## Multi-Product Architecture Integration Testing

**Date**: 2025-12-31
**Version**: 1.0
**Status**: Ready for Testing

---

## Prerequisites

### Environment Setup
- ✅ **Backend API**: Running on https://localhost:5001
- ✅ **Frontend**: Running on http://localhost:3000
- ✅ **Database**: Migration applied successfully
- ✅ **Test Data**: Test user and entity created

### Test Credentials
```
Email: testuser@test.com
Password: Test1234
```

### Existing Test Data
- **User ID**: 1
- **Entity**: Test Cafe (ID: 1, Type: Restaurant)
- **Subscription**: Menu product, Restaurant tier (Trial with 6 days remaining)

---

## Testing Workflow

### 🎯 Critical Path Testing (Must Complete)

#### Test 1: User Authentication
**Priority**: Critical
**Time**: 5 minutes

**Steps**:
1. Open browser to http://localhost:3000
2. Click "Login" or navigate to /login
3. Enter credentials:
   - Email: testuser@test.com
   - Password: Test1234
4. Click "Login"

**✅ Success Criteria**:
- Login successful
- Redirected to dashboard
- User name displayed in header
- No console errors

**📸 Screenshot**: Login page and dashboard

---

#### Test 2: View Subscription Dashboard
**Priority**: Critical
**Time**: 5 minutes

**Steps**:
1. Navigate to /dashboard/subscription
2. Observe subscription card

**✅ Success Criteria**:
- Subscription loads without errors
- Displays "Menu" product with restaurant icon
- Shows "Restaurant Plan"
- Trial badge visible with "6 days remaining"
- Usage statistics display:
  - Businesses: 1 / 1
  - Menus: 3 / 3
  - Library Items: 0 / 50
- Progress bars render correctly
- Green color for low usage
- Plan limits section shows:
  - 1 Business
  - 3 Menus per business
  - 50 Library items
  - 10 Categories per menu
- Invoice preview section displays (if active subscriptions)

**📸 Screenshots**:
- Full subscription card
- Usage statistics
- Plan limits
- Invoice preview

---

#### Test 3: Entity Selection in Menu Creation
**Priority**: Critical
**Time**: 10 minutes

**Steps**:
1. Navigate to /menu/create
2. Observe Step 1: "Select Your Business"
3. Check existing entities list

**✅ Success Criteria**:
- Step indicator shows "1 of 5"
- Step label: "Select Business"
- Entity card displays for "Test Cafe":
  - Restaurant icon (utensils)
  - Name: "Test Cafe"
  - Type: "Restaurant"
  - City displayed
  - Catalog count: 3 Menus
- Card is clickable
- Selection state highlights card
- "Create New Business" section visible
- "Continue to Plan Selection" button enabled when entity selected

**📸 Screenshots**:
- Entity selection page
- Entity card
- Selected state

---

#### Test 4: Create New Entity
**Priority**: Critical
**Time**: 10 minutes

**Steps**:
1. On /menu/create Step 1
2. Click "Create New Business" card
3. Form expands
4. Fill in form:
   ```
   Business Type: Store
   Business Name: My Test Store
   Description: A test retail store
   City: Johannesburg
   Phone Number: +27 11 123 4567
   Address: 456 Market Street
   ```
5. Click "Create Business"

**✅ Success Criteria**:
- Form expands smoothly
- All fields render correctly
- Business type dropdown has all options:
  - Restaurant
  - Store
  - Venue
  - Organization
- Form validation works (try submitting empty)
- Success toast: "Business created successfully!"
- New entity appears in entities list immediately
- New entity is auto-selected
- Form collapses
- Can proceed to next step

**📸 Screenshots**:
- Create form expanded
- Filled form
- Success toast
- New entity in list

---

#### Test 5: Complete Menu Creation Wizard
**Priority**: Critical
**Time**: 20 minutes

**Step 1: Entity Selection**
1. Select an entity (use existing or newly created)
2. Click "Continue to Plan Selection"

**Step 2: Plan Selection**
1. View 3 plans: Starter, Professional, Premium
2. Observe "Most Popular" badge on Professional
3. Check trial badges (14-Day Free Trial)
4. Observe pricing (R99, R199, R399)
5. Review features lists
6. Select "Professional" plan
7. Click "Continue to Menu Setup"

**Step 3: Menu Info**
1. Fill in menu profile:
   ```
   Menu Name: Summer Menu 2025
   Business Name: (auto-filled from entity)
   Description: Fresh seasonal dishes
   Cuisine: Mediterranean
   Phone Number: (from entity)
   Email: menu@testcafe.com
   Address: (from entity)
   City: (from entity)
   ```
2. Optionally enable SEO
3. Click "Continue to Categories"

**Step 4: Categories**
1. Click "Add Category"
2. Create categories:
   ```
   Category 1:
   - Name: Appetizers
   - Description: Starters and small plates
   - Icon: 🍽️

   Category 2:
   - Name: Main Courses
   - Description: Hearty entrees
   - Icon: 🍖

   Category 3:
   - Name: Desserts
   - Description: Sweet treats
   - Icon: 🍰
   ```
3. Click "Continue to Menu Items"

**Step 5: Menu Items**
1. Add items to "Appetizers":
   ```
   Item 1:
   - Name: Bruschetta
   - Description: Grilled bread with tomatoes
   - Price: 45.00
   - Available: Yes
   ```
2. Add items to other categories
3. Click "Complete"
4. Observe loading state
5. Wait for completion

**✅ Success Criteria**:
- All 5 steps navigate smoothly
- Step indicator updates (1/5, 2/5, etc.)
- Back button works on each step
- Form validation prevents invalid submissions
- Data persists when going back
- Selected entity persists through wizard
- Selected plan persists
- Final submission triggers API call
- Success message appears
- Redirects to dashboard
- New menu appears in dashboard

**📸 Screenshots**:
- Each step of the wizard
- Completed menu in dashboard

---

### 🎯 Secondary Path Testing (Should Complete)

#### Test 6: Subscription Cancellation
**Priority**: High
**Time**: 5 minutes

**Steps**:
1. Navigate to /dashboard/subscription
2. Find "Cancel" button on subscription card
3. Click "Cancel"
4. Observe confirmation dialog
5. Click "Yes, Cancel" (or equivalent)

**✅ Success Criteria**:
- Confirmation dialog appears
- Confirms product name in message
- Cancellation succeeds
- Success toast: "Subscription cancelled successfully"
- Page reloads
- Subscription status updates to "Cancelled"
- Cancel button disappears or disables

**📸 Screenshots**:
- Confirmation dialog
- Cancelled subscription card

---

#### Test 7: Error Handling - Network Error
**Priority**: High
**Time**: 5 minutes

**Steps**:
1. Stop the backend API (Ctrl+C in API terminal)
2. Refresh /dashboard/subscription
3. Observe error state
4. Restart backend API
5. Click "Try Again" button

**✅ Success Criteria**:
- Error message displays clearly
- Error icon shown
- "Try Again" button visible
- After restart, retry succeeds
- Data loads correctly
- No lingering errors

**📸 Screenshots**:
- Error state
- After successful retry

---

#### Test 8: Empty State - No Subscriptions
**Priority**: Medium
**Time**: 3 minutes

**Steps**:
1. Cancel all subscriptions (if any)
2. Navigate to /dashboard/subscription
3. Observe empty state

**✅ Success Criteria**:
- Empty state displays
- Shopping cart icon shown
- Message: "No Active Subscriptions"
- Helpful description text
- "Browse Products" button visible
- Button links to /products

**📸 Screenshot**:
- Empty state

---

#### Test 9: Entity Card Display
**Priority**: Medium
**Time**: 5 minutes

**Steps**:
1. Navigate to /menu/create
2. If you have multiple entities, observe all cards
3. Check different entity types (Restaurant, Store, etc.)

**✅ Success Criteria**:
- All entities display
- Each shows correct icon for type:
  - Restaurant: utensils icon
  - Store: store icon
  - Venue: building icon
  - Organization: briefcase icon
- Catalog counts accurate
- City displays if present
- Hover effects work
- Selection highlights correctly

**📸 Screenshots**:
- Multiple entity cards
- Different entity types

---

### 🎯 Edge Case Testing (Optional but Recommended)

#### Test 10: Long Text Handling
**Priority**: Low
**Time**: 5 minutes

**Steps**:
1. Create entity with very long name:
   ```
   Name: "This is an extremely long business name that should be truncated or wrapped properly in the UI to prevent breaking the layout"
   ```
2. Observe display in entity card

**✅ Success Criteria**:
- Text truncates with ellipsis, or
- Text wraps to multiple lines gracefully
- Card layout doesn't break
- Full text visible on hover (title attribute)

---

#### Test 11: Special Characters
**Priority**: Low
**Time**: 5 minutes

**Steps**:
1. Create entity with special characters:
   ```
   Name: "José's Café & Restaurant"
   ```
2. Observe entity creation
3. Check slug generation

**✅ Success Criteria**:
- Entity creates successfully
- Special characters preserved in name
- Slug generated correctly (joses-cafe-restaurant)
- No encoding issues

---

#### Test 12: Maximum Limits
**Priority**: Medium
**Time**: 5 minutes

**Steps**:
1. With "Restaurant" plan (max 1 entity)
2. Try to create a second entity
3. Observe behavior

**✅ Success Criteria**:
- Error message about limit reached, or
- Upgrade prompt appears
- Cannot create beyond limit
- Clear messaging to user

---

### 🎯 Mobile Responsiveness Testing

#### Test 13: Mobile Entity Selection
**Priority**: Medium
**Time**: 10 minutes

**Steps**:
1. Open DevTools (F12)
2. Toggle device toolbar (Ctrl+Shift+M)
3. Set to "iPhone 12 Pro" or similar (390x844)
4. Navigate to /menu/create
5. Interact with entity cards
6. Try creating new entity

**✅ Success Criteria**:
- Entity cards stack vertically
- Cards are touch-friendly (44px+ tap targets)
- Text remains readable
- Form fields are usable
- Buttons are accessible
- No horizontal scrolling
- Images/icons scale appropriately

**📸 Screenshots**:
- Mobile entity selection
- Mobile create form

---

#### Test 14: Mobile Subscription Dashboard
**Priority**: Medium
**Time**: 10 minutes

**Steps**:
1. In mobile view (390x844)
2. Navigate to /dashboard/subscription
3. Scroll through subscription card
4. Interact with buttons

**✅ Success Criteria**:
- Subscription card stacks vertically
- Usage stats stack to 1-2 columns
- Progress bars visible and functional
- All text readable
- Buttons reachable and tappable
- No content overflow

**📸 Screenshots**:
- Mobile subscription dashboard
- Mobile usage stats

---

## Console Error Checks

### During All Testing
Monitor browser console (F12 → Console) for:

**❌ Should NOT appear**:
- Red errors
- Failed API calls (except when testing error handling)
- Vue warnings
- Unhandled promise rejections
- CORS errors
- 404 errors

**✅ Can appear**:
- Info messages
- Nuxt HMR (Hot Module Replacement) messages
- Development mode warnings

---

## Network Tab Checks

### During All Testing
Monitor Network tab (F12 → Network) for:

**✅ Should see**:
- Successful API calls (200, 201 status codes)
- Proper request/response payloads
- Authorization headers on protected routes
- Fast response times (< 1s typically)

**❌ Should NOT see**:
- 500 Internal Server Errors
- 401 Unauthorized (except on logout)
- 404 Not Found
- Extremely slow requests (> 5s)

---

## Performance Checks

### Page Load Times
**Acceptable**:
- Initial page load: < 3 seconds
- Route navigation: < 500ms
- API calls: < 1 second
- Form submissions: < 2 seconds

**Monitor**:
- Lighthouse score (F12 → Lighthouse)
- First Contentful Paint
- Time to Interactive

---

## Accessibility Checks

### Basic Checks
1. **Keyboard Navigation**:
   - Tab through forms
   - Can reach all interactive elements
   - Focus indicators visible

2. **Screen Reader**:
   - Alt text on images
   - ARIA labels where needed
   - Semantic HTML

3. **Color Contrast**:
   - Text readable on backgrounds
   - Links distinguishable
   - Buttons clearly visible

---

## Bug Reporting Template

### If You Find Issues

**Title**: [Component] Brief description

**Severity**:
- 🔴 Critical (Blocks functionality)
- 🟡 High (Major issue, workaround exists)
- 🟢 Medium (Minor issue)
- ⚪ Low (Cosmetic)

**Steps to Reproduce**:
1. Step one
2. Step two
3. Step three

**Expected Behavior**:
What should happen

**Actual Behavior**:
What actually happens

**Screenshots**:
Attach relevant screenshots

**Console Errors**:
Copy any errors from console

**Environment**:
- Browser: Chrome 120
- OS: Windows 11
- Screen size: 1920x1080

---

## Test Completion Checklist

### Critical Tests (Must Pass)
- [ ] Test 1: User Authentication
- [ ] Test 2: View Subscription Dashboard
- [ ] Test 3: Entity Selection
- [ ] Test 4: Create New Entity
- [ ] Test 5: Complete Menu Creation

### High Priority Tests
- [ ] Test 6: Subscription Cancellation
- [ ] Test 7: Network Error Handling
- [ ] Test 8: Empty State

### Medium Priority Tests
- [ ] Test 9: Entity Card Display
- [ ] Test 12: Maximum Limits
- [ ] Test 13: Mobile Entity Selection
- [ ] Test 14: Mobile Subscription Dashboard

### Low Priority Tests
- [ ] Test 10: Long Text
- [ ] Test 11: Special Characters

### Quality Checks
- [ ] No console errors during normal flow
- [ ] All API calls succeed
- [ ] Page load times acceptable
- [ ] Mobile experience acceptable
- [ ] Keyboard navigation works

---

## Sign-off

**Tester Name**: ___________________________
**Date**: ___________________________
**Overall Status**: ___________________________
**Ready for Production**: ☐ Yes  ☐ No  ☐ With Issues

**Comments**:
_______________________________________________________________
_______________________________________________________________
_______________________________________________________________

---

## Quick Reference

### URLs
- Frontend: http://localhost:3000
- Backend API: https://localhost:5001
- Swagger: https://localhost:5001/swagger

### Test Credentials
- Email: testuser@test.com
- Password: Test1234

### Key Pages to Test
- `/login` - Login page
- `/dashboard` - Main dashboard
- `/dashboard/subscription` - Subscription management
- `/menu/create` - Menu creation wizard

### Expected Results at a Glance
- ✅ All pages load without errors
- ✅ Entity selection works smoothly
- ✅ Entity creation succeeds
- ✅ Menu creation completes successfully
- ✅ Subscription dashboard displays correctly
- ✅ Usage tracking shows accurate data
- ✅ Mobile experience is functional
- ✅ Error states handle gracefully

---

**Happy Testing! 🧪**
