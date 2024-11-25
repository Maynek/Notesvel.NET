//********************************
// (c) 2024 Ada Maynek
// This software is released under the MIT License.
//********************************
using System.Text.Encodings.Web;
using System.Text.Json;

namespace Maynek.Notesvel.Other
{
    public class MySiteUtil
    {
        private static readonly JsonSerializerOptions SerializerOptions;

        static MySiteUtil()
        {
            SerializerOptions = new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                WriteIndented = true,
            };
        }

        private static string GetJsonSerializedText(object obj)
        {
            return JsonSerializer.Serialize(obj, MySiteUtil.SerializerOptions).Replace("\r\n", "\n");
        }

        public static void OutputNovels(string path, MySite site)
        {
            var jsonText = MySiteUtil.GetJsonSerializedText(site.NovelIdList);
            File.WriteAllText(path, jsonText);
        }

        public static void OutputEpisodes(string path, MySite site)
        {
            var jsonText = MySiteUtil.GetJsonSerializedText(site.EpisodePageList);
            File.WriteAllText(path, jsonText);
        }
        public static void OutputGlossaries(string path, MySite site)
        {
            var jsonText = MySiteUtil.GetJsonSerializedText(site.GlossaryPageList);
            File.WriteAllText(path, jsonText);
        }

        public static void OutputNotes(string path, MySite site)
        {
            var jsonText = MySiteUtil.GetJsonSerializedText(site.NotePageList);
            File.WriteAllText(path, jsonText);
        }

        public static void Output(string dir, MySite site)
        {
            string novelsPath = Path.Combine(dir, @"indexes.json");
            MySiteUtil.OutputNovels(novelsPath, site);

            string episodesPath = Path.Combine(dir, @"episodes.json");
            MySiteUtil.OutputEpisodes(episodesPath, site);

            string glossariesPath = Path.Combine(dir, @"glossaries.json");
            MySiteUtil.OutputGlossaries(glossariesPath, site);

            string notesPath = Path.Combine(dir, @"notes.json");
            MySiteUtil.OutputNotes(notesPath, site);
        }

    }
}
