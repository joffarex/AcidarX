using System.Numerics;
using AcidarX.Core.Layers;
using AcidarX.Graphics;
using AcidarX.Graphics.Camera;
using AcidarX.Graphics.Renderer;
using AcidarX.Graphics.Scene;
using AcidarX.Kernel.Events;
using AcidarX.Kernel.Logging;
using AcidarX.Kernel.Utils;
using ImGuiNET;
using Microsoft.Extensions.Logging;
using Silk.NET.Maths;

namespace AcidarX.Sandbox
{
    public sealed class Sandbox2DLayer : Layer
    {
        private static readonly ILogger<Sandbox2DLayer> Logger = AXLogger.CreateLogger<Sandbox2DLayer>();

        private readonly AssetManager _assetManager;

        private readonly AXRenderer2D _renderer2D;

        private Player _player;

        private Scene _scene;

        public Sandbox2DLayer(AXRenderer2D renderer2D, AssetManager assetManager)
            : base("Sandbox 2D layer")
        {
            _renderer2D = renderer2D;
            _assetManager = assetManager;
        }

        public override void OnAttach()
        {
        }

        public override void OnLoad()
        {
            _renderer2D.Init();

            _scene = new Scene(_renderer2D, new OrthographicCameraController(16.0f / 9.0f));
            _scene.AddComponentType<TransformComponent>();
            _scene.AddComponentType<SpriteRendererComponent>();

            _player = new Player(_assetManager);
            _scene.AddSprite(_player.Transform, _player.SpriteRenderer);
        }

        public override void OnDetach()
        {
        }

        public override void OnImGuiRender(AppRenderEvent e)
        {
            // AXStatistics.ImGuiWindow();
            FpsUtils.ImGuiWindow(e.DeltaTime);
            ImGui.ShowDemoWindow();
        }

        public override void OnUpdate(double deltaTime)
        {
            _player.OnUpdate(deltaTime);
            _scene.OnUpdate(deltaTime);
        }

        public override void OnRender(double deltaTime)
        {
            // Transparency is weird with depth buffer. We need to first draw non-transparent objects and then draw transparent ones
            // But even for them, render order MATTERS! also we'll need to order Z Index as well.
            // Related link: https://research.ncl.ac.uk/game/mastersdegree/graphicsforgames/transparencyanddepth/Tutorial%204%20-%20Transparency%20and%20Depth.pdf

            _renderer2D.SetClearColor(new Vector4D<float>(24.0f, 24.0f, 24.0f, 1.0f));
            _renderer2D.Clear();

            AXStatistics.Reset();

            _scene.OnRender(deltaTime);
        }

        private void StressTest()
        {
            const float offset = 0.1f;
            const int quadPerAxis = 100;

            for (var x = 0; x < quadPerAxis; x++)
            {
                for (var y = 0; y < quadPerAxis; y++)
                {
                    float xPos = offset + x * 1.1f;
                    float yPos = offset + y * 1.1f;

                    _scene.AddSprite(new TransformComponent
                    {
                        Translation = new Vector3(xPos, yPos, 0.0f),
                        Scale = Vector2.One * 0.1f
                    }, new SpriteRendererComponent
                    {
                        Color = new Vector4(xPos / quadPerAxis, yPos / quadPerAxis, 0.8f, 1.0f)
                    });
                }
            }
        }

        public override void OnEvent(Event e)
        {
            _scene.OnEvent(e);
        }

        public override void Dispose(bool manual)
        {
            Logger.Assert(manual, $"Memory leak detected on object: {this}");
        }
    }
}