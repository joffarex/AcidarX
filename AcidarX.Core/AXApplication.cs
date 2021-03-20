using AcidarX.Core.Events;
using AcidarX.Core.Windowing;
using Microsoft.Extensions.Logging;

namespace AcidarX.Core
{
    public abstract class AXApplication
    {
        private static readonly ILogger<AXApplication> Logger = AXLogger.CreateLogger<AXApplication>();
        private readonly AXWindow _window;

        protected AXApplication(AXWindowOptions axWindowOptions)
        {
            _window = new AXWindow(axWindowOptions);
            _window.SetEventCallback(OnEvent);
        }

        private void OnEvent(Event e)
        {
            var eventDispatcher = new EventDispatcher(e);
            eventDispatcher.Dispatch<WindowCloseEvent>(OnWindowClose);
            eventDispatcher.Dispatch<AppLoadEvent>(OnLoad);

            // Logger.LogTrace($"{e}");
        }

        public void Run()
        {
            _window.Run();
        }

        private static bool OnLoad(AppLoadEvent e)
        {
            Logger.LogInformation($"[{e}]: WOAH, LOADED THIS MF!");
            return true;
        }

        private static bool OnWindowClose(WindowCloseEvent e)
        {
            Logger.LogInformation($"[{e}]: Closing Window... WOW REPEATED!!!");
            return true; // Handled
        }
    }
}