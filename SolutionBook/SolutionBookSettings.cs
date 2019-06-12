using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SolutionBook
{
    public class SolutionBookSettings
    {
        public class Element
        {
            public string Name { get; set; }
        }

        public class Solution : Element
        {
            public string Path { get; set; }
        }

        public class Folder : Element
        {
            public List<Element> Elements{ get; set; } = new List<Element>();
        }

        public List<Element> Elements { get; set; } = new List<Element>();

        public static async Task<SolutionBookSettings> LoadAsync()
        {
            var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SolutionBook.settings");

            if (!File.Exists(path))
                return new SolutionBookSettings();

            void Read(IEnumerable<XElement> xElements, List<Element> elements)
            {
                foreach (var xElement in xElements)
                {
                    if (xElement.Name == "Solution")
                    {
                        var solution = new Solution { Name = xElement.Attribute("name").Value, Path = xElement.Attribute("path").Value };
                        elements.Add(solution);
                    }
                    else
                    {
                        var folder = new Folder { Name = xElement.Attribute("name").Value };
                        elements.Add(folder);
                        Read(xElement.Elements(), folder.Elements);
                    }
                }
            }

            return await Task.Run(() =>
            {
                var document = XDocument.Load(path);
                var settings = new SolutionBookSettings();
                var elements = settings.Elements;

                Read(document.Root.Elements(), settings.Elements);

                return settings;

            }).ConfigureAwait(false);
        }

        public static Task SaveAsync(SolutionBookSettings settings)
        {
            /*
            await Task.Run(async () =>
            {
                var serializer = new XmlSerializer(typeof(SolutionPageConfiguration));

                for (var i = 0; i <= _MAXIMUM_RETRIES; i++)
                {
                    try
                    {
                        using (var writer = new StreamWriter(_settingsFilePath))
                        {
                            serializer.Serialize(writer, configuration);
                            return;
                        }
                    }
                    catch (Exception)
                    {
                        if (i >= _MAXIMUM_RETRIES)
                            return;
                    }
                    await Task.Delay(200);
                }
            }).ConfigureAwait(false);
            */
            throw new NotImplementedException();
        }
    }
}
