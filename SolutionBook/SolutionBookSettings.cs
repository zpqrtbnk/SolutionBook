using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SolutionBook
{
    /// <summary>
    /// Represents the settings.
    /// </summary>
    public class SolutionBookSettings
    {
        /// <summary>
        /// Gets the items from the settings.
        /// </summary>
        public IList<BookItem> Load()
        {
            var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SolutionBook.settings");

            if (!File.Exists(path))
                return new List<BookItem>();

            void Read(IEnumerable<XElement> elements, ICollection<BookItem> items, BookItem parent)
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
                        Read(element.Elements(), folder.Items, folder);
                    }
                }
            }

            XDocument document = null;
            var attempts = 0;
            while (document == null && attempts++ < 8)
            {
                try
                {
                    using (var stream = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.None))
                    {
                        document = XDocument.Load(stream);
                    }
                }
                catch
                {
                    Thread.Sleep(250);
                }
            }

            var settingItems = new List<BookItem>();

            if (document != null)
                Read(document.Root.Elements(), settingItems, null);

            return settingItems;
        }

        /// <summary>
        /// Saves the items in the settings.
        /// </summary>
        public void Save(IEnumerable<BookItem> items)
        {
            var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SolutionBook.settings");
            
            void Write(XElement elements, IEnumerable<BookItem> eitems)
            {
                foreach (var item in eitems)
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
                        Write(element, item.Items);
                    }
                }
            }

            var document = new XDocument();
            var book = new XElement("SolutionBook");
            document.Add(book);

            Write(book, items);

            var attempts = 0;

            while (attempts == 0 && attempts++ < 8)
            {
                try
                {
                    using (var stream = File.Open(path, FileMode.Create, FileAccess.Write, FileShare.None))
                    {
                        document.Save(stream, SaveOptions.None);
                    }
                }
                catch
                {
                    Thread.Sleep(250);
                }
            }
        }
    }
}
