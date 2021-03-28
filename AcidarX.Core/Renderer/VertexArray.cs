using System;
using System.Collections.Generic;

namespace AcidarX.Core.Renderer
{
    public abstract class VertexArray : IDisposable
    {
        public abstract void Dispose();
        public abstract void Bind();
        public abstract void Unbind();
        public abstract void AddVertexBuffer(VertexBuffer vertexBuffer);
        public abstract void SetIndexBuffer(IndexBuffer indexBuffer);
        public abstract IndexBuffer GetIndexBuffer();
        public abstract List<VertexBuffer> GetVertexBuffers();

        protected abstract void Dispose(bool manual);

        ~VertexArray()
        {
            Dispose(false);
        }
    }
}