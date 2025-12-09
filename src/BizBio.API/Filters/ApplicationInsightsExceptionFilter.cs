using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.AspNetCore.Mvc.Filters;

namespace BizBio.API.Filters
{
    public class ApplicationInsightsExceptionFilter : IExceptionFilter
    {
        private readonly TelemetryClient _telemetryClient;

        public ApplicationInsightsExceptionFilter(TelemetryClient telemetryClient)
        {
            _telemetryClient = telemetryClient;
        }

        public void OnException(ExceptionContext context)
        {
            var exception = context.Exception;

            var telemetry = new ExceptionTelemetry(exception)
            {
                SeverityLevel = SeverityLevel.Error
            };

            // Add custom properties
            telemetry.Properties.Add("HandledAt", "Platform");
            telemetry.Properties.Add("Controller", context.RouteData.Values["controller"]?.ToString() ?? "Unknown");
            telemetry.Properties.Add("Action", context.RouteData.Values["action"]?.ToString() ?? "Unknown");
            telemetry.Properties.Add("UserAgent", context.HttpContext.Request.Headers["User-Agent"].ToString());
            telemetry.Properties.Add("RequestPath", context.HttpContext.Request.Path);
            telemetry.Properties.Add("RequestMethod", context.HttpContext.Request.Method);

            if (context.HttpContext.User?.Identity?.IsAuthenticated == true)
            {
                telemetry.Properties.Add("UserId", context.HttpContext.User.Identity.Name ?? "Unknown");
            }

            _telemetryClient.TrackException(telemetry);
        }
    }
}
