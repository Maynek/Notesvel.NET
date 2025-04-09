//********************************
// (c) 2024 Ada Maynek
// This software is released under the MIT License.
//********************************
using System.Text.RegularExpressions;
using System.Text;

namespace Maynek.Notesvel.Writer.Narou
{
    internal class Writer
    {
        private static readonly string HeadlineReplace = @"${TEXT}";
        private static readonly string RubyReplace = @"｜${WORD}《${RUBY}》";
        private static readonly string LinkReplace = "${WORD}";
        private static readonly string WikipediaReplace = "${WORD}";
        private static readonly string NoteReplace = @"${WORD}";

        public string InputEpisodeDirectory { get; set; } = string.Empty;
        public string OutputEpisodeDirectory { get; set; } = string.Empty;

        private static string ConvertBodyForNarou(string text)
        {
            text = WriterUtil.HeadlineRegex.Replace(text, Writer.HeadlineReplace);
            text = WriterUtil.RubyRegex.Replace(text, Writer.RubyReplace);

            text = Regex.Replace(text, WriterUtil.PointPattern, delegate (Match m)
            {
                var sb = new StringBuilder();
                var word = m.Groups["WORD"].ToString();

                for (int i = 0; i < word.Length; i++)
                {
                    var line = Writer.RubyReplace;
                    line = line.Replace("${WORD}", word[i].ToString());
                    line = line.Replace("${RUBY}", "・");
                    sb.Append(line);
                }

                return sb.ToString();
            });

            text = WriterUtil.LinkRegex.Replace(text, Writer.LinkReplace);

            return text;
        }

        private void WriteEpisode(Novel novel)
        {
            //Write Episode
            foreach (var chapter in novel.Chapters)
            {
                foreach (var episode in chapter.Episodes)
                {
                    string inputPath = Path.Combine(this.InputEpisodeDirectory, episode.Id + ".ntv");
                    var bodyText = WriterUtil.ReadFile(inputPath);

                    bodyText = WriterUtil.ConvertBodyForWebNovel(bodyText);
                    bodyText = Writer.ConvertBodyForNarou(bodyText);

                    string fileName = episode.Id + "_" + episode.Title + ".txt";

                    string outputPath = Path.Combine(this.OutputEpisodeDirectory, fileName);
                    File.WriteAllText(outputPath, bodyText);
                }
            }
        }

        public void Write(Novel novel)
        {
            if (!Directory.Exists(this.OutputEpisodeDirectory))
            {
                Directory.CreateDirectory(this.OutputEpisodeDirectory);
            }

            WriteEpisode(novel);
        }
    }
}
