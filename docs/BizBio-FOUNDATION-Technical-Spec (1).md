# BizBio Platform - Complete Foundation Technical Specification

**Version:** 1.0  
**Date:** November 2025  
**Stack:** ASP.NET 6 + MySQL + Vue.js 3 + Tailwind CSS

---

## Table of Contents

1. [System Architecture](#1-system-architecture)
2. [Database Schema](#2-database-schema)
3. [Authentication System](#3-authentication-system)
4. [Subscription & Payment System](#4-subscription--payment-system)
5. [Email System](#5-email-system)
6. [API Endpoints](#6-api-endpoints)
7. [Frontend Components](#7-frontend-components)
8. [PayFast Integration](#8-payfast-integration)
9. [Deployment Setup](#9-deployment-setup)
10. [Environment Configuration](#10-environment-configuration)

---

## 1. System Architecture

### 1.1 Stack Overview

```
┌─────────────────────────────────────────────┐
│           Users (Browser)                    │
└──────────────────┬──────────────────────────┘
                   │
                   │ HTTPS
                   ▼
┌─────────────────────────────────────────────┐
│     NGINX (Reverse Proxy on Debian VPS)     │
└──────────┬────────────────────┬─────────────┘
           │                    │
           │                    │
           ▼                    ▼
┌──────────────────┐   ┌──────────────────────┐
│  Vue.js Frontend │   │  ASP.NET 6 API       │
│  (Port 3000)     │   │  (Port 5000)         │
│                  │   │                       │
│  - Vite          │   │  - JWT Auth          │
│  - Tailwind CSS  │   │  - PayFast SDK       │
│  - Pinia Store   │   │  - SendGrid          │
└──────────────────┘   └──────────┬───────────┘
                                  │
                                  │
                                  ▼
                       ┌──────────────────────┐
                       │  MySQL Database      │
                       │  (cPanel Hosting)    │
                       │                      │
                       │  - Users             │
                       │  - Subscriptions     │
                       │  - Payments          │
                       └──────────────────────┘
```

### 1.2 Technology Versions

| Component | Version | Notes |
|-----------|---------|-------|
| ASP.NET Core | 6.0 LTS | Backend API |
| Entity Framework Core | 6.0 | ORM |
| MySQL | 8.0+ | Database |
| Vue.js | 3.x | Frontend framework |
| Vite | 4.x | Build tool |
| Tailwind CSS | 3.x | Styling |
| Pinia | 2.x | State management |
| Node.js | 18 LTS | Frontend build |
| Debian | 11+ | VPS OS |
| NGINX | 1.18+ | Reverse proxy |

---

## 2. Database Schema

### 2.1 Users Table

```sql
CREATE TABLE Users (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    
    -- Basic Info
    Email VARCHAR(255) NOT NULL UNIQUE,
    PasswordHash VARCHAR(255) NOT NULL,
    FirstName VARCHAR(100) NOT NULL,
    LastName VARCHAR(100) NOT NULL,
    
    -- Status
    EmailVerified BOOLEAN DEFAULT FALSE,
    IsActive BOOLEAN DEFAULT TRUE,
    
    -- Security
    EmailVerificationToken VARCHAR(255) NULL,
    EmailVerificationTokenExpiry DATETIME NULL,
    PasswordResetToken VARCHAR(255) NULL,
    PasswordResetTokenExpiry DATETIME NULL,
    LastPasswordChangeAt DATETIME NULL,
    
    -- Login Tracking
    LastLoginAt DATETIME NULL,
    FailedLoginAttempts INT DEFAULT 0,
    LockoutEnd DATETIME NULL,
    
    -- Timestamps
    CreatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    
    -- Indexes
    INDEX idx_email (Email),
    INDEX idx_email_verified (EmailVerified),
    INDEX idx_active (IsActive)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
```

### 2.2 SubscriptionTiers Table

```sql
CREATE TABLE SubscriptionTiers (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    
    -- Product Line
    ProductLine ENUM('Professional', 'Menu', 'Retail') NOT NULL,
    TierName VARCHAR(50) NOT NULL,
    TierCode VARCHAR(50) NOT NULL UNIQUE,
    DisplayName VARCHAR(100) NOT NULL,
    Description TEXT,
    
    -- Pricing
    MonthlyPrice DECIMAL(10,2) NOT NULL,
    
    -- Trial
    TrialDays INT DEFAULT 14,
    
    -- Feature Limits
    MaxProfiles INT DEFAULT 0,
    MaxCatalogItems INT DEFAULT 0,
    MaxLocations INT DEFAULT 1,
    MaxTeamMembers INT DEFAULT 0,
    MaxDocuments INT DEFAULT 0,
    MaxDocumentSizeMB INT DEFAULT 5,
    
    -- Feature Flags
    CustomBranding BOOLEAN DEFAULT FALSE,
    RemoveBranding BOOLEAN DEFAULT FALSE,
    Analytics BOOLEAN DEFAULT FALSE,
    VCardDownload BOOLEAN DEFAULT FALSE,
    
    -- Status
    IsActive BOOLEAN DEFAULT TRUE,
    DisplayOrder INT DEFAULT 0,
    
    -- Timestamps
    CreatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    
    INDEX idx_product_line (ProductLine),
    INDEX idx_tier_code (TierCode),
    INDEX idx_active (IsActive)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
```

### 2.3 UserSubscriptions Table

```sql
CREATE TABLE UserSubscriptions (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    
    -- Foreign Keys
    UserId INT NOT NULL,
    TierId INT NOT NULL,
    
    -- Status
    Status ENUM('Trial', 'Active', 'PastDue', 'Cancelled', 'Expired') DEFAULT 'Trial',
    
    -- Trial Info
    TrialStartDate DATETIME NOT NULL,
    TrialEndDate DATETIME NOT NULL,
    TrialReminderSent BOOLEAN DEFAULT FALSE,
    
    -- Subscription Dates
    StartDate DATETIME NULL,  -- When paid subscription starts (after trial)
    EndDate DATETIME NULL,    -- Cancellation/expiry date
    NextBillingDate DATETIME NULL,
    CancelledAt DATETIME NULL,
    
    -- Pricing
    Price DECIMAL(10,2) NOT NULL,
    Currency VARCHAR(3) DEFAULT 'ZAR',
    
    -- PayFast
    PayFastToken VARCHAR(255) NULL,  -- PayFast subscription token
    PayFastSubscriptionId VARCHAR(255) NULL,
    
    -- Payment
    LastPaymentDate DATETIME NULL,
    LastPaymentAmount DECIMAL(10,2) NULL,
    LastPaymentStatus VARCHAR(50) NULL,
    
    -- Timestamps
    CreatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    
    -- Constraints
    FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE CASCADE,
    FOREIGN KEY (TierId) REFERENCES SubscriptionTiers(Id),
    
    INDEX idx_user_id (UserId),
    INDEX idx_status (Status),
    INDEX idx_trial_end (TrialEndDate),
    INDEX idx_next_billing (NextBillingDate)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
```

### 2.4 Payments Table

```sql
CREATE TABLE Payments (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    
    -- Foreign Keys
    UserId INT NOT NULL,
    SubscriptionId INT NULL,
    
    -- Payment Info
    Amount DECIMAL(10,2) NOT NULL,
    Currency VARCHAR(3) DEFAULT 'ZAR',
    Status ENUM('Pending', 'Complete', 'Failed', 'Refunded') DEFAULT 'Pending',
    
    -- PayFast
    PayFastPaymentId VARCHAR(255) NULL,
    PayFastToken VARCHAR(255) NULL,
    MerchantTransactionId VARCHAR(255) UNIQUE NOT NULL,
    
    -- Details
    PaymentMethod VARCHAR(50) NULL,
    Description TEXT NULL,
    
    -- Response Data (store PayFast IPN data)
    ResponseData JSON NULL,
    
    -- Timestamps
    PaidAt DATETIME NULL,
    CreatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    
    -- Constraints
    FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE CASCADE,
    FOREIGN KEY (SubscriptionId) REFERENCES UserSubscriptions(Id) ON DELETE SET NULL,
    
    INDEX idx_user_id (UserId),
    INDEX idx_subscription_id (SubscriptionId),
    INDEX idx_status (Status),
    INDEX idx_merchant_txn (MerchantTransactionId)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
```

### 2.5 SubscriptionAddOns Table

```sql
CREATE TABLE SubscriptionAddOns (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    
    AddOnCode VARCHAR(50) NOT NULL UNIQUE,
    Name VARCHAR(100) NOT NULL,
    Description TEXT,
    MonthlyPrice DECIMAL(10,2) NOT NULL,
    
    -- Applicable to
    ApplicableToProductLine ENUM('Professional', 'Menu', 'Retail', 'All') DEFAULT 'All',
    
    IsActive BOOLEAN DEFAULT TRUE,
    DisplayOrder INT DEFAULT 0,
    
    CreatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    
    INDEX idx_addon_code (AddOnCode),
    INDEX idx_active (IsActive)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
```

### 2.6 UserAddOns Table

```sql
CREATE TABLE UserAddOns (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    
    UserId INT NOT NULL,
    SubscriptionId INT NOT NULL,
    AddOnId INT NOT NULL,
    
    Status ENUM('Active', 'Cancelled') DEFAULT 'Active',
    Price DECIMAL(10,2) NOT NULL,
    
    StartDate DATETIME NOT NULL,
    EndDate DATETIME NULL,
    
    CreatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    
    FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE CASCADE,
    FOREIGN KEY (SubscriptionId) REFERENCES UserSubscriptions(Id) ON DELETE CASCADE,
    FOREIGN KEY (AddOnId) REFERENCES SubscriptionAddOns(Id),
    
    INDEX idx_user_id (UserId),
    INDEX idx_subscription_id (SubscriptionId),
    INDEX idx_status (Status)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
```

### 2.7 EmailLogs Table

```sql
CREATE TABLE EmailLogs (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    
    UserId INT NULL,
    
    -- Email Info
    ToEmail VARCHAR(255) NOT NULL,
    FromEmail VARCHAR(255) NOT NULL,
    Subject VARCHAR(255) NOT NULL,
    EmailType ENUM(
        'EmailVerification',
        'Welcome',
        'PasswordReset',
        'TrialReminder',
        'TrialEnded',
        'PaymentSuccess',
        'PaymentFailed',
        'SubscriptionCancelled'
    ) NOT NULL,
    
    -- Status
    Status ENUM('Queued', 'Sent', 'Failed') DEFAULT 'Queued',
    ErrorMessage TEXT NULL,
    
    -- Provider
    Provider VARCHAR(50) DEFAULT 'SendGrid',
    ProviderMessageId VARCHAR(255) NULL,
    
    -- Timestamps
    SentAt DATETIME NULL,
    CreatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    
    FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE SET NULL,
    
    INDEX idx_user_id (UserId),
    INDEX idx_email_type (EmailType),
    INDEX idx_status (Status),
    INDEX idx_created_at (CreatedAt)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
```

---

## 3. Authentication System

### 3.1 User Registration Flow

```
User submits registration form
  ↓
Validate email uniqueness
  ↓
Hash password (BCrypt)
  ↓
Generate email verification token
  ↓
Create User record (EmailVerified = FALSE)
  ↓
Send verification email
  ↓
Return success (user can't login yet)
  ↓
User clicks verification link
  ↓
Verify token & mark EmailVerified = TRUE
  ↓
Send welcome email
  ↓
User can now login
```

### 3.2 C# Entity Models

**File:** `Entities/User.cs`

```csharp
using System;
using System.ComponentModel.DataAnnotations;

namespace BizBio.Core.Entities
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        
        // Basic Info
        [Required]
        [MaxLength(255)]
        [EmailAddress]
        public string Email { get; set; }
        
        [Required]
        [MaxLength(255)]
        public string PasswordHash { get; set; }
        
        [Required]
        [MaxLength(100)]
        public string FirstName { get; set; }
        
        [Required]
        [MaxLength(100)]
        public string LastName { get; set; }
        
        // Status
        public bool EmailVerified { get; set; }
        public bool IsActive { get; set; } = true;
        
        // Security
        [MaxLength(255)]
        public string EmailVerificationToken { get; set; }
        public DateTime? EmailVerificationTokenExpiry { get; set; }
        
        [MaxLength(255)]
        public string PasswordResetToken { get; set; }
        public DateTime? PasswordResetTokenExpiry { get; set; }
        public DateTime? LastPasswordChangeAt { get; set; }
        
        // Login Tracking
        public DateTime? LastLoginAt { get; set; }
        public int FailedLoginAttempts { get; set; }
        public DateTime? LockoutEnd { get; set; }
        
        // Timestamps
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        
        // Computed Property
        public string FullName => $"{FirstName} {LastName}";
        
        // Navigation Properties
        public virtual ICollection<UserSubscription> Subscriptions { get; set; }
        public virtual ICollection<Payment> Payments { get; set; }
    }
}
```

### 3.3 Authentication Service

**File:** `Services/AuthService.cs`

```csharp
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using BizBio.Core.Entities;
using BizBio.Core.Interfaces;
using BCrypt.Net;

namespace BizBio.Core.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepo;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _configuration;
        
        public AuthService(
            IUserRepository userRepo,
            IEmailService emailService,
            IConfiguration configuration)
        {
            _userRepo = userRepo;
            _emailService = emailService;
            _configuration = configuration;
        }
        
        public async Task<AuthResult> RegisterAsync(RegisterDto dto)
        {
            // Check if email exists
            var existingUser = await _userRepo.GetByEmailAsync(dto.Email);
            if (existingUser != null)
            {
                return AuthResult.Failure("Email already registered");
            }
            
            // Create user
            var user = new User
            {
                Email = dto.Email.ToLower().Trim(),
                FirstName = dto.FirstName.Trim(),
                LastName = dto.LastName.Trim(),
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                EmailVerified = false,
                EmailVerificationToken = GenerateSecureToken(),
                EmailVerificationTokenExpiry = DateTime.UtcNow.AddHours(24),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            
            await _userRepo.AddAsync(user);
            await _userRepo.SaveChangesAsync();
            
            // Send verification email
            await _emailService.SendEmailVerificationAsync(
                user.Email,
                user.FirstName,
                user.EmailVerificationToken
            );
            
            return AuthResult.Success(user);
        }
        
        public async Task<AuthResult> LoginAsync(LoginDto dto)
        {
            var user = await _userRepo.GetByEmailAsync(dto.Email);
            
            if (user == null)
            {
                return AuthResult.Failure("Invalid credentials");
            }
            
            // Check lockout
            if (user.LockoutEnd.HasValue && user.LockoutEnd > DateTime.UtcNow)
            {
                var minutesLeft = (user.LockoutEnd.Value - DateTime.UtcNow).Minutes;
                return AuthResult.Failure($"Account locked. Try again in {minutesLeft} minutes.");
            }
            
            // Verify password
            if (!BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
            {
                // Increment failed attempts
                user.FailedLoginAttempts++;
                
                if (user.FailedLoginAttempts >= 5)
                {
                    user.LockoutEnd = DateTime.UtcNow.AddMinutes(30);
                    await _userRepo.UpdateAsync(user);
                    return AuthResult.Failure("Account locked due to too many failed attempts");
                }
                
                await _userRepo.UpdateAsync(user);
                return AuthResult.Failure("Invalid credentials");
            }
            
            // Check email verification
            if (!user.EmailVerified)
            {
                return AuthResult.Failure("Please verify your email before logging in");
            }
            
            // Check if active
            if (!user.IsActive)
            {
                return AuthResult.Failure("Account is inactive");
            }
            
            // Reset failed attempts and update last login
            user.FailedLoginAttempts = 0;
            user.LockoutEnd = null;
            user.LastLoginAt = DateTime.UtcNow;
            await _userRepo.UpdateAsync(user);
            
            // Generate JWT token
            var token = GenerateJwtToken(user);
            
            return AuthResult.Success(user, token);
        }
        
        public async Task<bool> VerifyEmailAsync(string token)
        {
            var user = await _userRepo.GetByEmailVerificationTokenAsync(token);
            
            if (user == null)
            {
                return false;
            }
            
            if (user.EmailVerificationTokenExpiry < DateTime.UtcNow)
            {
                return false;
            }
            
            user.EmailVerified = true;
            user.EmailVerificationToken = null;
            user.EmailVerificationTokenExpiry = null;
            user.UpdatedAt = DateTime.UtcNow;
            
            await _userRepo.UpdateAsync(user);
            
            // Send welcome email
            await _emailService.SendWelcomeEmailAsync(user.Email, user.FirstName);
            
            return true;
        }
        
        public async Task<bool> RequestPasswordResetAsync(string email)
        {
            var user = await _userRepo.GetByEmailAsync(email);
            
            if (user == null)
            {
                // Don't reveal that email doesn't exist
                return true;
            }
            
            user.PasswordResetToken = GenerateSecureToken();
            user.PasswordResetTokenExpiry = DateTime.UtcNow.AddHours(1);
            user.UpdatedAt = DateTime.UtcNow;
            
            await _userRepo.UpdateAsync(user);
            
            // Send password reset email
            await _emailService.SendPasswordResetAsync(
                user.Email,
                user.FirstName,
                user.PasswordResetToken
            );
            
            return true;
        }
        
        public async Task<bool> ResetPasswordAsync(string token, string newPassword)
        {
            var user = await _userRepo.GetByPasswordResetTokenAsync(token);
            
            if (user == null)
            {
                return false;
            }
            
            if (user.PasswordResetTokenExpiry < DateTime.UtcNow)
            {
                return false;
            }
            
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
            user.PasswordResetToken = null;
            user.PasswordResetTokenExpiry = null;
            user.LastPasswordChangeAt = DateTime.UtcNow;
            user.UpdatedAt = DateTime.UtcNow;
            
            await _userRepo.UpdateAsync(user);
            
            return true;
        }
        
        private string GenerateJwtToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["JWT:Secret"]);
            
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Name, user.FullName)
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature
                )
            };
            
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
        
        private string GenerateSecureToken()
        {
            var randomBytes = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomBytes);
            }
            return Convert.ToBase64String(randomBytes)
                .Replace("+", "")
                .Replace("/", "")
                .Replace("=", "");
        }
    }
    
    public class AuthResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public User User { get; set; }
        public string Token { get; set; }
        
        public static AuthResult Success(User user, string token = null)
        {
            return new AuthResult
            {
                Success = true,
                User = user,
                Token = token
            };
        }
        
        public static AuthResult Failure(string message)
        {
            return new AuthResult
            {
                Success = false,
                Message = message
            };
        }
    }
}
```

---

## 4. Subscription & Payment System

### 4.1 Trial & Billing Flow

```
User completes registration → Email verified
  ↓
User selects subscription tier
  ↓
Create UserSubscription (Status = 'Trial')
  - TrialStartDate = NOW
  - TrialEndDate = NOW + 14 days
  - Status = 'Trial'
  ↓
User gets full access immediately
  ↓
Day 12 (2 days before trial ends)
  - Send trial reminder email
  - TrialReminderSent = TRUE
  ↓
Day 14 (trial ends)
  - Redirect to PayFast for payment
  - Create PayFast subscription
  ↓
Payment successful (IPN callback)
  - Status = 'Active'
  - StartDate = NOW
  - NextBillingDate = NOW + 30 days
  - Create Payment record
  ↓
Monthly billing
  - PayFast sends IPN on each payment
  - Update NextBillingDate
  - Create Payment record
```

### 4.2 Subscription Service

**File:** `Services/SubscriptionService.cs`

```csharp
using System;
using System.Threading.Tasks;
using BizBio.Core.Entities;
using BizBio.Core.Interfaces;

namespace BizBio.Core.Services
{
    public class SubscriptionService : ISubscriptionService
    {
        private readonly IUserSubscriptionRepository _subscriptionRepo;
        private readonly ISubscriptionTierRepository _tierRepo;
        private readonly IPaymentService _paymentService;
        private readonly IEmailService _emailService;
        
        public SubscriptionService(
            IUserSubscriptionRepository subscriptionRepo,
            ISubscriptionTierRepository tierRepo,
            IPaymentService paymentService,
            IEmailService emailService)
        {
            _subscriptionRepo = subscriptionRepo;
            _tierRepo = tierRepo;
            _paymentService = paymentService;
            _emailService = emailService;
        }
        
        public async Task<UserSubscription> StartTrialAsync(int userId, int tierId)
        {
            var tier = await _tierRepo.GetByIdAsync(tierId);
            if (tier == null)
            {
                throw new Exception("Subscription tier not found");
            }
            
            // Check if user already has active subscription for this product line
            var existing = await _subscriptionRepo
                .GetActiveSubscriptionByProductLineAsync(userId, tier.ProductLine);
            
            if (existing != null)
            {
                throw new Exception($"You already have an active {tier.ProductLine} subscription");
            }
            
            var now = DateTime.UtcNow;
            var trialEnd = now.AddDays(tier.TrialDays);
            
            var subscription = new UserSubscription
            {
                UserId = userId,
                TierId = tierId,
                Status = SubscriptionStatus.Trial,
                TrialStartDate = now,
                TrialEndDate = trialEnd,
                Price = tier.MonthlyPrice,
                Currency = "ZAR",
                CreatedAt = now,
                UpdatedAt = now
            };
            
            await _subscriptionRepo.AddAsync(subscription);
            await _subscriptionRepo.SaveChangesAsync();
            
            return subscription;
        }
        
        public async Task<string> GetPayFastPaymentUrlAsync(int subscriptionId, string returnUrl, string cancelUrl)
        {
            var subscription = await _subscriptionRepo.GetByIdAsync(subscriptionId);
            if (subscription == null)
            {
                throw new Exception("Subscription not found");
            }
            
            // Generate PayFast payment URL
            var paymentUrl = await _paymentService.CreateSubscriptionPaymentAsync(
                subscription,
                returnUrl,
                cancelUrl
            );
            
            return paymentUrl;
        }
        
        public async Task HandlePaymentSuccessAsync(int subscriptionId, string payFastToken)
        {
            var subscription = await _subscriptionRepo.GetByIdAsync(subscriptionId);
            if (subscription == null)
            {
                throw new Exception("Subscription not found");
            }
            
            var now = DateTime.UtcNow;
            
            subscription.Status = SubscriptionStatus.Active;
            subscription.StartDate = now;
            subscription.NextBillingDate = now.AddMonths(1);
            subscription.PayFastToken = payFastToken;
            subscription.LastPaymentDate = now;
            subscription.LastPaymentAmount = subscription.Price;
            subscription.LastPaymentStatus = "Complete";
            subscription.UpdatedAt = now;
            
            await _subscriptionRepo.UpdateAsync(subscription);
            
            // Send payment success email
            var user = subscription.User;
            await _emailService.SendPaymentSuccessEmailAsync(
                user.Email,
                user.FirstName,
                subscription.Price
            );
        }
        
        public async Task CancelSubscriptionAsync(int subscriptionId, int userId)
        {
            var subscription = await _subscriptionRepo.GetByIdAsync(subscriptionId);
            
            if (subscription == null || subscription.UserId != userId)
            {
                throw new Exception("Subscription not found");
            }
            
            // Cancel on PayFast
            if (!string.IsNullOrEmpty(subscription.PayFastToken))
            {
                await _paymentService.CancelPayFastSubscriptionAsync(subscription.PayFastToken);
            }
            
            subscription.Status = SubscriptionStatus.Cancelled;
            subscription.CancelledAt = DateTime.UtcNow;
            subscription.EndDate = subscription.NextBillingDate; // Active until next billing
            subscription.UpdatedAt = DateTime.UtcNow;
            
            await _subscriptionRepo.UpdateAsync(subscription);
            
            // Send cancellation email
            var user = subscription.User;
            await _emailService.SendSubscriptionCancelledEmailAsync(
                user.Email,
                user.FirstName,
                subscription.EndDate.Value
            );
        }
    }
    
    public enum SubscriptionStatus
    {
        Trial,
        Active,
        PastDue,
        Cancelled,
        Expired
    }
}
```

---

## 5. Email System

### 5.1 Email Service Interface

**File:** `Interfaces/IEmailService.cs`

```csharp
using System.Threading.Tasks;

namespace BizBio.Core.Interfaces
{
    public interface IEmailService
    {
        Task SendEmailVerificationAsync(string toEmail, string firstName, string token);
        Task SendWelcomeEmailAsync(string toEmail, string firstName);
        Task SendPasswordResetAsync(string toEmail, string firstName, string token);
        Task SendTrialReminderAsync(string toEmail, string firstName, DateTime trialEndDate);
        Task SendTrialEndedAsync(string toEmail, string firstName);
        Task SendPaymentSuccessEmailAsync(string toEmail, string firstName, decimal amount);
        Task SendPaymentFailedEmailAsync(string toEmail, string firstName, decimal amount);
        Task SendSubscriptionCancelledEmailAsync(string toEmail, string firstName, DateTime endDate);
    }
}
```

### 5.2 SendGrid Email Service

**File:** `Services/SendGridEmailService.cs`

```csharp
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using SendGrid;
using SendGrid.Helpers.Mail;
using BizBio.Core.Entities;
using BizBio.Core.Interfaces;

namespace BizBio.Infrastructure.Services
{
    public class SendGridEmailService : IEmailService
    {
        private readonly ISendGridClient _sendGridClient;
        private readonly IConfiguration _configuration;
        private readonly IEmailLogRepository _emailLogRepo;
        
        private string FromEmail => _configuration["SendGrid:FromEmail"];
        private string FromName => _configuration["SendGrid:FromName"];
        private string WebsiteUrl => _configuration["Website:Url"];
        
        public SendGridEmailService(
            ISendGridClient sendGridClient,
            IConfiguration configuration,
            IEmailLogRepository emailLogRepo)
        {
            _sendGridClient = sendGridClient;
            _configuration = configuration;
            _emailLogRepo = emailLogRepo;
        }
        
        public async Task SendEmailVerificationAsync(string toEmail, string firstName, string token)
        {
            var subject = "Verify Your BizBio Account";
            var verifyUrl = $"{WebsiteUrl}/verify-email?token={token}";
            
            var htmlContent = $@"
                <h2>Hi {firstName},</h2>
                <p>Welcome to BizBio! Please verify your email address to get started.</p>
                <p><a href=""{verifyUrl}"" style=""background-color:#4F46E5;color:white;padding:12px 24px;text-decoration:none;border-radius:6px;display:inline-block;"">Verify Email Address</a></p>
                <p>Or copy and paste this link into your browser:</p>
                <p>{verifyUrl}</p>
                <p>This link expires in 24 hours.</p>
                <p>If you didn't create a BizBio account, you can safely ignore this email.</p>
                <p>Thanks,<br>The BizBio Team</p>
            ";
            
            await SendEmailAsync(toEmail, subject, htmlContent, "EmailVerification");
        }
        
        public async Task SendWelcomeEmailAsync(string toEmail, string firstName)
        {
            var subject = "Welcome to BizBio! 🎉";
            var dashboardUrl = $"{WebsiteUrl}/dashboard";
            
            var htmlContent = $@"
                <h2>Welcome aboard, {firstName}!</h2>
                <p>Your email has been verified and your account is ready to go!</p>
                <p>You now have a <strong>14-day free trial</strong> with full access to all features.</p>
                <h3>What's next?</h3>
                <ol>
                    <li>Choose your subscription plan</li>
                    <li>Create your first digital profile or menu</li>
                    <li>Customize with your branding</li>
                </ol>
                <p><a href=""{dashboardUrl}"" style=""background-color:#4F46E5;color:white;padding:12px 24px;text-decoration:none;border-radius:6px;display:inline-block;"">Go to Dashboard</a></p>
                <p>Need help getting started? Check out our <a href=""{WebsiteUrl}/help"">Getting Started Guide</a>.</p>
                <p>Happy to have you here!<br>The BizBio Team</p>
            ";
            
            await SendEmailAsync(toEmail, subject, htmlContent, "Welcome");
        }
        
        public async Task SendPasswordResetAsync(string toEmail, string firstName, string token)
        {
            var subject = "Reset Your BizBio Password";
            var resetUrl = $"{WebsiteUrl}/reset-password?token={token}";
            
            var htmlContent = $@"
                <h2>Hi {firstName},</h2>
                <p>We received a request to reset your password for your BizBio account.</p>
                <p><a href=""{resetUrl}"" style=""background-color:#4F46E5;color:white;padding:12px 24px;text-decoration:none;border-radius:6px;display:inline-block;"">Reset Password</a></p>
                <p>Or copy and paste this link into your browser:</p>
                <p>{resetUrl}</p>
                <p>This link expires in 1 hour.</p>
                <p>If you didn't request a password reset, you can safely ignore this email. Your password will not be changed.</p>
                <p>Thanks,<br>The BizBio Team</p>
            ";
            
            await SendEmailAsync(toEmail, subject, htmlContent, "PasswordReset");
        }
        
        public async Task SendTrialReminderAsync(string toEmail, string firstName, DateTime trialEndDate)
        {
            var subject = "Your BizBio Trial Ends in 2 Days";
            var billingUrl = $"{WebsiteUrl}/dashboard/billing";
            
            var htmlContent = $@"
                <h2>Hi {firstName},</h2>
                <p>Your 14-day free trial is ending soon!</p>
                <p><strong>Trial ends:</strong> {trialEndDate:MMMM dd, yyyy} at {trialEndDate:h:mm tt}</p>
                <h3>Don't lose access to your profiles!</h3>
                <p>To continue using BizBio after your trial ends, please add your payment information.</p>
                <p><a href=""{billingUrl}"" style=""background-color:#4F46E5;color:white;padding:12px 24px;text-decoration:none;border-radius:6px;display:inline-block;"">Add Payment Method</a></p>
                <h3>What happens if I don't add payment?</h3>
                <ul>
                    <li>Your profiles will become private (not visible to public)</li>
                    <li>You won't be able to edit or create new content</li>
                    <li>You can reactivate anytime by adding payment</li>
                </ul>
                <p>Questions? Reply to this email and we'll help!</p>
                <p>Thanks,<br>The BizBio Team</p>
            ";
            
            await SendEmailAsync(toEmail, subject, htmlContent, "TrialReminder");
        }
        
        public async Task SendTrialEndedAsync(string toEmail, string firstName)
        {
            var subject = "Your BizBio Trial Has Ended";
            var billingUrl = $"{WebsiteUrl}/dashboard/billing";
            
            var htmlContent = $@"
                <h2>Hi {firstName},</h2>
                <p>Your 14-day free trial has ended.</p>
                <p>Your profiles are now private and you won't be able to make changes until you add a payment method.</p>
                <p><a href=""{billingUrl}"" style=""background-color:#4F46E5;color:white;padding:12px 24px;text-decoration:none;border-radius:6px;display:inline-block;"">Activate Subscription</a></p>
                <p>All your data is safely stored. Reactivate anytime to pick up where you left off!</p>
                <p>Thanks,<br>The BizBio Team</p>
            ";
            
            await SendEmailAsync(toEmail, subject, htmlContent, "TrialEnded");
        }
        
        public async Task SendPaymentSuccessEmailAsync(string toEmail, string firstName, decimal amount)
        {
            var subject = "Payment Received - Thank You!";
            
            var htmlContent = $@"
                <h2>Hi {firstName},</h2>
                <p>Thank you! We've received your payment.</p>
                <p><strong>Amount:</strong> R{amount:N2}</p>
                <p><strong>Date:</strong> {DateTime.UtcNow:MMMM dd, yyyy}</p>
                <p>Your subscription is now active and you have full access to all features.</p>
                <p>Need a receipt? You can download it from your <a href=""{WebsiteUrl}/dashboard/billing"">billing dashboard</a>.</p>
                <p>Thanks for being a valued customer!<br>The BizBio Team</p>
            ";
            
            await SendEmailAsync(toEmail, subject, htmlContent, "PaymentSuccess");
        }
        
        public async Task SendPaymentFailedEmailAsync(string toEmail, string firstName, decimal amount)
        {
            var subject = "Payment Failed - Action Required";
            var billingUrl = $"{WebsiteUrl}/dashboard/billing";
            
            var htmlContent = $@"
                <h2>Hi {firstName},</h2>
                <p>We couldn't process your payment of <strong>R{amount:N2}</strong>.</p>
                <p>This could be due to:</p>
                <ul>
                    <li>Insufficient funds</li>
                    <li>Expired card</li>
                    <li>Bank declined the transaction</li>
                </ul>
                <p><a href=""{billingUrl}"" style=""background-color:#EF4444;color:white;padding:12px 24px;text-decoration:none;border-radius:6px;display:inline-block;"">Update Payment Method</a></p>
                <p>Please update your payment information to avoid service interruption.</p>
                <p>Questions? Reply to this email and we'll help!</p>
                <p>Thanks,<br>The BizBio Team</p>
            ";
            
            await SendEmailAsync(toEmail, subject, htmlContent, "PaymentFailed");
        }
        
        public async Task SendSubscriptionCancelledEmailAsync(string toEmail, string firstName, DateTime endDate)
        {
            var subject = "Subscription Cancelled";
            
            var htmlContent = $@"
                <h2>Hi {firstName},</h2>
                <p>We've cancelled your subscription as requested.</p>
                <p>You'll continue to have access until <strong>{endDate:MMMM dd, yyyy}</strong>.</p>
                <p>After that:</p>
                <ul>
                    <li>Your profiles will become private</li>
                    <li>No further charges will be made</li>
                    <li>Your data will be saved for 30 days</li>
                </ul>
                <p>Changed your mind? You can reactivate anytime from your <a href=""{WebsiteUrl}/dashboard/billing"">billing dashboard</a>.</p>
                <p>We're sorry to see you go. If there's anything we could have done better, please let us know by replying to this email.</p>
                <p>Thanks for using BizBio!<br>The BizBio Team</p>
            ";
            
            await SendEmailAsync(toEmail, subject, htmlContent, "SubscriptionCancelled");
        }
        
        private async Task SendEmailAsync(string toEmail, string subject, string htmlContent, string emailType)
        {
            var from = new EmailAddress(FromEmail, FromName);
            var to = new EmailAddress(toEmail);
            var msg = MailHelper.CreateSingleEmail(from, to, subject, "", htmlContent);
            
            // Log email
            var emailLog = new EmailLog
            {
                ToEmail = toEmail,
                FromEmail = FromEmail,
                Subject = subject,
                EmailType = emailType,
                Status = "Queued",
                Provider = "SendGrid",
                CreatedAt = DateTime.UtcNow
            };
            
            try
            {
                var response = await _sendGridClient.SendEmailAsync(msg);
                
                if (response.IsSuccessStatusCode)
                {
                    emailLog.Status = "Sent";
                    emailLog.SentAt = DateTime.UtcNow;
                    var messageId = response.Headers.GetValues("X-Message-Id").FirstOrDefault();
                    emailLog.ProviderMessageId = messageId;
                }
                else
                {
                    emailLog.Status = "Failed";
                    emailLog.ErrorMessage = await response.Body.ReadAsStringAsync();
                }
            }
            catch (Exception ex)
            {
                emailLog.Status = "Failed";
                emailLog.ErrorMessage = ex.Message;
            }
            finally
            {
                await _emailLogRepo.AddAsync(emailLog);
                await _emailLogRepo.SaveChangesAsync();
            }
        }
    }
}
```

### 5.3 Background Job - Trial Reminder

**File:** `Jobs/TrialReminderJob.cs`

```csharp
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using BizBio.Core.Interfaces;

namespace BizBio.Infrastructure.Jobs
{
    public class TrialReminderJob : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<TrialReminderJob> _logger;
        
        public TrialReminderJob(
            IServiceProvider serviceProvider,
            ILogger<TrialReminderJob> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }
        
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await ProcessTrialRemindersAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing trial reminders");
                }
                
                // Run every hour
                await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
            }
        }
        
        private async Task ProcessTrialRemindersAsync()
        {
            using var scope = _serviceProvider.CreateScope();
            var subscriptionRepo = scope.ServiceProvider
                .GetRequiredService<IUserSubscriptionRepository>();
            var emailService = scope.ServiceProvider
                .GetRequiredService<IEmailService>();
            
            var now = DateTime.UtcNow;
            var reminderDate = now.AddDays(2); // 2 days from now
            
            // Get trials ending in ~2 days that haven't been reminded
            var trials = await subscriptionRepo.GetTrialsEndingBetweenAsync(
                reminderDate.AddHours(-1),
                reminderDate.AddHours(1)
            );
            
            var trialsToRemind = trials
                .Where(t => !t.TrialReminderSent && t.Status == "Trial")
                .ToList();
            
            foreach (var trial in trialsToRemind)
            {
                try
                {
                    await emailService.SendTrialReminderAsync(
                        trial.User.Email,
                        trial.User.FirstName,
                        trial.TrialEndDate
                    );
                    
                    trial.TrialReminderSent = true;
                    await subscriptionRepo.UpdateAsync(trial);
                    
                    _logger.LogInformation(
                        $"Sent trial reminder to user {trial.UserId}"
                    );
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, 
                        $"Failed to send trial reminder to user {trial.UserId}"
                    );
                }
            }
            
            await subscriptionRepo.SaveChangesAsync();
        }
    }
}
```

---

## 6. API Endpoints

### 6.1 Authentication Controller

**File:** `Controllers/AuthController.cs`

```csharp
using Microsoft.AspNetCore.Mvc;
using BizBio.Core.Interfaces;
using BizBio.Core.DTOs;
using System.Threading.Tasks;

namespace BizBio.API.Controllers
{
    [Route("api/v1/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }
        
        /// <summary>
        /// Register a new user
        /// </summary>
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            
            var result = await _authService.RegisterAsync(dto);
            
            if (!result.Success)
            {
                return BadRequest(new { message = result.Message });
            }
            
            return Ok(new
            {
                message = "Registration successful! Please check your email to verify your account.",
                email = dto.Email
            });
        }
        
        /// <summary>
        /// Login user
        /// </summary>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            
            var result = await _authService.LoginAsync(dto);
            
            if (!result.Success)
            {
                return Unauthorized(new { message = result.Message });
            }
            
            return Ok(new
            {
                token = result.Token,
                user = new
                {
                    id = result.User.Id,
                    email = result.User.Email,
                    firstName = result.User.FirstName,
                    lastName = result.User.LastName
                }
            });
        }
        
        /// <summary>
        /// Verify email address
        /// </summary>
        [HttpGet("verify-email")]
        public async Task<IActionResult> VerifyEmail([FromQuery] string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                return BadRequest(new { message = "Token is required" });
            }
            
            var success = await _authService.VerifyEmailAsync(token);
            
            if (!success)
            {
                return BadRequest(new { message = "Invalid or expired token" });
            }
            
            return Ok(new { message = "Email verified successfully! You can now login." });
        }
        
        /// <summary>
        /// Request password reset
        /// </summary>
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto dto)
        {
            await _authService.RequestPasswordResetAsync(dto.Email);
            
            return Ok(new 
            { 
                message = "If that email exists, we've sent password reset instructions." 
            });
        }
        
        /// <summary>
        /// Reset password
        /// </summary>
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            
            var success = await _authService.ResetPasswordAsync(dto.Token, dto.NewPassword);
            
            if (!success)
            {
                return BadRequest(new { message = "Invalid or expired token" });
            }
            
            return Ok(new { message = "Password reset successfully! You can now login." });
        }
    }
}
```

### 6.2 DTOs

**File:** `DTOs/AuthDTOs.cs`

```csharp
using System.ComponentModel.DataAnnotations;

namespace BizBio.Core.DTOs
{
    public class RegisterDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        
        [Required]
        [MinLength(8)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,}$", 
            ErrorMessage = "Password must contain at least one uppercase, one lowercase, and one number")]
        public string Password { get; set; }
        
        [Required]
        [MinLength(2)]
        [MaxLength(100)]
        public string FirstName { get; set; }
        
        [Required]
        [MinLength(2)]
        [MaxLength(100)]
        public string LastName { get; set; }
    }
    
    public class LoginDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        
        [Required]
        public string Password { get; set; }
    }
    
    public class ForgotPasswordDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
    
    public class ResetPasswordDto
    {
        [Required]
        public string Token { get; set; }
        
        [Required]
        [MinLength(8)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,}$")]
        public string NewPassword { get; set; }
    }
}
```

### 6.3 Subscription Controller

**File:** `Controllers/SubscriptionController.cs`

```csharp
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BizBio.Core.Interfaces;
using BizBio.Core.DTOs;
using System.Threading.Tasks;
using System.Security.Claims;

namespace BizBio.API.Controllers
{
    [Route("api/v1/subscriptions")]
    [ApiController]
    [Authorize]
    public class SubscriptionController : ControllerBase
    {
        private readonly ISubscriptionService _subscriptionService;
        private readonly ISubscriptionTierRepository _tierRepo;
        
        public SubscriptionController(
            ISubscriptionService subscriptionService,
            ISubscriptionTierRepository tierRepo)
        {
            _subscriptionService = subscriptionService;
            _tierRepo = tierRepo;
        }
        
        private int GetUserId() => int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
        
        /// <summary>
        /// Get available subscription tiers
        /// </summary>
        [HttpGet("tiers")]
        [AllowAnonymous]
        public async Task<IActionResult> GetTiers([FromQuery] string productLine = null)
        {
            var tiers = string.IsNullOrEmpty(productLine)
                ? await _tierRepo.GetAllActiveAsync()
                : await _tierRepo.GetByProductLineAsync(productLine);
            
            return Ok(tiers);
        }
        
        /// <summary>
        /// Start a trial subscription
        /// </summary>
        [HttpPost("start-trial")]
        public async Task<IActionResult> StartTrial([FromBody] StartTrialDto dto)
        {
            var userId = GetUserId();
            
            try
            {
                var subscription = await _subscriptionService.StartTrialAsync(userId, dto.TierId);
                
                return Ok(new
                {
                    message = "Trial started successfully!",
                    subscription = new
                    {
                        id = subscription.Id,
                        trialEndDate = subscription.TrialEndDate,
                        status = subscription.Status
                    }
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        
        /// <summary>
        /// Get PayFast payment URL for subscription
        /// </summary>
        [HttpPost("{subscriptionId}/payment-url")]
        public async Task<IActionResult> GetPaymentUrl(
            [FromRoute] int subscriptionId,
            [FromBody] PaymentUrlDto dto)
        {
            try
            {
                var paymentUrl = await _subscriptionService.GetPayFastPaymentUrlAsync(
                    subscriptionId,
                    dto.ReturnUrl,
                    dto.CancelUrl
                );
                
                return Ok(new { paymentUrl });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        
        /// <summary>
        /// Cancel subscription
        /// </summary>
        [HttpPost("{subscriptionId}/cancel")]
        public async Task<IActionResult> CancelSubscription([FromRoute] int subscriptionId)
        {
            var userId = GetUserId();
            
            try
            {
                await _subscriptionService.CancelSubscriptionAsync(subscriptionId, userId);
                
                return Ok(new { message = "Subscription cancelled successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
```

---

## 7. Frontend Components

### 7.1 Registration Page

**File:** `src/views/Auth/Register.vue`

```vue
<template>
  <div class="min-h-screen flex items-center justify-center bg-gray-50 py-12 px-4 sm:px-6 lg:px-8">
    <div class="max-w-md w-full space-y-8">
      <div>
        <img class="mx-auto h-12 w-auto" src="/logo.svg" alt="BizBio" />
        <h2 class="mt-6 text-center text-3xl font-extrabold text-gray-900">
          Create your account
        </h2>
        <p class="mt-2 text-center text-sm text-gray-600">
          Start your 14-day free trial
        </p>
      </div>
      
      <form class="mt-8 space-y-6" @submit.prevent="handleRegister">
        <div class="rounded-md shadow-sm space-y-4">
          <div class="grid grid-cols-2 gap-4">
            <div>
              <label for="firstName" class="sr-only">First name</label>
              <input
                id="firstName"
                v-model="form.firstName"
                type="text"
                required
                class="appearance-none rounded relative block w-full px-3 py-2 border border-gray-300 placeholder-gray-500 text-gray-900 focus:outline-none focus:ring-indigo-500 focus:border-indigo-500 focus:z-10 sm:text-sm"
                placeholder="First name"
              />
            </div>
            <div>
              <label for="lastName" class="sr-only">Last name</label>
              <input
                id="lastName"
                v-model="form.lastName"
                type="text"
                required
                class="appearance-none rounded relative block w-full px-3 py-2 border border-gray-300 placeholder-gray-500 text-gray-900 focus:outline-none focus:ring-indigo-500 focus:border-indigo-500 focus:z-10 sm:text-sm"
                placeholder="Last name"
              />
            </div>
          </div>
          
          <div>
            <label for="email" class="sr-only">Email address</label>
            <input
              id="email"
              v-model="form.email"
              type="email"
              required
              class="appearance-none rounded relative block w-full px-3 py-2 border border-gray-300 placeholder-gray-500 text-gray-900 focus:outline-none focus:ring-indigo-500 focus:border-indigo-500 focus:z-10 sm:text-sm"
              placeholder="Email address"
            />
          </div>
          
          <div>
            <label for="password" class="sr-only">Password</label>
            <input
              id="password"
              v-model="form.password"
              type="password"
              required
              class="appearance-none rounded relative block w-full px-3 py-2 border border-gray-300 placeholder-gray-500 text-gray-900 focus:outline-none focus:ring-indigo-500 focus:border-indigo-500 focus:z-10 sm:text-sm"
              placeholder="Password (min 8 characters)"
            />
            <p class="mt-1 text-xs text-gray-500">
              Must contain uppercase, lowercase, and number
            </p>
          </div>
        </div>

        <div v-if="error" class="rounded-md bg-red-50 p-4">
          <div class="flex">
            <div class="ml-3">
              <h3 class="text-sm font-medium text-red-800">
                {{ error }}
              </h3>
            </div>
          </div>
        </div>

        <div v-if="success" class="rounded-md bg-green-50 p-4">
          <div class="flex">
            <div class="ml-3">
              <h3 class="text-sm font-medium text-green-800">
                {{ success }}
              </h3>
            </div>
          </div>
        </div>

        <div>
          <button
            type="submit"
            :disabled="loading"
            class="group relative w-full flex justify-center py-2 px-4 border border-transparent text-sm font-medium rounded-md text-white bg-indigo-600 hover:bg-indigo-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500 disabled:opacity-50"
          >
            <span v-if="loading">Creating account...</span>
            <span v-else>Create account</span>
          </button>
        </div>

        <div class="text-center">
          <router-link to="/login" class="font-medium text-indigo-600 hover:text-indigo-500">
            Already have an account? Sign in
          </router-link>
        </div>
      </form>
    </div>
  </div>
</template>

<script setup>
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import { useAuthStore } from '@/stores/auth'

const router = useRouter()
const authStore = useAuthStore()

const form = ref({
  firstName: '',
  lastName: '',
  email: '',
  password: ''
})

const loading = ref(false)
const error = ref(null)
const success = ref(null)

const handleRegister = async () => {
  loading.value = true
  error.value = null
  success.value = null
  
  try {
    await authStore.register(form.value)
    
    success.value = 'Account created! Please check your email to verify your account.'
    
    // Clear form
    form.value = {
      firstName: '',
      lastName: '',
      email: '',
      password: ''
    }
    
    // Redirect after 3 seconds
    setTimeout(() => {
      router.push('/login')
    }, 3000)
  } catch (err) {
    error.value = err.response?.data?.message || 'Registration failed. Please try again.'
  } finally {
    loading.value = false
  }
}
</script>
```

### 7.2 Login Page

**File:** `src/views/Auth/Login.vue`

```vue
<template>
  <div class="min-h-screen flex items-center justify-center bg-gray-50 py-12 px-4 sm:px-6 lg:px-8">
    <div class="max-w-md w-full space-y-8">
      <div>
        <img class="mx-auto h-12 w-auto" src="/logo.svg" alt="BizBio" />
        <h2 class="mt-6 text-center text-3xl font-extrabold text-gray-900">
          Sign in to your account
        </h2>
      </div>
      
      <form class="mt-8 space-y-6" @submit.prevent="handleLogin">
        <div class="rounded-md shadow-sm space-y-4">
          <div>
            <label for="email" class="sr-only">Email address</label>
            <input
              id="email"
              v-model="form.email"
              type="email"
              required
              class="appearance-none rounded relative block w-full px-3 py-2 border border-gray-300 placeholder-gray-500 text-gray-900 focus:outline-none focus:ring-indigo-500 focus:border-indigo-500 focus:z-10 sm:text-sm"
              placeholder="Email address"
            />
          </div>
          
          <div>
            <label for="password" class="sr-only">Password</label>
            <input
              id="password"
              v-model="form.password"
              type="password"
              required
              class="appearance-none rounded relative block w-full px-3 py-2 border border-gray-300 placeholder-gray-500 text-gray-900 focus:outline-none focus:ring-indigo-500 focus:border-indigo-500 focus:z-10 sm:text-sm"
              placeholder="Password"
            />
          </div>
        </div>

        <div class="flex items-center justify-between">
          <div class="text-sm">
            <router-link to="/forgot-password" class="font-medium text-indigo-600 hover:text-indigo-500">
              Forgot your password?
            </router-link>
          </div>
        </div>

        <div v-if="error" class="rounded-md bg-red-50 p-4">
          <div class="flex">
            <div class="ml-3">
              <h3 class="text-sm font-medium text-red-800">
                {{ error }}
              </h3>
            </div>
          </div>
        </div>

        <div>
          <button
            type="submit"
            :disabled="loading"
            class="group relative w-full flex justify-center py-2 px-4 border border-transparent text-sm font-medium rounded-md text-white bg-indigo-600 hover:bg-indigo-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500 disabled:opacity-50"
          >
            <span v-if="loading">Signing in...</span>
            <span v-else">Sign in</span>
          </button>
        </div>

        <div class="text-center">
          <router-link to="/register" class="font-medium text-indigo-600 hover:text-indigo-500">
            Don't have an account? Sign up
          </router-link>
        </div>
      </form>
    </div>
  </div>
</template>

<script setup>
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import { useAuthStore } from '@/stores/auth'

const router = useRouter()
const authStore = useAuthStore()

const form = ref({
  email: '',
  password: ''
})

const loading = ref(false)
const error = ref(null)

const handleLogin = async () => {
  loading.value = true
  error.value = null
  
  try {
    await authStore.login(form.value)
    router.push('/dashboard')
  } catch (err) {
    error.value = err.response?.data?.message || 'Login failed. Please check your credentials.'
  } finally {
    loading.value = false
  }
}
</script>
```

### 7.3 Auth Store (Pinia)

**File:** `src/stores/auth.js`

```javascript
import { defineStore } from 'pinia'
import axios from 'axios'

const API_URL = import.meta.env.VITE_API_URL

export const useAuthStore = defineStore('auth', {
  state: () => ({
    user: null,
    token: localStorage.getItem('token') || null
  }),
  
  getters: {
    isAuthenticated: (state) => !!state.token,
    currentUser: (state) => state.user
  },
  
  actions: {
    async register(userData) {
      const response = await axios.post(`${API_URL}/auth/register`, userData)
      return response.data
    },
    
    async login(credentials) {
      const response = await axios.post(`${API_URL}/auth/login`, credentials)
      const { token, user } = response.data
      
      this.token = token
      this.user = user
      
      localStorage.setItem('token', token)
      axios.defaults.headers.common['Authorization'] = `Bearer ${token}`
      
      return user
    },
    
    async logout() {
      this.token = null
      this.user = null
      
      localStorage.removeItem('token')
      delete axios.defaults.headers.common['Authorization']
    },
    
    async verifyEmail(token) {
      const response = await axios.get(`${API_URL}/auth/verify-email?token=${token}`)
      return response.data
    },
    
    async forgotPassword(email) {
      const response = await axios.post(`${API_URL}/auth/forgot-password`, { email })
      return response.data
    },
    
    async resetPassword(token, newPassword) {
      const response = await axios.post(`${API_URL}/auth/reset-password`, {
        token,
        newPassword
      })
      return response.data
    }
  }
})
```

---

## 8. PayFast Integration

### 8.1 PayFast Service

**File:** `Services/PayFastService.cs`

```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using BizBio.Core.Entities;

namespace BizBio.Infrastructure.Services
{
    public class PayFastService : IPaymentService
    {
        private readonly IConfiguration _configuration;
        
        private string MerchantId => _configuration["PayFast:MerchantId"];
        private string MerchantKey => _configuration["PayFast:MerchantKey"];
        private string PassPhrase => _configuration["PayFast:PassPhrase"];
        private string PayFastUrl => _configuration["PayFast:Url"];  // Sandbox or Live
        
        public PayFastService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        
        public async Task<string> CreateSubscriptionPaymentAsync(
            UserSubscription subscription,
            string returnUrl,
            string cancelUrl)
        {
            // Generate unique merchant transaction ID
            var merchantTransactionId = $"SUB-{subscription.Id}-{DateTime.UtcNow:yyyyMMddHHmmss}";
            
            // Build PayFast parameters
            var parameters = new Dictionary<string, string>
            {
                {"merchant_id", MerchantId},
                {"merchant_key", MerchantKey},
                {"return_url", returnUrl},
                {"cancel_url", cancelUrl},
                {"notify_url", $"{_configuration["Website:Url"]}/api/v1/payfast/notify"},
                
                // Subscription details
                {"subscription_type", "1"},  // Monthly
                {"billing_date", DateTime.UtcNow.AddDays(subscription.Tier.TrialDays).ToString("yyyy-MM-dd")},
                {"recurring_amount", subscription.Price.ToString("F2")},
                {"frequency", "3"},  // Monthly
                {"cycles", "0"},  // Infinite
                
                // Item details
                {"item_name", $"BizBio {subscription.Tier.DisplayName} Subscription"},
                {"item_description", subscription.Tier.Description},
                {"amount", subscription.Price.ToString("F2")},
                
                // Transaction details
                {"m_payment_id", merchantTransactionId},
                {"email_address", subscription.User.Email},
                {"name_first", subscription.User.FirstName},
                {"name_last", subscription.User.LastName}
            };
            
            // Generate signature
            var signature = GenerateSignature(parameters);
            parameters.Add("signature", signature);
            
            // Build payment URL
            var queryString = string.Join("&", 
                parameters.Select(p => $"{p.Key}={Uri.EscapeDataString(p.Value)}"));
            
            return $"{PayFastUrl}/eng/process?{queryString}";
        }
        
        public async Task HandleIPNAsync(Dictionary<string, string> ipnData)
        {
            // Validate signature
            if (!ValidateSignature(ipnData))
            {
                throw new Exception("Invalid PayFast signature");
            }
            
            // Extract data
            var paymentStatus = ipnData["payment_status"];
            var merchantPaymentId = ipnData["m_payment_id"];
            var payFastPaymentId = ipnData["pf_payment_id"];
            var amount = decimal.Parse(ipnData["amount_gross"]);
            var token = ipnData.ContainsKey("token") ? ipnData["token"] : null;
            
            // Parse subscription ID from merchant payment ID
            // Format: SUB-{subscriptionId}-{timestamp}
            var parts = merchantPaymentId.Split('-');
            var subscriptionId = int.Parse(parts[1]);
            
            // Handle based on payment status
            switch (paymentStatus.ToLower())
            {
                case "complete":
                    await HandlePaymentCompleteAsync(subscriptionId, payFastPaymentId, token, amount);
                    break;
                    
                case "failed":
                    await HandlePaymentFailedAsync(subscriptionId, amount);
                    break;
                    
                case "cancelled":
                    await HandlePaymentCancelledAsync(subscriptionId);
                    break;
            }
        }
        
        private async Task HandlePaymentCompleteAsync(
            int subscriptionId, 
            string payFastPaymentId, 
            string token,
            decimal amount)
        {
            // This would call your SubscriptionService
            // Implementation in SubscriptionService.HandlePaymentSuccessAsync()
        }
        
        private string GenerateSignature(Dictionary<string, string> parameters)
        {
            // Sort parameters
            var sortedParams = parameters
                .OrderBy(p => p.Key)
                .Where(p => p.Key != "signature")
                .Select(p => $"{p.Key}={Uri.EscapeDataString(p.Value)}");
            
            var paramString = string.Join("&", sortedParams);
            
            // Add passphrase if configured
            if (!string.IsNullOrEmpty(PassPhrase))
            {
                paramString += $"&passphrase={Uri.EscapeDataString(PassPhrase)}";
            }
            
            // Generate MD5 hash
            using (var md5 = MD5.Create())
            {
                var hash = md5.ComputeHash(Encoding.UTF8.GetBytes(paramString));
                return BitConverter.ToString(hash).Replace("-", "").ToLower();
            }
        }
        
        private bool ValidateSignature(Dictionary<string, string> parameters)
        {
            var receivedSignature = parameters["signature"];
            var calculatedSignature = GenerateSignature(parameters);
            
            return receivedSignature == calculatedSignature;
        }
    }
}
```

### 8.2 PayFast IPN Controller

**File:** `Controllers/PayFastController.cs`

```csharp
using Microsoft.AspNetCore.Mvc;
using BizBio.Core.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BizBio.API.Controllers
{
    [Route("api/v1/payfast")]
    [ApiController]
    public class PayFastController : ControllerBase
    {
        private readonly IPaymentService _paymentService;
        private readonly ILogger<PayFastController> _logger;
        
        public PayFastController(
            IPaymentService paymentService,
            ILogger<PayFastController> logger)
        {
            _paymentService = paymentService;
            _logger = logger;
        }
        
        /// <summary>
        /// PayFast IPN (Instant Payment Notification) endpoint
        /// </summary>
        [HttpPost("notify")]
        public async Task<IActionResult> Notify()
        {
            try
            {
                // Read form data
                var ipnData = Request.Form.ToDictionary(
                    kvp => kvp.Key, 
                    kvp => kvp.Value.ToString()
                );
                
                _logger.LogInformation($"Received PayFast IPN: {string.Join(", ", ipnData.Select(x => $"{x.Key}={x.Value}"))}");
                
                // Process IPN
                await _paymentService.HandleIPNAsync(ipnData);
                
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing PayFast IPN");
                return StatusCode(500);
            }
        }
    }
}
```

---

## 9. Deployment Setup

### 9.1 Server Requirements (Debian VPS)

```bash
# Update system
sudo apt update && sudo apt upgrade -y

# Install .NET 6 SDK
wget https://packages.microsoft.com/config/debian/11/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
sudo dpkg -i packages-microsoft-prod.deb
sudo apt update
sudo apt install -y dotnet-sdk-6.0

# Install Node.js 18 LTS
curl -fsSL https://deb.nodesource.com/setup_18.x | sudo -E bash -
sudo apt install -y nodejs

# Install NGINX
sudo apt install -y nginx

# Install MySQL Client (to connect to cPanel MySQL)
sudo apt install -y mysql-client

# Install certbot for SSL
sudo apt install -y certbot python3-certbot-nginx
```

### 9.2 NGINX Configuration

**File:** `/etc/nginx/sites-available/bizbio`

```nginx
# API Backend
server {
    listen 80;
    server_name api.bizbio.co.za;
    
    location / {
        proxy_pass http://localhost:5000;
        proxy_http_version 1.1;
        proxy_set_header Upgrade $http_upgrade;
        proxy_set_header Connection keep-alive;
        proxy_set_header Host $host;
        proxy_cache_bypass $http_upgrade;
        proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto $scheme;
    }
}

# Frontend
server {
    listen 80;
    server_name bizbio.co.za www.bizbio.co.za;
    
    root /var/www/bizbio-frontend/dist;
    index index.html;
    
    location / {
        try_files $uri $uri/ /index.html;
    }
    
    # Cache static assets
    location ~* \.(js|css|png|jpg|jpeg|gif|ico|svg|woff|woff2|ttf|eot)$ {
        expires 1y;
        add_header Cache-Control "public, immutable";
    }
}
```

Enable site and restart NGINX:
```bash
sudo ln -s /etc/nginx/sites-available/bizbio /etc/nginx/sites-enabled/
sudo nginx -t
sudo systemctl restart nginx
```

### 9.3 SSL Setup

```bash
# Get SSL certificates for both domains
sudo certbot --nginx -d bizbio.co.za -d www.bizbio.co.za
sudo certbot --nginx -d api.bizbio.co.za

# Auto-renewal is set up automatically
# Test renewal
sudo certbot renew --dry-run
```

### 9.4 Systemd Service for API

**File:** `/etc/systemd/system/bizbio-api.service`

```ini
[Unit]
Description=BizBio ASP.NET 6 Web API
After=network.target

[Service]
WorkingDirectory=/var/www/bizbio-api
ExecStart=/usr/bin/dotnet /var/www/bizbio-api/BizBio.API.dll
Restart=always
RestartSec=10
KillSignal=SIGINT
SyslogIdentifier=bizbio-api
User=www-data
Environment=ASPNETCORE_ENVIRONMENT=Production
Environment=DOTNET_PRINT_TELEMETRY_MESSAGE=false

[Install]
WantedBy=multi-user.target
```

Enable and start service:
```bash
sudo systemctl enable bizbio-api
sudo systemctl start bizbio-api
sudo systemctl status bizbio-api
```

---

## 10. Environment Configuration

### 10.1 API Configuration

**File:** `appsettings.Production.json`

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "ConnectionStrings": {
    "DefaultConnection": "server=mysql.yourhost.com;database=bizbio_db;user=bizbio_user;password=YOUR_PASSWORD;SslMode=Required"
  },
  "JWT": {
    "Secret": "YOUR_SUPER_SECRET_KEY_AT_LEAST_32_CHARACTERS_LONG",
    "Issuer": "BizBio",
    "Audience": "BizBioUsers",
    "ExpiryInDays": 7
  },
  "SendGrid": {
    "ApiKey": "YOUR_SENDGRID_API_KEY",
    "FromEmail": "noreply@bizbio.co.za",
    "FromName": "BizBio"
  },
  "PayFast": {
    "MerchantId": "YOUR_PAYFAST_MERCHANT_ID",
    "MerchantKey": "YOUR_PAYFAST_MERCHANT_KEY",
    "PassPhrase": "YOUR_PAYFAST_PASSPHRASE",
    "Url": "https://www.payfast.co.za"
  },
  "Website": {
    "Url": "https://bizbio.co.za",
    "ApiUrl": "https://api.bizbio.co.za"
  }
}
```

### 10.2 Frontend Environment

**File:** `.env.production`

```bash
VITE_API_URL=https://api.bizbio.co.za/api/v1
VITE_WEBSITE_URL=https://bizbio.co.za
```

---

## 11. Deployment Commands

### 11.1 Deploy Backend

```bash
# On your development machine
cd BizBio.API
dotnet publish -c Release -o ./publish

# Upload to server
scp -r ./publish/* user@your-vps:/var/www/bizbio-api/

# On server
sudo systemctl restart bizbio-api
sudo systemctl status bizbio-api
```

### 11.2 Deploy Frontend

```bash
# On your development machine
cd bizbio-frontend
npm run build

# Upload to server
scp -r ./dist/* user@your-vps:/var/www/bizbio-frontend/dist/

# No restart needed - NGINX serves static files
```

---

## 12. Initial Database Setup

### 12.1 Run Migrations

```bash
# On your development machine with connection to cPanel MySQL
cd BizBio.Infrastructure

# Create migration
dotnet ef migrations add InitialCreate --startup-project ../BizBio.API

# Update database
dotnet ef database update --startup-project ../BizBio.API
```

### 12.2 Seed Subscription Tiers

**File:** `Data/SeedData.cs`

```csharp
public static class SeedData
{
    public static void Initialize(BizBioDbContext context)
    {
        if (context.SubscriptionTiers.Any())
            return; // Already seeded
        
        var tiers = new[]
        {
            // Professional - Free
            new SubscriptionTier
            {
                ProductLine = "Professional",
                TierName = "Free",
                TierCode = "PROF_FREE",
                DisplayName = "Free",
                Description = "Basic digital profile",
                MonthlyPrice = 0,
                TrialDays = 0,
                MaxProfiles = 1,
                CustomBranding = false,
                RemoveBranding = false,
                Analytics = false,
                VCardDownload = false,
                IsActive = true,
                DisplayOrder = 1
            },
            
            // Professional - Solo
            new SubscriptionTier
            {
                ProductLine = "Professional",
                TierName = "Solo",
                TierCode = "PROF_SOLO",
                DisplayName = "Solo",
                Description = "All premium features, one profile",
                MonthlyPrice = 59,
                TrialDays = 14,
                MaxProfiles = 1,
                CustomBranding = true,
                RemoveBranding = true,
                Analytics = true,
                VCardDownload = true,
                IsActive = true,
                DisplayOrder = 2
            },
            
            // Professional - Pro
            new SubscriptionTier
            {
                ProductLine = "Professional",
                TierName = "Pro",
                TierCode = "PROF_PRO",
                DisplayName = "Pro",
                Description = "Multiple profiles with all features",
                MonthlyPrice = 99,
                TrialDays = 14,
                MaxProfiles = 5,
                MaxDocuments = 10,
                CustomBranding = true,
                RemoveBranding = true,
                Analytics = true,
                VCardDownload = true,
                IsActive = true,
                DisplayOrder = 3
            }
            
            // Add more tiers as needed...
        };
        
        context.SubscriptionTiers.AddRange(tiers);
        context.SaveChanges();
    }
}
```

---

**End of Technical Specification**

This gives you everything you need to start development:
- ✅ Complete database schema
- ✅ Full authentication system
- ✅ Subscription & trial logic
- ✅ PayFast integration
- ✅ Email system
- ✅ API endpoints
- ✅ Frontend components
- ✅ Deployment setup

**Total Development Time:** 6-8 weeks @ 20 hours/week

**Next Step:** Run the database migrations and start implementing the authentication endpoints!
