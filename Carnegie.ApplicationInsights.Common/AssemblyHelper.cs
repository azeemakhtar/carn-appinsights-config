using System;
using System.Diagnostics;
using System.Reflection;

namespace Carnegie.ApplicationInsights.Common
{
    public static class AssemblyHelper
    {
        public static Assembly GetRunningAssembly()
        {
            return Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly();
        }

        public static bool IsDebugBuild(this Assembly assembly)
        {
            if (assembly == null)
            {
                throw new ArgumentNullException(nameof(assembly));
            }

            return assembly.GetCustomAttribute<DebuggableAttribute>()?.IsJITTrackingEnabled ?? false;
        }
    }
}