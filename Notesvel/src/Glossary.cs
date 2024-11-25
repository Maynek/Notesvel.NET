//********************************
// (c) 2024 Ada Maynek
// This software is released under the MIT License.
//********************************
namespace Maynek.Notesvel
{
    public class Glossary
    {
        public bool Enabled { get; set; } = true;
        public bool Visible { get; set; } = true;
        public string Title { get; set; } = "Glossary";

        public TabDictionary Tabs { get; } = [];
    }
}
