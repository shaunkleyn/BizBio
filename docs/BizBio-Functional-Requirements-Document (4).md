# BizBio Digital Profile Platform
## Functional Requirements Document (FRD) v3.0

**Document Version:** 3.0  
**Date:** November 2025  
**Project Name:** BizBio  
**Project Type:** SaaS Platform  
**Status:** Ready for Development

---

## Document Control

| Version | Date | Author | Changes |
|---------|------|--------|---------|
| 1.0 | Oct 2024 | Development Team | Initial draft |
| 2.0 | Nov 2024 | Development Team | Added Hierarchy & Catalog modules |
| 3.0 | Nov 2025 | Development Team | Updated architecture: Vue.js frontend, ASP.NET API backend |

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
This document defines the complete functional requirements for the BizBio Digital Profile Platform, a comprehensive SaaS solution for creating, managing, and sharing digital business cards, organizational profiles, and product catalogs. This version reflects the updated technical architecture with a decoupled frontend and backend.

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
6. Prepare API for future public exposure and third-party integrations

---

## 2. System Overview

### 2.1 Platform Architecture

```
BizBio Platform (Decoupled Architecture)
│
├── Frontend (Vue.js SPA)
│   ├── Public Profile Views (SSR/SEO-optimized)
│   ├── Catalog/Menu Views
│   ├── User Dashboard
│   ├── Admin Portal
│   └── Mobile-Responsive UI (Tailwind CSS)
│
├── Backend (ASP.NET 6 RESTful API)
│   ├── Authentication & Authorization (JWT)
│   ├── Profile Management API
│   ├── Catalog Management API
│   ├── File Storage API
│   ├── Analytics API
│   ├── Payment Processing API
│   └── Admin API
│
├── Database (MySQL)
│   ├── Code-First Design
│   ├── Integer-based IDs
│   └── 29 Tables (Relational Schema)
│
└── Third-Party Services
    ├── Payment Gateway (PayFast)
    ├── File Storage (Cloud CDN)
    ├── Email Service
    └── Maps API
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

**Frontend:**
- Framework: Vue.js 3 (Composition API)
- UI Framework: Tailwind CSS 3.x
- State Management: Pinia
- Routing: Vue Router 4
- HTTP Client: Axios
- Build Tool: Vite
- Responsive Design: Mobile-first approach

**Backend:**
- Framework: ASP.NET 6 Web API
- Language: C# 10
- ORM: Entity Framework Core 6 (Code-First)
- Database: MySQL 8.0+
- Authentication: JWT (Bearer tokens)
- API Documentation: Swagger/OpenAPI

**Infrastructure:**
- Hosting: Separate hosting for frontend and API
- File Storage: AWS S3 / Azure Blob Storage
- CDN: CloudFlare
- SSL: Let's Encrypt (HTTPS required)
- API Gateway: Ready for future implementation

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

### 3.3 API Access Control
- All API endpoints require authentication (except public profile views)
- Role-based access control enforced at API level
- Rate limiting based on subscription tier
- Future: API keys for third-party integrations (Enterprise tier)

---

## 4. Module 1: User Management

### 4.1 User Registration

**FR-UM-001: User Registration via API**
- **Priority:** Critical
- **Description:** Users can create accounts through the Vue.js frontend communicating with the API

**Acceptance Criteria:**
- Frontend validates input before API submission
- API performs server-side validation
- Email validation required
- Password minimum 8 characters, must include uppercase, lowercase, number
- Email verification link sent
- Account created in inactive state until verified
- Duplicate email detection
- Terms of service acceptance required
- POPIA/GDPR consent checkboxes
- Returns JWT token upon successful verification

**API Endpoint:**
```
POST /api/v1/auth/register
Request Body: {
  "email": "string",
  "password": "string",
  "firstName": "string",
  "lastName": "string",
  "phoneNumber": "string (optional)",
  "acceptedTerms": true,
  "acceptedPrivacyPolicy": true
}
Response: {
  "success": true,
  "message": "Verification email sent",
  "userId": 123
}
```

**Fields:**
- Email (required, unique)
- Password (required, hashed with bcrypt)
- First Name (required)
- Last Name (required)
- Phone Number (optional)

**Validation:**
- Client-side validation in Vue.js
- Server-side validation in API
- Email format validation
- Password strength validation
- Phone number format validation (South African format)

---

### 4.2 Authentication

**FR-UM-002: User Login via JWT**
- **Priority:** Critical
- **Description:** Registered users can log in and receive JWT tokens

**Acceptance Criteria:**
- Email/password authentication
- JWT token issued on successful login
- Refresh token for extended sessions
- Token expiry: 1 hour (access), 7 days (refresh)
- Failed login attempt tracking (max 5 attempts, 15-minute lockout)
- Last login timestamp recorded
- Frontend stores token securely (httpOnly cookies or secure storage)

**API Endpoint:**
```
POST /api/v1/auth/login
Request Body: {
  "email": "string",
  "password": "string",
  "rememberMe": boolean
}
Response: {
  "success": true,
  "accessToken": "string",
  "refreshToken": "string",
  "expiresIn": 3600,
  "user": {
    "id": 123,
    "email": "string",
    "firstName": "string",
    "lastName": "string",
    "role": "string",
    "subscriptionTier": "string"
  }
}
```

**FR-UM-003: Email Verification**
- **Priority:** Critical
- **Description:** Users must verify email before full access

**API Endpoint:**
```
POST /api/v1/auth/verify-email
Request Body: {
  "token": "string"
}
Response: {
  "success": true,
  "message": "Email verified successfully",
  "accessToken": "string"
}
```

**FR-UM-004: Password Reset**
- **Priority:** High
- **Description:** Users can reset forgotten passwords

**API Endpoints:**
```
POST /api/v1/auth/forgot-password
Request Body: {
  "email": "string"
}
Response: {
  "success": true,
  "message": "Reset link sent to email"
}

