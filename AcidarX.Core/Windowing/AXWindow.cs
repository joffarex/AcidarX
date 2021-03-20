using System;
using System.Numerics;
using AcidarX.Core.Events;
using Microsoft.Extensions.Logging;
using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.Windowing;

namespace AcidarX.Core.Windowing
{
    public class AXWindow
    {
        private static readonly ILogger<AXWindow> Logger = AXLogger.CreateLogger<AXWindow>();
        private readonly IWindow _window;

        public Action<Event> EventCallback;

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

            _window.Load += OnLoadContext;
            _window.Load += OnLoad;
            _window.Resize += OnResize;
            _window.Update += OnUpdate;
            _window.Render += OnRender;
            _window.Closing += OnClose;
        }

        public void SetEventCallback(Action<Event> onEvent)
        {
            EventCallback = onEvent;
        }

        private void OnLoadContext()
        {
            IInputContext inputContext = _window.CreateInput();

            foreach (IKeyboard keyboard in inputContext.Keyboards)
            {
                keyboard.KeyDown += OnKeyPressed;
                keyboard.KeyUp += OnKeyReleased;
            }

            foreach (IMouse mouse in inputContext.Mice)
            {
                mouse.MouseDown += OnMousePressed;
                mouse.MouseUp += OnMouseReleased;
                mouse.MouseMove += OnMouseMove;
                mouse.Scroll += OnMouseScroll;
            }
        }

        private void OnLoad()
        {
            var appLoadEvent = new AppLoadEvent();
            EventCallback(appLoadEvent);
        }

        private void OnMousePressed(IMouse mouse, MouseButton mouseButton)
        {
            var mousePressedEvent = new MouseButtonPressedEvent((int) mouseButton);
            EventCallback(mousePressedEvent);
        }

        private void OnMouseReleased(IMouse mouse, MouseButton mouseButton)
        {
            var mouseReleasedEvent = new MouseButtonReleasedEvent((int) mouseButton);
            EventCallback(mouseReleasedEvent);
        }

        private void OnMouseScroll(IMouse mouse, ScrollWheel offset)
        {
            var mouseScrolledEvent = new MouseScrollEvent(new Vector2(offset.X, offset.Y));
            EventCallback(mouseScrolledEvent);
        }

        private void OnMouseMove(IMouse mouse, Vector2 mousePos)
        {
            var mouseMovedEvent = new MouseMoveEvent(mousePos);
            EventCallback(mouseMovedEvent);
        }

        private void OnResize(Vector2D<int> size)
        {
            var windowResizeEvent = new WindowResizeEvent(size);
            EventCallback(windowResizeEvent);
        }

        private void OnClose()
        {
            var windowCloseEvent = new WindowCloseEvent();
            EventCallback(windowCloseEvent);
        }

        private void OnUpdate(double deltaTime)
        {
            var windowUpdateEvent = new AppUpdateEvent(deltaTime);
            EventCallback(windowUpdateEvent);
        }

        private void OnRender(double deltaTime)
        {
            var windowRenderEvent = new AppRenderEvent(deltaTime);
            EventCallback(windowRenderEvent);
        }

        private void OnKeyPressed(IKeyboard keyboard, Key key, int keyCode)
        {
            var keyPressedEvent = new KeyPressedEvent(keyCode, 0);
            EventCallback(keyPressedEvent);
        }

        private void OnKeyReleased(IKeyboard keyboard, Key key, int keyCode)
        {
            var keyReleasedEvent = new KeyReleasedEvent(keyCode);
            EventCallback(keyReleasedEvent);
        }

        public void Run()
        {
            _window.Run();
        }
    }
}