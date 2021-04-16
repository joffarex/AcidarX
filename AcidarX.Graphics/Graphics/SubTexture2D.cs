using System.Drawing;
using System.Numerics;
using AcidarX.Graphics.Renderer;

namespace AcidarX.Graphics.Graphics
{
    public class SubTexture2D
    {
        /// <summary>
        ///     Creates an object which holds only a "sub texture" of sprite sheet
        /// </summary>
        /// <param name="texture2D">Texture to get "sub texture" from</param>
        /// <param name="coordinates">Coordinates on what point "sub texture" starts in scale of sprite size</param>
        /// <param name="spriteSize">Size of "sub texture" in pixels</param>
        /// <param name="spriteBlockSize">Size of "sub texture" in blocks</param>
        public SubTexture2D(Texture2D texture2D, Vector2 coordinates, SizeF spriteSize, Vector2 spriteBlockSize)
        {
            Texture2D = texture2D;

            TextureCoordinates[0] = new Vector2(coordinates.X * spriteSize.Width / Texture2D.GetWidth(),
                coordinates.Y * spriteSize.Height / Texture2D.GetHeight());
            TextureCoordinates[1] = new Vector2(
                (coordinates.X + spriteBlockSize.X) * spriteSize.Width / Texture2D.GetWidth(),
                coordinates.Y * spriteSize.Height / Texture2D.GetHeight());
            TextureCoordinates[2] = new Vector2(
                (coordinates.X + spriteBlockSize.X) * spriteSize.Width / Texture2D.GetWidth(),
                (coordinates.Y + spriteBlockSize.Y) * spriteSize.Height / Texture2D.GetHeight());
            TextureCoordinates[3] = new Vector2(coordinates.X * spriteSize.Width / Texture2D.GetWidth(),
                (coordinates.Y + spriteBlockSize.Y) * spriteSize.Height / Texture2D.GetHeight());
        }

        public SubTexture2D
            (Texture2D texture2D, Vector2 coordinates, SizeF spriteSize) : this(texture2D, coordinates, spriteSize,
            Vector2.One)
        {
        }

        public Texture2D Texture2D { get; }

        public Vector2[] TextureCoordinates { get; } = new Vector2[4];
    }
}