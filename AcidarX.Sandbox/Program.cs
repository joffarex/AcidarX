using AcidarX.Core.Hosting;
using AcidarX.Core.Profiling;
using AcidarX.Core.Windowing;
using AcidarX.Sandbox;

Instrumentation.Instance.BeginSession("Startup", "profile-Startup.json");
var windowOptions = AXWindowOptions.CreateDefault();
var application = AXHostApplication.Create(windowOptions);
Instrumentation.Instance.EndSession();
Instrumentation.Instance.BeginSession("Runtime", "profile-Runtime.json");
application.PushLayer<Sandbox2DLayer>();
application.Run();
Instrumentation.Instance.EndSession();
Instrumentation.Instance.BeginSession("Shutdown", "profile-Shutdown.json");
application.Dispose();
Instrumentation.Instance.EndSession();