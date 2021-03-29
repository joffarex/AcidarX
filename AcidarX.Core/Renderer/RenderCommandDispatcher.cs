﻿using System;
using System.Collections.Generic;
using System.Linq;
using Silk.NET.Maths;

namespace AcidarX.Core.Renderer
{
    // TODO: call all the commands in `_renderQueue` onto separate thread
    public sealed class RenderCommandDispatcher
    {
        private readonly RendererAPI _rendererAPI;
        private readonly Queue<Action> _renderQueue;

        public RenderCommandDispatcher(RendererAPI rendererAPI)
        {
            _rendererAPI = rendererAPI;
            _renderQueue = new Queue<Action>();
        }

        public void DrawIndexed(VertexArray vertexArray)
        {
            _renderQueue.Enqueue(() => { _rendererAPI.DrawIndexed(vertexArray); });
        }

        public void UseShader(Shader shader)
        {
            _renderQueue.Enqueue(() => { _rendererAPI.UseShader(shader); });
        }

        public void SetClearColor(Vector4D<float> color)
        {
            _renderQueue.Enqueue(() => { _rendererAPI.SetClearColor(color); });
        }

        public void Clear()
        {
            _renderQueue.Enqueue(() => { _rendererAPI.Clear(); });
        }

        public void Dispatch()
        {
            while (_renderQueue.Any())
            {
                Action renderCommand = _renderQueue.Dequeue();
                renderCommand();
            }
        }
    }
}