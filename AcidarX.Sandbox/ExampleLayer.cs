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
        private static Texture2D _squareTexture;

        private readonly AssetManager _assetManager;

        private readonly OrthographicCamera _camera;

        private readonly Vector3 _squarePosition = Vector3.Zero;

        private Vector4 _color = new(1.0f, 0.0f, 0.0f, 1.0f);


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
            _squareTexture = _assetManager.GetTexture2D("assets/Textures/awesomeface.png");

            float[] squareVertices =
            {
                // X, Y, Z | R, G, B, A | U, V
                // -0.5f, -0.5f, 0.0f, 0.4f, 0.0f, 1.0f, 1.0f, 0.0f, 0.0f,
                // 0.5f, -0.5f, 0.0f, 0.6f, 0.0f, 1.0f, 1.0f, 1.0f, 0.0f,
                // 0.5f, 0.5f, 0.0f, 0.8f, 0.0f, 1.0f, 1.0f, 1.0f, 1.0f,
                // -0.5f, 0.5f, 0.5f, 0.2f, 0.0f, 1.0f, 1.0f, 0.0f, 1.0f,
                -0.5f, -0.5f, 0.0f, 1.0f, 1.0f, 1.0f, 1.0f, 0.0f, 0.0f,
                0.5f, -0.5f, 0.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 0.0f,
                0.5f, 0.5f, 0.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f,
                -0.5f, 0.5f, 0.5f, 1.0f, 1.0f, 1.0f, 1.0f, 0.0f, 1.0f
            };

            VertexBuffer squareVertexBuffer = GraphicsFactory.CreateVertexBuffer(squareVertices);
            squareVertexBuffer.SetLayout(new BufferLayout(new List<BufferElement>
            {
                new("a_Position", ShaderDataType.Float3),
                new("a_Color", ShaderDataType.Float4),
                new("a_TextureCoordinates", ShaderDataType.Float2)
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
        }

        public override void OnDetach()
        {
        }

        public override void OnImGuiRender()
        {
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

            Matrix4x4 transform = Matrix4x4.CreateTranslation(_squarePosition) *
                                  Matrix4x4.CreateScale(new Vector3(1.5f, 1.5f, 1.5f));
            Renderer.Submit(_squareVertexArray, _squareShader, transform, _squareTexture);

            Renderer.EndScene();
        }

        public override void OnEvent(Event e)
        {
        }

        public override void Dispose(bool manual)
        {
            _squareShader.Dispose();
            _squareVertexArray.Dispose();
        }
    }
}