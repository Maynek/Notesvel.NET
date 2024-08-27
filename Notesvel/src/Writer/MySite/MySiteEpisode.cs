//********************************
// (c) 2024 Ada Maynek
// This software is released under the MIT License.
//********************************
using System.Text.Json.Serialization;

namespace Maynek.Notesvel.Writer.MySite
{
    public class MySiteEpisode
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;
        [JsonPropertyName("title")]
        public string Title { get; set; } = string.Empty;
        [JsonPropertyName("prevId")]
        public string PrevId { get; set; } = string.Empty;
        [JsonPropertyName("nextId")]
        public string NextId { get; set; } = string.Empty;
        [JsonPropertyName("body")]
        public string Body { get; set; } = string.Empty;

        public static MySiteEpisode Create(Episode episode, string bodyText)
        {
            var newSiteChapter = new MySiteEpisode()
            {
                Id = episode.Id,
                Title = episode.Title,
                PrevId = episode.PrevId,
                NextId = episode.NextId,
                Body = bodyText,
            };

            return newSiteChapter;
        }
    }
}
