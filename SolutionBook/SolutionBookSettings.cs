using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Xml.Linq;
using SolutionBook.Models;

namespace SolutionBook
{
    public class SolutionBookSettings
    {
        private static readonly string SettingsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SolutionBook.settings");

        private readonly FileSystemWatcher _watcher;

        public SolutionBookSettings()
        {
            _watcher = new FileSystemWatcher(Path.GetDirectoryName(SettingsPath), Path.GetFileName(SettingsPath));
            _watcher.Changed += (sender, arg) => Changed?.Invoke(this, EventArgs.Empty);
            _watcher.EnableRaisingEvents = true;
        }

        public event EventHandler Changed;

        private static void ReadXml(IEnumerable<XElement> elements, ICollection<BookItem> items, BookItem parent = null)
        {
            foreach (var element in elements)
            {
                if (element.Name == "Solution")
                {
                    items.Add(new BookItem(parent, BookItemType.Solution, element.Attribute("path").Value) { Header = element.Attribute("name").Value });
                }
                else
                {
                    var folder = new BookItem(parent, BookItemType.Folder) { Header = element.Attribute("name").Value };
                    items.Add(folder);
                    ReadXml(element.Elements(), folder.Items, folder);
                }
            }
        }

        public async Task<IList<BookItem>> LoadAsync(string path = null)
        {
            if (string.IsNullOrWhiteSpace(path)) path = SettingsPath;

            if (!File.Exists(path)) return new List<BookItem>();

            var (success, result) = await TryCatch.RetryAsync(async () =>
            {
                using (var stream = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.None))
                using (var reader = new StreamReader(stream))
                {
                    return await reader.ReadToEndAsync().ConfigureAwait(false);
                }
            }, 250, 4_000).ConfigureAwait(false); // try every 250ms for max 4s

            if (!success) return new List<BookItem>();

            XDocument document;
            try
            {
                var settingItems = new List<BookItem>();
                document = XDocument.Parse(result);
                ReadXml(document.Root.Elements(), settingItems);
                return settingItems;
            }
            catch
            {
                return new List<BookItem>();
            }
        }

        private static void WriteXml(XElement elements, IEnumerable<BookItem> items)
        {
            foreach (var item in items)
            {
                if (item.Type == BookItemType.Solution)
                {
                    var element = new XElement("Solution");
                    element.Add(new XAttribute("name", item.Header));
                    element.Add(new XAttribute("path", item.Path));
                    elements.Add(element);
                }
                else
                {
                    var element = new XElement("Folder");
                    element.Add(new XAttribute("name", item.Header));
                    elements.Add(element);
                    WriteXml(element, item.Items);
                }
            }
        }

        public async Task SaveAsync(IEnumerable<BookItem> items, string path = null)
        {
            if (string.IsNullOrWhiteSpace(path)) path = SettingsPath;

            var document = new XDocument();
            var book = new XElement("SolutionBook");
            document.Add(book);

            WriteXml(book, items);

            var text = document.ToString(SaveOptions.None);

            var success = await TryCatch.RetryAsync(async () =>
            {
                using (var stream = File.Open(path, FileMode.Create, FileAccess.Write, FileShare.None))
                using (var writer = new StreamWriter(stream))
                {
                    await writer.WriteAsync(text).ConfigureAwait(false);
                }
            }, 250, 4_000).ConfigureAwait(false); // try every 250ms for max 4s
        }
    }
}
