//********************************
// (c) 2024 Ada Maynek
// This software is released under the MIT License.
//********************************
using System.Text.Json.Serialization;

namespace Maynek.Notesvel.Other
{
    public class MySite
    {
        public class EpisodeIdListItem
        {
            [JsonPropertyName("novelId")]
            public string NovelId { get; set; } = string.Empty;
            [JsonPropertyName("episodeId")]
            public string EpisodeId { get; set; } = string.Empty;
        }

        public class NoteIdListItem
        {
            [JsonPropertyName("novelId")]
            public string NovelId { get; set; } = string.Empty;
            [JsonPropertyName("noteId")]
            public string NoteId { get; set; } = string.Empty;
        }

        public List<string> NovelIdLIst { get; } = [];

        public IList<EpisodeIdListItem> EpisodeIdList { get; } = [];

        [JsonPropertyName("notes")]
        public IList<NoteIdListItem> NoteIdList { get; } = [];


        public void AddPath(string novelId, Novel novel)
        {
            // Episode
            foreach(var chapter in novel.Chapters)
            {
                foreach(var episode in chapter.Episodes)
                {
                    var newItem = new EpisodeIdListItem()
                    {
                        NovelId = novelId,
                        EpisodeId = episode.Id,
                    };
                    this.EpisodeIdList.Add(newItem);
                }
            }

            // Note
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

                    var newItem = new NoteIdListItem()
                    {
                        NovelId = novelId,
                        NoteId = note.Id,
                    };
                    this.NoteIdList.Add(newItem);
                }
            }
        }
    }
}
