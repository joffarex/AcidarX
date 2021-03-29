using System.Collections.Generic;
using System.Linq;
using System.Numerics;
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

        public override void UseShader(Shader shader, IEnumerable<UniformPublicInfo> uniforms)
        {
            shader.Bind();

            // TODO: check performance impact
            foreach (UniformPublicInfo uniformPublicInfo in uniforms)
            {
                Logger.Assert(shader.UniformFieldInfos.Any(x => x.Type == uniformPublicInfo.Type),
                    $"Shader does not contain uniform with type: {uniformPublicInfo.Type}");

                switch (uniformPublicInfo.Type)
                {
                    case UniformType.FloatMat4:
                        shader.UploadMatrix4(uniformPublicInfo.Name, (Matrix4x4) uniformPublicInfo.Data);
                        break;
                    default:
                        Logger.Assert(false, "Unknown Uniform Type");
                        break;
                }
            }
        }

        public override unsafe void DrawIndexed(VertexArray vertexArray)
        {
            vertexArray.Bind();
            _gl.DrawElements(PrimitiveType.Triangles, vertexArray.GetIndexBuffer().GetCount(),
                DrawElementsType.UnsignedInt, null);
        }
    }
}