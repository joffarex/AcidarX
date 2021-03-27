using System.Linq;
using AcidarX.Core.Events;
using AcidarX.Core.Input;
using AcidarX.Core.Layers;
using AcidarX.Core.Windowing;
using Microsoft.Extensions.Logging;
using Silk.NET.OpenGL;
using static AcidarX.Core.Renderer.OpenGL.OpenGLGraphicsContext;

namespace AcidarX.Core
{
    public abstract class AXApplication
    {
        private static readonly ILogger<AXApplication> Logger = AXLogger.CreateLogger<AXApplication>();
        private readonly LayerStack _layers;
        private readonly AXWindow _window;
        private ImGuiLayer _imGuiLayer;

        protected AXApplication(AXWindowOptions axWindowOptions)
        {
            _layers = new LayerStack();

            _window = new AXWindow(axWindowOptions);
            _window.Init();
            _window.EventCallback = OnEvent;
        }

        private void OnEvent(Event e)
        {
            var eventDispatcher = new EventDispatcher(e);
            eventDispatcher.Dispatch<WindowCloseEvent>(OnWindowClose);
            eventDispatcher.Dispatch<AppLoadEvent>(OnLoad);
            eventDispatcher.Dispatch<AppUpdateEvent>(OnUpdate);
            eventDispatcher.Dispatch<AppRenderEvent>(OnRender);
            eventDispatcher.Dispatch<KeyPressedEvent>(OnKeyPressed);

            foreach (Layer layer in _layers.Reverse())
            {
                if (e.Handled)
                {
                    break;
                }

                layer.OnEvent(e);
            }
        }

        public void PushLayer(Layer layer)
        {
            _layers.PushLayer(layer);
            layer.OnAttach();
        }

        public void PushOverlay(Layer overlay)
        {
            _layers.PushOverlay(overlay);
            overlay.OnAttach();
        }

        public void Run()
        {
            _window.Run();
        }

        private static uint _vertexBuffer;
        private static uint _indexBuffer;
        private static uint _vertexArray;
        private static uint _shader;

        private const string VertexShaderSource = @"
        #version 330 core //Using version GLSL version 3.3
        layout (location = 0) in vec4 vPos;
        
        void main()
        {
            gl_Position = vec4(vPos.x, vPos.y, vPos.z, 1.0);
        }
        ";

        private const string FragmentShaderSource = @"
        #version 330 core
        out vec4 FragColor;
        void main()
        {
            FragColor = vec4(1.0f, 0.5f, 0.2f, 1.0f);
        }
        ";

        //Vertex data, uploaded to the VBO.
        private static readonly float[] Vertices =
        {
            //X    Y      Z
            0.5f, 0.5f, 0.0f,
            0.5f, -0.5f, 0.0f,
            -0.5f, -0.5f, 0.0f,
            -0.5f, 0.5f, 0.5f
        };

        //Index data, uploaded to the EBO.
        private static readonly uint[] Indices =
        {
            0, 1, 3,
            1, 2, 3
        };

        private unsafe bool OnLoad(AppLoadEvent e)
        {
            Logger.Assert(Gl != null, "OpenGL context has not been initialized");

            _imGuiLayer = new ImGuiLayer(Gl, _window.NativeWindow, _window.InputContext);
            PushLayer(_imGuiLayer);

            _vertexArray = Gl.GenVertexArray();
            Gl.BindVertexArray(_vertexArray);

            _vertexBuffer = Gl.GenBuffer(); 
            Gl.BindBuffer(BufferTargetARB.ArrayBuffer, _vertexBuffer); 
            fixed (void* v = &Vertices[0])
            {
                Gl.BufferData(BufferTargetARB.ArrayBuffer,
                    (nuint) (Vertices.Length * sizeof(uint)), v, BufferUsageARB.StaticDraw); 
            }

            _indexBuffer = Gl.GenBuffer();
            Gl.BindBuffer(BufferTargetARB.ElementArrayBuffer, _indexBuffer); 
            fixed (void* i = &Indices[0])
            {
                Gl.BufferData(BufferTargetARB.ElementArrayBuffer,
                    (nuint) (Indices.Length * sizeof(uint)), i, BufferUsageARB.StaticDraw);
            }

            uint vertexShader = Gl.CreateShader(ShaderType.VertexShader);
            Gl.ShaderSource(vertexShader, VertexShaderSource);
            Gl.CompileShader(vertexShader);

            string infoLog = Gl.GetShaderInfoLog(vertexShader);
            if (!string.IsNullOrWhiteSpace(infoLog))
            {
                Logger.LogError($"Error compiling vertex shader {infoLog}");
            }

            uint fragmentShader = Gl.CreateShader(ShaderType.FragmentShader);
            Gl.ShaderSource(fragmentShader, FragmentShaderSource);
            Gl.CompileShader(fragmentShader);

            infoLog = Gl.GetShaderInfoLog(fragmentShader);
            if (!string.IsNullOrWhiteSpace(infoLog))
            {
                Logger.LogError($"Error compiling fragment shader {infoLog}");
            }

            _shader = Gl.CreateProgram();
            Gl.AttachShader(_shader, vertexShader);
            Gl.AttachShader(_shader, fragmentShader);
            Gl.LinkProgram(_shader);

            Gl.GetProgram(_shader, GLEnum.LinkStatus, out var status);
            if (status == 0)
            {
                Logger.LogError($"Error linking shader {Gl.GetProgramInfoLog(_shader)}");
            }

            Gl.DetachShader(_shader, vertexShader);
            Gl.DetachShader(_shader, fragmentShader);
            Gl.DeleteShader(vertexShader);
            Gl.DeleteShader(fragmentShader);

            Gl.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float),
                null);
            Gl.EnableVertexAttribArray(0);

            return true;
        }

        private bool OnUpdate(AppUpdateEvent e)
        {
            foreach (Layer layer in _layers)
            {
                layer.OnUpdate(e.DeltaTime);
            }

            return true;
        }

        private unsafe bool OnRender(AppRenderEvent e)
        {
            foreach (Layer layer in _layers)
            {
                layer.OnRender(e.DeltaTime);
            }

            _imGuiLayer.Begin(e.DeltaTime);
            foreach (Layer layer in _layers)
            {
                layer.OnImGuiRender();
            }

            _imGuiLayer.End();

            _window.GraphicsContext.Clear();
            _window.GraphicsContext.ClearColor();

            Gl.BindVertexArray(_vertexArray);
            Gl.UseProgram(_shader);

            Gl.DrawElements(PrimitiveType.Triangles, (uint) Indices.Length,
                DrawElementsType.UnsignedInt, null);

            return true;
        }

        private bool OnKeyPressed(KeyPressedEvent e)
        {
            if (e.Key == AXKey.Escape)
            {
                _window.NativeWindow.Close();
            }

            return true;
        }

        private static bool OnWindowClose(WindowCloseEvent e)
        {
            Logger.LogInformation($"[{e}]: Closing Window... WOW REPEATED!!!");
            return true; // Handled
        }
    }
}