using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Extensions.DependencyInjection;

namespace Carnegie.ApplicationInsights.Common
{
    public static class ServiceExtensions
    {
        public static IServiceCollection EnableApplicationRoles(this IServiceCollection services)
        {
            // Set application name on Application Insights events
            services.AddSingleton<ITelemetryInitializer, ApplicationRoleTelemetryInitializer>();
            return services;
        }
    }
}
