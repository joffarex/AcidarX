using AcidarX.Core.Windowing;
using Microsoft.Extensions.Logging;
using Silk.NET.Maths;

namespace AcidarX.Core
{
    public abstract class AXApplication
    {
        private static readonly ILogger<AXApplication> Logger = AXLogger.CreateLogger<AXApplication>();
        private readonly AXWindow _window;

        protected AXApplication(AXWindowOptions axWindowOptions)
        {
            _window = new AXWindow(axWindowOptions);
            _window.SetOnLoad(OnLoad);
            _window.SetOnResize(OnResize);
            _window.SetOnUpdate(OnUpdate);
            _window.SetOnRender(OnRender);
        }

        protected virtual void OnLoad()
        {
        }

        protected virtual void OnResize(Vector2D<int> size)
        {
        }

        protected virtual void OnUpdate(double dt)
        {
        }

        protected virtual void OnRender(double dt)
        {
        }

        public void Run()
        {
            _window.Run();
        }
    }
}