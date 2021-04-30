using System.Collections.Generic;
using AcidarX.Graphics.Renderer;
using AcidarX.Kernel.Utils;

namespace AcidarX.Graphics
{
    public class AssetManager
    {
        private static readonly Dictionary<string, Shader> Shaders = new();
        private static readonly Dictionary<string, Texture2D> Texture2Ds = new();
        private readonly GraphicsFactory _graphicsFactory;

        public AssetManager(GraphicsFactory graphicsFactory) => _graphicsFactory = graphicsFactory;

        public Shader GetShader(string resourceName)
        {
            string vertexPath = PathUtils.GetFullPath($"{resourceName}.vert");
            string fragmentPath = PathUtils.GetFullPath($"{resourceName}.frag");

            if (Shaders.TryGetValue(resourceName, out Shader shader))
            {
                return shader;
            }

            string vertexSource = FileUtils.LoadSource(vertexPath);
            string fragmentSource = FileUtils.LoadSource(fragmentPath);
            shader = _graphicsFactory.CreateShader(vertexSource, fragmentSource);
            Shaders.Add(resourceName, shader);
            return shader;
        }

        public Texture2D GetTexture2D(string resourceName)
        {
            string texturePath = PathUtils.GetFullPath(resourceName);

            if (Texture2Ds.TryGetValue(texturePath, out Texture2D texture2D))
            {
                return texture2D;
            }

            texture2D = _graphicsFactory.CreateTexture(texturePath);
            Texture2Ds.Add(texturePath, texture2D);
            return texture2D;
        }
    }
}