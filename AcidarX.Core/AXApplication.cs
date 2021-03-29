using System;
using System.Linq;
using AcidarX.Core.Events;
using AcidarX.Core.Input;
using AcidarX.Core.Layers;
using AcidarX.Core.Logging;
using AcidarX.Core.Renderer;
using AcidarX.Core.Utils;
using AcidarX.Core.Windowing;
using Microsoft.Extensions.Logging;
using Silk.NET.Maths;

namespace AcidarX.Core
{
    public class AXApplication : IDisposable
    {
        private static readonly ILogger<AXApplication> Logger = AXLogger.CreateLogger<AXApplication>();

        private readonly GraphicsFactory _graphicsFactory;
        private readonly LayerStack _layers;
        private readonly RenderCommandDispatcher _renderCommandDispatcher;
        private readonly AXWindow _window;

        private ImGuiLayer _imGuiLayer;

        public AXApplication
            (AXWindow window, RenderCommandDispatcher renderCommandDispatcher, GraphicsFactory graphicsFactory)
        {
            _layers = new LayerStack();

            _window = window;
            _window.EventCallback = OnEvent;

            _renderCommandDispatcher = renderCommandDispatcher;
            _graphicsFactory = graphicsFactory;
        }

        public void Dispose()
        {
            _window?.Dispose();
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
            _imGuiLayer = _graphicsFactory.CreateImGuiLayer(_window.NativeWindow, _window.InputContext);
            PushLayer(_imGuiLayer);

            foreach (Layer layer in _layers)
            {
                layer.OnLoad();
            }

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
            _renderCommandDispatcher.SetClearColor(new Vector4D<float>(24.0f / 255.0f, 24.0f / 255.0f,
                24.0f / 255.0f, 1.0f));
            _renderCommandDispatcher.Clear();

            foreach (Layer layer in _layers)
            {
                layer.OnRender(e.DeltaTime);
            }

            _renderCommandDispatcher.Dispatch();

            // This is currently not tied to our renderer api
            _imGuiLayer.Begin(e.DeltaTime);
            foreach (Layer layer in _layers)
            {
                layer.OnImGuiRender();
            }

            FpsUtils.ImGuiWindow(e.DeltaTime);

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

        private bool OnWindowClose(WindowCloseEvent e)
        {
            foreach (Layer layer in _layers)
            {
                layer.Dispose();
            }

            return true;
        }
    }
}