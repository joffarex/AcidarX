using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using ImGuiNET;
using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.OpenGL;

namespace AcidareX.ImGui
{
    public class ImGuiController : IDisposable
    {
        private Texture _fontTexture;
        private bool _frameBegun;
        private readonly GL _gl;
        private readonly Version _glVersion;
        private uint _indexBuffer;
        private uint _indexBufferSize;
        private Shader _shader;


        private readonly Vector2D<int> _size;

        // OpenGL objects
        private uint _vertexArray;
        private uint _vertexBuffer;
        private uint _vertexBufferSize;

        /// <summary>
        ///     Constructs a new ImGuiController.
        /// </summary>
        public ImGuiController(GL gl, Vector2D<int> size)
        {
            _gl = gl;
            _glVersion = new Version(gl.GetInteger(GLEnum.MajorVersion), gl.GetInteger(GLEnum.MinorVersion));
            _size = size;
        }

        /// <summary>
        ///     Frees all graphics resources used by the renderer.
        /// </summary>
        public void Dispose()
        {
            _fontTexture.Dispose();
            _shader.Dispose();
        }


        public void Init(ImGuiFontConfig fontConfig)
        {
            ImGuiNET.ImGui.CreateContext();
            ImGuiNET.ImGui.StyleColorsClassic();

            ImGuiIOPtr io = ImGuiNET.ImGui.GetIO();
            if (fontConfig.IsDefault)
            {
                io.Fonts.AddFontDefault();
            }
            else
            {
                io.Fonts.AddFontFromFileTTF(fontConfig.Path, fontConfig.Size);
            }

            io.BackendFlags |= ImGuiBackendFlags.RendererHasVtxOffset;
            io.BackendFlags |= ImGuiBackendFlags.HasMouseCursors;
            io.BackendFlags |= ImGuiBackendFlags.HasSetMousePos;
        }

        public void BeginFrame()
        {
            ImGuiNET.ImGui.NewFrame();
            _frameBegun = true;
        }

        public void Render()
        {
            if (_frameBegun)
            {
                _frameBegun = false;
                ImGuiNET.ImGui.Render();
                RenderImDrawData(ImGuiNET.ImGui.GetDrawData());
            }
        }

        /// <summary>
        ///     Updates ImGui input and IO configuration state.
        /// </summary>
        public void Update(float deltaSeconds)
        {
            _gl.Clear((uint) ClearBufferMask.ColorBufferBit);

            if (_frameBegun)
            {
                ImGuiNET.ImGui.Render();
            }

            SetPerFrameImGuiData(deltaSeconds);

            _frameBegun = true;
            ImGuiNET.ImGui.NewFrame();
        }

        public void DestroyDeviceObjects()
        {
            Dispose();
        }

        public unsafe void CreateDeviceResources()
        {
            _gl.CreateVertexArray("ImGui", out _vertexArray);

            _vertexBufferSize = 10000;
            _indexBufferSize = 2000;

            _gl.CreateVertexBuffer("ImGui", out _vertexBuffer);
            _gl.CreateElementBuffer("ImGui", out _indexBuffer);
            _gl.NamedBufferData(_vertexBuffer, _vertexBufferSize, null, VertexBufferObjectUsage.DynamicDraw);
            _gl.NamedBufferData(_indexBuffer, _indexBufferSize, null, VertexBufferObjectUsage.DynamicDraw);

            RecreateFontDeviceTexture();

            var vertexSource = @"#version 330 core
uniform mat4 projection_matrix;
in vec2 in_position;
in vec2 in_texCoord;
in vec4 in_color;
out vec4 color;
out vec2 texCoord;
void main()
{
    gl_Position = projection_matrix * vec4(in_position, 0, 1);
    color = in_color;
    texCoord = in_texCoord;
}";
            var fragmentSource = @"#version 330 core
uniform sampler2D in_fontTexture;
in vec4 color;
in vec2 texCoord;
out vec4 outputColor;
void main()
{
    outputColor = color * texture(in_fontTexture, texCoord);
}";
            _shader = new Shader(_gl, "ImGui", vertexSource, fragmentSource);

            _gl.VertexArrayVertexBuffer(_vertexArray, 0, _vertexBuffer, IntPtr.Zero,
                (uint) Unsafe.SizeOf<ImDrawVert>());
            _gl.VertexArrayElementBuffer(_vertexArray, _indexBuffer);

            _gl.EnableVertexArrayAttrib(_vertexArray, 0);
            _gl.VertexArrayAttribBinding(_vertexArray, 0, 0);
            _gl.VertexArrayAttribFormat(_vertexArray, 0, 2, VertexAttribType.Float, false, 0);

            _gl.EnableVertexArrayAttrib(_vertexArray, 1);
            _gl.VertexArrayAttribBinding(_vertexArray, 1, 0);
            _gl.VertexArrayAttribFormat(_vertexArray, 1, 2, VertexAttribType.Float, false, 8);

            _gl.EnableVertexArrayAttrib(_vertexArray, 2);
            _gl.VertexArrayAttribBinding(_vertexArray, 2, 0);
            _gl.VertexArrayAttribFormat(_vertexArray, 2, 4, VertexAttribType.UnsignedByte, true, 16);

            _gl.CheckGlError("End of ImGui setup");
        }

