using System.Collections.Generic;
using System.Linq;
using Silk.NET.Input;

namespace AcidarX.Kernel.Input
{
    public static class KeyboardState
    {
        private static readonly List<IKeyboard> Keyboards = new();

        public static void AddKeyboard(IKeyboard keyboard)
        {
            Keyboards.Add(keyboard);
        }

        public static bool IsKeyPressed
            (AXKey key) => Keyboards.Select(keyboard => keyboard.IsKeyPressed(AXInputCodeMapper.AXKeyToSilkKey(key)))
            .FirstOrDefault();
    }
}