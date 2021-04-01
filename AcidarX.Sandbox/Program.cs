using AcidarX.Core.Hosting;
using AcidarX.Core.Windowing;
using AcidarX.Sandbox;

var windowOptions = AXWindowOptions.CreateDefault();
using var application = AXHostApplication.Create(windowOptions);
// application.PushLayer<ExampleLayer>();
application.PushLayer<Sandbox2DLayer>();
application.Run();