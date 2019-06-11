using System.IO;

namespace VSIXProject1.Services
{
    class Files
    {
        public bool Missing(string path) =>
            !File.Exists(path) && !Directory.Exists(path);
    }
}
