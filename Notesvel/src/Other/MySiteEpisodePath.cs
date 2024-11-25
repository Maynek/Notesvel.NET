//********************************
// (c) 2024 Ada Maynek
// This software is released under the MIT License.
//********************************
using System.Text.Json.Serialization;

namespace Maynek.Notesvel.Other
{
    public class MySiteEpisodePath
    {
        public class EpisodesItem
        {
            [JsonPropertyName("novelId")]
            public string NovelId { get; set; } = string.Empty;
            [JsonPropertyName("episodeId")]
            public string EpisodeId { get; set; } = string.Empty;
        }

        [JsonPropertyName("episodes")]
        public IList<EpisodesItem> Episodes { get; set; } = [];


        public static MySiteEpisodePath Create(MySite site)
        {
            var newList = new MySiteEpisodePath();
            foreach (var sitePath in site.SitePathList)
            {
                var newListItem = new EpisodesItem()
                {
                    NovelId = sitePath.NovelId,
                    EpisodeId = sitePath.EpisodeId,
                };
                newList.Episodes.Add(newListItem);
            }

            return newList;
        }
    }
}
