//********************************
// (c) 2024 Ada Maynek
// This software is released under the MIT License.
//********************************
using Maynek.Notesvel.Reader;
using Maynek.Notesvel.Writer.MySite;
using Maynek.Notesvel.Writer.Narou;

namespace Maynek.Notesvel.Console
{
    internal class Program
    {
        class Parameter
        {
            public string InputDirectory { get; private set; } = string.Empty;
            public string OutputDirectory { get; private set; } = string.Empty;
            
            public static Parameter CreateParameter(string[] args)
            {
                var parameter = new Parameter();

                var parser = new Parser();

                parser.AddOptionDefinition(new OptionDefinition("-i", "--input")
                {
                    Type = OptionType.RequireValue,
                    EventHandler = delegate (object sender, OptionEventArgs e)
                    {
                        parameter.InputDirectory = e.Value;
                    }
                });

                parser.AddOptionDefinition(new OptionDefinition("-o", "--output")
                {
                    Type = OptionType.RequireValue,
                    EventHandler = delegate (object sender, OptionEventArgs e)
                    {
                        parameter.OutputDirectory = e.Value;
                    }
                });

                parser.Parse(args);

                return parameter;
            }


            static void Main(string[] args)
            {
                var param = Parameter.CreateParameter(args);
                if (param.InputDirectory != string.Empty && param.OutputDirectory != string.Empty)
                {
                    string inputEpisodeDirectory = Path.Combine(param.InputDirectory, @"episodes\");
                    string inputNoteDirectory = Path.Combine(param.InputDirectory, @"notes\");

                    //Read index.xml
                    var novel = NovelReader.Read(Path.Combine(param.InputDirectory, "index.xml"));

                    //Setup Novel
                    novel.SetEpisodePagenation();


                    //Write for My Web Site.
                    new MySiteWriter()
                    {
                        InputEpisodeDirectory = inputEpisodeDirectory,
                        InputNoteDirectory = inputNoteDirectory,
                        OutputEpisodeDirectory = Path.Combine(param.OutputDirectory, @"site\"),
                        OutputNoteDirectory = Path.Combine(param.OutputDirectory, @"site\note\")
                    }.Write(novel);

                    //Write for Narou.
                    new NarouWriter()
                    {
                        InputEpisodeDirectory = inputEpisodeDirectory,
                        OutputEpisodeDirectory = Path.Combine(param.OutputDirectory, @"narou\"),
                    }.Write(novel);

                }
            }
        }
    }
}
