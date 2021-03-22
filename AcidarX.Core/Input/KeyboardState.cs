using System.Collections.Generic;
using System.Linq;
using Silk.NET.Input;

namespace AcidarX.Core.Input
{
    public static class KeyboardState
    {
        private static readonly List<IKeyboard> Keyboards = new();

        public static void AddKeyboard(IKeyboard keyboard)
        {
            Keyboards.Add(keyboard);
        }

        public static bool IsKeyPressed
            (Key key) => Keyboards.Select(keyboard => keyboard.IsKeyPressed(key)).FirstOrDefault();
    }
}