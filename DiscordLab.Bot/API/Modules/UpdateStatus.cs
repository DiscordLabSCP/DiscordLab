using Newtonsoft.Json;
using System.Net.Http;
using Exiled.API.Features;
using Exiled.API.Interfaces;
using Exiled.Loader;

namespace DiscordLab.Bot.API.Modules
{
    public class GitHubRelease
    {
        [JsonProperty("tag_name")]
        public string TagName { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("html_url")]
        public string Url { get; set; }
        [JsonProperty("published_at")]
        public DateTime PublishedAt { get; set; }
        [JsonProperty("assets")]
        public List<GitHubReleaseAsset> Assets { get; set; }
    }

    public class GitHubReleaseAsset
    {
        [JsonProperty("url")]
        public string Url { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("browser_download_url")]
        public string DownloadUrl { get; set; }
    }
    
    public static class UpdateStatus
    {
        private static readonly HttpClient Client = new ();

        private static readonly string Path = Paths.Plugins;

        /// <summary>
        /// This will write a plugin to the Plugins folder.
        /// </summary>
        /// <param name="bytes">The response from a download via <see cref="HttpClient"/></param>
        /// <param name="name">The name of the plugin, without .dll</param>
        private static void WritePlugin(byte[] bytes, string name)
        {
            string pluginPath = Path + name + ".dll";
            File.WriteAllBytes(pluginPath, bytes);
        }
        
        /// <summary>
        /// This will check the GitHub API for the latest version of the modules and plugins.
        /// </summary>
        /// <remarks>
        /// Should only be run once via the main bot, otherwise you'll just be doing unnecessary requests.
        /// </remarks>
        public static async Task GetStatus()
        {
            Client.DefaultRequestHeaders.UserAgent.ParseAdd("request");
            string response = await Client.GetStringAsync("https://api.github.com/repos/JayXTQ/DiscordLab/releases");
            List<API.Features.UpdateStatus> statuses = new();
            List<GitHubRelease> releases = JsonConvert.DeserializeObject<List<GitHubRelease>>(response);
            foreach (GitHubRelease release in releases)
            {
                foreach (GitHubReleaseAsset asset in release.Assets)
                {
                    Features.UpdateStatus status = new()
                    {
                        ModuleName = asset.Name.Replace(".dll", ""),
                        Version = new (string.Join(".", release.TagName.Split('.').Take(3))),
                        Url = asset.DownloadUrl
                    };
                    List<API.Features.UpdateStatus> moduleStatuses = statuses.Where(s => s.ModuleName == status.ModuleName).ToList();
                    if (moduleStatuses.Any(s => s.Version < status.Version))
                    {
                        statuses.RemoveAll(s => s.ModuleName == status.ModuleName);
                        statuses.Add(status);
                    }
                    else if (!moduleStatuses.Any())
                    {
                        statuses.Add(status);
                    }
                }
            }

            List<IPlugin<IConfig>> plugins = Loader.Plugins.Where(x => x.Name.StartsWith("DiscordLab.")).ToList();
            plugins.Add(Loader.Plugins.First(x => x.Name == Plugin.Instance.Name));
            bool restartServer = false;
            foreach (IPlugin<IConfig> plugin in plugins)
            {
                API.Features.UpdateStatus status = statuses.FirstOrDefault(x => x.ModuleName == plugin.Name);
                if (status == null)
                {
                    if(plugin.Name == Plugin.Instance.Name) status = statuses.First(x => x.ModuleName == "DiscordLab.Bot");
                    else continue;
                }
                if (status.Version > plugin.Version)
                {
                    if (Plugin.Instance.Config.AutoUpdate)
                    {
                        restartServer = true;
                        byte[] pluginData = await Client.GetByteArrayAsync(status.Url);
                        WritePlugin(pluginData, status.ModuleName);
                    }
                    else
                    {
                        Log.Warn($"There is a new version of {status.ModuleName} available, version {status.Version}, you are currently on {plugin.Version}! Download it from {status.Url}");
                    }
                }
            }

            if (restartServer)
            {
                Server.ExecuteCommand("rnr");
            }
        }
    }
}