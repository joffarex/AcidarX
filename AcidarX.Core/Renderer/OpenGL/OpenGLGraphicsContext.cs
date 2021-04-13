using AcidarX.Kernel.Profiling;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;

namespace AcidarX.Core.Renderer.OpenGL
{
    public class OpenGLGraphicsContext : GraphicsContext
    {
        private readonly IWindow _window;

        public OpenGLGraphicsContext(IWindow window) => _window = window;

        public static GL Gl { get; private set; }


        public override void Init()
        {
            AXProfiler.Capture(() => { Gl = GL.GetApi(_window); });
        }
    }
}