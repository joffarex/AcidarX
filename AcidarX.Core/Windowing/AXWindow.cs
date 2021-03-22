using System;
using System.Numerics;
using AcidarX.Core.Events;
using AcidarX.Core.Input;
using Microsoft.Extensions.Logging;
using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;

namespace AcidarX.Core.Windowing
{
    public class AXWindow
    {
        private static readonly ILogger<AXWindow> Logger = AXLogger.CreateLogger<AXWindow>();

        public AXWindow(AXWindowOptions axWindowOptions)
        {
            var windowOptions = WindowOptions.Default;
            windowOptions.Title = axWindowOptions.Title;
            windowOptions.Size = new Vector2D<int>(axWindowOptions.Width, axWindowOptions.Height);
            windowOptions.VSync = axWindowOptions.VSync;
            windowOptions.ShouldSwapAutomatically = true;
            windowOptions.WindowBorder = WindowBorder.Resizable;
            NativeWindow = Window.Create(windowOptions);

            Logger.Assert(NativeWindow != null, "Could not initialize Common Window");

            NativeWindow.Load += OnLoadContext;
            NativeWindow.Load += OnLoad;
            NativeWindow.Resize += OnResize;
            NativeWindow.Update += OnUpdate;
            NativeWindow.Render += OnRender;
            NativeWindow.Closing += OnClose;

            Gl = NativeWindow.CreateOpenGL();
        }

        public Action<Event> EventCallback { get; set; }

        public IWindow NativeWindow { get; }
        public GL Gl { get; }

        public IInputContext InputContext { get; private set; }

        private void OnLoadContext()
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

        private void OnKeyChar(IKeyboard keyboard, char keyChar)
        {
            var keyTypedEvent = new KeyTypedEvent(keyChar);
            EventCallback(keyTypedEvent);
        }

        public void Run()
        {
            NativeWindow.Run();
        }
    }
}