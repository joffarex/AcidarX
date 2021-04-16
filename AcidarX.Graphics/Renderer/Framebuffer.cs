using System;

namespace AcidarX.Graphics.Renderer
{
    public record FramebufferSpecs
    {
        public uint Width { get; set; }
        public uint Height { get; set; }
        public uint Samples { get; init; } = 1;
        public bool SwapChainTarget { get; init; } = false;
    }


    public abstract class Framebuffer : IDisposable
    {
        public abstract void Dispose();
        public abstract void Invalidate();
        public abstract void Resize(uint width, uint height);
        public abstract void Bind();
        public abstract void Unbind();
        public abstract FramebufferSpecs GetFramebufferSpecs();
        public abstract RendererID GetColorAttachmentRendererID();

        protected abstract void Dispose(bool manual);

        ~Framebuffer()
        {
            Dispose(false);
        }
    }
}