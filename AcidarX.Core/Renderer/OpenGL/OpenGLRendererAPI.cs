using Microsoft.Extensions.Logging;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using static AcidarX.Core.Renderer.OpenGL.OpenGLGraphicsContext;

namespace AcidarX.Core.Renderer.OpenGL
{
    public class OpenGLRendererAPI : RendererAPI
    {
        private static readonly ILogger<OpenGLRendererAPI> Logger = AXLogger.CreateLogger<OpenGLRendererAPI>();

        public OpenGLRendererAPI()
        {
            Logger.Assert(Gl != null, "OpenGL context has not been initialized");
        }

        public override void SetClearColor(Vector4D<float> color)
        {
            Gl.ClearColor(color);
        }

        public override void Clear()
        {
            Gl.Clear((uint) (ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit));
        }

        public override unsafe void DrawIndexed(VertexArray vertexArray)
        {
            Gl.DrawElements(PrimitiveType.Triangles, vertexArray.GetIndexBuffer().GetCount(),
                DrawElementsType.UnsignedInt, null);
        }
    }
}