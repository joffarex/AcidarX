namespace AcidarX.Core.Renderer
{
    public sealed class AXRenderer
    {
        private readonly RenderCommandDispatcher _renderCommandDispatcher;

        public AXRenderer
            (RenderCommandDispatcher renderCommandDispatcher) => _renderCommandDispatcher = renderCommandDispatcher;

        public static API API => RendererAPI.API;

        public void BeginScene()
        {
        }

        public void UseShader(Shader shader)
        {
            _renderCommandDispatcher.UseShader(shader);
        }

        public void Submit(VertexArray vertexArray)
        {
            vertexArray.Bind();
            _renderCommandDispatcher.DrawIndexed(vertexArray);
        }

        public void EndScene()
        {
        }
    }
}