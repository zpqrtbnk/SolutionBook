using System;
using System.Collections.Generic;
using System.IO;
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
        public static async Task<IList<BookItem>> LoadAsync()
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

            return await Task.Run(() =>
            {
                var document = XDocument.Load(path);
                var items = new List<BookItem>();

                Read(document.Root.Elements(), items, null);

                return items;

            }).ConfigureAwait(false);
        }

        /// <summary>
        /// Saves the items in the settings.
        /// </summary>
        public async Task SaveAsync(IEnumerable<BookItem> items)
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

            await Task.Run(() =>
            {
                var document = new XDocument();
                var elements = new XElement("SolutionBook");
                document.Add(elements);

                Write(elements, items);

                document.Save(path, SaveOptions.None);
            }).ConfigureAwait(false);
        }
    }
}
