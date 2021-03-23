using System.Linq;
using AcidarX.Core.Events;
using AcidarX.Core.Input;
using AcidarX.Core.Layers;
using AcidarX.Core.Renderer.OpenGL;
using AcidarX.Core.Windowing;
using Microsoft.Extensions.Logging;

namespace AcidarX.Core
{
    public abstract class AXApplication
    {
        private static readonly ILogger<AXApplication> Logger = AXLogger.CreateLogger<AXApplication>();
        private readonly LayerStack _layers;
        private readonly AXWindow _window;
        private ImGuiLayer _imGuiLayer;

        protected AXApplication(AXWindowOptions axWindowOptions)
        {
            _layers = new LayerStack();

            _window = new AXWindow(axWindowOptions);
            _window.Init();
            _window.EventCallback = OnEvent;
        }

        private void OnEvent(Event e)
        {
            var eventDispatcher = new EventDispatcher(e);
            eventDispatcher.Dispatch<WindowCloseEvent>(OnWindowClose);
            eventDispatcher.Dispatch<AppLoadEvent>(OnLoad);
            eventDispatcher.Dispatch<AppUpdateEvent>(OnUpdate);
            eventDispatcher.Dispatch<AppRenderEvent>(OnRender);
            eventDispatcher.Dispatch<KeyPressedEvent>(OnKeyPressed);

            foreach (Layer layer in _layers.Reverse())
            {
                if (e.Handled)
                {
                    break;
                }

                layer.OnEvent(e);
            }
        }

        public void PushLayer(Layer layer)
        {
            _layers.PushLayer(layer);
            layer.OnAttach();
        }

        public void PushOverlay(Layer overlay)
        {
            _layers.PushOverlay(overlay);
            overlay.OnAttach();
        }

        public void Run()
        {
            _window.Run();
        }

        private bool OnLoad(AppLoadEvent e)
        {
            Logger.Assert(OpenGLGraphicsContext.Gl != null, "OpenGL context has not been initialized");

            _imGuiLayer = new ImGuiLayer(OpenGLGraphicsContext.Gl, _window.NativeWindow, _window.InputContext);
            PushLayer(_imGuiLayer);

            return true;
        }

        private bool OnUpdate(AppUpdateEvent e)
        {
            _window.GraphicsContext.Clear();
            _window.GraphicsContext.ClearColor();

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

            _imGuiLayer.Begin(e.DeltaTime);
            foreach (Layer layer in _layers)
            {
                layer.OnImGuiRender();
            }

            _imGuiLayer.End();

            return true;
        }

        private bool OnKeyPressed(KeyPressedEvent e)
        {
            if (e.Key == AXKey.Escape)
            {
                _window.NativeWindow.Close();
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