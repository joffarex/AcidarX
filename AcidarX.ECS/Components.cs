using System.Collections.Generic;
using System.Numerics;
using AcidarX.Graphics.Renderer;

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

        public override string ToString() => $"[{GetType().Name}]: {{ Translation: {Translation}, Scale: {Scale} }}";
    }

    public readonly struct PositionComponent : IComponent
    {
        public float X { get; init; }
        public float Y { get; init; }
        public float Z { get; init; }

        public override string ToString() => $"[{GetType().Name}]: {{ X: {X}, Y: {Y}, Z: {Z} }}";
    }

    public struct SpriteRendererComponent : IComponent
    {
        public Vector4 Color { get; }
        public List<Texture2D> Textures { get; }
        public bool IsDirty { get; set; }

        public SpriteRendererComponent
            (ref SpriteRendererComponent spriteRendererComponent) =>
            (Color, Textures, IsDirty) = (spriteRendererComponent.Color, spriteRendererComponent.Textures, true);

        public SpriteRendererComponent(Vector4 color, List<Texture2D> texture) => (
            Color, Textures, IsDirty) = (color, texture, true);

        public override string ToString() =>
            $"[{GetType().Name}]: {{ Color: {Color}, Texture: {Textures} }}";
    }
}