        /// <summary>
        ///     Recreates the device texture used to render text.
        /// </summary>
        private void RecreateFontDeviceTexture()
        {
            ImGuiIOPtr io = ImGuiNET.ImGui.GetIO();
            io.Fonts.GetTexDataAsRGBA32(out IntPtr pixels, out int width, out int height, out int bytesPerPixel);

            _fontTexture = new Texture(_gl, "ImGui Text Atlas", width, height, pixels);
            _fontTexture.SetMagFilter(TextureMagFilter.Linear);
            _fontTexture.SetMinFilter(TextureMinFilter.Linear);

            io.Fonts.SetTexID((IntPtr) _fontTexture.GlTexture);

            io.Fonts.ClearTexData();
        }


        /// <summary>
        ///     Sets per-frame data based on the associated window.
        ///     This is called by Update(float).
        /// </summary>
        public void SetPerFrameImGuiData(float deltaSeconds)
        {
            ImGuiIOPtr io = ImGuiNET.ImGui.GetIO();
            io.DisplaySize = new Vector2(
                _size.X / Vector2.One.X,
                _size.Y / Vector2.One.Y);
            io.DisplayFramebufferScale = Vector2.One;
            io.DeltaTime = deltaSeconds; // DeltaTime is in seconds.
        }

        public void SetKeyMappings()
        {
            ImGuiIOPtr io = ImGuiNET.ImGui.GetIO();
            io.KeyMap[(int) ImGuiKey.Tab] = (int) Key.Tab;
            io.KeyMap[(int) ImGuiKey.LeftArrow] = (int) Key.Left;
            io.KeyMap[(int) ImGuiKey.RightArrow] = (int) Key.Right;
            io.KeyMap[(int) ImGuiKey.UpArrow] = (int) Key.Up;
            io.KeyMap[(int) ImGuiKey.DownArrow] = (int) Key.Down;
            io.KeyMap[(int) ImGuiKey.PageUp] = (int) Key.PageUp;
            io.KeyMap[(int) ImGuiKey.PageDown] = (int) Key.PageDown;
            io.KeyMap[(int) ImGuiKey.Home] = (int) Key.Home;
            io.KeyMap[(int) ImGuiKey.End] = (int) Key.End;
            io.KeyMap[(int) ImGuiKey.Delete] = (int) Key.Delete;
            io.KeyMap[(int) ImGuiKey.Backspace] = (int) Key.Backspace;
            io.KeyMap[(int) ImGuiKey.Enter] = (int) Key.Enter;
            io.KeyMap[(int) ImGuiKey.Escape] = (int) Key.Escape;
            io.KeyMap[(int) ImGuiKey.A] = (int) Key.A;
            io.KeyMap[(int) ImGuiKey.C] = (int) Key.C;
            io.KeyMap[(int) ImGuiKey.V] = (int) Key.V;
            io.KeyMap[(int) ImGuiKey.X] = (int) Key.X;
            io.KeyMap[(int) ImGuiKey.Y] = (int) Key.Y;
            io.KeyMap[(int) ImGuiKey.Z] = (int) Key.Z;
        }

