using AcidarX.Core;
using AcidarX.Core.Windowing;
using Microsoft.Extensions.Logging;

namespace AcidarX.Sandbox
{
    public sealed class Application : AXApplication
    {
        private static readonly ILogger<Application> Logger = AXLogger.CreateLogger<Application>();

        public Application(AXWindowOptions axWindowOptions) : base(axWindowOptions)
        {
        }
    }
}