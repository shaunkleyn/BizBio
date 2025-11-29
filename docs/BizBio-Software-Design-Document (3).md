# BizBio Platform - Software Design Document (SDD)

**Document Version:** 1.0  
**Date:** November 2025  
**Project Name:** BizBio Digital Profile Platform  
**Project Type:** SaaS Platform  
**Status:** Design Approved  
**Author:** System Architect & Business Analyst

---

## Document Control

| Version | Date | Author | Changes |
|---------|------|--------|---------|
| 1.0 | 2025-11-08 | System Architect | Initial Software Design Document |

---

## Executive Summary

### Purpose of This Document
This Software Design Document (SDD) provides a comprehensive overview of the BizBio Digital Profile Platform, including its business purpose, architectural design, technical approach, and implementation strategy. This document serves as the primary reference for stakeholders, developers, and project managers to understand the complete system design.

### Document Audience
- **Business Stakeholders:** Understand business value and ROI
- **Project Managers:** Coordinate development phases and resources
- **Development Team:** Implement system components
- **Quality Assurance:** Validate against design specifications
- **Operations Team:** Plan deployment and maintenance

### Project Overview
BizBio is a modern SaaS platform that revolutionizes how businesses and professionals share contact information and showcase their offerings. The platform replaces traditional physical business cards with dynamic digital profiles accessible via NFC tags, QR codes, and web URLs. Additionally, it enables restaurants to publish digital menus and businesses to showcase product catalogs.

---

## Table of Contents

