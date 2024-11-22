using Exiled.API.Features;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DiscordLab.Bot.API.Modules
{
    public static class WriteableConfig
    {
        private static string Path =
            System.IO.Path.Combine(System.IO.Path.Combine(Paths.Configs, "DiscordLab"), "config.json");

        public static JObject GetConfig()
        {
            if (!File.Exists(Path))
            {
                Directory.CreateDirectory(System.IO.Path.GetDirectoryName(Path)!);
                File.WriteAllText(Path, "{}");
            }

            return JObject.Parse(File.ReadAllText(Path));
        }

        public static void WriteConfigOption(string key, JToken value)
        {
            JObject config = GetConfig();
            config[key] = value;
            File.WriteAllText(Path, JsonConvert.SerializeObject(config));
        }
    }
}