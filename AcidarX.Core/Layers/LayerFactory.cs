using System;
using AcidarX.Graphics;
using AcidarX.Graphics.Renderer;
using Silk.NET.Input;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;

namespace AcidarX.Core.Layers
{
    public class LayerFactory
    {
        private readonly AssetManager _assetManager;
        private readonly GL _gl;
        private readonly AXRenderer2D _renderer2D;

        public LayerFactory(GL gl, AXRenderer2D renderer2D, AssetManager assetManager)
        {
            _gl = gl;
            _renderer2D = renderer2D;
            _assetManager = assetManager;
        }

        public T CreateLayer<T>() where T : Layer =>
            (T) Activator.CreateInstance(typeof(T), _renderer2D, _assetManager);

        public ImGuiLayer CreateImGuiLayer
            (IWindow window, IInputContext inputContext) => new(_gl, window, inputContext);
    }
}