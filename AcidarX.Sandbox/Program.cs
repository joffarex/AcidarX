using AcidarX.Core.Hosting;
using AcidarX.Core.Windowing;
using AcidarX.Kernel.Profiling;
using AcidarX.Sandbox;

Instrumentation.Instance.BeginSession("Startup", "Sandbox/profile-Startup.json");
var windowOptions = AXWindowOptions.CreateDefault();
var application = AXHostApplication.Create(windowOptions);
Instrumentation.Instance.EndSession();
Instrumentation.Instance.BeginSession("Runtime", "Sandbox/profile-Runtime.json");
application.PushLayer<Sandbox2DLayer>();
application.Run();
Instrumentation.Instance.EndSession();
Instrumentation.Instance.BeginSession("Shutdown", "Sandbox/profile-Shutdown.json");
application.Dispose();
Instrumentation.Instance.EndSession();