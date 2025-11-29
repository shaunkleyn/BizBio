# Application Insights Telemetry - Quick Reference

## Setup Checklist

- [ ] Get Application Insights Connection String from Azure Portal
- [ ] Add to `appsettings.json`: `ApplicationInsights:ConnectionString`
- [ ] For Azure deployment, add as App Setting: `ApplicationInsights__ConnectionString`
- [ ] Verify telemetry is appearing in Azure Portal (wait 2-5 minutes)

## Common Telemetry Patterns

### Controller Actions

```csharp
public class MyController : ControllerBase
{
    private readonly ILogger<MyController> _logger;
    private readonly ITelemetryService _telemetry;

    // 1. Log the attempt
    _logger.LogInformation("Action started: {UserId}", userId);

    try
    {
        // 2. Do work
        var result = await _service.DoWorkAsync();

        // 3. Track success event
        _telemetry.TrackEvent("ActionSuccess", new Dictionary<string, string>
        {
            { "UserId", userId.ToString() }
        });

        // 4. Track business metric
        _telemetry.TrackBusinessMetric("ActionsCompleted", 1);

        // 5. Log completion
        _logger.LogInformation("Action completed: {UserId}", userId);

        return Ok(result);
    }
    catch (Exception ex)
    {
        // 6. Log error
        _logger.LogError(ex, "Action failed: {UserId}", userId);

        // 7. Track exception
        _telemetry.TrackException(ex, new Dictionary<string, string>
        {
            { "UserId", userId.ToString() }
        });

        throw; // Let global exception handler deal with it
    }
}
```

### Repository Database Calls

```csharp
public async Task<Entity> GetByIdAsync(int id)
{
    var stopwatch = Stopwatch.StartNew();

    try
    {
        var entity = await _context.Entities
            .FirstOrDefaultAsync(e => e.Id == id);
        stopwatch.Stop();

        // Track database dependency
        _telemetry.TrackDependency(
            "Database",
            "Entities.GetById",
            $"SELECT * FROM Entities WHERE Id = {id}",
            DateTimeOffset.UtcNow.Subtract(stopwatch.Elapsed),
            stopwatch.Elapsed,
            entity != null);

        return entity;
    }
    catch (Exception ex)
    {
        _telemetry.TrackException(ex);
        throw;
    }
}
```

### External API Calls

```csharp
public async Task<ApiResponse> CallExternalApiAsync(string endpoint)
{
    var stopwatch = Stopwatch.StartNew();
    var success = false;

    try
    {
        var response = await _httpClient.GetAsync(endpoint);
        success = response.IsSuccessStatusCode;
        stopwatch.Stop();

        _telemetry.TrackDependency(
            "HTTP",
            endpoint,
            endpoint,
            DateTimeOffset.UtcNow.Subtract(stopwatch.Elapsed),
            stopwatch.Elapsed,
            success);

        return await response.Content.ReadFromJsonAsync<ApiResponse>();
    }
    catch (Exception ex)
    {
        stopwatch.Stop();
        _telemetry.TrackDependency(
            "HTTP",
            endpoint,
            endpoint,
            DateTimeOffset.UtcNow.Subtract(stopwatch.Elapsed),
            stopwatch.Elapsed,
            false);

        _telemetry.TrackException(ex);
        throw;
    }
}
```

## Useful Application Insights Queries

### Top 10 Slowest Requests
```kusto
requests
| where timestamp > ago(24h)
| summarize avg(duration), max(duration), count() by name
| top 10 by max_duration desc
```

### Error Rate by Hour
```kusto
requests
| where timestamp > ago(24h)
| summarize total=count(), errors=countif(success == false) by bin(timestamp, 1h)
| project timestamp, errorRate = (errors * 100.0 / total)
| render timechart
```

### Most Common Exceptions
```kusto
exceptions
| where timestamp > ago(24h)
| summarize count() by type, outerMessage
| order by count_ desc
```

### Database Query Performance
```kusto
dependencies
| where type == "Database"
| where timestamp > ago(1h)
| summarize avg(duration), max(duration), count() by name
| order by avg_duration desc
```

### User Actions
```kusto
customEvents
| where name startswith "UserAction_"
| where timestamp > ago(24h)
| summarize count() by name
| order by count_ desc
```

