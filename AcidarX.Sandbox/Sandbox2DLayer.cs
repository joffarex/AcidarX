using System.Collections.Generic;
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


        private static VertexArray _squareVertexArray;
        private static Shader _squareShader;
        private readonly AssetManager _assetManager;

        private readonly OrthographicCameraController _cameraController;
        private readonly GraphicsFactory _graphicsFactory;

        private readonly AXRenderer _renderer;
        private Vector4 _squareColor;
        private Vector3 _squarePosition;

        public Sandbox2DLayer(AXRenderer renderer, AssetManager assetManager, GraphicsFactory graphicsFactory)
            : base("Sandbox 2D layer")
        {
            _renderer = renderer;
            _assetManager = assetManager;
            _graphicsFactory = graphicsFactory;
            _cameraController = new OrthographicCameraController(16.0f / 9.0f);
        }

        public override void OnAttach()
        {
        }

        public override void OnLoad()
        {
            _squareColor = new Vector4(0.4f, 0.1f, 0.8f, 1.0f);
            _squarePosition = Vector3.Zero;

            _squareVertexArray = _graphicsFactory.CreateVertexArray();

            float[] squareVertices =
            {
                // X, Y, Z
                -0.5f, -0.5f, 0.0f,
                0.5f, -0.5f, 0.0f,
                0.5f, 0.5f, 0.0f,
                -0.5f, 0.5f, 0.5f
            };

            VertexBuffer squareVertexBuffer = _graphicsFactory.CreateVertexBuffer(squareVertices);
            squareVertexBuffer.SetLayout(new BufferLayout(new List<BufferElement>
            {
                new("a_Position", ShaderDataType.Float3)
            }));
            _squareVertexArray.AddVertexBuffer(squareVertexBuffer);

            uint[] squareIndices =
            {
                0, 1, 3,
                1, 2, 3
            };
            IndexBuffer squareIndexBuffer = _graphicsFactory.CreateIndexBuffer(squareIndices);
            _squareVertexArray.SetIndexBuffer(squareIndexBuffer);

            _squareShader = _assetManager.GetShader("assets/Shaders/FlatColor");
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
            _renderer.BeginScene(_cameraController.Camera);

            var transform = Matrix4x4.CreateTranslation(_squarePosition);
            _renderer.Submit(_squareVertexArray, _squareShader, transform, _squareColor);

            _renderer.EndScene();
        }

        public override void OnEvent(Event e)
        {
            _cameraController.OnEvent(e);
        }

        public override void Dispose(bool manual)
        {
            _squareShader.Dispose();
        }
    }
}