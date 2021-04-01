using System.Collections.Generic;
using System.Numerics;
using AcidarX.Core.Camera;
using AcidarX.Core.Logging;
using Microsoft.Extensions.Logging;

namespace AcidarX.Core.Renderer
{
    public static class Renderer2DData
    {
        public static Matrix4x4 ViewProjectionMatrix { get; set; }
        public static VertexArray VertexArray { get; set; }
        public static Shader Shader { get; set; }
        public static float[] Vertices { get; set; }
        public static uint[] Indices { get; set; }

        public static void Dispose()
        {
            VertexArray?.Dispose();
            Shader?.Dispose();
        }
    }

    public sealed class AXRenderer2D
    {
        private static readonly ILogger<AXRenderer2D> Logger = AXLogger.CreateLogger<AXRenderer2D>();
        private readonly GraphicsFactory _graphicsFactory;
        private readonly RenderCommandDispatcher _renderCommandDispatcher;

        public AXRenderer2D
            (RenderCommandDispatcher renderCommandDispatcher, GraphicsFactory graphicsFactory)
        {
            _renderCommandDispatcher = renderCommandDispatcher;
            _graphicsFactory = graphicsFactory;
        }

        public static API API => RendererAPI.API;

        public void BeginScene(OrthographicCamera camera)
        {
            Renderer2DData.ViewProjectionMatrix = camera.ViewProjectionMatrix;

            _renderCommandDispatcher.UseShader(Renderer2DData.Shader, new List<ShaderInputData>
            {
                new()
                {
                    Name = "u_ViewProjection", Type = ShaderDataType.Mat4, Data = Renderer2DData.ViewProjectionMatrix
                },
            });
        }

        public void Init(Shader shader)
        {
            Renderer2DData.VertexArray = _graphicsFactory.CreateVertexArray();

            Renderer2DData.Vertices = new[]
            {
                // X, Y, Z
                -0.5f, -0.5f, 0.0f,
                0.5f, -0.5f, 0.0f,
                0.5f, 0.5f, 0.0f,
                -0.5f, 0.5f, 0.5f
            };

            VertexBuffer squareVertexBuffer = _graphicsFactory.CreateVertexBuffer(Renderer2DData.Vertices);
            squareVertexBuffer.SetLayout(new BufferLayout(new List<BufferElement>
            {
                new("a_Position", ShaderDataType.Float3)
            }));
            Renderer2DData.VertexArray.AddVertexBuffer(squareVertexBuffer);

            Renderer2DData.Indices = new uint[]
            {
                0, 1, 2,
                2, 3, 0
            };
            IndexBuffer squareIndexBuffer = _graphicsFactory.CreateIndexBuffer(Renderer2DData.Indices);
            Renderer2DData.VertexArray.SetIndexBuffer(squareIndexBuffer);

            Renderer2DData.Shader = shader;
        }

        public void DrawQuad(Vector2 position, Vector2 size, Vector4 color)
        {
            DrawQuad(new Vector3(position, 0.0f), size, color);
        }

        public void DrawQuad(Vector3 position, Vector2 size, Vector4 color)
        {
            Matrix4x4 transform = Matrix4x4.CreateTranslation(position) *
                                  Matrix4x4.CreateScale(new Vector3(size, 1.0f));

            _renderCommandDispatcher.UseShader(Renderer2DData.Shader, new List<ShaderInputData>
            {
                new() {Name = "u_Model", Type = ShaderDataType.Mat4, Data = transform},
                new() {Name = "u_Color", Type = ShaderDataType.Float4, Data = color}
            });
            Renderer2DData.VertexArray.Bind();
            _renderCommandDispatcher.DrawIndexed(Renderer2DData.VertexArray);
        }

        public void EndScene()
        {
        }
    }
}