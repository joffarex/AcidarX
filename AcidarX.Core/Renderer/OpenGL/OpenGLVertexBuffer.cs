using System;
using System.Runtime.InteropServices;
using AcidarX.Core.Logging;
using AcidarX.Core.Profiling;
using Microsoft.Extensions.Logging;
using Silk.NET.OpenGL;

namespace AcidarX.Core.Renderer.OpenGL
{
    public sealed class OpenGLVertexBuffer<T> : VertexBuffer
        where T : unmanaged
    {
        private static readonly ILogger<OpenGLVertexBuffer<T>> Logger = AXLogger.CreateLogger<OpenGLVertexBuffer<T>>();
        private readonly GL _gl;
        private readonly RendererID _rendererID;
        private bool _isDisposed;
        private BufferLayout? _layout;

        public OpenGLVertexBuffer(GL gl, ReadOnlySpan<T> vertices)
        {
            _gl = gl;
            _rendererID = (RendererID) _gl.CreateBuffer();
            Bind();

            int size = Marshal.SizeOf<T>();

            _gl.BufferData(BufferTargetARB.ArrayBuffer, (nuint) (vertices.Length * size), vertices,
                GLEnum.StaticDraw);
        }

        public unsafe OpenGLVertexBuffer(GL gl, int size, uint count)
        {
            _gl = gl;
            _rendererID = (RendererID) _gl.CreateBuffer();
            Bind();

            _gl.BufferData(BufferTargetARB.ArrayBuffer, (nuint) (size * count), null, GLEnum.DynamicDraw);
        }

        public override void Dispose()
        {
            Logger.Assert(!_isDisposed, $"{this} is already disposed");

            _isDisposed = true;
            Dispose(true);
            GC.SuppressFinalize(this);
            GC.KeepAlive(this);
        }

        public override void Bind()
        {
            AXProfiler.Capture(
                () => { _gl.BindBuffer(BufferTargetARB.ArrayBuffer, _rendererID); });
        }

        public override void Unbind()
        {
            AXProfiler.Capture(() => { _gl.BindBuffer(BufferTargetARB.ArrayBuffer, 0); });
        }

        public override void SetLayout(BufferLayout layout)
        {
            _layout = layout;
        }

        public override void SetData<TData>(ReadOnlySpan<TData> data)
        {
            Bind();

            int size = Marshal.SizeOf<TData>();
            _gl.BufferSubData(BufferTargetARB.ArrayBuffer, 0, (nuint) (size * data.Length), data);
        }

        public override unsafe void SetData(void* data, uint size)
        {
            Bind();

            _gl.BufferSubData(BufferTargetARB.ArrayBuffer, 0, size, data);
        }

        public override BufferLayout? GetLayout() => _layout;

        protected override void Dispose(bool manual)
        {
            Logger.Assert(manual, $"Memory leak detected on object: {this}");

            AXProfiler.Capture(() => { _gl.DeleteBuffers(1, _rendererID); });
        }

        public override string ToString() => $"VertexBuffer|{_rendererID}";
    }
}