using System.Collections.Generic;
using AcidarX.Core.Renderer;
using AcidarX.Core.Utils;
using Silk.NET.OpenGL;
using Shader = AcidarX.Core.Renderer.Shader;

namespace AcidarX.Core
{
    public class AssetManager
    {
        private static readonly Dictionary<string, Shader> Shaders = new();
        private static readonly Dictionary<string, Texture2D> Texture2Ds = new();
        private readonly GL _gl;
        private readonly GraphicsFactory _graphicsFactory;

        public AssetManager(GL gl, GraphicsFactory graphicsFactory)
        {
            _gl = gl;
            _graphicsFactory = graphicsFactory;
        }

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