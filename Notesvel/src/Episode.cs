//********************************
// (c) 2024 Ada Maynek
// This software is released under the MIT License.
//********************************
namespace Maynek.Notesvel
{
    public class EpisodeDictionary : MyDictionaryBase<Episode>
    {
        public void Add(Episode episode)
        {
            this.Add(episode.Id, episode);
        }
    }

    public class Episode
    {
        public string Id { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string PrevId { get; set; } = string.Empty;
        public string NextId { get; set; } = string.Empty;

        public string NextImageFile { get; set; } = string.Empty;
        public string NextImageWidth { get; set; } = string.Empty;
        public string NextImageHeight { get; set; } = string.Empty;

    }
}
