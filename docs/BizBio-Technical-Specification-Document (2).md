# BizBio Platform - Technical Specification Document v2.0

**Version:** 2.0  
**Date:** November 2025  
**Status:** Ready for Development  
**Author:** System Architect

---

## Document Control

| Version | Date | Author | Changes |
|---------|------|--------|---------|
| 1.0 | 2025-11-07 | System Architect | Initial technical specification |
| 2.0 | 2025-11-08 | System Architect | Updated: Vue.js frontend, ASP.NET API backend, Integer IDs |

---

## Table of Contents

1. [Executive Summary](#1-executive-summary)
2. [System Overview](#2-system-overview)
3. [Technical Architecture](#3-technical-architecture)
4. [Technology Stack](#4-technology-stack)
5. [Frontend Architecture (Vue.js)](#5-frontend-architecture-vuejs)
6. [Backend Architecture (ASP.NET API)](#6-backend-architecture-aspnet-api)
7. [Database Design (Code-First)](#7-database-design-code-first)
8. [API Specifications](#8-api-specifications)
9. [Security Architecture](#9-security-architecture)
10. [Performance Requirements](#10-performance-requirements)
11. [Infrastructure & Deployment](#11-infrastructure--deployment)
12. [Integration Specifications](#12-integration-specifications)
13. [Testing Strategy](#13-testing-strategy)
14. [Development Phases](#14-development-phases)
15. [Technical Constraints](#15-technical-constraints)
16. [Appendices](#16-appendices)

---

## 1. Executive Summary

### 1.1 Project Overview
BizBio is a SaaS platform that provides digital business cards and profile management for businesses and professionals. Version 2.0 introduces a modern, decoupled architecture with a Vue.js frontend and ASP.NET RESTful API backend, designed for scalability, maintainability, and future public API exposure.

### 1.2 Technical Objectives
- Build a scalable, decoupled SaaS application
- Implement modern frontend with Vue.js 3 and Tailwind CSS
- Create RESTful API ready for future public exposure
- Support 10,000+ concurrent users
- Provide sub-200ms API response times
- Ensure 99.9% uptime
- Enable seamless NFC integration
- Support mobile-first responsive design

### 1.3 Key Deliverables
- Vue.js 3 Single Page Application (SPA) with Tailwind CSS
- ASP.NET 6 RESTful Web API
- MySQL database with Code-First Entity Framework Core
- JWT-based authentication system
- Payment gateway integration (PayFast)
- Analytics dashboard
- Admin portal
- Public profile pages (SSR/SEO-optimized)
- NFC tag management system
- Comprehensive API documentation (Swagger)

### 1.4 Architecture Philosophy
- **Separation of Concerns:** Frontend and backend are completely decoupled
- **API-First Design:** All functionality accessible via RESTful API
- **Mobile-First:** Responsive design prioritizing mobile experience
- **Scalability:** Horizontal scaling of API and frontend independently
- **Future-Ready:** Architecture supports future public API exposure with OAuth 2.0

---

## 2. System Overview

### 2.1 High-Level Architecture

```
┌─────────────────────────────────────────────────────────────────┐
│                       External Systems                           │
├─────────────────────────────────────────────────────────────────┤
│  PayFast  │  Google Maps  │  Email Service  │  Cloud Storage    │
└─────────────────────────────────────────────────────────────────┘
                              ↕
┌─────────────────────────────────────────────────────────────────┐
│                    BizBio Frontend (Vue.js)                      │
├─────────────────────────────────────────────────────────────────┤
│  Vue.js 3 SPA  │  Tailwind CSS  │  Pinia State  │  Vue Router  │
│  Public Views  │  Dashboard     │  Admin Portal │  Mobile-First │
└─────────────────────────────────────────────────────────────────┘
                              ↕
                        HTTPS/REST API
                              ↕
┌─────────────────────────────────────────────────────────────────┐
│                BizBio Backend (ASP.NET 6 API)                    │
├─────────────────────────────────────────────────────────────────┤
│  Controllers  │  Services  │  Repositories  │  Middleware       │
│  JWT Auth     │  Validators│  DTOs         │  AutoMapper        │
└─────────────────────────────────────────────────────────────────┘
                              ↕
┌─────────────────────────────────────────────────────────────────┐
│            Entity Framework Core (Code-First)                    │
├─────────────────────────────────────────────────────────────────┤
│  DbContext  │  Entities  │  Migrations  │  Integer IDs         │
└─────────────────────────────────────────────────────────────────┘
                              ↕
┌─────────────────────────────────────────────────────────────────┐
│                    MySQL 8.0+ Database                           │
├─────────────────────────────────────────────────────────────────┤
│  29 Tables  │  Relationships  │  Indexes  │  Constraints        │
└─────────────────────────────────────────────────────────────────┘
```

### 2.2 System Context Diagram

```
┌───────────────────────────────────────────────────────────────┐
│                        End Users                               │
│  ┌──────────────┐  ┌──────────────┐  ┌──────────────┐       │
│  │   Mobile     │  │   Desktop    │  │   Tablet     │       │
│  │   Browser    │  │   Browser    │  │   Browser    │       │
│  └──────────────┘  └──────────────┘  └──────────────┘       │
└───────────────────────────────────────────────────────────────┘
                              ↓
┌───────────────────────────────────────────────────────────────┐
│                     CloudFlare CDN                             │
│                  (Static Assets & Caching)                     │
└───────────────────────────────────────────────────────────────┘
                              ↓
┌───────────────────────────────────────────────────────────────┐
│                   Vue.js Frontend (SPA)                        │
│  ┌──────────────────────────────────────────────────────────┐│
│  │  Public Routes │ Auth Routes │ Dashboard │ Admin Panel   ││
│  └──────────────────────────────────────────────────────────┘│
└───────────────────────────────────────────────────────────────┘
                              ↓ REST API (JSON)
┌───────────────────────────────────────────────────────────────┐
│                ASP.NET 6 Web API (Backend)                     │
│  ┌──────────────────────────────────────────────────────────┐│
│  │  Auth API │ Profile API │ Catalog API │ Payment API      ││
│  └──────────────────────────────────────────────────────────┘│
└───────────────────────────────────────────────────────────────┘
                              ↓
        ┌────────────────────┼────────────────────┐
        ↓                    ↓                    ↓
┌───────────────┐   ┌────────────────┐   ┌───────────────┐
│  MySQL DB     │   │  Redis Cache   │   │  File Storage │
│  (Primary)    │   │  (Session)     │   │  (S3/Azure)   │
└───────────────┘   └────────────────┘   └───────────────┘
```

### 2.3 Component Architecture

**Frontend Components:**
```
src/
├── assets/                 # Static assets
├── components/             # Reusable Vue components
│   ├── common/            # Common UI components
│   ├── profile/           # Profile-specific components
│   ├── catalog/           # Catalog components
│   └── admin/             # Admin components
├── views/                 # Page components
│   ├── public/            # Public-facing views
│   ├── auth/              # Authentication views
│   ├── dashboard/         # User dashboard views
│   └── admin/             # Admin panel views
├── composables/           # Vue composables (reusable logic)
├── stores/                # Pinia stores (state management)
├── services/              # API service layer
├── router/                # Vue Router configuration
├── utils/                 # Utility functions
└── types/                 # TypeScript type definitions
```

**Backend Structure:**
```
BizBio.API/
├── Controllers/           # API Controllers
├── Services/              # Business logic
├── Repositories/          # Data access layer
├── Models/
│   ├── Entities/         # EF Core entities (Code-First)
│   ├── DTOs/             # Data Transfer Objects
│   └── ViewModels/       # Response models
├── Middleware/            # Custom middleware
├── Validators/            # Input validation
├── Helpers/               # Utility classes
└── Extensions/            # Extension methods

BizBio.Core/               # Core domain logic (optional)
BizBio.Infrastructure/     # Infrastructure concerns (optional)
```

---

## 3. Technical Architecture

### 3.1 Architecture Pattern

The system follows a **Clean Architecture** approach with clear separation between layers:

```
┌─────────────────────────────────────────────────────────────┐
│                  Presentation Layer (Vue.js)                 │
│  ┌───────────────────────────────────────────────────────┐  │
│  │  Vue Components │ Tailwind UI │ Pinia Stores │ Router │  │
│  └───────────────────────────────────────────────────────┘  │
└─────────────────────────────────────────────────────────────┘
                            ↓ HTTP/REST
┌─────────────────────────────────────────────────────────────┐
│                    API Layer (Controllers)                   │
│  ┌───────────────────────────────────────────────────────┐  │
│  │  JWT Auth │ Request Validation │ Response Formatting  │  │
│  └───────────────────────────────────────────────────────┘  │
└─────────────────────────────────────────────────────────────┘
                            ↓
┌─────────────────────────────────────────────────────────────┐
│               Business Logic Layer (Services)                │
│  ┌───────────────────────────────────────────────────────┐  │
│  │  Domain Logic │ Validation │ Authorization │ Mapping  │  │
│  └───────────────────────────────────────────────────────┘  │
└─────────────────────────────────────────────────────────────┘
                            ↓
┌─────────────────────────────────────────────────────────────┐
│              Data Access Layer (Repositories)                │
│  ┌───────────────────────────────────────────────────────┐  │
│  │  EF Core │ DbContext │ Queries │ Transactions        │  │
│  └───────────────────────────────────────────────────────┘  │
└─────────────────────────────────────────────────────────────┘
                            ↓
┌─────────────────────────────────────────────────────────────┐
│                    Database Layer (MySQL)                    │
│                  Integer-based Primary Keys                  │
└─────────────────────────────────────────────────────────────┘
```

### 3.2 API Request Flow

```
Client (Vue.js)
    ↓
1. HTTP Request (with JWT in Authorization header)
    ↓
2. API Gateway / Load Balancer (future)
    ↓
3. ASP.NET Core Middleware Pipeline
    │
    ├→ Authentication Middleware (JWT validation)
    │
    ├→ Authorization Middleware (role/permission check)
    │
    ├→ Rate Limiting Middleware
    │
    ├→ Request Logging Middleware
    ↓
4. Controller Action
    │
    ├→ Model Validation
    │
    ├→ DTO Mapping
    ↓
5. Service Layer
    │
    ├→ Business Logic Execution
    │
    ├→ Authorization Checks
    │
    ├→ Data Validation
    ↓
6. Repository Layer
    │
    ├→ EF Core Query
    │
    ├→ Database Access
    ↓
7. MySQL Database
    ↓
8. Response Flow (reverse)
    │
    ├→ Entity to DTO Mapping
    │
    ├→ Response Formatting
    │
    ├→ Error Handling
    ↓
9. JSON Response to Client
```

### 3.3 Frontend State Management

```
┌─────────────────────────────────────────────────────────────┐
│                     Pinia Stores                             │
├─────────────────────────────────────────────────────────────┤
│                                                              │
│  ┌──────────────┐  ┌──────────────┐  ┌──────────────┐     │
│  │  Auth Store  │  │ Profile Store│  │ Catalog Store│     │
│  │              │  │              │  │              │     │
│  │ - user       │  │ - profiles   │  │ - catalogs   │     │
│  │ - token      │  │ - selected   │  │ - items      │     │
│  │ - isAuth     │  │ - loading    │  │ - categories │     │
│  └──────────────┘  └──────────────┘  └──────────────┘     │
│                                                              │
│  ┌──────────────┐  ┌──────────────┐  ┌──────────────┐     │
│  │ Subscription │  │ Analytics    │  │  UI Store    │     │
│  │    Store     │  │    Store     │  │              │     │
│  │              │  │              │  │ - loading    │     │
│  │ - current    │  │ - data       │  │ - modals     │     │
│  │ - tiers      │  │ - views      │  │ - toast      │     │
│  └──────────────┘  └──────────────┘  └──────────────┘     │
│                                                              │
└─────────────────────────────────────────────────────────────┘
```

---

## 4. Technology Stack

### 4.1 Frontend Technologies

| Component | Technology | Version | Purpose |
|-----------|-----------|---------|---------|
| Framework | Vue.js | 3.3+ | Progressive JavaScript framework |
| Language | TypeScript | 5.0+ | Type-safe JavaScript |
| UI Framework | Tailwind CSS | 3.3+ | Utility-first CSS framework |
| State Management | Pinia | 2.1+ | Vue state management |
| Routing | Vue Router | 4.2+ | Client-side routing |
| HTTP Client | Axios | 1.5+ | API communication |
| Build Tool | Vite | 4.4+ | Fast build tool and dev server |
| Forms | VeeValidate | 4.11+ | Form validation |
| Charts | Chart.js | 4.4+ | Data visualization |
| Icons | Heroicons | 2.0+ | SVG icon library |
| Date/Time | date-fns | 2.30+ | Date manipulation |
| Rich Text | Tiptap | 2.1+ | Rich text editor (optional) |

### 4.2 Backend Technologies

| Component | Technology | Version | Purpose |
|-----------|-----------|---------|---------|
| Framework | ASP.NET Core | 6.0 LTS | Web API framework |
| Language | C# | 10.0 | Primary programming language |
| ORM | Entity Framework Core | 6.0 | Database access (Code-First) |
| Database | MySQL | 8.0+ | Relational database |
| Authentication | JWT Bearer | 6.0 | Token-based authentication |
| API Documentation | Swashbuckle (Swagger) | 6.5+ | API documentation |
| Validation | FluentValidation | 11.7+ | Object validation |
| Mapping | AutoMapper | 12.0+ | Object-to-object mapping |
| Logging | Serilog | 3.0+ | Structured logging |
| Caching | Redis | 7.0+ | Distributed caching |
| Testing | xUnit | 2.5+ | Unit testing framework |
| API Testing | RestSharp | 109+ | Integration testing |

### 4.3 Database Technologies

| Component | Technology | Version | Purpose |
|-----------|-----------|---------|---------|
| Database | MySQL | 8.0+ | Primary data store |
| ORM | Entity Framework Core | 6.0 | Code-First database design |
| Migration Tool | EF Core Migrations | 6.0 | Database version control |
| Connection Pool | Pomelo.EntityFrameworkCore.MySql | 6.0+ | MySQL provider for EF Core |
| Backup | mysqldump | 8.0+ | Database backup utility |

### 4.4 DevOps & Infrastructure

| Component | Technology | Version | Purpose |
|-----------|-----------|---------|---------|
| Version Control | Git | - | Source code management |
| Repository | GitHub/GitLab | - | Code hosting |
| CI/CD | GitHub Actions | - | Automated deployment |
| Frontend Hosting | Vercel / Netlify | - | Static site hosting |
| API Hosting | Azure / AWS / DigitalOcean | - | API server hosting |
| Database Hosting | Managed MySQL | - | Database hosting |
| File Storage | AWS S3 / Azure Blob | - | File and media storage |
| CDN | CloudFlare | - | Content delivery |
| SSL | Let's Encrypt | - | HTTPS certificates |
| Monitoring | Application Insights | - | Application monitoring |

### 4.5 Third-Party Services

| Service | Purpose | Provider |
|---------|---------|----------|
| Payment Processing | Subscription payments | PayFast |
| Email Delivery | Transactional emails | SendGrid / Mailgun |
| File Storage | Media and document storage | AWS S3 / Azure Blob |
| Maps | Location services | Google Maps API |
| Analytics | Usage analytics | Mixpanel / Google Analytics |
| Error Tracking | Error monitoring | Sentry |
| Uptime Monitoring | Server monitoring | Pingdom / UptimeRobot |

---

## 5. Frontend Architecture (Vue.js)

### 5.1 Project Structure

```
bizbio-frontend/
├── public/                      # Static files
│   ├── favicon.ico
│   └── robots.txt
│
├── src/
│   ├── assets/                  # Images, fonts, etc.
│   │   ├── images/
│   │   └── fonts/
│   │
│   ├── components/              # Reusable components
│   │   ├── common/
│   │   │   ├── AppButton.vue
│   │   │   ├── AppInput.vue
│   │   │   ├── AppModal.vue
│   │   │   ├── AppCard.vue
│   │   │   ├── AppTable.vue
│   │   │   └── Loading.vue
│   │   │
│   │   ├── layout/
│   │   │   ├── AppHeader.vue
│   │   │   ├── AppFooter.vue
│   │   │   ├── AppSidebar.vue
│   │   │   └── DashboardLayout.vue
│   │   │
│   │   ├── profile/
│   │   │   ├── ProfileCard.vue
│   │   │   ├── ProfileEditor.vue
│   │   │   ├── ProfilePreview.vue
│   │   │   ├── ContactForm.vue
│   │   │   ├── SocialLinks.vue
│   │   │   └── BrandingEditor.vue
│   │   │
│   │   ├── catalog/
│   │   │   ├── CatalogView.vue
│   │   │   ├── CategoryList.vue
│   │   │   ├── ItemCard.vue
│   │   │   ├── ItemEditor.vue
│   │   │   └── VariantSelector.vue
│   │   │
│   │   └── admin/
│   │       ├── UserList.vue
│   │       ├── SubscriptionManager.vue
│   │       └── AnalyticsDashboard.vue
│   │
│   ├── composables/             # Reusable composition functions
│   │   ├── useAuth.ts
│   │   ├── useProfile.ts
│   │   ├── useCatalog.ts
│   │   ├── useAnalytics.ts
│   │   ├── useSubscription.ts
│   │   └── useToast.ts
│   │
│   ├── views/                   # Page components
│   │   ├── public/
│   │   │   ├── Home.vue
│   │   │   ├── ProfileView.vue
│   │   │   ├── CatalogView.vue
│   │   │   └── Pricing.vue
│   │   │
│   │   ├── auth/
│   │   │   ├── Login.vue
│   │   │   ├── Register.vue
│   │   │   ├── ForgotPassword.vue
│   │   │   └── VerifyEmail.vue
│   │   │
│   │   ├── dashboard/
│   │   │   ├── Dashboard.vue
│   │   │   ├── Profiles.vue
│   │   │   ├── CreateProfile.vue
│   │   │   ├── EditProfile.vue
│   │   │   ├── ManageCatalog.vue
│   │   │   ├── Analytics.vue
│   │   │   ├── Subscription.vue
│   │   │   └── Settings.vue
│   │   │
│   │   └── admin/
│   │       ├── AdminDashboard.vue
│   │       ├── Users.vue
│   │       ├── Subscriptions.vue
│   │       └── Reports.vue
│   │
│   ├── stores/                  # Pinia stores
│   │   ├── auth.ts
│   │   ├── profile.ts
│   │   ├── catalog.ts
│   │   ├── subscription.ts
│   │   ├── analytics.ts
│   │   └── ui.ts
│   │
│   ├── services/                # API services
│   │   ├── api.ts              # Axios instance
│   │   ├── auth.service.ts
│   │   ├── profile.service.ts
│   │   ├── catalog.service.ts
│   │   ├── subscription.service.ts
│   │   ├── payment.service.ts
│   │   └── analytics.service.ts
│   │
│   ├── router/                  # Vue Router
│   │   ├── index.ts
│   │   └── guards.ts
│   │
│   ├── utils/                   # Utility functions
│   │   ├── validators.ts
│   │   ├── formatters.ts
│   │   ├── helpers.ts
│   │   └── constants.ts
│   │
│   ├── types/                   # TypeScript types
│   │   ├── api.types.ts
│   │   ├── profile.types.ts
│   │   ├── catalog.types.ts
│   │   └── user.types.ts
│   │
│   ├── App.vue                  # Root component
│   └── main.ts                  # Application entry point
│
├── .env                         # Environment variables
├── .env.production
├── tailwind.config.js           # Tailwind configuration
├── vite.config.ts               # Vite configuration
├── tsconfig.json                # TypeScript configuration
└── package.json
```

### 5.2 Key Frontend Components

#### 5.2.1 Authentication Flow

```typescript
// src/composables/useAuth.ts
export function useAuth() {
  const authStore = useAuthStore()
  const router = useRouter()

  const login = async (credentials: LoginCredentials) => {
    try {
      const response = await authService.login(credentials)
      authStore.setUser(response.user)
      authStore.setToken(response.accessToken)
      router.push('/dashboard')
    } catch (error) {
      throw error
    }
  }

  const logout = async () => {
    authStore.clearAuth()
    router.push('/login')
  }

  return { login, logout }
}
```

#### 5.2.2 API Service Layer

```typescript
// src/services/api.ts
import axios from 'axios'

const api = axios.create({
  baseURL: import.meta.env.VITE_API_BASE_URL,
  timeout: 10000,
  headers: {
    'Content-Type': 'application/json',
  },
})

// Request interceptor (add JWT token)
api.interceptors.request.use(
  (config) => {
    const token = localStorage.getItem('accessToken')
    if (token) {
      config.headers.Authorization = `Bearer ${token}`
    }
    return config
  },
  (error) => Promise.reject(error)
)

// Response interceptor (handle errors)
api.interceptors.response.use(
  (response) => response,
  async (error) => {
    if (error.response?.status === 401) {
      // Token expired - attempt refresh or logout
      await refreshToken()
    }
    return Promise.reject(error)
  }
)

export default api
```

#### 5.2.3 Profile Service

```typescript
// src/services/profile.service.ts
import api from './api'
import { Profile, CreateProfileDTO } from '@/types/profile.types'

export const profileService = {
  getProfiles: async (): Promise<Profile[]> => {
    const response = await api.get('/api/v1/profiles')
    return response.data
  },

  getProfileById: async (id: number): Promise<Profile> => {
    const response = await api.get(`/api/v1/profiles/${id}`)
    return response.data
  },

  getProfileBySlug: async (slug: string): Promise<Profile> => {
    const response = await api.get(`/api/v1/profiles/by-slug/${slug}`)
    return response.data
  },

  createProfile: async (data: CreateProfileDTO): Promise<Profile> => {
    const response = await api.post('/api/v1/profiles', data)
    return response.data.profile
  },

  updateProfile: async (id: number, data: Partial<Profile>): Promise<Profile> => {
    const response = await api.put(`/api/v1/profiles/${id}`, data)
    return response.data.profile
  },

  deleteProfile: async (id: number): Promise<void> => {
    await api.delete(`/api/v1/profiles/${id}`)
  },

  updateBranding: async (id: number, branding: any): Promise<void> => {
    await api.put(`/api/v1/profiles/${id}/branding`, branding)
  },
}
```

### 5.3 Tailwind CSS Configuration

```javascript
// tailwind.config.js
module.exports = {
  content: [
    "./index.html",
    "./src/**/*.{vue,js,ts,jsx,tsx}",
  ],
  theme: {
    extend: {
      colors: {
        primary: {
          50: '#eff6ff',
          100: '#dbeafe',
          // ... custom brand colors
          900: '#1e3a8a',
        },
      },
      fontFamily: {
        sans: ['Inter', 'system-ui', 'sans-serif'],
      },
    },
  },
  plugins: [
    require('@tailwindcss/forms'),
    require('@tailwindcss/typography'),
    require('@tailwindcss/aspect-ratio'),
  ],
}
```

### 5.4 Vue Router Configuration

```typescript
// src/router/index.ts
import { createRouter, createWebHistory } from 'vue-router'
import { useAuthStore } from '@/stores/auth'

const routes = [
  // Public routes
  {
    path: '/',
    name: 'Home',
    component: () => import('@/views/public/Home.vue'),
  },
  {
    path: '/:slug',
    name: 'ProfileView',
    component: () => import('@/views/public/ProfileView.vue'),
  },
  {
    path: '/c/:slug',
    name: 'CatalogView',
    component: () => import('@/views/public/CatalogView.vue'),
  },

  // Auth routes
  {
    path: '/login',
    name: 'Login',
    component: () => import('@/views/auth/Login.vue'),
  },
  {
    path: '/register',
    name: 'Register',
    component: () => import('@/views/auth/Register.vue'),
  },

  // Protected routes
  {
    path: '/dashboard',
    name: 'Dashboard',
    component: () => import('@/views/dashboard/Dashboard.vue'),
    meta: { requiresAuth: true },
  },
  {
    path: '/manage/profile/:id',
    name: 'EditProfile',
    component: () => import('@/views/dashboard/EditProfile.vue'),
    meta: { requiresAuth: true },
  },

  // Admin routes
  {
    path: '/admin',
    name: 'AdminDashboard',
    component: () => import('@/views/admin/AdminDashboard.vue'),
    meta: { requiresAuth: true, requiresAdmin: true },
  },
]

const router = createRouter({
  history: createWebHistory(),
  routes,
})

// Navigation guard
router.beforeEach((to, from, next) => {
  const authStore = useAuthStore()

  if (to.meta.requiresAuth && !authStore.isAuthenticated) {
    next('/login')
  } else if (to.meta.requiresAdmin && !authStore.isAdmin) {
    next('/dashboard')
  } else {
    next()
  }
})

export default router
```

### 5.5 Responsive Design with Tailwind

All components will be built with mobile-first responsive design:

```vue
<!-- Example responsive profile card -->
<template>
  <div class="
    w-full 
    p-4 md:p-6 
    bg-white 
    rounded-lg 
    shadow-lg 
    max-w-sm md:max-w-2xl 
    mx-auto
  ">
    <div class="
      flex 
      flex-col md:flex-row 
      items-center md:items-start 
      gap-4
    ">
      <!-- Profile image -->
      <img 
        :src="profile.photoUrl" 
        class="
          w-24 h-24 md:w-32 md:h-32 
          rounded-full 
          object-cover
        "
      />
      
      <!-- Profile info -->
      <div class="flex-1 text-center md:text-left">
        <h2 class="text-2xl md:text-3xl font-bold">
          {{ profile.displayName }}
        </h2>
        <p class="text-gray-600 mt-2">
          {{ profile.bio }}
        </p>
      </div>
    </div>
  </div>
</template>
```

---

## 6. Backend Architecture (ASP.NET API)

### 6.1 Project Structure

```
BizBio.API/
├── Controllers/                 # API Controllers
│   ├── AuthController.cs
│   ├── ProfilesController.cs
│   ├── CatalogsController.cs
│   ├── SubscriptionsController.cs
│   ├── PaymentsController.cs
│   ├── AnalyticsController.cs
│   └── AdminController.cs
│
├── Services/                    # Business Logic
│   ├── Interfaces/
│   │   ├── IAuthService.cs
│   │   ├── IProfileService.cs
│   │   ├── ICatalogService.cs
│   │   └── ...
│   │
│   └── Implementation/
│       ├── AuthService.cs
│       ├── ProfileService.cs
│       ├── CatalogService.cs
│       ├── SubscriptionService.cs
│       ├── PaymentService.cs
│       ├── AnalyticsService.cs
│       └── EmailService.cs
│
├── Repositories/                # Data Access
│   ├── Interfaces/
│   │   ├── IUserRepository.cs
│   │   ├── IProfileRepository.cs
│   │   └── ...
│   │
│   └── Implementation/
│       ├── UserRepository.cs
│       ├── ProfileRepository.cs
│       ├── CatalogRepository.cs
│       └── GenericRepository.cs
│
├── Data/                        # Database Context
│   ├── BizBioDbContext.cs      # Main DbContext
│   ├── Entities/               # EF Core entities (Code-First)
│   │   ├── User.cs
│   │   ├── Profile.cs
│   │   ├── Catalog.cs
│   │   ├── CatalogItem.cs
│   │   ├── Subscription.cs
│   │   ├── Payment.cs
│   │   └── ...
│   │
│   └── Configurations/         # Entity configurations
│       ├── UserConfiguration.cs
│       ├── ProfileConfiguration.cs
│       └── ...
│
├── Models/                      # DTOs and ViewModels
│   ├── DTOs/
│   │   ├── Auth/
│   │   │   ├── LoginDTO.cs
│   │   │   ├── RegisterDTO.cs
│   │   │   └── TokenResponseDTO.cs
│   │   │
│   │   ├── Profile/
│   │   │   ├── CreateProfileDTO.cs
│   │   │   ├── UpdateProfileDTO.cs
│   │   │   └── ProfileResponseDTO.cs
│   │   │
│   │   └── Catalog/
│   │       └── ...
│   │
│   └── Responses/
│       ├── ApiResponse.cs
│       ├── PaginatedResponse.cs
│       └── ErrorResponse.cs
│
├── Middleware/                  # Custom Middleware
│   ├── JwtMiddleware.cs
│   ├── ErrorHandlingMiddleware.cs
│   ├── RateLimitingMiddleware.cs
│   └── RequestLoggingMiddleware.cs
│
├── Validators/                  # FluentValidation
│   ├── RegisterValidator.cs
│   ├── CreateProfileValidator.cs
│   └── ...
│
├── Helpers/                     # Utility Classes
│   ├── JwtHelper.cs
│   ├── PasswordHelper.cs
│   ├── SlugHelper.cs
│   └── FileHelper.cs
│
├── Extensions/                  # Extension Methods
│   ├── ServiceExtensions.cs
│   ├── ApplicationBuilderExtensions.cs
│   └── ClaimsPrincipalExtensions.cs
│
├── Migrations/                  # EF Core Migrations
│   └── (auto-generated)
│
├── appsettings.json
├── appsettings.Development.json
├── appsettings.Production.json
└── Program.cs
```

### 6.2 Database Context (Code-First)

```csharp
// Data/BizBioDbContext.cs
using Microsoft.EntityFrameworkCore;
using BizBio.API.Data.Entities;

namespace BizBio.API.Data
{
    public class BizBioDbContext : DbContext
    {
        public BizBioDbContext(DbContextOptions<BizBioDbContext> options)
            : base(options)
        {
        }

        // DbSets with INTEGER primary keys
        public DbSet<User> Users { get; set; }
        public DbSet<Profile> Profiles { get; set; }
        public DbSet<ProfileBranding> ProfileBrandings { get; set; }
        public DbSet<ProfileRelationship> ProfileRelationships { get; set; }
        public DbSet<Catalog> Catalogs { get; set; }
        public DbSet<CatalogCategory> CatalogCategories { get; set; }
        public DbSet<CatalogItem> CatalogItems { get; set; }
        public DbSet<CatalogItemVariant> CatalogItemVariants { get; set; }
        public DbSet<CatalogItemAddon> CatalogItemAddons { get; set; }
        public DbSet<SocialMediaLink> SocialMediaLinks { get; set; }
        public DbSet<ProfilePhoto> ProfilePhotos { get; set; }
        public DbSet<ProfileDocument> ProfileDocuments { get; set; }
        public DbSet<NFCTag> NFCTags { get; set; }
        public DbSet<NFCOrder> NFCOrders { get; set; }
        public DbSet<Subscription> Subscriptions { get; set; }
        public DbSet<SubscriptionTier> SubscriptionTiers { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<ProfileAnalytic> ProfileAnalytics { get; set; }
        public DbSet<CatalogAnalytic> CatalogAnalytics { get; set; }
        public DbSet<SupportTicket> SupportTickets { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Apply configurations
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(BizBioDbContext).Assembly);

            // Configure decimal precision for prices
            foreach (var property in modelBuilder.Model.GetEntityTypes()
                .SelectMany(t => t.GetProperties())
                .Where(p => p.ClrType == typeof(decimal) || p.ClrType == typeof(decimal?)))
            {
                property.SetColumnType("decimal(18,2)");
            }

            // Soft delete global query filter
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                if (typeof(ISoftDelete).IsAssignableFrom(entityType.ClrType))
                {
                    modelBuilder
                        .Entity(entityType.ClrType)
                        .HasQueryFilter(GetSoftDeleteFilter(entityType.ClrType));
                }
            }
        }

        private LambdaExpression GetSoftDeleteFilter(Type type)
        {
            var parameter = Expression.Parameter(type, "e");
            var property = Expression.Property(parameter, "IsDeleted");
            var condition = Expression.Equal(property, Expression.Constant(false));
            return Expression.Lambda(condition, parameter);
        }
    }

    public interface ISoftDelete
    {
        bool IsDeleted { get; set; }
        DateTime? DeletedAt { get; set; }
    }
}
```

### 6.3 Entity Examples (Code-First with Integer IDs)

```csharp
// Data/Entities/User.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BizBio.API.Data.Entities
{
    [Table("Users")]
    public class User : ISoftDelete
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [StringLength(255)]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [StringLength(255)]
        public string PasswordHash { get; set; }

        [Required]
        [StringLength(100)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(100)]
        public string LastName { get; set; }

        [StringLength(20)]
        public string? PhoneNumber { get; set; }

        [Required]
        [StringLength(50)]
        public string Role { get; set; } = "User";

        public bool IsEmailVerified { get; set; } = false;

        public string? EmailVerificationToken { get; set; }

        public DateTime? EmailVerifiedAt { get; set; }

        public int FailedLoginAttempts { get; set; } = 0;

        public DateTime? LockedUntil { get; set; }

        public DateTime? LastLoginAt { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        public bool IsDeleted { get; set; } = false;

        public DateTime? DeletedAt { get; set; }

        // Navigation properties
        public virtual ICollection<Profile> Profiles { get; set; }
        public virtual Subscription? Subscription { get; set; }
        public virtual ICollection<Payment> Payments { get; set; }
    }
}
```

```csharp
// Data/Entities/Profile.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BizBio.API.Data.Entities
{
    [Table("Profiles")]
    public class Profile : ISoftDelete
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        [StringLength(50)]
        public string ProfileType { get; set; } // Personal, Business, Organization

        [Required]
        [StringLength(200)]
        public string DisplayName { get; set; }

        [Required]
        [StringLength(255)]
        public string Slug { get; set; } // URL-friendly name

        [StringLength(1000)]
        public string? Bio { get; set; }

        [StringLength(100)]
        public string? BusinessCategory { get; set; }

        public int? TemplateId { get; set; }

        // Contact Information
        [StringLength(255)]
        [EmailAddress]
        public string? Email { get; set; }

        [StringLength(20)]
        public string? Phone { get; set; }

        [StringLength(255)]
        public string? Website { get; set; }

        [StringLength(20)]
        public string? WhatsApp { get; set; }

        // Address
        [StringLength(255)]
        public string? Street { get; set; }

        [StringLength(100)]
        public string? City { get; set; }

        [StringLength(100)]
        public string? Province { get; set; }

        [StringLength(100)]
        public string? Country { get; set; }

        [StringLength(20)]
        public string? PostalCode { get; set; }

        [Column(TypeName = "decimal(10,7)")]
        public decimal? Latitude { get; set; }

        [Column(TypeName = "decimal(10,7)")]
        public decimal? Longitude { get; set; }

        // Media
        [StringLength(500)]
        public string? PhotoUrl { get; set; }

        [StringLength(500)]
        public string? CoverImageUrl { get; set; }

        [StringLength(500)]
        public string? LogoUrl { get; set; }

        // Settings
        public bool IsPublic { get; set; } = true;

        public bool IsSearchable { get; set; } = true;

        public bool ShowEmail { get; set; } = true;

        public bool ShowPhone { get; set; } = true;

        // Metrics
        public int ViewCount { get; set; } = 0;

        public DateTime? PublishedAt { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        public bool IsDeleted { get; set; } = false;

        public DateTime? DeletedAt { get; set; }

        // Navigation properties
        [ForeignKey(nameof(UserId))]
        public virtual User User { get; set; }

        public virtual ProfileBranding? Branding { get; set; }

        public virtual ICollection<SocialMediaLink> SocialLinks { get; set; }

        public virtual ICollection<ProfilePhoto> Photos { get; set; }

        public virtual ICollection<ProfileDocument> Documents { get; set; }

        public virtual Catalog? Catalog { get; set; }

        public virtual ICollection<NFCTag> NFCTags { get; set; }

        // Organization relationships
        public virtual ICollection<ProfileRelationship> ParentRelationships { get; set; }
        public virtual ICollection<ProfileRelationship> ChildRelationships { get; set; }
    }
}
```

```csharp
// Data/Entities/Catalog.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BizBio.API.Data.Entities
{
    [Table("Catalogs")]
    public class Catalog
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int ProfileId { get; set; }

        [Required]
        [StringLength(50)]
        public string CatalogType { get; set; } // Menu, Products, Services

        [Required]
        [StringLength(200)]
        public string Name { get; set; }

        [StringLength(1000)]
        public string? Description { get; set; }

        [Required]
        [StringLength(3)]
        public string Currency { get; set; } = "ZAR";

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        // Navigation properties
        [ForeignKey(nameof(ProfileId))]
        public virtual Profile Profile { get; set; }

        public virtual ICollection<CatalogCategory> Categories { get; set; }

        public virtual ICollection<CatalogItem> Items { get; set; }
    }
}
```

```csharp
// Data/Entities/CatalogItem.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BizBio.API.Data.Entities
{
    [Table("CatalogItems")]
    public class CatalogItem : ISoftDelete
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int CatalogId { get; set; }

        [Required]
        public int CategoryId { get; set; }

        [Required]
        [StringLength(200)]
        public string Name { get; set; }

        [StringLength(2000)]
        public string? Description { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? CompareAtPrice { get; set; }

        [StringLength(100)]
        public string? SKU { get; set; }

        public bool IsAvailable { get; set; } = true;

        [StringLength(500)]
        public string? ImageUrl { get; set; }

        [StringLength(500)]
        public string? Allergens { get; set; }

        [StringLength(1000)]
        public string? Tags { get; set; }

        public int DisplayOrder { get; set; } = 0;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        public bool IsDeleted { get; set; } = false;

        public DateTime? DeletedAt { get; set; }

        // Navigation properties
        [ForeignKey(nameof(CatalogId))]
        public virtual Catalog Catalog { get; set; }

        [ForeignKey(nameof(CategoryId))]
        public virtual CatalogCategory Category { get; set; }

        public virtual ICollection<CatalogItemVariant> Variants { get; set; }

        public virtual ICollection<CatalogItemAddon> Addons { get; set; }
    }
}
```

### 6.4 Entity Configuration Example

```csharp
// Data/Configurations/ProfileConfiguration.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using BizBio.API.Data.Entities;

namespace BizBio.API.Data.Configurations
{
    public class ProfileConfiguration : IEntityTypeConfiguration<Profile>
    {
        public void Configure(EntityTypeBuilder<Profile> builder)
        {
            // Table name
            builder.ToTable("Profiles");

            // Primary key
            builder.HasKey(p => p.Id);
            builder.Property(p => p.Id)
                .ValueGeneratedOnAdd()
                .IsRequired();

            // Indexes
            builder.HasIndex(p => p.Slug).IsUnique();
            builder.HasIndex(p => p.UserId);
            builder.HasIndex(p => p.IsPublic);
            builder.HasIndex(p => p.CreatedAt);

            // Relationships
            builder.HasOne(p => p.User)
                .WithMany(u => u.Profiles)
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(p => p.Branding)
                .WithOne(b => b.Profile)
                .HasForeignKey<ProfileBranding>(b => b.ProfileId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(p => p.SocialLinks)
                .WithOne(s => s.Profile)
                .HasForeignKey(s => s.ProfileId)
                .OnDelete(DeleteBehavior.Cascade);

            // Self-referencing for organization hierarchy
            builder.HasMany(p => p.ChildRelationships)
                .WithOne(r => r.ParentProfile)
                .HasForeignKey(r => r.ParentProfileId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(p => p.ParentRelationships)
                .WithOne(r => r.ChildProfile)
                .HasForeignKey(r => r.ChildProfileId)
                .OnDelete(DeleteBehavior.Restrict);

            // Value conversions and defaults
            builder.Property(p => p.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            builder.Property(p => p.IsDeleted)
                .HasDefaultValue(false);
        }
    }
}
```

### 6.5 Controller Example

```csharp
// Controllers/ProfilesController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using BizBio.API.Services.Interfaces;
using BizBio.API.Models.DTOs.Profile;
using BizBio.API.Models.Responses;

namespace BizBio.API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    [Produces("application/json")]
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

        /// <summary>
        /// Get all profiles for the authenticated user
        /// </summary>
        [HttpGet]
        [Authorize]
        [ProducesResponseType(typeof(ApiResponse<List<ProfileResponseDTO>>), 200)]
        public async Task<IActionResult> GetProfiles()
        {
            try
            {
                var userId = GetUserIdFromToken();
                var profiles = await _profileService.GetUserProfilesAsync(userId);

                return Ok(new ApiResponse<List<ProfileResponseDTO>>
                {
                    Success = true,
                    Data = profiles
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting profiles");
                return StatusCode(500, new ErrorResponse
                {
                    Message = "An error occurred while retrieving profiles"
                });
            }
        }

        /// <summary>
        /// Get profile by ID
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<ProfileResponseDTO>), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetProfileById(int id)
        {
            try
            {
                var profile = await _profileService.GetProfileByIdAsync(id);

                if (profile == null)
                {
                    return NotFound(new ErrorResponse
                    {
                        Message = "Profile not found"
                    });
                }

                return Ok(new ApiResponse<ProfileResponseDTO>
                {
                    Success = true,
                    Data = profile
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting profile {id}");
                return StatusCode(500, new ErrorResponse
                {
                    Message = "An error occurred while retrieving the profile"
                });
            }
        }

        /// <summary>
        /// Get profile by slug (public access)
        /// </summary>
        [HttpGet("by-slug/{slug}")]
        [ProducesResponseType(typeof(ApiResponse<ProfileResponseDTO>), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetProfileBySlug(string slug)
        {
            try
            {
                var profile = await _profileService.GetProfileBySlugAsync(slug);

                if (profile == null)
                {
                    return NotFound(new ErrorResponse
                    {
                        Message = "Profile not found"
                    });
                }

                // Increment view count
                await _profileService.IncrementViewCountAsync(profile.Id);

                return Ok(new ApiResponse<ProfileResponseDTO>
                {
                    Success = true,
                    Data = profile
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting profile by slug: {slug}");
                return StatusCode(500, new ErrorResponse
                {
                    Message = "An error occurred while retrieving the profile"
                });
            }
        }

        /// <summary>
        /// Create a new profile
        /// </summary>
        [HttpPost]
        [Authorize]
        [ProducesResponseType(typeof(ApiResponse<ProfileResponseDTO>), 201)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> CreateProfile([FromBody] CreateProfileDTO dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var userId = GetUserIdFromToken();

                // Check subscription limits
                var canCreate = await _profileService.CanUserCreateProfileAsync(userId);
                if (!canCreate)
                {
                    return BadRequest(new ErrorResponse
                    {
                        Message = "Profile limit reached for your subscription tier"
                    });
                }

                var profile = await _profileService.CreateProfileAsync(userId, dto);

                return CreatedAtAction(
                    nameof(GetProfileById),
                    new { id = profile.Id },
                    new ApiResponse<ProfileResponseDTO>
                    {
                        Success = true,
                        Message = "Profile created successfully",
                        Data = profile
                    });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating profile");
                return StatusCode(500, new ErrorResponse
                {
                    Message = "An error occurred while creating the profile"
                });
            }
        }

        /// <summary>
        /// Update an existing profile
        /// </summary>
        [HttpPut("{id}")]
        [Authorize]
        [ProducesResponseType(typeof(ApiResponse<ProfileResponseDTO>), 200)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateProfile(int id, [FromBody] UpdateProfileDTO dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var userId = GetUserIdFromToken();

                // Check ownership
                var isOwner = await _profileService.IsProfileOwnerAsync(id, userId);
                if (!isOwner)
                {
                    return Forbid();
                }

                var profile = await _profileService.UpdateProfileAsync(id, dto);

                return Ok(new ApiResponse<ProfileResponseDTO>
                {
                    Success = true,
                    Message = "Profile updated successfully",
                    Data = profile
                });
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new ErrorResponse
                {
                    Message = "Profile not found"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating profile {id}");
                return StatusCode(500, new ErrorResponse
                {
                    Message = "An error occurred while updating the profile"
                });
            }
        }

        /// <summary>
        /// Delete a profile
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize]
        [ProducesResponseType(200)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteProfile(int id)
        {
            try
            {
                var userId = GetUserIdFromToken();

                // Check ownership
                var isOwner = await _profileService.IsProfileOwnerAsync(id, userId);
                if (!isOwner)
                {
                    return Forbid();
                }

                await _profileService.DeleteProfileAsync(id);

                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Message = "Profile deleted successfully"
                });
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new ErrorResponse
                {
                    Message = "Profile not found"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting profile {id}");
                return StatusCode(500, new ErrorResponse
                {
                    Message = "An error occurred while deleting the profile"
                });
            }
        }

        // Helper method to extract user ID from JWT token
        private int GetUserIdFromToken()
        {
            var userIdClaim = User.FindFirst("userId")?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
            {
                throw new UnauthorizedAccessException("User ID not found in token");
            }
            return int.Parse(userIdClaim);
        }
    }
}
```

### 6.6 Service Layer Example

```csharp
// Services/Implementation/ProfileService.cs
using AutoMapper;
using BizBio.API.Data;
using BizBio.API.Data.Entities;
using BizBio.API.Models.DTOs.Profile;
using BizBio.API.Repositories.Interfaces;
using BizBio.API.Services.Interfaces;
using BizBio.API.Helpers;
using Microsoft.EntityFrameworkCore;

namespace BizBio.API.Services.Implementation
{
    public class ProfileService : IProfileService
    {
        private readonly IProfileRepository _profileRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<ProfileService> _logger;

        public ProfileService(
            IProfileRepository profileRepository,
            IUserRepository userRepository,
            IMapper mapper,
            ILogger<ProfileService> logger)
        {
            _profileRepository = profileRepository;
            _userRepository = userRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<List<ProfileResponseDTO>> GetUserProfilesAsync(int userId)
        {
            var profiles = await _profileRepository.GetByUserIdAsync(userId);
            return _mapper.Map<List<ProfileResponseDTO>>(profiles);
        }

        public async Task<ProfileResponseDTO?> GetProfileByIdAsync(int id)
        {
            var profile = await _profileRepository.GetByIdAsync(id);
            return _mapper.Map<ProfileResponseDTO>(profile);
        }

        public async Task<ProfileResponseDTO?> GetProfileBySlugAsync(string slug)
        {
            var profile = await _profileRepository.GetBySlugAsync(slug);
            return _mapper.Map<ProfileResponseDTO>(profile);
        }

        public async Task<bool> CanUserCreateProfileAsync(int userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null) return false;

            var userProfileCount = await _profileRepository.GetProfileCountByUserIdAsync(userId);
            var subscription = user.Subscription;

            if (subscription == null)
            {
                // Free tier - max 1 profile
                return userProfileCount < 1;
            }

            // Check subscription tier limits
            return userProfileCount < subscription.Tier.MaxProfiles;
        }

        public async Task<ProfileResponseDTO> CreateProfileAsync(int userId, CreateProfileDTO dto)
        {
            // Generate unique slug
            var slug = SlugHelper.GenerateSlug(dto.DisplayName);
            slug = await EnsureUniqueSlugAsync(slug);

            var profile = _mapper.Map<Profile>(dto);
            profile.UserId = userId;
            profile.Slug = slug;
            profile.CreatedAt = DateTime.UtcNow;

            var created = await _profileRepository.CreateAsync(profile);

            return _mapper.Map<ProfileResponseDTO>(created);
        }

        public async Task<ProfileResponseDTO> UpdateProfileAsync(int id, UpdateProfileDTO dto)
        {
            var profile = await _profileRepository.GetByIdAsync(id);
            if (profile == null)
            {
                throw new KeyNotFoundException($"Profile with ID {id} not found");
            }

            // Map updates
            _mapper.Map(dto, profile);
            profile.UpdatedAt = DateTime.UtcNow;

            // Update slug if display name changed
            if (dto.DisplayName != null && dto.DisplayName != profile.DisplayName)
            {
                var newSlug = SlugHelper.GenerateSlug(dto.DisplayName);
                profile.Slug = await EnsureUniqueSlugAsync(newSlug, id);
            }

            await _profileRepository.UpdateAsync(profile);

            return _mapper.Map<ProfileResponseDTO>(profile);
        }

        public async Task DeleteProfileAsync(int id)
        {
            await _profileRepository.DeleteAsync(id);
        }

        public async Task<bool> IsProfileOwnerAsync(int profileId, int userId)
        {
            var profile = await _profileRepository.GetByIdAsync(profileId);
            return profile != null && profile.UserId == userId;
        }

        public async Task IncrementViewCountAsync(int profileId)
        {
            await _profileRepository.IncrementViewCountAsync(profileId);
        }

        private async Task<string> EnsureUniqueSlugAsync(string slug, int? excludeId = null)
        {
            var counter = 1;
            var uniqueSlug = slug;

            while (await _profileRepository.SlugExistsAsync(uniqueSlug, excludeId))
            {
                uniqueSlug = $"{slug}-{counter}";
                counter++;
            }

            return uniqueSlug;
        }
    }
}
```

### 6.7 JWT Authentication Configuration

```csharp
// Program.cs - Startup configuration
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using BizBio.API.Data;
using BizBio.API.Services.Interfaces;
using BizBio.API.Services.Implementation;
using BizBio.API.Repositories.Interfaces;
using BizBio.API.Repositories.Implementation;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container

// Database
builder.Services.AddDbContext<BizBioDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DefaultConnection"))
    )
);

// JWT Authentication
var jwtSettings = builder.Configuration.GetSection("JWT");
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtSettings["Key"])
            ),
            ClockSkew = TimeSpan.Zero
        };
    });

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins(builder.Configuration["Frontend:Url"])
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    });
});

// AutoMapper
builder.Services.AddAutoMapper(typeof(Program));

// Dependency Injection
// Services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IProfileService, ProfileService>();
builder.Services.AddScoped<ICatalogService, CatalogService>();
builder.Services.AddScoped<ISubscriptionService, SubscriptionService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddScoped<IAnalyticsService, AnalyticsService>();
builder.Services.AddScoped<IEmailService, EmailService>();

// Repositories
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IProfileRepository, ProfileRepository>();
builder.Services.AddScoped<ICatalogRepository, CatalogRepository>();
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

// Redis Cache
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration["Redis:ConnectionString"];
    options.InstanceName = "BizBio_";
});

// Rate Limiting
builder.Services.AddMemoryCache();

// Controllers
builder.Services.AddControllers()
    .AddNewtonsoftJson(options =>
    {
        options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
    });

// Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "BizBio API",
        Version = "v1",
        Description = "RESTful API for BizBio Digital Profile Platform",
        Contact = new OpenApiContact
        {
            Name = "BizBio Support",
            Email = "support@bizbio.co.za"
        }
    });

    // JWT Authentication in Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowFrontend");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
```

---

## 7. Database Design (Code-First)

### 7.1 Code-First Migration Strategy

All database schema is defined through Entity Framework Core entities and migrations. Integer-based IDs are used throughout.

**Creating Initial Migration:**
```bash
dotnet ef migrations add InitialCreate
dotnet ef database update
```

**Adding New Migration:**
```bash
dotnet ef migrations add AddCatalogFeatures
dotnet ef database update
```

### 7.2 Complete Entity List (29 Tables)

| Table Name | Entity Class | Primary Key Type | Description |
|-----------|--------------|------------------|-------------|
| Users | User | int | User accounts |
| SubscriptionTiers | SubscriptionTier | int | Subscription plan definitions |
| Subscriptions | Subscription | int | User subscriptions |
| Categories | BusinessCategory | int | Business categories |
| ProfileTemplates | ProfileTemplate | int | Profile templates |
| Profiles | Profile | int | User profiles |
| ProfileBrandings | ProfileBranding | int | Profile customization |
| ProfileRelationships | ProfileRelationship | int | Org hierarchy |
| Catalogs | Catalog | int | Profile catalogs |
| CatalogCategories | CatalogCategory | int | Catalog categories |
| CatalogItems | CatalogItem | int | Catalog items |
| CatalogItemVariants | CatalogItemVariant | int | Item variants |
| CatalogItemImages | CatalogItemImage | int | Item images |
| CatalogItemAddons | CatalogItemAddon | int | Item add-ons |
| SocialMediaLinks | SocialMediaLink | int | Social media links |
| ProfilePhotos | ProfilePhoto | int | Profile photos |
| ProfileVideos | ProfileVideo | int | Profile videos |
| ProfileDocuments | ProfileDocument | int | Profile documents |
| ProfileQRCodes | ProfileQRCode | int | Generated QR codes |
| NFCProducts | NFCProduct | int | NFC product catalog |
| NFCOrders | NFCOrder | int | NFC product orders |
| NFCTags | NFCTag | int | Registered NFC tags |
| ProfileAnalytics | ProfileAnalytic | int | Profile analytics |
| CatalogAnalytics | CatalogAnalytic | int | Catalog analytics |
| ProfileServices | ProfileService | int | Service listings |
| ProfileCustomFields | ProfileCustomField | int | Custom profile fields |
| ProfileReviews | ProfileReview | int | User reviews |
| SupportTickets | SupportTicket | int | Support tickets |
| Payments | Payment | int | Payment transactions |

### 7.3 Database Relationships

```
Users (1) ──────── (Many) Profiles
Users (1) ──────── (1) Subscriptions
Users (1) ──────── (Many) Payments

Profiles (1) ────── (1) ProfileBrandings
Profiles (1) ────── (Many) SocialMediaLinks
Profiles (1) ────── (Many) ProfilePhotos
Profiles (1) ────── (Many) ProfileDocuments
Profiles (1) ────── (1) Catalogs
Profiles (1) ────── (Many) NFCTags
Profiles (1) ────── (Many) ProfileAnalytics

ProfileRelationships (Many-to-Many with self-referencing)
Profiles (1 Parent) ──── (Many) ProfileRelationships
Profiles (1 Child) ───── (Many) ProfileRelationships

Catalogs (1) ────── (Many) CatalogCategories
Catalogs (1) ────── (Many) CatalogItems

CatalogCategories (1) ── (Many) CatalogItems

CatalogItems (1) ─────── (Many) CatalogItemVariants
CatalogItems (1) ─────── (Many) CatalogItemImages
CatalogItems (1) ─────── (Many) CatalogItemAddons

Subscriptions (Many) ──── (1) SubscriptionTiers

NFCOrders (Many) ──────── (1) Users
NFCOrders (Many) ──────── (1) NFCProducts

SupportTickets (Many) ─── (1) Users
```

### 7.4 Key Indexes for Performance

```csharp
// Applied via Fluent API in entity configurations

// Users
CreateIndex("IX_Users_Email", unique: true)
CreateIndex("IX_Users_CreatedAt")

// Profiles
CreateIndex("IX_Profiles_Slug", unique: true)
CreateIndex("IX_Profiles_UserId")
CreateIndex("IX_Profiles_IsPublic")
CreateIndex("IX_Profiles_CreatedAt")
CreateIndex("IX_Profiles_BusinessCategory")

// CatalogItems
CreateIndex("IX_CatalogItems_CatalogId")
CreateIndex("IX_CatalogItems_CategoryId")
CreateIndex("IX_CatalogItems_IsAvailable")
CreateIndex("IX_CatalogItems_DisplayOrder")

// ProfileAnalytics
CreateIndex("IX_ProfileAnalytics_ProfileId")
CreateIndex("IX_ProfileAnalytics_CreatedAt")

// NFCTags
CreateIndex("IX_NFCTags_TagId", unique: true)
CreateIndex("IX_NFCTags_ProfileId")
```

---

## 8. API Specifications

### 8.1 API Versioning

All APIs are versioned with `/api/v1/` prefix. Future versions will use `/api/v2/`, etc.

### 8.2 Standard Response Format

**Success Response:**
```json
{
  "success": true,
  "message": "Operation successful",
  "data": { /* response data */ }
}
```

**Error Response:**
```json
{
  "success": false,
  "message": "Error description",
  "errors": [
    {
      "field": "email",
      "message": "Email is required"
    }
  ]
}
```

**Paginated Response:**
```json
{
  "success": true,
  "data": [ /* array of items */ ],
  "pagination": {
    "currentPage": 1,
    "pageSize": 20,
    "totalPages": 5,
    "totalItems": 97
  }
}
```

### 8.3 Complete API Endpoint List

#### Authentication Endpoints

```
POST   /api/v1/auth/register          Register new user
POST   /api/v1/auth/login             Login user
POST   /api/v1/auth/refresh           Refresh access token
POST   /api/v1/auth/logout            Logout user
POST   /api/v1/auth/verify-email      Verify email address
POST   /api/v1/auth/resend-verification   Resend verification email
POST   /api/v1/auth/forgot-password   Request password reset
POST   /api/v1/auth/reset-password    Reset password with token
```

#### User Endpoints

```
GET    /api/v1/users/{id}             Get user details
PUT    /api/v1/users/{id}             Update user details
DELETE /api/v1/users/{id}             Delete user account
PATCH  /api/v1/users/{id}/password    Change password
GET    /api/v1/users/{id}/subscription    Get user subscription
```

#### Profile Endpoints

```
GET    /api/v1/profiles               Get user's profiles
POST   /api/v1/profiles               Create profile
GET    /api/v1/profiles/{id}          Get profile by ID
GET    /api/v1/profiles/by-slug/{slug}    Get profile by slug (public)
PUT    /api/v1/profiles/{id}          Update profile
DELETE /api/v1/profiles/{id}          Delete profile
PATCH  /api/v1/profiles/{id}/visibility   Update visibility
PUT    /api/v1/profiles/{id}/contact  Update contact info
PUT    /api/v1/profiles/{id}/branding Update branding
PUT    /api/v1/profiles/{id}/layout   Update layout
POST   /api/v1/profiles/{id}/apply-template   Apply template
```

#### Social Media Endpoints

```
GET    /api/v1/profiles/{id}/social-links     Get social links
POST   /api/v1/profiles/{id}/social-links     Add social link
PUT    /api/v1/profiles/{id}/social-links/{linkId}   Update social link
DELETE /api/v1/profiles/{id}/social-links/{linkId}   Delete social link
```

#### Photo/Media Endpoints

```
POST   /api/v1/profiles/{id}/photos           Upload photo
GET    /api/v1/profiles/{id}/photos           Get photos
DELETE /api/v1/profiles/{id}/photos/{photoId} Delete photo
POST   /api/v1/profiles/{id}/videos           Add video
POST   /api/v1/profiles/{id}/cover-image      Upload cover image
```

#### Document Endpoints

```
GET    /api/v1/profiles/{id}/documents        Get documents
POST   /api/v1/profiles/{id}/documents        Upload document
PUT    /api/v1/profiles/{id}/documents/{docId}   Update document
DELETE /api/v1/profiles/{id}/documents/{docId}   Delete document
```

#### Organization/Team Endpoints

```
GET    /api/v1/profiles/{orgId}/team-members  Get team members
POST   /api/v1/profiles/{orgId}/team-members  Add team member
PUT    /api/v1/profiles/{orgId}/team-members/{relationshipId}   Update member
DELETE /api/v1/profiles/{orgId}/team-members/{relationshipId}   Remove member
GET    /api/v1/profiles/{orgId}/hierarchy     Get org chart
```

#### Catalog Endpoints

```
POST   /api/v1/catalogs               Create catalog
GET    /api/v1/catalogs/{id}          Get catalog
PUT    /api/v1/catalogs/{id}          Update catalog
DELETE /api/v1/catalogs/{id}          Delete catalog
GET    /api/v1/catalogs/{id}/public   Get catalog (public view)
```

#### Catalog Category Endpoints

```
GET    /api/v1/catalogs/{catalogId}/categories     Get categories
POST   /api/v1/catalogs/{catalogId}/categories     Create category
PUT    /api/v1/catalogs/{catalogId}/categories/{categoryId}   Update category
DELETE /api/v1/catalogs/{catalogId}/categories/{categoryId}   Delete category
PATCH  /api/v1/catalogs/{catalogId}/categories/{categoryId}/reorder   Reorder
```

#### Catalog Item Endpoints

```
GET    /api/v1/catalogs/{catalogId}/items      Get items
POST   /api/v1/catalogs/{catalogId}/items      Create item
GET    /api/v1/catalog-items/{itemId}          Get item details
PUT    /api/v1/catalog-items/{itemId}          Update item
DELETE /api/v1/catalog-items/{itemId}          Delete item
PATCH  /api/v1/catalog-items/{itemId}/availability  Toggle availability
POST   /api/v1/catalogs/{catalogId}/items/bulk-import  Bulk import (CSV)
PUT    /api/v1/catalogs/{catalogId}/items/bulk-update Bulk update
DELETE /api/v1/catalogs/{catalogId}/items/bulk-delete  Bulk delete
```

#### Variant Endpoints

```
GET    /api/v1/catalog-items/{itemId}/variants    Get variants
POST   /api/v1/catalog-items/{itemId}/variants    Create variant
PUT    /api/v1/catalog-items/{itemId}/variants/{variantId}   Update variant
DELETE /api/v1/catalog-items/{itemId}/variants/{variantId}   Delete variant
```

#### Add-on Endpoints

```
GET    /api/v1/catalog-items/{itemId}/addons      Get add-ons
POST   /api/v1/catalog-items/{itemId}/addons      Create add-on
PUT    /api/v1/catalog-items/{itemId}/addons/{addonId}   Update add-on
DELETE /api/v1/catalog-items/{itemId}/addons/{addonId}   Delete add-on
```

#### NFC Endpoints

```
GET    /api/v1/nfc-products           Get NFC products
POST   /api/v1/nfc-products/order     Create NFC order
GET    /api/v1/nfc-products/orders    Get my orders
GET    /api/v1/nfc-products/orders/{orderId}  Get order details
POST   /api/v1/profiles/{profileId}/nfc-tags  Register NFC tag
GET    /api/v1/profiles/{profileId}/nfc-tags  Get NFC tags
DELETE /api/v1/profiles/{profileId}/nfc-tags/{tagId}   Deregister tag
POST   /api/v1/nfc-tags/{tagId}/scan  Track NFC scan (public)
```

#### QR Code Endpoints

```
POST   /api/v1/profiles/{profileId}/qr-codes   Generate QR code
GET    /api/v1/profiles/{profileId}/qr-codes   Get QR codes
DELETE /api/v1/profiles/{profileId}/qr-codes/{qrId}   Delete QR code
```

#### Analytics Endpoints

```
POST   /api/v1/analytics/track        Track event (public)
GET    /api/v1/profiles/{id}/analytics    Get profile analytics
GET    /api/v1/catalogs/{id}/analytics    Get catalog analytics
GET    /api/v1/profiles/{id}/analytics/export   Export analytics
GET    /api/v1/analytics/dashboard    Get dashboard summary
```

#### Subscription Endpoints

```
GET    /api/v1/subscription-tiers     Get all subscription tiers
GET    /api/v1/subscription-tiers/{id}    Get tier details
POST   /api/v1/subscriptions/subscribe    Subscribe to tier
GET    /api/v1/subscriptions/{id}     Get subscription details
PUT    /api/v1/subscriptions/{id}/change-tier  Change subscription tier
POST   /api/v1/subscriptions/{id}/cancel  Cancel subscription
POST   /api/v1/subscriptions/{id}/reactivate  Reactivate subscription
GET    /api/v1/users/{userId}/subscription   Get user's current subscription
GET    /api/v1/users/{userId}/subscription/usage   Get usage statistics
```

#### Payment Endpoints

```
POST   /api/v1/payments/initiate      Initiate payment
POST   /api/v1/payments/webhook/payfast   PayFast ITN webhook
GET    /api/v1/payments/{transactionId}   Get payment details
GET    /api/v1/users/{userId}/payments    Get payment history
GET    /api/v1/payments/{transactionId}/invoice   Download invoice PDF
POST   /api/v1/payments/{transactionId}/refund    Request refund
```

#### Template Endpoints

```
GET    /api/v1/templates              Get all templates
GET    /api/v1/templates/{id}         Get template details
POST   /api/v1/profiles/{profileId}/apply-template  Apply template
```

#### Admin Endpoints

```
GET    /api/v1/admin/users            Get all users
GET    /api/v1/admin/users/{id}       Get user details
PUT    /api/v1/admin/users/{id}       Update user
DELETE /api/v1/admin/users/{id}       Delete user
POST   /api/v1/admin/users/{id}/suspend   Suspend user
POST   /api/v1/admin/users/{id}/unsuspend Unsuspend user

GET    /api/v1/admin/profiles         Get all profiles
GET    /api/v1/admin/subscriptions    Get all subscriptions
GET    /api/v1/admin/payments         Get all payments
GET    /api/v1/admin/analytics/dashboard   Admin dashboard stats
GET    /api/v1/admin/reports/revenue  Revenue report
GET    /api/v1/admin/reports/users    User growth report
```

#### Support Ticket Endpoints

```
GET    /api/v1/support/tickets        Get my tickets
POST   /api/v1/support/tickets        Create ticket
GET    /api/v1/support/tickets/{id}   Get ticket details
PUT    /api/v1/support/tickets/{id}   Update ticket
POST   /api/v1/support/tickets/{id}/messages   Add message to ticket
```

### 8.4 Rate Limiting

Rate limits are applied per subscription tier:

| Tier | Requests per Minute | Burst Limit |
|------|---------------------|-------------|
| Anonymous | 60 | 100 |
| Free | 100 | 200 |
| Basic | 300 | 500 |
| Professional | 500 | 1000 |
| Enterprise | 1000 | 2000 |
| Admin | Unlimited | Unlimited |

Headers returned:
```
X-RateLimit-Limit: 300
X-RateLimit-Remaining: 287
X-RateLimit-Reset: 1635789600
```

Response when limit exceeded:
```json
{
  "success": false,
  "message": "Rate limit exceeded",
  "retryAfter": 45
}
```

### 8.5 Pagination

List endpoints support pagination via query parameters:

```
GET /api/v1/profiles?page=1&pageSize=20&sortBy=createdAt&sortOrder=desc
```

Parameters:
- `page` (default: 1)
- `pageSize` (default: 20, max: 100)
- `sortBy` (varies by endpoint)
- `sortOrder` (asc/desc)

---

## 9. Security Architecture

### 9.1 Authentication Flow

```
1. User submits credentials to /api/v1/auth/login
2. API validates credentials
3. API generates JWT access token (1 hour expiry)
4. API generates refresh token (7 days expiry)
5. Tokens returned to client
6. Client stores tokens securely
7. Client includes access token in Authorization header
8. API validates token on each request
9. When access token expires, client uses refresh token
10. API issues new access token
```

### 9.2 JWT Token Structure

```json
{
  "userId": 123,
  "email": "user@example.com",
  "role": "User",
  "subscriptionTier": "Professional",
  "iat": 1635789600,
  "exp": 1635793200
}
```

### 9.3 Authorization Levels

| Level | Description | Implementation |
|-------|-------------|----------------|
| Public | No authentication required | No `[Authorize]` attribute |
| Authenticated | Valid JWT token required | `[Authorize]` attribute |
| Owner | User must own the resource | Custom authorization policy |
| Role-Based | Specific role required | `[Authorize(Roles = "Admin")]` |
| Policy-Based | Custom policy | `[Authorize(Policy = "CanEditProfile")]` |

### 9.4 Security Headers

All API responses include security headers:

```
X-Content-Type-Options: nosniff
X-Frame-Options: DENY
X-XSS-Protection: 1; mode=block
Strict-Transport-Security: max-age=31536000; includeSubDomains
Content-Security-Policy: default-src 'self'
```

### 9.5 Input Validation

All input is validated at multiple layers:

1. **Client-side validation** (Vue.js with VeeValidate)
2. **Model validation** (Data Annotations)
3. **FluentValidation** (Business rules)
4. **Database constraints** (Final layer)

Example FluentValidation:

```csharp
public class CreateProfileValidator : AbstractValidator<CreateProfileDTO>
{
    public CreateProfileValidator()
    {
        RuleFor(x => x.DisplayName)
            .NotEmpty().WithMessage("Display name is required")
            .MaximumLength(200).WithMessage("Display name must not exceed 200 characters");

        RuleFor(x => x.ProfileType)
            .NotEmpty().WithMessage("Profile type is required")
            .Must(BeValidProfileType).WithMessage("Invalid profile type");

        RuleFor(x => x.Email)
            .EmailAddress().When(x => !string.IsNullOrEmpty(x.Email))
            .WithMessage("Invalid email format");

        RuleFor(x => x.Phone)
            .Matches(@"^(\+27|0)[0-9]{9}$").When(x => !string.IsNullOrEmpty(x.Phone))
            .WithMessage("Invalid South African phone number");
    }

    private bool BeValidProfileType(string profileType)
    {
        return new[] { "Personal", "Business", "Organization" }.Contains(profileType);
    }
}
```

### 9.6 SQL Injection Prevention

Entity Framework Core with parameterized queries provides protection against SQL injection:

```csharp
// Safe - parameterized query
var profile = await _context.Profiles
    .Where(p => p.Slug == slug)
    .FirstOrDefaultAsync();

// Safe - LINQ to Entities
var profiles = await _context.Profiles
    .Where(p => p.UserId == userId && p.IsPublic)
    .ToListAsync();
```

### 9.7 File Upload Security

```csharp
public class FileUploadHelper
{
    private static readonly string[] AllowedImageExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
    private static readonly string[] AllowedDocumentExtensions = { ".pdf", ".docx", ".xlsx", ".pptx" };
    private const long MaxFileSize = 10 * 1024 * 1024; // 10 MB

    public static async Task<string> UploadFileAsync(IFormFile file, string type)
    {
        // Validate file
        if (file == null || file.Length == 0)
            throw new ArgumentException("No file provided");

        if (file.Length > MaxFileSize)
            throw new ArgumentException($"File size exceeds {MaxFileSize / (1024 * 1024)} MB limit");

        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        
        if (type == "image" && !AllowedImageExtensions.Contains(extension))
            throw new ArgumentException("Invalid image file type");
        
        if (type == "document" && !AllowedDocumentExtensions.Contains(extension))
            throw new ArgumentException("Invalid document file type");

        // Validate file content (magic number check)
        if (!IsValidFileContent(file, extension))
            throw new ArgumentException("File content does not match extension");

        // Generate unique filename
        var fileName = $"{Guid.NewGuid()}{extension}";

        // Upload to cloud storage
        var url = await CloudStorageService.UploadAsync(file, fileName);

        return url;
    }

    private static bool IsValidFileContent(IFormFile file, string extension)
    {
        // Check file signature (magic numbers)
        using var reader = new BinaryReader(file.OpenReadStream());
        var headerBytes = reader.ReadBytes(8);

        // Implement magic number validation
        // (simplified example)
        return true;
    }
}
```

### 9.8 CORS Configuration

```csharp
// Restrict CORS to specific frontend domain
services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins(
            "https://bizbio.co.za",
            "https://www.bizbio.co.za",
            "http://localhost:5173" // Development
        )
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials()
        .SetIsOriginAllowedToAllowWildcardSubdomains();
    });
});
```

---

## 10. Performance Requirements

### 10.1 API Performance Targets

| Metric | Target | Measurement |
|--------|--------|-------------|
| Average Response Time | < 200ms | 95th percentile |
| Database Query Time | < 50ms | Average |
| Concurrent Users | 10,000+ | Load testing |
| Requests per Second | 1,000+ | Per API instance |
| Error Rate | < 0.1% | Successful responses |

### 10.2 Caching Strategy

**Redis Cache Implementation:**

```csharp
public class ProfileService
{
    private readonly IDistributedCache _cache;
    private const int CacheDurationMinutes = 60;

    public async Task<ProfileResponseDTO> GetProfileBySlugAsync(string slug)
    {
        var cacheKey = $"profile_slug_{slug}";

        // Try get from cache
        var cachedProfile = await _cache.GetStringAsync(cacheKey);
        if (cachedProfile != null)
        {
            return JsonSerializer.Deserialize<ProfileResponseDTO>(cachedProfile);
        }

        // Get from database
        var profile = await _profileRepository.GetBySlugAsync(slug);
        var dto = _mapper.Map<ProfileResponseDTO>(profile);

        // Store in cache
        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(CacheDurationMinutes)
        };
        await _cache.SetStringAsync(
            cacheKey,
            JsonSerializer.Serialize(dto),
            options
        );

        return dto;
    }

    public async Task InvalidateProfileCacheAsync(string slug)
    {
        var cacheKey = $"profile_slug_{slug}";
        await _cache.RemoveAsync(cacheKey);
    }
}
```

**Cache Strategy by Resource:**

| Resource | Cache Duration | Invalidation |
|----------|----------------|--------------|
| Public Profiles | 60 minutes | On update/delete |
| Catalogs | 30 minutes | On item change |
| User Data | 5 minutes | On profile update |
| Analytics | 10 minutes | On new event |
| Static Data | 24 hours | Manual |

### 10.3 Database Optimization

**Query Optimization:**

```csharp
// Efficient query with includes
public async Task<Profile> GetProfileWithRelationsAsync(int id)
{
    return await _context.Profiles
        .Include(p => p.Branding)
        .Include(p => p.SocialLinks)
        .Include(p => p.Photos)
        .AsSplitQuery() // Separate queries for collections
        .AsNoTracking() // Read-only query
        .FirstOrDefaultAsync(p => p.Id == id);
}

// Pagination
public async Task<List<Profile>> GetProfilesPagedAsync(int page, int pageSize)
{
    return await _context.Profiles
        .OrderByDescending(p => p.CreatedAt)
        .Skip((page - 1) * pageSize)
        .Take(pageSize)
        .AsNoTracking()
        .ToListAsync();
}

// Projection for better performance
public async Task<List<ProfileSummaryDTO>> GetProfileSummariesAsync(int userId)
{
    return await _context.Profiles
        .Where(p => p.UserId == userId)
        .Select(p => new ProfileSummaryDTO
        {
            Id = p.Id,
            DisplayName = p.DisplayName,
            Slug = p.Slug,
            PhotoUrl = p.PhotoUrl,
            IsPublic = p.IsPublic
        })
        .ToListAsync();
}
```

### 10.4 Frontend Performance

**Vue.js Optimizations:**

1. **Lazy Loading:**
```typescript
// router/index.ts
const routes = [
  {
    path: '/dashboard',
    component: () => import('@/views/dashboard/Dashboard.vue'), // Lazy load
  },
]
```

2. **Component Lazy Loading:**
```vue
<template>
  <Suspense>
    <template #default>
      <HeavyComponent />
    </template>
    <template #fallback>
      <Loading />
    </template>
  </Suspense>
</template>

<script setup>
import { defineAsyncComponent } from 'vue'

const HeavyComponent = defineAsyncComponent(() =>
  import('./HeavyComponent.vue')
)
</script>
```

3. **Image Optimization:**
```vue
<template>
  <img
    :src="imageUrl"
    loading="lazy"
    class="w-full h-auto object-cover"
    :srcset="`
      ${imageUrl}?w=400 400w,
      ${imageUrl}?w=800 800w,
      ${imageUrl}?w=1200 1200w
    `"
    sizes="(max-width: 768px) 100vw, 50vw"
  />
</template>
```

4. **Debouncing API Calls:**
```typescript
import { debounce } from 'lodash-es'

const searchProfiles = debounce(async (query: string) => {
  const results = await profileService.search(query)
  // Update results
}, 300)
```

### 10.5 API Response Compression

```csharp
// Program.cs
builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
    options.Providers.Add<GzipCompressionProvider>();
    options.Providers.Add<BrotliCompressionProvider>();
});

builder.Services.Configure<GzipCompressionProviderOptions>(options =>
{
    options.Level = System.IO.Compression.CompressionLevel.Optimal;
});
```

---

## 11. Infrastructure & Deployment

### 11.1 Deployment Architecture

```
┌─────────────────────────────────────────────────────────────┐
│                      CloudFlare CDN                          │
│                   (SSL, DDoS Protection)                     │
└─────────────────────────────────────────────────────────────┘
                          │
        ┌─────────────────┴─────────────────┐
        │                                    │
        ▼                                    ▼
┌────────────────────┐           ┌─────────────────────┐
│  Frontend Hosting  │           │   API Hosting       │
│  (Vercel/Netlify)  │           │   (Azure/AWS/DO)    │
│                    │           │                     │
│  Vue.js SPA        │◄─────────►│  ASP.NET 6 API     │
│  Static Files      │   CORS    │  Load Balanced      │
└────────────────────┘           └─────────────────────┘
                                           │
                 ┌─────────────────────────┼─────────────────┐
                 │                         │                 │
                 ▼                         ▼                 ▼
         ┌──────────────┐       ┌──────────────┐   ┌───────────────┐
         │  MySQL DB    │       │  Redis Cache │   │ File Storage  │
         │  (Managed)   │       │  (Managed)   │   │ (S3/Azure)    │
         └──────────────┘       └──────────────┘   └───────────────┘
```

### 11.2 Environment Configuration

**.env (Frontend):**
```env
VITE_API_BASE_URL=https://api.bizbio.co.za
VITE_ENVIRONMENT=production
VITE_GOOGLE_MAPS_KEY=your_key_here
VITE_CDN_URL=https://cdn.bizbio.co.za
```

**appsettings.Production.json (Backend):**
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "ConnectionStrings": {
    "DefaultConnection": "Server=prod-mysql.bizbio.co.za;Database=bizbio_prod;Uid=bizbio_user;Pwd=${DB_PASSWORD};SslMode=Required;"
  },
  "JWT": {
    "Issuer": "bizbio.co.za",
    "Audience": "bizbio.co.za",
    "Key": "${JWT_SECRET_KEY}",
    "ExpiryInHours": 1,
    "RefreshTokenExpiryInDays": 7
  },
  "Frontend": {
    "Url": "https://bizbio.co.za"
  },
  "Redis": {
    "ConnectionString": "${REDIS_CONNECTION_STRING}"
  },
  "PayFast": {
    "MerchantId": "${PAYFAST_MERCHANT_ID}",
    "MerchantKey": "${PAYFAST_MERCHANT_KEY}",
    "Passphrase": "${PAYFAST_PASSPHRASE}",
    "ProcessUrl": "https://www.payfast.co.za/eng/process",
    "TestMode": false
  },
  "SendGrid": {
    "ApiKey": "${SENDGRID_API_KEY}",
    "FromEmail": "noreply@bizbio.co.za",
    "FromName": "BizBio"
  },
  "CloudStorage": {
    "Provider": "S3",
    "BucketName": "bizbio-prod",
    "AccessKey": "${AWS_ACCESS_KEY}",
    "SecretKey": "${AWS_SECRET_KEY}",
    "Region": "af-south-1"
  }
}
```

### 11.3 CI/CD Pipeline (GitHub Actions)

**Frontend Deployment:**
```yaml
# .github/workflows/deploy-frontend.yml
name: Deploy Frontend

on:
  push:
    branches: [ main ]
    paths:
      - 'frontend/**'

jobs:
  deploy:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v3
      
      - name: Setup Node.js
        uses: actions/setup-node@v3
        with:
          node-version: '18'
      
      - name: Install dependencies
        working-directory: ./frontend
        run: npm ci
      
      - name: Run tests
        working-directory: ./frontend
        run: npm test
      
      - name: Build
        working-directory: ./frontend
        run: npm run build
        env:
          VITE_API_BASE_URL: ${{ secrets.API_BASE_URL }}
      
      - name: Deploy to Vercel
        uses: amondnet/vercel-action@v25
        with:
          vercel-token: ${{ secrets.VERCEL_TOKEN }}
          vercel-org-id: ${{ secrets.VERCEL_ORG_ID }}
          vercel-project-id: ${{ secrets.VERCEL_PROJECT_ID }}
          working-directory: ./frontend
```

**Backend Deployment:**
```yaml
# .github/workflows/deploy-api.yml
name: Deploy API

on:
  push:
    branches: [ main ]
    paths:
      - 'backend/**'

jobs:
  deploy:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v3
      
      - name: Setup .NET
        uses: actions/setup-dotnet@v3
        with:
          dotnet-version: '6.0.x'
      
      - name: Restore dependencies
        working-directory: ./backend
        run: dotnet restore
      
      - name: Build
        working-directory: ./backend
        run: dotnet build --configuration Release --no-restore
      
      - name: Test
        working-directory: ./backend
        run: dotnet test --no-restore --verbosity normal
      
      - name: Publish
        working-directory: ./backend
        run: dotnet publish -c Release -o ./publish
      
      - name: Deploy to Azure
        uses: azure/webapps-deploy@v2
        with:
          app-name: 'bizbio-api'
          publish-profile: ${{ secrets.AZURE_PUBLISH_PROFILE }}
          package: ./backend/publish
```

### 11.4 Database Migrations in Production

```bash
# Apply migrations manually
dotnet ef database update --project BizBio.API

# Or via script in deployment pipeline
dotnet ef migrations script --idempotent --output migration.sql
```

**Migration Strategy:**
1. Create migration in development
2. Test migration in staging environment
3. Generate idempotent SQL script
4. Apply to production during maintenance window
5. Verify migration success
6. Rollback plan ready if needed

---

## 12. Integration Specifications

### 12.1 PayFast Payment Integration

**Payment Flow:**
```
1. User selects subscription tier
2. Frontend calls /api/v1/payments/initiate
3. API creates payment record with "Pending" status
4. API generates PayFast payment form data
5. User redirected to PayFast with payment data
6. User completes payment on PayFast
7. PayFast sends ITN to /api/v1/payments/webhook/payfast
8. API validates ITN signature
9. API updates payment and subscription status
10. API sends confirmation email to user
11. User redirected back to platform with success message
```

**ITN Webhook Handler:**
```csharp
[HttpPost("webhook/payfast")]
[AllowAnonymous]
public async Task<IActionResult> PayFastWebhook()
{
    // Read ITN data
    string requestBody;
    using (var reader = new StreamReader(Request.Body))
    {
        requestBody = await reader.ReadToEndAsync();
    }

    var data = ParsePayFastData(requestBody);

    // Validate signature
    if (!ValidatePayFastSignature(data))
    {
        _logger.LogWarning("Invalid PayFast signature");
        return BadRequest();
    }

    // Validate payment status
    if (data["payment_status"] != "COMPLETE")
    {
        _logger.LogWarning($"Payment not complete: {data["payment_status"]}");
        return Ok();
    }

    // Process payment
    var paymentId = int.Parse(data["m_payment_id"]);
    await _paymentService.ProcessPaymentAsync(paymentId, data);

    return Ok();
}
```

### 12.2 Email Service Integration

**Email Templates:**
- Welcome email (registration)
- Email verification
- Password reset
- Payment confirmation
- Subscription renewal
- Subscription cancellation
- NFC order confirmation

**SendGrid Implementation:**
```csharp
public class EmailService : IEmailService
{
    private readonly SendGridClient _client;
    private readonly IConfiguration _config;

    public async Task SendVerificationEmailAsync(string email, string token)
    {
        var verificationUrl = $"{_config["Frontend:Url"]}/verify-email?token={token}";

        var msg = new SendGridMessage()
        {
            From = new EmailAddress("noreply@bizbio.co.za", "BizBio"),
            Subject = "Verify your email address",
            HtmlContent = await GetEmailTemplate("verification", new { verificationUrl })
        };
        msg.AddTo(email);

        await _client.SendEmailAsync(msg);
    }

    private async Task<string> GetEmailTemplate(string templateName, object data)
    {
        // Load template from file or database
        var template = await File.ReadAllTextAsync($"Templates/Emails/{templateName}.html");
        
        // Replace placeholders
        foreach (var property in data.GetType().GetProperties())
        {
            template = template.Replace($"{{{property.Name}}}", property.GetValue(data)?.ToString());
        }

        return template;
    }
}
```

### 12.3 File Storage Integration (AWS S3)

```csharp
public class S3FileStorageService : IFileStorageService
{
    private readonly IAmazonS3 _s3Client;
    private readonly string _bucketName;

    public async Task<string> UploadFileAsync(Stream fileStream, string fileName, string contentType)
    {
        var key = $"uploads/{DateTime.UtcNow:yyyy/MM/dd}/{Guid.NewGuid()}/{fileName}";

        var request = new PutObjectRequest
        {
            BucketName = _bucketName,
            Key = key,
            InputStream = fileStream,
            ContentType = contentType,
            CannedACL = S3CannedACL.PublicRead
        };

        await _s3Client.PutObjectAsync(request);

        return $"https://{_bucketName}.s3.amazonaws.com/{key}";
    }

    public async Task DeleteFileAsync(string fileUrl)
    {
        var key = ExtractKeyFromUrl(fileUrl);
        await _s3Client.DeleteObjectAsync(_bucketName, key);
    }
}
```

### 12.4 Google Maps Integration

**Frontend Implementation:**
```vue
<template>
  <div ref="mapContainer" class="w-full h-96"></div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { Loader } from '@googlemaps/js-api-loader'

const props = defineProps<{
  latitude: number
  longitude: number
  address?: string
}>()

const mapContainer = ref<HTMLElement>()

onMounted(async () => {
  const loader = new Loader({
    apiKey: import.meta.env.VITE_GOOGLE_MAPS_KEY,
    version: 'weekly',
  })

  const { Map } = await loader.importLibrary('maps')
  const { AdvancedMarkerElement } = await loader.importLibrary('marker')

  const map = new Map(mapContainer.value!, {
    center: { lat: props.latitude, lng: props.longitude },
    zoom: 15,
    mapId: 'BIZBIO_MAP',
  })

  new AdvancedMarkerElement({
    map,
    position: { lat: props.latitude, lng: props.longitude },
    title: props.address,
  })
})
</script>
```

---

## 13. Testing Strategy

### 13.1 Backend Testing

**Unit Tests (xUnit):**
```csharp
public class ProfileServiceTests
{
    private readonly Mock<IProfileRepository> _mockRepo;
    private readonly Mock<IMapper> _mockMapper;
    private readonly ProfileService _service;

    public ProfileServiceTests()
    {
        _mockRepo = new Mock<IProfileRepository>();
        _mockMapper = new Mock<IMapper>();
        _service = new ProfileService(_mockRepo.Object, _mockMapper.Object);
    }

    [Fact]
    public async Task CreateProfile_ValidInput_ReturnsProfile()
    {
        // Arrange
        var dto = new CreateProfileDTO
        {
            DisplayName = "Test Profile",
            ProfileType = "Business"
        };
        var profile = new Profile { Id = 1, DisplayName = "Test Profile" };

        _mockRepo.Setup(r => r.CreateAsync(It.IsAny<Profile>()))
            .ReturnsAsync(profile);
        _mockMapper.Setup(m => m.Map<Profile>(dto))
            .Returns(profile);

        // Act
        var result = await _service.CreateProfileAsync(1, dto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        _mockRepo.Verify(r => r.CreateAsync(It.IsAny<Profile>()), Times.Once);
    }
}
```

**Integration Tests:**
```csharp
public class ProfilesControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public ProfilesControllerIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetProfile_ValidId_ReturnsProfile()
    {
        // Arrange
        var token = await GetAuthTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync("/api/v1/profiles/1");

        // Assert
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        var profile = JsonSerializer.Deserialize<ApiResponse<ProfileResponseDTO>>(content);
        Assert.NotNull(profile.Data);
    }
}
```

### 13.2 Frontend Testing

**Component Tests (Vitest):**
```typescript
import { describe, it, expect } from 'vitest'
import { mount } from '@vue/test-utils'
import ProfileCard from '@/components/profile/ProfileCard.vue'

describe('ProfileCard', () => {
  it('renders profile information', () => {
    const wrapper = mount(ProfileCard, {
      props: {
        profile: {
          displayName: 'John Doe',
          bio: 'Software Developer',
          photoUrl: '/photo.jpg'
        }
      }
    })

    expect(wrapper.text()).toContain('John Doe')
    expect(wrapper.text()).toContain('Software Developer')
    expect(wrapper.find('img').attributes('src')).toBe('/photo.jpg')
  })
})
```

**E2E Tests (Playwright):**
```typescript
import { test, expect } from '@playwright/test'

test.describe('Profile Creation', () => {
  test('user can create a new profile', async ({ page }) => {
    // Login
    await page.goto('/login')
    await page.fill('input[name="email"]', 'test@example.com')
    await page.fill('input[name="password"]', 'password123')
    await page.click('button[type="submit"]')

    // Navigate to create profile
    await page.click('text=Create Profile')
    await expect(page).toHaveURL('/dashboard/profiles/create')

    // Fill form
    await page.fill('input[name="displayName"]', 'Test Profile')
    await page.selectOption('select[name="profileType"]', 'Business')
    await page.fill('textarea[name="bio"]', 'Test bio')

    // Submit
    await page.click('button[type="submit"]')

    // Verify success
    await expect(page.locator('.toast-success')).toBeVisible()
    await expect(page).toHaveURL(/\/manage\/profile\/\d+/)
  })
})
```

### 13.3 API Testing

**Postman Collection Structure:**
```
BizBio API Tests
├── Authentication
│   ├── Register
│   ├── Login
│   ├── Verify Email
│   └── Refresh Token
├── Profiles
│   ├── Create Profile
│   ├── Get Profiles
│   ├── Update Profile
│   └── Delete Profile
├── Catalogs
│   └── ...
└── Admin
    └── ...
```

---

## 14. Development Phases

### 14.1 Phase 1: Foundation (Weeks 1-4)

**Backend:**
- [x] Project setup and structure
- [x] Database entities (Code-First) with integer IDs
- [x] Initial EF Core migrations
- [x] Authentication API (JWT)
- [x] User management API
- [x] Basic profile API

**Frontend:**
- [x] Vue.js project setup with Vite
- [x] Tailwind CSS configuration
- [x] Routing setup
- [x] Authentication pages (login, register)
- [x] Dashboard layout
- [x] API service layer

**Database:**
- [x] Initial migration with core tables
- [x] Seed data for subscription tiers

### 14.2 Phase 2: Core Features (Weeks 5-8)

**Backend:**
- [ ] Complete profile management API
- [ ] Profile branding API
- [ ] Catalog/menu API
- [ ] Social media links API
- [ ] File upload API

**Frontend:**
- [ ] Profile management UI
- [ ] Profile customization UI
- [ ] Catalog management UI
- [ ] Public profile views (responsive)
- [ ] Image upload components

**Database:**
- [ ] Add catalog tables
- [ ] Add branding tables
- [ ] File storage integration

### 14.3 Phase 3: Advanced Features (Weeks 9-12)

**Backend:**
- [ ] Organization hierarchy API
- [ ] Team member management API
- [ ] NFC integration API
- [ ] QR code generation API
- [ ] Analytics API

**Frontend:**
- [ ] Organization management UI
- [ ] Team member UI
- [ ] Analytics dashboard
- [ ] NFC management UI
- [ ] QR code display

**Database:**
- [ ] Relationship tables
- [ ] NFC tables
- [ ] Analytics tables

### 14.4 Phase 4: Subscriptions & Payments (Weeks 13-14)

**Backend:**
- [ ] Subscription management API
- [ ] Payment processing API
- [ ] PayFast integration
- [ ] Invoice generation API

**Frontend:**
- [ ] Pricing page
- [ ] Subscription management UI
- [ ] Payment flow
- [ ] Invoice display

**Integration:**
- [ ] PayFast ITN webhook
- [ ] Email notifications
- [ ] Subscription limits enforcement

### 14.5 Phase 5: Polish & Launch (Weeks 15-16)

**Backend:**
- [ ] Performance optimization
- [ ] Security audit
- [ ] API documentation (Swagger)
- [ ] Error handling improvements
- [ ] Logging and monitoring

**Frontend:**
- [ ] UI/UX polish
- [ ] Mobile responsiveness testing
- [ ] SEO optimization
- [ ] Performance optimization
- [ ] Accessibility improvements

**Testing:**
- [ ] Complete test coverage
- [ ] Load testing
- [ ] Security testing
- [ ] User acceptance testing

**Deployment:**
- [ ] Production environment setup
- [ ] CI/CD pipeline
- [ ] Monitoring and alerts
- [ ] Backup procedures
- [ ] Launch!

---

## 15. Technical Constraints

### 15.1 Technology Constraints

| Constraint | Impact | Mitigation |
|-----------|--------|------------|
| .NET 6 (LTS ends 2024) | Will need upgrade to .NET 8 | Plan migration for 2024 |
| MySQL 8.0+ required | Limits hosting options | Use managed MySQL services |
| Tailwind CSS learning curve | Team training needed | Documentation and examples |
| Vue.js 3 ecosystem maturity | Some libraries still in beta | Careful library selection |

### 15.2 Performance Constraints

| Constraint | Limit | Solution |
|-----------|-------|----------|
| API response time | < 200ms | Caching, optimization |
| Database connections | 100 concurrent | Connection pooling |
| File upload size | 10 MB | Client-side compression |
| CDN bandwidth | 1 TB/month initially | Optimize images |

### 15.3 Security Constraints

| Constraint | Requirement | Implementation |
|-----------|-------------|----------------|
| POPIA compliance | South African privacy law | Data export, deletion |
| GDPR compliance | European privacy law | Cookie consent, rights |
| PCI DSS | Payment security | Use PayFast (PCI compliant) |
| JWT token security | Secure storage | httpOnly cookies |

### 15.4 Budget Constraints

**Estimated Monthly Costs:**

| Service | Cost (ZAR) | Notes |
|---------|-----------|-------|
| Frontend Hosting (Vercel) | R0 | Free tier |
| API Hosting (Azure/AWS) | R500-2000 | Depends on traffic |
| Database (Managed MySQL) | R300-1000 | Size dependent |
| Redis Cache | R200-500 | Managed service |
| File Storage (S3) | R100-500 | Storage + bandwidth |
| Email Service (SendGrid) | R0-300 | Free tier up to 100/day |
| Domain & SSL | R150 | Annual cost / 12 |
| Monitoring Tools | R300 | Application Insights |
| **Total** | **R1,550-4,750** | Monthly |

---

## 16. Appendices

### 16.1 Code-First Migration Commands

```bash
# Add new migration
dotnet ef migrations add MigrationName --project BizBio.API

# Update database
dotnet ef database update --project BizBio.API

# Remove last migration (if not applied)
dotnet ef migrations remove --project BizBio.API

# Generate SQL script
dotnet ef migrations script --output migration.sql --project BizBio.API

# Generate idempotent script (safe for production)
dotnet ef migrations script --idempotent --output migration.sql --project BizBio.API

# List migrations
dotnet ef migrations list --project BizBio.API

# Update to specific migration
dotnet ef database update SpecificMigrationName --project BizBio.API
```

### 16.2 Git Workflow

```
main (production)
  │
  ├── develop (integration)
  │     │
  │     ├── feature/user-authentication
  │     ├── feature/profile-management
  │     ├── feature/catalog-system
  │     └── feature/payment-integration
  │
  └── hotfix/critical-bug-fix
```

### 16.3 Environment Variables Reference

**Frontend (.env):**
```env
VITE_API_BASE_URL=http://localhost:5000
VITE_ENVIRONMENT=development
VITE_GOOGLE_MAPS_KEY=your_key
VITE_CDN_URL=http://localhost:5000
```

**Backend (Environment Variables):**
```env
ASPNETCORE_ENVIRONMENT=Development
ConnectionStrings__DefaultConnection=Server=localhost;Database=bizbio;Uid=root;Pwd=password;
JWT__Key=your-very-secure-secret-key-here
JWT__Issuer=bizbio.co.za
JWT__Audience=bizbio.co.za
PayFast__MerchantId=your_merchant_id
PayFast__MerchantKey=your_merchant_key
SendGrid__ApiKey=your_sendgrid_key
AWS__AccessKey=your_access_key
AWS__SecretKey=your_secret_key
Redis__ConnectionString=localhost:6379
```

### 16.4 Useful Resources

**Documentation:**
- Vue.js: https://vuejs.org/
- Tailwind CSS: https://tailwindcss.com/
- ASP.NET Core: https://docs.microsoft.com/aspnet/core
- Entity Framework Core: https://docs.microsoft.com/ef/core
- MySQL: https://dev.mysql.com/doc/

**Tutorials:**
- Vue.js + Tailwind: https://www.youtube.com/vuejs-tailwind
- ASP.NET API: https://www.youtube.com/dotnet-api
- EF Core Code-First: https://www.entityframeworktutorial.net/

**Tools:**
- Postman: https://www.postman.com/
- MySQL Workbench: https://www.mysql.com/products/workbench/
- VS Code: https://code.visualstudio.com/
- Visual Studio 2022: https://visualstudio.microsoft.com/

---

## Document Approval

| Role | Name | Signature | Date |
|------|------|-----------|------|
| Project Manager | | | |
| Technical Lead | | | |
| Frontend Architect | | | |
| Backend Architect | | | |
| Database Architect | | | |
| Security Officer | | | |
| QA Lead | | | |

---

**End of Technical Specification Document**

**Version:** 2.0  
**Date:** November 2025  
**Status:** Ready for Development

*This document will be updated as the project evolves and new requirements emerge.*
