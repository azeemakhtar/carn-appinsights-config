using System;

namespace Carnegie.ApplicationInsights.Common
{
    public class InstrumentationKeyManager
    {
        public static string GetInstrumentationKey(string environmentName)
        {
            if (string.IsNullOrEmpty(environmentName))
            {
                throw new ArgumentNullException(nameof(environmentName));
            }

            switch (environmentName.ToUpper())
            {
                case "DEVELOPMENT":
                    return null;
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
    }
}
