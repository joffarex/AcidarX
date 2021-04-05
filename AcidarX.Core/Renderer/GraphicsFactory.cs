using System;
using System.Runtime.InteropServices;
using AcidarX.Core.Layers;
using AcidarX.Core.Profiling;
using AcidarX.Core.Renderer.OpenGL;
using Silk.NET.Input;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;

namespace AcidarX.Core.Renderer
{
    public class GraphicsFactory
    {
        public GraphicsFactory(GL gl) => Gl = gl;

        public GL Gl { get; }

        public IndexBuffer CreateIndexBuffer<T>(T[] indices)
            where T : unmanaged
        {
            return AXRenderer.API switch
            {
                API.None => null,
                API.OpenGL => AXProfiler.Capture(
                    () => new OpenGLIndexBuffer<T>(Gl, new ReadOnlySpan<T>(indices))),
                _ => throw new Exception("Not supported API")
            };
        }

        public VertexBuffer CreateVertexBuffer<T>(T[] vertices)
            where T : unmanaged
        {
            return AXProfiler.Capture(() =>
            {
                return AXRenderer.API switch
                {
                    API.None => null,
                    API.OpenGL => AXProfiler.Capture(
                        () => new OpenGLVertexBuffer<T>(Gl, new ReadOnlySpan<T>(vertices))),
                    _ => throw new Exception("Not supported API")
                };
            });
        }
        
        public VertexBuffer CreateVertexBuffer<T>(uint count)
            where T : unmanaged
        {
            return AXProfiler.Capture(() =>
            {
                return AXRenderer.API switch
                {
                    API.None => null,
                    API.OpenGL => AXProfiler.Capture(
                        () => new OpenGLVertexBuffer<T>(Gl, Marshal.SizeOf<T>(), count)),
                    _ => throw new Exception("Not supported API")
                };
            });
        }

        public VertexArray CreateVertexArray()
        {
            return AXRenderer.API switch
            {
                API.None => null,
                API.OpenGL => AXProfiler.Capture(() => new OpenGLVertexArray(Gl)),
                _ => throw new Exception("Not supported API")
            };
        }

        public Shader CreateShader(string vertexSource, string fragmentSource)
        {
            return AXRenderer.API switch
            {
                API.None => null,
                API.OpenGL => AXProfiler.Capture(
                    () => new OpenGLShader(Gl, vertexSource, fragmentSource)),
                _ => throw new Exception("Not supported API")
            };
        }

        public Texture2D CreateTexture(string path)
        {
            return AXRenderer.API switch
            {
                API.None => null,
                API.OpenGL => AXProfiler.Capture(() => new OpenGLTexture2D(Gl, path)),
                _ => throw new Exception("Not supported API")
            };
        }

        public Texture2D CreateTexture(uint width, uint height)
        {
            return AXRenderer.API switch
            {
                API.None => null,
                API.OpenGL => AXProfiler.Capture(() => new OpenGLTexture2D(Gl, width, height)),
                _ => throw new Exception("Not supported API")
            };
        }

        public ImGuiLayer CreateImGuiLayer(IWindow window, IInputContext inputContext) => new(Gl, window, inputContext);
    }
}