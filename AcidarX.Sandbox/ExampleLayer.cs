using System.Collections.Generic;
using System.Numerics;
using AcidarX.Core;
using AcidarX.Core.Camera;
using AcidarX.Core.Events;
using AcidarX.Core.Input;
using AcidarX.Core.Layers;
using AcidarX.Core.Logging;
using AcidarX.Core.Renderer;
using Microsoft.Extensions.Logging;

namespace AcidarX.Sandbox
{
    public class ExampleLayer : Layer
    {
        private const float _cameraSpeed = 1.5f;
        private static readonly ILogger<ExampleLayer> Logger = AXLogger.CreateLogger<ExampleLayer>();

        private static VertexArray _squareVertexArray;
        private static Shader _squareShader;

        private static VertexArray _triangleVertexArray;
        private static Shader _triangleShader;
        private readonly AssetManager _assetManager;

        private readonly OrthographicCamera _camera;

        private Vector4 _color = new(1.0f, 0.0f, 0.0f, 1.0f);

        private Vector3 _squarePosition = Vector3.Zero;

        public ExampleLayer(AXRenderer renderer, GraphicsFactory graphicsFactory, AssetManager assetManager)
            : base("Example layer", renderer, graphicsFactory)
        {
            _assetManager = assetManager;
            _camera = new OrthographicCamera(-1.6f, 1.6f, -0.9f, 0.9f);
        }

        public override void OnAttach()
        {
        }

        public override void OnLoad()
        {
            #region square

            _squareVertexArray = GraphicsFactory.CreateVertexArray();

            float[] squareVertices =
            {
                //X    Y      Z
                0.5f, 0.5f, 0.0f, 0.8f, 0.0f, 1.0f, 1.0f,
                0.5f, -0.5f, 0.0f, 0.6f, 0.0f, 1.0f, 1.0f,
                -0.5f, -0.5f, 0.0f, 0.4f, 0.0f, 1.0f, 1.0f,
                -0.5f, 0.5f, 0.5f, 0.2f, 0.0f, 1.0f, 1.0f
            };

            VertexBuffer squareVertexBuffer = GraphicsFactory.CreateVertexBuffer(squareVertices);
            squareVertexBuffer.SetLayout(new BufferLayout(new List<BufferElement>
            {
                new("a_Position", ShaderDataType.Float3),
                new("a_Color", ShaderDataType.Float4)
            }));
            _squareVertexArray.AddVertexBuffer(squareVertexBuffer);

            uint[] squareIndices =
            {
                0, 1, 3,
                1, 2, 3
            };
            IndexBuffer squareIndexBuffer = GraphicsFactory.CreateIndexBuffer(squareIndices);
            _squareVertexArray.SetIndexBuffer(squareIndexBuffer);

            _squareShader = _assetManager.GetShader("assets/Shaders/Square");

            #endregion

            #region triangle

            _triangleVertexArray = GraphicsFactory.CreateVertexArray();

            float[] triangleVertices =
            {
                //X    Y      Z
                -0.5f, -0.5f, 0.0f,
                0.5f, -0.5f, 0.0f,
                0.0f, 0.5f, 0.0f
            };

            VertexBuffer triangleVertexBuffer = GraphicsFactory.CreateVertexBuffer(triangleVertices);
            triangleVertexBuffer.SetLayout(new BufferLayout(new List<BufferElement>
            {
                new("a_Position", ShaderDataType.Float3)
            }));
            _triangleVertexArray.AddVertexBuffer(triangleVertexBuffer);

            uint[] triangleIndices =
            {
                0, 1, 2
            };
            IndexBuffer triangleIndexBuffer = GraphicsFactory.CreateIndexBuffer(triangleIndices);
            _triangleVertexArray.SetIndexBuffer(triangleIndexBuffer);

            _triangleShader = _assetManager.GetShader("assets/Shaders/Triangle");

            #endregion
        }

        public override void OnDetach()
        {
        }

        public override void OnImGuiRender()
        {
            ImGuiNET.ImGui.Begin("Triangle");
            ImGuiNET.ImGui.SetWindowFontScale(1.4f);
            ImGuiNET.ImGui.ColorPicker4("Color", ref _color);
            ImGuiNET.ImGui.End();
        }

        public override void OnUpdate(double deltaTime)
        {
        }

        public override void OnRender(double deltaTime)
        {
            Renderer.BeginScene(_camera);

            if (KeyboardState.IsKeyPressed(AXKey.A))
            {
                Vector3 pos = _camera.Position;
                _camera.Position = new Vector3(pos.X - _cameraSpeed * (float) deltaTime, pos.Y, pos.Z);
            }

            if (KeyboardState.IsKeyPressed(AXKey.D))
            {
                Vector3 pos = _camera.Position;
                _camera.Position = new Vector3(pos.X + _cameraSpeed * (float) deltaTime, pos.Y, pos.Z);
            }

            if (KeyboardState.IsKeyPressed(AXKey.W))
            {
                Vector3 pos = _camera.Position;
                _camera.Position = new Vector3(pos.X, pos.Y + _cameraSpeed * (float) deltaTime, pos.Z);
            }

            if (KeyboardState.IsKeyPressed(AXKey.S))
            {
                Vector3 pos = _camera.Position;
                _camera.Position = new Vector3(pos.X, pos.Y - _cameraSpeed * (float) deltaTime, pos.Z);
            }

            Renderer.Submit(_squareVertexArray, _squareShader);
            Renderer.Submit(_triangleVertexArray, _triangleShader, _color);

            Renderer.EndScene();
        }

        public override void OnEvent(Event e)
        {
        }

        public override void Dispose(bool manual)
        {
            _squareShader.Dispose();
            _squareVertexArray.Dispose();
            _triangleShader.Dispose();
            _triangleVertexArray.Dispose();
        }
    }
}