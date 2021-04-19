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
        // public float RotationInRadians { get; set; } = 0.0f;
        public Vector2 Scale { get; set; }


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