using System;

namespace Carnegie.ApplicationInsights.Common
{
    public class InstrumentationKeyManager
    {
        public static string GetInstrumentationKey(string environmentName)
        {
            // Check if the calling assembly is a debug build, which most likely indicates a developer machine
            if (AssemblyHelper.GetRunningAssembly().IsDebugBuild())
            {
                return EmptyKey();
            }

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
