using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Silk.NET.Input;

namespace AcidarX.Core.Input
{
    public static class MouseState
    {
        private static readonly List<IMouse> Mice = new();

        public static void AddMouse(IMouse mouse)
        {
            Mice.Add(mouse);
        }

        public static bool IsButtonPressed
            (AXMouseButton mouseButton) => Mice.Select(mouse => mouse.IsButtonPressed(AXInputCodeMapper.AXMouseButtonToSilkMouseButton(mouseButton))).FirstOrDefault();

        public static Vector2 GetMousePosition() => Mice.Select(mouse => mouse.Position).FirstOrDefault();
    }
}