using System;

namespace AcidarX.Core.Renderer
{
    public enum TextureSlot
    {
        Texture0 = 0,
        Texture1 = 1,
        Texture2 = 2,
        Texture3 = 3,
        Texture4 = 4,
        Texture5 = 5,
        Texture6 = 6,
        Texture7 = 7,
        Texture8 = 8,
        Texture9 = 9,
        Texture10 = 10,
        Texture11 = 11,
        Texture12 = 12,
        Texture13 = 13,
        Texture14 = 14,
        Texture15 = 15,
        Texture16 = 16,
        Texture17 = 17,
        Texture18 = 18,
        Texture19 = 19,
        Texture20 = 20,
        Texture21 = 21,
        Texture22 = 22,
        Texture23 = 23,
        Texture24 = 24,
        Texture25 = 25,
        Texture26 = 26,
        Texture27 = 27,
        Texture28 = 28,
        Texture29 = 29,
        Texture30 = 30,
        Texture31 = 31
    }


    public abstract class Texture : IDisposable
    {
        public abstract void Dispose();
        public abstract void Bind();
        public abstract void Unbind();
        public abstract void Use(TextureSlot slot);
        public abstract unsafe void SetData(void* data, uint size);


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