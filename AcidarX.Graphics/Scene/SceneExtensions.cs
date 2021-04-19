namespace AcidarX.Graphics.Scene
{
    public static class SceneExtensions
    {
        public static void AddSprite
            (this Scene scene, TransformComponent transformComponent, SpriteRendererComponent spriteRendererComponent)
        {
            int entity = scene.CreateEntity();
            scene.AddComponent(entity, transformComponent);
            scene.AddComponent(entity, spriteRendererComponent);
        }
    }
}