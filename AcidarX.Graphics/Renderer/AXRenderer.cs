using System.Collections.Generic;
using System.Numerics;
using AcidarX.Graphics.Camera;

namespace AcidarX.Graphics.Renderer
{
    public struct SceneData
    {
        public Matrix4x4 ViewProjectionMatrix;
    }

    public sealed class AXRenderer
    {
        private readonly RenderCommandDispatcher _renderCommandDispatcher;
        private SceneData _sceneData;

        public AXRenderer
            (RenderCommandDispatcher renderCommandDispatcher) => _renderCommandDispatcher = renderCommandDispatcher;

        public static API API => RendererAPI.API;

        public void BeginScene(OrthographicCamera camera)
        {
            _sceneData.ViewProjectionMatrix = camera.ViewProjectionMatrix;
        }

        public void UseShader(Shader shader)
        {
            _renderCommandDispatcher.UseShader(shader);
        }

        public void UseShader(Shader shader, List<ShaderInputData> uniforms)
        {
            _renderCommandDispatcher.UseShader(shader, uniforms);
        }

        public void Submit(VertexArray vertexArray, Shader shader)
        {
            Submit(vertexArray, shader, Matrix4x4.Identity);
        }

        public void Submit(VertexArray vertexArray, Shader shader, Vector3 position)
        {
            Submit(vertexArray, shader, Matrix4x4.CreateTranslation(position));
        }

        public void Submit(VertexArray vertexArray, Shader shader, Matrix4x4 transform)
        {
            _renderCommandDispatcher.UseShader(shader, new List<ShaderInputData>
            {
                new() {Name = "u_ViewProjection", Type = ShaderDataType.Mat4, Data = _sceneData.ViewProjectionMatrix},
                new() {Name = "u_Model", Type = ShaderDataType.Mat4, Data = transform}
            });
            vertexArray.Bind();
            _renderCommandDispatcher.DrawIndexed(vertexArray);
        }

        public void Submit(VertexArray vertexArray, Shader shader, Vector4 color)
        {
            Submit(vertexArray, shader, Matrix4x4.Identity, color);
        }


        public void Submit(VertexArray vertexArray, Shader shader, Matrix4x4 transform, Vector4 color)
        {
            _renderCommandDispatcher.UseShader(shader, new List<ShaderInputData>
            {
                new() {Name = "u_ViewProjection", Type = ShaderDataType.Mat4, Data = _sceneData.ViewProjectionMatrix},
                new() {Name = "u_Model", Type = ShaderDataType.Mat4, Data = transform},
                new() {Name = "u_Color", Type = ShaderDataType.Float4, Data = color}
            });
            vertexArray.Bind();
            _renderCommandDispatcher.DrawIndexed(vertexArray);
        }

        public void Submit(VertexArray vertexArray, Shader shader, Matrix4x4 transform, Texture2D texture2D)
        {
            _renderCommandDispatcher.UseTexture2D(TextureSlot.Texture0, texture2D);
            _renderCommandDispatcher.UseShader(shader, new List<ShaderInputData>
            {
                new() {Name = "u_ViewProjection", Type = ShaderDataType.Mat4, Data = _sceneData.ViewProjectionMatrix},
                new() {Name = "u_Model", Type = ShaderDataType.Mat4, Data = transform}
            });
            vertexArray.Bind();
            _renderCommandDispatcher.DrawIndexed(vertexArray);
        }

        public void Submit
            (VertexArray vertexArray, Shader shader, Matrix4x4 transform, Vector4 color, Texture2D texture2D)
        {
            _renderCommandDispatcher.UseTexture2D(TextureSlot.Texture0, texture2D);
            _renderCommandDispatcher.UseShader(shader, new List<ShaderInputData>
            {
                new() {Name = "u_ViewProjection", Type = ShaderDataType.Mat4, Data = _sceneData.ViewProjectionMatrix},
                new() {Name = "u_Model", Type = ShaderDataType.Mat4, Data = transform},
                new() {Name = "u_Color", Type = ShaderDataType.Float4, Data = color}
            });
            vertexArray.Bind();
            _renderCommandDispatcher.DrawIndexed(vertexArray);
        }

        public void EndScene()
        {
        }
    }
}