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

            _renderer2D.DrawQuad(Vector2.Zero, Vector2.One * 1.2f, new Vector4(0.4f, 0.1f, 0.8f, 1.0f));
            _renderer2D.DrawQuad(-Vector2.One * 0.5f, Vector2.One * 0.8f, new Vector4(0.8f, 0.1f, 0.4f, 1.0f));
            _renderer2D.DrawQuad(new Vector3(Vector2.One * 0.3f, 0.1f), Vector2.One, _texture);
            _renderer2D.DrawQuad(new Vector3(Vector2.One * -0.5f, -0.1f), Vector2.One * 1.1f, _texture);

            _renderer2D.EndScene();
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