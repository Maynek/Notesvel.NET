//********************************
// (c) 2024 Ada Maynek
// This software is released under the MIT License.
//********************************
using System.Text.Json.Serialization;

namespace Maynek.Notesvel.Writer.NextSite
{
    public class Note
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;
        [JsonPropertyName("title")]
        public string Title { get; set; } = string.Empty;
        [JsonPropertyName("summary")]
        public string Summary { get; set; } = string.Empty;
        [JsonPropertyName("body")]
        public string Body { get; set; } = string.Empty;

        public static Note Create(Notesvel.Note note, string summaryText, string bodyText)
        {
            var newSiteNote = new Note()
            {
                Id = note.Id,
                Title = note.Title,
                Summary = summaryText,
                Body = bodyText,
            };

            return newSiteNote;
        }
    }
}
