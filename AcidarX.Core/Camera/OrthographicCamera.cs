using System;
using System.Numerics;
using AcidarX.Core.Profiling;

namespace AcidarX.Core.Camera
{
    public class OrthographicCamera
    {
        private Vector3 _position;
        private float _rotation;
        private Matrix4x4 _viewMatrix;

        public OrthographicCamera(float left, float right, float bottom, float top)
        {
            AXProfiler.Capture(() =>
            {
                Left = left;
                Right = right;
                Bottom = bottom;
                Top = top;

                Rotation = 0;
                _viewMatrix = Matrix4x4.Identity;
                ProjectionMatrix = Matrix4x4.CreateOrthographicOffCenter(left, right, bottom, top, -1.0f, 1.0f);
                ViewProjectionMatrix = ProjectionMatrix * _viewMatrix;
            });
        }

        public float Left { get; private set; }
        public float Right { get; private set; }
        public float Bottom { get; private set; }
        public float Top { get; private set; }

        public Matrix4x4 ProjectionMatrix { get; private set; }

        public Matrix4x4 ViewMatrix
        {
            get => _viewMatrix;
            private set => _viewMatrix = value;
        }

        public Matrix4x4 ViewProjectionMatrix { get; private set; }

        public Vector3 Position
        {
            get => _position;
            set
            {
                _position = value;
                RecalculateViewMatrix();
            }
        }

        // This rotation is for Z axis, as we'll be using this camera only in 2D scenes
        public float Rotation
        {
            get => _rotation;
            set
            {
                _rotation = value;
                RecalculateViewMatrix();
            }
        }

        private void RecalculateViewMatrix()
        {
            AXProfiler.Capture(() =>
            {
                Matrix4x4 transform = Matrix4x4.CreateTranslation(_position) *
                                      Matrix4x4.CreateRotationZ((float) (Math.PI / 180f) * _rotation);

                Matrix4x4.Invert(transform, out _viewMatrix);
                ViewProjectionMatrix = _viewMatrix * ProjectionMatrix;
            });
        }

        public void SetProjection(float left, float right, float bottom, float top)
        {
            AXProfiler.Capture(() =>
            {
                _viewMatrix = Matrix4x4.Identity;
                ProjectionMatrix = Matrix4x4.CreateOrthographicOffCenter(left, right, bottom, top, -1.0f, 1.0f);
                ViewProjectionMatrix = ProjectionMatrix * _viewMatrix;
            });
        }
    }
}