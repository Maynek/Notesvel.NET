//********************************
// (c) 2024 Ada Maynek
// This software is released under the MIT License.
//********************************
using System.Xml;

namespace Maynek.Notesvel.Other
{
    public class MySiteReader
    {
        public static MySite Read(string path)
        {
            var xmlDocument = new XmlDocument();
            xmlDocument.Load(path);

            var element = xmlDocument.DocumentElement;

            var site = new MySite();
            foreach (var childElement in element.ChildNodes.OfType<XmlElement>())
            {
                switch (childElement.Name)
                {
                    case "Novels":
                        MySiteReader.ParseNovels(childElement, site);
                        break;
                }
            }

            return site;
        }

        protected static void ParseNovels(XmlElement element, MySite site)
        {
            foreach (var childElement in element.ChildNodes.OfType<XmlElement>())
            {
                switch (childElement.Name)
                {
                    case "Novel":
                        var novelId = childElement.GetAttribute("Id");
                        site.AddNovelId(novelId);
                        break;
                }
            }

            return;
        }

        protected static Chapter ParseChapter(XmlElement element, Chapter chapter)
        {
            chapter.Id = element.GetAttribute("Id");
            return chapter;
        }

    }
}
