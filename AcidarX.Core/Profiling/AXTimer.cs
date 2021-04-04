using System;
using System.Collections.Generic;
using System.Diagnostics;
using AcidarX.Core.Logging;
using Microsoft.Extensions.Logging;

namespace AcidarX.Core.Profiling
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
            ImGuiNET.ImGui.Begin("Profile Results");
            ImGuiNET.ImGui.SetWindowFontScale(1.5f);
            foreach (KeyValuePair<string, double> profileResult in _profileResults)
            {
                ImGuiNET.ImGui.Text($"{profileResult.Value:0.0000}ms {profileResult.Key}");
            }

            ImGuiNET.ImGui.End();
        }

        public void RecordTime()
        {
            _sw.Stop();
            _profileResults[Name] = _sw.Elapsed.TotalMilliseconds;
        }
    }
}