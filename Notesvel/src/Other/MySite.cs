//********************************
// (c) 2024 Ada Maynek
// This software is released under the MIT License.
//********************************
using System.Text.Json.Serialization;

namespace Maynek.Notesvel.Other
{
    public class MySite
    {
        public class IndexPageListItem
        {
            [JsonPropertyName("novelId")]
            public string NovelId { get; set; } = string.Empty;
        }

        public class EpisodePageListItem
        {
            [JsonPropertyName("novelId")]
            public string NovelId { get; set; } = string.Empty;
            [JsonPropertyName("episodeId")]
            public string EpisodeId { get; set; } = string.Empty;
        }

        public class GlossaryPageListItem
        {
            [JsonPropertyName("novelId")]
            public string NovelId { get; set; } = string.Empty;
        }

        public class NotePageListItem
        {
            [JsonPropertyName("novelId")]
            public string NovelId { get; set; } = string.Empty;
            [JsonPropertyName("noteId")]
            public string NoteId { get; set; } = string.Empty;
        }

        public IList<IndexPageListItem> NovelIdList { get; } = [];
        public IList<EpisodePageListItem> EpisodePageList { get; } = [];
        public IList<GlossaryPageListItem> GlossaryPageList { get; } = [];
        public IList<NotePageListItem> NotePageList { get; } = [];

        public void AddNovelId(string novelId)
        {
            var newItem = new IndexPageListItem()
            {
                NovelId = novelId,
            };
            this.NovelIdList.Add(newItem);
        }

        public void AddPath(string novelId, Novel novel)
        {
            // Episode
            foreach(var chapter in novel.Chapters)
            {
                foreach(var episode in chapter.Episodes)
                {
                    var newItem = new EpisodePageListItem()
                    {
                        NovelId = novelId,
                        EpisodeId = episode.Id,
                    };
                    this.EpisodePageList.Add(newItem);
                }
            }

            // Note
            bool hasNote = false;
            foreach (var tab in novel.Glossary.Tabs)
            {
                if (!tab.Enabled)
                {
                    continue;
                }

                foreach (var note in tab.Notes)
                {
                    if (!note.Enabled)
                    {
                        continue;
                    }

                    var newItem = new NotePageListItem()
                    {
                        NovelId = novelId,
                        NoteId = note.Id,
                    };
                    this.NotePageList.Add(newItem);

                    hasNote = true;
                }
            }

            if (novel.Glossary.Enabled && hasNote)
            {
                var newItem = new GlossaryPageListItem()
                {
                    NovelId = novelId,
                };
                this.GlossaryPageList.Add(newItem);
            }
        }
    }
}
