using System.Collections.Generic;
using System.Numerics;
using AcidarX.Core.Camera;
using Silk.NET.OpenGL;

namespace AcidarX.Core.Renderer
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

        public void UseShader(Shader shader, List<UniformPublicInfo> uniforms)
        {
            _renderCommandDispatcher.UseShader(shader, uniforms);
        }

        public void Submit(VertexArray vertexArray, Shader shader)
        {
            var viewProjectionUniform = new UniformPublicInfo
            {
                Name = "u_ViewProjection",
                Type = UniformType.FloatMat4,
                Data = _sceneData.ViewProjectionMatrix
            };

            _renderCommandDispatcher.UseShader(shader, new List<UniformPublicInfo> {viewProjectionUniform});

            vertexArray.Bind();
            _renderCommandDispatcher.DrawIndexed(vertexArray);
        }

        public void EndScene()
        {
        }
    }
}