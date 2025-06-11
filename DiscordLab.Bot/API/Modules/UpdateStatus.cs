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
        [JsonProperty("prerelease")]
        public bool Prerelease { get; set; }
        [JsonProperty("draft")]
        public bool Draft { get; set; }
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
        
        public static List<API.Features.UpdateStatus> Statuses { get; private set; }

        /// <summary>
        /// This will write a plugin to the Plugins folder.
        /// </summary>
        /// <param name="bytes">The response from a download via <see cref="HttpClient"/></param>
        /// <param name="name">The name of the plugin, without .dll</param>
        private static void WritePlugin(byte[] bytes, string name)
        {
            string[] files = Directory.GetFiles(Path);

            string existingPlugin = files.FirstOrDefault(x => x.Contains(name));
            
            if (existingPlugin != null)
            {
                File.Delete(existingPlugin);
            }
            
            string pluginPath = System.IO.Path.Combine(Path, name + ".dll");
            File.WriteAllBytes(pluginPath, bytes);
        }

        /// <summary>
        /// This will download a plugin using <see cref="HttpClient"/>
        /// </summary>
        /// <param name="status">The <see cref="API.Features.UpdateStatus"/></param>
        public static async Task DownloadPlugin(API.Features.UpdateStatus status)
        {
            byte[] pluginData = await Client.GetByteArrayAsync(status.Url);
            WritePlugin(pluginData, status.ModuleName);
        }
        
        /// <summary>
        /// This will check the GitHub API for the latest version of the modules and plugins.
        /// </summary>
        /// <remarks>
        /// Should only be run once via the main bot, otherwise you'll just be doing unnecessary requests.
        /// </remarks>
        public static async Task GetStatus()
        {
            Statuses = new();
            Client.DefaultRequestHeaders.UserAgent.ParseAdd("request");
            string response = await Client.GetStringAsync("https://api.github.com/repos/DiscordLabSCP/DiscordLab/releases");
            List<API.Features.UpdateStatus> statuses = new();
            List<GitHubRelease> releases = JsonConvert.DeserializeObject<List<GitHubRelease>>(response);
            foreach (GitHubRelease release in releases)
            {
                if(release.Prerelease || release.Draft) continue;
                foreach (GitHubReleaseAsset asset in release.Assets)
                {
                    Features.UpdateStatus status = new()
                    {
                        ModuleName = asset.Name.Replace(".dll", ""),
                        Version = new (string.Join(".", release.TagName.Split('.').Take(3))),
                        Url = asset.DownloadUrl
                    };
                    
                    // do not want to auto update to breaking changes
                    if(status.Version.Major != Plugin.Instance.Version.Major) continue;
                    
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
            
            Statuses = statuses;

            List<IPlugin<IConfig>> plugins = Loader.Plugins.Where(x => x.Name.StartsWith("DiscordLab.")).ToList();
            plugins.Add(Loader.Plugins.First(x => x.Name == Plugin.Instance.Name));
            List<string> pluginsToUpdate = new();
            foreach (IPlugin<IConfig> plugin in plugins)
            {
                API.Features.UpdateStatus status = statuses.FirstOrDefault(x => x.ModuleName == plugin.Name);
                if (status == null)
                {
                    if(plugin.Name == Plugin.Instance.Name) status = statuses.First(x => x.ModuleName == "DiscordLab.Bot");
                    else continue;
                }

                if (status.Version <= plugin.Version) continue;
                if (Plugin.Instance.Config.AutoUpdate)
                {
                    pluginsToUpdate.Add(status.ModuleName);
                    await DownloadPlugin(status);
                }
                else
                {
                    Log.Warn($"There is a new version of {status.ModuleName} available, version {status.Version}, you are currently on {plugin.Version}! Download it from {status.Url}");
                }
            }

            if (pluginsToUpdate.Any())
            {
                ServerStatic.StopNextRound = ServerStatic.NextRoundAction.Restart;
                Log.Info("Server will restart next round for updates. Updating plugins: " + string.Join(", ", pluginsToUpdate));
            }
        }
    }
}