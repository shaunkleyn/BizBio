using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using System.Net;
using System.Text.Json;

namespace BizBio.API.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;
    private readonly TelemetryClient _telemetryClient;
    private readonly IHostEnvironment _environment;

    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger,
        TelemetryClient telemetryClient,
        IHostEnvironment environment)
    {
        _next = next;
        _logger = logger;
        _telemetryClient = telemetryClient;
        _environment = environment;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        // Log the exception
        _logger.LogError(exception,
            "Unhandled exception occurred | Path: {Path} | Method: {Method} | TraceId: {TraceId} | User: {User}",
            context.Request.Path,
            context.Request.Method,
            context.TraceIdentifier,
            context.User?.Identity?.Name ?? "Anonymous");

        // Track exception in Application Insights
        var exceptionTelemetry = new ExceptionTelemetry(exception)
        {
            SeverityLevel = SeverityLevel.Error,
            Timestamp = DateTimeOffset.UtcNow
        };

        // Add custom properties
        exceptionTelemetry.Properties.Add("Path", context.Request.Path);
        exceptionTelemetry.Properties.Add("Method", context.Request.Method);
        exceptionTelemetry.Properties.Add("QueryString", context.Request.QueryString.ToString());
        exceptionTelemetry.Properties.Add("TraceId", context.TraceIdentifier);
        exceptionTelemetry.Properties.Add("IpAddress", context.Connection.RemoteIpAddress?.ToString() ?? "Unknown");
        exceptionTelemetry.Properties.Add("UserAgent", context.Request.Headers.UserAgent.ToString());

        if (context.User?.Identity?.IsAuthenticated == true)
        {
            exceptionTelemetry.Properties.Add("UserId", context.User.Identity.Name ?? "Unknown");
        }

        // Track the exception
        _telemetryClient.TrackException(exceptionTelemetry);

        // Determine status code and error message
        var (statusCode, message) = exception switch
        {
            UnauthorizedAccessException => (HttpStatusCode.Unauthorized, "Unauthorized access"),
            ArgumentException => (HttpStatusCode.BadRequest, exception.Message),
            KeyNotFoundException => (HttpStatusCode.NotFound, "Resource not found"),
            InvalidOperationException => (HttpStatusCode.BadRequest, exception.Message),
            _ => (HttpStatusCode.InternalServerError, "An unexpected error occurred")
        };

        // Create error response
        var response = new
        {
            success = false,
            error = new
            {
                message = message,
                traceId = context.TraceIdentifier,
                timestamp = DateTime.UtcNow,
                // Only include stack trace in development
                details = _environment.IsDevelopment() ? exception.ToString() : null
            }
        };

        // Set response
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        var jsonResponse = JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        });

        await context.Response.WriteAsync(jsonResponse);

        // Track custom event for error response
        _telemetryClient.TrackEvent("ErrorResponse", new Dictionary<string, string>
        {
            { "StatusCode", ((int)statusCode).ToString() },
            { "ErrorMessage", message },
            { "ExceptionType", exception.GetType().Name },
            { "Path", context.Request.Path },
            { "TraceId", context.TraceIdentifier }
        });
    }
}
