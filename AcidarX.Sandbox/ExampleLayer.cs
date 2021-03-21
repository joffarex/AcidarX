using AcidarX.Core;
using AcidarX.Core.Events;
using AcidarX.Core.Layers;
using Microsoft.Extensions.Logging;

namespace AcidarX.Sandbox
{
    public class ExampleLayer : Layer
    {
        private static readonly ILogger<ExampleLayer> Logger = AXLogger.CreateLogger<ExampleLayer>();

        public ExampleLayer() : base("Example layer")
        {
        }

        public override void OnAttach()
        {
        }

        public override void OnDetach()
        {
        }

        public override void OnUpdate(double deltaTime)
        {
            Logger.LogTrace("OnUpdate");
        }

        public override void OnRender(double deltaTime)
        {
            Logger.LogTrace("OnRender");
        }

        public override void OnEvent(Event e)
        {
            Logger.LogTrace($"{e} from examplelayer");
        }

        public override void Dispose(bool manual)
        {
        }
    }
}