POST /api/v1/auth/reset-password
Request Body: {
  "token": "string",
  "newPassword": "string"
}
Response: {
  "success": true,
  "message": "Password reset successfully"
}
```

---

### 4.3 Account Management

**FR-UM-005: Profile Settings via API**
- **Priority:** High
- **Description:** Users can update account information

**API Endpoint:**
```
PUT /api/v1/users/{userId}
Headers: Authorization: Bearer {token}
Request Body: {
  "firstName": "string",
  "lastName": "string",
  "phoneNumber": "string",
  "currentPassword": "string (if changing email/password)",
  "newPassword": "string (optional)",
  "newEmail": "string (optional)"
}
Response: {
  "success": true,
  "user": { updated user object }
}
```

**FR-UM-006: Account Deletion**
- **Priority:** Medium
- **Description:** Users can delete their accounts

**API Endpoint:**
```
DELETE /api/v1/users/{userId}
Headers: Authorization: Bearer {token}
Request Body: {
  "password": "string",
  "confirmation": "DELETE"
}
Response: {
  "success": true,
  "message": "Account deleted successfully"
}
```

**Acceptance Criteria:**
- Confirmation required in frontend
- Password verification required
- All user data anonymized or deleted
- Profiles set to inactive
- Subscriptions cancelled
- 30-day grace period before permanent deletion
- Export user data option (GDPR compliance)

---

## 5. Module 2: Profile System

### 5.1 Profile Creation

**FR-PR-001: Create Profile via API**
- **Priority:** Critical
- **Description:** Users can create digital profiles through the API

**API Endpoint:**
```
POST /api/v1/profiles
Headers: Authorization: Bearer {token}
Request Body: {
  "profileType": "Personal|Business|Organization",
  "displayName": "string",
  "slug": "string (optional, auto-generated if not provided)",
  "bio": "string",
  "businessCategory": "string",
  "templateId": integer (optional),
  "contactInfo": {
    "email": "string",
    "phone": "string",
    "website": "string"
  },
  "address": {
    "street": "string",
    "city": "string",
    "province": "string",
    "country": "string",
    "postalCode": "string",
    "latitude": decimal,
    "longitude": decimal
  }
}
Response: {
  "success": true,
  "profile": {
    "id": 123,
    "slug": "string",
    "publicUrl": "https://bizbio.co.za/profile-slug",
    ...profile data
  }
}
```

**Profile Types:**
1. **Personal Profile** - Individual professional
2. **Business Profile** - Small business or sole proprietor
3. **Organization Profile** - Company with team structure

**Acceptance Criteria:**
- Frontend validates profile limits based on subscription tier
- API enforces subscription tier limits
- Unique slug generation
- Slug validation (URL-safe)
- Profile created in draft state
- Template application (if selected)
- Default branding applied
- Integer-based ID assigned

---

### 5.2 Profile Management

**FR-PR-002: View Profile**

**API Endpoints:**
```
GET /api/v1/profiles/{profileId}
Headers: Authorization: Bearer {token} (for private profiles)
Response: { full profile object }

GET /api/v1/profiles/by-slug/{slug}
Response: { public profile data }
```

**FR-PR-003: Update Profile**

**API Endpoint:**
```
PUT /api/v1/profiles/{profileId}
Headers: Authorization: Bearer {token}
Request Body: { updated profile fields }
Response: {
  "success": true,
  "profile": { updated profile object }
}
```

**FR-PR-004: Delete Profile**

**API Endpoint:**
```
DELETE /api/v1/profiles/{profileId}
Headers: Authorization: Bearer {token}
Response: {
  "success": true,
  "message": "Profile deleted"
}
```

**FR-PR-005: Profile Visibility**

**API Endpoint:**
```
PATCH /api/v1/profiles/{profileId}/visibility
Headers: Authorization: Bearer {token}
Request Body: {
  "isPublic": boolean,
  "isSearchable": boolean
}
Response: {
  "success": true,
  "visibility": { updated visibility settings }
}
```

---

### 5.3 Profile Content

**FR-PR-006: Contact Information via API**

**API Endpoint:**
```
PUT /api/v1/profiles/{profileId}/contact
Headers: Authorization: Bearer {token}
Request Body: {
  "email": "string",
  "phone": "string",
  "alternatePhone": "string",
  "website": "string",
  "whatsapp": "string",
  "showEmail": boolean,
  "showPhone": boolean
}
Response: { success: true, contactInfo: {...} }
```

**FR-PR-007: Social Media Links**

**API Endpoints:**
```
POST /api/v1/profiles/{profileId}/social-links
PUT /api/v1/profiles/{profileId}/social-links/{linkId}
DELETE /api/v1/profiles/{profileId}/social-links/{linkId}
GET /api/v1/profiles/{profileId}/social-links

