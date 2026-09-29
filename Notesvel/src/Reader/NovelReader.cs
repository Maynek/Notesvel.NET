//********************************
// (c) 2024 Ada Maynek
// This software is released under the MIT License.
//********************************
using DocumentFormat.OpenXml.Office2016.Drawing.Command;
using Maynek.Notesvel.Writer.NextSite;
using System.Runtime.CompilerServices;
using System.Xml;

namespace Maynek.Notesvel.Reader
{
    public class InvalidFormatException : Exception { }

    public class NovelReader
    {
        public static Novel Read(string path, string novelId)
        {
            var xmlDocument = new XmlDocument();
            xmlDocument.Load(path);

            var element = xmlDocument.DocumentElement;
            if (element == null)
            {
                throw new InvalidFormatException();
            }

            var novel = new Novel();
            novel.Id = novelId;

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

                    case "Work":
                        NovelReader.ParseWork(childElement, novel);
                        break;

                    case "Chapters":
                        NovelReader.ParseChapters(childElement, novel);
                        break;

                    case "Glossary":
                        NovelReader.ParseGlossary(childElement, novel);
                        break;
                }
            }

            return novel;
        }

        protected static void ParseWork(XmlElement element, Novel novel)
        {
            var work = new Work();

            if (element.HasAttribute("Enabled"))
            {
                work.Enabled = element.GetAttribute("Enabled").ToLower() == "true";
            }

            if (element.HasAttribute("Target"))
            {
                var typeValue = element.GetAttribute("Target").ToLower();

                switch (typeValue)
                {
                    case "nextsite":
                        work.Target = WorkTargetType.NextSite;
                        break;

                    case "word":
                        work.Target = WorkTargetType.OfficeWord;
                        break;

                    case "narou":
                        work.Target = WorkTargetType.ServiceNarou;
                        break;

                    case "kakuyomu":
                        work.Target = WorkTargetType.ServiceKakuyomu;
                        break;

                    case "alphapolis":
                        work.Target = WorkTargetType.ServiceAlphaPolis;
                        break;
                }
            }

            if (element.HasAttribute("OutputFileName"))
            {
                work.OutputFileName = element.GetAttribute("OutputFileName");
            }

            if (element.HasAttribute("TemplateFileName"))
            {
                work.TemplateFileName= element.GetAttribute("TemplateFileName");
            }

            novel.AddWork(work);
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

                    case "Episode":
                        var episode = new Episode();
                        NovelReader.ParseEpisode(childElement, episode);
                        chapter.Episodes.Add(episode);
                        break;
                }
            }

            return chapter;
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

                    case "NextImage":
                        if (childElement.HasAttribute("File"))
                        {
                            episode.NextImageFile = childElement.GetAttribute("File");
                        }

                        if (childElement.HasAttribute("Width"))
                        {
                            episode.NextImageWidth = childElement.GetAttribute("Width");
                        }

                        if (childElement.HasAttribute("Height"))
                        {
                            episode.NextImageHeight = childElement.GetAttribute("Height");
                        }

                        break;
                }
            }

            return episode;
        }

        protected static void ParseGlossary(XmlElement element, Novel novel)
        {
            var glossary = novel.Glossary;

            if (element.HasAttribute("Enabled"))
            {
                glossary.Enabled = element.GetAttribute("Enabled") == "true";
            }

            if (element.HasAttribute("Visible"))
            {
                glossary.Visible = element.GetAttribute("Visible") == "true";
            }            

            foreach (var childElement in element.ChildNodes.OfType<XmlElement>())
            {
                switch (childElement.Name)
                {
                    case "Title":
                        glossary.Title = childElement.InnerText;
                        break;

                    case "Tab":
                        var tab = new Tab();
                        NovelReader.ParseTab(childElement, tab);
                        glossary.Tabs.Add(tab);
                        break;
                }
            }

            return;
        }

        protected static void ParseTab(XmlElement element, Tab tab)
        {
            tab.Id = element.GetAttribute("Id");

            foreach (var childElement in element.ChildNodes.OfType<XmlElement>())
            {
                switch (childElement.Name)
                {
                    case "Title":
                        tab.Title = childElement.InnerText;
                        break;

                    case "Note":
                        var note = new Note();
                        NovelReader.ParseNote(childElement, note);
                        tab.Notes.Add(note);
                        break;
                }
            }

            return;
        }

        protected static void ParseNote(XmlElement element, Note note)
        {
            note.Id = element.GetAttribute("Id");

            if (element.HasAttribute("Enabled"))
            {
                note.Enabled = element.GetAttribute("Enabled") == "true";
            }

            if (element.HasAttribute("Visible"))
            {
                note.Visible = element.GetAttribute("Visible") == "true";
            }

            foreach (var childElement in element.ChildNodes.OfType<XmlElement>())
            {
                switch (childElement.Name)
                {
                    case "Title":
                        note.Title = childElement.InnerText;
                        break;
                }
            }

            return;
        }
    }
}
