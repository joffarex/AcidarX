using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.InteropServices;
using AcidarX.Core.Camera;
using AcidarX.Core.Logging;
using AcidarX.Core.Profiling;
using Microsoft.Extensions.Logging;

namespace AcidarX.Core.Renderer
{
    public static unsafe class Renderer2DData
    {
        public static Matrix4x4 ViewProjectionMatrix { get; set; }
        public static VertexArray VertexArray { get; set; }
        public static VertexBuffer VertexBuffer { get; set; }
        public static Shader TextureShader { get; set; }
        public static Texture2D WhiteTexture { get; set; }

        public static uint MaxQuads { get; } = 10000;
        public static uint MaxVertices { get; } = MaxQuads * 4;
        public static uint MaxIndices { get; } = MaxQuads * 6;
        public static uint QuadIndexCount { get; set; } = 0; // How many indices has been drawn, so we can flush data

        public static QuadVertex* QuadVertexBufferBase { get; set; } = null;
        public static QuadVertex* QuadVertexBufferPtr { get; set; } = null;

        public static List<QuadVertex> QuadVertices { get; set; } = new();

        public static void Dispose()
        {
            VertexArray?.Dispose();
            WhiteTexture?.Dispose();
            TextureShader?.Dispose();
        }
    }

    [StructLayout(LayoutKind.Explicit, Size = 36, CharSet = CharSet.Ansi)]
    public struct QuadVertex
    {
        [field: FieldOffset(0)] public Vector3 Position { get; set; }

        [field: FieldOffset(12)] public Vector4 Color { get; set; }
        [field: FieldOffset(28)] public Vector2 TextureCoordinate { get; set; }
    }

    public record QuadProperties
    {
        public Vector3 Position { get; init; } = Vector3.One;
        public Vector2 Size { get; init; } = Vector2.One;
        public Vector4 Color { get; init; } = Vector4.One;
        public Texture2D Texture2D { get; init; }
        public float TilingFactor { get; init; } = 1.0f;
        public float RotationInRadians { get; init; }
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

        public unsafe void Init()
        {
            AXProfiler.Capture(() =>
            {
                Renderer2DData.VertexArray = _graphicsFactory.CreateVertexArray();

                Renderer2DData.VertexBuffer =
                    _graphicsFactory.CreateVertexBuffer<float>(Renderer2DData.MaxVertices);
                Renderer2DData.VertexBuffer.SetLayout(new BufferLayout(new List<BufferElement>
                {
                    new("a_Position", ShaderDataType.Float3),
                    new("a_Color", ShaderDataType.Float4),
                    new("a_TextureCoordinates", ShaderDataType.Float2)
                }));
                Renderer2DData.VertexArray.AddVertexBuffer(Renderer2DData.VertexBuffer);

                QuadVertex[] quadVertices = new QuadVertex[Renderer2DData.MaxVertices];
                fixed (QuadVertex* ptr = &quadVertices[0])
                {
                    Renderer2DData.QuadVertexBufferBase = ptr;
                }

                // Generate indices
                var quadIndices = new uint[Renderer2DData.MaxIndices];

                uint offset = 0;
                for (var i = 0; i < Renderer2DData.MaxIndices; i += 6)
                {
                    quadIndices[i + 0] = offset + 0;
                    quadIndices[i + 1] = offset + 1;
                    quadIndices[i + 2] = offset + 2;

                    quadIndices[i + 3] = offset + 2;
                    quadIndices[i + 4] = offset + 3;
                    quadIndices[i + 5] = offset + 0;

                    offset += 4;
                }

                IndexBuffer quadIndexBuffer = _graphicsFactory.CreateIndexBuffer(quadIndices);
                Renderer2DData.VertexArray.SetIndexBuffer(quadIndexBuffer);

                // If we are drawing with color API, use this texture
                Renderer2DData.WhiteTexture = _graphicsFactory.CreateTexture(1, 1);
                var whiteTextureData = 0xffffffff;
                Renderer2DData.WhiteTexture.SetData(&whiteTextureData, sizeof(uint));

                Renderer2DData.TextureShader = _assetManager.GetShader("assets/Shaders/Texture");
            });
        }


        public unsafe void BeginScene(OrthographicCamera camera)
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

            Renderer2DData.QuadVertexBufferPtr = Renderer2DData.QuadVertexBufferBase;
        }


