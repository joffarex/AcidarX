using Silk.NET.Maths;

namespace AcidarX.Core.Renderer
{
    public enum API
    {
        None,
        OpenGL
    }

    public abstract class RendererAPI
    {
        public static API API { get; set; } = API.OpenGL;

        public abstract void SetClearColor(Vector4D<float> color);
        public abstract void Clear();

        public abstract void UseShader(Shader shader);
        public abstract void DrawIndexed(VertexArray vertexArray);
    }
}