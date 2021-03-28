using System;
using Microsoft.Extensions.Logging;
using Silk.NET.OpenGL;
using static AcidarX.Core.Renderer.OpenGL.OpenGLGraphicsContext;

namespace AcidarX.Core.Renderer
{
    public sealed class Shader : IDisposable
    {
        private static readonly ILogger<Shader> Logger = AXLogger.CreateLogger<Shader>();

        private readonly RendererID _rendererID;

        private bool _isDisposed;

        public Shader(string vertexSource, string fragmentSource)
        {
            uint? vertexShader = CreateShader(ShaderType.VertexShader, vertexSource);
            if (!vertexShader.HasValue)
            {
                return;
            }

            uint? fragmentShader = CreateShader(ShaderType.FragmentShader, fragmentSource);
            if (!fragmentShader.HasValue)
            {
                return;
            }

            _rendererID = (RendererID) Gl.CreateProgram();
            Gl.AttachShader(_rendererID, vertexShader.Value);
            Gl.AttachShader(_rendererID, fragmentShader.Value);

            bool result = LinkProgram(_rendererID);
            if (!result)
            {
                Gl.DeleteShader(vertexShader.Value);
                Gl.DeleteShader(fragmentShader.Value);
                return;
            }

            Gl.DetachShader(_rendererID, vertexShader.Value);
            Gl.DetachShader(_rendererID, fragmentShader.Value);
            Gl.DeleteShader(vertexShader.Value);
            Gl.DeleteShader(fragmentShader.Value);
        }

        public void Dispose()
        {
            Logger.Assert(!_isDisposed, $"{this} is already disposed");

            _isDisposed = true;
            Dispose(true);
            GC.SuppressFinalize(this);
            GC.KeepAlive(this);
        }

        public void Bind()
        {
            Gl.UseProgram(_rendererID);
        }

        public void Unbind()
        {
            Gl.UseProgram(0);
        }

        private static uint? CreateShader(ShaderType shaderType, string shaderSource)
        {
            uint shader = Gl.CreateShader(shaderType);
            Gl.ShaderSource(shader, shaderSource);
            Gl.CompileShader(shader);

            string infoLog = Gl.GetShaderInfoLog(shader);
            if (!string.IsNullOrWhiteSpace(infoLog))
            {
                Gl.DeleteShader(shader);

                Logger.LogError(infoLog);
                Logger.Assert(false, "Error compiling vertex shader");
                return null;
            }

            return shader;
        }

        private static bool LinkProgram(uint rendererId)
        {
            Gl.LinkProgram(rendererId);

            Gl.GetProgram(rendererId, GLEnum.LinkStatus, out int status);
            if (status == 0)
            {
                string infoLog = Gl.GetProgramInfoLog(rendererId);

                Gl.DeleteProgram(rendererId);

                Logger.LogError(infoLog);
                Logger.Assert(false, "Error linking program");

                return false;
            }

            return true;
        }

        private void Dispose(bool manual)
        {
            Logger.Assert(manual, $"Memory leak detected on object: {this}");

            Gl.DeleteShader(_rendererID);
        }

        ~Shader()
        {
            Dispose(false);
        }

        public override string ToString() => string.Format("Shader|{0}", _rendererID);
    }
}