### Business Metrics
```kusto
customMetrics
| where name == "NewUserRegistrations"
| where timestamp > ago(7d)
| summarize sum(value) by bin(timestamp, 1d)
| render timechart
```

## Log Levels Guide

| Level | When to Use | Example |
|-------|-------------|---------|
| **Debug** | Detailed diagnostics (dev only) | `_logger.LogDebug("Cache hit for key: {Key}", key)` |
| **Information** | Normal operation flow | `_logger.LogInformation("User logged in: {UserId}", userId)` |
| **Warning** | Abnormal but expected | `_logger.LogWarning("Login attempt failed: {Email}", email)` |
| **Error** | Error requiring attention | `_logger.LogError(ex, "Failed to process order: {OrderId}", orderId)` |
| **Critical** | Critical system failure | `_logger.LogCritical(ex, "Database connection lost")` |

## Event Naming Conventions

Use clear, consistent naming:

- **User Actions**: `UserLoggedIn`, `UserRegistered`, `UserPasswordReset`
- **Business Events**: `OrderPlaced`, `PaymentProcessed`, `SubscriptionUpgraded`
- **System Events**: `CacheCleared`, `DataMigrationCompleted`, `BackupCreated`
- **Errors**: `LoginFailed`, `PaymentFailed`, `EmailSendFailed`

## Custom Properties Best Practices

Always include relevant context:

```csharp
_telemetry.TrackEvent("OrderPlaced", new Dictionary<string, string>
{
    { "UserId", userId.ToString() },
    { "OrderId", orderId.ToString() },
    { "PaymentMethod", paymentMethod },
    { "ItemCount", items.Count.ToString() }
},
new Dictionary<string, double>
{
    { "OrderTotal", total },
    { "ShippingCost", shipping },
    { "Tax", tax }
});
```

## Performance Tips

1. **Use async telemetry methods** - Telemetry is already async, don't block on it
2. **Batch operations** - Telemetry SDK automatically batches
3. **Use sampling in production** - Set `EnableAdaptiveSampling: true`
4. **Avoid logging in loops** - Aggregate first, log summary
5. **Use appropriate log levels** - Debug only in development

## Common Mistakes to Avoid

❌ **Don't log sensitive data**
```csharp
// BAD
_logger.LogInformation("User password: {Password}", password);

// GOOD
_logger.LogInformation("User logged in: {UserId}", userId);
```

❌ **Don't use string interpolation in logs**
```csharp
// BAD
_logger.LogInformation($"User {userId} logged in");

// GOOD
_logger.LogInformation("User {UserId} logged in", userId);
```

❌ **Don't ignore exceptions**
```csharp
// BAD
catch (Exception ex)
{
    return null;
}

// GOOD
catch (Exception ex)
{
    _logger.LogError(ex, "Operation failed");
    _telemetry.TrackException(ex);
    throw;
}
```

❌ **Don't create multiple telemetry clients**
```csharp
// BAD
var telemetry = new TelemetryClient();

// GOOD - Use DI
private readonly ITelemetryService _telemetry;
```

## Monitoring Dashboard Setup

1. Go to Azure Portal → Application Insights
2. Create a new Dashboard
3. Pin these charts:
   - Request rate (requests/sec)
   - Response time (avg)
   - Failed requests (count)
   - Server response time
   - Custom events (by name)
   - Exceptions (count)
   - Dependency duration

## Alerts to Configure

1. **High Error Rate**: Alert when error rate > 5% for 5 minutes
2. **Slow Response Time**: Alert when avg response > 3s for 5 minutes
3. **High Exception Count**: Alert when exceptions > 10 in 5 minutes
4. **Dependency Failures**: Alert when database calls fail > 3 times in 5 minutes

## Testing Locally

To see telemetry locally:

1. Add connection string to `appsettings.Development.json`
2. Run the application
3. Make some requests
4. Check Application Insights in Azure Portal (wait 2-5 minutes)
5. Or use Application Insights Profiler for local debugging

## Support

- Application Insights Docs: https://docs.microsoft.com/azure/azure-monitor/app/
- Kusto Query Docs: https://docs.microsoft.com/azure/data-explorer/kusto/query/
- ASP.NET Core Logging: https://docs.microsoft.com/aspnet/core/fundamentals/logging
