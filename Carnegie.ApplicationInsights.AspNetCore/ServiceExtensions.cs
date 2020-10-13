using System.Diagnostics;
using Carnegie.ApplicationInsights.Common;
using Microsoft.ApplicationInsights.AspNetCore.Extensions;
using Microsoft.ApplicationInsights.DependencyCollector;
using Microsoft.Extensions.DependencyInjection;

namespace Carnegie.ApplicationInsights.AspNetCore
{
    public static class ServiceExtensions
    {
        /// <summary>
        /// Enable Application insights and return the applied instrumentation key.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="instrumentationKey"></param>
        /// <param name="environmentName"></param>
        /// <returns>The applied instrumentation key, or null if not enabled.</returns>
        public static string AddCarnegieApplicationInsightsAspNetCore(this IServiceCollection services, string instrumentationKey = null, string environmentName = null)
        {
            if (string.IsNullOrEmpty(instrumentationKey) && string.IsNullOrEmpty(environmentName))
            {
                Trace.TraceWarning("No instrumentation key configured. Application Insights not enabled.");
                return null;
            }
            
            var key = instrumentationKey ?? InstrumentationKeyManager.GetInstrumentationKey(environmentName);
            if (string.IsNullOrEmpty(key))
            {
                Trace.TraceInformation($"Application Insights not enabled for environment: {environmentName}");
                return null;
            }

            services
                .AddApplicationInsightsTelemetry(
                    new ApplicationInsightsServiceOptions { InstrumentationKey = key })
                .EnableSqlLogging()
                .EnableApplicationRoles();

            return key;
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
