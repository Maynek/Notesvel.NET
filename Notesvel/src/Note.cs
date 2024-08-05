//********************************
// (c) 2024 Ada Maynek
// This software is released under the MIT License.
//********************************
namespace Maynek.Notesvel
{
    public class NoteDictionary : MyDictionaryBase<Note>
    {
        public void Add(Note note)
        {
            this.Add(note.Id, note);
        }
    }

    public class Note
    {
        public string Id { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
    }
}
