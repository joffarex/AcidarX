using System;

namespace AcidarX.Core.Renderer
{
    public record FramebufferSpecs
    {
        public uint Width { get; init; }
        public uint Height { get; init; }
        public uint Samples { get; init; } = 1;
        public bool SwapChainTarget { get; init; } = false;
    }


    public abstract class Framebuffer : IDisposable
    {
        public abstract void Dispose();
        public abstract void Invalidate();
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