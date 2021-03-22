using System.Numerics;
using AcidarX.ImGui;
using AcidarX.Core.Events;
using ImGuiNET;
using Microsoft.Extensions.Logging;
using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.OpenGL;

namespace AcidarX.Core.Layers
{
    public class ImGuiLayer : Layer
    {
        private static readonly ILogger<ImGuiLayer> Logger = AXLogger.CreateLogger<ImGuiLayer>();

        private readonly ImGuiController _imGuiController;

        public ImGuiLayer
            (GL gl, Vector2D<int> size) : base("ImGui layer") => _imGuiController = new ImGuiController(gl, size);

        public override void OnAttach()
        {
            string fontPath = PathUtils.GetFullPath("assets/Fonts/OpenSans-Regular.ttf");

            _imGuiController.Init(new ImGuiFontConfig {Path = fontPath, Size = 18});

            _imGuiController.SetKeyMappings();

            _imGuiController.SetPerFrameImGuiData(1f / 60f);
            _imGuiController.CreateDeviceResources();
            _imGuiController.BeginFrame();
        }

        public override void OnDetach()
        {
            _imGuiController.DestroyDeviceObjects();
        }

        public override void OnUpdate(double deltaTime)
        {
            _imGuiController.Update((float) deltaTime);
        }

        public override void OnRender(double deltaTime)
        {
            ImGui.ShowDemoWindow();

            _imGuiController.Render();
        }

        private bool OnMouseButtonPressedEvent(MouseButtonPressedEvent e)
        {
            ImGuiIOPtr io = ImGui.GetIO();
            io.MouseDown[e.Button] = true;

            // We want other layers to handle this
            return false;
        }

        private bool OnMouseButtonReleasedEvent(MouseButtonReleasedEvent e)
        {
            ImGuiIOPtr io = ImGui.GetIO();
            io.MouseDown[e.Button] = false;

            // We want other layers to handle this
            return false;
        }

        private bool OnMouseScrollEvent(MouseScrollEvent e)
        {
            ImGuiIOPtr io = ImGui.GetIO();
            io.MouseWheel += e.Offset.X;
            io.MouseWheelH += e.Offset.Y;

            // We want other layers to handle this
            return false;
        }

        private bool OnMouseMoveEvent(MouseMoveEvent e)
        {
            ImGuiIOPtr io = ImGui.GetIO();
            io.MousePos = e.MousePos;

            // We want other layers to handle this
            return false;
        }

        private bool OnKeyPressedEvent(KeyPressedEvent e)
        {
            ImGuiIOPtr io = ImGui.GetIO();
            io.KeysDown[e.KeyCode] = true;

            io.KeyCtrl = io.KeysDown[(int) Key.ControlLeft] || io.KeysDown[(int) Key.ControlRight];
            io.KeyAlt = io.KeysDown[(int) Key.AltLeft] || io.KeysDown[(int) Key.AltRight];
            io.KeyShift = io.KeysDown[(int) Key.ShiftLeft] || io.KeysDown[(int) Key.ShiftRight];
            io.KeySuper = io.KeysDown[(int) Key.SuperLeft] || io.KeysDown[(int) Key.SuperRight];

            // We want other layers to handle this
            return false;
        }

        private bool OnKeyReleasedEvent(KeyReleasedEvent e)
        {
            ImGuiIOPtr io = ImGui.GetIO();
            io.KeysDown[e.KeyCode] = false;

            // We want other layers to handle this
            return false;
        }

        private bool OnKeyTypedEvent(KeyTypedEvent e)
        {
            ImGuiIOPtr io = ImGui.GetIO();
            io.AddInputCharacter((uint) e.KeyCode);

            // We want other layers to handle this
            return false;
        }

        private bool OnWindowResizeEvent(WindowResizeEvent e)
        {
            ImGuiIOPtr io = ImGui.GetIO();
            io.DisplaySize = (Vector2) e.Size;
            io.DisplayFramebufferScale = Vector2.One;

            // We want other layers to handle this
            return false;
        }

        public override void OnEvent(Event e)
        {
            var eventDispatcher = new EventDispatcher(e);
            eventDispatcher.Dispatch<MouseButtonPressedEvent>(OnMouseButtonPressedEvent);
            eventDispatcher.Dispatch<MouseButtonReleasedEvent>(OnMouseButtonReleasedEvent);
            eventDispatcher.Dispatch<MouseScrollEvent>(OnMouseScrollEvent);
            eventDispatcher.Dispatch<MouseMoveEvent>(OnMouseMoveEvent);
            eventDispatcher.Dispatch<KeyPressedEvent>(OnKeyPressedEvent);
            eventDispatcher.Dispatch<KeyReleasedEvent>(OnKeyReleasedEvent);
            eventDispatcher.Dispatch<KeyTypedEvent>(OnKeyTypedEvent);
            eventDispatcher.Dispatch<WindowResizeEvent>(OnWindowResizeEvent);
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