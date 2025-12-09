# BizBio Digital Profile Platform
## Complete Functional Requirements Document (FRD)

**Document Version:** 2.0  
**Date:** November 2024  
**Project Name:** BizBio  
**Project Type:** SaaS Platform  
**Status:** Approved for Development

---

## Document Control

| Version | Date | Author | Changes |
|---------|------|--------|---------|
| 1.0 | Oct 2024 | Development Team | Initial draft |
| 2.0 | Nov 2024 | Development Team | Added Hierarchy & Catalog modules |

---

## Table of Contents

1. [Executive Summary](#1-executive-summary)
2. [System Overview](#2-system-overview)
3. [User Roles & Permissions](#3-user-roles--permissions)
4. [Module 1: User Management](#4-module-1-user-management)
5. [Module 2: Profile System](#5-module-2-profile-system)
6. [Module 3: Profile Customization](#6-module-3-profile-customization)
7. [Module 4: Organizational Hierarchy](#7-module-4-organizational-hierarchy)
8. [Module 5: Catalog System](#8-module-5-catalog-system)
9. [Module 6: Document Management](#9-module-6-document-management)
10. [Module 7: NFC Integration](#10-module-7-nfc-integration)
11. [Module 8: Analytics & Reporting](#11-module-8-analytics--reporting)
12. [Module 9: Subscription Management](#12-module-9-subscription-management)
13. [Module 10: Payment Processing](#13-module-10-payment-processing)
14. [Integration Requirements](#14-integration-requirements)
15. [Non-Functional Requirements](#15-non-functional-requirements)
16. [Data Requirements](#16-data-requirements)
17. [Security Requirements](#17-security-requirements)
18. [User Stories & Use Cases](#18-user-stories--use-cases)
19. [Success Criteria](#19-success-criteria)
20. [Assumptions & Constraints](#20-assumptions--constraints)
21. [Appendices](#21-appendices)

---

## 1. Executive Summary

### 1.1 Purpose
This document defines the complete functional requirements for the BizBio Digital Profile Platform, a comprehensive SaaS solution for creating, managing, and sharing digital business cards, organizational profiles, and product catalogs.

### 1.2 Scope
BizBio enables:
- **Individuals** to create professional digital business cards
- **Businesses** to build company profiles with team directories
- **Organizations** to manage hierarchical team structures
- **Restaurants** to publish digital menus accessible via QR codes
- **Retailers** to showcase product catalogs
- **Service providers** to display service offerings

### 1.3 Target Audience
- Primary: Small to medium businesses (1-50 employees)
- Secondary: Large enterprises, franchises, independent professionals
- Geographic: Initial launch in South Africa, expandable globally

### 1.4 Business Objectives
1. Provide modern alternative to physical business cards
2. Enable contactless information sharing via NFC and QR codes
3. Offer restaurant menu digitization (post-COVID solution)
4. Create recurring revenue through subscription model
5. Build scalable platform for organizational management

---

## 2. System Overview

### 2.1 Platform Architecture

```
BizBio Platform
│
├── Frontend (Web Application)
│   ├── Public Profile Views
│   ├── Catalog/Menu Views
│   ├── User Dashboard
│   └── Admin Portal
│
├── Backend (ASP.NET Core 6 MVC)
│   ├── Authentication & Authorization
│   ├── Profile Management
│   ├── Catalog Management
│   ├── File Storage
│   ├── Analytics Engine
│   └── Payment Processing
│
├── Database (MySQL)
│   ├── 29 Tables
│   └── Relational Schema
│
└── Third-Party Services
    ├── Payment Gateway (PayFast)
    ├── File Storage (Cloud)
    ├── Email Service
    └── CDN
```

### 2.2 Core Modules

1. **User Management** - Registration, authentication, account management
2. **Profile System** - Create and manage profiles (personal, business, organization)
3. **Profile Customization** - Branding, colors, fonts, layouts
4. **Organizational Hierarchy** - Team structures, reporting relationships
5. **Catalog System** - Digital menus, product catalogs
6. **Document Management** - Upload brochures, flyers, PDFs
7. **NFC Integration** - Physical NFC products, tag management
8. **Analytics & Reporting** - Track views, engagement, performance
9. **Subscription Management** - Tiered plans, upgrades, billing
10. **Payment Processing** - Secure payment handling

### 2.3 Technology Stack

**Backend:**
- Framework: ASP.NET Core 6.0 MVC
- Language: C# 10
- ORM: Entity Framework Core 6
- Database: MySQL 8.0+

**Frontend:**
- HTML5, CSS3, JavaScript ES6
- Bootstrap 5.3
- Razor Views
- AJAX for dynamic content

**Infrastructure:**
- Hosting: cPanel with .NET support
- File Storage: AWS S3 / Azure Blob Storage
- CDN: CloudFlare
- SSL: Let's Encrypt (HTTPS required)

---

## 3. User Roles & Permissions

### 3.1 User Types

| Role | Description | Key Permissions |
|------|-------------|----------------|
| **Guest** | Unauthenticated visitor | View public profiles, browse catalogs |
| **Free User** | Basic registered user | 1 profile, limited features |
| **Basic Subscriber** | Paid basic tier | 3 profiles, custom branding, 100 catalog items |
| **Professional Subscriber** | Mid-tier subscriber | 10 profiles, advanced features, 500 catalog items |
| **Enterprise Subscriber** | Top-tier subscriber | Unlimited profiles and items, all features |
| **Profile Owner** | Owner of specific profile | Full control over owned profile |
| **Organization Admin** | Owner of organization profile | Manage team members, view all analytics |
| **Team Member** | Linked to organization | Manage own profile, limited org visibility |
| **System Admin** | Platform administrator | Full system access, user management |

### 3.2 Permission Matrix

| Feature | Guest | Free | Basic | Professional | Enterprise | Admin |
|---------|-------|------|-------|--------------|------------|-------|
| View profiles | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ |
| Create profile | ❌ | 1 | 3 | 10 | ∞ | ∞ |
| Custom branding | ❌ | ❌ | ✅ | ✅ | ✅ | ✅ |
| Catalog items | ❌ | ❌ | 100 | 500 | ∞ | ∞ |
| Team members | ❌ | ❌ | ✅ | ✅ | ✅ | ✅ |
| Analytics | ❌ | ❌ | ✅ | ✅ | ✅ | ✅ |
| NFC integration | ❌ | ❌ | ✅ | ✅ | ✅ | ✅ |
| Document uploads | ❌ | 3 | 10 | 50 | ∞ | ∞ |
| Custom domain | ❌ | ❌ | ❌ | ✅ | ✅ | ✅ |
| Remove branding | ❌ | ❌ | ❌ | ✅ | ✅ | ✅ |
| Priority support | ❌ | ❌ | ❌ | ✅ | ✅ | ✅ |
| API access | ❌ | ❌ | ❌ | ❌ | ✅ | ✅ |

---

## 4. Module 1: User Management

### 4.1 User Registration

**FR-UM-001: User Registration**
- **Priority:** Critical
- **Description:** Users can create accounts with email and password

**Acceptance Criteria:**
- Email validation required
- Password minimum 8 characters, must include uppercase, lowercase, number
- Email verification link sent
- Account created in inactive state until verified
- Duplicate email detection
- Terms of service acceptance required
- POPIA/GDPR consent checkboxes

**Fields:**
- Email (required, unique)
- Password (required, hashed)
- First Name (required)
- Last Name (required)
- Phone Number (optional)

**Validation:**
- Email format validation
- Password strength validation
- Phone number format validation (South African format)

---

### 4.2 Authentication

**FR-UM-002: User Login**
- **Priority:** Critical
- **Description:** Registered users can log in with email and password

**Acceptance Criteria:**
- Email/password authentication
- Remember me functionality
- Session management (12 hours without remember me, 30 days with)
- Failed login attempt tracking (max 5 attempts, 15-minute lockout)
- Last login timestamp recorded

**FR-UM-003: Email Verification**
- **Priority:** Critical
- **Description:** Users must verify email before full access

**Acceptance Criteria:**
- Verification email sent on registration
- Token valid for 24 hours
- Resend verification option
- Account activated on verification

**FR-UM-004: Password Reset**
- **Priority:** High
- **Description:** Users can reset forgotten passwords

**Acceptance Criteria:**
- Reset link sent to email
- Token valid for 24 hours
- New password requirements same as registration
- Old password invalidated on reset

---

### 4.3 Account Management

**FR-UM-005: Profile Settings**
- **Priority:** High
- **Description:** Users can update account information

**Editable Fields:**
- First Name, Last Name
- Phone Number
- Password (with current password confirmation)
- Email (requires re-verification)

**FR-UM-006: Account Deletion**
- **Priority:** Medium
- **Description:** Users can delete their accounts

**Acceptance Criteria:**
- Confirmation required
- Grace period: 30 days before permanent deletion
- All profiles marked for deletion
- Subscriptions cancelled
- Data export offered
- Comply with POPIA right to deletion

---

## 5. Module 2: Profile System

### 5.1 Profile Types

**FR-PS-001: Profile Type Selection**
- **Priority:** Critical
- **Description:** Users can create three types of profiles

**Profile Types:**
1. **Personal** - Individual professional profile/digital business card
2. **Business** - Company or business profile
3. **Organization** - Large organization with team structure

**Type Characteristics:**
- Personal: Single person, resume/CV focus
- Business: Company info, services, team (small)
- Organization: Hierarchical structure, departments, many team members

---

### 5.2 Profile Creation

**FR-PS-002: Create Profile**
- **Priority:** Critical
- **Description:** Users can create new profiles up to subscription limit

**Required Fields:**
- Profile Type (personal/business/organization)
- Display Name
- Profile Slug (unique URL identifier)

**Optional Fields:**
- Tagline
- Description
- Logo/Avatar
- Cover Image
- Contact Information (email, phone, WhatsApp, website)
- Location (address, city, province, postal code, country)
- GPS Coordinates (latitude, longitude)
- Company Registration Number (business/organization)
- VAT Number (business/organization)
- Founder/Owner Name

**Validation:**
- Profile slug unique across platform
- Profile slug alphanumeric, lowercase, hyphens only
- Respect subscription tier limits on profile count

**FR-PS-003: Profile URL Structure**
- **Priority:** High
- **Description:** Each profile accessible via clean URL

**URL Format:**
- Profile: `https://bizbio.co.za/{profile-slug}`
- Example: `https://bizbio.co.za/siyanda-engineering`

---

### 5.3 Profile Information Management

**FR-PS-004: Contact Information**
- **Priority:** High
- **Description:** Multiple contact methods supported

**Contact Fields:**
- Email (with mailto: link)
- Phone Number (with tel: link)
- WhatsApp Number (with WhatsApp API link)
- Website URL
- Physical Address
- Operating Hours (JSON format)

**FR-PS-005: Social Media Links**
- **Priority:** High
- **Description:** Unlimited social media links

**Supported Platforms:**
- Facebook, Instagram, Twitter/X, LinkedIn, TikTok
- YouTube, Pinterest, Snapchat
- WhatsApp Business
- Custom links

**Attributes:**
- Platform name
- URL
- Display order
- Visibility toggle

**FR-PS-006: Location & Maps**
- **Priority:** High
- **Description:** Physical location with map integration

**Features:**
- Address entry (manual or Google Maps autocomplete)
- GPS coordinates (auto-populated or manual)
- Google Maps embed
- "Get Directions" link
- Show/hide map toggle

---

### 5.4 Profile Media

**FR-PS-007: Logo Upload**
- **Priority:** High
- **Description:** Profile logo/avatar image

**Requirements:**
- Formats: JPG, PNG, WebP
- Max size: 5MB
- Recommended: Square, 500x500px minimum
- Auto-cropping option

**FR-PS-008: Cover Image**
- **Priority:** Medium
- **Description:** Header/banner image

**Requirements:**
- Formats: JPG, PNG, WebP
- Max size: 5MB
- Recommended: 1920x400px
- Aspect ratio maintained

**FR-PS-009: Photo Gallery**
- **Priority:** Medium
- **Description:** Multiple profile photos

**Features:**
- Up to tier limit (Free: 3, Basic: 10, Professional: 50, Enterprise: unlimited)
- Caption per photo
- Display order
- Visibility toggle
- Image optimization (auto-compress)

**FR-PS-010: Video Support**
- **Priority:** Low
- **Description:** Background or showcase videos

**Types:**
- Background video (like Six Door example)
- Intro video
- Gallery video

**Requirements:**
- Formats: MP4, WebM
- Max size: 50MB
- Host on cloud storage, embed URL

---

### 5.5 Profile Visibility

**FR-PS-011: Publish/Unpublish**
- **Priority:** High
- **Description:** Control profile visibility

**States:**
- Draft: Only owner can view
- Published: Publicly accessible
- Archived: Hidden but preserved

**FR-PS-012: Featured Profiles**
- **Priority:** Medium
- **Description:** Highlight profiles in directory

**Requirements:**
- Available to Professional+ tiers
- Featured badge on profile
- Priority in search results
- Homepage feature (rotated)

---

## 6. Module 3: Profile Customization

### 6.1 Branding

**FR-PC-001: Color Customization**
- **Priority:** High
- **Description:** Full color scheme customization

**Customizable Colors:**
- Primary Color
- Secondary Color
- Tertiary Color
- Background Color
- Text Color
- Icon Color

**Requirements:**
- Color picker UI
- Hex code input
- Preview before save
- Default palette provided
- Available to Basic+ tiers

**FR-PC-002: Typography**
- **Priority:** High
- **Description:** Custom font selection

**Font Options:**
- Google Fonts library (50+ fonts)
- Separate heading and body font
- Font size controls
- Available to Basic+ tiers

**Fonts Included:**
- Arial, Helvetica, Times New Roman, Georgia
- Roboto, Open Sans, Lato, Montserrat
- Playfair Display, Merriweather
- Custom font upload (Enterprise only)

**FR-PC-003: Icon Customization**
- **Priority:** Medium
- **Description:** Choose icons for profile sections

**Customizable Icons:**
- Address icon
- Phone icon
- Email icon
- Social media icons

**Icon Libraries:**
- Font Awesome (free icons)
- Lucide Icons
- Material Icons

**Attributes:**
- Icon selection
- Icon color
- Icon style (solid, outline, rounded)

---

### 6.2 Layout & Templates

**FR-PC-004: Template Selection**
- **Priority:** High
- **Description:** Pre-designed profile templates

**Template Types:**
- Modern (clean, minimal)
- Classic (traditional, formal)
- Creative (bold, artistic)
- Corporate (professional, structured)

**Template Features:**
- Thumbnail preview
- Live preview before applying
- Responsive design (mobile/tablet/desktop)
- Tier restrictions (some templates Premium+)

**FR-PC-005: Layout Customization**
- **Priority:** Medium
- **Description:** Adjust layout elements

**Customizable Elements:**
- Button style (rounded, square, pill)
- Card style (shadow, border, flat)
- Section spacing
- Component visibility toggles

**FR-PC-006: Custom CSS**
- **Priority:** Low
- **Description:** Advanced CSS customization

**Requirements:**
- Available to Professional+ tiers
- CSS code editor with syntax highlighting
- Sandbox preview
- Validation to prevent breaking
- Save and revert options

---

## 7. Module 4: Organizational Hierarchy

### 7.1 Hierarchical Structure

**FR-OH-001: Parent Profile Relationship**
- **Priority:** High
- **Description:** Profiles can have parent profiles for reporting structure

**Use Case:** 
- Employee reports to Manager
- Manager reports to CEO
- CEO has no parent

**Attributes:**
- `ParentProfileId` (nullable foreign key)
- Auto-generate org chart visualization
- Breadcrumb navigation (Person → Manager → CEO)

**FR-OH-002: Organization Membership**
- **Priority:** High
- **Description:** Profiles can belong to organizations

**Use Case:**
- All employees belong to "Siyanda Engineering"
- Contractors belong to multiple organizations

**Attributes:**
- `OrganizationProfileId` (nullable foreign key)
- Display "Part of [Organization]" badge
- Show organization logo on member profile

---

### 7.2 Profile Relationships

**FR-OH-003: Flexible Relationships**
- **Priority:** High
- **Description:** Create explicit relationships between profiles

**Relationship Types:**
- Employee
- Manager
- Contractor
- Consultant
- Partner
- Advisor
- Founder

**Relationship Attributes:**
- Primary Profile (organization)
- Related Profile (team member)
- Relationship Type
- Job Title
- Department
- Start Date
- End Date (for historical relationships)
- Is Current (boolean)
- Is Visible on Profile (boolean)
- Is Featured (show prominently)
- Display Order

**FR-OH-004: Team Member Display**
- **Priority:** High
- **Description:** Show team members on organization profile

**Display Options:**
- Featured members (3-10 cards)
- All members (grid or list)
- Filterable by department
- Searchable by name

**Card Content:**
- Member photo
- Name
- Job title
- Department
- Contact info preview
- Link to full profile

**Example (Siyanda Engineering):**
```
[Photo]  David Slabbert
         CEO
         [Call] [Email] [View Profile]

[Photo]  KG Mogoale
         Sales Director
         [Call] [Email] [View Profile]
```

---

### 7.3 Team Management

**FR-OH-005: Add Team Member**
- **Priority:** High
- **Description:** Organization admins can add team members

**Methods:**
1. **Link Existing Profile**
   - Search for profile by email/name
   - Send invitation
   - Member accepts → linked

2. **Create New Profile**
   - Create profile on behalf of team member
   - Set as member of organization
   - Member can claim and customize later

3. **Invite via Email**
   - Send invitation to email
   - Recipient creates account and profile
   - Auto-link to organization

**FR-OH-006: Manage Team Member**
- **Priority:** High
- **Description:** Update team member relationships

**Editable Fields:**
- Job title
- Department
- Relationship type
- Visibility on profile
- Featured status
- Display order

**FR-OH-007: Remove Team Member**
- **Priority:** High
- **Description:** Remove member from organization

**Options:**
- Soft delete: Mark as "Former Employee", hide from active list
- Hard delete: Remove relationship completely
- Archive: Keep relationship but set end date

**Team Member Permissions:**
- Member can edit their own profile
- Member cannot remove self from organization
- Organization admin can add/remove members

---

### 7.4 Organizational Features

**FR-OH-008: Department Management**
- **Priority:** Medium
- **Description:** Organize team by departments

**Features:**
- Create departments (Sales, Operations, IT, etc.)
- Assign members to departments
- Filter team view by department
- Department-specific analytics

**FR-OH-009: Org Chart Visualization**
- **Priority:** Low
- **Description:** Visual organizational chart

**Features:**
- Tree view of hierarchy
- Clickable nodes (link to profiles)
- Expand/collapse branches
- Export as image/PDF
- Available to Professional+ tiers

---

## 8. Module 5: Catalog System

### 8.1 Catalog Configuration

**FR-CAT-001: Enable Catalog**
- **Priority:** High
- **Description:** Profile owners can enable catalog functionality

**Settings:**
- Catalog Name (e.g., "Our Menu", "Product Catalog")
- Catalog Description
- Catalog Type (Menu, Products, Services, Other)
- Show Prices (boolean)
- Show Images (boolean)
- Allow Search (boolean)
- Header Image

**Tier Restrictions:**
- Free: No catalog
- Basic: 100 items
- Professional: 500 items
- Enterprise: Unlimited items

**FR-CAT-002: Catalog Access**
- **Priority:** High
- **Description:** Public access to published catalogs

**URL Format:**
- Catalog: `https://bizbio.co.za/c/{profile-slug}`
- Category: `https://bizbio.co.za/c/{profile-slug}/category/{category-id}`
- Example: `https://bizbio.co.za/c/burger-king-stonehill`

**Access Methods:**
- Direct URL
- QR Code
- Link from profile
- NFC tag

---

### 8.2 Category Management

**FR-CAT-003: Create Categories**
- **Priority:** High
- **Description:** Organize items into categories

**Required Fields:**
- Category Name
- Profile ID

**Optional Fields:**
- Description
- Image
- Icon
- Parent Category (for nested categories)
- Display Order

**Examples:**
- Restaurant: Breakfast, Lunch, Dinner, Beverages
- Retail: Clothing, Electronics, Home & Garden
- Services: Consulting, Training, Support

**FR-CAT-004: Nested Categories**
- **Priority:** Medium
- **Description:** Support hierarchical categories

**Structure:**
- Parent → Child (max 3 levels)
- Example: Beverages → Hot Drinks → Coffee

**Display:**
- Breadcrumb navigation
- Expand/collapse subcategories
- Filter by parent category

**FR-CAT-005: Category Visibility**
- **Priority:** High
- **Description:** Control category display

**Options:**
- Visible/Hidden toggle
- Featured/Highlighted categories
- Display order (drag-and-drop reordering)

---

### 8.3 Item Management

**FR-CAT-006: Create Items**
- **Priority:** Critical
- **Description:** Add products/menu items to catalog

**Required Fields:**
- Item Name
- Category
- Profile ID

**Optional Fields:**
- Short Description (max 500 chars)
- Full Description
- Base Price
- Currency (default: ZAR)
- Primary Image
- Preparation Time (for food)
- Calories (for food)
- Allergens
- Tags (comma-separated, for search)
- Display Order

**FR-CAT-007: Item Attributes**
- **Priority:** High
- **Description:** Mark items with special attributes

**Boolean Flags:**
- Available/Unavailable
- Highlighted (show in highlights section)
- Featured (show in featured section)
- Bestseller
- New
- Vegetarian
- Vegan
- Gluten-Free
- Halal
- Kosher

**Display:**
- Icon badges on item cards
- Filter by attributes

---

### 8.4 Item Variants

**FR-CAT-008: Product Variants**
- **Priority:** High
- **Description:** Different sizes/options for items

**Use Cases:**
- Pizza: Small (R80), Medium (R120), Large (R150)
- Coffee: Regular (R25), Large (R35)
- T-Shirt: S, M, L, XL, XXL

**Variant Attributes:**
- Variant Name (required)
- Variant Type (size, flavor, style, option)
- Price (required)
- Compare At Price (for showing discounts)
- SKU
- Available/Unavailable
- Is Default Selection
- Display Order

**Display (Mr D Food style):**
```
Choose type:
○ Whopper Cheese Only          R86.90
○ Whopper Cheese Medium Meal   R114.90  [Selected]
○ Whopper Cheese Large Meal    R134.90
```

**FR-CAT-009: Add-ons/Extras**
- **Priority:** Medium
- **Description:** Optional additions to items

**Use Cases:**
- Extra Cheese (+R15)
- Bacon (+R20)
- Gluten-free base (+R25)
- Gift wrapping (+R10)

**Add-on Attributes:**
- Add-on Name (required)
- Description
- Price (required)
- Max Quantity (default: 1)
- Available/Unavailable
- Display Order

---

### 8.5 Item Media

**FR-CAT-010: Multiple Images per Item**
- **Priority:** High
- **Description:** Image gallery for each item

**Features:**
- Primary image (required for highlights)
- Up to 5 additional images
- Alt text for accessibility
- Display order
- Image gallery viewer (swipe/click through)

**Requirements:**
- Formats: JPG, PNG, WebP
- Max size: 5MB per image
- Auto-optimization (compress to < 500KB)
- Lazy loading

---

### 8.6 Catalog Display

**FR-CAT-011: Catalog Home Page**
- **Priority:** Critical
- **Description:** Main catalog landing page

**Layout (Mr D Food inspired):**
1. **Header Section**
   - Hero image/video background
   - Business logo (overlay)
   - Business name & tagline
   - Quick actions: Call, Email, WhatsApp buttons
   - Distance, rating, delivery time (optional)

2. **Search Bar**
   - Prominent search input
   - Real-time results
   - Search by item name, description, tags

3. **Highlights Section** (Horizontal Scroll)
   - 3-10 featured items
   - Large images (square cards)
   - Item name
   - Price
   - "From R..." if variants

4. **Featured/Most Ordered** (Horizontal Scroll)
   - Secondary highlights
   - Popular items
   - Bestsellers

5. **Category List** (Vertical)
   - All categories
   - Category icon/image
   - Item count
   - Chevron/arrow →
   - Tap to view category

**Mobile Optimized:**
- Touch-friendly (min 44x44px tap targets)
- Swipeable horizontal sections
- Fast loading
- Minimal data usage

**FR-CAT-012: Category View**
- **Priority:** High
- **Description:** Display all items in a category

**Layout:**
1. **Header**
   - Back button
   - Category name
   - Search icon (optional)

2. **Item Cards** (Vertical List)
   - Item image (left side or top)
   - Item name (bold)
   - Short description
   - Price or "From R80"
   - Badges (New, Bestseller, dietary icons)
   - Tap to view details

**Example (Breakfast category):**
```
┌─────────────────────────────────────┐
│ [IMG] Egg Boerie Bacon Muffin      │
│       Go bigger with a flame-       │
│       grilled boerie patty...       │
│       From R59.90          [New]    │
└─────────────────────────────────────┘
```

**FR-CAT-013: Item Detail View**
- **Priority:** Critical
- **Description:** Full-screen modal with complete item information

**Layout (WHOPPER example):**
1. **Header**
   - Close button (X) in top corner

2. **Image Section**
   - Large primary image
   - Image gallery (swipeable)
   - Gallery indicators (dots)

3. **Item Information**
   - Item Name (large heading)
   - Full Description
   - Prep Time (if applicable)
   - Calorie Info (if applicable)
   - Allergen warnings (prominent)

4. **Dietary Badges**
   - Icons: 🌱 Vegetarian, 🌿 Vegan, etc.
   - Halal, Kosher, Gluten-Free

5. **Variant Selection** (if applicable)
   - Section heading: "Choose type"
   - Radio buttons (single select)
   - Variant name + price
   - Default selected

6. **Add-ons Section** (if applicable)
   - Section heading: "Add Extras"
   - Checkboxes (multi-select)
   - Add-on name + price
   - Max quantity

7. **Footer**
   - Contact buttons (if not e-commerce)
   - Share button

**Modal Behavior:**
- Mobile: Full-screen takeover
- Desktop: Large modal (max 800px width)
- Backdrop click to close
- Swipe down to close (mobile)

---

### 8.7 Catalog Search

**FR-CAT-014: Search Functionality**
- **Priority:** High
- **Description:** Search across all catalog items

**Search Features:**
- Real-time search (AJAX)
- Search by item name
- Search by description
- Search by tags
- Minimum 2 characters to trigger
- Debounced (300ms delay)
- Display results in dropdown/modal

**Result Display:**
- Item image thumbnail
- Item name (highlighted search term)
- Short description
- Price
- Category name
- Tap to view full details

**No Results:**
- "No items found" message
- Suggest: "Try different keywords"
- Show popular items instead

**FR-CAT-015: Filtering**
- **Priority:** Medium
- **Description:** Filter catalog items

**Filter Options:**
- Category
- Price range
- Dietary (Vegetarian, Vegan, Gluten-Free, Halal)
- Availability (In Stock only)
- New items
- Bestsellers

**UI:**
- Filter button (funnel icon)
- Bottom sheet/modal with filters
- Apply/Clear buttons
- Show result count

---

### 8.8 QR Code Integration

**FR-CAT-016: QR Code Generation**
- **Priority:** High
- **Description:** Auto-generate QR codes for catalog access

**QR Types:**
1. **Main Catalog QR**
   - Links to catalog home
   - URL: `/c/{profile-slug}`

2. **Category QR** (optional)
   - Links to specific category
   - URL: `/c/{profile-slug}/category/{category-id}`

3. **Item QR** (optional)
   - Links to specific item
   - URL: `/c/{profile-slug}/item/{item-id}`

**QR Formats:**
- PNG (high resolution, 300 DPI)
- SVG (vector, scalable)
- Downloadable
- Printable (with instructions)

**Features:**
- Customizable size
- Logo overlay (center)
- Color customization
- Error correction level: High

**FR-CAT-017: QR Code Tracking**
- **Priority:** Medium
- **Description:** Track QR code scans

**Analytics:**
- Total scans per QR
- Scans by date/time
- Device type (mobile/tablet/desktop)
- Location (if available)
- Unique vs repeat scans

---

### 8.9 Catalog Analytics

**FR-CAT-018: View Tracking**
- **Priority:** High
- **Description:** Track catalog and item views

**Tracked Events:**
- Catalog home view
- Category view
- Item view
- Search query
- Item click (from search/category)

**Stored Data:**
- Profile ID
- Item/Category ID (if applicable)
- Event type
- Search query (if search)
- IP Address
- User Agent
- Device type
- Timestamp

**FR-CAT-019: Popular Items Report**
- **Priority:** High
- **Description:** Show most viewed/popular items

**Metrics:**
- Total views
- Unique views
- View trend (up/down)
- Most viewed by category
- Least viewed items

**Display:**
- Dashboard widget
- Sortable table
- Date range filter

---

## 9. Module 6: Document Management

### 9.1 Document Upload

**FR-DOC-001: Upload Documents**
- **Priority:** High
- **Description:** Upload business documents to profile

**Supported Formats:**
- PDF (preferred)
- Microsoft Word (DOC, DOCX)
- Images (JPG, PNG)
- Excel (XLS, XLSX)

**Requirements:**
- Max file size: 10MB per document
- Virus scanning on upload
- Store in cloud storage
- Generate thumbnail (for PDFs/images)

**FR-DOC-002: Document Types**
- **Priority:** High
- **Description:** Categorize documents by type

**Types:**
- Brochure
- Flyer
- Menu (PDF)
- Catalog (PDF)
- Price List
- Certificate (business licenses, awards)
- Other

**FR-DOC-003: Document Metadata**
- **Priority:** Medium
- **Description:** Store document information

**Metadata:**
- Document Name (required)
- Description
- Document Type
- File Size
- MIME Type
- Upload Date
- Download Count

---

### 9.2 Document Display

**FR-DOC-004: Document Gallery**
- **Priority:** High
- **Description:** Display documents on profile

**Display Options:**
- Grid view (thumbnail cards)
- List view (with icons)
- Featured documents (larger cards)

**Card Content:**
- Document thumbnail
- Document name
- Document type icon
- File size
- Download button
- View/Preview button (for PDFs)

**FR-DOC-005: Document Visibility**
- **Priority:** High
- **Description:** Control document access

**Visibility Options:**
- Public (anyone can view/download)
- Hidden (not displayed)
- Display order (drag-and-drop)

---

## 10. Module 7: NFC Integration

### 10.1 NFC Products

**FR-NFC-001: Product Catalog**
- **Priority:** High
- **Description:** Offer physical NFC products for sale

**Products:**
1. **NFC Business Card** - R250
   - Premium PVC or metal card
   - Embedded NFC chip
   - Custom printing (logo, name, contact)
   - Rewritable NFC tag

2. **NFC Keyring** - R150
   - Durable plastic/metal
   - Water-resistant
   - Multiple colors available

3. **NFC Sticker** (5-pack) - R120
   - Adhesive NFC stickers
   - Stick to phone, laptop, car
   - Thin profile

**Product Attributes:**
- Product Name
- Type
- Description
- Price
- Image
- Stock status

---

### 10.2 NFC Orders

**FR-NFC-002: Place Order**
- **Priority:** High
- **Description:** Users can order NFC products

**Order Process:**
1. Select product
2. Select quantity
3. Choose profile to link
4. Enter shipping address
5. Confirm order
6. Make payment
7. Receive order confirmation email

**Order Attributes:**
- User ID
- Profile ID (to link NFC tag)
- Product ID
- Quantity
- Total Amount
- Shipping Address
- Order Status
- Tracking Number (when shipped)

**Order Status:**
- Pending (awaiting payment)
- Processing (payment received, preparing)
- Shipped (in transit)
- Delivered
- Cancelled

**FR-NFC-003: Order Management**
- **Priority:** High
- **Description:** Track and manage orders

**User View:**
- Order history
- Order status tracking
- Download invoice
- Request cancellation (before shipping)

**Admin View:**
- All orders dashboard
- Update order status
- Add tracking number
- Process refunds

---

### 10.3 NFC Tag Management

**FR-NFC-004: Assign NFC Tag**
- **Priority:** High
- **Description:** Link NFC tag to profile

**Process:**
1. Admin receives order
2. Admin programs NFC tag with unique UID
3. Admin assigns tag to user's profile in system
4. Tag ships to customer
5. Customer scans tag → redirects to profile

**Tag Attributes:**
- Tag UID (unique identifier)
- Profile ID (linked profile)
- Order ID (order it came from)
- Tag Type (card, keyring, sticker)
- Activation Date
- Is Active (boolean)
- Last Scanned Date
- Scan Count

**FR-NFC-005: NFC Scan Tracking**
- **Priority:** Medium
- **Description:** Track when NFC tags are scanned

**Tracked Data:**
- Tag UID
- Profile ID
- Scan Date/Time
- IP Address
- User Agent
- Device Type
- Location (if available)

**Analytics:**
- Total scans
- Unique scans
- Scans over time
- Most active tags
- Scan locations

**FR-NFC-006: Relink NFC Tag**
- **Priority:** Low
- **Description:** Change which profile an NFC tag links to

**Use Case:**
- User changes jobs (same tag, new company profile)
- User has multiple profiles (switch between them)

**Requirements:**
- User owns the NFC tag
- Verify ownership via email/SMS
- Update tag link in database
- NFC tag physically redirects to new profile

---

## 11. Module 8: Analytics & Reporting

### 11.1 Profile Analytics

**FR-AN-001: Profile Views**
- **Priority:** High
- **Description:** Track profile page views

**Metrics:**
- Total views
- Unique visitors
- Views over time (daily, weekly, monthly)
- Device breakdown (mobile, tablet, desktop)
- Browser breakdown
- Geographic location (country, city)
- Referrer (where visitors came from)

**FR-AN-002: Interaction Tracking**
- **Priority:** High
- **Description:** Track user interactions with profile

**Tracked Events:**
- VCF download
- Share button click
- Phone number click
- Email click
- WhatsApp click
- Website click
- Social media link click
- Map/directions click

**FR-AN-003: NFC/QR Scan Tracking**
- **Priority:** High
- **Description:** Track NFC tag and QR code scans

**Metrics:**
- Total scans
- Scans by source (NFC vs QR)
- Scans over time
- Location of scans
- Device type
- Unique vs repeat scans

---

### 11.2 Catalog Analytics

**FR-AN-004: Catalog Performance**
- **Priority:** High
- **Description:** Track catalog/menu performance

**Metrics:**
- Total catalog views
- Views per category
- Views per item
- Most viewed items
- Least viewed items
- Average time on catalog
- Search queries (what people search for)
- Failed searches (no results)

**FR-AN-005: Item Performance**
- **Priority:** High
- **Description:** Track individual item analytics

**Per-Item Metrics:**
- View count
- View trend (increasing/decreasing)
- Variant selection frequency
- Add-on selection frequency
- Share count

**Use Case:**
- Restaurant sees "WHOPPER" is most viewed item
- Restaurant promotes bestsellers
- Restaurant removes unpopular items

---

### 11.3 Dashboard & Reports

**FR-AN-006: Analytics Dashboard**
- **Priority:** High
- **Description:** Visual dashboard with key metrics

**Dashboard Widgets:**
1. **Overview Cards**
   - Total views (today, this week, this month)
   - Total shares
   - Total downloads
   - NFC/QR scans

2. **Views Over Time Chart**
   - Line chart showing views by day
   - Date range selector
   - Compare to previous period

3. **Top Items/Pages**
   - Bar chart of most viewed
   - Clickable to view details

4. **Device Breakdown**
   - Pie chart (mobile, tablet, desktop)
   - Percentage of each

5. **Geographic Map**
   - World/country map with view locations
   - Zoom to regions

6. **Recent Activity**
   - Live feed of recent views/clicks
   - Real-time updates (optional)

**FR-AN-007: Export Reports**
- **Priority:** Medium
- **Description:** Download analytics data

**Export Formats:**
- CSV (raw data)
- PDF (formatted report with charts)
- Excel (XLSX)

**Report Types:**
- Profile performance report
- Catalog performance report
- Custom date range report
- Monthly summary report (auto-emailed)

**FR-AN-008: Real-Time Analytics**
- **Priority:** Low
- **Description:** Live analytics updates

**Features:**
- Dashboard updates every 30 seconds
- Notification on new view/interaction
- Live visitor count
- Available to Professional+ tiers

---

## 12. Module 9: Subscription Management

### 12.1 Subscription Tiers

**FR-SUB-001: Tier Definitions**
- **Priority:** Critical
- **Description:** Four subscription tiers with different features

| Feature | Free | Basic (R99) | Professional (R249) | Enterprise (R499) |
|---------|------|-------------|---------------------|-------------------|
| **Profiles** | 1 | 3 | 10 | Unlimited |
| **Templates** | 2 | 5 | All | All |
| **Photos per profile** | 3 | 10 | 50 | Unlimited |
| **Catalog items** | - | 100 | 500 | Unlimited |
| **Documents** | 3 | 10 | 50 | Unlimited |
| **Team members** | - | 10 | 50 | Unlimited |
| **Custom branding** | ❌ | ✅ | ✅ | ✅ |
| **Analytics** | ❌ | ✅ | ✅ | ✅ |
| **NFC integration** | ❌ | ✅ | ✅ | ✅ |
| **Custom domain** | ❌ | ❌ | ✅ | ✅ |
| **Remove branding** | ❌ | ❌ | ✅ | ✅ |
| **Featured listing** | ❌ | ❌ | ✅ | ✅ |
| **Priority support** | ❌ | ❌ | ✅ | ✅ |
| **API access** | ❌ | ❌ | ❌ | ✅ |

**FR-SUB-002: Tier Enforcement**
- **Priority:** Critical
- **Description:** Enforce tier limits throughout system

**Enforcement Points:**
- Profile creation (count check)
- Catalog item creation (count check)
- Document upload (count check)
- Feature access (permission check)
- UI elements (hide unavailable features)

**Upgrade Prompts:**
- Show upgrade button when limit reached
- "Upgrade to [tier] to add more..."
- Compare tier features

---

### 12.2 Subscription Operations

**FR-SUB-003: Subscribe**
- **Priority:** Critical
- **Description:** Users can subscribe to paid tiers

**Subscription Flow:**
1. View pricing page
2. Select tier (Monthly or Annual)
3. Review features and cost
4. Enter payment information
5. Process payment
6. Activate subscription
7. Send confirmation email

**Annual Discount:**
- 15-20% discount on annual payment
- Display savings prominently
- Default to annual (upsell)

**FR-SUB-004: Upgrade/Downgrade**
- **Priority:** High
- **Description:** Change subscription tier

**Upgrade:**
- Immediate access to new tier features
- Pro-rated credit for remaining period
- Pay difference immediately

**Downgrade:**
- Takes effect at end of current billing period
- Credit applied to next invoice
- Warning if exceeding new tier limits
- Grace period to adjust (30 days)

**FR-SUB-005: Cancel Subscription**
- **Priority:** High
- **Description:** Cancel paid subscription

**Cancellation Process:**
1. Confirm cancellation
2. Ask reason (survey, optional)
3. Offer discount/pause instead
4. Confirm cancellation
5. Access until end of billing period
6. Downgrade to Free tier
7. Send cancellation confirmation email

**Data Retention:**
- All data preserved
- Features locked per Free tier
- Option to resubscribe anytime

**FR-SUB-006: Pause Subscription**
- **Priority:** Low
- **Description:** Temporarily pause subscription

**Use Case:**
- Seasonal business (closed for winter)
- Personal leave

**Requirements:**
- Maximum pause: 3 months
- Features locked during pause
- No charges during pause
- Resume anytime
- Available to Professional+ tiers

---

### 12.3 Billing

**FR-SUB-007: Billing Cycle**
- **Priority:** High
- **Description:** Automatic recurring billing

**Cycles:**
- Monthly: Billed every 30 days
- Annual: Billed every 365 days

**Billing Date:**
- Set on initial subscription
- Same day each month/year
- Email reminder 7 days before

**FR-SUB-008: Failed Payment**
- **Priority:** High
- **Description:** Handle failed payment attempts

**Failed Payment Flow:**
1. Retry payment 3 times (day 1, 3, 5)
2. Email notification on each failure
3. Downgrade to Free after 5 days
4. 30-day grace period to update payment
5. Account suspended after 30 days
6. Data deleted after 90 days (with notice)

**FR-SUB-009: Invoices**
- **Priority:** Medium
- **Description:** Generate and send invoices

**Invoice Content:**
- Invoice number
- Date
- Billing period
- Tier name
- Amount
- VAT (15%)
- Payment method
- Company details (for VAT invoices)

**Delivery:**
- Email PDF on successful payment
- Download from account dashboard
- View invoice history

---

## 13. Module 10: Payment Processing

### 13.1 Payment Gateway

**FR-PAY-001: Payment Provider**
- **Priority:** Critical
- **Description:** Integrate with PayFast (South African payment gateway)

**Supported Methods:**
- Credit/Debit Cards (Visa, MasterCard)
- Instant EFT
- SnapScan
- Zapper
- Mobicred
- PayPal (optional)

**FR-PAY-002: Secure Payment**
- **Priority:** Critical
- **Description:** PCI-DSS compliant payment handling

**Security Requirements:**
- HTTPS required
- No card details stored on server
- Tokenization via PayFast
- 3D Secure verification
- CVV verification
- Fraud detection

---

### 13.2 Payment Flows

**FR-PAY-003: Subscription Payment**
- **Priority:** Critical
- **Description:** Process subscription payments

**Flow:**
1. User selects tier
2. Redirect to PayFast payment page
3. User enters payment details
4. PayFast processes payment
5. PayFast sends webhook to BizBio
6. BizBio activates subscription
7. Redirect user to success page

**FR-PAY-004: NFC Product Payment**
- **Priority:** High
- **Description:** Process NFC product orders

**Flow:**
1. User adds products to cart
2. Enter shipping address
3. Redirect to payment page
4. Process payment
5. Create order in system
6. Send order confirmation
7. Admin fulfills order

**FR-PAY-005: Refunds**
- **Priority:** Medium
- **Description:** Process refund requests

**Refund Policy:**
- Subscription: Pro-rated refund within 14 days
- NFC Products: Full refund if unopened within 30 days

**Refund Process:**
1. User requests refund (support ticket)
2. Admin reviews and approves
3. Process refund via PayFast
4. Notify user
5. Cancel subscription or order

---

### 13.3 Transaction Records

**FR-PAY-006: Transaction Logging**
- **Priority:** High
- **Description:** Store all payment transactions

**Transaction Data:**
- User ID
- Transaction Type (subscription, NFC order)
- Amount
- Currency (ZAR)
- Payment Method
- PayFast Reference
- Status (pending, completed, failed, refunded)
- Transaction Date

**FR-PAY-007: Payment History**
- **Priority:** Medium
- **Description:** Users can view payment history

**Display:**
- List of all transactions
- Date, amount, description
- Invoice download link
- Receipt download
- Filter by date range
- Export to CSV

---

## 14. Integration Requirements

### 14.1 Email Service

**INT-001: Email Provider**
- **Description:** SMTP or email service (SendGrid, Mailgun)

**Email Types:**
- Welcome email (registration)
- Email verification
- Password reset
- Subscription confirmation
- Invoice/receipt
- NFC order confirmation
- Monthly report summary

**INT-002: Email Templates**
- **Description:** HTML email templates

**Template Requirements:**
- Responsive design
- Brand colors
- Logo
- Call-to-action buttons
- Unsubscribe link

---

### 14.2 File Storage

**INT-003: Cloud Storage**
- **Description:** AWS S3 or Azure Blob Storage

**Stored Files:**
- Profile images (logos, covers, photos)
- Catalog item images
- Documents (PDFs, brochures)
- Videos
- NFC product images

**Requirements:**
- Secure access (signed URLs)
- CDN integration (CloudFlare)
- Automatic backups
- Image optimization pipeline

---

### 14.3 CDN

**INT-004: Content Delivery Network**
- **Description:** CloudFlare for global content delivery

**Benefits:**
- Faster image loading
- Reduced bandwidth costs
- DDoS protection
- SSL/TLS encryption
- Caching

---

### 14.4 QR Code Generation

**INT-005: QR Code Library**
- **Description:** QRCoder library (.NET)

**Features:**
- Generate QR codes programmatically
- Customizable size and quality
- Logo overlay
- Multiple formats (PNG, SVG)

---

### 14.5 Maps Integration

**INT-006: Google Maps API**
- **Description:** Integrate Google Maps

**Features:**
- Address autocomplete
- Geocoding (address → coordinates)
- Embedded maps
- Directions link
- Distance calculation

**API Keys Required:**
- Maps JavaScript API
- Places API
- Geocoding API

---

## 15. Non-Functional Requirements

### 15.1 Performance

**NFR-001: Page Load Time**
- **Requirement:** < 3 seconds on 4G connection
- **Measurement:** Google PageSpeed Insights score > 80

**NFR-002: Database Queries**
- **Requirement:** < 100ms average query time
- **Implementation:** Proper indexing, query optimization

**NFR-003: Image Loading**
- **Requirement:** Progressive loading, lazy loading
- **Implementation:** WebP format, < 500KB per image

**NFR-004: Concurrent Users**
- **Requirement:** Support 1,000 concurrent users
- **Implementation:** Load testing, horizontal scaling

---

### 15.2 Scalability

**NFR-005: Database Scalability**
- **Requirement:** Support 100,000 users, 500,000 profiles
- **Implementation:** Database sharding, read replicas

**NFR-006: Storage Scalability**
- **Requirement:** Support 10TB of user files
- **Implementation:** Cloud storage with automatic scaling

**NFR-007: Catalog Items**
- **Requirement:** Support 10,000 items per catalog
- **Implementation:** Pagination, search indexing

---

### 15.3 Availability

**NFR-008: Uptime**
- **Requirement:** 99.9% uptime (< 8.76 hours downtime/year)
- **Implementation:** Redundant servers, automated failover

**NFR-009: Backup**
- **Requirement:** Daily automated backups, 30-day retention
- **Implementation:** Database backups, file backups

**NFR-010: Disaster Recovery**
- **Requirement:** Recovery time objective (RTO) < 4 hours
- **Implementation:** Backup restoration procedures

---

### 15.4 Security

**NFR-011: Data Encryption**
- **Requirement:** Encryption in transit (HTTPS) and at rest
- **Implementation:** SSL/TLS, database encryption

**NFR-012: Authentication**
- **Requirement:** Secure password storage, session management
- **Implementation:** BCrypt hashing, CSRF protection

**NFR-013: Vulnerability Scanning**
- **Requirement:** Regular security audits
- **Implementation:** OWASP Top 10 compliance

---

### 15.5 Usability

**NFR-014: Mobile Responsiveness**
- **Requirement:** Full functionality on mobile devices
- **Implementation:** Responsive design, touch-friendly UI

**NFR-015: Accessibility**
- **Requirement:** WCAG 2.1 Level AA compliance
- **Implementation:** Semantic HTML, alt text, keyboard navigation

**NFR-016: Browser Support**
- **Requirement:** Support latest 2 versions of major browsers
- **Browsers:** Chrome, Safari, Firefox, Edge

---

## 16. Data Requirements

### 16.1 Data Storage

**DR-001: Database**
- **Type:** MySQL 8.0+
- **Size:** Initial 10GB, scalable to 1TB+
- **Tables:** 29 tables
- **Relationships:** Enforced foreign keys

**DR-002: File Storage**
- **Type:** Cloud storage (S3/Azure Blob)
- **Size:** 10TB capacity
- **Access:** CDN for public files, signed URLs for private

---

### 16.2 Data Privacy

**DR-003: Personal Data**
- **Regulation:** POPIA (South Africa), GDPR (if EU users)
- **Requirements:**
  - Explicit consent for data collection
  - Right to access personal data
  - Right to data portability (export)
  - Right to deletion (with 30-day grace period)
  - Data breach notification (within 72 hours)

**DR-004: Data Retention**
- **Active Users:** Indefinite storage while subscribed
- **Cancelled Subscriptions:** 90 days before deletion (with notice)
- **Deleted Accounts:** Immediate anonymization, 30-day recovery period

---

### 16.3 Data Backup

**DR-005: Backup Schedule**
- **Daily:** Full database backup at 02:00 AM
- **Weekly:** Full system backup (database + files)
- **Retention:** 30 days

**DR-006: Backup Testing**
- **Frequency:** Monthly restoration test
- **Verification:** Ensure data integrity

---

## 17. Security Requirements

### 17.1 Authentication & Authorization

**SEC-001: Password Policy**
- Minimum 8 characters
- Must include: uppercase, lowercase, number
- Special characters encouraged
- No common passwords (check against list)
- Max 128 characters

**SEC-002: Session Management**
- Session timeout: 12 hours (without remember me)
- Persistent session: 30 days (with remember me)
- One session per device
- Session invalidation on logout

**SEC-003: Failed Login Protection**
- Max 5 attempts per 15 minutes
- Account lockout: 15 minutes
- Email notification on lockout
- CAPTCHA after 3 failures

---

### 17.2 Data Security

**SEC-004: HTTPS Enforcement**
- All traffic over HTTPS
- HTTP automatically redirects to HTTPS
- HSTS enabled

**SEC-005: Input Validation**
- Sanitize all user inputs
- SQL injection prevention (parameterized queries)
- XSS protection (encode output)
- CSRF tokens on all forms

**SEC-006: File Upload Security**
- Validate file types (whitelist)
- Scan for viruses/malware
- Restrict file size (10MB documents, 5MB images)
- Store files outside web root
- Generate random filenames

---

### 17.3 Payment Security

**SEC-007: PCI-DSS Compliance**
- No card data stored on server
- All payments via PayFast (PCI-compliant gateway)
- Tokenization for recurring payments
- Secure transmission (HTTPS)

---

## 18. User Stories & Use Cases

### 18.1 Individual Professional

**User Story 1:**
> As a freelance graphic designer, I want to create a digital business card so that I can share my contact information and portfolio easily via QR code or NFC.

**Acceptance Criteria:**
- Create personal profile with name, job title, contact info
- Upload portfolio images (up to 10 photos)
- Customize colors and fonts to match personal brand
- Generate QR code for profile
- Track how many people view my profile

---

**User Story 2:**
> As a business consultant, I want to have multiple profiles so that I can have separate profiles for different client engagements.

**Acceptance Criteria:**
- Create up to 3 profiles (Basic tier)
- Different branding for each profile
- Different contact information per profile
- Switch between profiles in dashboard

---

### 18.2 Restaurant Owner

**User Story 3:**
> As a restaurant owner, I want to create a digital menu accessible via QR code so that customers can view our menu on their phones without physical menus.

**Acceptance Criteria:**
- Enable catalog feature for restaurant profile
- Create menu categories (Breakfast, Lunch, Dinner, Drinks)
- Add 50+ menu items with photos and descriptions
- Set prices and add variants (Small, Medium, Large)
- Mark items as "Chef's Special" or "New"
- Generate QR codes for tables
- Track which menu items are viewed most

---

**User Story 4:**
> As a restaurant manager, I want to update menu prices quickly so that I can respond to ingredient cost changes.

**Acceptance Criteria:**
- Edit item prices from dashboard
- Changes reflected immediately on public menu
- Bulk price update (e.g., increase all by 5%)
- Price history tracking

---

### 18.3 Company Owner

**User Story 5:**
> As a company owner, I want to create a company profile with my team members so that clients can see our team and contact the right person.

**Acceptance Criteria:**
- Create organization profile
- Add company logo, description, contact info
- Add 10 team members with their roles
- Each team member has their own profile
- Team members displayed on company profile
- Click team member → view their full profile

---

**User Story 6:**
> As an HR manager, I want to manage team member profiles so that our company directory is always up-to-date.

**Acceptance Criteria:**
- Add new team members (create profiles or link existing)
- Update team member job titles and departments
- Remove former employees (but keep data for reference)
- Set which team members are featured on homepage
- Reorder team member display

---

### 18.4 Retail Store

**User Story 7:**
> As a retail store owner, I want to showcase my products online so that customers can browse before visiting my store.

**Acceptance Criteria:**
- Create product catalog
- Organize products into categories
- Add 100+ products with photos, descriptions, prices
- Add product variants (colors, sizes)
- Upload product brochure (PDF)
- Share catalog link on social media
- Track which products are most viewed

---

### 18.5 Customer

**User Story 8:**
> As a customer at a restaurant, I want to scan a QR code to view the menu so that I can order quickly.

**Acceptance Criteria:**
- Scan QR code with phone camera (no app needed)
- Menu loads in browser within 3 seconds
- Browse menu categories
- View item photos and descriptions
- See prices and item options
- Contact restaurant (call, WhatsApp)

---

**User Story 9:**
> As someone networking at an event, I want to receive a digital business card so that I don't have to carry paper cards.

**Acceptance Criteria:**
- Scan person's NFC card or QR code
- View their profile in browser
- See contact information
- Download VCF file to phone contacts
- Save for later reference

---

## 19. Success Criteria

### 19.1 Adoption Metrics

**Metric:** User Registrations  
**Target:** 1,000 registered users in first 6 months  
**Measurement:** User count in database

**Metric:** Active Profiles  
**Target:** 500 published profiles in first 6 months  
**Measurement:** Published profile count

**Metric:** Paid Conversions  
**Target:** 15% conversion from Free to paid tier  
**Measurement:** (Paid users / Total users) × 100

**Metric:** Restaurant Adoption  
**Target:** 50 restaurants with active catalogs in first 3 months  
**Measurement:** Profiles with catalog enabled and > 10 items

---

### 19.2 Engagement Metrics

**Metric:** Profile Views  
**Target:** Average 500 views/month per profile  
**Measurement:** Analytics data

**Metric:** Catalog Views  
**Target:** Average 1,000 views/month per restaurant  
**Measurement:** Catalog analytics

**Metric:** NFC Scans  
**Target:** Average 20 scans/week per active NFC tag  
**Measurement:** NFC scan tracking

**Metric:** Return Visitors  
**Target:** 30% return rate  
**Measurement:** Unique vs total visitors

---

### 19.3 Business Metrics

**Metric:** Monthly Recurring Revenue (MRR)  
**Target:** R50,000 by month 6  
**Measurement:** Sum of active subscription revenue

**Metric:** Customer Lifetime Value (CLV)  
**Target:** R2,500  
**Measurement:** Average revenue per customer over lifetime

**Metric:** Churn Rate  
**Target:** < 10% monthly  
**Measurement:** (Cancelled subscriptions / Active subscriptions) × 100

**Metric:** Net Promoter Score (NPS)  
**Target:** > 50  
**Measurement:** User survey "How likely to recommend?" (0-10 scale)

---

### 19.4 Performance Metrics

**Metric:** Page Load Time  
**Target:** < 3 seconds  
**Measurement:** Google PageSpeed Insights

**Metric:** Uptime  
**Target:** 99.9%  
**Measurement:** Uptime monitoring service

**Metric:** Error Rate  
**Target:** < 1%  
**Measurement:** Error logging

**Metric:** Support Response Time  
**Target:** < 24 hours (Free/Basic), < 4 hours (Professional+)  
**Measurement:** Support ticket system

---

## 20. Assumptions & Constraints

### 20.1 Assumptions

1. Users have smartphones with NFC or QR code scanning capability
2. Majority of traffic will be mobile (80%+)
3. Users comfortable with digital payments
4. Reliable internet connection in target markets
5. English primary language (multi-language future phase)
6. South African Rand (ZAR) primary currency

### 20.2 Constraints

1. **Budget:** Initial development budget constraints
2. **Timeline:** MVP delivery in 12 weeks
3. **Team Size:** Small development team
4. **Technology:** Limited to .NET 6 and MySQL (cPanel hosting)
5. **Payment Gateway:** Limited to PayFast (South African gateway)
6. **Legal:** POPIA compliance mandatory (South African privacy law)

### 20.3 Dependencies

1. **PayFast:** Payment processing dependent on third-party
2. **Cloud Storage:** AWS/Azure availability and pricing
3. **Email Service:** Third-party email delivery (SendGrid/Mailgun)
4. **Google Maps API:** Map features dependent on Google API
5. **Browser Support:** Modern browser features required

---

## 21. Appendices

### Appendix A: Glossary

| Term | Definition |
|------|------------|
| **Profile** | A digital business card or page for an individual or organization |
| **Catalog** | Collection of products, menu items, or services |
| **Category** | Grouping of related items within a catalog |
| **Item** | Individual product, dish, or service in a catalog |
| **Variant** | Different size, flavor, or option of an item |
| **Add-on** | Optional extra that can be added to an item |
| **NFC** | Near Field Communication - technology for contactless data transfer |
| **QR Code** | Quick Response code - 2D barcode scanned by cameras |
| **VCF** | vCard Format - digital business card file format |
| **Tier** | Subscription level (Free, Basic, Professional, Enterprise) |
| **Profile Owner** | User who created and controls a profile |
| **Organization Profile** | Profile type for companies with team structures |
| **Team Member** | Individual profile linked to an organization |
| **Relationship** | Connection between profiles (e.g., employee, manager) |

### Appendix B: Acronyms

| Acronym | Definition |
|---------|------------|
| **API** | Application Programming Interface |
| **CDN** | Content Delivery Network |
| **CRUD** | Create, Read, Update, Delete |
| **CSV** | Comma-Separated Values |
| **EFT** | Electronic Funds Transfer |
| **FRD** | Functional Requirements Document |
| **GDPR** | General Data Protection Regulation |
| **GPS** | Global Positioning System |
| **HTTPS** | Hypertext Transfer Protocol Secure |
| **JSON** | JavaScript Object Notation |
| **MRR** | Monthly Recurring Revenue |
| **MVC** | Model-View-Controller |
| **NFC** | Near Field Communication |
| **NPS** | Net Promoter Score |
| **ORM** | Object-Relational Mapping |
| **PDF** | Portable Document Format |
| **POPIA** | Protection of Personal Information Act |
| **QR** | Quick Response |
| **SaaS** | Software as a Service |
| **SEO** | Search Engine Optimization |
| **SKU** | Stock Keeping Unit |
| **SMS** | Short Message Service |
| **SSL** | Secure Sockets Layer |
| **TLS** | Transport Layer Security |
| **UI** | User Interface |
| **URL** | Uniform Resource Locator |
| **UX** | User Experience |
| **VAT** | Value Added Tax |
| **VCF** | vCard Format |
| **WCAG** | Web Content Accessibility Guidelines |

### Appendix C: Database Schema Summary

**Total Tables:** 29

**Core Tables:**
1. Users
2. SubscriptionTiers
3. UserSubscriptions
4. Categories (business categories)
5. ProfileTemplates
6. Profiles
7. ProfileBranding
8. ProfileRelationships

**Hierarchy Tables:**
9. Team member relationships stored in ProfileRelationships

**Catalog Tables:**
10. ProfileCatalogs
11. CatalogCategories
12. CatalogItems
13. CatalogItemVariants
14. CatalogItemImages
15. CatalogItemAddons
16. CatalogAnalytics

**Media & Content Tables:**
17. SocialMediaLinks
18. ProfilePhotos
19. ProfileVideos
20. ProfileDocuments
21. ProfileQRCodes

**NFC Tables:**
22. NFCProducts
23. NFCOrders
24. NFCTags

**Analytics Tables:**
25. ProfileAnalytics

**Services & Support:**
26. ProfileServices
27. ProfileCustomFields
28. ProfileReviews
29. SupportTickets
30. PaymentTransactions (for payments)

### Appendix D: URL Structure

**Public URLs:**
- Profile: `https://bizbio.co.za/{profile-slug}`
- Catalog: `https://bizbio.co.za/c/{profile-slug}`
- Category: `https://bizbio.co.za/c/{profile-slug}/category/{category-id}`
- Search: `https://bizbio.co.za/search?q={query}`

**Authenticated URLs:**
- Dashboard: `https://bizbio.co.za/dashboard`
- Manage Profile: `https://bizbio.co.za/manage/profile/{id}`
- Manage Catalog: `https://bizbio.co.za/manage/catalog/{profile-id}`
- Manage Team: `https://bizbio.co.za/manage/team/{org-id}`

---

## Document Approval

| Role | Name | Signature | Date |
|------|------|-----------|------|
| Product Owner | | | |
| Technical Lead | | | |
| Stakeholder | | | |

---

**End of Document**

**Version:** 2.0  
**Date:** November 2024  
**Status:** Approved for Development  
**Next Review:** December 2024