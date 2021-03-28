using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace AcidarX.Core.Renderer
{
    public readonly struct BufferLayout : IEnumerable<BufferElement>
    {
        private static readonly ILogger<BufferLayout> Logger = AXLogger.CreateLogger<BufferLayout>();

        public List<BufferElement> Elements { get; }

        public uint Stride { get; }

        public BufferLayout(List<BufferElement> elements)
        {
            Logger.Assert(elements.Any(), "Layout must contain at least one element");

            Elements = elements;
            Stride = 0;
            Stride = CalculateOffsetAndStride();
        }

        private uint CalculateOffsetAndStride()
        {
            uint offset = 0;
            uint stride = 0;

            for (var i = 0; i < Elements.Count; i++)
            {
                Elements[i] = new BufferElement(Elements[i].Name, Elements[i].Type, offset, Elements[i].Normalized);
                offset += Elements[i].Size;
                stride += Elements[i].Size;
            }

            return stride;
        }

        public IEnumerator<BufferElement> GetEnumerator() => Elements.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}