using System;
using System.Diagnostics;
using Carnegie.ApplicationInsights.Common;
using Carnegie.ApplicationInsights.Common.TelemetryInitializers;
using Microsoft.ApplicationInsights.Extensibility;
using Serilog;
using Serilog.Configuration;
using Serilog.Events;

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
        /// <param name="roleName">Overrides the role name. The default is the assembly name.</param>
        /// <param name="logLevel">Overrides the minimum log event level required in order to write an event to the sink. The default is verbose.</param>
        public static LoggerConfiguration ApplicationInsightsSink(this LoggerSinkConfiguration writeTo, string instrumentationKey = null, string environmentName = null, string roleName = null, LogEventLevel logLevel = LevelAlias.Minimum)
        {
            var key = instrumentationKey
                      ?? (environmentName != null
                          ? InstrumentationKeyManager.GetInstrumentationKey(environmentName)
                          : null)
                      ?? Guid.Empty.ToString();

            Trace.TraceInformation($"Application Insights instrumentation key used for Serilog log events: {key}");

            var telemetryConfiguration = TelemetryConfiguration.CreateDefault();
            telemetryConfiguration.InstrumentationKey = key;
            telemetryConfiguration.TelemetryInitializers.Add(new ApplicationRoleTelemetryInitializer(roleName));

            return writeTo.ApplicationInsights(telemetryConfiguration, TelemetryConverter.Traces, logLevel);
        }
    }
}
