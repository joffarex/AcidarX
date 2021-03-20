namespace AcidarX.Core.Windowing
{
    public readonly struct AXWindowOptions
    {
        public string Title { get; }
        public int Width { get; }
        public int Height { get; }
        public bool VSync { get; }

        public AXWindowOptions(string title, int width, int height, bool vSync)
        {
            Title = title;
            Width = width;
            Height = height;
            VSync = vSync;
        }

        public static AXWindowOptions CreateDefault() => new("AcidarX", 1280, 820, true);
    }
}