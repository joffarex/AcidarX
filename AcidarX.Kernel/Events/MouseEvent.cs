using System.Numerics;
using AcidarX.Kernel.Input;

namespace AcidarX.Kernel.Events
{
    public class MouseMoveEvent : Event
    {
        public MouseMoveEvent(Vector2 mousePos)
        {
            MousePos = mousePos;
            EventType = GetStaticType<MouseMoveEvent>();
        }

        public Vector2 MousePos { get; }

        public override int GetCategoryFlags() => (int) EventCategory.Mouse | (int) EventCategory.Input;

        public override string ToString() => $"MouseMoveEvent: {MousePos.X},{MousePos.Y}";
    }

    public class MouseScrollEvent : Event
    {
        public MouseScrollEvent(Vector2 offset)
        {
            Offset = offset;
            EventType = GetStaticType<MouseScrollEvent>();
        }

        public Vector2 Offset { get; }

        public override int GetCategoryFlags() => (int) EventCategory.Mouse | (int) EventCategory.Input;

        public override string ToString() => $"MouseScrollEvent: {Offset.X},{Offset.Y}";
    }

    public abstract class MouseButtonEvent : Event
    {
        public MouseButtonEvent
            (int buttonCode, AXMouseButton mouseButton) => (ButtonCode, MouseButton) = (buttonCode, mouseButton);

        public int ButtonCode { get; }
        public AXMouseButton MouseButton { get; }

        public override int GetCategoryFlags() =>
            (int) EventCategory.Mouse | (int) EventCategory.Input | (int) EventCategory.MouseButton;
    }

    public class MouseButtonPressedEvent : MouseButtonEvent
    {
        public MouseButtonPressedEvent
            (int buttonCode, AXMouseButton mouseButton) : base(buttonCode, mouseButton) =>
            EventType = GetStaticType<MouseButtonPressedEvent>();

        public override string ToString() => $"MouseBeingPressedEvent: {ButtonCode}";
    }

    public class MouseButtonReleasedEvent : MouseButtonEvent
    {
        public MouseButtonReleasedEvent
            (int buttonCode, AXMouseButton mouseButton) : base(buttonCode, mouseButton) =>
            EventType = GetStaticType<MouseButtonReleasedEvent>();

        public override string ToString() => $"MouseBeingPressedEvent: {ButtonCode}";
    }
}