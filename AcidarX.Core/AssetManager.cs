using System.Collections.Generic;
using AcidarX.Core.Utils;
using Silk.NET.OpenGL;
using Shader = AcidarX.Core.Renderer.Shader;

namespace AcidarX.Core
{
    public class AssetManager
    {
        private static readonly Dictionary<string, Shader> Shaders = new();
        private readonly GL _gl;

        public AssetManager(GL gl) => _gl = gl;

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
            shader = new Shader(_gl, vertexSource, fragmentSource);
            Shaders.Add(resourceName, shader);
            return shader;
        }
    }
}