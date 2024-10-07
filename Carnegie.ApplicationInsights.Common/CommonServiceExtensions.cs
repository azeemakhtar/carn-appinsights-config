using Carnegie.ApplicationInsights.Common.TelemetryInitializers;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Extensions.DependencyInjection;

namespace Carnegie.ApplicationInsights.Common
{
    public static class CommonServiceExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="services"></param>
        /// <param name="roleName">Overrides the role name. The default is the assembly name.</param>
        /// <returns></returns>
        public static IServiceCollection EnableApplicationRoles(this IServiceCollection services, string roleName = null)
        {
            // Set application name on Application Insights events
            services.AddSingleton<ITelemetryInitializer>(new ApplicationRoleTelemetryInitializer(roleName));
            return services;
        }

        /// <summary>
        /// Clones tags from Activity.Current.Tags into the request context.
        /// Example usage:
        ///     System.Diagnostics.Activity.Current.AddTag("AccountNo", 123);
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection EnableActivityTags(this IServiceCollection services)
        {
            services.AddSingleton<ITelemetryInitializer>(new ActivityTagsTelemetryInitializer());
            return services;
        }
    }
}
