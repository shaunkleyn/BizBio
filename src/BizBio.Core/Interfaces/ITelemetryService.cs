namespace BizBio.Core.Interfaces;

public interface ITelemetryService
{
    /// <summary>
    /// Track a custom event with optional properties and metrics
    /// </summary>
    void TrackEvent(string eventName, Dictionary<string, string>? properties = null, Dictionary<string, double>? metrics = null);

    /// <summary>
    /// Track a custom metric
    /// </summary>
    void TrackMetric(string metricName, double value, Dictionary<string, string>? properties = null);

    /// <summary>
    /// Track a dependency call (external service, database, etc.)
    /// </summary>
    void TrackDependency(string dependencyType, string dependencyName, string data, DateTimeOffset startTime, TimeSpan duration, bool success);

    /// <summary>
    /// Track an exception with additional context
    /// </summary>
    void TrackException(Exception exception, Dictionary<string, string>? properties = null, Dictionary<string, double>? metrics = null);

    /// <summary>
    /// Track a page view
    /// </summary>
    void TrackPageView(string pageName, Dictionary<string, string>? properties = null);

    /// <summary>
    /// Track a user action/event
    /// </summary>
    void TrackUserAction(string userId, string actionName, Dictionary<string, string>? properties = null);

    /// <summary>
    /// Track business metrics
    /// </summary>
    void TrackBusinessMetric(string metricName, double value, string? userId = null, Dictionary<string, string>? properties = null);

    /// <summary>
    /// Flush all pending telemetry
    /// </summary>
    void Flush();
}
