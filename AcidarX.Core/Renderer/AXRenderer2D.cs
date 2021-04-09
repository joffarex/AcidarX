using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.InteropServices;
using AcidarX.Core.Camera;
using AcidarX.Core.Graphics;
using AcidarX.Core.Logging;
using AcidarX.Core.Profiling;
using Microsoft.Extensions.Logging;

namespace AcidarX.Core.Renderer
{
    [StructLayout(LayoutKind.Explicit, Size = 44, CharSet = CharSet.Ansi)]
    public struct QuadVertex
    {
        [field: FieldOffset(0)] public Vector3 Position { get; set; }

        [field: FieldOffset(12)] public Vector4 Color { get; set; }
        [field: FieldOffset(28)] public Vector2 TextureCoordinate { get; set; }
        [field: FieldOffset(36)] public float TextureIndex { get; set; }
        [field: FieldOffset(40)] public float TilingFactor { get; set; }
    }

    public record QuadProperties
    {
        public Vector3 Position { get; init; } = Vector3.One;
        public Vector2 Size { get; init; } = Vector2.One;
        public Vector4 Color { get; init; } = Vector4.One;
        public Texture2D Texture2D { get; init; }
        public SubTexture2D SubTexture2D { get; init; }
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

            Renderer2DData.QuadVertexPositions[0] = new Vector3(-0.5f, -0.5f, 0.0f); // bottom left
            Renderer2DData.QuadVertexPositions[1] = new Vector3(0.5f, -0.5f, 0.0f); // bottom right
            Renderer2DData.QuadVertexPositions[2] = new Vector3(0.5f, 0.5f, 0.0f); // top right
            Renderer2DData.QuadVertexPositions[3] = new Vector3(-0.5f, 0.5f, 0.0f); // top left

            Renderer2DData.QuadTextureCoordinates[0] = Vector2.Zero;
            Renderer2DData.QuadTextureCoordinates[1] = Vector2.UnitX;
            Renderer2DData.QuadTextureCoordinates[2] = Vector2.One;
            Renderer2DData.QuadTextureCoordinates[3] = Vector2.UnitY;
        }

        public static API API => RendererAPI.API;

        public unsafe void Init()
        {
            AXProfiler.Capture(() =>
            {
                Renderer2DData.VertexArray = _graphicsFactory.CreateVertexArray();

                Renderer2DData.VertexBuffer =
                    _graphicsFactory.CreateVertexBuffer<QuadVertex>((uint) Renderer2DData.QuadVertices.Length);
                Renderer2DData.VertexBuffer.SetLayout(new BufferLayout(new List<BufferElement>
                {
                    new("a_Position", ShaderDataType.Float3),
                    new("a_Color", ShaderDataType.Float4),
                    new("a_TextureCoordinates", ShaderDataType.Float2),
                    new("a_TextureIndex", ShaderDataType.Float),
                    new("a_TilingFactor", ShaderDataType.Float)
                }));
                Renderer2DData.VertexArray.AddVertexBuffer(Renderer2DData.VertexBuffer);

                // Generate indices
                var quadIndices = new uint[Renderer2DData.MaxIndices];

                uint offset = 0;
                for (uint i = 0; i < Renderer2DData.MaxIndices; i += Renderer2DData.IndexPerQuad)
                {
                    quadIndices[i + 0] = offset + 0;
                    quadIndices[i + 1] = offset + 1;
                    quadIndices[i + 2] = offset + 2;

                    quadIndices[i + 3] = offset + 2;
                    quadIndices[i + 4] = offset + 3;
                    quadIndices[i + 5] = offset + 0;

                    offset += Renderer2DData.VertexPerQuad;
                }

                IndexBuffer quadIndexBuffer = _graphicsFactory.CreateIndexBuffer(quadIndices);
                Renderer2DData.VertexArray.SetIndexBuffer(quadIndexBuffer);

                Renderer2DData.WhiteTexture = _graphicsFactory.CreateTexture(1, 1);
                var whiteTextureData = 0xffffffff;
                Renderer2DData.WhiteTexture.SetData(&whiteTextureData, sizeof(uint));
                Renderer2DData.TextureSlots[0] = Renderer2DData.WhiteTexture;

                Renderer2DData.TextureShader = _assetManager.GetShader("assets/Shaders/Texture");
            });
        }


        public void BeginScene(OrthographicCamera camera)
        {
            StartBatch();
            Renderer2DData.ViewProjectionMatrix = camera.ViewProjectionMatrix;

            var samplers = new uint[Renderer2DData.MaxTextureSlots];
            for (uint i = 0; i < Renderer2DData.MaxTextureSlots; i++)
            {
                samplers[i] = i;
            }

            _renderCommandDispatcher.UseShader(Renderer2DData.TextureShader, new List<ShaderInputData>
            {
                new()
                {
                    Name = "u_ViewProjection", Type = ShaderDataType.Mat4,
                    Data = Renderer2DData.ViewProjectionMatrix
                },
                new() {Name = "u_Textures", Type = ShaderDataType.IntSamplerArr, Data = samplers}
            });
        }

