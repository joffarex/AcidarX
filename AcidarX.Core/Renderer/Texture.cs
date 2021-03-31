using System;

namespace AcidarX.Core.Renderer
{
    public enum TextureSlot
    {
        Texture0,
        Texture1,
        Texture2,
        Texture3,
        Texture4,
        Texture5,
        Texture6,
        Texture7,
        Texture8,
        Texture9,
        Texture10,
        Texture11,
        Texture12,
        Texture13,
        Texture14,
        Texture15,
        Texture16,
        Texture17,
        Texture18,
        Texture19,
        Texture20,
        Texture21,
        Texture22,
        Texture23,
        Texture24,
        Texture25,
        Texture26,
        Texture27,
        Texture28,
        Texture29,
        Texture30,
        Texture31
    }


    public abstract class Texture : IDisposable
    {
        public abstract void Dispose();
        public abstract void Bind();
        public abstract void Unbind();
        public abstract void Use(TextureSlot slot);
        public abstract unsafe void SetData(void* data);


        public abstract uint GetWidth();
        public abstract uint GetHeight();
        public abstract string GetPath();

        protected abstract void Dispose(bool manual);

        ~Texture()
        {
            Dispose(false);
        }
    }

    public abstract class Texture2D : Texture
    {
    }
}