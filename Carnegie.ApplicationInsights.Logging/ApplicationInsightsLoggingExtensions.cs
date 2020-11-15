using Carnegie.ApplicationInsights.Common;
using Microsoft.ApplicationInsights.Extensibility;
using Serilog;
using Serilog.Configuration;

namespace Carnegie.ApplicationInsights.Logging
{
    public static class ApplicationInsightsLoggingExtensions
    {
        //public static void ApplicationInsightsSink(this LoggerSinkConfiguration writeTo)
        //{
        //    writeTo.ApplicationInsights(TelemetryConfiguration.Active, TelemetryConverter.Traces);
        //}
        
        public static void ApplicationInsightsSink(this LoggerSinkConfiguration writeTo, string instrumentationKey)
        {
            var telemetryConfiguration = TelemetryConfiguration.CreateDefault();
            telemetryConfiguration.InstrumentationKey = instrumentationKey;
            telemetryConfiguration.TelemetryInitializers.Add(new ApplicationRoleTelemetryInitializer());

            writeTo.ApplicationInsights(telemetryConfiguration, TelemetryConverter.Traces);
        }
    }
}
