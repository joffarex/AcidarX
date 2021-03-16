using Microsoft.Extensions.Logging;

namespace AcidarX.Core
{
    abstract public class AXApplication
    {
        private static readonly ILogger<AXApplication> Logger = AXLogger.CreateLogger<AXApplication>();
        
        public void Run()
        {
        }
    }
}