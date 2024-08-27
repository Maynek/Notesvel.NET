//********************************
// (c) 2024 Ada Maynek
// This software is released under the MIT License.
//********************************
using System.Text;
using System.Text.RegularExpressions;

namespace Maynek.Notesvel.Writer
{
    public class WriterUtil
    {
        private class ParenthesesState
        {
            private readonly List<Char> parentheses = [];

            public void Push(char c)
            {
                this.parentheses.Add(c);
            }

            public void Pop()
            {
                this.parentheses.RemoveAt(this.parentheses.Count - 1);
            }

            public char GetLastParenthesis()
            {
                if (this.parentheses.Count == 0)
                {
                    return ' ';
                }
                return this.parentheses[^1];
            }

            public bool IsInParentheses()
            {
                return (this.parentheses.Count > 0);
            }

            public bool IsInParentheses(char c)
            {
                return this.IsInParentheses() && (this.GetLastParenthesis() == c);
            }
        }

        private static readonly RegexOptions Options = RegexOptions.Singleline | RegexOptions.Compiled;

        public static readonly string HeadlinePattern = @"^(?<HEAD>#+?)\s+(?<TEXT>.+?)\n";
        public static readonly Regex HeadlineRegex;

        public static readonly string RubyPattern = @"<<(?<WORD>.+?)\|(?<RUBY>.+?)>>";
        public static readonly Regex RubyRegex;

        public static readonly string LinkPattern = @"\[\[(?<WORD>.+?)=>(?<URL>.+?)\]\]";
        public static readonly Regex LinkRegex;

        public static readonly string WikipediaPattern = @"\[\((?<WORD>.+?)=>(?<TITLE>.+?)\)\]";
        public static readonly Regex WikipediaRegex;

        public static readonly string NotePattern = @"\[\{(?<WORD>.+?)=>(?<ID>.+?)\}\]";
        public static readonly Regex NoteRegex;

        static WriterUtil()
        {
            HeadlineRegex = new Regex(HeadlinePattern, Options);
            RubyRegex = new Regex(RubyPattern, Options);
            LinkRegex = new Regex(LinkPattern, Options);
            WikipediaRegex = new Regex(WikipediaPattern, Options);
            NoteRegex = new Regex(NotePattern, Options);
        }

        public static string ConvertBodyForAll(string text)
        {
            text = text.Replace("\r\n", "\n");

            text = text.Replace("--", "――");
            text = text.Replace("...", "……");

            return text;
        }

        public static string ConvertBodyForWebNovel(string text)
        {
            var sb = new StringBuilder();

            var textLines = text.Split("\n");
            foreach (var line in textLines)
            {
                var sbLine = new StringBuilder();
                var lineArray = line.ToCharArray();

                int startIndex = 0;
                if (lineArray.Length > 1 && lineArray[0] == '\u3000')
                {
                    startIndex = 1;
                }

                var ps = new ParenthesesState();

                var isAfterLineBreak = false;

                for (int i = startIndex; i < lineArray.Length; i++)
                {
                    var c = lineArray[i];

                    if (isAfterLineBreak && c == '\u3000')
                    {
                        continue;
                    }

                    switch (c)
                    {
                        case '「':
                            sbLine.Append(lineArray[i]);
                            ps.Push('」');
                            break;

                        case '『':
                            sbLine.Append(lineArray[i]);
                            ps.Push('』');
                            break;

                        case '（':
                            sbLine.Append(lineArray[i]);
                            ps.Push('）');
                            break;                            

                        case '」':
                        case '』':
                        case '）':
                            sbLine.Append(lineArray[i]);
                            if (ps.IsInParentheses(c))
                            {
                                ps.Pop();
                            }                            
                            break;

                        case '。':
                        case '？':
                        case '！':
                            sbLine.Append(lineArray[i]);
                            if (!ps.IsInParentheses() && i != lineArray.Length - 1)
                            {
                                isAfterLineBreak = true;
                                sbLine.Append('\n');
                            }
                            break;

                        default:
                            sbLine.Append(lineArray[i]);
                            break;

                    }
                }

                sbLine.Append('\n');
                sbLine.Append('\n');

                sb.Append(sbLine);            
            }
                       
            
            return sb.ToString();
        }

        public static string ReadFile(string path)
        {
            var text = File.ReadAllText(path);
            text = WriterUtil.ConvertBodyForAll(text);

            return text;
        }

    }
}