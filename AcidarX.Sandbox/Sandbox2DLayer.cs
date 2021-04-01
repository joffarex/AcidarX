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

        private Vector4 _squareColor;
        private Vector3 _squarePosition;

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
            _squareColor = new Vector4(0.4f, 0.1f, 0.8f, 1.0f);
            _squarePosition = Vector3.Zero;

            _renderer2D.Init(_assetManager.GetShader("assets/Shaders/FlatColor"));
        }

        public override void OnDetach()
        {
        }

        public override void OnImGuiRender()
        {
            ImGuiNET.ImGui.Begin("Square");
            ImGuiNET.ImGui.SetWindowFontScale(1.5f);
            ImGuiNET.ImGui.ColorPicker4("Color", ref _squareColor);
            ImGuiNET.ImGui.DragFloat3("Position", ref _squarePosition);
            ImGuiNET.ImGui.End();
        }

        public override void OnUpdate(double deltaTime)
        {
            _cameraController.OnUpdate(deltaTime);
        }

        public override void OnRender(double deltaTime)
        {
            _renderer2D.BeginScene(_cameraController.Camera);

            _renderer2D.DrawQuad(_squarePosition, Vector2.One, _squareColor);

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