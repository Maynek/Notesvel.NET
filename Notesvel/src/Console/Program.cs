//********************************
// (c) 2024 Ada Maynek
// This software is released under the MIT License.
//********************************
using Maynek.Notesvel.Reader;

namespace Maynek.Notesvel.Console
{
    internal class Program
    {
        class Parameter
        {
            public string InputRoot { get; private set; } = string.Empty;
            public string OutputRoot { get; private set; } = string.Empty;
            public string TemplateDir { get; private set; } = string.Empty;

            public static Parameter CreateParameter(string[] args)
            {
                var parameter = new Parameter();

                var parser = new Parser();

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

                parser.AddOptionDefinition(new OptionDefinition("-t", "--template")
                {
                    Type = OptionType.RequireValue,
                    EventHandler = delegate (object sender, OptionEventArgs e)
                    {
                        parameter.TemplateDir = e.Value;
                    }
                });

                parser.Parse(args);

                return parameter;
            }
        }

        class InputPaths
        {
            public string NovelDirectory { get; set; } = String.Empty;
            public string EpisodeDirectory { get; set; } = String.Empty;
            public string NoteDirectory { get; set; } = String.Empty;
            public string ImageDirectory { get; set; } = String.Empty;

            public string NovelPath { get; set; } = String.Empty;


            public static InputPaths CreatePaths(string inputRootDirectory, string novelId)
            {
                var paths = new InputPaths();

                paths.NovelDirectory = Path.Combine(inputRootDirectory, novelId);

                paths.EpisodeDirectory = Path.Combine(paths.NovelDirectory, @"episodes\");
                paths.NoteDirectory = Path.Combine(paths.NovelDirectory, @"notes\");
                paths.ImageDirectory = Path.Combine(paths.NovelDirectory, @"images\");

                paths.NovelPath = Path.Combine(paths.NovelDirectory, "novel.xml");

                return paths;
            }
        }

        static void Main(string[] args)
        {
            var param = Parameter.CreateParameter(args);

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
            var inputSitePath = Path.Combine(param.InputRoot, "shelf.xml");

            if (param.TemplateDir == string.Empty)
            {
                System.Console.WriteLine("Template Directory is not set.");
                return;
            }

            //Read site.xml
            var shelf = ShelfReader.Read(inputSitePath);

            if (Directory.Exists(param.OutputRoot))
            {
                Directory.Delete(param.OutputRoot, true);
            }

            foreach (var item in shelf.ItemList)
            {
                var novelId = item.NovelId;

                var inputPaths = InputPaths.CreatePaths(param.InputRoot, novelId);

                //Read novel.xml
                var novel = NovelReader.Read(inputPaths.NovelPath, novelId);

                //Setup Novel
                novel.SetEpisodePagenation();

                //Write
                foreach (var work in novel.Works)
                {
                    if (work.Enabled == false)
                    {
                        continue;
                    }

                    switch (work.Target)
                    {
                        case WorkTargetType.NextSite:
                            WriteNextSite(novel, param, inputPaths);
                            break;

                        case WorkTargetType.OfficeWord:
                            WriteOfficeWord(novel, work, param, inputPaths);
                            break;

                        case WorkTargetType.ServiceNarou:
                            WriteServiceNarou(novel, param, inputPaths);
                            break;

                        case WorkTargetType.ServiceKakuyomu:
                            WriteServiceKakuyomu(novel, param, inputPaths);
                            break;

                        case WorkTargetType.ServiceAlphaPolis:
                            WriteServiceAlphaPolis(novel, param, inputPaths);
                            break;
                    }
                }
            }
        }

        static void WriteNextSite(Novel novel, Parameter param, InputPaths inputPaths)
        {
            var nextSiteEpisodeDirectory = Path.Combine(param.OutputRoot, @"_nextsite\", novel.Id);
            var nextSiteNoteDirectory = Path.Combine(nextSiteEpisodeDirectory, @"note\");
            new Writer.NextSite.Writer()
            {
                InputEpisodeDirectory = inputPaths.EpisodeDirectory,
                InputNoteDirectory = inputPaths.NoteDirectory,
                OutputEpisodeDirectory = nextSiteEpisodeDirectory,
                OutputNoteDirectory = nextSiteNoteDirectory
            }.Write(novel);


            //Copy Images.
            if (Directory.Exists(inputPaths.ImageDirectory))
            {
                var siteImageDirectory = Path.Combine(nextSiteEpisodeDirectory, @"images\");
                if (!Directory.Exists(siteImageDirectory))
                {
                    Directory.CreateDirectory(siteImageDirectory);
                }

                foreach (var srcPath in Directory.GetFiles(inputPaths.ImageDirectory))
                {
                    var fileName = Path.GetFileName(srcPath);
                    var dstPath = Path.Combine(siteImageDirectory, fileName);
                    File.Copy(srcPath, dstPath);
                }
            }
        }

        static void WriteOfficeWord(Novel novel, Work work, Parameter param, InputPaths inputPaths)
        {
            string wordTemplatePath;
            if (work.TemplateFileName == Work.WORK_DEFAULT_VALUE)
            {
                wordTemplatePath = Path.Combine(param.TemplateDir, @"word.docx");
            }
            else
            {
                wordTemplatePath = Path.Combine(param.TemplateDir, work.TemplateFileName);
            }

            var wordDir = Path.Combine(param.OutputRoot, novel.Id, @"word");

            string wordFileName;
            if (work.OutputFileName == Work.WORK_DEFAULT_VALUE)
            {
                wordFileName = novel.MainTitle + ".docx";
            }
            else
            {
                wordFileName = work.OutputFileName;
            }

            new Writer.OfficeWord.Writer()
            {
                InputEpisodeDirectory = inputPaths.EpisodeDirectory,
                OutputWordDirectory = wordDir,
                OutputWordFileName = wordFileName,
                TemplatePath = wordTemplatePath,
            }.Write(novel);

        }

        static void WriteServiceNarou(Novel novel, Parameter param, InputPaths inputPaths)
        {
            var outputEpisodeDirectory = Path.Combine(param.OutputRoot, novel.Id, @"narou\");

            new Writer.ServiceNarou.Writer()
            {
                InputEpisodeDirectory = inputPaths.EpisodeDirectory,
                OutputEpisodeDirectory = outputEpisodeDirectory,
            }.Write(novel);
        }

        static void WriteServiceKakuyomu(Novel novel, Parameter param, InputPaths inputPaths)
        {
            var outputEpisodeDirectory = Path.Combine(param.OutputRoot, novel.Id, @"kakuyomu\");

            new Writer.ServiceKakuyomu.Writer()
            {
                InputEpisodeDirectory = inputPaths.EpisodeDirectory,
                OutputEpisodeDirectory = outputEpisodeDirectory,
            }.Write(novel);
        }

        static void WriteServiceAlphaPolis(Novel novel, Parameter param, InputPaths inputPaths)
        {
            var outputEpisodeDirectory = Path.Combine(param.OutputRoot, novel.Id, @"alphapolis\");

            new Writer.ServiceAlphapolis.Writer()
            {
                InputEpisodeDirectory = inputPaths.EpisodeDirectory,
                OutputEpisodeDirectory = outputEpisodeDirectory,
            }.Write(novel);
        }

    }
}
