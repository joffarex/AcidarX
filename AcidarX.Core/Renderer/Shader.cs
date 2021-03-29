using System;
using AcidarX.Core.Logging;
using Microsoft.Extensions.Logging;
using Silk.NET.OpenGL;

namespace AcidarX.Core.Renderer
{
    public sealed class Shader : IDisposable
    {
        private static readonly ILogger<Shader> Logger = AXLogger.CreateLogger<Shader>();
        private readonly GL _gl;

        private readonly RendererID _rendererID;

        private bool _isDisposed;

        public Shader(GL gl, string vertexSource, string fragmentSource)
        {
            _gl = gl;
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

            _rendererID = (RendererID) _gl.CreateProgram();
            _gl.AttachShader(_rendererID, vertexShader.Value);
            _gl.AttachShader(_rendererID, fragmentShader.Value);

            bool result = LinkProgram(_rendererID);
            if (!result)
            {
                _gl.DeleteShader(vertexShader.Value);
                _gl.DeleteShader(fragmentShader.Value);
                return;
            }

            _gl.DetachShader(_rendererID, vertexShader.Value);
            _gl.DetachShader(_rendererID, fragmentShader.Value);
            _gl.DeleteShader(vertexShader.Value);
            _gl.DeleteShader(fragmentShader.Value);
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
            _gl.UseProgram(_rendererID);
        }

        public void Unbind()
        {
            _gl.UseProgram(0);
        }

        private uint? CreateShader(ShaderType shaderType, string shaderSource)
        {
            uint shader = _gl.CreateShader(shaderType);
            _gl.ShaderSource(shader, shaderSource);
            _gl.CompileShader(shader);

            string infoLog = _gl.GetShaderInfoLog(shader);
            if (!string.IsNullOrWhiteSpace(infoLog))
            {
                _gl.DeleteShader(shader);

                Logger.LogError(infoLog);
                Logger.Assert(false, "Error compiling vertex shader");
                return null;
            }

            return shader;
        }

        private bool LinkProgram(uint rendererId)
        {
            _gl.LinkProgram(rendererId);

            _gl.GetProgram(rendererId, GLEnum.LinkStatus, out int status);
            if (status == 0)
            {
                string infoLog = _gl.GetProgramInfoLog(rendererId);

                _gl.DeleteProgram(rendererId);

                Logger.LogError(infoLog);
                Logger.Assert(false, "Error linking program");

                return false;
            }

            return true;
        }

        private void Dispose(bool manual)
        {
            Logger.Assert(manual, $"Memory leak detected on object: {this}");

            _gl.DeleteShader(_rendererID);
        }

        ~Shader()
        {
            Dispose(false);
        }

        public override string ToString() => string.Format("Shader|{0}", _rendererID);
    }
}