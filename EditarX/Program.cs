using AcidarX.Core.Hosting;
using AcidarX.Core.Profiling;
using AcidarX.Core.Windowing;
using EditarX;

Instrumentation.Instance.BeginSession("Startup", "profile-Startup.json");
var windowOptions = new AXWindowOptions() {Title = "EditarX", Width = 1920, Height = 1080, VSync = true};
var application = AXHostApplication.Create(windowOptions);
Instrumentation.Instance.EndSession();
Instrumentation.Instance.BeginSession("Runtime", "profile-Runtime.json");
application.PushLayer<EditorLayer>();
application.Run();
Instrumentation.Instance.EndSession();
Instrumentation.Instance.BeginSession("Shutdown", "profile-Shutdown.json");
application.Dispose();
Instrumentation.Instance.EndSession();