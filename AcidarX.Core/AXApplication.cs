using System.Numerics;
using AcidarX.Core.Events;
using Microsoft.Extensions.Logging;

namespace AcidarX.Core
{
    public abstract class AXApplication
    {
        private static readonly ILogger<AXApplication> Logger = AXLogger.CreateLogger<AXApplication>();

        public void Run()
        {
            var windowResizeEvent = new WindowResizeEvent(new Vector2(1280, 720));
            if (windowResizeEvent.IsInCategory(EventCategory.Application))
            {
                Logger.LogTrace(windowResizeEvent.ToString());
            }

            while (true)
            {
            }
        }
    }
}