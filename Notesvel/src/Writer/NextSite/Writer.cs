//********************************
// (c) 2024 Ada Maynek
// This software is released under the MIT License.
//********************************
using System.Reflection.Emit;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace Maynek.Notesvel.Writer.NextSite
{
    public class Writer
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

        static Writer()
        {
            SerializerOptions = new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                WriteIndented = true,
            };
        }

        private static string GetJsonSerializedText(object obj)
        {
            return JsonSerializer.Serialize(obj, Writer.SerializerOptions).Replace("\r\n", "\n");
        }


        private static string ConvertBodyForMySite(string text)
        {
            text = Regex.Replace(text, WriterUtil.HeadlinePattern, delegate (Match m)
            {
                int level = m.Groups["HEAD"].ToString().Length;

                string ret = Writer.HeadlineReplace;
                ret = ret.Replace("${NUM}", level.ToString());
                ret = ret.Replace("${TEXT}", m.Groups["TEXT"].ToString());

                return ret;
            });

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
            text = WriterUtil.WikipediaRegex.Replace(text, Writer.WikipediaReplace);
            text = WriterUtil.NoteRegex.Replace(text, Writer.NoteReplace);

            text = text.Replace("\n", "<br/>");

            return text;
        }

        private void WriteEpisodesIndex(Novel novel)
        {
            var index = Index.Create(novel);
            var jsonText = Writer.GetJsonSerializedText(index);

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
                    bodyText = Writer.ConvertBodyForMySite(bodyText);

                    var newEpisode = Episode.Create(episode, bodyText);
                    var jsonText = Writer.GetJsonSerializedText(newEpisode);

                    string outputPath = Path.Combine(this.OutputEpisodeDirectory, episode.Id + ".json");
                    File.WriteAllText(outputPath, jsonText);
                }
            }

        }

        private void WriteGlossary(Novel novel)
        {
            if (novel.Glossary.Enabled && novel.Glossary.Visible)
            {
                var g = Glossary.Create(novel);
                var jsonText = Writer.GetJsonSerializedText(g);

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


                    var summaryTextSB = new StringBuilder();
                    var bodyTextSB = new StringBuilder();

                    var isSummary = false;
                    foreach (var textLine in fullText.Split("\n"))
                    {
                        var hasEnter = true;
                        var newLine = textLine;

                        if (textLine.IndexOf("#HEAD") == 0)
                        {
                            hasEnter = false;

                            var headLevel = textLine.Substring("#HEAD".Length, 1);
                            var headtext = textLine.Substring("#HEADx:".Length);

                            newLine = "<h" + headLevel + "> " + headtext + "</h" + headLevel + ">";
                        }
                        else if (isSummary == false && textLine == "#SUMMRY")
                        {
                            isSummary = true;
                            continue;
                        }
                        else if (isSummary == true && textLine == "#END")
                        {
                            isSummary = false;
                            continue;
                        }

                        if (isSummary)
                        {
                            summaryTextSB.Append(newLine);
                            if (hasEnter)
                            {
                                summaryTextSB.Append('\n');
                            }
                        }
                        else
                        {
                            bodyTextSB.Append(newLine);
                            if ( (hasEnter))
                            {
                                bodyTextSB.Append('\n');
                            }
                        }
                    }

                    var summaryText = summaryTextSB.ToString();
                    summaryText = Writer.ConvertBodyForMySite(summaryText);

                    var bodyText = bodyTextSB.ToString();
                    bodyText = Writer.ConvertBodyForMySite(bodyText);

                    var newNote = Note.Create(note, summaryText, bodyText);
                    var jsonText = Writer.GetJsonSerializedText(newNote);

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
