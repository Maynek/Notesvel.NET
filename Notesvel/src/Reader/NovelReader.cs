//********************************
// (c) 2024 Ada Maynek
// This software is released under the MIT License.
//********************************
using System.Xml;

namespace Maynek.Notesvel.Reader
{
    public class InvalidFormatException : Exception { }

    public class NovelReader
    {
        public static Novel Read(string path)
        {
            var xmlDocument = new XmlDocument();
            xmlDocument.Load(path);

            var element = xmlDocument.DocumentElement;
            if (element == null)
            {
                throw new InvalidFormatException();
            }

            var novel = new Novel();
            foreach (var childElement in element.ChildNodes.OfType<XmlElement>())
            {
                switch (childElement.Name)
                {
                    case "MainTitle":
                        novel.MainTitle = childElement.InnerText;
                        break;
                    
                    case "SubTitle":
                        novel.SubTitle = childElement.InnerText;
                        break;

                    case "Chapters":
                        NovelReader.ParseChapters(childElement, novel);
                        break;

                    case "Notes":
                        NovelReader.ParseNotes(childElement, novel);
                        break;
                }
            }

            return novel;
        }

        protected static void ParseChapters(XmlElement element, Novel novel)
        {
            foreach (var childElement in element.ChildNodes.OfType<XmlElement>())
            {
                switch (childElement.Name)
                {
                    case "Chapter":
                        var chapter = new Chapter();
                        NovelReader.ParseChapter(childElement, chapter);
                        novel.Chapters.Add(chapter);
                        break;
                }
            }

            return;
        }

        protected static Chapter ParseChapter(XmlElement element, Chapter chapter)
        {
            chapter.Id = element.GetAttribute("Id");

            foreach (var childElement in element.ChildNodes.OfType<XmlElement>())
            {
                switch (childElement.Name)
                {
                    case "Title":
                        chapter.Title = childElement.InnerText;
                        break;

                    case "Episodes":
                        NovelReader.ParseEpisodes(childElement, chapter);
                        break;
                }
            }

            return chapter;
        }


        protected static void ParseEpisodes(XmlElement element, Chapter chapter)
        {
            foreach (var childElement in element.ChildNodes.OfType<XmlElement>())
            {
                switch (childElement.Name)
                {
                    case "Episode":
                        var episode = new Episode();
                        NovelReader.ParseEpisode(childElement, episode);
                        chapter.Episodes.Add(episode);
                        break;
                }
            }

            return;
        }

        protected static Episode ParseEpisode(XmlElement element, Episode episode)
        {            
            episode.Id = element.GetAttribute("Id");

            foreach (var childElement in element.ChildNodes.OfType<XmlElement>())
            {
                switch (childElement.Name)
                {
                    case "Title":
                        episode.Title = childElement.InnerText;
                        break;
                }
            }

            return episode;
        }

        protected static void ParseNotes(XmlElement element, Novel novel)
        {
            foreach (var childElement in element.ChildNodes.OfType<XmlElement>())
            {
                switch (childElement.Name)
                {
                    case "Note":
                        var note = new Note();
                        NovelReader.ParseNote(childElement, note);
                        novel.Notes.Add(note);
                        break;
                }
            }

            return;
        }

        protected static Note ParseNote(XmlElement element, Note note)
        {
            note.Id = element.GetAttribute("Id");

            foreach (var childElement in element.ChildNodes.OfType<XmlElement>())
            {
                switch (childElement.Name)
                {
                    case "Title":
                        note.Title = childElement.InnerText;
                        break;
                }
            }

            return note;
        }
    }
}
