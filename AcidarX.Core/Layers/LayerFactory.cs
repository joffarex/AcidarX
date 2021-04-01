using System;
using AcidarX.Core.Renderer;

namespace AcidarX.Core.Layers
{
    public class LayerFactory
    {
        private readonly AssetManager _assetManager;
        private readonly AXRenderer2D _renderer2D;

        public LayerFactory(AXRenderer2D renderer2D, AssetManager assetManager)
        {
            _renderer2D = renderer2D;
            _assetManager = assetManager;
        }

        public T CreateLayer<T>() where T : Layer =>
            (T) Activator.CreateInstance(typeof(T), _renderer2D, _assetManager);
    }
}