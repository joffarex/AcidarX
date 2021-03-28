using System.Linq;
using AcidarX.Core.Events;
using AcidarX.Core.Input;
using AcidarX.Core.Layers;
using AcidarX.Core.Renderer;
using AcidarX.Core.Windowing;
using Microsoft.Extensions.Logging;
using Silk.NET.OpenGL;
using static AcidarX.Core.Renderer.OpenGL.OpenGLGraphicsContext;

namespace AcidarX.Core
{
    public abstract class AXApplication
    {
        private const string VertexShaderSource = @"
        #version 330 core
        layout (location = 0) in vec3 a_Position;
        
        out vec3 v_Position;

        void main()
        {
            v_Position = a_Position;
            gl_Position = vec4(a_Position, 1.0);
        }
        ";

        private const string FragmentShaderSource = @"
        #version 330 core

        layout (location = 0) out vec4 color;

        in vec3 v_Position;

        void main()
        {
            color = vec4(v_Position * 0.5f + 0.5f, 1.0f);
        }
        ";

        private static readonly ILogger<AXApplication> Logger = AXLogger.CreateLogger<AXApplication>();

        private static VertexBuffer _vertexBuffer;
        private static IndexBuffer _indexBuffer;
        private static uint _vertexArray;
        private static AXShader _shader;

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

        private unsafe bool OnLoad(AppLoadEvent e)
        {
            Logger.Assert(Gl != null, "OpenGL context has not been initialized");

            _imGuiLayer = new ImGuiLayer(Gl, _window.NativeWindow, _window.InputContext);
            PushLayer(_imGuiLayer);

            _vertexArray = Gl.GenVertexArray();
            Gl.BindVertexArray(_vertexArray);

            _vertexBuffer = BufferFactory.CreateVertexBuffer(Vertices);
            _indexBuffer = BufferFactory.CreateIndexBuffer(Indices);

            _shader = new AXShader(VertexShaderSource, FragmentShaderSource);

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
            _window.GraphicsContext.Clear();
            _window.GraphicsContext.ClearColor();

            foreach (Layer layer in _layers)
            {
                layer.OnRender(e.DeltaTime);
            }

            _shader.Bind();
            Gl.BindVertexArray(_vertexArray);

            Gl.DrawElements(PrimitiveType.Triangles, _indexBuffer.GetCount(),
                DrawElementsType.UnsignedInt, null);

            _shader.Unbind();

            _imGuiLayer.Begin(e.DeltaTime);
            foreach (Layer layer in _layers)
            {
                layer.OnImGuiRender();
            }

            _imGuiLayer.End();

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
            _shader.Dispose();

            return true; // Handled
        }
    }
}