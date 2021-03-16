﻿using System;

namespace AcidarX.Core.Events
{
    // Events are going to be blocking for now, meaning once it gets created, it must be handled there. In future
    // maybe implement EventBus and buffer events there

    public enum EventType
    {
        None = 0,
        WindowClose,
        WindowResize,
        WindowFocus,
        WindowLostFocus,
        WindowMoved,

        AppTick,
        AppUpdate,
        AppRender,

        KeyPressed,
        KeyReleased,

        MouseButtonPressed,
        MouseButtonReleased,
        MouseMoved,
        MouseScrolled
    }

    public enum EventCategory
    {
        None = 0,
        Application = 1 << 0,
        Input = 1 << 1,
        Keyboard = 1 << 2,
        Mouse = 1 << 3,
        MouseButton = 1 << 4
    }


    public abstract class Event
    {
        protected bool Handled { get; set; }
        public EventType EventType { get; protected set; }
        public string Name => EventType.ToString("G");

        public static EventType GetStaticType<T>() where T : Event
        {
            return typeof(T).Name switch
            {
                nameof(KeyPressedEvent) => EventType.KeyPressed,
                nameof(KeyReleasedEvent) => EventType.KeyReleased,
                nameof(MouseMovedEvent) => EventType.MouseMoved,
                nameof(MouseScrolledEvent) => EventType.MouseMoved,
                nameof(MouseButtonPressedEvent) => EventType.MouseButtonPressed,
                nameof(MouseButtonReleasedEvent) => EventType.MouseButtonReleased,
                nameof(WindowResizeEvent) => EventType.WindowResize,
                nameof(WindowCloseEvent) => EventType.WindowClose,
                nameof(AppTickEvent) => EventType.AppTick,
                nameof(AppUpdateEvent) => EventType.AppUpdate,
                nameof(AppRenderEvent) => EventType.AppRender,
                _ => throw new Exception("Something went wrong")
            };
        }

        public abstract int GetCategoryFlags();
        public override string ToString() => Name;

        public bool IsInCategory(EventCategory category) => (GetCategoryFlags() & (int) category) != 0;

        public class EventDispatcher
        {
            public EventDispatcher(Event @event) => Event = @event;

            public Event Event { get; }

            public bool Dispatch<T>(Func<T, bool> func) where T : Event
            {
                if (Event.EventType != GetStaticType<T>())
                {
                    return false;
                }

                Event.Handled = func((T) Event);
                return true;
            }
        }
    }
}