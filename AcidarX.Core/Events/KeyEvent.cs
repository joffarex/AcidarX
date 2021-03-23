using AcidarX.Core.Input;

namespace AcidarX.Core.Events
{
    public abstract class KeyEvent : Event
    {
        protected KeyEvent(int keyCode, AXKey key) => (KeyCode, Key) = (keyCode, key);

        public int KeyCode { get; }
        public AXKey Key { get; }

        public override int GetCategoryFlags() =>
            (int) EventCategory.Keyboard | (int) EventCategory.Input;
    }

    public class KeyPressedEvent : KeyEvent
    {
        public KeyPressedEvent(int keyCode, AXKey key, int repeatCount) : base(keyCode, key)
        {
            RepeatCount = repeatCount;
            EventType = GetStaticType<KeyPressedEvent>();
        }

        public int RepeatCount { get; }


        public override string ToString() => string.Format("KeyPressedEvent: {0} ({1} Repeats)", KeyCode, RepeatCount);
    }

    public class KeyReleasedEvent : KeyEvent
    {
        public KeyReleasedEvent
            (int keyCode, AXKey key) : base(keyCode, key) => EventType = GetStaticType<KeyReleasedEvent>();

        public override string ToString() => string.Format("KeyReleasedEvent: {0}", KeyCode);
    }

    public class KeyTypedEvent : KeyEvent
    {
        public KeyTypedEvent(int keyCode, AXKey key) : base(keyCode, key) => EventType = GetStaticType<KeyTypedEvent>();

        public override string ToString() => string.Format("KeyTypedEvent: {0}", KeyCode);
    }
}