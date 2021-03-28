namespace AcidarX.Core.Renderer
{
    public enum RendererAPI
    {
        None,
        OpenGL
    }

    public class Renderer
    {
        public static RendererAPI API { get; set; } = RendererAPI.OpenGL;
    }
}