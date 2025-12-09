using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.Extensibility;

namespace BizBio.API.Telemetry
{
    public class BizBioTelemetryInitializer : ITelemetryInitializer
    {
        private readonly IConfiguration _configuration;

        public BizBioTelemetryInitializer(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void Initialize(ITelemetry telemetry)
        {
            if (string.IsNullOrEmpty(telemetry.Context.Cloud.RoleName))
            {
                telemetry.Context.Cloud.RoleName = "BizBio.API";
            }

            if (string.IsNullOrEmpty(telemetry.Context.Cloud.RoleInstance))
            {
                telemetry.Context.Cloud.RoleInstance = Environment.MachineName;
            }

            // Add custom properties
            telemetry.Context.GlobalProperties["Application"] = "BizBio";
            telemetry.Context.GlobalProperties["Environment"] = _configuration["ASPNETCORE_ENVIRONMENT"] ?? "Production";
            telemetry.Context.GlobalProperties["Version"] = "1.0.0";
        }
    }
}
