//********************************
// (c) 2024 Ada Maynek
// This software is released under the MIT License.
//********************************
namespace Maynek.Notesvel
{
    public class Novel
    {
        public string MainTitle { get; set; } = string.Empty;
        public string SubTitle { get; set; } = string.Empty;
        public EpisodeDictionary Episodes { get; set; } = [];
        public NoteDictionary Notes { get; set; } = [];
    }
}