        public unsafe void RenderImDrawData(ImDrawDataPtr drawData)
        {
            uint vertexOffsetInVertices = 0;
            uint indexOffsetInElements = 0;

            if (drawData.CmdListsCount == 0)
            {
                return;
            }

            var totalVbSize = (uint) (drawData.TotalVtxCount * Unsafe.SizeOf<ImDrawVert>());
            if (totalVbSize > _vertexBufferSize)
            {
                var newSize = (int) Math.Max(_vertexBufferSize * 1.5f, totalVbSize);
                _gl.NamedBufferData(_vertexBuffer, (uint) newSize, null, VertexBufferObjectUsage.DynamicDraw);
                _vertexBufferSize = (uint) newSize;

                if (_glVersion >= new Version(4, 3))
                {
                    var str = $"Silk.NET ImGui: Resized vertex buffer to new size {_vertexBufferSize}";
                    _gl.DebugMessageInsert(DebugSource.DebugSourceApi, DebugType.DontCare, 1879701u,
                        DebugSeverity.DebugSeverityNotification, (uint) str.Length, str);
                }
            }

            var totalIbSize = (uint) (drawData.TotalIdxCount * sizeof(ushort));
            if (totalIbSize > _indexBufferSize)
            {
                var newSize = (int) Math.Max(_indexBufferSize * 1.5f, totalIbSize);
                _gl.NamedBufferData(_indexBuffer, (uint) newSize, null, VertexBufferObjectUsage.DynamicDraw);
                _indexBufferSize = (uint) newSize;

                if (_glVersion >= new Version(4, 3))
                {
                    var str = $"Silk.NET ImGui: Resized index buffer to new size {_indexBufferSize}";
                    _gl.DebugMessageInsert(DebugSource.DebugSourceApi, DebugType.DontCare, 1879702u,
                        DebugSeverity.DebugSeverityNotification, (uint) str.Length, str);
                }
            }


            for (var i = 0; i < drawData.CmdListsCount; i++)
            {
                ImDrawListPtr cmdList = drawData.CmdListsRange[i];

                _gl.NamedBufferSubData(_vertexBuffer, (IntPtr) (vertexOffsetInVertices * Unsafe.SizeOf<ImDrawVert>()),
                    (UIntPtr) (cmdList.VtxBuffer.Size * Unsafe.SizeOf<ImDrawVert>()), (void*) cmdList.VtxBuffer.Data);
                _gl.CheckGlError($"Data Vert {i}");
                _gl.NamedBufferSubData(_indexBuffer, (IntPtr) (indexOffsetInElements * sizeof(ushort)),
                    (UIntPtr) (cmdList.IdxBuffer.Size * sizeof(ushort)), (void*) cmdList.IdxBuffer.Data);

                _gl.CheckGlError($"Data Idx {i}");

                vertexOffsetInVertices += (uint) cmdList.VtxBuffer.Size;
                indexOffsetInElements += (uint) cmdList.IdxBuffer.Size;
            }

            // Setup orthographic projection matrix into our constant buffer
            ImGuiIOPtr io = ImGuiNET.ImGui.GetIO();
            var mvp = Matrix4x4.CreateOrthographicOffCenter(
                -1.0f,
                io.DisplaySize.X,
                io.DisplaySize.Y,
                0.0f,
                -1.0f,
                1.0f);

            _shader.UseShader();
            _gl.ProgramUniformMatrix4(_shader.Program, _shader.GetUniformLocation("projection_matrix"), 1, false,
                (float*) Unsafe.AsPointer(ref mvp));
            _gl.ProgramUniform1(_shader.Program, _shader.GetUniformLocation("in_fontTexture"), 0);
            _gl.CheckGlError("Projection");

            _gl.BindVertexArray(_vertexArray);
            _gl.CheckGlError("VAO");

            drawData.ScaleClipRects(io.DisplayFramebufferScale);

            _gl.Enable(EnableCap.Blend);
            _gl.Enable(EnableCap.ScissorTest);
            _gl.BlendEquation(BlendEquationModeEXT.FuncAdd);
            _gl.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            _gl.Disable(EnableCap.CullFace);
            _gl.Disable(EnableCap.DepthTest);

            // Render command lists
            var vtxOffset = 0;
            var idxOffset = 0;
            for (var n = 0; n < drawData.CmdListsCount; n++)
            {
                ImDrawListPtr cmdList = drawData.CmdListsRange[n];
                for (var cmdI = 0; cmdI < cmdList.CmdBuffer.Size; cmdI++)
                {
                    ImDrawCmdPtr pcmd = cmdList.CmdBuffer[cmdI];
                    if (pcmd.UserCallback != IntPtr.Zero)
                    {
                        throw new NotImplementedException();
                    }

                    _gl.ActiveTexture(TextureUnit.Texture0);
                    _gl.BindTexture(TextureTarget.Texture2D, (uint) pcmd.TextureId);
                    _gl.CheckGlError("Texture");

                    // We do _windowHeight - (int)clip.W instead of (int)clip.Y because gl has flipped Y when it comes to these coordinates
                    Vector4 clip = pcmd.ClipRect;
                    _gl.Scissor((int) clip.X, _size.Y - (int) clip.W, (uint) (clip.Z - clip.X),
                        (uint) (clip.W - clip.Y));
                    _gl.CheckGlError("Scissor");

                    _gl.DrawElementsBaseVertex(PrimitiveType.Triangles, pcmd.ElemCount, DrawElementsType.UnsignedShort,
                        (void*) (idxOffset * sizeof(ushort)), vtxOffset);
                    _gl.CheckGlError("Draw");

                    idxOffset += (int) pcmd.ElemCount;
                }

                vtxOffset += cmdList.VtxBuffer.Size;
            }

            _gl.Disable(EnableCap.Blend);
            _gl.Disable(EnableCap.ScissorTest);
        }
    }
}