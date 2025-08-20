#nullable disable

namespace DiscordLab.Bot.API.Updates;

using Newtonsoft.Json;

/// <summary>
/// Gets details for a GitHub release asset.
/// </summary>
public class GitHubReleaseAsset
{
    /// <summary>
    /// Gets or sets the name of the asset.
    /// </summary>
    [JsonProperty("name")]
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the download name of the asset.
    /// </summary>
    [JsonProperty("browser_download_url")]
    public string DownloadUrl { get; set; }
}