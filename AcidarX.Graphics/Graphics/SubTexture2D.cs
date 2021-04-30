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
            SpriteSize = spriteSize;
            SpriteBlockSize = spriteBlockSize;

            UpdateSpriteCoordinates(coordinates);
        }

        public SubTexture2D
            (Texture2D texture2D, Vector2 coordinates, SizeF spriteSize) : this(texture2D, coordinates, spriteSize,
            Vector2.One)
        {
        }

        public Texture2D Texture2D { get; }
        public SizeF SpriteSize { get; }
        public Vector2 SpriteBlockSize { get; }

        public Vector2[] TextureCoordinates { get; } = new Vector2[4];

        public void UpdateSpriteCoordinates(Vector2 coordinates, bool mirrored = false)
        {
            if (mirrored)
            {
                TextureCoordinates[1] = new Vector2(coordinates.X * SpriteSize.Width / Texture2D.GetWidth(),
                    coordinates.Y * SpriteSize.Height / Texture2D.GetHeight());
                TextureCoordinates[0] = new Vector2(
                    (coordinates.X + SpriteBlockSize.X) * SpriteSize.Width / Texture2D.GetWidth(),
                    coordinates.Y * SpriteSize.Height / Texture2D.GetHeight());
                TextureCoordinates[3] = new Vector2(
                    (coordinates.X + SpriteBlockSize.X) * SpriteSize.Width / Texture2D.GetWidth(),
                    (coordinates.Y + SpriteBlockSize.Y) * SpriteSize.Height / Texture2D.GetHeight());
                TextureCoordinates[2] = new Vector2(coordinates.X * SpriteSize.Width / Texture2D.GetWidth(),
                    (coordinates.Y + SpriteBlockSize.Y) * SpriteSize.Height / Texture2D.GetHeight());
            }
            else
            {
                TextureCoordinates[0] = new Vector2(coordinates.X * SpriteSize.Width / Texture2D.GetWidth(),
                    coordinates.Y * SpriteSize.Height / Texture2D.GetHeight());
                TextureCoordinates[1] = new Vector2(
                    (coordinates.X + SpriteBlockSize.X) * SpriteSize.Width / Texture2D.GetWidth(),
                    coordinates.Y * SpriteSize.Height / Texture2D.GetHeight());
                TextureCoordinates[2] = new Vector2(
                    (coordinates.X + SpriteBlockSize.X) * SpriteSize.Width / Texture2D.GetWidth(),
                    (coordinates.Y + SpriteBlockSize.Y) * SpriteSize.Height / Texture2D.GetHeight());
                TextureCoordinates[3] = new Vector2(coordinates.X * SpriteSize.Width / Texture2D.GetWidth(),
                    (coordinates.Y + SpriteBlockSize.Y) * SpriteSize.Height / Texture2D.GetHeight());
            }
        }

        public Vector2 GetBaseScaleFromSpriteSize() => SpriteSize.Width > SpriteSize.Height
            ? new Vector2(SpriteSize.Width / SpriteSize.Height, 1.0f)
            : new Vector2(1.0f, SpriteSize.Height / SpriteSize.Width);
    }
}