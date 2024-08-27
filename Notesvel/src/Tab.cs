//********************************
// (c) 2024 Ada Maynek
// This software is released under the MIT License.
//********************************
namespace Maynek.Notesvel
{
    public class TabDictionary : MyDictionaryBase<Tab>
    {
        public void Add(Tab tab)
        {
            this.Add(tab.Id, tab);
        }
    }

    public class Tab
    {
        public bool Enabled { get; set; } = true;
        public bool Visible { get; set; } = true;
        public string Id { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public NoteDictionary Notes { get; } = [];
    }
}
