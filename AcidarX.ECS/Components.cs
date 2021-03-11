using System.Collections.Generic;
using System.Numerics;

namespace AcidarX.ECS
{
    public interface IComponent
    {
    }

    public readonly struct TransformComponent : IComponent
    {
        public Vector3 Translation { get; }
        public Vector3 Scale { get; }

        public TransformComponent(Vector3 translation, Vector3 scale)
        {
            Translation = translation;
            Scale = scale;
        }

        public TransformComponent(ref TransformComponent transformComponent)
        {
            Translation = transformComponent.Translation;
            Scale = transformComponent.Scale;
        }

        public override string ToString() => string.Format("[{0}]: {{ Translation: {1}, Scale: {2} }}",
            GetType().Name, Translation, Scale);
    }

    public readonly struct Transform3DComponent : IComponent
    {
        public Matrix4x4 Translation { get; }

        public Transform3DComponent(Matrix4x4 translation)
        {
            Translation = translation;
        }

        public override string ToString() => string.Format("[{0}]: {{ Translation: {1} }}",
            GetType().Name, Translation);
    }

    public readonly struct PositionComponent : IComponent
    {
        public float X { get; }
        public float Y { get; }
        public float Z { get; }

        public PositionComponent(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public PositionComponent(Vector3 fullPosition)
        {
            X = fullPosition.X;
            Y = fullPosition.Y;
            Z = fullPosition.Z;
        }

        public override string ToString() => string.Format("[{0}]: {{ X: {1}, Y: {2}, Z: {3} }}",
            GetType().Name, X, Y, Z);
    }

    public struct SpriteRendererComponent<TTexture> : IComponent
    {
        public Vector4 Color { get; }
        public List<TTexture> Textures { get; }
        public bool IsDirty { get; set; }

        public SpriteRendererComponent
            (ref SpriteRendererComponent<TTexture> spriteRendererComponent) =>
            (Color, Textures, IsDirty) = (spriteRendererComponent.Color, spriteRendererComponent.Textures, true);

        public SpriteRendererComponent(Vector4 color, List<TTexture> texture) => (
            Color, Textures, IsDirty) = (color, texture, true);

        public override string ToString() =>
            string.Format("[{0}]: {{ Color: {1}, Texture: {2} }}", GetType().Name, Color, Textures);
    }
}
