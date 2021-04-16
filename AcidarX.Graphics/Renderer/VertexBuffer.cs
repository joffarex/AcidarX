using System;

namespace AcidarX.Graphics.Renderer
{
    public abstract class VertexBuffer : IDisposable
    {
        public abstract void Dispose();
        public abstract void Bind();
        public abstract void Unbind();
        public abstract void SetLayout(BufferLayout layout);
        public abstract unsafe void SetData(void* data, uint count);
        public abstract BufferLayout? GetLayout();

        protected abstract void Dispose(bool manual);

        ~VertexBuffer()
        {
            Dispose(false);
        }
    }
}