1. [Business Context](#1-business-context)
2. [Project Vision & Objectives](#2-project-vision--objectives)
3. [System Architecture Overview](#3-system-architecture-overview)
4. [Design Principles & Philosophy](#4-design-principles--philosophy)
5. [User Experience Design](#5-user-experience-design)
6. [Technical Architecture](#6-technical-architecture)
7. [Data Architecture](#7-data-architecture)
8. [Security Architecture](#8-security-architecture)
9. [Integration Architecture](#9-integration-architecture)
10. [Deployment Architecture](#10-deployment-architecture)
11. [Development Methodology](#11-development-methodology)
12. [Quality Assurance Strategy](#12-quality-assurance-strategy)
13. [Risk Management](#13-risk-management)
14. [Project Timeline & Phases](#14-project-timeline--phases)
15. [Success Metrics & KPIs](#15-success-metrics--kpis)
16. [Future Roadmap](#16-future-roadmap)
17. [Appendices](#17-appendices)

---

## 1. Business Context

### 1.1 Problem Statement

**Industry Challenge:**
Traditional business cards are:
- Easily lost or misplaced (88% never used after receiving)
- Environmentally wasteful (10 billion cards printed annually, 88% thrown away)
- Require manual data entry into contacts
- Cannot be updated after distribution
- Limited information capacity
- Expensive to reprint

**Restaurant-Specific Challenge:**
- Physical menus require frequent sanitization (post-COVID concern)
- Menu changes require reprinting costs
- Limited space for detailed item descriptions
- No ability to track customer preferences
- Difficult to manage seasonal menu items

**Business Directory Challenge:**
- Traditional company directories outdated quickly
- No easy way to showcase team hierarchies
- Limited ability to present organizational structure
- Difficult to maintain and update

### 1.2 Market Opportunity

**Target Market Size:**
- **South Africa SME Market:** 600,000+ small and medium businesses
- **Restaurant Market:** 13,000+ registered restaurants
- **Professional Services:** 250,000+ independent professionals
- **Global Expansion Potential:** Multi-billion dollar digital business card market

**Market Trends:**
- Shift to contactless information sharing (accelerated by COVID-19)
- Increased smartphone penetration (92% in South Africa)
- Growing preference for sustainability
- Digital-first business operations
- NFC technology adoption in smartphones

**Competitive Landscape:**
- Few South African-focused solutions
- International competitors lack local payment integration
- No comprehensive solution combining profiles, menus, and catalogs
- Opportunity for first-mover advantage with NFC integration

### 1.3 Business Model

**Revenue Streams:**
1. **Subscription Plans:**
   - Free Tier: Lead generation (limited features)
   - Basic Plan: R99/month (3 profiles, 100 catalog items)
   - Professional Plan: R299/month (10 profiles, 500 catalog items)
   - Enterprise Plan: R799/month (unlimited, API access)

2. **NFC Product Sales:**
   - Business cards: R150-250 per card
   - Table stands: R300-500 per stand
   - Bulk orders: Volume discounts

3. **Premium Services:**
   - Custom domain setup: R500 one-time
   - Profile design services: R1,500-5,000
   - Migration services: R2,000+
   - Training and onboarding: Custom pricing

4. **Future Revenue:**
   - API access for enterprises
   - White-label solutions
   - Affiliate commissions
   - Premium analytics

**Cost Structure:**
- Development and maintenance
- Hosting and infrastructure (AWS/Azure)
- Payment gateway fees (PayFast)
- Email service (SendGrid)
- Cloud storage (S3/Azure Blob)
- Customer support
- Marketing and sales

### 1.4 Value Proposition

**For Individual Professionals:**
- ✅ Always up-to-date contact information
- ✅ Professional online presence
- ✅ Easy sharing via QR code or NFC
- ✅ Trackable engagement analytics
- ✅ Eco-friendly alternative
- ✅ Cost-effective (vs. printing cards)

**For Businesses:**
- ✅ Centralized team directory
- ✅ Consistent branding across team
- ✅ Easy onboarding for new employees
- ✅ Organizational hierarchy visualization
- ✅ Analytics on team engagement
- ✅ Scalable solution

**For Restaurants:**
- ✅ Digital menu accessible via QR code
- ✅ Easy menu updates (no reprinting)
- ✅ Rich media support (photos, descriptions)
- ✅ Allergen and dietary information
- ✅ Multi-language support (future)
- ✅ Contactless and hygienic

**For Retailers/Service Providers:**
- ✅ Online product catalog
- ✅ Showcase service offerings
- ✅ Customer engagement tracking
- ✅ Integration-ready for e-commerce (future)

---

## 2. Project Vision & Objectives

### 2.1 Vision Statement

*"To be the leading digital business identity platform in Africa, empowering professionals and businesses to connect, engage, and grow through innovative digital solutions."*

### 2.2 Mission Statement

*"BizBio transforms how businesses and professionals share information by providing an intuitive, eco-friendly, and powerful digital platform that replaces traditional business cards, menus, and directories."*

### 2.3 Strategic Objectives

**Short-Term Objectives (6-12 months):**
1. Launch MVP with core features (profiles, catalogs, NFC integration)
2. Acquire 1,000 registered users
3. Convert 150 paying subscribers
4. Onboard 50 restaurants with active menus
5. Achieve R50,000 MRR (Monthly Recurring Revenue)
6. Establish brand presence in South Africa

**Medium-Term Objectives (12-24 months):**
1. Expand to 10,000 active users
2. Launch public API for third-party integrations
3. Introduce white-label solutions
4. Expand to 3 additional African markets
5. Achieve R500,000 MRR
6. Develop mobile applications (iOS/Android)

**Long-Term Objectives (24-36 months):**
1. Become market leader in South Africa
2. Expand to 10+ countries
3. Achieve 100,000+ active users
4. Introduce AI-powered features (recommendations, chatbots)
5. Explore acquisition or strategic partnerships
6. Achieve profitability and sustainable growth

### 2.4 Success Criteria

**Technical Success:**
- ✅ 99.9% uptime
- ✅ <200ms API response time
- ✅ <3 second page load time
- ✅ Zero critical security vulnerabilities
- ✅ Successful public API launch

**Business Success:**
- ✅ 15% free-to-paid conversion rate
- ✅ <10% monthly churn rate
- ✅ Net Promoter Score (NPS) >50
- ✅ Customer Lifetime Value (CLV) >R2,500
- ✅ Break-even by month 18

**User Success:**
- ✅ 500 average monthly profile views per user
- ✅ 80% mobile usage (responsive design working)
- ✅ 30% return visitor rate
- ✅ <24 hour support response time (Free/Basic)
- ✅ <4 hour support response time (Professional+)

---

## 3. System Architecture Overview

### 3.1 Architectural Style

**Decoupled Architecture:**
BizBio implements a modern, decoupled architecture separating frontend presentation from backend business logic. This approach provides:
- Independent scaling of frontend and backend
- Technology flexibility (can replace frontend/backend independently)
- Improved performance through CDN for static assets
- Better developer experience with clear separation
- Future-ready for mobile apps using same API

**Key Architectural Patterns:**
1. **Separation of Concerns (SoC)**
2. **RESTful API Design**
3. **Stateless Authentication (JWT)**
4. **Repository Pattern (Data Access)**
5. **Service Layer Pattern (Business Logic)**
6. **DTO Pattern (Data Transfer)**
7. **Dependency Injection**

### 3.2 High-Level System Architecture

```
┌────────────────────────────────────────────────────────────────┐
│                         CLIENT LAYER                            │
├────────────────────────────────────────────────────────────────┤
│                                                                 │
│  ┌──────────────────┐  ┌──────────────────┐  ┌──────────────┐ │
│  │   Web Browser    │  │   Mobile App     │  │  Third-Party │ │
│  │   (Vue.js SPA)   │  │   (Future)       │  │  API Clients │ │
│  └──────────────────┘  └──────────────────┘  └──────────────┘ │
│                                                                 │
└────────────────────────────────────────────────────────────────┘
                              ↕
                         HTTPS/TLS
                              ↕
┌────────────────────────────────────────────────────────────────┐
│                      PRESENTATION LAYER                         │
├────────────────────────────────────────────────────────────────┤
│                                                                 │
│  ┌──────────────────────────────────────────────────────────┐  │
│  │              CDN (Static Assets)                         │  │
│  │  Vue.js Components, CSS, Images, Fonts                   │  │
│  └──────────────────────────────────────────────────────────┘  │
│                                                                 │
└────────────────────────────────────────────────────────────────┘
                              ↕
                         REST API
                              ↕
┌────────────────────────────────────────────────────────────────┐
│                        API GATEWAY                              │
├────────────────────────────────────────────────────────────────┤
│  Rate Limiting │ Authentication │ Logging │ CORS │ Validation  │
└────────────────────────────────────────────────────────────────┘
                              ↕
┌────────────────────────────────────────────────────────────────┐
│                     APPLICATION LAYER                           │
├────────────────────────────────────────────────────────────────┤
│                                                                 │
│  ┌──────────────────────────────────────────────────────────┐  │
│  │           ASP.NET 6 Web API Controllers                  │  │
│  │  Auth │ Profiles │ Catalogs │ Analytics │ Subscriptions │  │
│  └──────────────────────────────────────────────────────────┘  │
│                              ↕                                  │
│  ┌──────────────────────────────────────────────────────────┐  │
│  │              Business Logic Services                     │  │
│  │  Profile Service │ Catalog Service │ Payment Service    │  │
│  └──────────────────────────────────────────────────────────┘  │
│                              ↕                                  │
│  ┌──────────────────────────────────────────────────────────┐  │
│  │                Data Access Layer                         │  │
│  │  Repositories │ EF Core Context │ Query Builders        │  │
│  └──────────────────────────────────────────────────────────┘  │
│                                                                 │
└────────────────────────────────────────────────────────────────┘
                              ↕
┌────────────────────────────────────────────────────────────────┐
│                        DATA LAYER                               │
├────────────────────────────────────────────────────────────────┤
│                                                                 │
│  ┌──────────────┐  ┌──────────────┐  ┌──────────────────────┐ │
│  │    MySQL     │  │    Redis     │  │   Blob Storage       │ │
│  │   Database   │  │    Cache     │  │  (Images/Files)      │ │
│  └──────────────┘  └──────────────┘  └──────────────────────┘ │
│                                                                 │
└────────────────────────────────────────────────────────────────┘
                              ↕
┌────────────────────────────────────────────────────────────────┐
│                    EXTERNAL SERVICES                            │
├────────────────────────────────────────────────────────────────┤
│  PayFast │ SendGrid │ Google Maps │ Analytics │ Monitoring    │
└────────────────────────────────────────────────────────────────┘
```

### 3.3 Component Architecture

**Frontend Components (Vue.js):**
```
Vue.js Application
│
├── Core Components
│   ├── Authentication (Login, Register, Reset Password)
│   ├── Layout (Header, Footer, Navigation, Sidebar)
│   └── Common (Loading, Error, Success, Modal)
│
├── Public Views
│   ├── Profile View (Public profile display)
│   ├── Catalog View (Public menu/product view)
│   ├── Landing Page (Marketing)
│   └── Search Results
│
├── Dashboard Views
│   ├── User Dashboard (Overview, stats)
│   ├── Profile Manager (CRUD profiles)
│   ├── Catalog Manager (CRUD catalogs/items)
│   ├── Team Manager (Organizational hierarchy)
│   ├── Analytics Dashboard
│   ├── Subscription Manager
│   └── Account Settings
│
├── Admin Portal
│   ├── User Management
│   ├── System Analytics
│   ├── Support Tickets
│   └── Configuration
│
└── Shared
    ├── State Management (Pinia stores)
    ├── API Services (Axios clients)
    ├── Utilities (Helpers, formatters)
    └── Composables (Reusable logic)
```

**Backend Components (ASP.NET):**
```
ASP.NET 6 Web API
│
├── API Controllers
│   ├── AuthController (Register, Login, Token refresh)
│   ├── ProfilesController (CRUD, publish, search)
│   ├── CatalogsController (Catalog management)
│   ├── ItemsController (Catalog items)
│   ├── AnalyticsController (Views, stats)
│   ├── SubscriptionsController (Plans, billing)
│   ├── PaymentsController (PayFast integration)
│   ├── DocumentsController (File uploads)
│   └── AdminController (System management)
│
├── Services (Business Logic)
│   ├── AuthService (JWT, password hashing)
│   ├── ProfileService (Profile business logic)
│   ├── CatalogService (Catalog business logic)
│   ├── SubscriptionService (Tier management)
│   ├── PaymentService (PayFast processing)
│   ├── EmailService (SendGrid integration)
│   ├── FileStorageService (Cloud storage)
│   ├── AnalyticsService (Tracking, reporting)
│   └── QRCodeService (QR generation)
│
├── Repositories (Data Access)
│   ├── UserRepository
│   ├── ProfileRepository
│   ├── CatalogRepository
│   ├── SubscriptionRepository
│   └── Generic Repository Pattern
│
├── Data (EF Core)
│   ├── ApplicationDbContext
│   ├── Entity Models (POCOs)
│   ├── Migrations
│   └── Seed Data
│
├── DTOs (Data Transfer Objects)
│   ├── Request DTOs
│   ├── Response DTOs
│   └── Mapping Profiles (AutoMapper)
│
├── Middleware
│   ├── JWT Authentication
│   ├── Exception Handling
│   ├── Request Logging
│   └── Rate Limiting
│
└── Utilities
    ├── Validators (FluentValidation)
    ├── Constants
    ├── Enums
    └── Extensions
```

### 3.4 Data Flow Architecture

**User Authentication Flow:**
```
1. User submits credentials (Vue.js)
   ↓
2. POST /api/v1/auth/login (API)
   ↓
3. AuthService validates credentials
   ↓
4. UserRepository queries database
   ↓
5. Password verified with BCrypt
   ↓
6. JWT token generated
   ↓
7. Token returned to client
   ↓
8. Token stored in localStorage/sessionStorage
   ↓
9. Subsequent requests include JWT in header
   ↓
10. JWT Middleware validates token
    ↓
11. User context injected into request
    ↓
12. Controller action executes with user context
```

**Profile Creation Flow:**
```
1. User fills profile form (Vue.js)
   ↓
2. POST /api/v1/profiles (with JWT)
   ↓
3. JWT Middleware validates token
   ↓
4. ProfilesController receives request
   ↓
5. FluentValidation validates DTO
   ↓
6. ProfileService checks tier limits
   ↓
7. Slug uniqueness validated
   ↓
8. ProfileRepository creates entity
   ↓
9. EF Core saves to MySQL
   ↓
10. AutoMapper maps entity to DTO
    ↓
11. Response returned to client
    ↓
12. Pinia store updated
    ↓
13. Vue component re-renders
```

**Public Profile View Flow:**
```
1. User/Guest accesses profile URL
   ↓
2. Vue Router loads PublicProfile component
   ↓
3. GET /api/v1/profiles/slug/{slug}
   ↓
4. No JWT required (public endpoint)
   ↓
5. ProfilesController fetches profile
   ↓
6. ProfileService checks if published
   ↓
7. AnalyticsService increments view count (async)
   ↓
8. Profile data returned
   ↓
9. Vue component renders profile
   ↓
10. CDN serves images/assets
```

---

## 4. Design Principles & Philosophy

### 4.1 Core Design Principles

**1. Mobile-First Design**
- Principle: Design for mobile screens first, then scale up
- Rationale: 80%+ of traffic expected from mobile devices
- Implementation: Tailwind CSS breakpoints, touch-friendly UI
- Example: Profile cards stack vertically on mobile, grid on desktop

**2. Progressive Enhancement**
- Principle: Core functionality works without JavaScript, enhanced with JS
- Rationale: Accessibility, SEO, graceful degradation
- Implementation: Server-side rendering for public profiles, SPA for dashboard
- Example: Public profiles viewable with JS disabled (future SSR)

**3. API-First Development**
- Principle: All features accessible via API
- Rationale: Future mobile apps, third-party integrations
- Implementation: RESTful API, comprehensive documentation
- Example: Every UI action maps to API endpoint

**4. Security by Design**
- Principle: Security considerations in every architectural decision
- Rationale: Protect user data, comply with POPIA/GDPR
- Implementation: JWT auth, encrypted passwords, input validation
- Example: All sensitive data encrypted at rest and in transit

**5. Scalability from Day One**
- Principle: Architecture supports horizontal scaling
- Rationale: Prepare for growth without major refactoring
- Implementation: Stateless API, database optimization, caching
- Example: API instances can be added behind load balancer

**6. Developer Experience (DX)**
- Principle: Code should be easy to understand and maintain
- Rationale: Faster development, easier onboarding
- Implementation: Clean architecture, consistent patterns, documentation
- Example: Repository pattern, service layer, clear naming conventions

**7. User Experience (UX) Excellence**
- Principle: Intuitive, fast, and delightful user experience
- Rationale: User satisfaction drives retention
- Implementation: Modern UI, fast interactions, helpful feedback
- Example: Instant validation, optimistic UI updates, loading states

### 4.2 Architectural Principles

**Separation of Concerns (SoC):**
- Frontend handles presentation and user interaction
- Backend handles business logic and data management
- Database handles data persistence
- External services handle specialized functions

**Don't Repeat Yourself (DRY):**
- Reusable components (Vue.js)
- Shared services and utilities
- Generic repository pattern
- Common middleware

**SOLID Principles:**
- **S**ingle Responsibility: Each class has one job
- **O**pen/Closed: Open for extension, closed for modification
- **L**iskov Substitution: Derived classes usable as base classes
- **I**nterface Segregation: Many specific interfaces vs. one general
- **D**ependency Inversion: Depend on abstractions, not concretions

**RESTful API Design:**
- Resource-based URLs (`/api/v1/profiles`, not `/api/v1/getProfiles`)
- HTTP methods for actions (GET, POST, PUT, DELETE)
- Stateless authentication (JWT)
- Consistent response formats
- Proper HTTP status codes

### 4.3 Coding Standards

**C# Backend Standards:**
- PascalCase for classes, methods, properties
- camelCase for variables, parameters
- Async/await for I/O operations
- Dependency injection for all services
- Null-checking and exception handling
- XML documentation comments
- Unit tests for business logic

**JavaScript/Vue.js Standards:**
- Composition API (not Options API)
- camelCase for variables, functions
- PascalCase for components
- Props validation and typing
- Composables for reusable logic
- Tailwind utility classes (no custom CSS unless necessary)
- Component documentation

**Database Standards:**
- PascalCase for table names (singular)
- PascalCase for column names
- Meaningful foreign key names
- Indexes on frequently queried columns
- Database migrations for all schema changes
- Never edit migrations after deployment

---

## 5. User Experience Design

### 5.1 Design System

**Color Palette:**
```
Primary Colors:
- Brand Blue: #0066CC (buttons, links, primary actions)
- Brand Dark: #003366 (headers, emphasis)
- Brand Light: #E6F2FF (backgrounds, highlights)

Secondary Colors:
- Success Green: #10B981 (success messages, positive actions)
- Warning Orange: #F59E0B (warnings, alerts)
- Error Red: #EF4444 (errors, destructive actions)
- Info Blue: #3B82F6 (information, notifications)

Neutral Colors:
- Text Dark: #1F2937 (primary text)
- Text Medium: #6B7280 (secondary text)
- Text Light: #9CA3AF (disabled text, placeholders)
- Background: #F9FAFB (page backgrounds)
- White: #FFFFFF (cards, containers)
- Border: #E5E7EB (dividers, borders)
```

**Typography:**
```
Font Family: Inter (primary), system fonts (fallback)

Heading Scale:
- H1: 2.5rem (40px) - font-bold
- H2: 2rem (32px) - font-semibold
- H3: 1.5rem (24px) - font-semibold
- H4: 1.25rem (20px) - font-medium
- H5: 1.125rem (18px) - font-medium
- H6: 1rem (16px) - font-medium

Body Text:
- Large: 1.125rem (18px)
- Base: 1rem (16px)
- Small: 0.875rem (14px)
- Extra Small: 0.75rem (12px)
```

**Spacing System (Tailwind):**
```
4px  (1)  - Tight spacing
8px  (2)  - Small spacing
12px (3)  - Medium-small spacing
16px (4)  - Medium spacing
24px (6)  - Large spacing
32px (8)  - Extra large spacing
48px (12) - Section spacing
64px (16) - Component spacing
```

**Iconography:**
- Library: Heroicons (Tailwind's recommended icon set)
- Style: Outline for light touch, Solid for emphasis
- Size: 20px (default), 24px (large), 16px (small)

### 5.2 User Flows

**User Registration Flow:**
```
1. Landing Page
   ↓
2. Click "Sign Up" button
   ↓
3. Registration Form
   - Email
   - Password
   - First Name / Last Name
   - Accept Terms
   ↓
4. Submit Form
   ↓
5. Success Message
   "Check your email to verify your account"
   ↓
6. Email Sent
   ↓
7. User Clicks Email Link
   ↓
8. Email Verified
   ↓
9. Redirect to Dashboard
   ↓
10. Welcome Tour (optional)
```

**Profile Creation Flow:**
```
1. Dashboard
   ↓
2. Click "Create New Profile"
   ↓
3. Select Profile Type
   - Personal
   - Business
   - Organization
   ↓
4. Choose Template (optional)
   ↓
5. Basic Information Form
   - Display Name
   - Headline
   - Email/Phone
   ↓
6. Upload Profile Image
   ↓
7. Add Social Links (optional)
   ↓
8. Preview Profile
   ↓
9. Save as Draft OR Publish
   ↓
10. Profile Created
    ↓
11. View Profile OR Continue Editing
```

**Restaurant Menu Creation Flow:**
```
1. Profile Dashboard
   ↓
2. Enable Catalog Feature
   ↓
3. Select "Menu" Type
   ↓
4. Create Categories
   - Appetizers
   - Main Course
   - Desserts
   - Beverages
   ↓
5. Add Menu Items
   - Item Name
   - Description
   - Price
   - Upload Image
   - Variants (Small, Medium, Large)
   - Add-ons (Extra Cheese, etc.)
   ↓
6. Arrange Items
   ↓
7. Preview Menu
   ↓
8. Generate QR Code
   ↓
9. Download/Print QR Code
   ↓
10. Place on Tables
```

### 5.3 Responsive Design Strategy

**Breakpoints (Tailwind CSS):**
```
Mobile:    < 640px   (sm)  - Single column, stacked layout
Tablet:    640-768px (md)  - 2 columns, compact layout
Desktop:   768-1024px (lg) - 3 columns, expanded layout
Large:     1024-1280px (xl) - 4 columns, full layout
X-Large:   > 1280px (2xl) - Max-width container
```

**Mobile Optimizations:**
- Touch-friendly buttons (min 44x44px)
- Simplified navigation (hamburger menu)
- Swipe gestures where appropriate
- Optimized images (WebP format, lazy loading)
- Reduced animations on low-power devices

**Desktop Enhancements:**
- Multi-column layouts
- Hover effects and tooltips
- Keyboard shortcuts
- Drag-and-drop functionality
- Context menus

### 5.4 Accessibility (WCAG 2.1 Level AA)

**Visual Accessibility:**
- Color contrast ratio ≥ 4.5:1 for normal text
- Color contrast ratio ≥ 3:1 for large text
- Text resizable up to 200% without loss of functionality
- No content conveyed by color alone

**Keyboard Accessibility:**
- All functionality accessible via keyboard
- Visible focus indicators
- Logical tab order
- Skip to main content link
- Keyboard shortcuts documented

**Screen Reader Accessibility:**
- Semantic HTML elements
- ARIA labels where needed
- Alt text for all images
- Form labels associated with inputs
- Error messages announced

**Other Considerations:**
- Captions for video content (future)
- Text alternatives for audio (future)
- Simple, clear language
- Consistent navigation
- Error prevention and recovery

---

## 6. Technical Architecture

### 6.1 Frontend Architecture (Vue.js)

**Technology Stack:**
```
Framework:        Vue.js 3.4+ (Composition API)
UI Framework:     Tailwind CSS 3.4+
State Management: Pinia 2.1+
Routing:          Vue Router 4.2+
HTTP Client:      Axios 1.6+
Build Tool:       Vite 5.0+
Icons:            Heroicons
Validation:       VeeValidate
Date Handling:    Day.js
Rich Text:        TipTap (for descriptions)
```

**Project Structure:**
```
frontend/
├── public/
│   ├── favicon.ico
│   └── robots.txt
├── src/
│   ├── assets/
│   │   ├── images/
│   │   └── styles/
│   │       └── main.css (Tailwind imports)
│   ├── components/
│   │   ├── common/
│   │   │   ├── BaseButton.vue
│   │   │   ├── BaseInput.vue
│   │   │   ├── BaseModal.vue
│   │   │   ├── LoadingSpinner.vue
│   │   │   └── ErrorAlert.vue
│   │   ├── layout/
│   │   │   ├── AppHeader.vue
│   │   │   ├── AppFooter.vue
│   │   │   ├── AppSidebar.vue
│   │   │   └── AppNav.vue
│   │   ├── profile/
│   │   │   ├── ProfileCard.vue
│   │   │   ├── ProfileForm.vue
│   │   │   ├── ProfilePreview.vue
│   │   │   └── SocialLinks.vue
│   │   ├── catalog/
│   │   │   ├── CatalogGrid.vue
│   │   │   ├── CategoryList.vue
│   │   │   ├── ItemCard.vue
│   │   │   └── ItemModal.vue
│   │   └── analytics/
│   │       ├── StatsCard.vue
│   │       ├── ViewsChart.vue
│   │       └── GeographyMap.vue
│   ├── composables/
│   │   ├── useAuth.js
│   │   ├── useApi.js
│   │   ├── useProfiles.js
│   │   ├── useCatalogs.js
│   │   └── useNotifications.js
│   ├── views/
│   │   ├── public/
│   │   │   ├── Home.vue
│   │   │   ├── ProfileView.vue
│   │   │   ├── CatalogView.vue
│   │   │   └── Search.vue
│   │   ├── auth/
│   │   │   ├── Login.vue
│   │   │   ├── Register.vue
│   │   │   ├── ForgotPassword.vue
│   │   │   └── ResetPassword.vue
│   │   ├── dashboard/
│   │   │   ├── Dashboard.vue
│   │   │   ├── ProfileManager.vue
│   │   │   ├── CatalogManager.vue
│   │   │   ├── TeamManager.vue
│   │   │   ├── Analytics.vue
│   │   │   └── Settings.vue
│   │   └── admin/
│   │       ├── AdminDashboard.vue
│   │       ├── UserManagement.vue
│   │       └── SystemAnalytics.vue
│   ├── stores/
│   │   ├── auth.js
│   │   ├── profile.js
│   │   ├── catalog.js
│   │   └── ui.js
│   ├── services/
│   │   ├── api.js (Axios instance)
│   │   ├── authService.js
│   │   ├── profileService.js
│   │   ├── catalogService.js
│   │   └── analyticsService.js
│   ├── utils/
│   │   ├── validators.js
│   │   ├── formatters.js
│   │   ├── constants.js
│   │   └── helpers.js
│   ├── router/
│   │   ├── index.js
│   │   └── guards.js (auth guards)
│   ├── App.vue
│   └── main.js
├── .env.development
├── .env.production
├── tailwind.config.js
├── vite.config.js
├── package.json
└── README.md
```

**State Management (Pinia):**
```javascript
// stores/auth.js
import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import authService from '@/services/authService'

export const useAuthStore = defineStore('auth', () => {
  // State
  const user = ref(null)
  const token = ref(localStorage.getItem('token'))
  const isAuthenticated = computed(() => !!token.value)
  
  // Actions
  async function login(credentials) {
    const response = await authService.login(credentials)
    user.value = response.user
    token.value = response.accessToken
    localStorage.setItem('token', response.accessToken)
  }
  
  function logout() {
    user.value = null
    token.value = null
    localStorage.removeItem('token')
  }
  
  return { user, token, isAuthenticated, login, logout }
})
```

**API Service Pattern:**
```javascript
// services/profileService.js
import api from './api'

export default {
  getProfiles(params) {
    return api.get('/profiles', { params })
  },
  
  getProfileById(id) {
    return api.get(`/profiles/${id}`)
  },
  
  getProfileBySlug(slug) {
    return api.get(`/profiles/slug/${slug}`)
  },
  
  createProfile(data) {
    return api.post('/profiles', data)
  },
  
  updateProfile(id, data) {
    return api.put(`/profiles/${id}`, data)
  },
  
  deleteProfile(id) {
    return api.delete(`/profiles/${id}`)
  },
  
  publishProfile(id) {
    return api.post(`/profiles/${id}/publish`)
  }
}
```

### 6.2 Backend Architecture (ASP.NET 6 API)

**Technology Stack:**
```
Framework:          ASP.NET 6.0 Web API
Language:           C# 10
ORM:                Entity Framework Core 6.0
Database:           MySQL 8.0+
Authentication:     JWT Bearer Tokens
Validation:         FluentValidation
Mapping:            AutoMapper
Logging:            Serilog
Caching:            Redis (IDistributedCache)
API Documentation:  Swagger/OpenAPI
Testing:            xUnit, Moq
```

**Project Structure:**
```
backend/
├── BizBio.API/
│   ├── Controllers/
│   │   ├── AuthController.cs
│   │   ├── ProfilesController.cs
│   │   ├── CatalogsController.cs
│   │   ├── ItemsController.cs
│   │   ├── AnalyticsController.cs
│   │   ├── SubscriptionsController.cs
│   │   └── AdminController.cs
│   ├── Middleware/
│   │   ├── JwtMiddleware.cs
│   │   ├── ExceptionHandlerMiddleware.cs
│   │   └── RequestLoggingMiddleware.cs
│   ├── Filters/
│   │   ├── ValidateModelAttribute.cs
│   │   └── AuthorizeRolesAttribute.cs
│   ├── Program.cs
│   ├── appsettings.json
│   └── appsettings.Development.json
├── BizBio.Core/
│   ├── Entities/
│   │   ├── User.cs
│   │   ├── Profile.cs
│   │   ├── Catalog.cs
│   │   ├── CatalogItem.cs
│   │   ├── Subscription.cs
│   │   └── ...
│   ├── Enums/
│   │   ├── ProfileType.cs
│   │   ├── SubscriptionTier.cs
│   │   └── ProfileStatus.cs
│   ├── Interfaces/
│   │   ├── IRepository.cs
│   │   ├── IUserRepository.cs
│   │   ├── IProfileRepository.cs
│   │   └── ...
│   └── Specifications/
│       └── ProfileSpecifications.cs
├── BizBio.Application/
│   ├── DTOs/
│   │   ├── Auth/
│   │   │   ├── LoginRequestDto.cs
│   │   │   ├── RegisterRequestDto.cs
│   │   │   └── AuthResponseDto.cs
│   │   ├── Profile/
│   │   │   ├── ProfileRequestDto.cs
│   │   │   ├── ProfileResponseDto.cs
│   │   │   └── ProfileListDto.cs
│   │   └── ...
│   ├── Services/
│   │   ├── AuthService.cs
│   │   ├── ProfileService.cs
│   │   ├── CatalogService.cs
│   │   ├── SubscriptionService.cs
│   │   ├── PaymentService.cs
│   │   ├── EmailService.cs
│   │   └── FileStorageService.cs
│   ├── Mappings/
│   │   └── AutoMapperProfile.cs
│   ├── Validators/
│   │   ├── RegisterRequestValidator.cs
│   │   ├── ProfileRequestValidator.cs
│   │   └── ...
│   └── Interfaces/
│       ├── IAuthService.cs
│       ├── IProfileService.cs
│       └── ...
├── BizBio.Infrastructure/
│   ├── Data/
│   │   ├── ApplicationDbContext.cs
│   │   ├── Configurations/
│   │   │   ├── UserConfiguration.cs
│   │   │   ├── ProfileConfiguration.cs
│   │   │   └── ...
│   │   └── Migrations/
│   │       └── (EF Core migrations)
│   ├── Repositories/
│   │   ├── GenericRepository.cs
│   │   ├── UserRepository.cs
│   │   ├── ProfileRepository.cs
│   │   └── ...
│   ├── Services/
│   │   ├── CloudinaryFileStorageService.cs
│   │   ├── SendGridEmailService.cs
│   │   └── PayFastPaymentService.cs
│   └── Utilities/
│       ├── JwtHelper.cs
│       ├── PasswordHasher.cs
│       └── SlugGenerator.cs
├── BizBio.Tests/
│   ├── Unit/
│   │   ├── Services/
│   │   └── Validators/
│   ├── Integration/
│   │   └── Controllers/
│   └── TestFixtures/
└── BizBio.sln
```

**Controller Example:**
```csharp
// Controllers/ProfilesController.cs
[ApiController]
[Route("api/v1/[controller]")]
public class ProfilesController : ControllerBase
{
    private readonly IProfileService _profileService;
    private readonly ILogger<ProfilesController> _logger;
    
    public ProfilesController(
        IProfileService profileService,
        ILogger<ProfilesController> logger)
    {
        _profileService = profileService;
        _logger = logger;
    }
    
    [HttpGet]
    [Authorize]
    public async Task<ActionResult<PagedResult<ProfileListDto>>> GetProfiles(
        [FromQuery] ProfileQueryParams queryParams)
    {
        var userId = User.GetUserId();
        var result = await _profileService.GetUserProfilesAsync(userId, queryParams);
        return Ok(result);
    }
    
    [HttpGet("{id}")]
    [Authorize]
    public async Task<ActionResult<ProfileResponseDto>> GetProfile(int id)
    {
        var userId = User.GetUserId();
        var profile = await _profileService.GetProfileByIdAsync(id, userId);
        
        if (profile == null)
            return NotFound();
            
        return Ok(profile);
    }
    
    [HttpGet("slug/{slug}")]
    [AllowAnonymous]
    public async Task<ActionResult<ProfileResponseDto>> GetProfileBySlug(string slug)
    {
        var profile = await _profileService.GetPublicProfileBySlugAsync(slug);
        
        if (profile == null)
            return NotFound();
            
        return Ok(profile);
    }
    
    [HttpPost]
    [Authorize]
    public async Task<ActionResult<ProfileResponseDto>> CreateProfile(
        [FromBody] ProfileRequestDto dto)
    {
        var userId = User.GetUserId();
        var profile = await _profileService.CreateProfileAsync(userId, dto);
        
        return CreatedAtAction(
            nameof(GetProfile),
            new { id = profile.Id },
            profile);
    }
    
    [HttpPut("{id}")]
    [Authorize]
    public async Task<ActionResult<ProfileResponseDto>> UpdateProfile(
        int id,
        [FromBody] ProfileRequestDto dto)
    {
        var userId = User.GetUserId();
        var profile = await _profileService.UpdateProfileAsync(id, userId, dto);
        
        if (profile == null)
            return NotFound();
            
        return Ok(profile);
    }
    
    [HttpPost("{id}/publish")]
    [Authorize]
    public async Task<ActionResult<ProfileResponseDto>> PublishProfile(int id)
    {
        var userId = User.GetUserId();
        var profile = await _profileService.PublishProfileAsync(id, userId);
        
        if (profile == null)
            return NotFound();
            
        return Ok(profile);
    }
    
    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> DeleteProfile(int id)
    {
        var userId = User.GetUserId();
        var success = await _profileService.DeleteProfileAsync(id, userId);
        
        if (!success)
            return NotFound();
            
        return NoContent();
    }
}
```

**Service Layer Example:**
```csharp
// Application/Services/ProfileService.cs
public class ProfileService : IProfileService
{
    private readonly IProfileRepository _profileRepository;
    private readonly ISubscriptionService _subscriptionService;
    private readonly IMapper _mapper;
    private readonly ILogger<ProfileService> _logger;
    
    public ProfileService(
        IProfileRepository profileRepository,
        ISubscriptionService subscriptionService,
        IMapper mapper,
        ILogger<ProfileService> logger)
    {
        _profileRepository = profileRepository;
        _subscriptionService = subscriptionService;
        _mapper = mapper;
        _logger = logger;
    }
    
    public async Task<ProfileResponseDto> CreateProfileAsync(
        int userId,
        ProfileRequestDto dto)
    {
        // Check tier limits
        var canCreate = await _subscriptionService
            .CanCreateProfileAsync(userId);
            
        if (!canCreate)
            throw new BusinessException(
                "Profile limit reached for your subscription tier");
        
        // Generate unique slug
        var slug = await GenerateUniqueSlugAsync(dto.DisplayName);
        
        // Create entity
        var profile = new Profile
        {
            OwnerId = userId,
            ProfileType = dto.ProfileType,
            DisplayName = dto.DisplayName,
            Slug = slug,
            Headline = dto.Headline,
            Description = dto.Description,
            Email = dto.Email,
            PhoneNumber = dto.PhoneNumber,
            Status = ProfileStatus.Draft,
            CreatedAt = DateTime.UtcNow
        };
        
        // Save to database
        await _profileRepository.AddAsync(profile);
        await _profileRepository.SaveChangesAsync();
        
        _logger.LogInformation(
            "Profile {ProfileId} created by user {UserId}",
            profile.Id,
            userId);
        
        // Map and return
        return _mapper.Map<ProfileResponseDto>(profile);
    }
    
    private async Task<string> GenerateUniqueSlugAsync(string displayName)
    {
        var baseSlug = SlugGenerator.Generate(displayName);
        var slug = baseSlug;
        var counter = 1;
        
        while (await _profileRepository.SlugExistsAsync(slug))
        {
            slug = $"{baseSlug}-{counter}";
            counter++;
        }
        
        return slug;
    }
}
```

---

## 7. Data Architecture

### 7.1 Database Design Philosophy

**Code-First Approach:**
- Entity models defined in C# code
- Database schema generated from models
- Migrations track schema changes
- Easy to version control
- Database-agnostic (can switch from MySQL to PostgreSQL)

**Design Principles:**
- Normalization (3NF minimum)
- Integer primary keys (auto-increment)
- Foreign key constraints enforced
- Indexes on frequently queried columns
- Soft deletes where appropriate
- Audit fields (CreatedAt, UpdatedAt)

### 7.2 Entity Relationship Diagram (ERD)

```
┌─────────────────┐
│     Users       │
├─────────────────┤
│ Id (PK)         │
│ Email           │
│ PasswordHash    │
│ FirstName       │
│ LastName        │
│ PhoneNumber     │
│ SubscriptionId  │◄──┐
│ CreatedAt       │   │
│ UpdatedAt       │   │
└─────────────────┘   │
        │              │
        │ 1            │
        │              │
        │ *            │
        ▼              │
┌─────────────────┐   │
│    Profiles     │   │
├─────────────────┤   │
│ Id (PK)         │   │
│ OwnerId (FK)    │───┘
│ ProfileType     │
│ DisplayName     │
│ Slug (unique)   │
│ Headline        │
│ Description     │
│ Email           │
│ PhoneNumber     │
│ Status          │
│ IsPublished     │
│ ViewCount       │
│ ProfileImageUrl │
│ CoverImageUrl   │
│ CreatedAt       │
│ UpdatedAt       │
└─────────────────┘
        │
        │ 1
        │
        │ *
        ▼
┌─────────────────────┐
│ ProfileCatalogs     │
├─────────────────────┤
│ Id (PK)             │
│ ProfileId (FK)      │
│ CatalogType         │
│ DisplayName         │
│ Currency            │
│ IsPublished         │
│ CreatedAt           │
└─────────────────────┘
        │
        │ 1
        │
        │ *
        ▼
┌─────────────────────┐
│ CatalogCategories   │
├─────────────────────┤
│ Id (PK)             │
│ CatalogId (FK)      │
│ Name                │
│ Description         │
│ DisplayOrder        │
│ IsActive            │
│ CreatedAt           │
└─────────────────────┘
        │
        │ 1
        │
        │ *
        ▼
┌─────────────────────┐
│   CatalogItems      │
├─────────────────────┤
│ Id (PK)             │
│ CatalogId (FK)      │
│ CategoryId (FK)     │
│ Name                │
│ Description         │
│ Price               │
│ CompareAtPrice      │
│ SKU                 │
│ IsAvailable         │
│ Featured            │
│ ViewCount           │
│ DisplayOrder        │
│ CreatedAt           │
│ UpdatedAt           │
└─────────────────────┘
        │
        ├───1:* ─────────────────────┐
        │                             │
        ▼                             ▼
┌──────────────────┐    ┌────────────────────────┐
│  ItemImages      │    │    ItemVariants        │
├──────────────────┤    ├────────────────────────┤
│ Id (PK)          │    │ Id (PK)                │
│ ItemId (FK)      │    │ ItemId (FK)            │
│ ImageUrl         │    │ Name                   │
│ ThumbnailUrl     │    │ Price                  │
│ DisplayOrder     │    │ PriceAdjustment        │
│ AltText          │    │ SKU                    │
│ UploadedAt       │    │ IsAvailable            │
└──────────────────┘    │ CreatedAt              │
                        └────────────────────────┘

┌─────────────────────────┐
│  SubscriptionTiers      │
├─────────────────────────┤
│ Id (PK)                 │
│ Name                    │
│ Price                   │
│ MaxProfiles             │
│ MaxCatalogItems         │
│ Features (JSON)         │
│ IsActive                │
└─────────────────────────┘
        │
        │ 1
        │
        │ *
        ▼
┌─────────────────────────┐
│  UserSubscriptions      │
├─────────────────────────┤
│ Id (PK)                 │
│ UserId (FK)             │
│ TierId (FK)             │
│ Status                  │
│ StartDate               │
│ EndDate                 │
│ AutoRenew               │
│ CreatedAt               │
└─────────────────────────┘

┌───────────────────────┐
│  ProfileRelationships │
├───────────────────────┤
│ Id (PK)               │
│ OrganizationId (FK)   │
│ MemberId (FK)         │
│ RelationshipType      │
│ JobTitle              │
│ Department            │
│ StartDate             │
│ EndDate               │
│ IsActive              │
│ CreatedAt             │
└───────────────────────┘

┌──────────────────────┐
│  ProfileAnalytics    │
├──────────────────────┤
│ Id (PK)              │
│ ProfileId (FK)       │
│ Date                 │
│ ViewCount            │
│ UniqueVisitors       │
│ ClickCount           │
│ DeviceType           │
│ Country              │
│ City                 │
│ ReferrerUrl          │
│ CreatedAt            │
└──────────────────────┘
```

### 7.3 Core Entity Models

**User Entity:**
```csharp
public class User
{
    public int Id { get; set; }
    public string Email { get; set; }
    public string PasswordHash { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string PhoneNumber { get; set; }
    public bool IsEmailVerified { get; set; }
    public string EmailVerificationToken { get; set; }
    public DateTime? EmailVerifiedAt { get; set; }
    public string PasswordResetToken { get; set; }
    public DateTime? PasswordResetExpiry { get; set; }
    public int FailedLoginAttempts { get; set; }
    public DateTime? LockoutEnd { get; set; }
    public DateTime? LastLoginAt { get; set; }
    public UserRole Role { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    // Navigation properties
    public UserSubscription Subscription { get; set; }
    public ICollection<Profile> Profiles { get; set; }
}
```

**Profile Entity:**
```csharp
public class Profile
{
    public int Id { get; set; }
    public int OwnerId { get; set; }
    public ProfileType ProfileType { get; set; }
    public string DisplayName { get; set; }
    public string Slug { get; set; }
    public string Headline { get; set; }
    public string Description { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public string Website { get; set; }
    public string ProfileImageUrl { get; set; }
    public string CoverImageUrl { get; set; }
    
    // Address
    public string AddressStreet { get; set; }
    public string AddressCity { get; set; }
    public string AddressProvince { get; set; }
    public string AddressPostalCode { get; set; }
    public string AddressCountry { get; set; }
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
    
    // Business details
    public string BusinessCategory { get; set; }
    public string RegistrationNumber { get; set; }
    public string VatNumber { get; set; }
    public string HoursOfOperation { get; set; } // JSON
    
    // Status
    public ProfileStatus Status { get; set; }
    public bool IsPublished { get; set; }
    public DateTime? PublishedAt { get; set; }
    public int ViewCount { get; set; }
    
    // SEO
    public string MetaTitle { get; set; }
    public string MetaDescription { get; set; }
    public string MetaKeywords { get; set; }
    
    // Timestamps
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
    
    // Navigation properties
    public User Owner { get; set; }
    public ProfileCustomization Customization { get; set; }
    public ProfileCatalog Catalog { get; set; }
    public ICollection<SocialMediaLink> SocialLinks { get; set; }
    public ICollection<ProfileDocument> Documents { get; set; }
    public ICollection<ProfileAnalytics> Analytics { get; set; }
    public ICollection<ProfileRelationship> TeamMembers { get; set; }
}
```

**Catalog Item Entity:**
```csharp
public class CatalogItem
{
    public int Id { get; set; }
    public int CatalogId { get; set; }
    public int CategoryId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public decimal? CompareAtPrice { get; set; }
    public string SKU { get; set; }
    public bool IsAvailable { get; set; }
    public bool Featured { get; set; }
    public string Tags { get; set; } // JSON array
    public string Specifications { get; set; } // JSON object
    public int ViewCount { get; set; }
    public int DisplayOrder { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
    
    // Navigation properties
    public ProfileCatalog Catalog { get; set; }
    public CatalogCategory Category { get; set; }
    public ICollection<CatalogItemImage> Images { get; set; }
    public ICollection<CatalogItemVariant> Variants { get; set; }
    public ICollection<CatalogItemAddon> Addons { get; set; }
}
```

### 7.4 Database Configuration (EF Core)

**DbContext Example:**
```csharp
public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }
    
    public DbSet<User> Users { get; set; }
    public DbSet<Profile> Profiles { get; set; }
    public DbSet<ProfileCatalog> ProfileCatalogs { get; set; }
    public DbSet<CatalogCategory> CatalogCategories { get; set; }
    public DbSet<CatalogItem> CatalogItems { get; set; }
    public DbSet<SubscriptionTier> SubscriptionTiers { get; set; }
    public DbSet<UserSubscription> UserSubscriptions { get; set; }
    // ... other DbSets
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Apply all configurations
        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(ApplicationDbContext).Assembly);
            
        // Global query filters (soft delete)
        modelBuilder.Entity<Profile>()
            .HasQueryFilter(p => p.DeletedAt == null);
            
        modelBuilder.Entity<CatalogItem>()
            .HasQueryFilter(i => i.DeletedAt == null);
    }
}
```

**Entity Configuration Example:**
```csharp
public class ProfileConfiguration : IEntityTypeConfiguration<Profile>
{
    public void Configure(EntityTypeBuilder<Profile> builder)
    {
        builder.ToTable("Profiles");
        
        builder.HasKey(p => p.Id);
        
        builder.Property(p => p.Id)
            .UseIdentityColumn();
            
        builder.Property(p => p.DisplayName)
            .IsRequired()
            .HasMaxLength(100);
            
        builder.Property(p => p.Slug)
            .IsRequired()
            .HasMaxLength(150);
            
        builder.HasIndex(p => p.Slug)
            .IsUnique();
            
        builder.Property(p => p.Email)
            .HasMaxLength(255);
            
        builder.Property(p => p.Description)
            .HasMaxLength(2000);
            
        builder.Property(p => p.ProfileType)
            .HasConversion<string>()
            .HasMaxLength(50);
            
        builder.Property(p => p.Status)
            .HasConversion<string>()
            .HasMaxLength(50);
            
        builder.Property(p => p.CreatedAt)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");
            
        builder.Property(p => p.UpdatedAt)
            .HasDefaultValueSql("CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP");
            
        // Relationships
        builder.HasOne(p => p.Owner)
            .WithMany(u => u.Profiles)
            .HasForeignKey(p => p.OwnerId)
            .OnDelete(DeleteBehavior.Restrict);
            
        builder.HasOne(p => p.Customization)
            .WithOne(c => c.Profile)
            .HasForeignKey<ProfileCustomization>(c => c.ProfileId);
            
        builder.HasOne(p => p.Catalog)
            .WithOne(c => c.Profile)
            .HasForeignKey<ProfileCatalog>(c => c.ProfileId);
    }
}
```

### 7.5 Migration Strategy

**Development Environment:**
```bash
# Create new migration
dotnet ef migrations add InitialCreate --project BizBio.Infrastructure

# Update database
dotnet ef database update --project BizBio.Infrastructure

# Rollback migration
dotnet ef database update PreviousMigrationName
```

**Production Environment:**
- Migrations applied automatically on deployment (via CI/CD)
- Database backup before applying migrations
- Rollback plan for each migration
- Zero-downtime migrations using blue-green deployment

---

## 8. Security Architecture

### 8.1 Authentication & Authorization

**JWT-Based Authentication:**
```
User Login → Validate Credentials → Generate JWT Token
↓
Token contains:
- User ID
- Email
- Role
- Subscription Tier
- Expiration (1 hour)
↓
Client stores token in localStorage/sessionStorage
↓
Subsequent requests include token in Authorization header
↓
Server validates token signature and expiration
↓
User context extracted from token
↓
Authorization checks performed
```

**Token Structure:**
```json
{
  "header": {
    "alg": "HS256",
    "typ": "JWT"
  },
  "payload": {
    "sub": "user-id",
    "email": "user@example.com",
    "role": "User",
    "tier": "Professional",
    "iat": 1699430400,
    "exp": 1699434000
  },
  "signature": "..."
}
```

**Password Security:**
- Hashing algorithm: BCrypt (cost factor: 12)
- Password minimum 8 characters
- Must contain: uppercase, lowercase, number
- Password history (last 5 passwords)
- Account lockout after 5 failed attempts
- Lockout duration: 15 minutes

**Authorization Levels:**
1. **Public:** No authentication required
2. **Authenticated:** Valid JWT required
3. **Owner:** Must own the resource
4. **Admin:** Admin role required
5. **Subscription Tier:** Specific tier required

### 8.2 Data Protection

**Encryption:**
- TLS 1.3 for all data in transit
- HTTPS enforced (HTTP redirects to HTTPS)
- Database connections encrypted (MySQL SSL)
- Sensitive data encrypted at rest (passwords, tokens)
- AES-256 encryption for file storage

**Data Privacy:**
- POPIA compliance (South African law)
- GDPR compliance (for EU users)
- Data minimization principle
- User consent tracking
- Right to access data
- Right to delete data (30-day retention)
- Data export functionality

**Input Validation:**
- Client-side validation (Vue.js/VeeValidate)
- Server-side validation (FluentValidation)
- SQL injection prevention (parameterized queries)
- XSS prevention (sanitized outputs)
- CSRF protection (SameSite cookies, CORS)
- File upload validation (type, size, content)

### 8.3 API Security

**Rate Limiting:**
```
Public endpoints:    100 requests/hour per IP
Authenticated:       1000 requests/hour per user
Admin:               No limit
Future Public API:   Based on subscription tier
```

**CORS Configuration:**
```csharp
services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", builder =>
    {
        builder.WithOrigins("https://bizbio.co.za", "https://www.bizbio.co.za")
               .AllowAnyMethod()
               .AllowAnyHeader()
               .AllowCredentials();
    });
});
```

**Request Validation:**
- Content-Type validation
- Request size limits (10MB max)
- URL parameter validation
- JSON schema validation
- Anti-automation (CAPTCHA for registration)

### 8.4 Infrastructure Security

**Server Hardening:**
- Firewall configured (allow only 80, 443)
- SSH key-based authentication only
- Automated security updates
- Intrusion detection system
- Regular security audits
- DDoS protection (CloudFlare)

**Application Monitoring:**
- Real-time error tracking (Serilog + Seq)
- Performance monitoring (Application Insights)
- Security event logging
- Failed authentication tracking
- Unusual activity alerts

**Backup & Recovery:**
- Automated daily database backups
- Backup retention: 30 days
- Encrypted backups
- Tested recovery procedures
- Disaster recovery plan

---

## 9. Integration Architecture

### 9.1 Third-Party Integrations

**Payment Gateway (PayFast):**
```
Purpose: Process subscription payments
Integration Type: HTTP API
Authentication: Merchant ID + Key + Passphrase
Data Flow:
  1. User subscribes to plan
  2. Backend generates PayFast payment URL
  3. User redirected to PayFast
  4. User completes payment
  5. PayFast sends ITN (Instant Transaction Notification)
  6. Backend validates ITN signature
  7. Subscription activated
  8. User notified via email
```

**Email Service (SendGrid):**
```
Purpose: Transactional emails
Integration Type: HTTP API
Authentication: API Key
Use Cases:
  - Welcome email
  - Email verification
  - Password reset
  - Subscription notifications
  - Payment receipts
  - Team member invitations
```

**File Storage (AWS S3 / Azure Blob):**
```
Purpose: Store profile images, catalog images, documents
Integration Type: SDK
Authentication: Access Key + Secret
File Structure:
  /profiles/{profile-id}/
    - profile-image.jpg
    - cover-image.jpg
  /catalogs/{catalog-id}/
    /items/{item-id}/
      - image-1.jpg
      - image-2.jpg
  /documents/{profile-id}/
    - brochure.pdf
    - menu.pdf
```

**Google Maps (Optional):**
```
Purpose: Location display, geocoding
Integration Type: JavaScript API
Authentication: API Key
Use Cases:
  - Display business location on profile
  - Geocode addresses
  - Distance calculations (future)
```

### 9.2 Future Public API

**Preparation Strategy:**
- API already RESTful and well-structured
- Documentation via Swagger/OpenAPI
- Versioning in place (v1)
- Rate limiting implemented
- Authentication ready (JWT)

**Future Enhancements:**
- OAuth 2.0 authentication
- API key management UI
- Webhook support
- Detailed API analytics
- Developer portal
- API client libraries (Python, PHP, JavaScript)

---

## 10. Deployment Architecture

### 10.1 Infrastructure Overview

**Deployment Environment:**
```
┌──────────────────────────────────────────────────────────┐
│                    CloudFlare CDN                         │
│  (Static assets, DDoS protection, SSL termination)       │
└──────────────────────────────────────────────────────────┘
                          ↓
┌──────────────────────────────────────────────────────────┐
│                    Load Balancer                          │
│  (NGINX / AWS ALB / Azure Load Balancer)                 │
└──────────────────────────────────────────────────────────┘
           ↓                              ↓
┌────────────────────┐        ┌────────────────────┐
│  Frontend Server   │        │  Frontend Server   │
│  (Vue.js SPA)      │        │  (Vue.js SPA)      │
│  NGINX Static      │        │  NGINX Static      │
└────────────────────┘        └────────────────────┘
           ↓                              ↓
┌──────────────────────────────────────────────────────────┐
│                    API Load Balancer                      │
└──────────────────────────────────────────────────────────┘
           ↓                              ↓
┌────────────────────┐        ┌────────────────────┐
│  API Server 1      │        │  API Server 2      │
│  (ASP.NET 6)       │        │  (ASP.NET 6)       │
│  Kestrel           │        │  Kestrel           │
└────────────────────┘        └────────────────────┘
           ↓                              ↓
┌──────────────────────────────────────────────────────────┐
│                    MySQL Database                         │
│  (Primary + Read Replica)                                │
└──────────────────────────────────────────────────────────┘
           ↓                              ↓
┌────────────────────┐        ┌────────────────────┐
│  Redis Cache       │        │  Blob Storage      │
└────────────────────┘        └────────────────────┘
```

### 10.2 Hosting Options

**Option 1: Cloud Provider (AWS/Azure) - Recommended:**
- Frontend: S3/Azure Blob + CloudFront/Azure CDN
- Backend: EC2/Azure App Service or Kubernetes
- Database: RDS MySQL/Azure Database for MySQL
- Cache: ElastiCache Redis/Azure Cache for Redis
- Storage: S3/Azure Blob Storage
- Scalability: Excellent (auto-scaling)
- Cost: Pay-as-you-grow

**Option 2: VPS (DigitalOcean, Linode):**
- Frontend: NGINX serving static files
- Backend: Kestrel with NGINX reverse proxy
- Database: MySQL on same server or managed
- Cache: Redis on same server
- Storage: Local or object storage add-on
- Scalability: Manual (vertical scaling)
- Cost: Predictable, lower initial cost

**Option 3: Managed Kubernetes:**
- Frontend: Containerized, multiple replicas
- Backend: Containerized, auto-scaling
- Database: Managed MySQL
- Cache: Redis Helm chart
- Scalability: Excellent, complex setup
- Cost: Higher, professional operations

### 10.3 CI/CD Pipeline

**Development Workflow:**
```
Developer commits code
    ↓
Git push to repository (GitHub/GitLab)
    ↓
CI/CD pipeline triggered
    ↓
┌─────────────────────────────────────┐
│  Build Stage                        │
│  - Install dependencies             │
│  - Build Vue.js app (npm run build)│
│  - Build .NET app (dotnet publish) │
└─────────────────────────────────────┘
    ↓
┌─────────────────────────────────────┐
│  Test Stage                         │
│  - Run unit tests                   │
│  - Run integration tests            │
│  - Code quality checks (SonarQube) │
│  - Security scans                   │
└─────────────────────────────────────┘
    ↓
┌─────────────────────────────────────┐
│  Deploy to Staging                  │
│  - Deploy frontend to staging CDN   │
│  - Deploy backend to staging server │
│  - Run database migrations          │
│  - Run smoke tests                  │
└─────────────────────────────────────┘
    ↓
┌─────────────────────────────────────┐
│  Manual Approval (Production)       │
└─────────────────────────────────────┘
    ↓
┌─────────────────────────────────────┐
│  Deploy to Production               │
│  - Blue-green deployment            │
│  - Zero-downtime migration          │
│  - Health checks                    │
│  - Rollback if needed               │
└─────────────────────────────────────┘
```

**GitHub Actions Example:**
```yaml
name: Deploy to Production

on:
  push:
    branches: [ main ]

jobs:
  build-frontend:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v3
      - uses: actions/setup-node@v3
        with:
          node-version: '18'
      - run: cd frontend && npm ci
      - run: cd frontend && npm run build
      - uses: aws-actions/configure-aws-credentials@v2
      - run: aws s3 sync frontend/dist s3://bizbio-frontend
      
  build-backend:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v3
      - uses: actions/setup-dotnet@v3
        with:
          dotnet-version: '6.0.x'
      - run: dotnet restore
      - run: dotnet build --configuration Release
      - run: dotnet test
      - run: dotnet publish -c Release -o ./publish
      - run: scp -r publish/* server:/var/www/bizbio-api
```

### 10.4 Monitoring & Observability

**Application Monitoring:**
- Uptime monitoring (Uptime Robot, Pingdom)
- Error tracking (Sentry, Raygun)
- Performance monitoring (Application Insights, New Relic)
- Log aggregation (Seq, ELK Stack)

**Infrastructure Monitoring:**
- Server metrics (CPU, memory, disk)
- Database performance (query times, connections)
- Cache hit rates
- Network latency

**Business Metrics:**
- User registrations
- Profile creations
- Subscription conversions
- Revenue tracking
- Feature usage

---

## 11. Development Methodology

### 11.1 Agile Development Approach

**Sprint Structure:**
- Sprint duration: 2 weeks
- Sprint planning: Monday Week 1
- Daily standups: 15 minutes
- Sprint review: Friday Week 2
- Sprint retrospective: Friday Week 2

**Team Roles:**
- Product Owner: Prioritizes backlog
- Scrum Master: Facilitates process
- Developers: Build features
- QA: Test features
- DevOps: Manage infrastructure

### 11.2 Development Phases

**Phase 1: Foundation (Weeks 1-4)**
- Database schema and migrations
- Authentication system (JWT)
- User registration and login
- Basic profile CRUD
- Admin panel skeleton

**Phase 2: Core Features (Weeks 5-10)**
- Profile management (complete)
- Profile customization
- Public profile views
- Catalog system (categories, items)
- Image uploads

**Phase 3: Advanced Features (Weeks 11-16)**
- Organization hierarchy
- Team member management
- Analytics dashboard
- Subscription management
- Payment integration (PayFast)

**Phase 4: Polish & Launch (Weeks 17-20)**
- NFC integration
- QR code generation
- Email notifications
- Performance optimization
- Security audit
- Beta testing
- Launch preparation

**Phase 5: Post-Launch (Weeks 21-28)**
- Bug fixes
- User feedback implementation
- Feature enhancements
- Documentation
- Marketing support
- Future roadmap planning

---

## 12. Quality Assurance Strategy

### 12.1 Testing Pyramid

```
                 ╱ ╲
                ╱ E2E ╲          10% - End-to-end tests
               ╱───────╲
              ╱Integration╲      30% - Integration tests
             ╱─────────────╲
            ╱  Unit Tests   ╲    60% - Unit tests
           ╱─────────────────╲
```

**Unit Tests:**
- Test individual functions/methods
- Mock dependencies
- Fast execution (<1s per test)
- 80%+ code coverage target
- Tools: xUnit, Moq (C#), Vitest (Vue.js)

**Integration Tests:**
- Test API endpoints
- Use test database
- Test service interactions
- Tools: xUnit with WebApplicationFactory

**End-to-End Tests:**
- Test complete user flows
- Automated browser testing
- Critical path testing
- Tools: Playwright, Cypress

### 12.2 Quality Gates

**Code Quality:**
- Code reviews required (2 approvals)
- Static code analysis (SonarQube)
- No critical issues
- Code coverage >70%
- No security vulnerabilities

**Performance:**
- API response time <200ms (95th percentile)
- Page load time <3s
- Lighthouse score >90

**Security:**
- OWASP Top 10 checks
- Dependency vulnerability scan
- Penetration testing (quarterly)

---

## 13. Risk Management

### 13.1 Technical Risks

| Risk | Impact | Probability | Mitigation |
|------|--------|-------------|------------|
| Database performance degradation | High | Medium | Implement caching, optimize queries, use read replicas |
| Third-party API downtime | Medium | Low | Implement retry logic, fallback mechanisms, status page |
| Security breach | High | Low | Regular security audits, encryption, monitoring |
| Hosting infrastructure failure | High | Low | Multi-region deployment, automated backups |
| Code quality issues | Medium | Medium | Code reviews, automated testing, CI/CD |

### 13.2 Business Risks

| Risk | Impact | Probability | Mitigation |
|------|--------|-------------|------------|
| Low user adoption | High | Medium | Marketing, user research, MVP validation |
| High churn rate | High | Medium | Excellent onboarding, customer support, value delivery |
| Competitor enters market | Medium | Medium | Unique features, local focus, first-mover advantage |
| Payment gateway issues | Medium | Low | Multiple payment options (future), clear error handling |
| Regulatory changes (POPIA) | Medium | Low | Legal consultation, compliance monitoring |

---

## 14. Project Timeline & Phases

### 14.1 Gantt Chart Overview

```
Months:         1      2      3      4      5      6      7
Weeks:     1234 1234 1234 1234 1234 1234 1234

Phase 1:   ████
(Foundation)

Phase 2:        ██████████
(Core Features)

Phase 3:                  ████████████
(Advanced)

Phase 4:                              ██████
(Polish/Launch)

Phase 5:                                    ████████
(Post-Launch)
```

### 14.2 Detailed Timeline

**Month 1 (Weeks 1-4): Foundation**
- Week 1: Project setup, repository, development environment
- Week 2: Database design, EF Core setup, initial migrations
- Week 3: Authentication system (JWT, registration, login)
- Week 4: Basic user management, admin panel

**Month 2 (Weeks 5-8): Core Profile Features**
- Week 5: Profile CRUD operations, slug generation
- Week 6: Profile types (Personal, Business, Organization)
- Week 7: Profile image uploads, social links
- Week 8: Public profile view, SEO optimization

**Month 3 (Weeks 9-12): Profile Customization & Catalog**
- Week 9: Profile customization (colors, fonts, themes)
- Week 10: Catalog system (categories, basic items)
- Week 11: Catalog items (variants, pricing, images)
- Week 12: Public catalog view, responsive design

**Month 4 (Weeks 13-16): Advanced Features**
- Week 13: Organization hierarchy, team members
- Week 14: Analytics dashboard, view tracking
- Week 15: Subscription tiers, tier-based features
- Week 16: Payment integration (PayFast)

**Month 5 (Weeks 17-20): Polish & Launch Prep**
- Week 17: NFC integration, QR code generation
- Week 18: Email notifications, templates
- Week 19: Performance optimization, security audit
- Week 20: Beta testing, bug fixes

**Month 6 (Weeks 21-24): Launch & Iteration**
- Week 21: Public launch, monitoring
- Week 22-24: User feedback, iterations, support

**Month 7 (Weeks 25-28): Post-Launch**
- Week 25-28: Feature enhancements, marketing support, roadmap

---

## 15. Success Metrics & KPIs

### 15.1 Technical KPIs

| Metric | Target | Measurement Frequency |
|--------|--------|----------------------|
| API Uptime | 99.9% | Continuous |
| API Response Time | <200ms (p95) | Daily |
| Page Load Time | <3s | Daily |
| Error Rate | <1% | Daily |
| Test Coverage | >70% | Per commit |
| Build Success Rate | >95% | Per build |
| Security Vulnerabilities | 0 critical | Weekly scan |

### 15.2 Business KPIs

| Metric | Month 3 | Month 6 | Month 12 |
|--------|---------|---------|----------|
| Registered Users | 300 | 1,000 | 5,000 |
| Paid Subscribers | 30 | 150 | 1,000 |
| Active Profiles | 150 | 500 | 3,000 |
| Restaurant Menus | 20 | 50 | 200 |
| MRR (Monthly Recurring Revenue) | R10k | R50k | R250k |
| Conversion Rate (Free to Paid) | 10% | 15% | 20% |
| Churn Rate | 15% | 10% | <8% |
| NPS (Net Promoter Score) | 40 | 50 | 60 |

### 15.3 User Engagement KPIs

| Metric | Target |
|--------|--------|
| Average profile views per user | 500/month |
| Return visitor rate | 30% |
| Average session duration | 3 minutes |
| Mobile traffic percentage | >80% |
| Social share rate | 15% |
| Profile completion rate | 80% |

---

## 16. Future Roadmap

### 16.1 Short-Term (Months 7-12)

**Q3 2026:**
- Mobile apps (iOS and Android)
- Advanced analytics (conversion tracking, funnel analysis)
- Multi-language support (English, Afrikaans, Zulu)
- WhatsApp integration for profile sharing
- Email signature generator
- Review and rating system

**Q4 2026:**
- White-label solution for enterprises
- Custom domain support (fully managed)
- Advanced team permissions
- Bulk operations (CSV import/export)
- API rate limiting by tier
- Affiliate program

### 16.2 Medium-Term (Year 2)

**Q1 2027:**
- Public API launch (OAuth 2.0)
- Developer portal and documentation
- Webhook support
- Third-party integrations (CRM, email marketing)
- Advanced customization (custom CSS)
- Video support in profiles

**Q2 2027:**
- E-commerce integration (online ordering)
- Payment processing for restaurants
- Table reservation system
- Loyalty program features
- SMS notifications
- Progressive Web App (PWA)

**Q3 2027:**
- AI-powered features (content suggestions, chatbot)
- Voice profile (podcast integration)
- AR business card (augmented reality)
- Blockchain-based verification (optional)
- International expansion (3 new countries)

**Q4 2027:**
- Enterprise features (SSO, advanced security)
- Compliance certifications (ISO 27001)
- Advanced reporting and business intelligence
- Partner integrations (accounting, inventory)
- Franchisee management tools

### 16.3 Long-Term Vision (Years 3-5)

**Platform Evolution:**
- Become the "LinkedIn for local businesses"
- Marketplace for services
- Job board integration
- Event management
- Customer relationship management (CRM)
- Integrated communication tools

**Geographic Expansion:**
- Full African coverage (10+ countries)
- European markets
- North American markets
- Localized payment gateways
- Regional partnerships

**Technology Advancement:**
- AI/ML for personalization
- Predictive analytics
- Voice assistants integration
- IoT integration (smart NFC devices)
- Quantum-ready security

---

## 17. Appendices

### 17.1 Glossary of Terms

| Term | Definition |
|------|------------|
| **API** | Application Programming Interface - allows software to communicate |
| **JWT** | JSON Web Token - secure token for authentication |
| **Code-First** | Database schema generated from code, not vice versa |
| **ORM** | Object-Relational Mapping - converts database rows to objects |
| **RESTful** | Architectural style for web APIs using HTTP methods |
| **SPA** | Single Page Application - loads once, updates dynamically |
| **CDN** | Content Delivery Network - serves static assets globally |
| **CI/CD** | Continuous Integration/Continuous Deployment - automated pipeline |
| **Slug** | URL-friendly string (e.g., "my-business" from "My Business!") |
| **DTO** | Data Transfer Object - object for transferring data between layers |
| **POPIA** | Protection of Personal Information Act (South African privacy law) |
| **NFC** | Near Field Communication - wireless short-range technology |
| **QR Code** | Quick Response code - 2D barcode scanned by cameras |
| **ITN** | Instant Transaction Notification - PayFast payment callback |
| **SoC** | Separation of Concerns - architectural principle |
| **SOLID** | Five principles of object-oriented design |
| **WCAG** | Web Content Accessibility Guidelines |
| **SSR** | Server-Side Rendering - generates HTML on server |
| **MVP** | Minimum Viable Product - smallest version with core features |
| **MRR** | Monthly Recurring Revenue - subscription revenue per month |
| **NPS** | Net Promoter Score - customer satisfaction metric |
| **CLV** | Customer Lifetime Value - total revenue from a customer |
| **Churn** | Rate at which customers cancel subscriptions |

### 17.2 Acronyms

| Acronym | Expansion |
|---------|-----------|
| ASP | Active Server Pages |
| AWS | Amazon Web Services |
| CORS | Cross-Origin Resource Sharing |
| CRUD | Create, Read, Update, Delete |
| CSS | Cascading Style Sheets |
| DB | Database |
| DI | Dependency Injection |
| DNS | Domain Name System |
| EF | Entity Framework |
| GDPR | General Data Protection Regulation |
| HTML | HyperText Markup Language |
| HTTP | HyperText Transfer Protocol |
| HTTPS | HTTP Secure |
| IP | Internet Protocol |
| JSON | JavaScript Object Notation |
| MVC | Model-View-Controller |
| ORM | Object-Relational Mapping |
| RBAC | Role-Based Access Control |
| REST | Representational State Transfer |
| SaaS | Software as a Service |
| SDK | Software Development Kit |
| SEO | Search Engine Optimization |
| SKU | Stock Keeping Unit |
| SQL | Structured Query Language |
| SSL | Secure Sockets Layer |
| TLS | Transport Layer Security |
| UI | User Interface |
| URL | Uniform Resource Locator |
| UX | User Experience |
| VPS | Virtual Private Server |
| YAML | YAML Ain't Markup Language |

### 17.3 References & Resources

**Technical Documentation:**
- Vue.js Documentation: https://vuejs.org/guide/
- ASP.NET Core Documentation: https://docs.microsoft.com/aspnet/core
- Entity Framework Core: https://docs.microsoft.com/ef/core
- Tailwind CSS: https://tailwindcss.com/docs
- MySQL Documentation: https://dev.mysql.com/doc/

**Best Practices:**
- RESTful API Design: https://restfulapi.net/
- OWASP Security Guidelines: https://owasp.org/
- Vue.js Style Guide: https://vuejs.org/style-guide/
- C# Coding Conventions: https://docs.microsoft.com/dotnet/csharp/

**Third-Party Services:**
- PayFast Documentation: https://developers.payfast.co.za/
- SendGrid Documentation: https://docs.sendgrid.com/
- CloudFlare Documentation: https://developers.cloudflare.com/

### 17.4 Document Approval

| Role | Name | Signature | Date |
|------|------|-----------|------|
| Product Owner | | | |
| Technical Architect | | | |
| Business Analyst | | | |
| Development Lead | | | |
| QA Lead | | | |
| Stakeholder | | | |

---

**End of Software Design Document**

**Version:** 1.0  
**Date:** November 2025  
**Status:** Approved for Development  
**Next Review:** December 2025

---

*This Software Design Document provides a comprehensive overview of the BizBio platform design, architecture, and implementation strategy. It should be used in conjunction with the Functional Requirements Document (FRD) and Technical Specification Document for complete project understanding.*