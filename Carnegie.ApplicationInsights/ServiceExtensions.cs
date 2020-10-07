using Carnegie.ApplicationInsights.Common;
using Microsoft.ApplicationInsights.AspNetCore.Extensions;
using Microsoft.ApplicationInsights.DependencyCollector;
using Microsoft.Extensions.DependencyInjection;

namespace Carnegie.ApplicationInsights.AspNetCore
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddCarnegieApplicationInsightsAspNetCore(this IServiceCollection services, string instrumentationKey = null)
        {
            var key = instrumentationKey ?? InstrumentationKeyManager.GetInstrumentationKey();

            services
                .AddApplicationInsightsTelemetry(
                    new ApplicationInsightsServiceOptions { InstrumentationKey = key })
                .EnableSqlLogging()
                .EnableApplicationRoles();

            return services;
        }

        private static IServiceCollection EnableSqlLogging(this IServiceCollection services)
        {
            // Enable logging of SQL statements
            // https://docs.microsoft.com/en-us/azure/azure-monitor/app/asp-net-dependencies#advanced-sql-tracking-to-get-full-sql-query
            services.ConfigureTelemetryModule<DependencyTrackingTelemetryModule>((m, o) =>
            {
                m.EnableSqlCommandTextInstrumentation = true;
            });

            return services;
        }
    }
}