Request/Response includes:
{
  "platform": "Facebook|Instagram|LinkedIn|Twitter|YouTube|TikTok|Custom",
  "url": "string",
  "displayOrder": integer,
  "isActive": boolean
}
```

**FR-PR-008: Profile Photos & Media**

**API Endpoints:**
```
POST /api/v1/profiles/{profileId}/photos
Headers: Authorization: Bearer {token}
Content-Type: multipart/form-data
Request Body: FormData with file

DELETE /api/v1/profiles/{profileId}/photos/{photoId}

POST /api/v1/profiles/{profileId}/videos
POST /api/v1/profiles/{profileId}/cover-image
```

**Acceptance Criteria:**
- File upload handled by frontend
- API validates file type and size
- Images optimized and stored in CDN
- Multiple photo support (gallery)
- Profile picture and cover image
- Video embed support (YouTube, Vimeo)
- Integer-based IDs for all media

---

## 6. Module 3: Profile Customization

### 6.1 Branding & Themes

**FR-PC-001: Custom Branding via API**
- **Priority:** High
- **Description:** Users can customize profile appearance

**API Endpoint:**
```
PUT /api/v1/profiles/{profileId}/branding
Headers: Authorization: Bearer {token}
Request Body: {
  "primaryColor": "#hex",
  "secondaryColor": "#hex",
  "backgroundColor": "#hex",
  "textColor": "#hex",
  "fontFamily": "string",
  "logoUrl": "string",
  "faviconUrl": "string",
  "customCSS": "string (Professional+ tier)"
}
Response: {
  "success": true,
  "branding": { branding object }
}
```

**FR-PC-002: Template Selection**

**API Endpoints:**
```
GET /api/v1/templates
Response: [ array of available templates ]

POST /api/v1/profiles/{profileId}/apply-template
Request Body: { "templateId": integer }
```

**FR-PC-003: Layout Configuration**

**API Endpoint:**
```
PUT /api/v1/profiles/{profileId}/layout
Request Body: {
  "sections": [
    {
      "sectionType": "About|Contact|Services|Gallery|Reviews",
      "isVisible": boolean,
      "displayOrder": integer,
      "configuration": { section-specific config }
    }
  ]
}
```

---

## 7. Module 4: Organizational Hierarchy

### 7.1 Team Management

**FR-OH-001: Add Team Member**

**API Endpoint:**
```
POST /api/v1/profiles/{orgProfileId}/team-members
Headers: Authorization: Bearer {token}
Request Body: {
  "memberProfileId": integer,
  "relationshipType": "Employee|Contractor|Partner|Manager",
  "jobTitle": "string",
  "department": "string",
  "startDate": "date",
  "permissions": ["view", "edit", "admin"]
}
Response: {
  "success": true,
  "relationship": { relationship object with integer ID }
}
```

**FR-OH-002: View Organization Structure**

**API Endpoint:**
```
GET /api/v1/profiles/{orgProfileId}/team-members
Response: {
  "teamMembers": [
    {
      "relationshipId": integer,
      "memberProfile": { profile summary },
      "jobTitle": "string",
      "department": "string",
      "relationshipType": "string"
    }
  ],
  "hierarchy": { org chart structure }
}
```

**FR-OH-003: Update Team Member**

**API Endpoint:**
```
PUT /api/v1/profiles/{orgProfileId}/team-members/{relationshipId}
Request Body: { updated relationship fields }
```

**FR-OH-004: Remove Team Member**

**API Endpoint:**
```
DELETE /api/v1/profiles/{orgProfileId}/team-members/{relationshipId}
```

---

## 8. Module 5: Catalog System

### 8.1 Catalog Management

**FR-CS-001: Enable Catalog**

**API Endpoint:**
```
POST /api/v1/profiles/{profileId}/catalog
Headers: Authorization: Bearer {token}
Request Body: {
  "catalogType": "Menu|Products|Services",
  "name": "string",
  "description": "string",
  "currency": "ZAR",
  "isActive": true
}
Response: {
  "success": true,
  "catalog": {
    "id": integer,
    "profileId": integer,
    "catalogType": "string",
    ...catalog data
  }
}
```

**FR-CS-002: Category Management**

**API Endpoints:**
```
POST /api/v1/catalogs/{catalogId}/categories
GET /api/v1/catalogs/{catalogId}/categories
PUT /api/v1/catalogs/{catalogId}/categories/{categoryId}
DELETE /api/v1/catalogs/{catalogId}/categories/{categoryId}

Request/Response structure:
{
  "id": integer,
  "name": "string",
  "description": "string",
  "displayOrder": integer,
  "isActive": boolean,
  "parentCategoryId": integer (optional, for sub-categories),
  "iconUrl": "string"
}
```

**FR-CS-003: Item Management**

**API Endpoints:**
```
POST /api/v1/catalogs/{catalogId}/items
GET /api/v1/catalogs/{catalogId}/items
PUT /api/v1/catalogs/{catalogId}/items/{itemId}
DELETE /api/v1/catalogs/{catalogId}/items/{itemId}

Request/Response structure:
{
  "id": integer,
  "categoryId": integer,
  "name": "string",
  "description": "string",
  "price": decimal,
  "compareAtPrice": decimal,
  "sku": "string",
  "isAvailable": boolean,
  "images": [ array of image URLs ],
  "allergens": [ array of strings ],
  "nutritionalInfo": { object },
  "tags": [ array of strings ],
  "displayOrder": integer
}
```

**FR-CS-004: Variants**

**API Endpoints:**
```
POST /api/v1/catalog-items/{itemId}/variants
GET /api/v1/catalog-items/{itemId}/variants
PUT /api/v1/catalog-items/{itemId}/variants/{variantId}
DELETE /api/v1/catalog-items/{itemId}/variants/{variantId}

