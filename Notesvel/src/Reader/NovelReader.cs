//********************************
// (c) 2024 Ada Maynek
// This software is released under the MIT License.
//********************************
using System.Xml;

namespace Maynek.Notesvel.Reader
{
    public class NovelReader
    {
        protected Novel novel;

        protected NovelReader()
        {
            this.novel = new Novel();
        }

        protected Novel ReadDocument(string path)
        {
            var xmlDocument = new XmlDocument();
            xmlDocument.Load(path);

            var element = xmlDocument.DocumentElement;

            if (element != null)
            {
                foreach (var childElement in element.ChildNodes.OfType<XmlElement>())
                {
                    switch (childElement.Name)
                    {
                        case "MainTitle":
                            this.novel.MainTitle = childElement.InnerText;
                            break;

                        case "SubTitle":
                            this.novel.SubTitle = childElement.InnerText;
                            break;

                        case "Episodes":
                            this.ParseEpisodes(childElement);
                            break;

                        case "Notes":
                            this.ParseNotes(childElement);
                            break;
                    }
                }
            }

            return this.novel;
        }

        protected void ParseEpisodes(XmlElement element)
        {
            foreach (var childElement in element.ChildNodes.OfType<XmlElement>())
            {
                switch (childElement.Name)
                {
                    case "Episode":
                        var episode = this.ParseEpisode(childElement);
                        this.novel.Episodes.Add(episode);
                        break;
                }
            }

            return;
        }

        protected Episode ParseEpisode(XmlElement element)
        {
            var episode = new Episode();
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

        protected void ParseNotes(XmlElement element)
        {
            foreach (var childElement in element.ChildNodes.OfType<XmlElement>())
            {
                switch (childElement.Name)
                {
                    case "Note":
                        var note = this.ParseNote(childElement);
                        this.novel.Notes.Add(note);
                        break;
                }
            }

            return;
        }

        protected Note ParseNote(XmlElement element)
        {
            var note = new Note();
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

        public static Novel Read(string path)
        {
            var reader = new NovelReader();
            return reader.ReadDocument(path);
        }
    }
}
