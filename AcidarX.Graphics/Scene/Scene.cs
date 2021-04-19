using AcidarX.ECS;
using AcidarX.Graphics.Camera;
using AcidarX.Graphics.Renderer;
using AcidarX.Kernel.Events;

namespace AcidarX.Graphics.Scene
{
    public class Scene
    {
        private readonly OrthographicCameraController _cameraController;
        private readonly Registry _registry = new();
        private readonly AXRenderer2D _renderer2D;

        public Scene(AXRenderer2D renderer2D, OrthographicCameraController cameraController)
        {
            _renderer2D = renderer2D;
            _cameraController = cameraController;
        }

        public void AddComponentType<T>() where T : IComponent => _registry.AddComponentType<T>();

        public int CreateEntity() => _registry.CreateEntity();

        public void AddComponent<T>
            (int entity, T component) where T : IComponent => _registry.AddComponent(entity, component);

        public void OnUpdate(double deltaTime) => _cameraController.OnUpdate(deltaTime);

        public void OnRender(double deltaTime)
        {
            _renderer2D.BeginScene(_cameraController.Camera);

            for (var entity = 0; entity < _registry.EntityCount; entity++)
            {
                var transform = _registry.GetComponent<TransformComponent>(entity);
                var sprite = _registry.GetComponent<SpriteRendererComponent>(entity);

                if (transform != null && sprite != null)
                {
                    _renderer2D.DrawSprite(transform, sprite);
                }
            }

            _renderer2D.EndScene();
        }

        public void OnEvent(Event e) => _cameraController.OnEvent(e);
    }
}