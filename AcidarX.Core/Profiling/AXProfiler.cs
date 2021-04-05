// #define AX_PROFILE

using System;
using AcidarX.Core.Logging;
using Microsoft.Extensions.Logging;

namespace AcidarX.Core.Profiling
{
    public class AXProfiler
    {
        private static readonly ILogger<AXProfiler> Logger = AXLogger.CreateLogger<AXProfiler>();

        public static void Capture(string name, Action funcToProfile)
        {
#if AX_PROFILE
            using var timer = new InstrumentationTimer(name);
#endif
            funcToProfile();
        }

        public static void Capture(Action funcToProfile)
        {
#if AX_PROFILE
            MethodBase method = new StackTrace().GetFrame(1)?.GetMethod();
            Logger.Assert(method != null, "Should have a caller or stack trace");

            string methodName = method.Name;
            string className = method.ReflectedType.Name;

            using var timer = new InstrumentationTimer($"{className}::${methodName}");
#endif
            funcToProfile();
        }


        public static T Capture<T>(string name, Func<T> funcToProfile)
        {
#if AX_PROFILE
            using var timer = new InstrumentationTimer(name);
#endif
            return funcToProfile();
        }

        public static T Capture<T>(Func<T> funcToProfile)
        {
#if AX_PROFILE
            MethodBase method = new StackTrace().GetFrame(1)?.GetMethod();
            Logger.Assert(method != null, "Should have a caller or stack trace");

            string methodName = method.Name;
            string className = method.ReflectedType.Name;

            using var timer = new InstrumentationTimer($"{className}::{methodName}");
#endif
            return funcToProfile();
        }
    }
}