# Integration Testing Plan
## Multi-Product Architecture - Phase 3

**Date**: 2025-12-31
**Status**: 🔄 In Progress

---

## Test Environment

### Backend
- **API URL**: https://localhost:5001
- **Database**: Development database with migration applied
- **Environment**: Development
- **Auth**: JWT tokens

### Frontend
- **URL**: http://localhost:3000
- **Framework**: Nuxt 3
- **Build**: Development mode

---

## Test Scenarios

### 1. User Authentication Flow
**Priority**: Critical
**Prerequisites**: None

**Test Steps**:
1. ✅ Register new user
2. ✅ Verify email (dev bypass)
3. ✅ Login and receive JWT token
4. ⏳ Navigate to dashboard
5. ⏳ Verify user session persists

**Expected Results**:
- User successfully registers
- Email verification bypasses in dev mode
- JWT token received and stored
- Dashboard loads correctly
- User remains authenticated on page refresh

**Status**: ⏳ Pending

---

### 2. Entity Creation Flow
**Priority**: Critical
**Prerequisites**: Authenticated user

**Test Steps**:
1. ⏳ Navigate to menu creation page
2. ⏳ View "Select Your Business" step
3. ⏳ Click "Create New Business"
4. ⏳ Fill in entity form:
   - Type: Restaurant
   - Name: "Test Cafe"
   - Description: "A test cafe"
   - City: "Cape Town"
   - Phone: "+27 21 123 4567"
   - Address: "123 Main St"
5. ⏳ Submit form
6. ⏳ Verify entity created
7. ⏳ Verify entity appears in list
8. ⏳ Verify entity is auto-selected

**Expected Results**:
- Form validates correctly
- Entity created successfully via API
- Success toast notification shown
- Entity appears in entity list
- Entity is automatically selected
- Can proceed to next step

**Status**: ⏳ Pending

---

### 3. Entity Selection Flow
**Priority**: Critical
**Prerequisites**: At least one entity exists

**Test Steps**:
1. ⏳ Navigate to menu creation page
2. ⏳ View existing entities
3. ⏳ Verify entity cards display:
   - Entity name
   - Entity type icon
   - Description
   - City
   - Catalog count
4. ⏳ Select an entity
5. ⏳ Verify selection state
6. ⏳ Proceed to next step
7. ⏳ Verify entity persists in wizard state

**Expected Results**:
- All entities load from API
- Entity cards render correctly
- Selection state highlights chosen entity
- Can proceed to plan selection
- Selected entity persists through wizard

**Status**: ⏳ Pending

---

### 4. Complete Menu Creation Flow
**Priority**: Critical
**Prerequisites**: Authenticated user, entity selected

**Test Steps**:

**Step 1: Entity Selection**
1. ⏳ Load existing entities
2. ⏳ Select or create entity
3. ⏳ Click "Continue to Plan Selection"

**Step 2: Plan Selection**
1. ⏳ View available plans (Starter, Professional, Premium)
2. ⏳ Select "Professional" plan
3. ⏳ Verify trial information displays
4. ⏳ Click "Continue to Menu Setup"

**Step 3: Menu Profile**
1. ⏳ Fill in menu information:
   - Name: "Lunch Menu"
   - Business Name: "Test Cafe"
   - Cuisine: "Cafe"
   - Phone/Email/Address
2. ⏳ Verify form validation
3. ⏳ Click "Continue to Categories"

**Step 4: Categories**
1. ⏳ Click "Add Category"
2. ⏳ Create categories:
   - "Starters"
   - "Main Courses"
   - "Desserts"
3. ⏳ Verify category list
4. ⏳ Click "Continue to Menu Items"

**Step 5: Menu Items**
1. ⏳ Add items to each category
2. ⏳ Upload item images
3. ⏳ Set prices and descriptions
4. ⏳ Click "Complete"
5. ⏳ Verify menu creation API call
6. ⏳ Verify redirect to dashboard

**Expected Results**:
- All steps navigate correctly
- Form validation works at each step
- Data persists between steps
- API calls succeed
- Menu created successfully
- Redirect to dashboard with success message

**Status**: ⏳ Pending

---

### 5. Subscription Dashboard
**Priority**: High
**Prerequisites**: Authenticated user with subscription

**Test Steps**:
1. ⏳ Navigate to /dashboard/subscription
2. ⏳ Verify subscriptions load
3. ⏳ Verify subscription card displays:
   - Product type and icon
   - Tier name
   - Status badge
   - Trial information (if active)
   - Usage statistics
   - Plan limits
   - Billing information
4. ⏳ Verify usage progress bars:
   - Businesses count
   - Menus count
   - Library items count
5. ⏳ Verify colors:
   - Green: < 75%
   - Yellow: 75-90%
   - Red: > 90%
6. ⏳ Test "Upgrade Plan" button
7. ⏳ Test "Cancel" subscription
8. ⏳ Verify invoice preview loads
9. ⏳ Verify VAT calculation

