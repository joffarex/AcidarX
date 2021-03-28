using System.Collections.Generic;
using AcidarX.Core;
using AcidarX.Core.Events;
using AcidarX.Core.Layers;
using AcidarX.Core.Renderer;
using Microsoft.Extensions.Logging;

namespace AcidarX.Sandbox
{
    public class ExampleLayer : Layer
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

        private static readonly ILogger<ExampleLayer> Logger = AXLogger.CreateLogger<ExampleLayer>();

        private static VertexArray _squareVertexArray;
        private static Shader _squareShader;

        private static VertexArray _triangleVertexArray;
        private static Shader _triangleShader;

        public ExampleLayer(GraphicsFactory graphicsFactory, AXRenderer renderer)
            : base("Example layer", graphicsFactory, renderer)
        {
        }

        public override void OnAttach()
        {
        }

        public override void OnLoad()
        {
            #region square

            _squareVertexArray = GraphicsFactory.CreateVertexArray();

            float[] squareVertices =
            {
                //X    Y      Z
                0.5f, 0.5f, 0.0f, 0.8f, 0.0f, 1.0f, 1.0f,
                0.5f, -0.5f, 0.0f, 0.6f, 0.0f, 1.0f, 1.0f,
                -0.5f, -0.5f, 0.0f, 0.4f, 0.0f, 1.0f, 1.0f,
                -0.5f, 0.5f, 0.5f, 0.2f, 0.0f, 1.0f, 1.0f
            };

            VertexBuffer squareVertexBuffer = GraphicsFactory.CreateVertexBuffer(squareVertices);
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
            IndexBuffer squareIndexBuffer = GraphicsFactory.CreateIndexBuffer(squareIndices);
            _squareVertexArray.SetIndexBuffer(squareIndexBuffer);

            _squareShader = new Shader(GraphicsFactory.Gl, SquareVertexShaderSource, SquareFragmentShaderSource);

            #endregion

            #region triangle

            _triangleVertexArray = GraphicsFactory.CreateVertexArray();

            float[] triangleVertices =
            {
                //X    Y      Z
                -0.5f, -0.5f, 0.0f,
                0.5f, -0.5f, 0.0f,
                0.0f, 0.5f, 0.0f
            };

            VertexBuffer triangleVertexBuffer = GraphicsFactory.CreateVertexBuffer(triangleVertices);
            triangleVertexBuffer.SetLayout(new BufferLayout(new List<BufferElement>
            {
                new("a_Position", ShaderDataType.Float3)
            }));
            _triangleVertexArray.AddVertexBuffer(triangleVertexBuffer);

            uint[] triangleIndices =
            {
                0, 1, 2
            };
            IndexBuffer triangleIndexBuffer = GraphicsFactory.CreateIndexBuffer(triangleIndices);
            _triangleVertexArray.SetIndexBuffer(triangleIndexBuffer);

            _triangleShader = new Shader(GraphicsFactory.Gl, TriangleVertexShaderSource, TriangleFragmentShaderSource);

            #endregion
        }

        public override void OnDetach()
        {
        }

        public override void OnImGuiRender()
        {
            ImGuiNET.ImGui.Begin("Test");
            ImGuiNET.ImGui.Text("AX is cool");
            ImGuiNET.ImGui.End();
        }

        public override void OnUpdate(double deltaTime)
        {
        }

        public override void OnRender(double deltaTime)
        {
            Renderer.BeginScene();

            _squareShader.Bind();
            Renderer.Submit(_squareVertexArray);

            _triangleShader.Bind();
            Renderer.Submit(_triangleVertexArray);

            Renderer.EndScene();
        }

        public override void OnEvent(Event e)
        {
            // Logger.LogTrace($"{e} from examplelayer");
        }

        public override void Dispose(bool manual)
        {
            _squareShader.Dispose();
            _squareVertexArray.Dispose();
            _triangleShader.Dispose();
            _triangleVertexArray.Dispose();
        }
    }
}