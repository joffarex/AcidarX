using System;
using AcidarX.Core.Layers;
using AcidarX.Core.Windowing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace AcidarX.Core.Hosting
{
    public class AXHostApplication : IDisposable
    {
        private readonly IHost _host;

        public AXHostApplication(AXWindowOptions windowOptions)
        {
            _host = AXHost.Create(windowOptions).Build();
            _host.Start();
        }

        private AXApplication Application => _host.Services.GetRequiredService<AXApplication>();
        private LayerFactory LayerFactory => _host.Services.GetRequiredService<LayerFactory>();

        public void Dispose()
        {
            Application.Dispose();
            _host.StopAsync().GetAwaiter().GetResult();
            _host.Dispose();
        }

        public static AXHostApplication Create(AXWindowOptions windowOptions) => new(windowOptions);

        public void PushLayer<T>() where T : Layer
        {
            Application.PushLayer(LayerFactory.CreateLayer<T>());
        }

        public void Run()
        {
            Application.Run();
        }
    }
}