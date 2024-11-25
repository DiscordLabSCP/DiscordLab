using Exiled.API.Features;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DiscordLab.Bot.API.Modules
{
    public static class WriteableConfig
    {
        private static string Path =
            System.IO.Path.Combine(System.IO.Path.Combine(Paths.Configs, "DiscordLab"), "config.json");
        
        /// <summary>
        /// This will get the config.json file from the DiscordLab folder in the Exiled configs.
        /// </summary>
        /// <returns>
        /// The config.json file as a <see cref="JObject"/>.
        /// </returns>
        public static JObject GetConfig()
        {
            if (!File.Exists(Path))
            {
                Directory.CreateDirectory(System.IO.Path.GetDirectoryName(Path)!);
                File.WriteAllText(Path, "{}");
            }

            return JObject.Parse(File.ReadAllText(Path));
        }
        
        /// <summary>
        /// This will write a new option to the config.json file.
        /// It can also overwrite options.
        /// </summary>
        /// <param name="key">
        /// The key of the option you want to write.
        /// </param>
        /// <param name="value">
        /// The value of the option you want to write. Should be JSON serializable.
        /// </param>
        public static void WriteConfigOption(string key, JToken value)
        {
            JObject config = GetConfig();
            config[key] = value;
            File.WriteAllText(Path, JsonConvert.SerializeObject(config));
        }
    }
}