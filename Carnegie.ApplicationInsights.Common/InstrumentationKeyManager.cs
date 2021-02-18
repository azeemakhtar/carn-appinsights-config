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
                    return "d518983e-6365-4eb6-91f8-d89c443b124a"; // Old
                    //return "2c0b4ed7-0011-444d-97b6-3206a5da2953"; // New, we´ll switch when all developers have access
                case "TEST1":
                    return "5e758f1c-66ac-4fc7-a630-ea8e881b1986";
                case "PREPROD":
                    return "74f0e7d0-6cf3-4513-a35e-ca4c1ebd2663";
                case "PREPROD1":
                    return "aedeaa4a-3264-4994-8205-13bd7721da72";
                case "PRODUCTION":
                    return "0325c234-bedb-4d27-8037-f0fef59d75b4";
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
