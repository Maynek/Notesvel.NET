//********************************
// (c) 2024 Ada Maynek
// This software is released under the MIT License.
//********************************
using Maynek.Notesvel.Writer.MySite;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.RegularExpressions;

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

        public static void Output(string dir, MySite site)
        {
            var el = MySiteEpisodePath.Create(site);
            var jsonText = MySiteUtil.GetJsonSerializedText(el);

            string outputPath = Path.Combine(dir, @"paths.json");
            File.WriteAllText(outputPath, jsonText);
        }


    }
}
