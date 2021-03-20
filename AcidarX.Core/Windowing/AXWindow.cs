using System;
using Microsoft.Extensions.Logging;
using Silk.NET.Maths;
using Silk.NET.Windowing;

namespace AcidarX.Core.Windowing
{
    public class AXWindow
    {
        private static readonly ILogger<AXWindow> Logger = AXLogger.CreateLogger<AXWindow>();
        private readonly IWindow _window;

        public AXWindow(AXWindowOptions axWindowOptions)
        {
            var windowOptions = WindowOptions.Default;
            windowOptions.Title = axWindowOptions.Title;
            windowOptions.Size = new Vector2D<int>(axWindowOptions.Width, axWindowOptions.Height);
            windowOptions.VSync = axWindowOptions.VSync;
            windowOptions.ShouldSwapAutomatically = true;
            windowOptions.WindowBorder = WindowBorder.Resizable;
            _window = Window.Create(windowOptions);

            Logger.Assert(_window != null, "Could not initialize Common Window");

            _window.Closing += OnClose;
        }

        public void SetOnLoad(Action onLoad)
        {
            _window.Load += onLoad;
        }

        public void SetOnResize(Action<Vector2D<int>> onResize)
        {
            _window.Resize += onResize;
        }


        public void SetOnUpdate(Action<double> onUpdate)
        {
            _window.Update += onUpdate;
        }

        public void SetOnRender(Action<double> onRender)
        {
            _window.Render += onRender;
        }

        public void Run()
        {
            _window.Run();
        }

        public void OnClose()
        {
            Logger.LogInformation("Closing Window...");
        }
    }
}