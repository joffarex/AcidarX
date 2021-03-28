using AcidarX.Core;
using AcidarX.Core.Layers;
using AcidarX.Core.Windowing;
using AcidarX.Sandbox;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var windowOptions = AXWindowOptions.CreateDefault();
using IHost host = Entrypoint.CreateAXHost(windowOptions).ConfigureServices(services =>
{
    ServiceProvider provider = services.BuildServiceProvider();

    var application = provider.GetRequiredService<AXApplication>();
    var layerFactory = provider.GetRequiredService<LayerFactory>();

    application.PushLayer(layerFactory.CreateLayer<ExampleLayer>());
    application.Run();
}).Build();
host.Start();