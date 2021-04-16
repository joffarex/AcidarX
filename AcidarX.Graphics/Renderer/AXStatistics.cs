using ImGuiNET;

namespace AcidarX.Graphics.Renderer
{
    public static class AXStatistics
    {
        public static uint DrawCalls { get; set; }
        public static uint QuadCount { get; set; }

        public static void ImGuiWindow()
        {
            ImGui.Begin("Statistics");
            ImGui.SetWindowFontScale(1.4f);
            ImGui.Text($"QuadCount: {QuadCount}");
            ImGui.Text($"DrawCalls: {DrawCalls}");
            ImGui.Text($"VertexCount: {QuadCount * 4}");
            ImGui.Text($"IndexCount: {QuadCount * 6}");
            ImGui.End();
        }

        public static void Reset()
        {
            DrawCalls = 0;
            QuadCount = 0;
        }
    }
}