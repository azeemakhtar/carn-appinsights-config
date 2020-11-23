using System.Text.RegularExpressions;
using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;

namespace Carnegie.ApplicationInsights.Common.TelemetryProcessors
{
    /// <summary>
    /// Filter out Serilog logging events to log*.carnegie.se.
    /// </summary>
    public class SeqLogFilterProcessor : ITelemetryProcessor
    {
        private static readonly Regex SeqLogUrlRegex = new Regex(@"log.*\.carnegie\.se");

        private readonly ITelemetryProcessor _next;

        public SeqLogFilterProcessor(ITelemetryProcessor next)
        {
            // Next TelemetryProcessor in the chain
            _next = next;
        }

        public void Process(ITelemetry item)
        {
            if (item is DependencyTelemetry dep)
            {
                if (SeqLogUrlRegex.IsMatch(dep.Target))
                {
                    // Filter the event
                    return;
                }
            }

            // Send the item to the next TelemetryProcessor
            _next.Process(item);
        }
    }
}
