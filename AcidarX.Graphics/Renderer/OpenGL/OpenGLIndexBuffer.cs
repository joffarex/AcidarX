using System;
using System.Runtime.InteropServices;
using AcidarX.Kernel.Logging;
using AcidarX.Kernel.Profiling;
using Microsoft.Extensions.Logging;
using Silk.NET.OpenGL;

namespace AcidarX.Graphics.Renderer.OpenGL
{
    public sealed class OpenGLIndexBuffer<T> : IndexBuffer
        where T : unmanaged

    {
        private static readonly ILogger<OpenGLIndexBuffer<T>> Logger = AXLogger.CreateLogger<OpenGLIndexBuffer<T>>();
        private readonly uint _count;
        private readonly GL _gl;
        private readonly RendererID _rendererID;
        private bool _isDisposed;

        public OpenGLIndexBuffer(GL gl, ReadOnlySpan<T> indices)
        {
            _gl = gl;
            _count = (uint) indices.Length;

            _rendererID = (RendererID) _gl.CreateBuffer();
            Bind();
            int size = Marshal.SizeOf<T>();

            _gl.BufferData(BufferTargetARB.ElementArrayBuffer, (nuint) (_count * size), indices,
                GLEnum.StaticDraw);
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
            AXProfiler.Capture(() => { _gl.BindBuffer(BufferTargetARB.ElementArrayBuffer, _rendererID); });
        }

        public override void Unbind()
        {
            AXProfiler.Capture(() => { _gl.BindBuffer(BufferTargetARB.ElementArrayBuffer, 0); });
        }

        public override uint GetCount() => _count;

        protected override void Dispose(bool manual)
        {
            Logger.Assert(manual, $"Memory leak detected on object: {this}");

            AXProfiler.Capture(() => { _gl.DeleteBuffers(1, _rendererID); });
        }

        public override string ToString() => $"IndexBuffer|{_rendererID}";
    }
}