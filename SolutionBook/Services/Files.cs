using System.IO;

namespace SolutionBook.Services
{
    class Files
    {
        public bool Missing(string path) =>
            !File.Exists(path) && !Directory.Exists(path);
    }
}
