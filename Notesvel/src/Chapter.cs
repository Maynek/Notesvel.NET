//********************************
// (c) 2024 Ada Maynek
// This software is released under the MIT License.
//********************************
namespace Maynek.Notesvel
{
    public class ChapterDictionary : MyDictionaryBase<Chapter>
    {
        public void Add(Chapter chapter)
        {
            this.Add(chapter.Id, chapter);
        }
    }

    public class Chapter { 

        public string Id { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public EpisodeDictionary Episodes { get; } = [];
    }
}
