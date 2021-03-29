using AcidarX.Core.Logging;
using AcidarX.ImGui;
using Microsoft.Extensions.Logging;
using Silk.NET.Input;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;

namespace AcidarX.Core.Layers
{
    public class ImGuiLayer : Layer
    {
        private static readonly ILogger<ImGuiLayer> Logger = AXLogger.CreateLogger<ImGuiLayer>();

        private readonly ImGuiController _imGuiController;

        public ImGuiLayer
            (GL gl, IWindow window, IInputContext inputContext) : base("ImGui layer") =>
            _imGuiController = new ImGuiController(gl, window, inputContext);

        public override void OnAttach()
        {
            string fontPath = PathUtils.GetFullPath("assets/Fonts/OpenSans-Regular.ttf");

            _imGuiController.Init(new ImGuiFontConfig {Path = fontPath, Size = 18});
        }

        public override void OnLoad()
        {
        }

        public override void OnDetach()
        {
            _imGuiController.DestroyDeviceObjects();
        }

        public override void OnImGuiRender()
        {
            ImGuiNET.ImGui.ShowDemoWindow();
        }

        public void Begin(double deltaTime)
        {
            _imGuiController.Update((float) deltaTime);
        }

        public void End()
        {
            _imGuiController.Render();
        }


        public override void Dispose(bool manual)
        {
            if (!manual)
            {
                return;
            }

            _imGuiController.DestroyDeviceObjects();
        }
    }
}