//********************************
// (c) 2024 Ada Maynek
// This software is released under the MIT License.
//********************************
using System.Text.Json.Serialization;

namespace Maynek.Notesvel.Writer.MySite
{
    public class MySiteIndex
    {
        public class SiteEpisodeItem
        {
            [JsonPropertyName("id")]
            public string Id { get; set; } = string.Empty;
            [JsonPropertyName("title")]
            public string Title { get; set; } = string.Empty;
        }

        public class SiteChapterItem
        {
            [JsonPropertyName("id")]
            public string Id { get; set; } = string.Empty;
            [JsonPropertyName("title")]
            public string Title { get; set; } = string.Empty;
            [JsonPropertyName("episodes")]
            public IList<SiteEpisodeItem> Episodes { get; set; } = [];
        }

        [JsonPropertyName("maintitle")]
        public string MainTitle { get; set; } = string.Empty;
        [JsonPropertyName("subtitle")]
        public string SubTitle { get; set; } = string.Empty;
        [JsonPropertyName("glossary")]
        public string Glossary { get; set; } = string.Empty;
        [JsonPropertyName("chapters")]
        public IList<SiteChapterItem> Chapters { get; set; } = [];

        public static MySiteIndex Create(Novel novel)
        {
            var siteIndex = new MySiteIndex()
            {
                MainTitle = novel.MainTitle,
                SubTitle = novel.SubTitle
            };

            if (novel.Glossary.Enabled && novel.Glossary.Visible)
            {
                siteIndex.Glossary = novel.Glossary.Title;
            }
            else
            {
                siteIndex.Glossary = string.Empty;
            }

            foreach(var chapter in novel.Chapters)
            {
                var newChapter = new SiteChapterItem()
                {
                    Id = chapter.Id,
                    Title = chapter.Title
                };
                siteIndex.Chapters.Add(newChapter);

                foreach (var episode in chapter.Episodes)
                {
                    var newEpisode = new SiteEpisodeItem()
                    {
                        Id = episode.Id,
                        Title = episode.Title
                    };
                    newChapter.Episodes.Add(newEpisode);
                }
            }

            return siteIndex;
        }

    }
}
