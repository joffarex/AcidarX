using System.Threading;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Core;
using Serilog.Events;

namespace AcidarX.Core
{
    public class AXLogger
    {
        private const string OutputTemplate = "[{Timestamp:HH:mm}] [{SourceContext}] [{Level}] ({ThreadId}) {Message}{NewLine}{Exception}";

        private static readonly Serilog.Core.Logger SerilogLogger = new LoggerConfiguration().MinimumLevel.Verbose().MinimumLevel
            .Override("Microsoft", LogEventLevel.Information)
            .Enrich.With(new ThreadEnricher())
            .WriteTo.Console(outputTemplate: OutputTemplate)
            .CreateLogger();

        public static ILogger<T> CreateLogger<T>() where T : class
        {
            return new LoggerFactory().AddSerilog(SerilogLogger).CreateLogger<T>();
        }
    }

    public class ThreadEnricher : ILogEventEnricher
    {
        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            logEvent.AddPropertyIfAbsent(
                propertyFactory.CreateProperty("TheadId", Thread.CurrentThread.ManagedThreadId));
        }
    }
}