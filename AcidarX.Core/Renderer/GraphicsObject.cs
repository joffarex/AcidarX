using System;
using AcidarX.Core.Renderer.OpenGL;

namespace AcidarX.Core.Renderer
{
    public static class GraphicsObject
    {
        public static IndexBuffer CreateIndexBuffer<T>(T[] indices)
            where T : unmanaged
        {
            return Renderer.API switch
            {
                RendererAPI.None => null,
                RendererAPI.OpenGL => new OpenGLIndexBuffer<T>(new ReadOnlySpan<T>(indices)),
                _ => throw new Exception("Not supported API")
            };
        }

        public static VertexBuffer CreateVertexBuffer<T>(T[] vertices)
            where T : unmanaged
        {
            return Renderer.API switch
            {
                RendererAPI.None => null,
                RendererAPI.OpenGL => new OpenGLVertexBuffer<T>(new ReadOnlySpan<T>(vertices)),
                _ => throw new Exception("Not supported API")
            };
        }

        public static VertexArray CreateVertexArray()
        {
            return Renderer.API switch
            {
                RendererAPI.None => null,
                RendererAPI.OpenGL => new OpenGLVertexArray(),
                _ => throw new Exception("Not supported API")
            };
        }
    }
}