using System.Diagnostics;
using Carnegie.ApplicationInsights.Common;
using Carnegie.ApplicationInsights.Common.TelemetryInitializers;
using Carnegie.ApplicationInsights.Common.TelemetryProcessors;
using Microsoft.ApplicationInsights.DependencyCollector;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.ApplicationInsights.WorkerService;
using Microsoft.Extensions.DependencyInjection;

namespace Carnegie.ApplicationInsights.Worker
{
    public static class WorkerServiceExtensions
    {
        /// <summary>
        /// Enable Application insights and return the applied instrumentation key.
        /// NB: Call this after 'services.AddHostedService()'.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="instrumentationKey"></param>
        /// <param name="environmentName"></param>
        /// <param name="adaptiveSampling"></param>
        /// <param name="roleName">Overrides the role name. The default is the assembly name.</param>
        /// <returns>The applied instrumentation key, or null if not enabled.</returns>
        public static string AddCarnegieApplicationInsightsWorker(this IServiceCollection services,
            string instrumentationKey = null, string environmentName = null, bool adaptiveSampling = false,
            string roleName = null)
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
                .AddApplicationInsightsTelemetryWorkerService(
                    new ApplicationInsightsServiceOptions
                    {
                        InstrumentationKey = key,
                        EnableAdaptiveSampling = adaptiveSampling
                    })
                .AddSingleton<ITelemetryInitializer, AuthenticatedUserIdTelemetryInitializer>()
                .AddSingleton<ITelemetryInitializer, SoapActionHeaderTelemetryInitializer>()
                .AddApplicationInsightsTelemetryProcessor<SeqLogFilterProcessor>()
                .EnableSqlLogging()
                .EnableApplicationRoles(roleName)
                .AddSingleton<IMonitoringHelper, MonitoringHelper>();

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
