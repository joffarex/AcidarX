using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Silk.NET.OpenGL;
using static AcidarX.Core.Renderer.OpenGL.OpenGLGraphicsContext;

namespace AcidarX.Core.Renderer.OpenGL
{
    public sealed class OpenGLVertexArray : VertexArray
    {
        private static readonly ILogger<OpenGLVertexArray> Logger = AXLogger.CreateLogger<OpenGLVertexArray>();
        private readonly RendererID _rendererID;
        private readonly List<VertexBuffer> _vertexBuffers;
        private IndexBuffer _indexBuffer;
        private bool _isDisposed;
        private uint _vertexAttributeIndex;

        public OpenGLVertexArray()
        {
            _vertexBuffers = new List<VertexBuffer>();

            _rendererID = (RendererID) Gl.CreateVertexArray();
            Bind();
        }

        public override void Dispose()
        {
            Logger.Assert(!_isDisposed, $"{this} is already disposed");

            _isDisposed = true;
            Dispose(true);
            GC.SuppressFinalize(this);
            GC.KeepAlive(this);
        }

        public override void Bind()
        {
            Gl.BindVertexArray(_rendererID);
        }

        public override void Unbind()
        {
            Gl.BindVertexArray(0);
        }

        public override unsafe void AddVertexBuffer(VertexBuffer vertexBuffer)
        {
            Bind();
            vertexBuffer.Bind();

            BufferLayout? layout = vertexBuffer.GetLayout();
            Logger.Assert(layout.HasValue, "Layout should be initialized");

            foreach (BufferElement element in layout)
            {
                Gl.EnableVertexAttribArray(_vertexAttributeIndex);
                Gl.VertexAttribPointer(_vertexAttributeIndex, element.GetComponentCount(),
                    ShaderDataTypeToOpenGLBaseType(element.Type),
                    element.Normalized, layout.Value.Stride, (void*) element.Offset);
                _vertexAttributeIndex++;
            }

            _vertexBuffers.Add(vertexBuffer);
        }

        public override void SetIndexBuffer(IndexBuffer indexBuffer)
        {
            Bind();
            indexBuffer.Bind();
            _indexBuffer = indexBuffer;
        }

        public override IndexBuffer GetIndexBuffer() => _indexBuffer;
        public override List<VertexBuffer> GetVertexBuffers() => _vertexBuffers;

        private static VertexAttribPointerType ShaderDataTypeToOpenGLBaseType(ShaderDataType type)
        {
            switch (type)
            {
                case ShaderDataType.Float:
                case ShaderDataType.Float2:
                case ShaderDataType.Float3:
                case ShaderDataType.Float4:
                case ShaderDataType.Mat3:
                case ShaderDataType.Mat4: return VertexAttribPointerType.Float;
                case ShaderDataType.Int:
                case ShaderDataType.Int2:
                case ShaderDataType.Int3:
                case ShaderDataType.Int4: return VertexAttribPointerType.Int;
                case ShaderDataType.Bool: return VertexAttribPointerType.Byte;
                case ShaderDataType.None: return 0;
                default:
                    Logger.Assert(false, "Unknown ShaderDataType");
                    return 0;
            }
        }

        protected override void Dispose(bool manual)
        {
            Logger.Assert(manual, $"Memory leak detected on object: {this}");

            _vertexBuffers.Clear();
            Gl.DeleteVertexArrays(1, _rendererID);
        }

        public override string ToString() => string.Format("VertexArray|{0}", _rendererID);
    }
}