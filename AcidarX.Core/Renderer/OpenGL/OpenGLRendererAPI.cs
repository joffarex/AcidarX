using AcidarX.Core.Logging;
using Microsoft.Extensions.Logging;
using Silk.NET.Maths;
using Silk.NET.OpenGL;

namespace AcidarX.Core.Renderer.OpenGL
{
    public class OpenGLRendererAPI : RendererAPI
    {
        private static readonly ILogger<OpenGLRendererAPI> Logger = AXLogger.CreateLogger<OpenGLRendererAPI>();
        private readonly GL _gl;

        public OpenGLRendererAPI(GL gl) => _gl = gl;

        public override void SetClearColor(Vector4D<float> color)
        {
            _gl.ClearColor(color);
        }

        public override void Clear()
        {
            _gl.Clear((uint) (ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit));
        }

        public override void UseShader(Shader shader)
        {
            shader.Bind();
        }

        public override unsafe void DrawIndexed(VertexArray vertexArray)
        {
            vertexArray.Bind();
            _gl.DrawElements(PrimitiveType.Triangles, vertexArray.GetIndexBuffer().GetCount(),
                DrawElementsType.UnsignedInt, null);
        }
    }
}