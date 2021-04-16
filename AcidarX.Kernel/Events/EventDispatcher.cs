using System;

namespace AcidarX.Kernel.Events
{
    public class EventDispatcher
    {
        public EventDispatcher(Event e) => Event = e;

        public Event Event { get; }

        public bool Dispatch<T>(Func<T, bool> func) where T : Event
        {
            if (Event.EventType != Event.GetStaticType<T>())
            {
                return false;
            }

            Event.Handled = func((T) Event);
            return true;
        }
    }
}