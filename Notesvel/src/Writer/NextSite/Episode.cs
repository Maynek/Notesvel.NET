//********************************
// (c) 2024 Ada Maynek
// This software is released under the MIT License.
//********************************
using System.Text.Json.Serialization;

namespace Maynek.Notesvel.Writer.NextSite
{
    public class Episode
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;
        [JsonPropertyName("title")]
        public string Title { get; set; } = string.Empty;
        [JsonPropertyName("image")]
        public string Image { get; set; } = string.Empty;
        [JsonPropertyName("prevId")]
        public string PrevId { get; set; } = string.Empty;
        [JsonPropertyName("nextId")]
        public string NextId { get; set; } = string.Empty;
        [JsonPropertyName("body")]
        public string Body { get; set; } = string.Empty;

        public static Episode Create(Notesvel.Episode episode, string bodyText)
        {
            var newSiteChapter = new Episode()
            {
                Id = episode.Id,
                Title = episode.Title,
                Image = episode.Image,
                PrevId = episode.PrevId,
                NextId = episode.NextId,
                Body = bodyText,
            };

            return newSiteChapter;
        }
    }
}
