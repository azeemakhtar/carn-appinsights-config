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

        // Check if the assembly is a debug build, which most likely indicates a developer machine.
        public static bool IsDebugBuild(this Assembly assembly)
        {
            if (assembly == null)
            {
                throw new ArgumentNullException(nameof(assembly));
            }

            return assembly.GetCustomAttribute<DebuggableAttribute>()?.IsJITTrackingEnabled ?? false;
        }

        public static bool IsTestRunner(this Assembly assembly)
        {
            if (assembly == null)
            {
                throw new ArgumentNullException(nameof(assembly));
            }

            switch (assembly.GetName().Name)
            {
                case "ReSharperTestRunner64":
                case "testhost": // The dotnet test runner, also used in Visual Studio
                    return true;
            }

            return false;
        }

    }
}