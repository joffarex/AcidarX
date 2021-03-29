using AcidarX.Core.Layers;
using AcidarX.Core.Renderer;
using AcidarX.Core.Renderer.OpenGL;
using AcidarX.Core.Windowing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;

namespace AcidarX.Core.Hosting
{
    public static class AXHost
    {
        public static IServiceCollection AddAcidarX
            (this IServiceCollection serviceCollection, AXWindowOptions axWindowOptions)
        {
            return serviceCollection
                .AddSingleton<LayerStack>()
                .AddSingleton(axWindowOptions)
                .AddSingleton<AXWindow>()
                .AddSingleton(provider => provider.GetRequiredService<AXWindow>().NativeWindow)
                .AddSingleton(provider => GL.GetApi(provider.GetRequiredService<IWindow>()))
                .AddSingleton<AssetManager>()
                .AddSingleton<RendererAPI>(provider => new OpenGLRendererAPI(provider.GetRequiredService<GL>()))
                .AddSingleton<RenderCommandDispatcher>()
                .AddSingleton<GraphicsFactory>()
                .AddSingleton(provider => provider.GetRequiredService<GraphicsFactory>().CreateRenderer())
                .AddSingleton<LayerFactory>()
                .AddSingleton<AXApplication>();
        }

        public static IHostBuilder Create(AXWindowOptions options) => Host.CreateDefaultBuilder()
            .ConfigureServices(services => services.AddAcidarX(options));
    }
}