namespace AcidarX.Core.Windowing
{
    public class AXWindowOptions
    {
        public AXWindowOptions()
        {
        }

        public AXWindowOptions(string title, int width, int height, bool vSync)
        {
            Title = title;
            Width = width;
            Height = height;
            VSync = vSync;
        }

        public string Title { get; init; }
        public int Width { get; init; }
        public int Height { get; init; }
        public bool VSync { get; init; }

        public static AXWindowOptions CreateDefault() => new("AcidarX", 1280, 720, false);
    }
}