using AcidarX.Core;
using AcidarX.Core.Renderer;
using AcidarX.Core.Windowing;
using AcidarX.Sandbox;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var windowOptions = AXWindowOptions.CreateDefault();
IHost host = Host.CreateDefaultBuilder().ConfigureServices(x =>
{
    IServiceCollection services = x.AddAcidarX(windowOptions);

    ServiceProvider provider = services.BuildServiceProvider();

    var application = provider.GetRequiredService<AXApplication>();
    var graphicsFactory = provider.GetRequiredService<GraphicsFactory>();
    var renderer = provider.GetRequiredService<AXRenderer>();
    application.PushLayer(new ExampleLayer(graphicsFactory, renderer));
    application.Run();
}).Build();
host.Run();