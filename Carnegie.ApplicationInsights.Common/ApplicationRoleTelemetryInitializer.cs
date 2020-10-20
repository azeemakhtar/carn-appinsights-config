using System;
using System.Reflection;
using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.Extensibility;

namespace Carnegie.ApplicationInsights.Common
{
    public class ApplicationRoleTelemetryInitializer : ITelemetryInitializer
    {
        private string _roleName;
        private string _instanceName;

        public void Initialize(ITelemetry telemetry)
        {
            if (string.IsNullOrEmpty(telemetry.Context.Cloud.RoleName))
            {
                // Set role name to service or worker name so that events more easily can be associated to the correct components.
                // Also necessary for the application map to work.
                _roleName ??= (Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly()).GetName().Name;
                telemetry.Context.Cloud.RoleName = _roleName;

                // Explicitly set instance name to avoid mixing computer name ("TSSE109") and network names ("TSSE109.carnnet.com") in events.
                _instanceName ??= Environment.MachineName;
                telemetry.Context.Cloud.RoleInstance = _instanceName;
            }
        }
    }
}
