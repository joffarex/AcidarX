using System;
using System.Linq;
using AcidarX.Core.Events;
using AcidarX.Core.Input;
using AcidarX.Core.Layers;
using AcidarX.Core.Renderer;
using AcidarX.Core.Windowing;
using AcidarX.Kernel.Logging;
using AcidarX.Kernel.Profiling;
using Microsoft.Extensions.Logging;

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
        private bool _minimized;

        public AXApplication
        (
            AXWindow window, RenderCommandDispatcher renderCommandDispatcher, GraphicsFactory graphicsFactory,
            LayerStack layers
        )
        {
            _layers = layers;
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
            eventDispatcher.Dispatch<WindowResizeEvent>(OnWindowResize);
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

        private bool OnWindowResize(WindowResizeEvent e)
        {
            // Basically if window is minimized
            if (e.Size.X == 0 || e.Size.Y == 0)
            {
                _minimized = true;
                return false;
            }

            _minimized = false;

            _renderCommandDispatcher.OnWindowResize(e.Size);

            return false;
        }

        public void PushLayer(Layer layer)
        {
            AXProfiler.Capture(() =>
            {
                _layers.PushLayer(layer);
                layer.OnAttach();
            });
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
            AXProfiler.Capture(() =>
            {
                _imGuiLayer = _graphicsFactory.CreateImGuiLayer(_window.NativeWindow, _window.InputContext);
                PushLayer(_imGuiLayer);

                foreach (Layer layer in _layers)
                {
                    layer.OnLoad();
                }

                _renderCommandDispatcher.Init();
            });

            return true;
        }

        private bool OnUpdate(AppUpdateEvent e)
        {
            if (!_minimized)
            {
                AXProfiler.Capture(() =>
                {
                    foreach (Layer layer in _layers)
                    {
                        layer.OnUpdate(e.DeltaTime);
                    }
                });
            }

            return true;
        }

        private bool OnRender(AppRenderEvent e)
        {
            if (!_minimized)
            {
                AXProfiler.Capture(() =>
                {
                    AXProfiler.Capture("OnDraw", () =>
                    {
                        foreach (Layer layer in _layers)
                        {
                            layer.OnRender(e.DeltaTime);
                        }
                    });

                    AXProfiler.Capture("OnImGuiRender", () =>
                    {
                        // This is currently not tied to our renderer api
                        _imGuiLayer.Begin(e.DeltaTime);
                        foreach (Layer layer in _layers)
                        {
                            layer.OnImGuiRender(e);
                        }


                        _imGuiLayer.End();
                    });
                });
            }

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

            Renderer2DData.Dispose();

            return true;
        }
    }
}