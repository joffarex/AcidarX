using System.Numerics;
using AcidarX.Core.Events;
using AcidarX.Core.Input;
using Silk.NET.Input;
using Silk.NET.Maths;

namespace AcidarX.Core.Windowing
{
    public partial class AXWindow
    {
        private void OnLoad()
        {
            var appLoadEvent = new AppLoadEvent();
            EventCallback(appLoadEvent);
        }

        private void OnMousePressed(IMouse mouse, MouseButton mouseButton)
        {
            var mousePressedEvent = new MouseButtonPressedEvent((int) mouseButton,
                AXInputCodeMapper.SilkMouseButtonToAXMouseButton(mouseButton));
            EventCallback(mousePressedEvent);
        }

        private void OnMouseReleased(IMouse mouse, MouseButton mouseButton)
        {
            var mouseReleasedEvent = new MouseButtonReleasedEvent((int) mouseButton,
                AXInputCodeMapper.SilkMouseButtonToAXMouseButton(mouseButton));
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
            var keyPressedEvent = new KeyPressedEvent(keyCode, AXInputCodeMapper.SilkKeyToAXKey(key), 0);
            EventCallback(keyPressedEvent);
        }

        private void OnKeyReleased(IKeyboard keyboard, Key key, int keyCode)
        {
            var keyReleasedEvent = new KeyReleasedEvent(keyCode, AXInputCodeMapper.SilkKeyToAXKey(key));
            EventCallback(keyReleasedEvent);
        }

        private void OnKeyChar(IKeyboard keyboard, char keyChar)
        {
            var keyTypedEvent = new KeyTypedEvent(keyChar, AXInputCodeMapper.SilkKeyToAXKey((Key) keyChar));
            EventCallback(keyTypedEvent);
        }
    }
}