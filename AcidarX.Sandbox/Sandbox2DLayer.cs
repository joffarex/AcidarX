﻿using System;
using System.Drawing;
using System.Numerics;
using AcidarX.Core.Layers;
using AcidarX.Graphics;
using AcidarX.Graphics.Camera;
using AcidarX.Graphics.Graphics;
using AcidarX.Graphics.Renderer;
using AcidarX.Graphics.Scene;
using AcidarX.Kernel.Events;
using AcidarX.Kernel.Logging;
using AcidarX.Kernel.Utils;
using ImGuiNET;
using Microsoft.Extensions.Logging;
using Silk.NET.Maths;

namespace AcidarX.Sandbox
{
    public sealed class Sandbox2DLayer : Layer
    {
        private static readonly ILogger<Sandbox2DLayer> Logger = AXLogger.CreateLogger<Sandbox2DLayer>();

        private readonly AssetManager _assetManager;

        private readonly AXRenderer2D _renderer2D;

        private Scene _scene;

        private Texture2D _texture;
        private Texture2D _tilemapTexture;

        public Sandbox2DLayer(AXRenderer2D renderer2D, AssetManager assetManager)
            : base("Sandbox 2D layer")
        {
            _renderer2D = renderer2D;
            _assetManager = assetManager;
        }

        public override void OnAttach()
        {
        }

        public override void OnLoad()
        {
            _texture = _assetManager.GetTexture2D("assets/Textures/adv-test.png");
            _tilemapTexture = _assetManager.GetTexture2D("assets/Textures/rpg_tilemap.png");
            _renderer2D.Init();

            _scene = new Scene(_renderer2D, new OrthographicCameraController(16.0f / 9.0f));
            _scene.AddComponentType<TransformComponent>();
            _scene.AddComponentType<SpriteRendererComponent>();

            _scene.AddSprite(new TransformComponent
            {
                Translation = new Vector3(-Vector2.UnitX * 0.7f, 0.0f),
                Scale = Vector2.One * 1.1f
            }, new SpriteRendererComponent
            {
                Color = new Vector4(0.8f, 0.8f, 0.4f, 1.0f)
            });
            _scene.AddSprite(new TransformComponent
            {
                Translation = new Vector3(Vector2.Zero, 1.0f),
                Scale = Vector2.One * 1.2f,
                Rotation = Matrix4x4.CreateRotationZ(45 * (float) (Math.PI / 180.0f))
            }, new SpriteRendererComponent
            {
                Color = new Vector4(0.8f, 0.1f, 0.4f, 1.0f)
            });
            StressTest();

            _scene.AddSprite(new TransformComponent
            {
                Translation = new Vector3(-Vector2.One * 0.5f, 2.0f),
                Scale = Vector2.One * 1.8f
            }, new SpriteRendererComponent
            {
                // Color = new Vector4(0.8f, 0.1f, 0.4f, 0.7f),
                Texture = _texture
            });
            _scene.AddSprite(new TransformComponent
            {
                Translation = new Vector3(-Vector2.UnitX * 0.8f, 2.1f),
                Scale = Vector2.One * 0.8f
            }, new SpriteRendererComponent
            {
                // Color = new Vector4(0.4f, 0.8f, 0.4f, 0.7f),
                Texture = _texture,
                TilingFactor = 2.0f
            });
            _scene.AddSprite(new TransformComponent
            {
                Translation = new Vector3(Vector2.UnitY * 0.9f, 2.2f),
                Scale = Vector2.One
            }, new SpriteRendererComponent
            {
                SubTexture = new SubTexture2D(_tilemapTexture, new Vector2(4, 3), new SizeF(128.0f, 128.0f))
            });
            _scene.AddSprite(new TransformComponent
            {
                Translation = new Vector3(Vector2.UnitY * 0.9f, 2.3f),
                Scale = new Vector2(1.0f, 2.0f)
            }, new SpriteRendererComponent
            {
                SubTexture = new SubTexture2D(_tilemapTexture, new Vector2(4, 1), new SizeF(128.0f, 128.0f),
                    new Vector2(1, 2))
            });
        }

        public override void OnDetach()
        {
        }

        public override void OnImGuiRender(AppRenderEvent e)
        {
            // AXStatistics.ImGuiWindow();
            FpsUtils.ImGuiWindow(e.DeltaTime);
            ImGui.ShowDemoWindow();
        }

        public override void OnUpdate(double deltaTime)
        {
            _scene.OnUpdate(deltaTime);
        }

        public override void OnRender(double deltaTime)
        {
            // Transparency is weird with depth buffer. We need to first draw non-transparent objects and then draw transparent ones
            // But even for them, render order MATTERS! also we'll need to order Z Index as well.
            // Related link: https://research.ncl.ac.uk/game/mastersdegree/graphicsforgames/transparencyanddepth/Tutorial%204%20-%20Transparency%20and%20Depth.pdf

            _renderer2D.SetClearColor(new Vector4D<float>(24.0f, 24.0f, 24.0f, 1.0f));
            _renderer2D.Clear();

            AXStatistics.Reset();

            _scene.OnRender(deltaTime);
        }

        private void StressTest()
        {
            const float offset = 0.1f;
            const int quadPerAxis = 100;

            for (var x = 0; x < quadPerAxis; x++)
            {
                for (var y = 0; y < quadPerAxis; y++)
                {
                    float xPos = offset + x * 1.1f;
                    float yPos = offset + y * 1.1f;

                    _scene.AddSprite(new TransformComponent
                    {
                        Translation = new Vector3(xPos, yPos, 0.0f),
                        Scale = Vector2.One * 0.1f
                    }, new SpriteRendererComponent
                    {
                        Color = new Vector4(xPos / quadPerAxis, yPos / quadPerAxis, 0.8f, 1.0f)
                    });
                }
            }
        }

        public override void OnEvent(Event e)
        {
            _scene.OnEvent(e);
        }

        public override void Dispose(bool manual)
        {
            Logger.Assert(manual, $"Memory leak detected on object: {this}");
        }
    }
}