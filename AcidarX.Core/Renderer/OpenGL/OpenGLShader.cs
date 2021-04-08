using System;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using AcidarX.Core.Logging;
using AcidarX.Core.Profiling;
using Microsoft.Extensions.Logging;
using Silk.NET.OpenGL;

namespace AcidarX.Core.Renderer.OpenGL
{
    public struct UniformFieldData
    {
        public int Location;
        public string Name;
        public int Size;
        public UniformType Type;
    }

    public sealed class OpenGLShader : Shader
    {
        private static readonly ILogger<OpenGLShader> Logger = AXLogger.CreateLogger<OpenGLShader>();
        private readonly GL _gl;

        private readonly RendererID _rendererID;

        private bool _isDisposed;

        private UniformFieldData[] _uniformFieldData;

        public OpenGLShader(GL gl, string vertexSource, string fragmentSource)
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

            GetUniforms();
        }

        private static UniformType ShaderDataTypeToUniformTypeMapper(ShaderDataType shaderDataType)
        {
            switch (shaderDataType)
            {
                case ShaderDataType.None: return 0;
                case ShaderDataType.Float: return UniformType.Float;
                case ShaderDataType.Float2: return UniformType.FloatVec2;
                case ShaderDataType.Float3: return UniformType.FloatVec3;
                case ShaderDataType.Float4: return UniformType.FloatVec4;
                case ShaderDataType.Mat3: return UniformType.FloatMat3;
                case ShaderDataType.Mat4: return UniformType.FloatMat4;
                case ShaderDataType.Int: return UniformType.Int;
                case ShaderDataType.Int2: return UniformType.IntVec2;
                case ShaderDataType.Int3: return UniformType.IntVec3;
                case ShaderDataType.Int4: return UniformType.IntVec4;
                case ShaderDataType.IntSamplerArr: return UniformType.Sampler2D;
                case ShaderDataType.Bool: return UniformType.Bool;
                default:
                    Logger.Assert(false, "Unknown ShaderDataType");
                    return 0;
            }
        }

        public override void Dispose()
        {
            Logger.Assert(!_isDisposed, $"{this} is already disposed");

            _isDisposed = true;
            Dispose(true);
            GC.SuppressFinalize(this);
            GC.KeepAlive(this);
        }

        private unsafe void GetUniforms()
        {
            AXProfiler.Capture(() =>
            {
                _gl.GetProgram(_rendererID, ProgramPropertyARB.ActiveUniforms, out int numberOfUniforms);

                byte* version = _gl.GetString(StringName.Version);
                string? str = Marshal.PtrToStringUTF8((IntPtr) version);


                UniformFieldData[] uniforms = new UniformFieldData[numberOfUniforms];

                for (uint i = 0; i < numberOfUniforms; i++)
                {
                    string name = _gl.GetActiveUniform(_rendererID, i, out int size, out UniformType type);
                    if (type == UniformType.Sampler2D)
                    {
                        name = name.Split("[")[0];
                    }

                    int location = _gl.GetUniformLocation(_rendererID, name);
                    UniformFieldData fieldData;
                    fieldData.Location = location;
                    fieldData.Name = name;
                    fieldData.Size = size;
                    fieldData.Type = type;

                    uniforms[i] = fieldData;
                }

                _uniformFieldData = uniforms;
            });
        }

        public override void Bind()
        {
            AXProfiler.Capture(() => { _gl.UseProgram(_rendererID); });
        }

        public override void Unbind()
        {
            AXProfiler.Capture(() => { _gl.UseProgram(0); });
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
                Logger.Assert(false, $"Error compiling {shaderType} shader");
                return null;
            }

            return shader;
        }

        public unsafe void UploadMatrix4(string name, Matrix4x4 data)
        {
            Logger.Assert(_uniformFieldData.Any(fieldData => fieldData.Name == name),
                $"{name} uniform not found on a shader.");
            Logger.Assert(_uniformFieldData.Any(
                    fieldData => fieldData.Type == ShaderDataTypeToUniformTypeMapper(ShaderDataType.Mat4)),
                $"${name} has invalid type.");

            _gl.UniformMatrix4(_uniformFieldData.First(fieldData => fieldData.Name == name).Location, 1, false,
                (float*) &data);
        }

        public void UploadFloat(string name, float data)
        {
            Logger.Assert(_uniformFieldData.Any(fieldData => fieldData.Name == name),
                $"{name} uniform not found on a shader.");
            Logger.Assert(_uniformFieldData.Any(
                    fieldData => fieldData.Type == ShaderDataTypeToUniformTypeMapper(ShaderDataType.Mat4)),
                $"${name} has invalid type.");

            _gl.Uniform1(_uniformFieldData.First(fieldData => fieldData.Name == name).Location, data);
        }

        public void UploadFloat2(string name, Vector2 data)
        {
            Logger.Assert(_uniformFieldData.Any(fieldData => fieldData.Name == name),
                $"{name} uniform not found on a shader.");
            Logger.Assert(_uniformFieldData.Any(
                    fieldData => fieldData.Type == ShaderDataTypeToUniformTypeMapper(ShaderDataType.Mat4)),
                $"${name} has invalid type.");

            _gl.Uniform2(_uniformFieldData.First(fieldData => fieldData.Name == name).Location, data);
        }

        public void UploadFloat3(string name, Vector3 data)
        {
            Logger.Assert(_uniformFieldData.Any(fieldData => fieldData.Name == name),
                $"{name} uniform not found on a shader.");
            Logger.Assert(_uniformFieldData.Any(
                    fieldData => fieldData.Type == ShaderDataTypeToUniformTypeMapper(ShaderDataType.Mat4)),
                $"${name} has invalid type.");

            _gl.Uniform3(_uniformFieldData.First(fieldData => fieldData.Name == name).Location, data);
        }

        public void UploadFloat4(string name, Vector4 data)
        {
            Logger.Assert(_uniformFieldData.Any(fieldData => fieldData.Name == name),
                $"{name} uniform not found on a shader.");
            Logger.Assert(_uniformFieldData.Any(
                    fieldData => fieldData.Type == ShaderDataTypeToUniformTypeMapper(ShaderDataType.Mat4)),
                $"${name} has invalid type.");

            _gl.Uniform4(_uniformFieldData.First(fieldData => fieldData.Name == name).Location, data);
        }

        public unsafe void UploadIntArray(string name, int[] data)
        {
            Logger.Assert(_uniformFieldData.Any(fieldData => fieldData.Name == name),
                $"{name} uniform not found on a shader.");
            Logger.Assert(_uniformFieldData.Any(
                    fieldData => fieldData.Type == ShaderDataTypeToUniformTypeMapper(ShaderDataType.IntSamplerArr)),
                $"${name} has invalid type.");

            var count = (uint) data.Length;
            fixed (int* d = &data[0])
            {
                _gl.Uniform1(_uniformFieldData.First(fieldData => fieldData.Name == name).Location, count, d);
            }
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

        protected override void Dispose(bool manual)
        {
            Logger.Assert(manual, $"Memory leak detected on object: {this}");


            AXProfiler.Capture(
                () => { _gl.DeleteShader(_rendererID); });
        }

        ~OpenGLShader()
        {
            Dispose(false);
        }

        public override string ToString() => $"Shader|{_rendererID}";
    }
}