using System.Collections.Generic;
using System.Numerics;
using Silk.NET.Maths;

namespace AcidarX.Graphics.Renderer
{
    // TODO: call all the commands in `_renderQueue` onto separate thread
    public sealed class RenderCommandDispatcher
    {
        private readonly RendererAPI _rendererAPI;

        public RenderCommandDispatcher(RendererAPI rendererAPI) => _rendererAPI = rendererAPI;

        public void DrawIndexed(VertexArray vertexArray, uint indexCount = 0)
        {
            _rendererAPI.DrawIndexed(vertexArray, indexCount);
        }

        public void UnbindTexture2D(Texture2D texture2D)
        {
            _rendererAPI.UnbindTexture2D(texture2D);
        }

        public void Init()
        {
            _rendererAPI.Init();
        }

        public void OnWindowResize(Vector2D<int> size)
        {
            _rendererAPI.OnWindowResize(size);
        }

        public void UseShader(Shader shader)
        {
            _rendererAPI.UseShader(shader);
        }

        public void UseShader(Shader shader, List<ShaderInputData> uniforms)
        {
            _rendererAPI.UseShader(shader, uniforms);
        }

        public unsafe void SetVertexBufferData(VertexBuffer vertexBuffer, void* ptr, uint count)
        {
            _rendererAPI.SetVertexBufferData(vertexBuffer, ptr, count);
        }

        public void SetClearColor(Vector4D<float> color)
        {
            _rendererAPI.SetClearColor(color);
        }

        public void UseTexture2D(TextureSlot slot, Texture2D texture2D)
        {
            _rendererAPI.UseTexture2D(slot, texture2D);
        }

        public void Clear()
        {
            _rendererAPI.Clear();
        }

        public void ClearFramebuffer(Framebuffer framebuffer, Vector4 color)
        {
            _rendererAPI.ClearFramebuffer(framebuffer, color);
        }
    }
}