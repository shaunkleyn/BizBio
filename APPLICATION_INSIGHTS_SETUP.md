# Azure Application Insights Setup Guide

This document provides instructions for configuring Azure Application Insights in the BizBio project.

## Overview

Application Insights has been integrated throughout the BizBio application to provide:
- **Exception tracking and logging** across all layers
- **Custom event tracking** for business operations (user registration, login, payments, etc.)
- **Dependency tracking** for external services (SMTP, PayFast, MySQL database)
- **Performance monitoring** and metrics
- **Request/response telemetry** for all API calls
- **Custom properties** for better filtering and analysis

## Prerequisites

1. Azure subscription
2. Application Insights resource created in Azure Portal

## Setup Instructions

### Step 1: Create Application Insights Resource in Azure

1. Go to the [Azure Portal](https://portal.azure.com)
2. Create a new Application Insights resource:
   - Click "Create a resource"
   - Search for "Application Insights"
   - Fill in the details:
     - Name: `bizbio-api-insights` (or your preferred name)
     - Resource Group: Select or create a resource group
     - Region: Choose the same region as your API deployment
     - Workspace: Create new or use existing Log Analytics workspace

3. Once created, copy the **Connection String** from the Overview page

### Step 2: Configure Connection String

Update the Application Insights connection strings in your configuration files:

#### Production (`appsettings.json`)
Replace `YOUR_APPLICATION_INSIGHTS_CONNECTION_STRING_HERE` with your production connection string:

```json
"ApplicationInsights": {
  "ConnectionString": "InstrumentationKey=xxxxx;IngestionEndpoint=https://xxx.in.applicationinsights.azure.com/;LiveEndpoint=https://xxx.livediagnostics.monitor.azure.com/"
}
```

#### Development (`appsettings.Development.json`)
Replace `YOUR_DEV_APPLICATION_INSIGHTS_CONNECTION_STRING_HERE` with your development connection string (can be the same or different):

```json
"ApplicationInsights": {
  "ConnectionString": "InstrumentationKey=xxxxx;IngestionEndpoint=https://xxx.in.applicationinsights.azure.com/;LiveEndpoint=https://xxx.livediagnostics.monitor.azure.com/"
}
```

### Step 3: Alternative Configuration Methods

You can also configure the connection string using:

#### Environment Variable
```bash
export APPLICATIONINSIGHTS__CONNECTIONSTRING="your-connection-string-here"
```

#### Azure App Service Configuration
Add an application setting in Azure Portal:
- Name: `ApplicationInsights:ConnectionString`
- Value: Your connection string

## What's Been Implemented

### 1. Core Configuration Files

#### `Program.cs` (BizBio.API)
- Integrated Application Insights telemetry
- Configured logging to send logs to Application Insights
- Registered custom telemetry initializer
- Added global exception filter

#### `appsettings.json` / `appsettings.Development.json`
- Application Insights configuration section
- Feature flags for telemetry modules:
  - Adaptive Sampling (production only)
  - Performance Counter Collection
  - Dependency Tracking
  - Event Counter Collection
  - Quick Pulse Metric Stream (Live Metrics)
  - Heartbeat
  - Auto-collected Metric Extractor

### 2. Custom Components

#### `BizBioTelemetryInitializer` (BizBio.API/Telemetry)
Adds custom properties to all telemetry:
- Cloud role name: "BizBio.API"
- Cloud role instance: Machine name
- Application: "BizBio"
- Environment: Production/Development
- Version: 1.0.0

#### `ApplicationInsightsExceptionFilter` (BizBio.API/Filters)
Global exception filter that:
- Captures all unhandled exceptions
- Adds custom properties (controller, action, user info)
- Tracks to Application Insights

### 3. Infrastructure Services Telemetry

#### AuthService
Tracks:
- **Events**: UserRegistered, UserLogin, EmailVerified
- **Exceptions**: Registration failures, login errors, email sending failures
- **Custom Properties**: Email, UserId, Operation type

#### EmailService
Tracks:
- **Dependencies**: SMTP email sending operations with duration
- **Events**: EmailSent
- **Exceptions**: SMTP errors with status codes
- **Custom Properties**: Email recipient, subject, SMTP host

#### PayFastService
Tracks:
- **Events**: PaymentUrlGenerated, PaymentValidationFailed, PaymentNotComplete, SubscriptionCreated
- **Exceptions**: Payment processing errors
- **Custom Properties**: UserId, Amount, BillingCycle, PaymentId

## Monitored Telemetry Types

### Automatically Tracked
- **Requests**: All HTTP requests to your API
- **Dependencies**:
  - SQL queries (MySQL via EF Core)
  - HTTP calls
  - SMTP operations
- **Exceptions**: Unhandled exceptions
- **Performance Counters**: CPU, Memory, Request rates
- **Live Metrics**: Real-time monitoring

### Custom Tracked
- **Business Events**:
  - User registration and email verification
  - User login events
  - Payment URL generation
  - Subscription creation
  - Email sending

- **Custom Properties**:
  - User identifiers
  - Operation context
  - Business-specific metadata

## Viewing Telemetry in Azure Portal

### 1. Live Metrics
Go to Application Insights → Live Metrics to see:
- Real-time requests and failures
- Live performance counters
- Sample telemetry

### 2. Failures
Go to Application Insights → Failures to see:
- Exception details with stack traces
- Failed requests
- Failed dependencies

### 3. Performance
Go to Application Insights → Performance to see:
- Request duration
- Dependency duration
- Operation timings

### 4. Logs
Go to Application Insights → Logs to query:
```kusto
// All exceptions in the last 24 hours
exceptions
| where timestamp > ago(24h)
| order by timestamp desc

// Custom events
customEvents
| where name == "UserRegistered"
| where timestamp > ago(7d)

// Failed dependencies
dependencies
| where success == false
| where timestamp > ago(24h)

// Authentication errors
traces
| where message contains "Login failed"
| where timestamp > ago(24h)
```

### 5. Application Map
Go to Application Insights → Application Map to see:
- Service dependencies
- Call relationships
- Performance bottlenecks

## Sampling Configuration

### Production
Adaptive sampling is **enabled** (default: 5 items per second)
- Helps control costs
- Maintains statistical accuracy
- Automatically adjusts based on traffic

### Development
Adaptive sampling is **disabled**
- Captures all telemetry
- Better for debugging
- Not cost-optimized

## Best Practices

1. **Don't log sensitive data**: Avoid logging passwords, tokens, or PII in custom properties
2. **Use structured logging**: Use ILogger with structured parameters (e.g., `_logger.LogInformation("User {Email} logged in", email)`)
3. **Monitor costs**: Check Application Insights data ingestion in Azure Portal
4. **Set up alerts**: Configure alerts for exceptions, performance degradation, and availability
5. **Use correlation**: Application Insights automatically correlates requests, dependencies, and logs

## Troubleshooting

### No Data Appearing in Application Insights

1. **Check connection string**: Verify it's correctly configured
2. **Check network**: Ensure the application can reach `*.in.applicationinsights.azure.com`
3. **Wait 2-5 minutes**: Telemetry is not instantaneous
4. **Check logs**: Look for Application Insights errors in application logs

### Too Much Data / High Costs

1. Enable adaptive sampling in production
2. Adjust sampling percentage:
```json
"ApplicationInsights": {
  "EnableAdaptiveSampling": true,
  "SamplingSettings": {
    "MaxTelemetryItemsPerSecond": 5
  }
}
```

3. Filter out noise:
   - Reduce logging levels for Microsoft.* namespaces
   - Use telemetry processors to filter specific telemetry

## Additional Resources

- [Application Insights Documentation](https://docs.microsoft.com/azure/azure-monitor/app/app-insights-overview)
- [ASP.NET Core Application Insights](https://docs.microsoft.com/azure/azure-monitor/app/asp-net-core)
- [Kusto Query Language (KQL)](https://docs.microsoft.com/azure/data-explorer/kusto/query/)

## Support

For issues or questions, contact the development team or refer to the Azure Application Insights documentation.
