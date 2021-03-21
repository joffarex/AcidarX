using System.Linq;
using AcidarX.Core.Events;
using AcidarX.Core.Layers;
using AcidarX.Core.Windowing;
using Microsoft.Extensions.Logging;

namespace AcidarX.Core
{
    public abstract class AXApplication
    {
        private static readonly ILogger<AXApplication> Logger = AXLogger.CreateLogger<AXApplication>();
        private readonly LayerStack _layers;
        private readonly AXWindow _window;

        protected AXApplication(AXWindowOptions axWindowOptions)
        {
            _layers = new LayerStack();

            _window = new AXWindow(axWindowOptions);
            _window.SetEventCallback(OnEvent);
        }

        private void OnEvent(Event e)
        {
            var eventDispatcher = new EventDispatcher(e);
            eventDispatcher.Dispatch<WindowCloseEvent>(OnWindowClose);
            eventDispatcher.Dispatch<AppLoadEvent>(OnLoad);
            eventDispatcher.Dispatch<AppUpdateEvent>(OnUpdate);
            eventDispatcher.Dispatch<AppRenderEvent>(OnRender);

            foreach (Layer layer in _layers.Reverse())
            {
                layer.OnEvent(e);
                if (e.Handled)
                {
                    break;
                }
            }
        }

        public void PushLayer(Layer layer)
        {
            _layers.PushLayer(layer);
        }

        public void PushOverlay(Layer overlay)
        {
            _layers.PushOverlay(overlay);
        }

        public void Run()
        {
            _window.Run();
        }

        private static bool OnLoad(AppLoadEvent e)
        {
            Logger.LogInformation($"[{e}]: WOAH, LOADED THIS MF!");
            return true;
        }

        private bool OnUpdate(AppUpdateEvent e)
        {
            foreach (Layer layer in _layers)
            {
                layer.OnUpdate(e.DeltaTime);
            }

            return true;
        }

        private bool OnRender(AppRenderEvent e)
        {
            foreach (Layer layer in _layers)
            {
                layer.OnRender(e.DeltaTime);
            }

            return true;
        }

        private static bool OnWindowClose(WindowCloseEvent e)
        {
            Logger.LogInformation($"[{e}]: Closing Window... WOW REPEATED!!!");
            return true; // Handled
        }
    }
}