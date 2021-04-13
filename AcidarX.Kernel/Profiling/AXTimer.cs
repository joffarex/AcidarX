using System;
using System.Collections.Generic;
using System.Diagnostics;
using AcidarX.Kernel.Logging;
using ImGuiNET;
using Microsoft.Extensions.Logging;

namespace AcidarX.Kernel.Profiling
{
    public sealed class AXTimer : IDisposable
    {
        private static readonly ILogger<AXTimer> Logger = AXLogger.CreateLogger<AXTimer>();

        // private Action<ProfileResult> _callback;
        private static readonly Dictionary<string, double> _profileResults = new();

        private readonly Stopwatch _sw;

        public AXTimer(string name)
        {
            Name = name;
            _sw = new Stopwatch();
            _sw.Start();
        }

        public string Name { get; }

        public void Dispose()
        {
            _sw.Stop();
            _profileResults[Name] = _sw.Elapsed.TotalMilliseconds;
        }

        public static void ImGuiWindow()
        {
            ImGui.Begin("Profile Results");
            ImGui.SetWindowFontScale(1.5f);
            foreach (KeyValuePair<string, double> profileResult in _profileResults)
            {
                ImGui.Text($"{profileResult.Value:0.0000}ms {profileResult.Key}");
            }

            ImGui.End();
        }

        public void RecordTime()
        {
            _sw.Stop();
            _profileResults[Name] = _sw.Elapsed.TotalMilliseconds;
        }
    }
}