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
    public static class AXStatistics
    {
        public static uint DrawCalls { get; set; }
        public static uint QuadCount { get; set; }

        public static void ImGuiWindow()
        {
            ImGuiNET.ImGui.Begin("Statistics");
            ImGuiNET.ImGui.SetWindowFontScale(1.4f);
            ImGuiNET.ImGui.Text($"QuadCount: {QuadCount}");
            ImGuiNET.ImGui.Text($"DrawCalls: {DrawCalls}");
            ImGuiNET.ImGui.Text($"VertexCount: {QuadCount * 4}");
            ImGuiNET.ImGui.Text($"IndexCount: {QuadCount * 6}");
            ImGuiNET.ImGui.End();
        }

        public static void Reset()
        {
            DrawCalls = 0;
            QuadCount = 0;
        }
    }

    public static unsafe class Renderer2DData
    {
        public static Matrix4x4 ViewProjectionMatrix { get; set; }
        public static VertexArray VertexArray { get; set; }
        public static VertexBuffer VertexBuffer { get; set; }
        public static Shader TextureShader { get; set; }
        public static Texture2D WhiteTexture { get; set; }

        public static uint MaxQuads { get; } = 10000;
        public static uint VertexPerQuad { get; } = 4;
        public static uint IndexPerQuad { get; } = 6;
        public static uint MaxVertices { get; } = MaxQuads * VertexPerQuad;
        public static uint MaxIndices { get; } = MaxQuads * IndexPerQuad;
        public static uint QuadIndexCount { get; set; } // How many index has been drawn, so we can draw on the screen

        public static uint
            QuadVertexCount { get; set; } // How many vertex has been drawn, so we can safely set data to buffer

        public static Vector3[] QuadVertexPositions { get; set; } = new Vector3[VertexPerQuad];
        public static Vector2[] QuadTextureCoordinates { get; set; } = new Vector2[VertexPerQuad];

        public static QuadVertex* QuadVertexBufferBase { get; set; } = null;
        public static QuadVertex* QuadVertexBufferPtr { get; set; } = null;

        public static QuadVertex[] QuadVertices { get; set; } = new QuadVertex[MaxVertices];

        public static uint MaxTextureSlots { get; } = 32; // Get it from gpu
        public static Texture2D[] TextureSlots { get; } = new Texture2D[MaxTextureSlots];
        public static uint TextureSlotIndex { get; set; } = 1; // 0 is white texture

        public static void Dispose()
        {
            VertexArray?.Dispose();
            WhiteTexture?.Dispose();
            TextureShader?.Dispose();
        }
    }

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

                QuadVertex[] quadVertices = new QuadVertex[Renderer2DData.MaxVertices];
                fixed (QuadVertex* ptr = &quadVertices[0])
                {
                    Renderer2DData.QuadVertexBufferBase = ptr;
                }

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


        public unsafe void BeginScene(OrthographicCamera camera)
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

            Renderer2DData.QuadVertexBufferPtr = Renderer2DData.QuadVertexBufferBase;
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

        public void DrawQuad(QuadProperties quadProperties)
        {
            if (Renderer2DData.QuadIndexCount >= Renderer2DData.MaxIndices)
            {
                NextBatch();
            }

            var textureIndex = 0.0f;

            // If there is no texture, we'll just use default white one
            if (quadProperties.Texture2D != null)
            {
                for (uint i = 1; i < Renderer2DData.TextureSlotIndex; i++)
                {
                    if (Renderer2DData.TextureSlots[i].Equals(quadProperties.Texture2D))
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
                    Renderer2DData.TextureSlots[Renderer2DData.TextureSlotIndex] = quadProperties.Texture2D;
                    Renderer2DData.TextureSlotIndex++;
                }
            }

            var transform = Matrix4x4.CreateTranslation(quadProperties.Position);
            if (quadProperties.RotationInRadians != 0.0f)
            {
                transform *= Matrix4x4.CreateRotationZ(quadProperties.RotationInRadians);
            }

            transform *= Matrix4x4.CreateScale(new Vector3(quadProperties.Size, 1.0f));

            for (var i = 0; i < Renderer2DData.VertexPerQuad; i++)
            {
                Vector3 transformedPosition = Vector3.Transform(Renderer2DData.QuadVertexPositions[i], transform);

                Renderer2DData.QuadVertices[Renderer2DData.QuadVertexCount + i] = new QuadVertex
                {
                    Position = transformedPosition, TextureIndex = textureIndex,
                    TilingFactor = quadProperties.TilingFactor, Color = quadProperties.Color,
                    TextureCoordinate = Renderer2DData.QuadTextureCoordinates[i]
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