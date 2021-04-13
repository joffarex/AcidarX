using System.IO;
using System.Text;

namespace AcidarX.Kernel.Utils
{
    public static class FileUtils
    {
        public static string LoadSource(string path)
        {
            using var sr = new StreamReader(path, Encoding.UTF8);
            return sr.ReadToEnd();
        }
    }
}