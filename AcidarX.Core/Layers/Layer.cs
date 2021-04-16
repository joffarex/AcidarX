using System;
using AcidarX.Kernel.Events;
using AcidarX.Kernel.Logging;
using Microsoft.Extensions.Logging;

namespace AcidarX.Core.Layers
{
    public abstract class Layer : IDisposable
    {
        private static readonly ILogger<Layer> Logger = AXLogger.CreateLogger<Layer>();
        private bool _isDisposed;

        public Layer(string debugName) => DebugName = debugName;

        public string DebugName { get; }

        public void Dispose()
        {
            Logger.Assert(!_isDisposed, $"{this} is already disposed");

            _isDisposed = true;
            Dispose(true);
            // prevent the destructor from being called
            GC.SuppressFinalize(this);
            // make sure the garbage collector does not eat our object before it is properly disposed
            GC.KeepAlive(this);
        }

        public abstract void OnAttach();
        public abstract void OnLoad();
        public abstract void OnDetach();

        public virtual void OnUpdate(double deltaTime)
        {
        }

        public virtual void OnRender(double deltaTime)
        {
        }

        public virtual void OnImGuiRender(AppRenderEvent e)
        {
        }

        public virtual void OnEvent(Event e)
        {
        }

        public abstract void Dispose(bool manual);

        ~Layer()
        {
            Dispose(false);
        }
    }
}