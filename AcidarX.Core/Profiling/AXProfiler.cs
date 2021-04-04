using System;

namespace AcidarX.Core.Profiling
{
    public static class AXProfiler
    {
        public static void Capture(string name, Action funcToProfile)
        {
            using var timer = new InstrumentationTimer(name);
            funcToProfile();
        }
    }
}