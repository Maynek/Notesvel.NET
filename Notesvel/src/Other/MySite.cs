//********************************
// (c) 2024 Ada Maynek
// This software is released under the MIT License.
//********************************
namespace Maynek.Notesvel.Other
{
    public class MySite
    {
        public class SitePath
        {
            public string NovelId { get; set; } = string.Empty;
            public string EpisodeId { get; set; } = string.Empty;
        }


        public List<string> NovelIdLIst { get; } = [];

        public List<SitePath> SitePathList { get; } = [];

        public void AddPathList(string novelId, Novel novel)
        {
            foreach(var chapter in novel.Chapters)
            {
                foreach(var episode in chapter.Episodes)
                {
                    var newSitePath = new SitePath()
                    {
                        NovelId = novelId,
                        EpisodeId = episode.Id,
                    };
                    this.SitePathList.Add(newSitePath);
                }
            }
        }
    }
}
