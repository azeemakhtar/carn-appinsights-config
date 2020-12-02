using System;

namespace Carnegie.ApplicationInsights.Common
{
    public class InstrumentationKeyManager
    {
        public static string GetInstrumentationKey(string environmentName)
        {
            // If a developer machine or test runner is initializing, then don't log Application Insights events.
            var runningAssembly = AssemblyHelper.GetRunningAssembly();
            if (runningAssembly.IsDebugBuild() || runningAssembly.IsTestRunner())
            {
                return EmptyKey();
            }

            // Map environment name to default instrumentation key
            if (string.IsNullOrEmpty(environmentName))
            {
                throw new ArgumentNullException(nameof(environmentName));
            }

            switch (environmentName.ToUpper())
            {
                case "DEVELOPMENT":
                    return EmptyKey();
                case "TEST":
                    return "d518983e-6365-4eb6-91f8-d89c443b124a";
                case "PREPROD":
                    return null;
                case "PRODUCTION":
                    return null;
                default:
                    throw new ArgumentException($"Unknown environment: {environmentName}");
            }
        }

        /// <summary>
        /// Return empty instrumentation key so that Application Insights is enabled and the events are viewable in Visual Studio,
        /// while nothing is reported into the Azure resources.
        /// </summary>
        /// <returns></returns>
        private static string EmptyKey()
        {
            return Guid.Empty.ToString();
        }
    }
}
