using System;
using System.Globalization;
using AcidarX.Kernel.Logging;
using ImGuiNET;
using Microsoft.Extensions.Logging;

namespace AcidarX.Kernel.Utils
{
    public class FpsUtils
    {
        private static readonly ILogger<FpsUtils> Logger = AXLogger.CreateLogger<FpsUtils>();

        private static readonly double _fpsDisplayDelay = 1;
        private static double _timeBetweenFpsDisplay;

        private static string _currentFps;

        public static void ImGuiWindow(double deltaTime)
        {
            _timeBetweenFpsDisplay -= deltaTime;

            if (_timeBetweenFpsDisplay <= 0)
            {
                _timeBetweenFpsDisplay = _fpsDisplayDelay;
                _currentFps = Get(deltaTime);
            }

            ImGui.Begin("Benchmarks");
            ImGui.SetWindowFontScale(1.4f);
            ImGui.Text($"FPS: {_currentFps}");
            ImGui.End();
        }

        public static string Get
            (double deltaTime) => Math.Round(1.0 / deltaTime, 2).ToString(CultureInfo.InvariantCulture);

        public static void Print(double deltaTime)
        {
            Logger.LogInformation("FPS: {Fps}", Get(deltaTime));
        }
    }
}