Structure:
{
  "id": integer,
  "itemId": integer,
  "name": "string (e.g., Small, Medium, Large)",
  "price": decimal,
  "sku": "string",
  "isDefault": boolean,
  "isAvailable": boolean
}
```

**FR-CS-005: Add-ons**

**API Endpoints:**
```
POST /api/v1/catalog-items/{itemId}/addons
GET /api/v1/catalog-items/{itemId}/addons
PUT /api/v1/catalog-items/{itemId}/addons/{addonId}
DELETE /api/v1/catalog-items/{itemId}/addons/{addonId}

Structure:
{
  "id": integer,
  "itemId": integer,
  "name": "string",
  "price": decimal,
  "isDefault": boolean
}
```

**FR-CS-006: Bulk Operations**

**API Endpoints:**
```
POST /api/v1/catalogs/{catalogId}/items/bulk-import
Content-Type: multipart/form-data
Request Body: CSV file

POST /api/v1/catalogs/{catalogId}/items/bulk-update
Request Body: { "items": [ array of item updates ] }

DELETE /api/v1/catalogs/{catalogId}/items/bulk-delete
Request Body: { "itemIds": [ array of integers ] }
```

---

## 9. Module 6: Document Management

### 9.1 Document Upload

**FR-DM-001: Upload Documents**

**API Endpoint:**
```
POST /api/v1/profiles/{profileId}/documents
Headers: Authorization: Bearer {token}
Content-Type: multipart/form-data
Request Body: FormData with file

Response: {
  "success": true,
  "document": {
    "id": integer,
    "profileId": integer,
    "fileName": "string",
    "fileType": "PDF|DOCX|XLSX|PPTX",
    "fileSize": integer,
    "url": "string",
    "downloadUrl": "string",
    "uploadedAt": "datetime"
  }
}
```

**Acceptance Criteria:**
- File type validation (PDF, DOCX, XLSX, PPTX)
- Max file size: 10MB
- Virus scanning
- Tier-based limits enforced
- CDN storage
- Integer-based document IDs

**FR-DM-002: Document Management**

**API Endpoints:**
```
GET /api/v1/profiles/{profileId}/documents
Response: [ array of documents ]

DELETE /api/v1/profiles/{profileId}/documents/{documentId}

PUT /api/v1/profiles/{profileId}/documents/{documentId}
Request Body: {
  "displayName": "string",
  "description": "string",
  "isPublic": boolean
}
```

---

## 10. Module 7: NFC Integration

### 10.1 NFC Tag Management

**FR-NFC-001: Register NFC Tag**

**API Endpoint:**
```
POST /api/v1/profiles/{profileId}/nfc-tags
Headers: Authorization: Bearer {token}
Request Body: {
  "tagId": "string (NFC UID)",
  "nickname": "string",
  "productType": "Card|Sticker|Keychain|Custom"
}
Response: {
  "success": true,
  "nfcTag": {
    "id": integer,
    "tagId": "string",
    "profileId": integer,
    "activatedAt": "datetime",
    "qrCodeUrl": "string"
  }
}
```

**FR-NFC-002: QR Code Generation**

**API Endpoint:**
```
POST /api/v1/profiles/{profileId}/qr-codes
Request Body: {
  "destinationUrl": "string (optional, defaults to profile URL)",
  "size": integer,
  "format": "PNG|SVG"
}
Response: {
  "success": true,
  "qrCode": {
    "id": integer,
    "imageUrl": "string",
    "downloadUrl": "string"
  }
}
```

**FR-NFC-003: NFC Scan Tracking**

**API Endpoint:**
```
POST /api/v1/nfc-tags/{tagId}/scan
Request Body: {
  "location": { "latitude": decimal, "longitude": decimal },
  "deviceType": "string",
  "browser": "string"
}
Response: {
  "redirectUrl": "string (profile URL)"
}
```

**FR-NFC-004: NFC Product Orders**

**API Endpoints:**
```
POST /api/v1/nfc-products/order
Request Body: {
  "productType": "Card|Sticker|Keychain|Custom",
  "quantity": integer,
  "customization": { object },
  "shippingAddress": { object }
}

GET /api/v1/nfc-products/orders
GET /api/v1/nfc-products/orders/{orderId}
```

---

## 11. Module 8: Analytics & Reporting

### 11.1 Analytics Tracking

**FR-AN-001: Track Profile Views**

**API Endpoint:**
```
POST /api/v1/analytics/track
Request Body: {
  "profileId": integer,
  "eventType": "view|click|download|share",
  "referrer": "string",
  "userAgent": "string",
  "ipAddress": "string",
  "location": { "country": "string", "city": "string" }
}
Response: { "success": true }
```

**FR-AN-002: View Analytics Dashboard**

**API Endpoint:**
```
GET /api/v1/profiles/{profileId}/analytics
Headers: Authorization: Bearer {token}
Query Parameters:
  ?startDate=YYYY-MM-DD
  &endDate=YYYY-MM-DD
  &groupBy=day|week|month

