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
        public Glossary Glossary { get; } = new Glossary();
        public ChapterDictionary Chapters { get; } = [];

        public void SetEpisodePagenation()
        {
            for (int ci = 0; ci < this.Chapters.Count; ci++)
            {
                var chapter = this.Chapters[ci];
                for (int ei = 0; ei < chapter.Episodes.Count; ei++)
                {
                    var episode = chapter.Episodes[ei];

                    //Set Previous Id
                    if (ei - 1 > -1)
                    {
                        episode.PrevId = chapter.Episodes[ei - 1].Id;
                    }
                    else
                    {
                        for (int cj = ci - 1; cj >= 0; cj--)
                        {
                            if (this.Chapters[cj].Episodes.Count > 0)
                            {
                                episode.PrevId = this.Chapters[cj].Episodes[^1].Id;
                                break;
                            }
                        }
                    }

                    //Set Next Id
                    if (ei + 1 < chapter.Episodes.Count)
                    {
                        episode.NextId = chapter.Episodes[ei + 1].Id;
                    }
                    else
                    {
                        for (int cj = ci + 1; cj < this.Chapters.Count; cj++)
                        {
                            if (this.Chapters[cj].Episodes.Count > 0)
                            {
                                episode.NextId = this.Chapters[cj].Episodes[0].Id;
                                break;
                            }
                        }
                    }
                }
            }
        }
    }
}
