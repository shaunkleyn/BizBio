# Logging and Application Insights Implementation Guide

## Overview

This document describes the comprehensive logging and Application Insights telemetry implementation in the BizBio API project.

## Table of Contents

- [Components](#components)
- [Configuration](#configuration)
- [Middleware](#middleware)
- [Usage Examples](#usage-examples)
- [Application Insights Setup](#application-insights-setup)
- [Best Practices](#best-practices)

## Components

### 1. Application Insights Configuration

Application Insights is configured in `Program.cs` with the following features enabled:

- **Adaptive Sampling**: Automatically adjusts sampling to reduce telemetry volume while maintaining statistical significance
- **Performance Counter Collection**: Tracks system-level metrics (CPU, memory, etc.)
- **Quick Pulse Metric Stream**: Real-time metrics dashboard
- **Dependency Tracking**: Automatically tracks external dependencies (databases, HTTP calls, etc.)
- **Event Counter Collection**: Collects .NET runtime metrics

### 2. Custom Middleware

#### ExceptionHandlingMiddleware

Location: `BizBio.API/Middleware/ExceptionHandlingMiddleware.cs`

**Features:**
- Catches and handles all unhandled exceptions globally
- Logs exceptions with context (path, method, user, IP address)
- Tracks exceptions in Application Insights with custom properties
- Returns standardized error responses
- Includes stack trace in development environment only

**Properties tracked:**
- Path
- HTTP Method
- Query String
- Trace ID
- IP Address
- User Agent
- User ID (if authenticated)

#### RequestLoggingMiddleware

Location: `BizBio.API/Middleware/RequestLoggingMiddleware.cs`

**Features:**
- Logs every incoming HTTP request
- Tracks request duration
- Logs response status codes
- Records user information if authenticated
- Tracks custom Application Insights request telemetry

**Logged information:**
- HTTP Method
- Request Path
- Query String
- User Agent
- IP Address
- Response Status Code
- Request Duration
- Authentication details

### 3. Telemetry Service

Location: `BizBio.Infrastructure/Services/TelemetryService.cs`
Interface: `BizBio.Core/Interfaces/ITelemetryService.cs`

**Methods:**

#### TrackEvent
```csharp
void TrackEvent(string eventName, Dictionary<string, string>? properties = null, Dictionary<string, double>? metrics = null);
```
Track custom events with optional properties and metrics.

#### TrackMetric
```csharp
void TrackMetric(string metricName, double value, Dictionary<string, string>? properties = null);
```
Track custom numeric metrics.

#### TrackDependency
```csharp
void TrackDependency(string dependencyType, string dependencyName, string data, DateTimeOffset startTime, TimeSpan duration, bool success);
```
Track external service calls, database queries, etc.

#### TrackException
```csharp
void TrackException(Exception exception, Dictionary<string, string>? properties = null, Dictionary<string, double>? metrics = null);
```
Track exceptions with additional context.

#### TrackUserAction
```csharp
void TrackUserAction(string userId, string actionName, Dictionary<string, string>? properties = null);
```
Track specific user actions for behavioral analytics.

#### TrackBusinessMetric
```csharp
void TrackBusinessMetric(string metricName, double value, string? userId = null, Dictionary<string, string>? properties = null);
```
Track business KPIs and metrics.

## Configuration

### appsettings.json

```json
{
  "ApplicationInsights": {
    "ConnectionString": "<Your-Application-Insights-Connection-String>",
    "EnableAdaptiveSampling": true,
    "EnablePerformanceCounterCollectionModule": true,
    "EnableQuickPulseMetricStream": true,
    "EnableDependencyTracking": true,
    "EnableEventCounterCollectionModule": true
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning",
      "Microsoft.EntityFrameworkCore": "Warning"
    },
    "ApplicationInsights": {
      "LogLevel": {
        "Default": "Information",
        "Microsoft": "Warning"
      }
    }
  }
}
```

### appsettings.Development.json

```json
{
  "ApplicationInsights": {
    "ConnectionString": "",
    "EnableAdaptiveSampling": false,
    "EnablePerformanceCounterCollectionModule": true,
    "EnableQuickPulseMetricStream": true,
    "EnableDependencyTracking": true,
    "EnableEventCounterCollectionModule": true
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Information",
      "Microsoft.EntityFrameworkCore": "Information"
    },
    "ApplicationInsights": {
      "LogLevel": {
        "Default": "Debug",
        "Microsoft": "Information"
      }
    }
  }
}
```

## Usage Examples

### In Controllers

```csharp
[Route("api/v1/example")]
[ApiController]
public class ExampleController : ControllerBase
{
    private readonly ILogger<ExampleController> _logger;
    private readonly ITelemetryService _telemetry;

    public ExampleController(
        ILogger<ExampleController> logger,
        ITelemetryService telemetry)
    {
        _logger = logger;
        _telemetry = telemetry;
    }

    [HttpPost("action")]
    public async Task<IActionResult> PerformAction([FromBody] ActionDto dto)
    {
        _logger.LogInformation("Action started for user: {UserId}", dto.UserId);

        try
        {
            // Perform action
            var result = await _service.DoSomethingAsync(dto);

            // Track successful action
            _telemetry.TrackEvent("ActionCompleted", new Dictionary<string, string>
            {
                { "UserId", dto.UserId.ToString() },
                { "ActionType", dto.Type }
            });

            // Track business metric
            _telemetry.TrackBusinessMetric("ActionsCompleted", 1, dto.UserId.ToString());

            _logger.LogInformation("Action completed successfully for user: {UserId}", dto.UserId);

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Action failed for user: {UserId}", dto.UserId);

            _telemetry.TrackException(ex, new Dictionary<string, string>
            {
                { "UserId", dto.UserId.ToString() },
                { "ActionType", dto.Type }
            });

            throw;
        }
    }
}
```

### In Repositories

```csharp
public class ExampleRepository : IExampleRepository
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<ExampleRepository> _logger;
    private readonly ITelemetryService _telemetry;

    public ExampleRepository(
        ApplicationDbContext context,
        ILogger<ExampleRepository> logger,
        ITelemetryService telemetry)
    {
        _context = context;
        _logger = logger;
        _telemetry = telemetry;
    }

    public async Task<Entity?> GetByIdAsync(int id)
    {
        var stopwatch = Stopwatch.StartNew();

        try
        {
            _logger.LogDebug("Fetching entity by ID: {Id}", id);

            var dbStopwatch = Stopwatch.StartNew();
            var entity = await _context.Entities
                .FirstOrDefaultAsync(e => e.Id == id);
            dbStopwatch.Stop();

            // Track database dependency
            _telemetry.TrackDependency(
                "Database",
                "Entities.GetById",
                $"SELECT * FROM Entities WHERE Id = {id}",
                DateTimeOffset.UtcNow.Subtract(dbStopwatch.Elapsed),
                dbStopwatch.Elapsed,
                entity != null);

            stopwatch.Stop();
            _logger.LogDebug("Entity retrieved. ID: {Id}, Duration: {Duration}ms",
                id, stopwatch.ElapsedMilliseconds);

            return entity;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _logger.LogError(ex, "Error fetching entity by ID: {Id}", id);

            _telemetry.TrackException(ex, new Dictionary<string, string>
            {
                { "Method", "GetByIdAsync" },
                { "EntityId", id.ToString() }
            });

            throw;
        }
    }
}
```

## Application Insights Setup

### 1. Create Application Insights Resource in Azure

1. Go to Azure Portal
2. Create a new Application Insights resource
3. Copy the Connection String from the Overview page

### 2. Update Configuration

Add the connection string to your configuration:

**For Production (Azure App Service):**
- Add as an Application Setting in Azure Portal
- Key: `ApplicationInsights__ConnectionString`
- Value: Your connection string

**For Local Development:**
- Add to `appsettings.Development.json`
- Or use User Secrets: `dotnet user-secrets set "ApplicationInsights:ConnectionString" "your-connection-string"`

### 3. Verify Setup

After deployment, check Application Insights in Azure Portal:

- **Live Metrics**: Real-time telemetry stream
- **Failures**: Exception tracking and failed requests
- **Performance**: Request duration and dependency calls
- **Metrics**: Custom metrics and business KPIs
- **Logs**: Structured log queries

## Best Practices

### 1. Logging Levels

Use appropriate log levels:

- **Debug**: Detailed information for diagnosing problems (development only)
- **Information**: General informational messages
- **Warning**: Warnings about potential issues
- **Error**: Error messages for failures
- **Critical**: Critical failures requiring immediate attention

### 2. Structured Logging

Always use structured logging with named parameters:

```csharp
// Good
_logger.LogInformation("User {UserId} performed action {ActionName}", userId, actionName);

// Bad
_logger.LogInformation($"User {userId} performed action {actionName}");
```

### 3. Sensitive Data

Never log sensitive information:

- Passwords
- API keys
- Personal identification numbers
- Credit card details
- Email content

### 4. Performance Considerations

- Use log levels appropriately to avoid excessive logging in production
- Enable adaptive sampling in production to manage costs
- Use async methods for telemetry tracking
- Avoid logging inside tight loops

### 5. Custom Properties

Add meaningful custom properties to track context:

```csharp
_telemetry.TrackEvent("OrderPlaced", new Dictionary<string, string>
{
    { "UserId", userId.ToString() },
    { "OrderId", orderId.ToString() },
    { "ProductCount", order.Items.Count.ToString() },
    { "PaymentMethod", order.PaymentMethod }
},
new Dictionary<string, double>
{
    { "OrderTotal", order.Total },
    { "TaxAmount", order.Tax }
});
```

### 6. Dependency Tracking

Always track external dependencies:

- Database queries
- HTTP API calls
- Cache operations
- File I/O operations
- Third-party service calls

### 7. Business Metrics

Track important business KPIs:

```csharp
// User registrations
_telemetry.TrackBusinessMetric("NewUserRegistrations", 1);

// Revenue
_telemetry.TrackBusinessMetric("Revenue", orderTotal, userId);

// Feature usage
_telemetry.TrackBusinessMetric("FeatureUsage", 1, userId, new Dictionary<string, string>
{
    { "Feature", "ExportData" }
});
```

## Querying Telemetry

### Application Insights Query Language (Kusto)

Example queries:

```kusto
// Failed requests in last 24 hours
requests
| where timestamp > ago(24h)
| where success == false
| summarize count() by resultCode
| order by count_ desc

// Slowest database queries
dependencies
| where type == "Database"
| where timestamp > ago(1h)
| summarize avg(duration), max(duration), count() by name
| order by max_duration desc

// User actions by user
customEvents
| where name startswith "UserAction_"
| where customDimensions.UserId != ""
| summarize count() by tostring(customDimensions.UserId), name
| order by count_ desc

// Exception trends
exceptions
| where timestamp > ago(7d)
| summarize count() by bin(timestamp, 1h), type
| render timechart
```

## Middleware Order

The middleware is registered in the correct order in `Program.cs`:

1. **ExceptionHandlingMiddleware** - First to catch all errors
2. **RequestLoggingMiddleware** - Second to log all requests
3. Other middleware (Swagger, CORS, etc.)
4. Authentication/Authorization
5. Endpoints

This ensures that all requests are logged and all exceptions are handled properly.

## Troubleshooting

### Telemetry Not Appearing in Application Insights

1. Verify connection string is correct
2. Check that Application Insights resource exists
3. Wait 2-5 minutes for telemetry to appear
4. Check Azure Portal for any ingestion errors
5. Verify `ApplicationInsights.config` or configuration settings

### High Telemetry Costs

1. Enable adaptive sampling in production
2. Review and reduce debug-level logging
3. Use log levels appropriately
4. Consider filtering noisy telemetry

### Performance Impact

1. Telemetry is async and shouldn't impact performance significantly
2. Use buffering and batching (enabled by default)
3. Monitor CPU and memory usage
4. Adjust sampling rates if needed

## Additional Resources

- [Application Insights Documentation](https://docs.microsoft.com/azure/azure-monitor/app/app-insights-overview)
- [ASP.NET Core Logging](https://docs.microsoft.com/aspnet/core/fundamentals/logging)
- [Kusto Query Language](https://docs.microsoft.com/azure/data-explorer/kusto/query/)
