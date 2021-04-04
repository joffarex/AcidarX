using Silk.NET.Maths;

namespace AcidarX.Core.Events
{
    public class WindowResizeEvent : Event
    {
        public WindowResizeEvent(Vector2D<int> size)
        {
            Size = size;
            EventType = GetStaticType<WindowResizeEvent>();
        }

        public Vector2D<int> Size { get; }

        public override int GetCategoryFlags() => (int) EventCategory.Application;

        public override string ToString() => $"WindowResizeEvent: {Size.X},{Size.Y}";
    }

    public class WindowCloseEvent : Event
    {
        public WindowCloseEvent() => EventType = GetStaticType<WindowCloseEvent>();

        public override int GetCategoryFlags() => (int) EventCategory.Application;

        public override string ToString() => "WindowCloseEvent";
    }

    public class AppLoadEvent : Event
    {
        public AppLoadEvent() => EventType = GetStaticType<AppLoadEvent>();

        public override int GetCategoryFlags() => (int) EventCategory.Application;

        public override string ToString() => "AppLoadEvent";
    }

    public class AppTickEvent : Event
    {
        public AppTickEvent() => EventType = GetStaticType<AppTickEvent>();

        public override int GetCategoryFlags() => (int) EventCategory.Application;

        public override string ToString() => "AppTickEvent";
    }

    public class AppUpdateEvent : Event
    {
        public AppUpdateEvent(double deltatime)
        {
            DeltaTime = deltatime;
            EventType = GetStaticType<AppUpdateEvent>();
        }

        public double DeltaTime { get; }

        public override int GetCategoryFlags() => (int) EventCategory.Application;

        public override string ToString() => "AppUpdateEvent";
    }

    public class AppRenderEvent : Event
    {
        public AppRenderEvent(double deltatime)
        {
            DeltaTime = deltatime;
            EventType = GetStaticType<AppRenderEvent>();
        }

        public double DeltaTime { get; }

        public override int GetCategoryFlags() => (int) EventCategory.Application;

        public override string ToString() => "AppRenderEvent";
    }
}