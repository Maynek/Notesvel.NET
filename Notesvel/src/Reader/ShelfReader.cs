//********************************
// (c) 2024 Ada Maynek
// This software is released under the MIT License.
//********************************
using System.Xml;

namespace Maynek.Notesvel.Reader
{
    public class ShelfReader
    {
        public static Shelf Read(string path)
        {
            var xmlDocument = new XmlDocument();
            xmlDocument.Load(path);

            var element = xmlDocument.DocumentElement;

            var shelf = new Shelf();
            foreach (var childElement in element.ChildNodes.OfType<XmlElement>())
            {
                switch (childElement.Name)
                {
                    case "Novels":
                        ShelfReader.ParseNovels(childElement, shelf);
                        break;
                }
            }

            return shelf;
        }

        protected static void ParseNovels(XmlElement element, Shelf shelf)
        {
            foreach (var childElement in element.ChildNodes.OfType<XmlElement>())
            {
                switch (childElement.Name)
                {
                    case "Novel":
                        var novelId = childElement.GetAttribute("Id");
                        var target = childElement.GetAttribute("Target");
                        shelf.AddShelfItem(novelId, target);
                        break;
                }
            }

            return;
        }
    }
}
