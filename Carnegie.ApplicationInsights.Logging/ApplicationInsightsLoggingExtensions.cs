using System;
using System.Diagnostics;
using Carnegie.ApplicationInsights.Common;
using Carnegie.ApplicationInsights.Common.TelemetryInitializers;
using Microsoft.ApplicationInsights.Extensibility;
using Serilog;
using Serilog.Configuration;

namespace Carnegie.ApplicationInsights.Logging
{
    public static class ApplicationInsightsLoggingExtensions
    {
        /// <summary>
        /// Add the ApplicationInsights sink to Serilog. Use <paramref name="instrumentationKey"/> or <paramref name="environmentName"/>
        /// to control where the events are written.
        /// </summary>
        /// <param name="writeTo"></param>
        /// <param name="instrumentationKey">The instrumentation key to write the events to. If null, then <paramref name="environmentName"/> should be set instead.</param>
        /// <param name="environmentName">The environment to use for instrumentation key lookup.</param>
        public static LoggerConfiguration ApplicationInsightsSink(this LoggerSinkConfiguration writeTo, string instrumentationKey = null, string environmentName = null)
        {
            var key = instrumentationKey
                      ?? (environmentName != null
                          ? InstrumentationKeyManager.GetInstrumentationKey(environmentName)
                          : null)
                      ?? Guid.Empty.ToString();

            Trace.TraceInformation($"Application Insights instrumentation key used for Serilog log events: {key}");

            var telemetryConfiguration = TelemetryConfiguration.CreateDefault();
            telemetryConfiguration.InstrumentationKey = key;
            telemetryConfiguration.TelemetryInitializers.Add(new ApplicationRoleTelemetryInitializer());

            return writeTo.ApplicationInsights(telemetryConfiguration, TelemetryConverter.Traces);
        }
    }
}