        public unsafe void DrawQuad(QuadProperties quadProperties)
        {
            // CHORE: leaving this for maintenance purposes
            // Renderer2DData.QuadVertexBufferPtr->Position = quadProperties.Position;
            // Renderer2DData.QuadVertexBufferPtr->Color = quadProperties.Color;
            // Renderer2DData.QuadVertexBufferPtr->TextureCoordinate = Vector2.Zero;
            // Renderer2DData.QuadVertexBufferPtr++;
            //
            // Renderer2DData.QuadVertexBufferPtr->Position =
            //     new Vector3(quadProperties.Position.X + quadProperties.Size.X, quadProperties.Position.Y, 0.0f);
            // Renderer2DData.QuadVertexBufferPtr->Color = quadProperties.Color;
            // Renderer2DData.QuadVertexBufferPtr->TextureCoordinate = new Vector2(1.0f, 0.0f);
            // Renderer2DData.QuadVertexBufferPtr++;
            //
            // Renderer2DData.QuadVertexBufferPtr->Position = new Vector3(
            //     quadProperties.Position.X + quadProperties.Size.X, quadProperties.Position.Y + quadProperties.Size.Y,
            //     0.0f);
            // Renderer2DData.QuadVertexBufferPtr->Color = quadProperties.Color;
            // Renderer2DData.QuadVertexBufferPtr->TextureCoordinate = Vector2.One;
            // Renderer2DData.QuadVertexBufferPtr++;
            //
            // Renderer2DData.QuadVertexBufferPtr->Position = new Vector3(quadProperties.Position.X,
            //     quadProperties.Position.Y + quadProperties.Size.Y, 0.0f);
            // Renderer2DData.QuadVertexBufferPtr->Color = quadProperties.Color;
            // Renderer2DData.QuadVertexBufferPtr->TextureCoordinate = new Vector2(0.0f, 1.0f);
            // Renderer2DData.QuadVertexBufferPtr++;

            Renderer2DData.QuadVertices.Add(new QuadVertex()
            {
                Position = quadProperties.Position, Color = quadProperties.Color, TextureCoordinate = Vector2.Zero
            });
            Renderer2DData.QuadVertices.Add(new QuadVertex()
            {
                Position = new Vector3(quadProperties.Position.X + quadProperties.Size.X, quadProperties.Position.Y,
                    0.0f),
                Color = quadProperties.Color, TextureCoordinate = new Vector2(1.0f, 0.0f)
            });
            Renderer2DData.QuadVertices.Add(new QuadVertex()
            {
                Position = new Vector3(
                    quadProperties.Position.X + quadProperties.Size.X,
                    quadProperties.Position.Y + quadProperties.Size.Y,
                    0.0f),
                Color = quadProperties.Color, TextureCoordinate = Vector2.One
            });
            Renderer2DData.QuadVertices.Add(new QuadVertex()
            {
                Position = new Vector3(quadProperties.Position.X,
                    quadProperties.Position.Y + quadProperties.Size.Y, 0.0f),
                Color = quadProperties.Color, TextureCoordinate = new Vector2(0.0f, 1.0f)
            });

            Renderer2DData.QuadIndexCount += 6;

            // var transform = Matrix4x4.CreateTranslation(quadProperties.Position);
            // if (quadProperties.RotationInRadians != 0.0f)
            // {
            //     transform *= Matrix4x4.CreateRotationZ(quadProperties.RotationInRadians);
            // }
            //
            // transform *= Matrix4x4.CreateScale(new Vector3(quadProperties.Size, 1.0f));
            //
            // _renderCommandDispatcher.UseTexture2D(TextureSlot.Texture0,
            //     quadProperties.Texture2D ?? Renderer2DData.WhiteTexture);
            // _renderCommandDispatcher.UseShader(Renderer2DData.TextureShader, new List<ShaderInputData>
            // {
            //     new() {Name = "u_Model", Type = ShaderDataType.Mat4, Data = transform},
            //     new() {Name = "u_TilingFactor", Type = ShaderDataType.Float, Data = quadProperties.TilingFactor}
            // });
            // Renderer2DData.VertexArray.Bind();
            // _renderCommandDispatcher.DrawIndexed(Renderer2DData.VertexArray);
            // _renderCommandDispatcher.UnbindTexture2D(quadProperties.Texture2D ?? Renderer2DData.WhiteTexture);
        }


        public unsafe void EndScene()
        {
            Renderer2DData.VertexBuffer.SetData(new ReadOnlySpan<QuadVertex>(Renderer2DData.QuadVertices.ToArray()));
            
            // CHORE: leaving this for maintenance purposes
            // long dataSize = Renderer2DData.QuadVertexBufferPtr - Renderer2DData.QuadVertexBufferBase;
            // Renderer2DData.VertexBuffer.SetData(Renderer2DData.QuadVertexBufferBase, dataSize * sizeof(QuadVertex));

            Flush();
        }

        public void Flush()
        {
            Renderer2DData.VertexArray.Bind();
            _renderCommandDispatcher.DrawIndexed(Renderer2DData.VertexArray, Renderer2DData.QuadIndexCount);
        }
    }
}