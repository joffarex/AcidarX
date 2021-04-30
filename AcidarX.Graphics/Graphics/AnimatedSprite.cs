using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using AcidarX.Graphics.Renderer;
using AcidarX.Graphics.Scene;
using AcidarX.Kernel.Logging;
using Microsoft.Extensions.Logging;

namespace AcidarX.Graphics.Graphics
{
    public struct AnimationOptions
    {
        public string Name { get; init; }
        public double DeltaTime { get; init; }
        public double FrameDisplayOffset { get; init; }
        public bool Mirrored { get; init; }
    }

    public abstract class AnimatedSprite
    {
        private static readonly ILogger<AnimatedSprite> Logger = AXLogger.CreateLogger<AnimatedSprite>();
        private readonly Dictionary<string, Vector2[]> _animations = new();

        protected readonly SubTexture2D SubTexture2D;
        private uint _currentFrameIndex;

        private double _timeBetweenFrameDisplay;

        protected AnimatedSprite
            (Texture2D texture2D, SizeF spriteSize, Vector2 spriteBlockSize) => SubTexture2D =
            new SubTexture2D(texture2D, Vector2.Zero, spriteSize, spriteBlockSize);

        public TransformComponent Transform { get; protected init; } = null!;
        public SpriteRendererComponent SpriteRenderer { get; protected init; } = null!;

        protected void AddAnimation(string name, Vector2[] frames) => _animations.Add(name, frames);

        protected void PlayAnimation(AnimationOptions animationOptions)
        {
            Vector2[]? animation = _animations[animationOptions.Name];

            _timeBetweenFrameDisplay -= animationOptions.DeltaTime;

            if (_timeBetweenFrameDisplay <= 0)
            {
                _timeBetweenFrameDisplay = animationOptions.FrameDisplayOffset;

                if (_currentFrameIndex >= animation.Length)
                {
                    _currentFrameIndex = 0;
                }

                SubTexture2D.UpdateSpriteCoordinates(animation[_currentFrameIndex++], animationOptions.Mirrored);
            }
        }

        public abstract void OnUpdate(double deltaTime);
    }
}