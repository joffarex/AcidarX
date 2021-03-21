namespace AcidarX.Core.Events
{
    public abstract class KeyEvent : Event
    {
        protected KeyEvent(int keyCode) => KeyCode = keyCode;

        public int KeyCode { get; }

        public override int GetCategoryFlags() =>
            (int) EventCategory.Keyboard | (int) EventCategory.Input;
    }

    public class KeyPressedEvent : KeyEvent
    {
        public KeyPressedEvent(int keyCode, int repeatCount) : base(keyCode)
        {
            RepeatCount = repeatCount;
            EventType = GetStaticType<KeyPressedEvent>();
        }

        public int RepeatCount { get; }


        public override string ToString() => string.Format("KeyPressedEvent: {0} ({1} Repeats)", KeyCode, RepeatCount);
    }

    public class KeyReleasedEvent : KeyEvent
    {
        public KeyReleasedEvent(int keyCode) : base(keyCode) => EventType = GetStaticType<KeyReleasedEvent>();

        public override string ToString() => string.Format("KeyReleasedEvent: {0}", KeyCode);
    }

    public class KeyTypedEvent : KeyEvent
    {
        public KeyTypedEvent(int keyCode) : base(keyCode) => EventType = GetStaticType<KeyTypedEvent>();

        public override string ToString() => string.Format("KeyTypedEvent: {0}", KeyCode);
    }
}