**Expected Results**:
- Subscriptions load from API
- All data displays correctly
- Progress bars calculate correctly
- Colors match usage percentage
- Buttons trigger appropriate actions
- Invoice preview shows correct totals

**Status**: ⏳ Pending

---

### 6. Error Handling
**Priority**: High
**Prerequisites**: None

**Test Cases**:

**6.1 Network Errors**
1. ⏳ Disconnect network
2. ⏳ Try to load entities
3. ⏳ Verify error message displays
4. ⏳ Verify "Try Again" button works
5. ⏳ Reconnect and retry

**6.2 API Errors**
1. ⏳ Try to create entity with invalid data
2. ⏳ Verify validation errors display
3. ⏳ Verify error toast notifications

**6.3 Empty States**
1. ⏳ Load subscription page with no subscriptions
2. ⏳ Verify empty state message
3. ⏳ Verify "Browse Products" button

**6.4 Loading States**
1. ⏳ Verify spinners show during API calls
2. ⏳ Verify loading text displays
3. ⏳ Verify smooth transitions

**Expected Results**:
- All errors handled gracefully
- User-friendly error messages
- Retry mechanisms work
- Empty states guide user
- Loading states prevent user confusion

**Status**: ⏳ Pending

---

### 7. Mobile Responsiveness
**Priority**: Medium
**Prerequisites**: None

**Test Steps**:
1. ⏳ Resize browser to mobile width (375px)
2. ⏳ Test entity selection on mobile
3. ⏳ Test menu creation wizard on mobile
4. ⏳ Test subscription dashboard on mobile
5. ⏳ Verify all buttons are touch-friendly
6. ⏳ Verify text is readable
7. ⏳ Verify forms are usable

**Expected Results**:
- All pages responsive
- Touch targets are adequate (44px minimum)
- Text remains readable
- Forms stack properly
- Navigation works on mobile

**Status**: ⏳ Pending

---

### 8. Edge Cases
**Priority**: Medium
**Prerequisites**: Various

**Test Cases**:

**8.1 Maximum Limits**
1. ⏳ Try to create entities at subscription limit
2. ⏳ Verify limit enforcement
3. ⏳ Verify upgrade prompt

**8.2 Duplicate Names**
1. ⏳ Try to create entity with existing name
2. ⏳ Verify slug handling

**8.3 Long Text**
1. ⏳ Enter very long entity names
2. ⏳ Verify truncation/wrapping

**8.4 Special Characters**
1. ⏳ Test entity names with special chars
2. ⏳ Verify slug generation

**Expected Results**:
- All limits enforced correctly
- Duplicate handling works
- Long text handled gracefully
- Special characters sanitized

**Status**: ⏳ Pending

---

## Test Data

### Test User
- Email: testuser@test.com
- Password: Test1234
- Name: Test User
- User ID: 1

### Test Entity
- Type: Restaurant
- Name: Test Cafe
- City: Cape Town
- ID: 1

### Test Subscription
- Product: Menu (type 1)
- Tier: Restaurant (tier 5)
- Status: Trial
- Trial Days: 6 remaining

---

## Issues Found

### Critical Issues
_(None yet)_

### High Priority Issues
_(None yet)_

### Medium Priority Issues
_(None yet)_

### Low Priority Issues
_(None yet)_

---

## Test Results Summary

| Category | Total | Passed | Failed | Pending |
|----------|-------|--------|--------|---------|
| Authentication | 5 | 0 | 0 | 5 |
| Entity Creation | 8 | 0 | 0 | 8 |
| Entity Selection | 7 | 0 | 0 | 7 |
| Menu Creation | 15 | 0 | 0 | 15 |
| Subscription Dashboard | 9 | 0 | 0 | 9 |
| Error Handling | 12 | 0 | 0 | 12 |
| Mobile | 7 | 0 | 0 | 7 |
| Edge Cases | 8 | 0 | 0 | 8 |
| **TOTAL** | **71** | **0** | **0** | **71** |

**Overall Progress**: 0% (0/71 tests completed)

---

## Next Steps

1. ⏳ Start frontend development server
2. ⏳ Ensure backend API is running
3. ⏳ Begin systematic testing
4. ⏳ Document issues as they arise
5. ⏳ Fix critical issues immediately
6. ⏳ Create issue tracking document
7. ⏳ Retest after fixes
8. ⏳ Update test results

---

## Notes

- API is already running on https://localhost:5001
- Test user already created: testuser@test.com
- Test entity already created: Test Cafe (ID: 1)
- Test subscription exists: Menu product, Restaurant tier

---

## Success Criteria

**Phase 3 Complete When**:
- ✅ All critical tests pass
- ✅ All high priority tests pass
- ✅ Medium priority issues documented
- ✅ Low priority issues documented
- ✅ No blocking bugs remain
- ✅ User flow is smooth and intuitive
- ✅ All integrations work correctly
- ✅ Mobile experience is acceptable

**Ready for Production When**:
- All above criteria met
- Performance is acceptable
- Security review passed
- Documentation complete
- Deployment plan ready
