using System;
using System.Numerics;
using AcidarX.Core;
using AcidarX.Core.Camera;
using AcidarX.Core.Events;
using AcidarX.Core.Layers;
using AcidarX.Core.Logging;
using AcidarX.Core.Renderer;
using Microsoft.Extensions.Logging;

namespace AcidarX.Sandbox
{
    public sealed class Sandbox2DLayer : Layer
    {
        private static readonly ILogger<Sandbox2DLayer> Logger = AXLogger.CreateLogger<Sandbox2DLayer>();

        private readonly AssetManager _assetManager;

        private readonly OrthographicCameraController _cameraController;
        private readonly AXRenderer2D _renderer2D;

        private Texture2D _texture;

        public Sandbox2DLayer(AXRenderer2D renderer2D, AssetManager assetManager)
            : base("Sandbox 2D layer")
        {
            _renderer2D = renderer2D;
            _assetManager = assetManager;
            _cameraController = new OrthographicCameraController(16.0f / 9.0f);
        }

        public override void OnAttach()
        {
        }

        public override void OnLoad()
        {
            _texture = _assetManager.GetTexture2D("assets/Textures/awesomeface.png");
            _renderer2D.Init();
        }

        public override void OnDetach()
        {
        }

        public override void OnImGuiRender()
        {
        }

        public override void OnUpdate(double deltaTime)
        {
            _cameraController.OnUpdate(deltaTime);
        }

        public override void OnRender(double deltaTime)
        {
            _renderer2D.BeginScene(_cameraController.Camera);

            _renderer2D.DrawQuad(new QuadProperties
            {
                Position = new Vector3(Vector2.Zero, 1.0f), Size = Vector2.One * 1.2f,
                Color = new Vector4(0.4f, 0.1f, 0.8f, 1.0f), RotationInRadians = 45 * (float) (Math.PI / 180.0f)
            });
            _renderer2D.DrawQuad(new QuadProperties
            {
                Position = new Vector3(-Vector2.One * 0.5f, 2.0f), Size = Vector2.One * 0.8f,
                Color = new Vector4(0.8f, 0.1f, 0.4f, 1.0f)
            });
            _renderer2D.DrawQuad(new QuadProperties
            {
                Position = new Vector3(-Vector2.UnitX * 0.7f, 0.0f), Size = Vector2.One * 1.1f,
                Color = new Vector4(0.8f, 0.8f, 0.4f, 1.0f)
            });

            StressTest();

            // _renderer2D.DrawQuad(new QuadProperties
            // {
            //     Position = new Vector3(Vector2.One * 0.3f, 3.0f), Texture2D = _texture,
            //     Color = new Vector4(1.0f, 0.6f, 0.6f, 1.0f)
            // });
            // _renderer2D.DrawQuad(new QuadProperties
            // {
            //     Position = new Vector3(Vector2.One * -0.5f, 0.0f), Size = Vector2.One * 1.1f, Texture2D = _texture,
            //     RotationInRadians = 45 * (float) (Math.PI / 180.0f), TilingFactor = 2.0f
            // });

            _renderer2D.EndScene();
        }

        private void StressTest()
        {
            const float offset = 0.1f;
            const int quadPerAxis = 90;

            for (var x = 0; x < quadPerAxis; x++)
            {
                for (var y = 0; y < quadPerAxis; y++)
                {
                    float xPos = offset + (x * 1.1f);
                    float yPos = offset + (y * 1.1f);

                    _renderer2D.DrawQuad(new QuadProperties
                    {
                        Position = new Vector3(xPos, yPos, 0.0f), Size = Vector2.One * 0.1f,
                        Color = new Vector4(xPos / quadPerAxis, yPos / quadPerAxis, 0.8f, 1.0f)
                    });
                }
            }
        }

        public override void OnEvent(Event e)
        {
            _cameraController.OnEvent(e);
        }

        public override void Dispose(bool manual)
        {
            Logger.Assert(manual, $"Memory leak detected on object: {this}");
        }
    }
}