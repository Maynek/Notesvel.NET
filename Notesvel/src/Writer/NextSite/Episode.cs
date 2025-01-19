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
        [JsonPropertyName("prevId")]
        public string PrevId { get; set; } = string.Empty;
        [JsonPropertyName("nextId")]
        public string NextId { get; set; } = string.Empty;
        [JsonPropertyName("body")]
        public string Body { get; set; } = string.Empty;
        [JsonPropertyName("imageFile")]
        public string ImageFile { get; set; } = string.Empty;
        [JsonPropertyName("imageWidth")]
        public string ImageWidth { get; set; } = string.Empty;
        [JsonPropertyName("imageHeight")]
        public string ImageHeight { get; set; } = string.Empty;

        public static Episode Create(Notesvel.Episode episode, string bodyText)
        {
            var newSiteChapter = new Episode()
            {
                Id = episode.Id,
                Title = episode.Title,
                PrevId = episode.PrevId,
                NextId = episode.NextId,
                Body = bodyText,
                ImageFile = episode.NextImageFile,
                ImageWidth = episode.NextImageWidth,
                ImageHeight = episode.NextImageHeight,
            };

            return newSiteChapter;
        }
    }
}
