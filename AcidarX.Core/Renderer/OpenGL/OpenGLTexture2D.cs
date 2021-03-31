using System;
using System.Runtime.InteropServices;
using AcidarX.Core.Logging;
using Microsoft.Extensions.Logging;
using Silk.NET.OpenGL;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace AcidarX.Core.Renderer.OpenGL
{
    public sealed class OpenGLTexture2D : Texture2D
    {
        private static readonly ILogger<OpenGLTexture2D> Logger = AXLogger.CreateLogger<OpenGLTexture2D>();
        private readonly GL _gl;
        private readonly uint _height;
        private readonly InternalFormat _internalFormat;
        private readonly string _path;
        private readonly PixelFormat _pixelFormat;

        private readonly RendererID _rendererID;

        private readonly TextureTarget _textureTarget;

        private readonly uint _width;
        private bool _isDisposed;

        public unsafe OpenGLTexture2D(GL gl, string path, bool withAlpha = true)
        {
            _gl = gl;
            _path = path;
            _textureTarget = TextureTarget.Texture2D;
            Image<Rgba32> img = (Image<Rgba32>) Image.Load(_path);
            img.Mutate(x => x.Flip(FlipMode.Vertical));
            _width = (uint) img.Width;
            _height = (uint) img.Height;

            // TODO: Figure out every possible combination with SixLabors api and create mappings
            _internalFormat = withAlpha ? InternalFormat.Rgba : InternalFormat.Rgb;
            _pixelFormat = withAlpha ? PixelFormat.Rgba : PixelFormat.Rgb;

            fixed (void* data = &MemoryMarshal.GetReference(img.GetPixelRowSpan(0)))
            {
                _rendererID = (RendererID) _gl.GenTexture();
                // _gl.TextureStorage2D(_rendererID, 1, (GLEnum)_internalFormat, _width, _height);
                Bind();

                _gl.TexParameter(_textureTarget, TextureParameterName.TextureWrapS, (int) TextureWrapMode.Repeat);
                _gl.TexParameter(_textureTarget, TextureParameterName.TextureWrapT, (int) TextureWrapMode.Repeat);

                _gl.TexParameter(_textureTarget, TextureParameterName.TextureMinFilter, (int) TextureMinFilter.Linear);
                _gl.TexParameter(_textureTarget, TextureParameterName.TextureMagFilter, (int) TextureMagFilter.Nearest);

                _gl.TexImage2D(_textureTarget, 0, (int) _internalFormat, _width, _height, 0, _pixelFormat,
                    PixelType.UnsignedByte, data);
                _gl.GenerateMipmap(_textureTarget);
                // _gl.TextureSubImage2D(_rendererID, 0, 0, 0, _width, _height, _pixelFormat, PixelType.UnsignedByte, data);
            }

            img.Dispose();
        }

        public override void Dispose()
        {
            Logger.Assert(!_isDisposed, $"{this} is already disposed");

            _isDisposed = true;
            Dispose(true);
            GC.SuppressFinalize(this);
            GC.KeepAlive(this);
        }

        public override void Bind()
        {
            // _gl.BindTextureUnit(0);
            _gl.BindTexture(_textureTarget, _rendererID);
        }

        public override void Unbind()
        {
            _gl.BindTexture(_textureTarget, 0);
        }

        public override void Use(TextureSlot slot)
        {
            _gl.ActiveTexture(SlotToUnitMapper(slot));
            Bind();
        }

        public override unsafe void SetData(void* data)
        {
            _gl.TexSubImage2D(_textureTarget, 0, 0, 0, _width, _height, _pixelFormat, PixelType.UnsignedByte, data);
        }

        public override uint GetWidth() => _width;

        public override uint GetHeight() => _height;
        public override string GetPath() => _path;

        private static TextureUnit SlotToUnitMapper(TextureSlot slot)
        {
            switch (slot)
            {
                case TextureSlot.Texture0: return TextureUnit.Texture0;
                case TextureSlot.Texture1: return TextureUnit.Texture1;
                case TextureSlot.Texture2: return TextureUnit.Texture2;
                case TextureSlot.Texture3: return TextureUnit.Texture3;
                case TextureSlot.Texture4: return TextureUnit.Texture4;
                case TextureSlot.Texture5: return TextureUnit.Texture5;
                case TextureSlot.Texture6: return TextureUnit.Texture6;
                case TextureSlot.Texture7: return TextureUnit.Texture7;
                case TextureSlot.Texture8: return TextureUnit.Texture8;
                case TextureSlot.Texture9: return TextureUnit.Texture9;
                case TextureSlot.Texture10: return TextureUnit.Texture10;
                case TextureSlot.Texture11: return TextureUnit.Texture11;
                case TextureSlot.Texture12: return TextureUnit.Texture12;
                case TextureSlot.Texture13: return TextureUnit.Texture13;
                case TextureSlot.Texture14: return TextureUnit.Texture14;
                case TextureSlot.Texture15: return TextureUnit.Texture15;
                case TextureSlot.Texture16: return TextureUnit.Texture16;
                case TextureSlot.Texture17: return TextureUnit.Texture17;
                case TextureSlot.Texture18: return TextureUnit.Texture18;
                case TextureSlot.Texture19: return TextureUnit.Texture19;
                case TextureSlot.Texture20: return TextureUnit.Texture20;
                case TextureSlot.Texture21: return TextureUnit.Texture21;
                case TextureSlot.Texture22: return TextureUnit.Texture22;
                case TextureSlot.Texture23: return TextureUnit.Texture23;
                case TextureSlot.Texture24: return TextureUnit.Texture24;
                case TextureSlot.Texture25: return TextureUnit.Texture25;
                case TextureSlot.Texture26: return TextureUnit.Texture26;
                case TextureSlot.Texture27: return TextureUnit.Texture27;
                case TextureSlot.Texture28: return TextureUnit.Texture28;
                case TextureSlot.Texture29: return TextureUnit.Texture29;
                case TextureSlot.Texture30: return TextureUnit.Texture30;
                case TextureSlot.Texture31: return TextureUnit.Texture31;
                default:
                    Logger.Assert(false, "Unknown TextureSlot");
                    return 0;
            }
        }

        protected override void Dispose(bool manual)
        {
            Logger.Assert(manual, $"Memory leak detected on object: {this}");

            _gl.DeleteTexture(_rendererID);
        }

        ~OpenGLTexture2D()
        {
            Dispose(false);
        }

        public override string ToString() => string.Format("Texture|{0}", _rendererID);
    }
}