using System;

namespace AcidarX.Graphics.Renderer
{
    public abstract class IndexBuffer : IDisposable
    {
        public abstract void Dispose();
        public abstract void Bind();
        public abstract void Unbind();
        public abstract uint GetCount();

        protected abstract void Dispose(bool manual);

        ~IndexBuffer()
        {
            Dispose(false);
        }
    }
}