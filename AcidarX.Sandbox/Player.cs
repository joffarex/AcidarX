using System.Drawing;
using System.Numerics;
using AcidarX.Graphics;
using AcidarX.Graphics.Graphics;
using AcidarX.Graphics.Scene;
using AcidarX.Kernel.Input;

namespace AcidarX.Sandbox
{
    public sealed class Player : AnimatedSprite
    {
        private static readonly SizeF SpriteSize = new(50.0f, 37.0f);
        private static readonly Vector2 SpriteBlockSize = Vector2.One;
        private bool _mirrored;

        public Player(AssetManager assetManager)
            : base(assetManager.GetTexture2D("assets/Textures/adventurer-spritesheet.png"),
                SpriteSize, SpriteBlockSize)
        {
            AddAnimation("idle", new Vector2[]
            {
                new(0, 15), new(1, 15), new(2, 15), new(3, 15)
            });
            AddAnimation("run", new Vector2[]
            {
                new(1, 14), new(2, 14), new(3, 14), new(4, 14), new(5, 14), new(6, 14)
            });

            Transform = new TransformComponent {Translation = new Vector3(Vector2.Zero, 1.0f), Scale = Vector2.One};
            SpriteRenderer = new SpriteRendererComponent {SubTexture = SubTexture2D};
        }

        public override void OnUpdate(double deltaTime)
        {
            if (KeyboardState.IsKeyPressed(AXKey.D))
            {
                _mirrored = false;
                PlayAnimation(new AnimationOptions
                {
                    Name = "run", DeltaTime = deltaTime, FrameDisplayOffset = 0.1, Mirrored = _mirrored
                });
                Transform.UpdateTranslationX(Transform.Translation.X + (float) deltaTime * 3);
            }
            else if (KeyboardState.IsKeyPressed(AXKey.A))
            {
                _mirrored = true;
                PlayAnimation(new AnimationOptions
                {
                    Name = "run", DeltaTime = deltaTime, FrameDisplayOffset = 0.1, Mirrored = _mirrored
                });
                Transform.UpdateTranslationX(Transform.Translation.X - (float) deltaTime * 3);
            }
            else
            {
                PlayAnimation(new AnimationOptions
                {
                    Name = "idle", DeltaTime = deltaTime, FrameDisplayOffset = 0.1, Mirrored = _mirrored
                });
            }
        }
    }
}