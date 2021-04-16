using System;
using AcidarX.Kernel.Logging;
using AcidarX.Kernel.Profiling;
using Microsoft.Extensions.Logging;
using Silk.NET.OpenGL;

namespace AcidarX.Graphics.Renderer.OpenGL
{
    public sealed class OpenGLFramebuffer : Framebuffer
    {
        private static readonly ILogger<OpenGLFramebuffer> Logger = AXLogger.CreateLogger<OpenGLFramebuffer>();
        private readonly GL _gl;
        private readonly FramebufferSpecs _specs;
        private RendererID _colorAttachmentRendererID;
        private RendererID _depthAttachmentRendererID;
        private bool _isDisposed;
        private RendererID _rendererID;

        public OpenGLFramebuffer(GL gl, FramebufferSpecs specs)
        {
            _gl = gl;
            _specs = specs;

            Invalidate();
        }

        public override unsafe void Invalidate()
        {
            if (_rendererID != 0)
            {
                Dispose(true);
            }

            _rendererID = (RendererID) _gl.CreateFramebuffer();
            Bind();

            _gl.CreateTextures(TextureTarget.Texture2D, 1, out uint generatedId);
            _colorAttachmentRendererID = (RendererID) generatedId;
            _gl.BindTexture(TextureTarget.Texture2D, _colorAttachmentRendererID);
            _gl.TexImage2D(TextureTarget.Texture2D, 0, (int) GLEnum.Rgba8, _specs.Width, _specs.Height, 0, GLEnum.Rgba,
                PixelType.UnsignedByte, null);

            _gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int) TextureWrapMode.Repeat);
            _gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int) TextureWrapMode.Repeat);

            _gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter,
                (int) TextureMinFilter.Linear);
            _gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter,
                (int) TextureMagFilter.Linear);

            _gl.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0,
                TextureTarget.Texture2D, _colorAttachmentRendererID, 0);

            _gl.CreateTextures(TextureTarget.Texture2D, 1, out generatedId);
            _depthAttachmentRendererID = (RendererID) generatedId;
            _gl.BindTexture(TextureTarget.Texture2D, _depthAttachmentRendererID);
            _gl.TexStorage2D(TextureTarget.Texture2D, 1, SizedInternalFormat.Depth24Stencil8, _specs.Width,
                _specs.Height);
            _gl.FramebufferTexture2D(FramebufferTarget.Framebuffer, GLEnum.DepthStencilAttachment,
                TextureTarget.Texture2D, _depthAttachmentRendererID, 0);

            Logger.Assert(_gl.CheckFramebufferStatus(FramebufferTarget.Framebuffer) == GLEnum.FramebufferComplete,
                "Framebuffer is incomplete");

            Unbind();
        }

        public override void Resize(uint width, uint height)
        {
            _specs.Width = width;
            _specs.Height = height;
            Invalidate();
        }

        public override void Bind()
        {
            _gl.BindFramebuffer(FramebufferTarget.Framebuffer, _rendererID);

            _gl.BindTexture(TextureTarget.Texture2D, _colorAttachmentRendererID);
        }

        public override void Unbind()
        {
            _gl.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
            _gl.BindTexture(TextureTarget.Texture2D, 0);
        }

        public override void Dispose()
        {
            Logger.Assert(!_isDisposed, $"{this} is already disposed");

            _isDisposed = true;
            Dispose(true);
            GC.SuppressFinalize(this);
            GC.KeepAlive(this);
        }

        public override FramebufferSpecs GetFramebufferSpecs() => _specs;
        public override RendererID GetColorAttachmentRendererID() => _colorAttachmentRendererID;


        protected override void Dispose(bool manual)
        {
            Logger.Assert(manual, $"Memory leak detected on object: {this}");

            AXProfiler.Capture(() =>
            {
                _gl.DeleteFramebuffers(1, _rendererID);
                _gl.DeleteTextures(1, _colorAttachmentRendererID);
                _gl.DeleteTextures(1, _depthAttachmentRendererID);
            });
        }

        public override string ToString() => $"Framebuffer|{_rendererID}";
    }
}