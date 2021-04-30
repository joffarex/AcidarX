using System;
using System.Numerics;
using AcidarX.Kernel.Events;
using AcidarX.Kernel.Input;
using AcidarX.Kernel.Profiling;

namespace AcidarX.Graphics.Camera
{
    public class OrthographicCameraController
    {
        private readonly float _cameraSpeed;
        private readonly float _zoomSpeed;
        private float _aspectRatio;
        private Vector3 _cameraPosition;
        private float _zoomLevel;

        public OrthographicCameraController(float aspectRatio)
        {
            _aspectRatio = aspectRatio;
            _zoomLevel = 1.0f;
            _zoomSpeed = 0.25f;
            _cameraSpeed = 1.5f;
            Camera = new OrthographicCamera(-_aspectRatio * _zoomLevel, _aspectRatio * _zoomLevel, -_zoomLevel,
                _zoomLevel);
        }

        public OrthographicCamera Camera { get; }

        public void OnUpdate(double deltaTime)
        {
            AXProfiler.Capture(() =>
            {
                if (KeyboardState.IsKeyPressed(AXKey.J))
                {
                    _cameraPosition.X -= _cameraSpeed * (float) deltaTime;
                }

                if (KeyboardState.IsKeyPressed(AXKey.L))
                {
                    _cameraPosition.X += _cameraSpeed * (float) deltaTime;
                }

                if (KeyboardState.IsKeyPressed(AXKey.I))
                {
                    _cameraPosition.Y += _cameraSpeed * (float) deltaTime;
                }

                if (KeyboardState.IsKeyPressed(AXKey.K))
                {
                    _cameraPosition.Y -= _cameraSpeed * (float) deltaTime;
                }

                Camera.Position = _cameraPosition;
            });
        }

        public void OnEvent(Event e)
        {
            AXProfiler.Capture(() =>
            {
                var eventDispatcher = new EventDispatcher(e);
                eventDispatcher.Dispatch<MouseScrollEvent>(OnMouseScroll);
                eventDispatcher.Dispatch<WindowResizeEvent>(OnWindowResize);
            });
        }

        private bool OnMouseScroll(MouseScrollEvent e)
        {
            AXProfiler.Capture(() =>
            {
                _zoomLevel -= e.Offset.Y * _zoomSpeed;
                _zoomLevel = Math.Max(_zoomLevel, 0.25f);
                Camera.SetProjection(-_aspectRatio * _zoomLevel, _aspectRatio * _zoomLevel, -_zoomLevel,
                    _zoomLevel);
            });

            return false;
        }

        private bool OnWindowResize(WindowResizeEvent e)
        {
            AXProfiler.Capture(() => { OnFramebufferResize(e.Size.X, e.Size.Y); });

            return false;
        }

        public void OnFramebufferResize(float width, float height)
        {
            _aspectRatio = width / height;
            Camera.SetProjection(-_aspectRatio * _zoomLevel, _aspectRatio * _zoomLevel, -_zoomLevel,
                _zoomLevel);
        }
    }
}