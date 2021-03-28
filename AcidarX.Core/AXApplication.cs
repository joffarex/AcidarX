using System.Collections.Generic;
using System.Linq;
using AcidarX.Core.Events;
using AcidarX.Core.Input;
using AcidarX.Core.Layers;
using AcidarX.Core.Renderer;
using AcidarX.Core.Windowing;
using Microsoft.Extensions.Logging;
using Silk.NET.Maths;

namespace AcidarX.Core
{
    public class AXApplication
    {
        private const string SquareVertexShaderSource = @"
        #version 330 core
        layout (location = 0) in vec3 a_Position;
        layout (location = 1) in vec4 a_Color;
        
        out vec3 v_Position;
        out vec4 v_Color;

        void main()
        {
            v_Position = a_Position;
            v_Color = a_Color;
            gl_Position = vec4(a_Position, 1.0);
        }
        ";

        private const string SquareFragmentShaderSource = @"
        #version 330 core

        layout (location = 0) out vec4 color;

        in vec3 v_Position;
        in vec4 v_Color;

        void main()
        {
            color = v_Color;
        }
        ";

        private const string TriangleVertexShaderSource = @"
        #version 330 core
        layout (location = 0) in vec3 a_Position;
        
        out vec3 v_Position;

        void main()
        {
            v_Position = a_Position;
            gl_Position = vec4(a_Position, 1.0);
        }
        ";

        private const string TriangleFragmentShaderSource = @"
        #version 330 core

        layout (location = 0) out vec4 color;

        in vec3 v_Position;

        void main()
        {
            color = vec4(0.4f, 1.0f, 0.8f, 1.0f);
        }
        ";

        private static readonly ILogger<AXApplication> Logger = AXLogger.CreateLogger<AXApplication>();

        private static VertexArray _squareVertexArray;
        private static Shader _squareShader;

        private static VertexArray _triangleVertexArray;
        private static Shader _triangleShader;
        private readonly GraphicsFactory _graphicsFactory;

        private readonly LayerStack _layers;
        private readonly AXRenderer _renderer;
        private readonly AXWindow _window;
        private ImGuiLayer _imGuiLayer;

        public AXApplication(AXWindowOptions axWindowOptions, AXRenderer renderer, GraphicsFactory graphicsFactory)
        {
            _layers = new LayerStack();

            _window = new AXWindow(axWindowOptions);
            _window.EventCallback = OnEvent;

            _renderer = renderer;
            _graphicsFactory = graphicsFactory;
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

        private bool OnLoad(AppLoadEvent e)
        {
            _imGuiLayer = new ImGuiLayer(_graphicsFactory.Gl, _window.NativeWindow, _window.InputContext);
            PushLayer(_imGuiLayer);

            #region square

            _squareVertexArray = _graphicsFactory.CreateVertexArray();

            float[] squareVertices =
            {
                //X    Y      Z
                0.5f, 0.5f, 0.0f, 0.8f, 0.0f, 1.0f, 1.0f,
                0.5f, -0.5f, 0.0f, 0.6f, 0.0f, 1.0f, 1.0f,
                -0.5f, -0.5f, 0.0f, 0.4f, 0.0f, 1.0f, 1.0f,
                -0.5f, 0.5f, 0.5f, 0.2f, 0.0f, 1.0f, 1.0f
            };

            VertexBuffer squareVertexBuffer = _graphicsFactory.CreateVertexBuffer(squareVertices);
            squareVertexBuffer.SetLayout(new BufferLayout(new List<BufferElement>
            {
                new("a_Position", ShaderDataType.Float3),
                new("a_Color", ShaderDataType.Float4)
            }));
            _squareVertexArray.AddVertexBuffer(squareVertexBuffer);

            uint[] squareIndices =
            {
                0, 1, 3,
                1, 2, 3
            };
            IndexBuffer squareIndexBuffer = _graphicsFactory.CreateIndexBuffer(squareIndices);
            _squareVertexArray.SetIndexBuffer(squareIndexBuffer);

            _squareShader = new Shader(_graphicsFactory.Gl, SquareVertexShaderSource, SquareFragmentShaderSource);

            #endregion

            #region triangle

            _triangleVertexArray = _graphicsFactory.CreateVertexArray();

            float[] triangleVertices =
            {
                //X    Y      Z
                -0.5f, -0.5f, 0.0f,
                0.5f, -0.5f, 0.0f,
                0.0f, 0.5f, 0.0f
            };

            VertexBuffer triangleVertexBuffer = _graphicsFactory.CreateVertexBuffer(triangleVertices);
            triangleVertexBuffer.SetLayout(new BufferLayout(new List<BufferElement>
            {
                new("a_Position", ShaderDataType.Float3)
            }));
            _triangleVertexArray.AddVertexBuffer(triangleVertexBuffer);

            uint[] triangleIndices =
            {
                0, 1, 2
            };
            IndexBuffer triangleIndexBuffer = _graphicsFactory.CreateIndexBuffer(triangleIndices);
            _triangleVertexArray.SetIndexBuffer(triangleIndexBuffer);

            _triangleShader = new Shader(_graphicsFactory.Gl, TriangleVertexShaderSource, TriangleFragmentShaderSource);

            #endregion


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

        private bool OnRender(AppRenderEvent e)
        {
            _renderer.RenderCommandDispatcher.SetClearColor(new Vector4D<float>(24.0f / 255.0f, 24.0f / 255.0f,
                24.0f / 255.0f, 1.0f));
            _renderer.RenderCommandDispatcher.Clear();

            _renderer.BeginScene();

            _squareShader.Bind();
            _renderer.Submit(_squareVertexArray);

            _triangleShader.Bind();
            _renderer.Submit(_triangleVertexArray);

            _renderer.EndScene();

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
            _squareShader.Dispose();
            _squareVertexArray.Dispose();

            return true;
        }
    }
}