using System.Numerics;

namespace AcidarX.Graphics.Renderer
{
    public static class Renderer2DData
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
}