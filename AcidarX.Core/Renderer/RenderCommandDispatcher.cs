using Silk.NET.Maths;

namespace AcidarX.Core.Renderer
{
    // TODO: submit all renderer calls from this function into a "RenderQueue"
    public sealed class RenderCommandDispatcher
    {
        private readonly RendererAPI _rendererAPI;

        public RenderCommandDispatcher(RendererAPI rendererAPI) => _rendererAPI = rendererAPI;

        public void DrawIndexed(VertexArray vertexArray)
        {
            _rendererAPI.DrawIndexed(vertexArray);
        }

        public void SetClearColor(Vector4D<float> color)
        {
            _rendererAPI.SetClearColor(color);
        }

        public void Clear()
        {
            _rendererAPI.Clear();
        }
    }
}