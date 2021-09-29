using System.Collections.Generic;
using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;

namespace Carnegie.ApplicationInsights.Common.TelemetryInitializers
{
    /// <summary>
    /// Clones tags from Activity.Current.Tags into the custom properties of the telemetry.
    /// </summary>
    public class ActivityTagsTelemetryInitializer : ITelemetryInitializer
    {
        public void Initialize(ITelemetry telemetry)
        {
            var activityTags = System.Diagnostics.Activity.Current?.Tags;
            if (activityTags == null) 
                return;

            CloneActivityTagsToCustomProperties(activityTags, telemetry);
        }

        private static void CloneActivityTagsToCustomProperties(IEnumerable<KeyValuePair<string, string>> activityTags, ITelemetry telemetry)
        {
            if (telemetry is ISupportProperties telemetryProperties)
            {
                foreach (var (key, value) in activityTags)
                {
                    if (telemetryProperties.Properties.ContainsKey(key))
                    {
                        telemetryProperties.Properties[key] = value;
                    }
                    else
                    {
                        telemetryProperties.Properties.Add(key, value);
                    }
                }
            }
        }
    }
}
