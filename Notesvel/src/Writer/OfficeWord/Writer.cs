//********************************
// (c) 2026 Ada Maynek
// This software is released under the MIT License.
//********************************
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using System.Text;
using System.Text.RegularExpressions;

namespace Maynek.Notesvel.Writer.OfficeWord
{
    internal class Writer
    {
        private enum AnalysisState
        {
            ANALYSIS_STATE_BODY = 0,
            ANALYSIS_STATE_RUBY_BASE = 1,
            ANALYSIS_STATE_RUBY_TEXT = 2,
        }

        private static readonly string HeadlineReplace = @"${TEXT}";
        private static readonly string RubyReplace = @"｜${WORD}《${RUBY}》";
        private static readonly string LinkReplace = "${WORD}";
        public string InputEpisodeDirectory { get; set; } = string.Empty;
        public string OutputWordDirectory { get; set; } = string.Empty;
        public string OutputWordFileName { get; set; } = string.Empty;
        public string TemplatePath { get; set; } = string.Empty;

        private static Paragraph GetDocumentParagraph(string inputText)
        {
            var documentParagraph = new Paragraph();

            inputText = WriterUtil.HeadlineRegex.Replace(inputText, Writer.HeadlineReplace);
            inputText = WriterUtil.LinkRegex.Replace(inputText, Writer.LinkReplace);

            inputText = WriterUtil.RubyRegex.Replace(inputText, Writer.RubyReplace);
            inputText = Regex.Replace(inputText, WriterUtil.PointPattern, delegate (Match m)
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

            var state = AnalysisState.ANALYSIS_STATE_BODY;
            var textBuilder = new StringBuilder();
            var rubyBuilder = new StringBuilder();
            foreach (var c in inputText)
            {
                if (c == '\n')
                {
                    var run = new Run(new Text(textBuilder.ToString()));
                    documentParagraph.AppendChild(run);

                    var bk = new Break();
                    documentParagraph.AppendChild(bk);

                    state = AnalysisState.ANALYSIS_STATE_BODY;
                    textBuilder.Clear();
                    rubyBuilder.Clear();
                }
                else if (state == AnalysisState.ANALYSIS_STATE_BODY && c == '｜')
                {
                    var run = new Run(new Text(textBuilder.ToString()));
                    documentParagraph.AppendChild(run);

                    state = AnalysisState.ANALYSIS_STATE_RUBY_BASE;
                    textBuilder.Clear();
                    rubyBuilder.Clear();
                }
                else if (state == AnalysisState.ANALYSIS_STATE_RUBY_BASE)
                {
                    if (c == '《')
                    {
                        state = AnalysisState.ANALYSIS_STATE_RUBY_TEXT;
                    }
                    else
                    {
                        textBuilder.Append(c);
                    }
                }
                else if (state == AnalysisState.ANALYSIS_STATE_RUBY_TEXT)
                {
                    if (c == '》')
                    {
                        var ruby = new Ruby();

                        var rubyProperties = new RubyProperties(
                            new RubyAlign() { Val = RubyAlignValues.Center }
                        );
                        ruby.AppendChild(rubyProperties);

                        var rubyContent = new RubyContent();
                        var rubyRun = new Run();
                        rubyRun.AppendChild(new Text(rubyBuilder.ToString()));
                        rubyContent.AppendChild(rubyRun);
                        ruby.AppendChild(rubyContent);

                        var rubyBase = new RubyBase();
                        var baseRun = new Run();
                        baseRun.AppendChild(new Text(textBuilder.ToString()));
                        rubyBase.AppendChild(baseRun);
                        ruby.AppendChild(rubyBase);

                        documentParagraph.AppendChild(ruby);

                        state = AnalysisState.ANALYSIS_STATE_BODY;
                        textBuilder.Clear();
                        rubyBuilder.Clear();
                    }
                    else
                    {
                        rubyBuilder.Append(c);
                    }
                }
                else { 

                    textBuilder.Append(c);
                }
            }

           return documentParagraph;
        }

        private void WriteEpisode(Novel novel)
        {
            string outputPath = Path.Combine(this.OutputWordDirectory, this.OutputWordFileName);

            File.Copy(this.TemplatePath, outputPath, true);

            using (WordprocessingDocument wordDocument = WordprocessingDocument.Open(outputPath, true))
            {
                var mainPart = wordDocument.MainDocumentPart;
                //mainPart.Document = new Document();

                var documentBody = mainPart.Document.AppendChild(new Body());




                foreach (var chapter in novel.Chapters)
                {
                    foreach (var episode in chapter.Episodes)
                    {
                        string inputPath = Path.Combine(this.InputEpisodeDirectory, episode.Id + ".ntv");
                        var bodyText = WriterUtil.ReadFile(inputPath);

                        var paragraph = Writer.GetDocumentParagraph(bodyText);
                        documentBody.AppendChild(paragraph);
                    }
                }

                mainPart.Document.Save();
            }
        }

        public void Write(Novel novel)
        {
            if (!Directory.Exists(this.OutputWordDirectory))
            {
                Directory.CreateDirectory(this.OutputWordDirectory);
            }

            WriteEpisode(novel);
        }
    }
}
