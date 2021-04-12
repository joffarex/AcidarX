using System.Collections.Generic;
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

        public override void Init()
        {
            _gl.Enable(EnableCap.Blend);
            _gl.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
        }

        public override void OnWindowResize(Vector2D<int> size)
        {
            _gl.Viewport(0, 0, (uint) size.X, (uint) size.Y);
        }

        public override void Clear()
        {
            // Need these here as ImGui layer disables them. needs refactoring
            _gl.Enable(EnableCap.DepthTest);
            _gl.Enable(EnableCap.Blend);
            _gl.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            _gl.Enable(EnableCap.CullFace);

            _gl.Clear((uint) (ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit));
        }

        public override void ClearFramebuffer(Framebuffer framebuffer, Vector4 color)
        {
            _gl.Enable(EnableCap.DepthTest);
            _gl.Enable(EnableCap.Blend);
            _gl.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            _gl.Enable(EnableCap.CullFace);

            _gl.ClearTexImage(framebuffer.GetColorAttachmentRendererID(), 0, PixelFormat.Rgba, PixelType.Float, color);
            _gl.Clear((uint) ClearBufferMask.DepthBufferBit);
        }

        public override void UseShader(Shader shader)
        {
            shader.Bind();
        }

        public override unsafe void SetVertexBufferData(VertexBuffer vertexBuffer, void* ptr, uint count)
        {
            vertexBuffer.SetData(ptr, count);
        }

        public override void UseShader(Shader shader, IEnumerable<ShaderInputData> uniforms)
        {
            var openGLShader = (OpenGLShader) shader;
            openGLShader.Bind();

            // TODO: check performance impact
            foreach (ShaderInputData uniformPublicInfo in uniforms)
            {
                switch (uniformPublicInfo.Type)
                {
                    case ShaderDataType.Mat4:
                        openGLShader.UploadMatrix4(uniformPublicInfo.Name, (Matrix4x4) uniformPublicInfo.Data);
                        break;
                    case ShaderDataType.Float4:
                        openGLShader.UploadFloat4(uniformPublicInfo.Name, (Vector4) uniformPublicInfo.Data);
                        break;
                    case ShaderDataType.Float3:
                        openGLShader.UploadFloat3(uniformPublicInfo.Name, (Vector3) uniformPublicInfo.Data);
                        break;
                    case ShaderDataType.Float:
                        openGLShader.UploadFloat(uniformPublicInfo.Name, (float) uniformPublicInfo.Data);
                        break;
                    case ShaderDataType.IntSamplerArr:
                        openGLShader.UploadIntArray(uniformPublicInfo.Name, (int[]) uniformPublicInfo.Data);
                        break;
                    default:
                        Logger.Assert(false, "Unknown Uniform Type");
                        break;
                }
            }
        }

        public override void UseTexture2D(TextureSlot slot, Texture2D texture2D)
        {
            texture2D.Use(slot);
        }

        public override unsafe void DrawIndexed(VertexArray vertexArray, uint indexCount = 0)
        {
            vertexArray.Bind();
            _gl.DrawElements(PrimitiveType.Triangles,
                indexCount == 0 ? vertexArray.GetIndexBuffer().GetCount() : indexCount,
                DrawElementsType.UnsignedInt, null);
        }

        public override void UnbindTexture2D(Texture2D texture2D)
        {
            texture2D.Unbind();
        }
    }
}