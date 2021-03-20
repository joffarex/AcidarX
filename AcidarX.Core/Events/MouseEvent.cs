using System.Numerics;

namespace AcidarX.Core.Events
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

        public override string ToString() => string.Format("MouseMoveEvent: {0},{1}", MousePos.X, MousePos.Y);
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

        public override string ToString() => string.Format("MouseScrollEvent: {0},{1}", Offset.X, Offset.Y);
    }

    public abstract class MouseButtonEvent : Event
    {
        public MouseButtonEvent(int button) => Button = button;

        public int Button { get; }

        public override int GetCategoryFlags() =>
            (int) EventCategory.Mouse | (int) EventCategory.Input | (int) EventCategory.MouseButton;
    }

    public class MouseButtonPressedEvent : MouseButtonEvent
    {
        public MouseButtonPressedEvent
            (int button) : base(button) => EventType = GetStaticType<MouseButtonPressedEvent>();

        public override string ToString() => string.Format("MouseBeingPressedEvent: {0}", Button);
    }

    public class MouseButtonReleasedEvent : MouseButtonEvent
    {
        public MouseButtonReleasedEvent
            (int button) : base(button) => EventType = GetStaticType<MouseButtonReleasedEvent>();

        public override string ToString() => string.Format("MouseBeingPressedEvent: {0}", Button);
    }
}