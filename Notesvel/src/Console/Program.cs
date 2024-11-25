//********************************
// (c) 2024 Ada Maynek
// This software is released under the MIT License.
//********************************
using Maynek.Notesvel.Reader;
using Maynek.Notesvel.Writer.MySite;
using Maynek.Notesvel.Writer.Narou;
using Maynek.Notesvel.Other;

namespace Maynek.Notesvel.Console
{
    internal class Program
    {
        class Parameter
        {
            //public string ProjectName { get; private set; } = string.Empty;
            public string InputRoot { get; private set; } = string.Empty;
            public string OutputRoot { get; private set; } = string.Empty;
            
            public static Parameter CreateParameter(string[] args)
            {
                var parameter = new Parameter();

                var parser = new Parser();

                //parser.AddOptionDefinition(new OptionDefinition("-p", "--project")
                //{
                //    Type = OptionType.RequireValue,
                //    EventHandler = delegate (object sender, OptionEventArgs e)
                //    {
                //        parameter.ProjectName = e.Value;
                //    }
                //});

                parser.AddOptionDefinition(new OptionDefinition("-i", "--input")
                {
                    Type = OptionType.RequireValue,
                    EventHandler = delegate (object sender, OptionEventArgs e)
                    {
                        parameter.InputRoot = e.Value;
                    }
                });

                parser.AddOptionDefinition(new OptionDefinition("-o", "--output")
                {
                    Type = OptionType.RequireValue,
                    EventHandler = delegate (object sender, OptionEventArgs e)
                    {
                        parameter.OutputRoot = e.Value;
                    }
                });

                parser.Parse(args);

                return parameter;
            }


            static void Main(string[] args)
            {
                var param = Parameter.CreateParameter(args);

                //if (param.ProjectName == string.Empty)
                //{
                //   System.Console.WriteLine("Project Name is not set.");
                //    return;
                //}

                if (param.InputRoot == string.Empty)
                {
                    System.Console.WriteLine("Input Directory is not set.");
                    return;
                }

                if (param.OutputRoot == string.Empty)
                {
                    System.Console.WriteLine("Output Directory is not set.");
                    return;
                }
                string inputSitePath = Path.Combine(param.InputRoot, "site.xml");

                //Read site.xml
                var site = MySiteReader.Read(inputSitePath);

                foreach (var novelId in site.NovelIdLIst)
                {
                    string inputDirectory = Path.Combine(param.InputRoot, novelId);
                    string inputEpisodeDirectory = Path.Combine(inputDirectory, @"episodes\");
                    string inputNoteDirectory = Path.Combine(inputDirectory, @"notes\");
                    string inputIndexPath = Path.Combine(inputDirectory, "index.xml");

                    //Read index.xml
                    var novel = NovelReader.Read(inputIndexPath);

                    //Setup Novel
                    novel.SetEpisodePagenation();

                    //Write for My Web Site.
                    string siteEpisodeDirectory = Path.Combine(param.OutputRoot, @"site\", novelId);
                    string siteNoteDirectory = Path.Combine(siteEpisodeDirectory, @"note\");
                    new MySiteWriter()
                    {
                        InputEpisodeDirectory = inputEpisodeDirectory,
                        InputNoteDirectory = inputNoteDirectory,
                        OutputEpisodeDirectory = siteEpisodeDirectory,
                        OutputNoteDirectory = siteNoteDirectory
                    }.Write(novel);

                    site.AddPathList(novelId, novel);


                    //Write for Narou.
                    string narouEpisodeDirectory = Path.Combine(param.OutputRoot, @"narou\", novelId);
                    new NarouWriter()
                    {
                        InputEpisodeDirectory = inputEpisodeDirectory,
                        OutputEpisodeDirectory = narouEpisodeDirectory,
                    }.Write(novel);

                }

                // Output MySite
                string siteDirectory = Path.Combine(param.OutputRoot, @"site\");
                MySiteUtil.Output(siteDirectory, site);

            }
        }
    }
}
