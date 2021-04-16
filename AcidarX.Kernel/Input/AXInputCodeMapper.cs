using Silk.NET.Input;

namespace AcidarX.Kernel.Input
{
    /// <summary>
    ///     Basically, the idea of having `AXKey` and `AXMouseButton` is to abstract away SILK.NET dependency to the client
    ///     applications.
    /// </summary>
    public static class AXInputCodeMapper
    {
        public static Key AXKeyToSilkKey(AXKey key) => (Key) key;

        public static MouseButton AXMouseButtonToSilkMouseButton
            (AXMouseButton mouseButton) => (MouseButton) mouseButton;

        public static AXKey SilkKeyToAXKey(Key key) => (AXKey) key;

        public static AXMouseButton SilkMouseButtonToAXMouseButton
            (MouseButton mouseButton) => (AXMouseButton) mouseButton;
    }
}