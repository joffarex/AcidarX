using AcidarX.Core;
using AcidarX.Core.Windowing;
using Microsoft.Extensions.Logging;
using Silk.NET.Maths;

namespace AcidarX.Sandbox
{
    public sealed class Application : AXApplication
    {
        private static readonly ILogger<Application> Logger = AXLogger.CreateLogger<Application>();

        public Application(AXWindowOptions axWindowOptions) : base(axWindowOptions)
        {
        }

        protected override void OnLoad()
        {
            Logger.LogInformation("Loading...");

            base.OnLoad();
        }

        protected override void OnResize(Vector2D<int> size)
        {
            Logger.LogInformation($"Resizing {size.X},{size.Y}");

            base.OnResize(size);
        }

        protected override void OnUpdate(double dt)
        {
            Logger.LogInformation($"Updating {dt}");

            base.OnUpdate(dt);
        }

        protected override void OnRender(double dt)
        {
            Logger.LogInformation($"Rendering {dt}");

            base.OnRender(dt);
        }
    }
}