using System.Collections.Generic;
using System.Numerics;
using AcidarX.Core.Camera;
using AcidarX.Core.Logging;
using AcidarX.Core.Profiling;
using Microsoft.Extensions.Logging;

namespace AcidarX.Core.Renderer
{
    public static class Renderer2DData
    {
        public static Matrix4x4 ViewProjectionMatrix { get; set; }
        public static VertexArray VertexArray { get; set; }
        public static Shader TextureShader { get; set; }
        public static Texture2D WhiteTexture { get; set; }
        public static float[] Vertices { get; set; }
        public static uint[] Indices { get; set; }

        public static void Dispose()
        {
            VertexArray?.Dispose();
            WhiteTexture?.Dispose();
            TextureShader?.Dispose();
        }
    }

    public sealed class AXRenderer2D
    {
        private static readonly ILogger<AXRenderer2D> Logger = AXLogger.CreateLogger<AXRenderer2D>();
        private readonly AssetManager _assetManager;
        private readonly GraphicsFactory _graphicsFactory;
        private readonly RenderCommandDispatcher _renderCommandDispatcher;

        public AXRenderer2D
        (
            RenderCommandDispatcher renderCommandDispatcher, GraphicsFactory graphicsFactory, AssetManager assetManager
        )
        {
            _renderCommandDispatcher = renderCommandDispatcher;
            _graphicsFactory = graphicsFactory;
            _assetManager = assetManager;
        }

        public static API API => RendererAPI.API;

        public void BeginScene(OrthographicCamera camera)
        {
            Renderer2DData.ViewProjectionMatrix = camera.ViewProjectionMatrix;

            _renderCommandDispatcher.UseShader(Renderer2DData.TextureShader, new List<ShaderInputData>
            {
                new()
                {
                    Name = "u_ViewProjection", Type = ShaderDataType.Mat4,
                    Data = Renderer2DData.ViewProjectionMatrix
                }
            });
        }

        public unsafe void Init()
        {
            AXProfiler.Capture(() =>
            {
                Renderer2DData.VertexArray = _graphicsFactory.CreateVertexArray();

                Renderer2DData.Vertices = new[]
                {
                    // X, Y, Z | U, V
                    -0.5f, -0.5f, 0.0f, 0.0f, 0.0f,
                    0.5f, -0.5f, 0.0f, 1.0f, 0.0f,
                    0.5f, 0.5f, 0.0f, 1.0f, 1.0f,
                    -0.5f, 0.5f, 0.5f, 0.0f, 1.0f
                };

                VertexBuffer squareVertexBuffer = _graphicsFactory.CreateVertexBuffer(Renderer2DData.Vertices);
                squareVertexBuffer.SetLayout(new BufferLayout(new List<BufferElement>
                {
                    new("a_Position", ShaderDataType.Float3),
                    new("a_TextureCoordinates", ShaderDataType.Float2)
                }));
                Renderer2DData.VertexArray.AddVertexBuffer(squareVertexBuffer);

                Renderer2DData.Indices = new uint[]
                {
                    0, 1, 2,
                    2, 3, 0
                };
                IndexBuffer squareIndexBuffer = _graphicsFactory.CreateIndexBuffer(Renderer2DData.Indices);
                Renderer2DData.VertexArray.SetIndexBuffer(squareIndexBuffer);

                // If we are drawing with color API, use this texture
                Renderer2DData.WhiteTexture = _graphicsFactory.CreateTexture(1, 1);
                var whiteTextureData = 0xffffffff;
                Renderer2DData.WhiteTexture.SetData(&whiteTextureData, sizeof(uint));

                Renderer2DData.TextureShader = _assetManager.GetShader("assets/Shaders/Texture");
            });
        }

        public void DrawQuad(Vector2 position, Vector2 size, Vector4 color)
        {
            DrawQuad(new Vector3(position, 0.0f), size, color);
        }

        public void DrawQuad(Vector3 position, Vector2 size, Vector4 color)
        {
            Matrix4x4 transform = Matrix4x4.CreateTranslation(position) *
                                  Matrix4x4.CreateScale(new Vector3(size, 1.0f));

            _renderCommandDispatcher.UseTexture2D(TextureSlot.Texture0, Renderer2DData.WhiteTexture);
            _renderCommandDispatcher.UseShader(Renderer2DData.TextureShader, new List<ShaderInputData>
            {
                new() {Name = "u_Model", Type = ShaderDataType.Mat4, Data = transform},
                new() {Name = "u_Color", Type = ShaderDataType.Float4, Data = color}
            });
            Renderer2DData.VertexArray.Bind();
            _renderCommandDispatcher.DrawIndexed(Renderer2DData.VertexArray);
            _renderCommandDispatcher.UnbindTexture2D(Renderer2DData.WhiteTexture);
        }

        public void DrawQuad(Vector2 position, Vector2 size, Texture2D texture2D)
        {
            DrawQuad(new Vector3(position, 0.0f), size, texture2D);
        }

        public void DrawQuad(Vector3 position, Vector2 size, Texture2D texture2D)
        {
            Matrix4x4 transform = Matrix4x4.CreateTranslation(position) *
                                  Matrix4x4.CreateScale(new Vector3(size, 1.0f));

            _renderCommandDispatcher.UseTexture2D(TextureSlot.Texture0, texture2D);
            _renderCommandDispatcher.UseShader(Renderer2DData.TextureShader, new List<ShaderInputData>
            {
                new() {Name = "u_Model", Type = ShaderDataType.Mat4, Data = transform},
                new() {Name = "u_Color", Type = ShaderDataType.Float4, Data = Vector4.One}
            });
            Renderer2DData.VertexArray.Bind();
            _renderCommandDispatcher.DrawIndexed(Renderer2DData.VertexArray);
            _renderCommandDispatcher.UnbindTexture2D(texture2D);
        }

        public void EndScene()
        {
        }
    }
}