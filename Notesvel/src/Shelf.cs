//********************************
// (c) 2024 Ada Maynek
// This software is released under the MIT License.
//********************************
using System.Text.Json.Serialization;

namespace Maynek.Notesvel
{
    public class Shelf
    {
        public class ShelfItem
        {
            public string NovelId { get; set; } = string.Empty;
        }


        public IList<ShelfItem> ItemList { get; } = [];


        public void AddShelfItem(string novelId)
        {
            var newItem = new ShelfItem()
            {
                NovelId = novelId,
            };
            this.ItemList.Add(newItem);
        }        
    }
}
