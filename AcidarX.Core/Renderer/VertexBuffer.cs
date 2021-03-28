using System;

namespace AcidarX.Core.Renderer
{
    public abstract class VertexBuffer : IDisposable
    {
        public abstract void Dispose();
        public abstract void Bind();
        public abstract void Unbind();
        public abstract void SetLayout(BufferLayout layout);
        public abstract BufferLayout? GetLayout();

        protected abstract void Dispose(bool manual);

        ~VertexBuffer()
        {
            Dispose(false);
        }
    }
}