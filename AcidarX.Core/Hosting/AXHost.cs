using AcidarX.Core.Layers;
using AcidarX.Core.Windowing;
using AcidarX.Graphics;
using AcidarX.Graphics.Renderer;
using AcidarX.Graphics.Renderer.OpenGL;
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
            // TODO: fix disposal order
            return serviceCollection
                .AddSingleton<LayerStack>()
                .AddSingleton(axWindowOptions)
                .AddSingleton<AXWindow>()
                .AddSingleton(provider => provider.GetRequiredService<AXWindow>().NativeWindow)
                .AddSingleton(provider => GL.GetApi(provider.GetRequiredService<IWindow>()))
                .AddSingleton<RendererAPI>(provider => new OpenGLRendererAPI(provider.GetRequiredService<GL>()))
                .AddSingleton<RenderCommandDispatcher>()
                .AddSingleton<GraphicsFactory>()
                .AddSingleton<AssetManager>()
                // .AddSingleton<AXRenderer>()
                .AddSingleton<AXRenderer2D>()
                .AddSingleton<LayerStack>()
                .AddSingleton<LayerFactory>()
                .AddSingleton<AXApplication>();
        }

        public static IHostBuilder Create(AXWindowOptions options) => Host.CreateDefaultBuilder()
            .ConfigureServices(services => services.AddAcidarX(options));
    }
}