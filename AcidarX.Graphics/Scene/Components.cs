using System.Numerics;
using AcidarX.ECS;
using AcidarX.Graphics.Graphics;
using AcidarX.Graphics.Renderer;

namespace AcidarX.Graphics.Scene
{
    public class TransformComponent : IComponent
    {
        public Vector3 Translation { get; set; }

        public Matrix4x4 Rotation { get; set; } = Matrix4x4.Identity;

        /// <summary>
        ///     Actual scale of created sprite.
        ///     if sprite's dimensions are 50x37px, actual texture drawn would be that size * Scale
        /// </summary>
        public Vector2 Scale { get; set; }

        public void UpdateTranslationX(float x) => Translation = new Vector3(x, Translation.Y, Translation.Z);
        public void UpdateTranslationY(float y) => Translation = new Vector3(Translation.X, y, Translation.Z);
        public void UpdateTranslationZ(float z) => Translation = new Vector3(Translation.X, Translation.Y, z);


        public override string ToString() => $"[{GetType().Name}]: {{ Translation: {Translation}, Scale: {Scale} }}";
    }

    public class SpriteRendererComponent : IComponent
    {
        public Vector4 Color { get; set; } = Vector4.One;
        public Texture2D Texture { get; set; }
        public SubTexture2D SubTexture { get; set; }
        public float TilingFactor { get; set; } = 1.0f;


        public override string ToString() =>
            $"[{GetType().Name}]: {{ Color: {Color}, Texture: {Texture}, SubTexture: {SubTexture}, TilingFactor: {TilingFactor} }}";
    }
}