Response: {
  "summary": {
    "totalViews": integer,
    "uniqueVisitors": integer,
    "averageViewDuration": integer,
    "topReferrers": [ array ],
    "deviceBreakdown": { object },
    "geographicData": [ array ]
  },
  "timeSeries": [
    {
      "date": "YYYY-MM-DD",
      "views": integer,
      "uniqueVisitors": integer
    }
  ]
}
```

**FR-AN-003: Catalog Analytics**

**API Endpoint:**
```
GET /api/v1/catalogs/{catalogId}/analytics
Headers: Authorization: Bearer {token}
Response: {
  "totalCatalogViews": integer,
  "totalItemViews": integer,
  "topItems": [
    {
      "itemId": integer,
      "itemName": "string",
      "views": integer,
      "uniqueViewers": integer
    }
  ],
  "categoryPerformance": [ array ]
}
```

**FR-AN-004: Export Analytics**

**API Endpoint:**
```
GET /api/v1/profiles/{profileId}/analytics/export
Query Parameters: ?format=csv|xlsx|pdf
Response: File download
```

---

## 12. Module 9: Subscription Management

### 12.1 Subscription Tiers

**FR-SM-001: View Subscription Tiers**

**API Endpoint:**
```
GET /api/v1/subscription-tiers
Response: [
  {
    "id": integer,
    "name": "Free|Basic|Professional|Enterprise",
    "price": decimal,
    "billingCycle": "Monthly|Annual",
    "features": {
      "maxProfiles": integer,
      "maxCatalogItems": integer,
      "maxDocuments": integer,
      "customBranding": boolean,
      "analytics": boolean,
      "apiAccess": boolean,
      "prioritySupport": boolean
    },
    "limits": { object }
  }
]
```

**FR-SM-002: Subscribe to Tier**

**API Endpoint:**
```
POST /api/v1/subscriptions/subscribe
Headers: Authorization: Bearer {token}
Request Body: {
  "tierId": integer,
  "billingCycle": "Monthly|Annual",
  "paymentMethodId": "string"
}
Response: {
  "success": true,
  "subscription": {
    "id": integer,
    "userId": integer,
    "tierId": integer,
    "status": "Active",
    "startDate": "datetime",
    "nextBillingDate": "datetime",
    "amount": decimal
  },
  "paymentUrl": "string (redirect to payment gateway)"
}
```

**FR-SM-003: Upgrade/Downgrade Subscription**

**API Endpoint:**
```
PUT /api/v1/subscriptions/{subscriptionId}/change-tier
Request Body: {
  "newTierId": integer,
  "effectiveDate": "immediate|nextBillingCycle"
}
Response: {
  "success": true,
  "prorationAmount": decimal,
  "effectiveDate": "datetime"
}
```

**FR-SM-004: Cancel Subscription**

**API Endpoint:**
```
POST /api/v1/subscriptions/{subscriptionId}/cancel
Request Body: {
  "cancellationReason": "string",
  "effectiveDate": "immediate|endOfBillingCycle"
}
Response: {
  "success": true,
  "cancellationDate": "datetime",
  "accessExpiryDate": "datetime"
}
```

**FR-SM-005: View Subscription Status**

**API Endpoint:**
```
GET /api/v1/users/{userId}/subscription
Response: {
  "subscription": {
    "id": integer,
    "tier": { tier details },
    "status": "Active|Cancelled|Suspended|PastDue",
    "currentPeriodEnd": "datetime",
    "usage": {
      "profilesUsed": integer,
      "profilesAllowed": integer,
      "catalogItemsUsed": integer,
      "catalogItemsAllowed": integer
    }
  }
}
```

---

## 13. Module 10: Payment Processing

### 13.1 Payment Integration

**FR-PM-001: Process Payment**

**API Endpoint:**
```
POST /api/v1/payments/initiate
Headers: Authorization: Bearer {token}
Request Body: {
  "subscriptionId": integer,
  "amount": decimal,
  "currency": "ZAR",
  "paymentMethod": "PayFast",
  "returnUrl": "string",
  "cancelUrl": "string"
}
Response: {
  "success": true,
  "transactionId": integer,
  "paymentGatewayUrl": "string",
  "paymentReference": "string"
}
```

**FR-PM-002: Payment Webhook (ITN)**

**API Endpoint:**
```
POST /api/v1/payments/webhook/payfast
Request Body: { PayFast ITN data }
Response: { "success": true }
```

**Acceptance Criteria:**
- Verify payment signature
- Update subscription status
- Record transaction with integer ID
- Send confirmation email
- Generate invoice

**FR-PM-003: Payment History**

**API Endpoint:**
```
GET /api/v1/users/{userId}/payments
Response: {
  "payments": [
    {
      "id": integer,
      "transactionId": integer,
      "amount": decimal,
      "currency": "ZAR",
      "status": "Completed|Failed|Pending|Refunded",
      "date": "datetime",
      "invoiceUrl": "string"
    }
  ]
}
```

**FR-PM-004: Download Invoice**

**API Endpoint:**
```
GET /api/v1/payments/{transactionId}/invoice
Response: PDF file download
```

---

## 14. Integration Requirements

### 14.1 Third-Party Integrations

**FR-INT-001: Payment Gateway (PayFast)**
- South African payment processing
- Credit card and EFT support
- ITN (Instant Transaction Notification) handling
- Subscription billing support
- Sandbox and production modes

**FR-INT-002: Email Service**
- Transactional emails (verification, password reset)
- Notification emails (subscription, payments)
- Marketing emails (optional)
- Email templates
- Delivery tracking

**FR-INT-003: File Storage & CDN**
- AWS S3 or Azure Blob Storage
- CloudFlare CDN integration
- Image optimization
- Secure file access
- Backup and redundancy

**FR-INT-004: Maps API**
- Google Maps integration for location
- Address autocomplete
- Geocoding (address to coordinates)
- Map display on profiles
- Directions and navigation

**FR-INT-005: Social Media Integration**
- Facebook review import (optional)
- Google My Business integration (optional)
- Instagram feed display (optional)
- Social sharing functionality

---

## 15. Non-Functional Requirements

### 15.1 Performance

**NFR-PERF-001: API Response Time**
- Average response time < 200ms
- 95th percentile < 500ms
- Database queries optimized with indexes
- API caching with Redis
- CDN for static assets

**NFR-PERF-002: Frontend Performance**
- Initial page load < 2 seconds
- Lazy loading of images and components
- Code splitting in Vue.js
- Optimized bundle sizes
- Progressive Web App (PWA) capabilities

**NFR-PERF-003: Scalability**
- Support 10,000+ concurrent users
- Horizontal scaling capability
- Database read replicas
- Load balancing
- Auto-scaling based on traffic

**NFR-PERF-004: Mobile Performance**
- Mobile-first responsive design
- Touch-optimized interactions
- Reduced data usage on mobile
- Offline capability (PWA)

---

### 15.2 Security

**NFR-SEC-001: Authentication & Authorization**
- JWT-based authentication
- Token refresh mechanism
- Role-based access control (RBAC)
- API rate limiting per user/IP
- Protection against brute force attacks

**NFR-SEC-002: Data Protection**
- HTTPS only (TLS 1.2+)
- Data encryption at rest
- Password hashing (bcrypt)
- SQL injection prevention (parameterized queries)
- XSS prevention
- CSRF protection

**NFR-SEC-003: API Security**
- CORS configuration
- Input validation and sanitization
- Output encoding
- Secure headers
- API versioning
- Future: OAuth 2.0 for public API

**NFR-SEC-004: Privacy Compliance**
- POPIA compliance (South Africa)
- GDPR compliance (Europe)
- User data export capability
- Right to deletion
- Cookie consent
- Privacy policy and terms of service

---

### 15.3 Availability

**NFR-AVAIL-001: Uptime**
- Target: 99.9% uptime
- Maximum planned downtime: 4 hours/month
- Automated monitoring and alerts
- Health check endpoints
- Graceful degradation

**NFR-AVAIL-002: Backup & Recovery**
- Daily automated database backups
- Point-in-time recovery capability
- Backup retention: 30 days
- Disaster recovery plan
- Regular backup testing

---

### 15.4 Usability

**NFR-USE-001: User Experience**
- Intuitive navigation
- Consistent UI/UX across platform
- Mobile-responsive design (Tailwind CSS)
- Accessibility compliance (WCAG 2.1 Level AA)
- Multi-language support (future)

**NFR-USE-002: API Documentation**
- Swagger/OpenAPI documentation
- Interactive API explorer
- Code examples in multiple languages
- Postman collection
- Developer guides

---

### 15.5 Maintainability

**NFR-MAIN-001: Code Quality**
- Clean code principles
- SOLID design principles
- Code comments and documentation
- Unit test coverage > 80%
- Integration test coverage
- API contract testing

**NFR-MAIN-002: Monitoring & Logging**
- Application logging (Serilog)
- Error tracking and alerting
- Performance monitoring
- API usage analytics
- Audit trails for sensitive operations

---

## 16. Data Requirements

### 16.1 Database Design

**DR-001: Code-First Approach**
- Entity Framework Core migrations
- Integer-based primary keys (not GUIDs)
- Foreign key relationships with integer IDs
- Indexes on frequently queried fields
- Soft deletes (IsDeleted flag) where appropriate

**DR-002: Key Entities with Integer IDs**
```
Users (Id: int)
Profiles (Id: int, UserId: int)
ProfileBranding (Id: int, ProfileId: int)
ProfileRelationships (Id: int, ParentProfileId: int, ChildProfileId: int)
Catalogs (Id: int, ProfileId: int)
CatalogCategories (Id: int, CatalogId: int)
CatalogItems (Id: int, CategoryId: int)
CatalogItemVariants (Id: int, ItemId: int)
NFCTags (Id: int, ProfileId: int)
Subscriptions (Id: int, UserId: int, TierId: int)
Payments (Id: int, SubscriptionId: int, UserId: int)
Analytics (Id: int, ProfileId: int)
Documents (Id: int, ProfileId: int)
```

**DR-003: Data Retention**
- Active data: Indefinite
- Deleted user data: 30 days grace period
- Audit logs: 2 years
- Analytics data: 2 years
- Backup retention: 30 days

**DR-004: Data Validation**
- Server-side validation required
- Client-side validation for UX
- Database constraints
- Data type enforcement
- Foreign key constraints

---

## 17. Security Requirements

### 17.1 Authentication

**SR-AUTH-001: JWT Implementation**
- HS256 algorithm for token signing
- Access token expiry: 1 hour
- Refresh token expiry: 7 days
- Token revocation capability
- Secure token storage (httpOnly cookies preferred)

**SR-AUTH-002: Password Policy**
- Minimum 8 characters
- At least one uppercase letter
- At least one lowercase letter
- At least one number
- Optional: Special character requirement
- Password history (prevent reuse of last 5 passwords)

**SR-AUTH-003: Account Security**
- Failed login attempt tracking
- Account lockout after 5 failed attempts (15 minutes)
- Email verification required
- Password reset token expiry: 24 hours
- Session management

---

### 17.2 API Security

**SR-API-001: Rate Limiting**
- Anonymous: 60 requests/minute
- Free tier: 100 requests/minute
- Paid tiers: 500-1000 requests/minute
- Enterprise: Custom limits
- 429 Too Many Requests response

**SR-API-002: Input Validation**
- Whitelist validation
- Maximum length checks
- Data type validation
- SQL injection prevention
- Command injection prevention

**SR-API-003: CORS Configuration**
- Whitelist allowed origins
- Restrict HTTP methods
- Credential support configuration
- Preflight request handling

---

## 18. User Stories & Use Cases

### 18.1 Core User Stories

**User Story 1: Registration**
> As a new user, I want to register for an account so that I can create digital profiles.

**Acceptance Criteria:**
- Fill registration form in Vue.js frontend
- API validates and creates user account
- Receive verification email
- Click verification link
- Receive JWT token and access dashboard

---

**User Story 2: Create Digital Business Card**
> As a business professional, I want to create a digital business card so that I can share my contact information without physical cards.

**Acceptance Criteria:**
- Click "Create Profile" button
- Select "Personal Profile" type
- Fill in contact information
- Choose template via API
- Customize branding colors
- Upload profile photo
- Set profile to public
- Receive shareable URL and QR code

---

**User Story 3: Create Restaurant Menu**
> As a restaurant owner, I want to create a digital menu so that customers can view it via QR code.

**Acceptance Criteria:**
- Create business profile via API
- Enable catalog module
- Create menu categories (Appetizers, Mains, Desserts, Drinks)
- Add menu items with descriptions, prices, images
- Upload high-quality food photos
- Add allergen information
- Generate QR code for table placement
- Share catalog URL

---

**User Story 4: Manage Organization Team**
> As an organization admin, I want to add team members to my organization profile so that we have a unified company presence.

**Acceptance Criteria:**
- Create organization profile
- Invite team members via email through API
- Team members create their profiles
- Link team member profiles to organization
- Assign job titles and departments
- Display team directory on organization profile
- Each team member maintains their own profile

---

**User Story 5: Subscribe to Paid Tier**
> As a free user, I want to upgrade to a paid subscription so that I can access premium features.

**Acceptance Criteria:**
- View pricing plans via API
- Compare features and limits
- Click "Upgrade" button
- Select billing cycle (monthly/annual)
- Redirect to PayFast payment page
- Complete payment
- Receive confirmation email
- Access premium features immediately
- View invoice in payment history

---

**User Story 6: Track Profile Analytics**
> As a profile owner, I want to view analytics so that I can understand engagement with my profile.

**Acceptance Criteria:**
- Access analytics dashboard
- View total profile views
- See unique visitor count
- View traffic sources
- See geographic breakdown
- View device types (mobile/desktop)
- Export analytics as CSV via API
- View time-series graphs

---

**User Story 7: Share NFC Business Card**
> As a networker, I want to share my digital business card via NFC so that recipients can save my contact instantly.

**Acceptance Criteria:**
- Order NFC card through platform
- Register NFC tag with profile via API
- Tap NFC card to recipient's phone
- Profile opens in recipient's browser
- Recipient can download vCard to contacts
- Track NFC scan in analytics

---

**User Story 8: Customize Profile Branding**
> As a business owner, I want to customize my profile colors to match my brand so that it looks professional.

**Acceptance Criteria:**
- Access branding settings via API
- Select primary color (color picker)
- Select secondary color
- Choose font family
- Upload logo
- Preview changes in real-time (Vue.js)
- Save customizations
- View updated public profile

---

**User Story 9: Network at Event**
> As an attendee at a networking event, I want to receive someone's digital business card so that I don't have to carry paper cards.

**Acceptance Criteria:**
- Scan person's NFC card or QR code
- Profile opens in browser
- View contact information
- Download vCard to phone contacts
- View their social media links
- Save profile for later reference

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
**Measurement:** Analytics data via API

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

**Metric:** API Response Time  
**Target:** Average < 200ms  
**Measurement:** API monitoring tools

**Metric:** Frontend Load Time  
**Target:** < 2 seconds  
**Measurement:** Lighthouse / PageSpeed Insights

**Metric:** Uptime  
**Target:** 99.9%  
**Measurement:** Uptime monitoring service

**Metric:** Error Rate  
**Target:** < 1%  
**Measurement:** Error logging and monitoring

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
7. Modern browsers support (Chrome, Firefox, Safari, Edge - last 2 versions)
8. Users will accept JWT tokens in cookies or localStorage

### 20.2 Constraints

1. **Budget:** Initial development budget constraints
2. **Timeline:** MVP delivery target: 16 weeks
3. **Team Size:** Small development team (1-3 developers)
4. **Technology:** .NET 6, Vue.js 3, MySQL 8.0+
5. **Payment Gateway:** Limited to PayFast (South African gateway)
6. **Legal:** POPIA compliance mandatory (South African privacy law)
7. **API:** Not publicly exposing API initially (designing for future)

### 20.3 Dependencies

1. **PayFast:** Payment processing dependent on third-party
2. **Cloud Storage:** AWS/Azure availability and pricing
3. **Email Service:** Third-party email delivery (SendGrid/Mailgun)
4. **Google Maps API:** Map features dependent on Google API
5. **Browser Support:** Modern ES6+ features required
6. **CDN:** CloudFlare or similar for asset delivery

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
| **JWT** | JSON Web Token - authentication token format |
| **API** | Application Programming Interface |
| **SPA** | Single Page Application (Vue.js frontend) |
| **REST** | Representational State Transfer (API architecture) |

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
| **JWT** | JSON Web Token |
| **JSON** | JavaScript Object Notation |
| **MRR** | Monthly Recurring Revenue |
| **NFC** | Near Field Communication |
| **NPS** | Net Promoter Score |
| **ORM** | Object-Relational Mapping |
| **PDF** | Portable Document Format |
| **POPIA** | Protection of Personal Information Act |
| **PWA** | Progressive Web Application |
| **QR** | Quick Response |
| **REST** | Representational State Transfer |
| **SaaS** | Software as a Service |
| **SEO** | Search Engine Optimization |
| **SKU** | Stock Keeping Unit |
| **SMS** | Short Message Service |
| **SPA** | Single Page Application |
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

**Core Tables (Integer IDs):**
1. Users (Id: int, Primary Key)
2. SubscriptionTiers (Id: int)
3. UserSubscriptions (Id: int, UserId: int, TierId: int)
4. Categories (Id: int) - Business categories
5. ProfileTemplates (Id: int)
6. Profiles (Id: int, UserId: int, TemplateId: int)
7. ProfileBranding (Id: int, ProfileId: int)
8. ProfileRelationships (Id: int, ParentProfileId: int, ChildProfileId: int)

**Catalog Tables:**
9. ProfileCatalogs (Id: int, ProfileId: int)
10. CatalogCategories (Id: int, CatalogId: int)
11. CatalogItems (Id: int, CategoryId: int)
12. CatalogItemVariants (Id: int, ItemId: int)
13. CatalogItemImages (Id: int, ItemId: int)
14. CatalogItemAddons (Id: int, ItemId: int)
15. CatalogAnalytics (Id: int, CatalogId: int)

**Media & Content Tables:**
16. SocialMediaLinks (Id: int, ProfileId: int)
17. ProfilePhotos (Id: int, ProfileId: int)
18. ProfileVideos (Id: int, ProfileId: int)
19. ProfileDocuments (Id: int, ProfileId: int)
20. ProfileQRCodes (Id: int, ProfileId: int)

**NFC Tables:**
21. NFCProducts (Id: int)
22. NFCOrders (Id: int, UserId: int, ProductId: int)
23. NFCTags (Id: int, ProfileId: int)

**Analytics Tables:**
24. ProfileAnalytics (Id: int, ProfileId: int)

**Services & Support:**
25. ProfileServices (Id: int, ProfileId: int)
26. ProfileCustomFields (Id: int, ProfileId: int)
27. ProfileReviews (Id: int, ProfileId: int)
28. SupportTickets (Id: int, UserId: int)
29. PaymentTransactions (Id: int, SubscriptionId: int, UserId: int)

### Appendix D: API URL Structure

**API Base URL:** `https://api.bizbio.co.za/api/v1`

