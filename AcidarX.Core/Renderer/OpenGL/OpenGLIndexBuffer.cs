using System;
using System.Runtime.InteropServices;
using Microsoft.Extensions.Logging;
using Silk.NET.OpenGL;
using static AcidarX.Core.Renderer.OpenGL.OpenGLGraphicsContext;

namespace AcidarX.Core.Renderer.OpenGL
{
    public sealed class OpenGLIndexBuffer<T> : IndexBuffer
        where T : unmanaged

    {
        private static readonly ILogger<OpenGLIndexBuffer<T>> Logger = AXLogger.CreateLogger<OpenGLIndexBuffer<T>>();
        private readonly uint _rendererID;
        private readonly uint _count;
        private bool _isDisposed;

        public OpenGLIndexBuffer(ReadOnlySpan<T> indices)
        {
            _count = (uint) indices.Length;
            Gl.CreateBuffers(1, out _rendererID);
            Bind();

            int size = Marshal.SizeOf<T>();

            Gl.BufferData(BufferTargetARB.ElementArrayBuffer, (nuint) (_count * size), indices,
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
            Gl.BindBuffer(BufferTargetARB.ElementArrayBuffer, _rendererID);
        }

        public override void Unbind()
        {
            Gl.BindBuffer(BufferTargetARB.ElementArrayBuffer, 0);
        }

        public override uint GetCount() => _count;

        protected override void Dispose(bool manual)
        {
            Logger.Assert(manual, $"Memory leak detected on object: {this}");

            Gl.DeleteBuffers(1, _rendererID);
        }

        public override string ToString() => string.Format("IndexBuffer|{0}", _rendererID);
    }
}