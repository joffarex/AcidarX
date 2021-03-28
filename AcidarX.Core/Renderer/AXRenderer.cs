namespace AcidarX.Core.Renderer
{
    public sealed class AXRenderer
    {
        public AXRenderer
            (RenderCommandDispatcher renderCommandDispatcher) => RenderCommandDispatcher = renderCommandDispatcher;

        public RenderCommandDispatcher RenderCommandDispatcher { get; }
        public static API API => RendererAPI.API;

        public void BeginScene()
        {
        }

        public void Submit(VertexArray vertexArray)
        {
            vertexArray.Bind();
            RenderCommandDispatcher.DrawIndexed(vertexArray);
        }

        public void EndScene()
        {
        }
    }
}