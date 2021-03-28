using Microsoft.Extensions.Logging;

namespace AcidarX.Core.Renderer
{
    public readonly struct BufferElement
    {
        private static readonly ILogger<BufferElement> Logger = AXLogger.CreateLogger<BufferElement>();

        public string Name { get; }
        public ShaderDataType Type { get; }
        public uint Size { get; }
        public uint Offset { get; }
        public bool Normalized { get; }


        public BufferElement(string name, ShaderDataType type)
        {
            Name = name;
            Type = type;
            Size = ShaderDataTypeSize(type);
            Offset = 0;
            Normalized = false;
        }

        public BufferElement(string name, ShaderDataType type, uint offset, bool normalized = false)
        {
            Name = name;
            Type = type;
            Size = ShaderDataTypeSize(type);
            Offset = offset;
            Normalized = normalized;
        }

        /// <summary>
        ///     Returns shader input "in" variable component count a.k.a size of how many elements it has
        ///     Example: if input variable looks like this on CPU: 0.5f, 0.5f, 0.0f .. this means that component count must be
        ///     three
        /// </summary>
        public int GetComponentCount()
        {
            switch (Type)
            {
                case ShaderDataType.Float: return 1;
                case ShaderDataType.Float2: return 2;
                case ShaderDataType.Float3: return 3;
                case ShaderDataType.Float4: return 4;
                case ShaderDataType.Mat3: return 3 * 3;
                case ShaderDataType.Mat4: return 4 * 4;
                case ShaderDataType.Int: return 1;
                case ShaderDataType.Int2: return 2;
                case ShaderDataType.Int3: return 3;
                case ShaderDataType.Int4: return 4;
                case ShaderDataType.Bool: return 1;
                case ShaderDataType.None: return 0;
                default:
                    Logger.Assert(false, "Unknown ShaderDataType");
                    return 0;
            }
        }

        public static uint ShaderDataTypeSize(ShaderDataType type)
        {
            switch (type)
            {
                case ShaderDataType.Float: return sizeof(float);
                case ShaderDataType.Float2: return sizeof(float) * 2;
                case ShaderDataType.Float3: return sizeof(float) * 3;
                case ShaderDataType.Float4: return sizeof(float) * 4;
                case ShaderDataType.Mat3: return sizeof(float) * 3 * 3;
                case ShaderDataType.Mat4: return sizeof(float) * 4 * 4;
                case ShaderDataType.Int: return sizeof(float);
                case ShaderDataType.Int2: return sizeof(float) * 2;
                case ShaderDataType.Int3: return sizeof(float) * 3;
                case ShaderDataType.Int4: return sizeof(float) * 4;
                case ShaderDataType.Bool: return sizeof(bool);
                case ShaderDataType.None: return 0;
                default:
                    Logger.Assert(false, "Unknown ShaderDataType");
                    return 0;
            }
        }
    }
}