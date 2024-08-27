//********************************
// (c) 2024 Ada Maynek
// This software is released under the MIT License.
//********************************
using System.Data;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.RegularExpressions;
using static System.Net.Mime.MediaTypeNames;

namespace Maynek.Notesvel.Writer.MySite
{
    public class MySiteWriter
    {
        private static readonly JsonSerializerOptions SerializerOptions;
        private static readonly string HeadlineReplace = @"<h${NUM}>${TEXT}</h${NUM}>";
        private static readonly string RubyReplace = @"<ruby>${WORD}<rt>${RUBY}</rt></ruby>";
        private static readonly string LinkReplace = "<nv-link url=\"${URL}\">${WORD}</nv-link>";
        private static readonly string WikipediaReplace = "<nv-wiki title=\"${TITLE}\">${WORD}</nv-wiki>";
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

        private static string GetJsonSerializedText(object obj)
        {
            return JsonSerializer.Serialize(obj, MySiteWriter.SerializerOptions).Replace("\r\n", "\n");
        }


        private static string ConvertBodyForMySite(string text)
        {
            text = Regex.Replace(text, WriterUtil.HeadlinePattern, delegate (Match m)
            {
                int level = m.Groups["HEAD"].ToString().Length;

                string ret = MySiteWriter.HeadlineReplace;
                ret = ret.Replace("${NUM}", level.ToString());
                ret = ret.Replace("${TEXT}", m.Groups["TEXT"].ToString());

                return ret;
            });

            text = WriterUtil.RubyRegex.Replace(text, MySiteWriter.RubyReplace);
            text = WriterUtil.LinkRegex.Replace(text, MySiteWriter.LinkReplace);
            text = WriterUtil.WikipediaRegex.Replace(text, MySiteWriter.WikipediaReplace);
            text = WriterUtil.NoteRegex.Replace(text, MySiteWriter.NoteReplace);

            text = text.Replace("\n", "<br/>");

            return text;
        }

        private void WriteEpisodesIndex(Novel novel)
        {
            var index = MySiteIndex.Create(novel);
            var jsonText = MySiteWriter.GetJsonSerializedText(index);

            string outputPath = Path.Combine(this.OutputEpisodeDirectory, @"_index.json");
            File.WriteAllText(outputPath, jsonText);
        }

        private void WriteEpisode(Novel novel)
        {
            foreach (var chapter in novel.Chapters)
            {
                foreach (var episode in chapter.Episodes)
                {
                    string inputPath = Path.Combine(this.InputEpisodeDirectory, episode.Id + ".ntv");
                    var bodyText = WriterUtil.ReadFile(inputPath);

                    bodyText = WriterUtil.ConvertBodyForWebNovel(bodyText);
                    bodyText = MySiteWriter.ConvertBodyForMySite(bodyText);

                    var newEpisode = MySiteEpisode.Create(episode, bodyText);
                    var jsonText = MySiteWriter.GetJsonSerializedText(newEpisode);

                    string outputPath = Path.Combine(this.OutputEpisodeDirectory, episode.Id + ".json");
                    File.WriteAllText(outputPath, jsonText);
                }
            }

        }

        private void WriteGlossary(Novel novel)
        {
            if (novel.Glossary.Enabled && novel.Glossary.Visible)
            {
                var g = MySiteGlossary.Create(novel);
                var jsonText = MySiteWriter.GetJsonSerializedText(g);

                string outputPath = Path.Combine(this.OutputEpisodeDirectory, @"note\_glossary.json");
                File.WriteAllText(outputPath, jsonText);
            }
        }

        private void WriteNote(Novel novel)
        {
            foreach (var tab in novel.Glossary.Tabs)
            {
                if (!tab.Enabled)
                {
                    continue;

                }

                foreach (var note in tab.Notes)
                {
                    if (!note.Enabled)
                    {
                        continue;
                    }
                    
                    string inputPath = Path.Combine(this.InputNoteDirectory, note.Id + ".ntv");
                    var fullText = WriterUtil.ReadFile(inputPath);

                    var textArray = fullText.Split(">>>>>>>>\n");

                    var summaryText = textArray[0];
                    var bodyText = String.Join("", textArray, 1, textArray.Length - 1);

                    summaryText = MySiteWriter.ConvertBodyForMySite(summaryText);
                    bodyText = MySiteWriter.ConvertBodyForMySite(bodyText);

                    var newNote = MySiteNote.Create(note, summaryText, bodyText);
                    var jsonText = MySiteWriter.GetJsonSerializedText(newNote);

                    string outputPath = Path.Combine(this.OutputNoteDirectory, note.Id + ".json");
                    File.WriteAllText(outputPath, jsonText);
                }
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
            this.WriteGlossary(novel);
            this.WriteNote(novel);
        }
    }
}