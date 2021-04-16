using System.Collections.Generic;
using System.Numerics;
using AcidarX.Core.Layers;
using AcidarX.Graphics;
using AcidarX.Graphics.Camera;
using AcidarX.Graphics.Renderer;
using AcidarX.Kernel.Events;
using AcidarX.Kernel.Logging;
using Microsoft.Extensions.Logging;

namespace AcidarX.Sandbox
{
    public sealed class ExampleLayer : Layer
    {
        private static readonly ILogger<ExampleLayer> Logger = AXLogger.CreateLogger<ExampleLayer>();

        private static VertexArray _squareVertexArray;
        private static Shader _squareShader;
        private static Texture2D _squareTexture;
        private readonly AssetManager _assetManager;

        private readonly OrthographicCameraController _cameraController;
        private readonly GraphicsFactory _graphicsFactory;

        private readonly AXRenderer _renderer;

        private readonly Vector3 _squarePosition = Vector3.Zero;

        private Vector4 _color = new(1.0f, 0.0f, 0.0f, 1.0f);


        public ExampleLayer(AXRenderer renderer, GraphicsFactory graphicsFactory, AssetManager assetManager)
            : base("Example layer")
        {
            _renderer = renderer;
            _graphicsFactory = graphicsFactory;
            _assetManager = assetManager;
            _cameraController = new OrthographicCameraController(16.0f / 9.0f);
        }

        public override void OnAttach()
        {
        }

        public override void OnLoad()
        {
            #region square

            _squareVertexArray = _graphicsFactory.CreateVertexArray();
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

            VertexBuffer squareVertexBuffer = _graphicsFactory.CreateVertexBuffer(squareVertices);
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
            IndexBuffer squareIndexBuffer = _graphicsFactory.CreateIndexBuffer(squareIndices);
            _squareVertexArray.SetIndexBuffer(squareIndexBuffer);

            _squareShader = _assetManager.GetShader("assets/Shaders/Square");

            #endregion
        }

        public override void OnDetach()
        {
        }

        public override void OnImGuiRender(AppRenderEvent e)
        {
        }

        public override void OnUpdate(double deltaTime)
        {
            _cameraController.OnUpdate(deltaTime);
        }

        public override void OnRender(double deltaTime)
        {
            _renderer.BeginScene(_cameraController.Camera);

            Matrix4x4 transform = Matrix4x4.CreateTranslation(_squarePosition) *
                                  Matrix4x4.CreateScale(new Vector3(1.5f, 1.5f, 1.5f));
            _renderer.Submit(_squareVertexArray, _squareShader, transform, _squareTexture);

            _renderer.EndScene();
        }

        public override void OnEvent(Event e)
        {
            _cameraController.OnEvent(e);
        }

        public override void Dispose(bool manual)
        {
            _squareShader.Dispose();
            _squareVertexArray.Dispose();
        }
    }
}