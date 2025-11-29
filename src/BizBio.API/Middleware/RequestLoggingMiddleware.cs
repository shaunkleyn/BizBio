using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using System.Diagnostics;

namespace BizBio.API.Middleware;

public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;
    private readonly TelemetryClient _telemetryClient;

    public RequestLoggingMiddleware(
        RequestDelegate next,
        ILogger<RequestLoggingMiddleware> logger,
        TelemetryClient telemetryClient)
    {
        _next = next;
        _logger = logger;
        _telemetryClient = telemetryClient;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();
        var requestId = context.TraceIdentifier;

        // Log request details
        _logger.LogInformation(
            "Incoming Request: {Method} {Path} | TraceId: {TraceId} | IP: {IpAddress}",
            context.Request.Method,
            context.Request.Path,
            requestId,
            context.Connection.RemoteIpAddress);

        // Track custom event for request start
        var requestTelemetry = new RequestTelemetry
        {
            Name = $"{context.Request.Method} {context.Request.Path}",
            Timestamp = DateTimeOffset.UtcNow,
            Url = new Uri($"{context.Request.Scheme}://{context.Request.Host}{context.Request.Path}{context.Request.QueryString}")
        };

        // Add custom properties
        requestTelemetry.Properties.Add("Method", context.Request.Method);
        requestTelemetry.Properties.Add("Path", context.Request.Path);
        requestTelemetry.Properties.Add("QueryString", context.Request.QueryString.ToString());
        requestTelemetry.Properties.Add("UserAgent", context.Request.Headers.UserAgent.ToString());
        requestTelemetry.Properties.Add("IpAddress", context.Connection.RemoteIpAddress?.ToString() ?? "Unknown");

        // Add user information if authenticated
        if (context.User?.Identity?.IsAuthenticated == true)
        {
            requestTelemetry.Properties.Add("UserId", context.User.Identity.Name ?? "Unknown");
            requestTelemetry.Properties.Add("AuthenticationType", context.User.Identity.AuthenticationType ?? "Unknown");
        }

        try
        {
            // Store original response body stream
            var originalBodyStream = context.Response.Body;

            using (var responseBody = new MemoryStream())
            {
                context.Response.Body = responseBody;

                // Call the next middleware
                await _next(context);

                // Log response
                stopwatch.Stop();

                requestTelemetry.ResponseCode = context.Response.StatusCode.ToString();
                requestTelemetry.Duration = stopwatch.Elapsed;
                requestTelemetry.Success = context.Response.StatusCode < 400;

                _logger.LogInformation(
                    "Outgoing Response: {StatusCode} | {Method} {Path} | Duration: {Duration}ms | TraceId: {TraceId}",
                    context.Response.StatusCode,
                    context.Request.Method,
                    context.Request.Path,
                    stopwatch.ElapsedMilliseconds,
                    requestId);

                // Copy the response back to the original stream
                responseBody.Seek(0, SeekOrigin.Begin);
                await responseBody.CopyToAsync(originalBodyStream);
            }
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            requestTelemetry.ResponseCode = "500";
            requestTelemetry.Duration = stopwatch.Elapsed;
            requestTelemetry.Success = false;

            _logger.LogError(ex,
                "Request Failed: {Method} {Path} | Duration: {Duration}ms | TraceId: {TraceId}",
                context.Request.Method,
                context.Request.Path,
                stopwatch.ElapsedMilliseconds,
                requestId);

            throw;
        }
        finally
        {
            // Track the request
            _telemetryClient.TrackRequest(requestTelemetry);
        }
    }
}
