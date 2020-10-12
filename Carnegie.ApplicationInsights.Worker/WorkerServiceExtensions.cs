using System.Diagnostics;
using Carnegie.ApplicationInsights.Common;
using Microsoft.ApplicationInsights.DependencyCollector;
using Microsoft.ApplicationInsights.WorkerService;
using Microsoft.Extensions.DependencyInjection;

namespace Carnegie.ApplicationInsights.Worker
{
    public static class WorkerServiceExtensions
    {
        public static IServiceCollection AddCarnegieApplicationInsightsWorker(this IServiceCollection services, string instrumentationKey = null, string environmentName = null)
        {
            if (string.IsNullOrEmpty(instrumentationKey) && string.IsNullOrEmpty(environmentName))
            {
                Trace.TraceInformation("No instrumentation key configured. Application Insights not enabled.");
            }

            var key = instrumentationKey ?? InstrumentationKeyManager.GetInstrumentationKey(environmentName);
        
            services
                .AddApplicationInsightsTelemetryWorkerService(
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
