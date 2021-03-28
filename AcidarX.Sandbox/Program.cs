using AcidarX.Core;
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
    application.PushLayer(new ExampleLayer());
    application.Run();
}).Build();
host.Run();