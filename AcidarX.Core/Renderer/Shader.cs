using System;

namespace AcidarX.Core.Renderer
{
    public struct ShaderInputData
    {
        public string Name;
        public ShaderDataType Type;
        public object Data;
    }

    public abstract class Shader : IDisposable
    {
        public abstract void Dispose();
        public abstract void Bind();
        public abstract void Unbind();

        protected abstract void Dispose(bool manual);

        ~Shader()
        {
            Dispose(false);
        }
    }
}