using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace AcidarX.Core.Logging
{
    public static class AXLoggerExtensions
    {
        [Conditional("DEBUG")]
        public static void Assert<T>(this ILogger<T> logger, bool condition, string message)
        {
            if (!condition)
            {
                logger.LogError("Assertion Failed: {Message}", message);
                if (Debugger.IsAttached)
                {
                    logger.LogError(new StackTrace().ToString());
                    Debugger.Break();
                }
            }
        }
    }
}