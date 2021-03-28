using System;

namespace AcidarX.Core.Renderer
{
    public abstract class VertexBuffer : IDisposable
    {
        public abstract void Dispose();
        public abstract void Bind();
        public abstract void Unbind();


        protected abstract void Dispose(bool manual);

        ~VertexBuffer()
        {
            Dispose(false);
        }
    }
}