using Silk.NET.Windowing;

namespace AcidarX.Core
{
    public interface IWindowProvider
    {
        public IWindow NativeWindow { get; }
    }
}