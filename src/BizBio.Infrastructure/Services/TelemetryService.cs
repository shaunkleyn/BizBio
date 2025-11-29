using BizBio.Core.Interfaces;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.Extensions.Logging;

namespace BizBio.Infrastructure.Services;

public class TelemetryService : ITelemetryService
{
    private readonly TelemetryClient _telemetryClient;
    private readonly ILogger<TelemetryService> _logger;

    public TelemetryService(TelemetryClient telemetryClient, ILogger<TelemetryService> logger)
    {
        _telemetryClient = telemetryClient;
        _logger = logger;
    }

    public void TrackEvent(string eventName, Dictionary<string, string>? properties = null, Dictionary<string, double>? metrics = null)
    {
        try
        {
            _telemetryClient.TrackEvent(eventName, properties, metrics);
            _logger.LogDebug("Event tracked: {EventName}", eventName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to track event: {EventName}", eventName);
        }
    }

    public void TrackMetric(string metricName, double value, Dictionary<string, string>? properties = null)
    {
        try
        {
            var metric = new MetricTelemetry(metricName, value);

            if (properties != null)
            {
                foreach (var prop in properties)
                {
                    metric.Properties.Add(prop.Key, prop.Value);
                }
            }

            _telemetryClient.TrackMetric(metric);
            _logger.LogDebug("Metric tracked: {MetricName} = {Value}", metricName, value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to track metric: {MetricName}", metricName);
        }
    }

    public void TrackDependency(string dependencyType, string dependencyName, string data, DateTimeOffset startTime, TimeSpan duration, bool success)
    {
        try
        {
            var dependency = new DependencyTelemetry
            {
                Type = dependencyType,
                Name = dependencyName,
                Data = data,
                Timestamp = startTime,
                Duration = duration,
                Success = success
            };

            _telemetryClient.TrackDependency(dependency);
            _logger.LogDebug("Dependency tracked: {DependencyName} ({DependencyType}) - Success: {Success}, Duration: {Duration}ms",
                dependencyName, dependencyType, success, duration.TotalMilliseconds);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to track dependency: {DependencyName}", dependencyName);
        }
    }

    public void TrackException(Exception exception, Dictionary<string, string>? properties = null, Dictionary<string, double>? metrics = null)
    {
        try
        {
            var exceptionTelemetry = new ExceptionTelemetry(exception)
            {
                SeverityLevel = SeverityLevel.Error,
                Timestamp = DateTimeOffset.UtcNow
            };

            if (properties != null)
            {
                foreach (var prop in properties)
                {
                    exceptionTelemetry.Properties.Add(prop.Key, prop.Value);
                }
            }

            if (metrics != null)
            {
                foreach (var metric in metrics)
                {
                    exceptionTelemetry.Metrics.Add(metric.Key, metric.Value);
                }
            }

            _telemetryClient.TrackException(exceptionTelemetry);
            _logger.LogError(exception, "Exception tracked in telemetry");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to track exception in telemetry");
        }
    }

    public void TrackPageView(string pageName, Dictionary<string, string>? properties = null)
    {
        try
        {
            var pageView = new PageViewTelemetry(pageName)
            {
                Timestamp = DateTimeOffset.UtcNow
            };

            if (properties != null)
            {
                foreach (var prop in properties)
                {
                    pageView.Properties.Add(prop.Key, prop.Value);
                }
            }

            _telemetryClient.TrackPageView(pageView);
            _logger.LogDebug("Page view tracked: {PageName}", pageName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to track page view: {PageName}", pageName);
        }
    }

    public void TrackUserAction(string userId, string actionName, Dictionary<string, string>? properties = null)
    {
        try
        {
            var eventProperties = properties ?? new Dictionary<string, string>();
            eventProperties["UserId"] = userId;
            eventProperties["Action"] = actionName;
            eventProperties["Timestamp"] = DateTimeOffset.UtcNow.ToString("o");

            _telemetryClient.TrackEvent($"UserAction_{actionName}", eventProperties);
            _logger.LogInformation("User action tracked: {UserId} - {ActionName}", userId, actionName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to track user action: {ActionName} for user {UserId}", actionName, userId);
        }
    }

    public void TrackBusinessMetric(string metricName, double value, string? userId = null, Dictionary<string, string>? properties = null)
    {
        try
        {
            var metricProperties = properties ?? new Dictionary<string, string>();
            metricProperties["MetricType"] = "Business";
            metricProperties["Timestamp"] = DateTimeOffset.UtcNow.ToString("o");

            if (!string.IsNullOrEmpty(userId))
            {
                metricProperties["UserId"] = userId;
            }

            var metric = new MetricTelemetry(metricName, value);

            foreach (var prop in metricProperties)
            {
                metric.Properties.Add(prop.Key, prop.Value);
            }

            _telemetryClient.TrackMetric(metric);
            _logger.LogInformation("Business metric tracked: {MetricName} = {Value}", metricName, value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to track business metric: {MetricName}", metricName);
        }
    }

    public void Flush()
    {
        try
        {
            _telemetryClient.FlushAsync(CancellationToken.None).GetAwaiter().GetResult();
            _logger.LogDebug("Telemetry flushed");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to flush telemetry");
        }
    }
}
