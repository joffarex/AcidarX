using System;
using System.Diagnostics;
using System.Reflection;
using AcidarX.Core.Logging;
using Microsoft.Extensions.Logging;

namespace AcidarX.Core.Profiling
{
    public class AXProfiler
    {
        private static readonly ILogger<AXProfiler> Logger = AXLogger.CreateLogger<AXProfiler>();

        public static void Capture(string name, Action funcToProfile)
        {
            using var timer = new InstrumentationTimer(name);
            funcToProfile();
        }

        public static void Capture(Action funcToProfile)
        {
            MethodBase method = new StackTrace().GetFrame(1)?.GetMethod();
            Logger.Assert(method != null, "Should have a caller or stack trace");

            string methodName = method.Name;
            string className = method.ReflectedType.Name;

            using var timer = new InstrumentationTimer($"{className}::${methodName}");
            funcToProfile();
        }


        public static T Capture<T>(string name, Func<T> funcToProfile)
        {
            using var timer = new InstrumentationTimer(name);
            return funcToProfile();
        }

        public static T Capture<T>(Func<T> funcToProfile)
        {
            MethodBase method = new StackTrace().GetFrame(1)?.GetMethod();
            Logger.Assert(method != null, "Should have a caller or stack trace");

            string methodName = method.Name;
            string className = method.ReflectedType.Name;

            using var timer = new InstrumentationTimer($"{className}::{methodName}");
            return funcToProfile();
        }
    }
}