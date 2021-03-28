using System;
using AcidarX.Core.Renderer.OpenGL;
using Silk.NET.OpenGL;

namespace AcidarX.Core.Renderer
{
    public class GraphicsFactory
    {
        private readonly RenderCommandDispatcher _renderCommandDispatcher;

        public GraphicsFactory(RenderCommandDispatcher renderCommandDispatcher, GL gl)
        {
            _renderCommandDispatcher = renderCommandDispatcher;
            Gl = gl;
        }

        public GL Gl { get; }


        public AXRenderer CreateRenderer()
        {
            return AXRenderer.API switch
            {
                API.None => null,
                API.OpenGL => new AXRenderer(_renderCommandDispatcher),
                _ => throw new Exception("Not supported API")
            };
        }

        public IndexBuffer CreateIndexBuffer<T>(T[] indices)
            where T : unmanaged
        {
            return AXRenderer.API switch
            {
                API.None => null,
                API.OpenGL => new OpenGLIndexBuffer<T>(Gl, new ReadOnlySpan<T>(indices)),
                _ => throw new Exception("Not supported API")
            };
        }

        public VertexBuffer CreateVertexBuffer<T>(T[] vertices)
            where T : unmanaged
        {
            return AXRenderer.API switch
            {
                API.None => null,
                API.OpenGL => new OpenGLVertexBuffer<T>(Gl, new ReadOnlySpan<T>(vertices)),
                _ => throw new Exception("Not supported API")
            };
        }

        public VertexArray CreateVertexArray()
        {
            return AXRenderer.API switch
            {
                API.None => null,
                API.OpenGL => new OpenGLVertexArray(Gl),
                _ => throw new Exception("Not supported API")
            };
        }
    }
}