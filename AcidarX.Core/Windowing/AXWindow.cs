using System;
using AcidarX.Core.Events;
using AcidarX.Core.Input;
using AcidarX.Kernel.Logging;
using Microsoft.Extensions.Logging;
using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.Windowing;

namespace AcidarX.Core.Windowing
{
    public partial class AXWindow : IDisposable
    {
        private static readonly ILogger<AXWindow> Logger = AXLogger.CreateLogger<AXWindow>();
        private readonly AXWindowOptions _axWindowOptions;

        public AXWindow(AXWindowOptions axWindowOptions)
        {
            _axWindowOptions = axWindowOptions;

            var windowOptions = WindowOptions.Default;
            windowOptions.Title = _axWindowOptions.Title;
            windowOptions.Size = new Vector2D<int>(_axWindowOptions.Width, _axWindowOptions.Height);
            windowOptions.VSync = _axWindowOptions.VSync;
            windowOptions.ShouldSwapAutomatically = true;
            windowOptions.WindowBorder = WindowBorder.Resizable;
            windowOptions.API = new GraphicsAPI(ContextAPI.OpenGL, new APIVersion(4, 5));
            NativeWindow = Window.Create(windowOptions);

            Logger.Assert(NativeWindow != null, "Could not initialize Common Window");

            NativeWindow.Load += InitInputContextOnLoad;
            NativeWindow.Load += OnLoad;
            NativeWindow.Resize += OnResize;
            NativeWindow.Update += OnUpdate;
            NativeWindow.Render += OnRender;
            NativeWindow.Closing += OnClose;
        }

        public Action<Event> EventCallback { get; set; }

        public IInputContext InputContext { get; private set; }

        public IWindow NativeWindow { get; }

        public void Dispose()
        {
            InputContext?.Dispose();
            NativeWindow?.Dispose();
        }

        private void InitInputContextOnLoad()
        {
            InputContext = NativeWindow.CreateInput();

            foreach (IKeyboard keyboard in InputContext.Keyboards)
            {
                keyboard.KeyDown += OnKeyPressed;
                keyboard.KeyUp += OnKeyReleased;
                keyboard.KeyChar += OnKeyChar;
                KeyboardState.AddKeyboard(keyboard);
            }

            foreach (IMouse mouse in InputContext.Mice)
            {
                mouse.MouseDown += OnMousePressed;
                mouse.MouseUp += OnMouseReleased;
                mouse.MouseMove += OnMouseMove;
                mouse.Scroll += OnMouseScroll;
                MouseState.AddMouse(mouse);
            }
        }

        public void Run()
        {
            NativeWindow.Run();
        }
    }
}