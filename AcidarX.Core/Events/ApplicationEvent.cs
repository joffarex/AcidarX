using System.Numerics;

namespace AcidarX.Core.Events
{
    public class WindowResizeEvent : Event
    {
        public WindowResizeEvent(Vector2 size)
        {
            Size = size;
            EventType = GetStaticType<WindowResizeEvent>();
        }

        public Vector2 Size { get; }

        public override int GetCategoryFlags() => (int) EventCategory.Application;

        public override string ToString() => string.Format("WindowResizeEvent: {0},{1}", Size.X, Size.Y);
    }

    public class WindowCloseEvent : Event
    {
        public WindowCloseEvent() => EventType = GetStaticType<WindowCloseEvent>();

        public override int GetCategoryFlags() => (int) EventCategory.Application;

        public override string ToString() => "WindowCloseEvent";
    }

    public class AppTickEvent : Event
    {
        public AppTickEvent() => EventType = GetStaticType<AppTickEvent>();

        public override int GetCategoryFlags() => (int) EventCategory.Application;

        public override string ToString() => "AppTickEvent";
    }

    public class AppUpdateEvent : Event
    {
        public AppUpdateEvent() => EventType = GetStaticType<AppUpdateEvent>();

        public override int GetCategoryFlags() => (int) EventCategory.Application;

        public override string ToString() => "AppUpdateEvent";
    }

    public class AppRenderEvent : Event
    {
        public AppRenderEvent() => EventType = GetStaticType<AppRenderEvent>();

        public override int GetCategoryFlags() => (int) EventCategory.Application;

        public override string ToString() => "AppRenderEvent";
    }
}