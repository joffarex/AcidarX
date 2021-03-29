namespace AcidarX.Core.Windowing
{
    public class AXWindowOptions
    {
        public AXWindowOptions(string title, int width, int height, bool vSync)
        {
            Title = title;
            Width = width;
            Height = height;
            VSync = vSync;
        }

        public string Title { get; }
        public int Width { get; }
        public int Height { get; }
        public bool VSync { get; }

        public static AXWindowOptions CreateDefault() => new("AcidarX", 1280, 720, false);
    }
}