**Authentication Endpoints:**
- POST `/auth/register`
- POST `/auth/login`
- POST `/auth/refresh`
- POST `/auth/verify-email`
- POST `/auth/forgot-password`
- POST `/auth/reset-password`

**Profile Endpoints:**
- GET `/profiles` (list user's profiles)
- POST `/profiles` (create profile)
- GET `/profiles/{id}` (get profile by ID)
- GET `/profiles/by-slug/{slug}` (public access)
- PUT `/profiles/{id}` (update profile)
- DELETE `/profiles/{id}` (delete profile)
- PUT `/profiles/{id}/branding`
- PUT `/profiles/{id}/layout`

**Catalog Endpoints:**
- POST `/catalogs` (enable catalog)
- GET `/catalogs/{id}` (get catalog)
- POST `/catalogs/{catalogId}/categories`
- POST `/catalogs/{catalogId}/items`
- GET `/catalog-items/{itemId}/variants`

**Organization Endpoints:**
- POST `/profiles/{orgId}/team-members`
- GET `/profiles/{orgId}/team-members`
- PUT `/profiles/{orgId}/team-members/{relationshipId}`
- DELETE `/profiles/{orgId}/team-members/{relationshipId}`

**Analytics Endpoints:**
- POST `/analytics/track`
- GET `/profiles/{id}/analytics`
- GET `/catalogs/{id}/analytics`
- GET `/profiles/{id}/analytics/export`

**Subscription Endpoints:**
- GET `/subscription-tiers`
- POST `/subscriptions/subscribe`
- GET `/users/{userId}/subscription`
- PUT `/subscriptions/{id}/change-tier`
- POST `/subscriptions/{id}/cancel`

**Payment Endpoints:**
- POST `/payments/initiate`
- POST `/payments/webhook/payfast`
- GET `/users/{userId}/payments`
- GET `/payments/{transactionId}/invoice`

**Public Frontend URLs:**
- Profile: `https://bizbio.co.za/{profile-slug}`
- Catalog: `https://bizbio.co.za/c/{profile-slug}`
- Category: `https://bizbio.co.za/c/{profile-slug}/category/{category-id}`

**Authenticated Frontend URLs:**
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
| Frontend Architect | | | |
| Backend Architect | | | |
| Stakeholder | | | |

---

**End of Document**

**Version:** 3.0  
**Date:** November 2025  
**Status:** Ready for Development  
**Next Review:** December 2025
