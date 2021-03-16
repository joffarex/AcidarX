using AcidarX.Core;
using Microsoft.Extensions.Logging;

namespace AcidarX.Sandbox
{
    public class Application : AXApplication
    {
        private static readonly ILogger<Application> Logger = AXLogger.CreateLogger<Application>();
    }
}