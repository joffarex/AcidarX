using System;
using AcidarX.Core.Renderer;

namespace AcidarX.Core.Layers
{
    public class LayerFactory
    {
        private readonly AssetManager _assetManager;
        private readonly GraphicsFactory _graphicsFactory;
        private readonly AXRenderer _renderer;

        public LayerFactory(AXRenderer renderer, GraphicsFactory graphicsFactory, AssetManager assetManager)
        {
            _renderer = renderer;
            _graphicsFactory = graphicsFactory;
            _assetManager = assetManager;
        }

        public T CreateLayer<T>() where T : Layer =>
            (T) Activator.CreateInstance(typeof(T), _renderer, _assetManager, _graphicsFactory);
    }
}