using System;

namespace AcidarX.Core.Renderer
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