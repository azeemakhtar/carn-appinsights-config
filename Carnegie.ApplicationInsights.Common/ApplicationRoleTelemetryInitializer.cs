using System.Reflection;
using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.Extensibility;

namespace Carnegie.ApplicationInsights.Common
{
    public class ApplicationRoleTelemetryInitializer : ITelemetryInitializer
    {
        private string _roleName;

        public void Initialize(ITelemetry telemetry)
        {
            if (string.IsNullOrEmpty(telemetry.Context.Cloud.RoleName))
            {
                _roleName ??= (Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly()).GetName().Name;
                telemetry.Context.Cloud.RoleName = _roleName;
            }
        }
    }
}
