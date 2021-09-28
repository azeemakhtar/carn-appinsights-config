using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;

namespace Carnegie.ApplicationInsights.Common
{
    public interface IMonitoringHelper
    {
        /// <summary>
        /// Explicitly create an request/operation so that all events within it share a common operation id.
        /// For REST services this is done automatically by Application Insights, but this method is useful in
        /// background services etc where there are no automatic requests.
        /// </summary>
        /// <remarks>The operation is active until disposed, so the returned object should normally be used in a using statement.</remarks>
        /// <param name="operationName"></param>
        /// <returns></returns>
        CustomRequest CreateRequest(string operationName);

        /// <summary>
        /// Track a custom event into Application Insights.
        /// 
        /// Recommended format: {Application}.{EventName}
        /// Example: AccountService.AccountExportCompleted
        /// </summary>
        /// <param name="eventName">The name of the event.</param>
        /// <param name="data">Optional data to attach to the event.</param>
        /// <param name="metrics">Optional numeric metrics to attach to the event.</param>
        void TrackCustomEvent(string eventName, object data = null, IDictionary<string, double> metrics = null);

        /// <summary>
        /// Track a custom event into Application Insights.
        /// 
        /// Recommended format: {Application}.{EventName}
        /// Example: AccountService.AccountExportCompleted
        /// </summary>
        /// <param name="eventName">The name of the event.</param>
        /// <param name="properties">Name-value pairs with custom data to attach to the event.</param>
        /// <param name="metrics">Optional numeric metrics to attach to the event.</param>
        void TrackCustomEvent(string eventName, IDictionary<string, string> properties, IDictionary<string, double> metrics = null);
    }

    public class MonitoringHelper : IMonitoringHelper
    {
        private readonly TelemetryClient _telemetryClient;

        public MonitoringHelper(TelemetryClient telemetryClient)
        {
            _telemetryClient = telemetryClient;
        }

        /// <summary>
        /// Explicitly create an request/operation so that all events within it share a common operation id.
        /// For REST services this is done automatically by Application Insights, but this method is useful in
        /// background services etc where there are no automatic requests.
        /// </summary>
        /// <remarks>The operation is active until disposed, so the returned object should normally be used in a using statement.</remarks>
        /// <param name="operationName"></param>
        /// <returns></returns>
        public CustomRequest CreateRequest(string operationName)
        {
            // Register an Application Insights request event to use as a root for all other events for this message
            var operationTelemetry = new RequestTelemetry { Name = operationName };
            operationTelemetry.Context.Operation.Name = operationName;
            var requestTelemetry = _telemetryClient?.StartOperation(operationTelemetry);

            return new CustomRequest(requestTelemetry);
        }

        /// <summary>
        /// Track a custom event into Application Insights.
        /// 
        /// Recommended format: {Application}.{EventName}
        /// Example: AccountService.AccountExportCompleted
        /// </summary>
        /// <param name="eventName">The name of the event.</param>
        /// <param name="data">Optional data to attach to the event.</param>
        /// <param name="metrics">Optional numeric metrics to attach to the event.</param>
        public void TrackCustomEvent(string eventName, object data = null, IDictionary<string, double> metrics = null)
        {
            var props = data?
                .GetType()
                .GetProperties()
            .ToDictionary(p => p.Name, p => string.Format(CultureInfo.InvariantCulture, "{0}", p.GetValue(data)));

            _telemetryClient?.TrackEvent(eventName, props, metrics);
        }


        /// <summary>
        /// Track a custom event into Application Insights.
        /// 
        /// Recommended format: {Application}.{EventName}
        /// Example: AccountService.AccountExportCompleted
        /// </summary>
        /// <param name="eventName">The name of the event.</param>
        /// <param name="properties">Name-value pairs with custom data to attach to the event.</param>
        /// <param name="metrics">Optional numeric metrics to attach to the event.</param>
        public void TrackCustomEvent(string eventName, IDictionary<string,string> properties, IDictionary<string, double> metrics = null)
        {
            _telemetryClient?.TrackEvent(eventName, properties, metrics);
        }
    }
}
