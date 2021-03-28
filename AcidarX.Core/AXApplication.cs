using System.Collections.Generic;
using System.Linq;
using AcidarX.Core.Events;
using AcidarX.Core.Input;
using AcidarX.Core.Layers;
using AcidarX.Core.Renderer;
using AcidarX.Core.Windowing;
using Microsoft.Extensions.Logging;
using Silk.NET.OpenGL;
using static AcidarX.Core.Renderer.OpenGL.OpenGLGraphicsContext;
using Shader = AcidarX.Core.Renderer.Shader;

namespace AcidarX.Core
{
    public abstract class AXApplication
    {
        private const string VertexShaderSource = @"
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

        private const string FragmentShaderSource = @"
        #version 330 core

        layout (location = 0) out vec4 color;

        in vec3 v_Position;
        in vec4 v_Color;

        void main()
        {
            color = v_Color;
        }
        ";

        private static readonly ILogger<AXApplication> Logger = AXLogger.CreateLogger<AXApplication>();

        private static VertexBuffer _vertexBuffer;
        private static IndexBuffer _indexBuffer;
        private static uint _vertexArray;
        private static Shader _shader;

        //Vertex data, uploaded to the VBO.
        private static readonly float[] Vertices =
        {
            //X    Y      Z
            0.5f, 0.5f, 0.0f, 0.8f, 0.0f, 1.0f, 1.0f,
            0.5f, -0.5f, 0.0f, 0.6f, 0.0f, 1.0f, 1.0f,
            -0.5f, -0.5f, 0.0f, 0.4f, 0.0f, 1.0f, 1.0f,
            -0.5f, 0.5f, 0.5f, 0.2f, 0.0f, 1.0f, 1.0f
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

        public static VertexAttribPointerType ShaderDataTypeToOpenGLBaseType(ShaderDataType type)
        {
            switch (type)
            {
                case ShaderDataType.Float:
                case ShaderDataType.Float2:
                case ShaderDataType.Float3:
                case ShaderDataType.Float4:
                case ShaderDataType.Mat3:
                case ShaderDataType.Mat4: return VertexAttribPointerType.Float;
                case ShaderDataType.Int:
                case ShaderDataType.Int2:
                case ShaderDataType.Int3:
                case ShaderDataType.Int4: return VertexAttribPointerType.Int;
                case ShaderDataType.Bool: return VertexAttribPointerType.Byte;
                case ShaderDataType.None: return 0;
                default:
                    Logger.Assert(false, "Unknown ShaderDataType");
                    return 0;
            }
        }

        private unsafe bool OnLoad(AppLoadEvent e)
        {
            Logger.Assert(Gl != null, "OpenGL context has not been initialized");

            _imGuiLayer = new ImGuiLayer(Gl, _window.NativeWindow, _window.InputContext);
            PushLayer(_imGuiLayer);

            _vertexArray = Gl.GenVertexArray();
            Gl.BindVertexArray(_vertexArray);

            _vertexBuffer = BufferFactory.CreateVertexBuffer(Vertices);
            _vertexBuffer.SetLayout(new BufferLayout(new List<BufferElement>
            {
                new("a_Position", ShaderDataType.Float3),
                new("a_Color", ShaderDataType.Float4)
            }));

            _indexBuffer = BufferFactory.CreateIndexBuffer(Indices);

            _shader = new Shader(VertexShaderSource, FragmentShaderSource);

            uint index = 0;
            BufferLayout layout = _vertexBuffer.GetLayout();
            foreach (BufferElement element in layout)
            {
                Gl.EnableVertexAttribArray(index);
                Gl.VertexAttribPointer(index, element.GetComponentCount(), ShaderDataTypeToOpenGLBaseType(element.Type),
                    element.Normalized, layout.Stride, (void*) element.Offset);
                index++;
            }

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