# BizBio Platform - MVP

![BizBio Logo](https://bizbio.co.za/logo.svg)

**Version:** 1.0 MVP
**Date:** November 2025
**Stack:** ASP.NET 6 + MySQL + Entity Framework Core 6

## Overview

BizBio is a vertical-specific digital platform offering three product lines:
- **BizBio Professional** - Digital business cards
- **BizBio Menu** - Digital restaurant menus with NFC table tags
- **BizBio Retail** - Product catalogs

This MVP focuses on the core foundation and NFC Menu Pro features (Phase 1).

## Table of Contents

- [Features](#features)
- [Tech Stack](#tech-stack)
- [Project Structure](#project-structure)
- [Prerequisites](#prerequisites)
- [Setup Instructions](#setup-instructions)
- [Database Setup](#database-setup)
- [Running the Application](#running-the-application)
- [API Documentation](#api-documentation)
- [Configuration](#configuration)
- [Troubleshooting](#troubleshooting)

## Features

### Implemented (Phase 1 MVP)

✅ **Authentication System**
- User registration with email verification
- JWT-based authentication
- Password reset functionality
- Account lockout after failed login attempts

✅ **Profile Management**
- Create and manage digital profiles
- Unique slug-based URLs
- Support for Professional, Menu, and Retail profile types

✅ **Catalog System**
- Catalog and catalog item management
- Image support (JSON array storage)
- Event mode filtering

✅ **NFC Table Tags**
- Restaurant table management
- NFC tag code assignment
- Table personalities (fun facts, images)
- NFC scan tracking with analytics

✅ **Event Mode**
- Toggle event mode per profile
- Filter menu items for events
- Custom event descriptions

✅ **Subscription System** (Database schema ready)
- Vertical-specific subscription tiers
- Feature flags and limits
- Trial period support

## Tech Stack

### Backend
- **Framework:** ASP.NET 6 Web API
- **Language:** C# 10
- **ORM:** Entity Framework Core 6 (Code-First)
- **Database:** MySQL 8.0+
- **Authentication:** JWT Bearer Tokens
- **Password Hashing:** BCrypt.Net

### API Documentation
- **Swagger/OpenAPI:** Integrated with JWT authentication support

### Dependencies
```xml
<PackageReference Include="Microsoft.EntityFrameworkCore" Version="6.0.25" />
<PackageReference Include="Pomelo.EntityFrameworkCore.MySql" Version="6.0.2" />
<PackageReference Include="Microsoft.AspNetCore.Authentication.JwtBearer" Version="6.0.25" />
<PackageReference Include="BCrypt.Net-Next" Version="4.0.3" />
<PackageReference Include="Swashbuckle.AspNetCore" Version="6.5.0" />
<PackageReference Include="SendGrid" Version="9.28.1" />
```

## Project Structure

```
BizBio.App/
├── src/
│   ├── BizBio.Core/              # Domain layer (entities, interfaces, DTOs)
│   │   ├── Entities/             # Entity models (User, Profile, etc.)
│   │   ├── Interfaces/           # Repository and service interfaces
│   │   ├── DTOs/                 # Data Transfer Objects
│   │   ├── Enums/                # Enumeration types
│   │   └── Exceptions/           # Custom exceptions
│   │
│   ├── BizBio.Infrastructure/    # Data access and services
│   │   ├── Data/                 # DbContext
│   │   ├── Repositories/         # Repository implementations
│   │   ├── Services/             # Service implementations
│   │   └── Migrations/           # EF Core migrations
│   │
│   └── BizBio.API/               # Web API layer
│       ├── Controllers/          # API controllers
│       ├── Filters/              # Custom filters
│       ├── Middleware/           # Custom middleware
│       ├── Program.cs            # Application startup
│       └── appsettings.json      # Configuration
│
├── BizBio.sln                    # Solution file
└── README.md                     # This file
```

## Prerequisites

Before you begin, ensure you have the following installed:

- **.NET 6 SDK** - [Download](https://dotnet.microsoft.com/download/dotnet/6.0)
- **MySQL 8.0+** - [Download](https://dev.mysql.com/downloads/)
- **Visual Studio 2022**, VS Code, or JetBrains Rider
- **Git** (for version control)

## Setup Instructions

### 1. Clone the Repository

```bash
git clone https://github.com/yourusername/BizBio.App.git
cd BizBio.App
```

### 2. Restore NuGet Packages

```bash
dotnet restore
```

### 3. Configure Database Connection

Edit `src/BizBio.API/appsettings.Development.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=bizbio_dev;User=your_mysql_user;Password=your_mysql_password;SslMode=None;"
  }
}
```

**Important:** Replace `your_mysql_user` and `your_mysql_password` with your MySQL credentials.

### 4. Configure JWT Settings

The JWT secret key is already configured in `appsettings.Development.json`. For production, use a strong random key:

```json
{
  "JWT": {
    "Secret": "YourSuperSecretKeyThatIsAtLeast32CharactersLong",
    "Issuer": "BizBio",
    "Audience": "BizBioUsers",
    "ExpiryMinutes": 10080
  }
}
```

## Database Setup

### Option 1: Using EF Core Migrations (Recommended)

```bash
# Navigate to the API project
cd src/BizBio.API

# Create the initial migration
dotnet ef migrations add InitialCreate --project ../BizBio.Infrastructure

# Apply the migration to create the database
dotnet ef database update
```

### Option 2: Using SQL Scripts

You can also run the SQL scripts directly in MySQL Workbench or command line. See the documentation for detailed schema:

- [BizBio-FOUNDATION-Technical-Spec.md](./BizBio-FOUNDATION-Technical-Spec.md)
- [BizBio-Subscription-System-Technical-Spec.md](./BizBio-Subscription-System-Technical-Spec.md)

### Seed Sample Data (Optional)

To create sample subscription tiers for testing:

```sql
-- See BizBio-Subscription-System-Technical-Spec.md Section 8.1 for seed data
```

## Running the Application

### Development Mode

```bash
cd src/BizBio.API
dotnet run
```

The API will start on:
- **HTTP:** http://localhost:5000
- **HTTPS:** https://localhost:5001
- **Swagger UI:** http://localhost:5000 (root path in development)

### Production Mode

```bash
dotnet publish -c Release -o ./publish
cd publish
dotnet BizBio.API.dll
```

## API Documentation

Once the application is running, access the Swagger UI documentation at:

**http://localhost:5000**

### Available Endpoints

#### Authentication (`/api/v1/auth`)
- `POST /api/v1/auth/register` - Register new user
- `POST /api/v1/auth/login` - Login and get JWT token
- `GET /api/v1/auth/verify-email?token={token}` - Verify email address
- `POST /api/v1/auth/forgot-password` - Request password reset
- `POST /api/v1/auth/reset-password` - Reset password with token

#### Profiles (`/api/v1/profiles`) 🔒 Requires Auth
- `GET /api/v1/profiles` - Get user's profiles
- `GET /api/v1/profiles/{id}` - Get specific profile
- `POST /api/v1/profiles` - Create new profile

#### Public Menu (`/api/v1/c`)
- `GET /api/v1/c/{slug}?nfc={code}` - Get menu by slug with optional NFC code

#### Tables (`/api/v1/dashboard/tables`) 🔒 Requires Auth
- `GET /api/v1/dashboard/tables?profileId={id}` - Get tables for profile
- `POST /api/v1/dashboard/tables` - Create new table

#### Subscriptions (`/api/v1/subscriptions`)
- `GET /api/v1/subscriptions/tiers` - Get all subscription tiers

#### Health Check
- `GET /health` - API health check

## Configuration

### Environment Variables

Key configuration sections in `appsettings.json`:

#### Database
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=bizbio;User=root;Password=password;"
  }
}
```

#### JWT Authentication
```json
{
  "JWT": {
    "Secret": "32+ character secret key",
    "Issuer": "BizBio",
    "Audience": "BizBioUsers",
    "ExpiryMinutes": 10080
  }
}
```

#### SendGrid (Email - Optional for MVP)
```json
{
  "SendGrid": {
    "ApiKey": "YOUR_API_KEY",
    "FromEmail": "noreply@bizbio.co.za",
    "FromName": "BizBio"
  }
}
```

#### PayFast (Payments - Optional for MVP)
```json
{
  "PayFast": {
    "MerchantId": "YOUR_MERCHANT_ID",
    "MerchantKey": "YOUR_MERCHANT_KEY",
    "PassPhrase": "YOUR_PASSPHRASE",
    "Url": "https://sandbox.payfast.co.za"
  }
}
```

## Testing the API

### Using Swagger UI

1. Navigate to http://localhost:5000
2. Try the `/api/v1/auth/register` endpoint
3. Register a new user
4. Use `/api/v1/auth/login` to get a JWT token
5. Click "Authorize" button in Swagger UI
6. Enter: `Bearer YOUR_JWT_TOKEN`
7. Now you can test protected endpoints

### Using cURL

**Register:**
```bash
curl -X POST http://localhost:5000/api/v1/auth/register \
  -H "Content-Type: application/json" \
  -d '{
    "email": "test@example.com",
    "password": "Password123",
    "firstName": "John",
    "lastName": "Doe"
  }'
```

**Login:**
```bash
curl -X POST http://localhost:5000/api/v1/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "email": "test@example.com",
    "password": "Password123"
  }'
```

**Get Profiles (with token):**
```bash
curl -X GET http://localhost:5000/api/v1/profiles \
  -H "Authorization: Bearer YOUR_JWT_TOKEN"
```

## Troubleshooting

### Database Connection Issues

**Error:** "Unable to connect to any of the specified MySQL hosts"

**Solution:**
1. Verify MySQL is running: `sudo systemctl status mysql`
2. Check connection string in `appsettings.Development.json`
3. Test connection: `mysql -u root -p`

### Migration Issues

**Error:** "A network-related or instance-specific error occurred"

**Solution:**
```bash
# Drop database and recreate
dotnet ef database drop --force
dotnet ef database update
```

### JWT Token Issues

**Error:** "IDX10503: Signature validation failed"

**Solution:**
- Ensure JWT:Secret in appsettings.json is at least 32 characters
- Verify the token hasn't expired (default: 7 days)
- Check that Issuer and Audience match configuration

### Port Already in Use

**Error:** "Address already in use"

**Solution:**
```bash
# Kill process on port 5000
lsof -ti:5000 | xargs kill -9

# Or use a different port
dotnet run --urls "http://localhost:5050"
```

## Next Steps

This MVP includes the core foundation. To complete the full Phase 1, you'll need to:

1. ✅ Core entities and database schema
2. ✅ Authentication system
3. ✅ Profile management
4. ✅ NFC table tags and event mode
5. ⏳ Email integration (SendGrid) - Optional
6. ⏳ Payment integration (PayFast) - Optional
7. ⏳ Frontend application (Vue.js 3)
8. ⏳ Deployment configuration

## Documentation

For complete specifications, see:

- [📄 Complete Documentation Package](./00-README-START-HERE.md)
- [🏗️ Foundation Technical Spec](./BizBio-FOUNDATION-Technical-Spec.md)
- [💳 Subscription System Spec](./BizBio-Subscription-System-Technical-Spec.md)
- [📱 NFC Menu Pro Spec](./BizBio-NFC-Menu-Pro-Technical-Spec.md)
- [🚀 Master Roadmap](./BizBio-NFC-MASTER-ROADMAP.md)
- [👨‍💻 Dev Starter Kit](./BizBio-DEV-STARTER-KIT.md)

## License

Copyright © 2025 BizBio. All rights reserved.

## Support

For questions or issues:
- Email: support@bizbio.co.za
- Documentation: See `/docs` folder
- Issues: [GitHub Issues](https://github.com/yourusername/BizBio.App/issues)

---

**Built with ❤️ using ASP.NET 6, Entity Framework Core, and MySQL**