        private void StartBatch()
        {
            Array.Clear(Renderer2DData.QuadVertices, 0, (int) Renderer2DData.QuadVertexCount);
            Renderer2DData.QuadIndexCount = 0;
            Renderer2DData.QuadVertexCount = 0;
            Renderer2DData.TextureSlotIndex = 1;
        }

        private void NextBatch()
        {
            EndScene();
            StartBatch();
        }

        private void GetTextureIndex(Texture2D texture2D, ref float textureIndex)
        {
            for (uint i = 1; i < Renderer2DData.TextureSlotIndex; i++)
            {
                if (Renderer2DData.TextureSlots[i].Equals(texture2D))
                {
                    // If we already have submitted texture in our slots, we can just use it in shader
                    textureIndex = i;
                    break;
                }
            }

            if (textureIndex == 0.0f)
            {
                if (Renderer2DData.TextureSlotIndex >= Renderer2DData.MaxTextureSlots)
                {
                    NextBatch();
                }

                textureIndex = Renderer2DData.TextureSlotIndex;
                Renderer2DData.TextureSlots[Renderer2DData.TextureSlotIndex] = texture2D;
                Renderer2DData.TextureSlotIndex++;
            }
        }

        private void GetTextureIndex(QuadProperties quadProperties, ref float textureIndex)
        {
            if (quadProperties.Texture2D != null)
            {
                GetTextureIndex(quadProperties.Texture2D, ref textureIndex);
            }

            if (quadProperties.SubTexture2D?.Texture2D != null)
            {
                GetTextureIndex(quadProperties.SubTexture2D.Texture2D, ref textureIndex);
            }
        }

        public void DrawQuad(QuadProperties quadProperties)
        {
            Logger.Assert(!(quadProperties.SubTexture2D != null && quadProperties.Texture2D != null),
                "Can not have have SubTexture and Texture together");

            if (Renderer2DData.QuadIndexCount >= Renderer2DData.MaxIndices)
            {
                NextBatch();
            }

            // If there is no texture, we'll just use default white one
            var textureIndex = 0.0f;
            GetTextureIndex(quadProperties, ref textureIndex);

            var transform = Matrix4x4.CreateTranslation(quadProperties.Position);
            if (quadProperties.RotationInRadians != 0.0f)
            {
                transform *= Matrix4x4.CreateRotationZ(quadProperties.RotationInRadians);
            }

            transform *= Matrix4x4.CreateScale(new Vector3(quadProperties.Size, 1.0f));

            for (var i = 0; i < Renderer2DData.VertexPerQuad; i++)
            {
                Vector3 transformedPosition = Vector3.Transform(Renderer2DData.QuadVertexPositions[i], transform);

                Vector2 textureCoordinate = quadProperties.SubTexture2D?.Texture2D != null
                    ? quadProperties.SubTexture2D.TextureCoordinates[i]
                    : Renderer2DData.QuadTextureCoordinates[i];

                Renderer2DData.QuadVertices[Renderer2DData.QuadVertexCount + i] = new QuadVertex
                {
                    Position = transformedPosition, TextureIndex = textureIndex,
                    TilingFactor = quadProperties.TilingFactor, Color = quadProperties.Color,
                    TextureCoordinate = textureCoordinate
                };
            }

            Renderer2DData.QuadIndexCount += Renderer2DData.IndexPerQuad;
            Renderer2DData.QuadVertexCount += Renderer2DData.VertexPerQuad;

            AXStatistics.QuadCount++;
        }


        public unsafe void EndScene()
        {
            if (Renderer2DData.QuadIndexCount == 0)
            {
                return; // Nothing to draw
            }

            fixed (void* qvPtr = &Renderer2DData.QuadVertices[0])
            {
                _renderCommandDispatcher.SetVertexBufferData(Renderer2DData.VertexBuffer, qvPtr,
                    Renderer2DData.QuadVertexCount);
            }

            Flush();
        }

        public void Flush()
        {
            for (var i = 0; i < Renderer2DData.TextureSlotIndex; i++)
            {
                Texture2D texture2D = Renderer2DData.TextureSlots[i];
                _renderCommandDispatcher.UseTexture2D((TextureSlot) i, texture2D);
            }

            _renderCommandDispatcher.DrawIndexed(Renderer2DData.VertexArray, Renderer2DData.QuadIndexCount);

            AXStatistics.DrawCalls++;

            for (var i = 0; i < Renderer2DData.TextureSlotIndex; i++)
            {
                Texture2D texture2D = Renderer2DData.TextureSlots[i];
                _renderCommandDispatcher.UnbindTexture2D(texture2D);
            }
        }
    }
}