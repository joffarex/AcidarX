using System.Collections;
using System.Collections.Generic;

namespace AcidarX.Core.Layers
{
    public class LayerStack : IEnumerable<Layer>
    {
        private const int StackStartIndex = 0;

        // We need to push layers in the first half of stack, and overlays at the second half
        // That's why we need this index to keep track of layer's index so it wont get emlaced after overlay
        private int _layerIndex;

        public LayerStack() => Layers = new List<Layer>();

        public List<Layer> Layers { get; }

        public IEnumerator<Layer> GetEnumerator() => Layers.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public void PushLayer(Layer layer)
        {
            Layers.Insert(StackStartIndex + _layerIndex, layer);
            _layerIndex++;
        }

        public void PushOverlay(Layer overlay)
        {
            Layers.Add(overlay);
        }

        public void PopLayer(Layer layer)
        {
            Layers.Remove(layer);
            _layerIndex--;
        }

        public void PopOverlay(Layer overlay)
        {
            Layers.Remove(overlay);
        }
    }
}