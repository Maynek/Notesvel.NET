//********************************
// (c) 2024 Ada Maynek
// This software is released under the MIT License.
//********************************
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace Maynek.Notesvel.Writer.MySite
{
    public class MySiteWriter
    {
        private static readonly JsonSerializerOptions SerializerOptions;
        private static readonly string HeadlineReplace = @"<h${NUM}>${TEXT}</h${NUM}>";
        private static readonly string RubyReplace = @"<ruby>${WORD}<rt>${RUBY}</rt></ruby>";
        private static readonly string NoteReplace = "<nv-note id=\"${ID}\">${WORD}</nv-note>";

        public string InputEpisodeDirectory { get; set; } = string.Empty;
        public string InputNoteDirectory { get; set; } = string.Empty;
        public string OutputEpisodeDirectory { get; set; } = string.Empty;
        public string OutputNoteDirectory { get; set; } = string.Empty;

        static MySiteWriter()
        {
            SerializerOptions = new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                WriteIndented = true,
            };
        }

        private static string ConvertBodyForSite(string text)
        {
            text = WriterUtil.ConvertBody(text);
            text = WriterUtil.ConvertBodyForWeb(text);


            text = Regex.Replace(text, WriterUtil.HeadlinePattern, delegate (Match m)
            {
                int level = m.Groups["HEAD"].ToString().Length;

                string ret = MySiteWriter.HeadlineReplace;
                ret = ret.Replace("${NUM}", level.ToString());
                ret = ret.Replace("${TEXT}", m.Groups["TEXT"].ToString());

                return ret;
            });

            text = WriterUtil.RubyRegex.Replace(text, MySiteWriter.RubyReplace);
            text = WriterUtil.NoteRegex.Replace(text, MySiteWriter.NoteReplace);

            text = text.Replace("\n", "<br/>");

            return text;
        }

        private void WriteEpisodesIndex(Novel novel)
        {
            var index = MySiteIndex.Create(novel);
            var jsonText = JsonSerializer.Serialize(index, MySiteWriter.SerializerOptions).Replace("\r\n", "\n");

            string outputPath = Path.Combine(this.OutputEpisodeDirectory, @"index.json");
            File.WriteAllText(outputPath, jsonText);
        }

        private void WriteEpisode(Novel novel)
        {
            foreach (var episode in novel.Episodes)
            {
                string inputPath = Path.Combine(this.InputEpisodeDirectory, episode.Id + ".ntv");
                var bodyText = File.ReadAllText(inputPath);

                bodyText = MySiteWriter.ConvertBodyForSite(bodyText);

                var newChapter = MySiteEpisode.Create(episode, bodyText);
                var jsonText = JsonSerializer.Serialize(newChapter, MySiteWriter.SerializerOptions).Replace("\r\n", "\n");

                string outputPath = Path.Combine(this.OutputEpisodeDirectory, episode.Id + ".json");
                File.WriteAllText(outputPath, jsonText);
            }
        }

        private void WriteNote(Novel novel)
        {
            foreach (var note in novel.Notes)
            {
                string inputPath = Path.Combine(this.InputNoteDirectory, note.Id + ".ntv");
                var bodyText = File.ReadAllText(inputPath);
                
                bodyText = MySiteWriter.ConvertBodyForSite(bodyText);

                var newNote = MySiteNote.Create(note, bodyText);
                var jsonText = JsonSerializer.Serialize(newNote, MySiteWriter.SerializerOptions).Replace("\r\n", "\n");

                string outputPath = Path.Combine(this.OutputNoteDirectory, note.Id + ".json");
                File.WriteAllText(outputPath, jsonText);
            }
        }

        public void Write(Novel novel)
        {
            if (!Directory.Exists(this.OutputEpisodeDirectory))
            {
                Directory.CreateDirectory(this.OutputEpisodeDirectory);
            }

            if (!Directory.Exists(this.OutputNoteDirectory))
            {
                Directory.CreateDirectory(this.OutputNoteDirectory);
            }

            this.WriteEpisodesIndex(novel);
            this.WriteEpisode(novel);
            this.WriteNote(novel);
        }
    }
}
