//********************************
// (c) 2024 Ada Maynek
// This software is released under the MIT License.
//********************************
using System.Text.Json.Serialization;

namespace Maynek.Notesvel.Writer.NextSite
{
    public class Glossary
    {
        public class SiteNoteItem
        {
            [JsonPropertyName("id")]
            public string Id { get; set; } = string.Empty;
            [JsonPropertyName("title")]
            public string Title { get; set; } = string.Empty;
        }

        public class SiteTabItem
        {
            [JsonPropertyName("id")]
            public string Id { get; set; } = string.Empty;
            [JsonPropertyName("title")]
            public string Title { get; set; } = string.Empty;
            [JsonPropertyName("notes")]
            public IList<SiteNoteItem> Notes { get; set; } = [];
        }

        [JsonPropertyName("title")]
        public string Title { get; set; } = string.Empty;
        [JsonPropertyName("tabs")]
        public IList<SiteTabItem> Tabs { get; set; } = [];

        public static Glossary Create(Novel novel)
        {
            var siteGlossary = new Glossary()
            {
                Title = novel.Glossary.Title
            };

            foreach(var tab in novel.Glossary.Tabs)
            {
                if (!tab.Enabled || !tab.Visible)
                {
                    continue;
                }

                var newTab = new SiteTabItem()
                {
                    Id = tab.Id,
                    Title = tab.Title
                };
                siteGlossary.Tabs.Add(newTab);

                foreach (var note in tab.Notes)
                {
                    if (!note.Enabled || !note.Visible)
                    {
                        continue;
                    }

                    var newNote = new SiteNoteItem()
                    {
                        Id = note.Id,
                        Title = note.Title
                    };
                    newTab.Notes.Add(newNote);
                }
            }

            return siteGlossary;
        }
    }
}
