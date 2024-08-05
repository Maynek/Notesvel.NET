//********************************
// (c) 2024 Ada Maynek
// This software is released under the MIT License.
//********************************
using System.Text;

namespace Maynek.Notesvel.Writer.Narou
{
    internal class NarouWriter
    {
        private static readonly string HeadlineReplace = @"${TEXT}";
        private static readonly string RubyReplace = @"｜${WORD}《${RUBY}》";
        private static readonly string NoteReplace = @"${WORD}";

        public string InputEpisodeDirectory { get; set; } = string.Empty;
        public string OutputEpisodeDirectory { get; set; } = string.Empty;

        private static string ConvertBodyForNarou(string text)
        {
            text = WriterUtil.HeadlineRegex.Replace(text, NarouWriter.HeadlineReplace);
            text = WriterUtil.RubyRegex.Replace(text, NarouWriter.RubyReplace);
            text = WriterUtil.NoteRegex.Replace(text, NarouWriter.NoteReplace);

            return text;
        }

        private void WriteEpisode(Novel novel)
        {
            //Write Chapter
            foreach (var episode in novel.Episodes)
            {
                string inputPath = Path.Combine(this.InputEpisodeDirectory, episode.Id + ".ntv");

                var bodyText = File.ReadAllText(inputPath);

                bodyText = WriterUtil.ConvertBody(bodyText);
                bodyText = WriterUtil.ConvertBodyForWeb(bodyText);
                bodyText = NarouWriter.ConvertBodyForNarou(bodyText);

                string fileName = episode.Id + "_" + episode.Title + ".txt";

                string outputPath = Path.Combine(this.OutputEpisodeDirectory, fileName);
                File.WriteAllText(outputPath, bodyText);
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
