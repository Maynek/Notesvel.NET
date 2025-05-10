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

                parser.Parse(args);

                return parameter;
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

                //Read site.xml
                var shelf = ShelfReader.Read(inputSitePath);

                if (Directory.Exists(param.OutputRoot))
                {
                    Directory.Delete(param.OutputRoot, true);
                }

                foreach (var item in shelf.ItemList)
                {
                    var novelId = item.NovelId;

                    var inputDirectory = Path.Combine(param.InputRoot, novelId);
                    var inputEpisodeDirectory = Path.Combine(inputDirectory, @"episodes\");
                    var inputNoteDirectory = Path.Combine(inputDirectory, @"notes\");
                    var inputImageDirectory = Path.Combine(inputDirectory, @"images\");
                    var inputIndexPath = Path.Combine(inputDirectory, "index.xml");

                    //Read index.xml
                    var novel = NovelReader.Read(inputIndexPath);

                    //Setup Novel
                    novel.SetEpisodePagenation();

                    //Write for NextSite.
                    if (item.Target.Contains("NextSite"))
                    {
                        var siteEpisodeDirectory = Path.Combine(param.OutputRoot, @"site\", novelId);
                        var siteNoteDirectory = Path.Combine(siteEpisodeDirectory, @"note\");
                        new Writer.NextSite.Writer()
                        {
                            InputEpisodeDirectory = inputEpisodeDirectory,
                            InputNoteDirectory = inputNoteDirectory,
                            OutputEpisodeDirectory = siteEpisodeDirectory,
                            OutputNoteDirectory = siteNoteDirectory
                        }.Write(novel);


                        //Copy Images.
                        if (Directory.Exists(inputImageDirectory))
                        {
                            var siteImageDirectory = Path.Combine(siteEpisodeDirectory, @"images\");
                            if (!Directory.Exists(siteImageDirectory))
                            {
                                Directory.CreateDirectory(siteImageDirectory);
                            }

                            foreach (var srcPath in Directory.GetFiles(inputImageDirectory))
                            {
                                var fileName = Path.GetFileName(srcPath);
                                var dstPath = Path.Combine(siteImageDirectory, fileName);
                                File.Copy(srcPath, dstPath);
                            }
                        }
                    }

                    //Write for Narou.
                    if (item.Target.Contains("Narou"))
                    {
                        var narouEpisodeDirectory = Path.Combine(param.OutputRoot, @"narou\", novelId);
                        new Writer.Narou.Writer()
                        {
                            InputEpisodeDirectory = inputEpisodeDirectory,
                            OutputEpisodeDirectory = narouEpisodeDirectory,
                        }.Write(novel);
                    }

                    //Write for Kakuyomu.
                    if (item.Target.Contains("Kakuyomu"))
                    {
                        var narouEpisodeDirectory = Path.Combine(param.OutputRoot, @"kakuyomu\", novelId);
                        new Writer.Kakuyomu.Writer()
                        {
                            InputEpisodeDirectory = inputEpisodeDirectory,
                            OutputEpisodeDirectory = narouEpisodeDirectory,
                        }.Write(novel);
                    }

                    //Write for Alpha.
                    if (item.Target.Contains("Alpha"))
                    {
                        var narouEpisodeDirectory = Path.Combine(param.OutputRoot, @"alpha\", novelId);
                        new Writer.Alpha.Writer()
                        {
                            InputEpisodeDirectory = inputEpisodeDirectory,
                            OutputEpisodeDirectory = narouEpisodeDirectory,
                        }.Write(novel);
                    }

                }
            }
        }
    }
}
