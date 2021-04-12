using System.Collections.Generic;
using System.Numerics;
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
        public abstract void Init();
        public abstract void OnWindowResize(Vector2D<int> size);

        public abstract void UseShader(Shader shader);
        public abstract void UseShader(Shader shader, IEnumerable<ShaderInputData> uniforms);
        public abstract void UseTexture2D(TextureSlot slot, Texture2D texture2D);
        public abstract void DrawIndexed(VertexArray vertexArray, uint indexCount = 0);
        public abstract unsafe void SetVertexBufferData(VertexBuffer vertexBuffer, void* ptr, uint count);
        public abstract void UnbindTexture2D(Texture2D texture2D);

        public abstract void ClearFramebuffer(Framebuffer framebuffer, Vector4 color);
    }
}