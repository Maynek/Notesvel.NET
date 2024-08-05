//********************************
// (c) 2024 Ada Maynek
// This software is released under the MIT License.
//********************************
using System.Text.Json.Serialization;

namespace Maynek.Notesvel.Writer.MySite
{
    public class MySiteIndex
    {
        public class SiteEpisodesItem
        {
            [JsonPropertyName("id")]
            public string Id { get; set; } = string.Empty;
            [JsonPropertyName("title")]
            public string Title { get; set; } = string.Empty;
        }

        [JsonPropertyName("maintitle")]
        public string MainTitle { get; set; } = string.Empty;
        [JsonPropertyName("subtitle")]
        public string SubTitle { get; set; } = string.Empty;

        [JsonPropertyName("episodes")]
        public IList<SiteEpisodesItem> Episodes { get; set; } = [];

        public static MySiteIndex Create(Novel novel)
        {
            var episodesIndex = new MySiteIndex()
            {
                MainTitle = novel.MainTitle,
                SubTitle = novel.SubTitle
            };

            foreach(Episode episode in novel.Episodes)
            {
                var newEpisodes = new SiteEpisodesItem()
                {
                    Id = episode.Id,
                    Title = episode.Title
                };
                episodesIndex.Episodes.Add(newEpisodes);
            }

            return episodesIndex;
        }

    }
}
