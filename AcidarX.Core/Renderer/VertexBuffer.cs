using System;

namespace AcidarX.Core.Renderer
{
    public abstract class VertexBuffer : IDisposable
    {
        public abstract void Dispose();
        public abstract void Bind();
        public abstract void Unbind();
        public abstract void SetLayout(BufferLayout layout);
        public abstract void SetData<TData>(ReadOnlySpan<TData> data) where TData : unmanaged;
        public abstract unsafe void SetData(void* data, uint count);
        public abstract BufferLayout? GetLayout();

        protected abstract void Dispose(bool manual);

        ~VertexBuffer()
        {
            Dispose(false);
        }
    }
}