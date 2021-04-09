namespace AcidarX.Core.Renderer
{
    public static class AXStatistics
    {
        public static uint DrawCalls { get; set; }
        public static uint QuadCount { get; set; }

        public static void ImGuiWindow()
        {
            ImGuiNET.ImGui.Begin("Statistics");
            ImGuiNET.ImGui.SetWindowFontScale(1.4f);
            ImGuiNET.ImGui.Text($"QuadCount: {QuadCount}");
            ImGuiNET.ImGui.Text($"DrawCalls: {DrawCalls}");
            ImGuiNET.ImGui.Text($"VertexCount: {QuadCount * 4}");
            ImGuiNET.ImGui.Text($"IndexCount: {QuadCount * 6}");
            ImGuiNET.ImGui.End();
        }

        public static void Reset()
        {
            DrawCalls = 0;
            QuadCount = 0;
        }
    }
}