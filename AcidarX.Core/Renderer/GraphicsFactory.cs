using System;
using AcidarX.Core.Renderer.OpenGL;

namespace AcidarX.Core.Renderer
{
    public static class GraphicsFactory
    {
        public static AXRenderer CreateRenderer()
        {
            return AXRenderer.API switch
            {
                API.None => null,
                API.OpenGL => new AXRenderer(new RenderCommandDispatcher(new OpenGLRendererAPI())),
                _ => throw new Exception("Not supported API")
            };
        }

        public static IndexBuffer CreateIndexBuffer<T>(T[] indices)
            where T : unmanaged
        {
            return AXRenderer.API switch
            {
                API.None => null,
                API.OpenGL => new OpenGLIndexBuffer<T>(new ReadOnlySpan<T>(indices)),
                _ => throw new Exception("Not supported API")
            };
        }

        public static VertexBuffer CreateVertexBuffer<T>(T[] vertices)
            where T : unmanaged
        {
            return AXRenderer.API switch
            {
                API.None => null,
                API.OpenGL => new OpenGLVertexBuffer<T>(new ReadOnlySpan<T>(vertices)),
                _ => throw new Exception("Not supported API")
            };
        }

        public static VertexArray CreateVertexArray()
        {
            return AXRenderer.API switch
            {
                API.None => null,
                API.OpenGL => new OpenGLVertexArray(),
                _ => throw new Exception("Not supported API")
            };
        }
    }
}