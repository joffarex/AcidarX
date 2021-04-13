using System;
using System.Drawing;
using System.Numerics;
using AcidarX.Core;
using AcidarX.Core.Camera;
using AcidarX.Core.Events;
using AcidarX.Core.Graphics;
using AcidarX.Core.Layers;
using AcidarX.Core.Renderer;
using AcidarX.Kernel.Logging;
using AcidarX.Kernel.Utils;
using ImGuiNET;
using Microsoft.Extensions.Logging;

namespace EditarX
{
    public sealed class EditorLayer : Layer
    {
        private static readonly ILogger<EditorLayer> Logger = AXLogger.CreateLogger<EditorLayer>();

        private readonly AssetManager _assetManager;

        private readonly OrthographicCameraController _cameraController;
        private readonly AXRenderer2D _renderer2D;

        private Texture2D _texture;
        private Texture2D _tilemapTexture;

        public EditorLayer(AXRenderer2D renderer2D, AssetManager assetManager)
            : base("Editor layer")
        {
            _renderer2D = renderer2D;
            _assetManager = assetManager;
            _cameraController = new OrthographicCameraController(16.0f / 9.0f);
        }

        public override void OnAttach()
        {
        }

        public override void OnLoad()
        {
            _texture = _assetManager.GetTexture2D("assets/Textures/awesomeface.png");
            _tilemapTexture = _assetManager.GetTexture2D("assets/Textures/rpg_tilemap.png");
            _renderer2D.Init();
            _renderer2D.SetFramebuffer(new FramebufferSpecs
            {
                Width = 1920, Height = 1080
            });
        }

        public override void OnDetach()
        {
        }

        public override void OnImGuiRender(AppRenderEvent e)
        {
            _renderer2D.DrawDockSpace(_cameraController, () =>
            {
                AXStatistics.ImGuiWindow();
                FpsUtils.ImGuiWindow(e.DeltaTime);
                ImGui.ShowDemoWindow();
            });
        }

        public override void OnUpdate(double deltaTime)
        {
            if (_renderer2D.ViewportFocused)
            {
                _cameraController.OnUpdate(deltaTime);
            }
        }

        public override void OnRender(double deltaTime)
        {
            _renderer2D.DrawInFramebuffer(() =>
            {
                _renderer2D.ClearFramebuffer(new Vector4(24.0f / 255.0f, 24.0f / 255.0f, 24.0f / 255.0f, 1.0f));

                AXStatistics.Reset();

                _renderer2D.BeginScene(_cameraController.Camera);
                _renderer2D.DrawQuad(new QuadProperties
                {
                    Position = new Vector3(-Vector2.UnitX * 0.7f, 0.0f), Size = Vector2.One * 1.1f,
                    Color = new Vector4(0.8f, 0.8f, 0.4f, 1.0f)
                });
                _renderer2D.DrawQuad(new QuadProperties
                {
                    Position = new Vector3(Vector2.Zero, 1.0f), Size = Vector2.One * 1.2f,
                    Color = new Vector4(0.4f, 0.1f, 0.8f, 1.0f), RotationInRadians = 45 * (float) (Math.PI / 180.0f)
                });
                _renderer2D.EndScene();

                // Transparency is weird with depth buffer. We need to first draw non-transparent objects and then draw transparent ones
                // But even for them, render order MATTERS! also we'll need to order Z Index as well.
                // Related link: https://research.ncl.ac.uk/game/mastersdegree/graphicsforgames/transparencyanddepth/Tutorial%204%20-%20Transparency%20and%20Depth.pdf
                _renderer2D.BeginScene(_cameraController.Camera);
                _renderer2D.DrawQuad(new QuadProperties
                {
                    Position = new Vector3(-Vector2.One * 0.5f, 2.0f), Size = Vector2.One * 0.8f,
                    Color = new Vector4(0.8f, 0.1f, 0.4f, 0.7f), Texture2D = _texture
                });
                _renderer2D.DrawQuad(new QuadProperties
                {
                    Position = new Vector3(-Vector2.UnitX * 0.8f, 2.1f), Size = Vector2.One * 0.8f,
                    Color = new Vector4(0.4f, 0.8f, 0.2f, 1.0f), Texture2D = _texture, TilingFactor = 2.0f
                });
                _renderer2D.DrawQuad(new QuadProperties
                {
                    Position = new Vector3(Vector2.UnitY * 0.9f, 2.2f), Size = Vector2.One,
                    SubTexture2D = new SubTexture2D(_tilemapTexture, new Vector2(4, 3), new SizeF(128.0f, 128.0f))
                });
                _renderer2D.DrawQuad(new QuadProperties
                {
                    Position = new Vector3(Vector2.UnitY * 0.9f, 2.3f), Size = new Vector2(1.0f, 2.0f),
                    SubTexture2D = new SubTexture2D(_tilemapTexture, new Vector2(4, 1), new SizeF(128.0f, 128.0f),
                        new Vector2(1, 2))
                });
                _renderer2D.EndScene();
            });
        }

        public override void OnEvent(Event e)
        {
            _cameraController.OnEvent(e);
        }

        public override void Dispose(bool manual)
        {
            Logger.Assert(manual, $"Memory leak detected on object: {this}");
        }
    }
}