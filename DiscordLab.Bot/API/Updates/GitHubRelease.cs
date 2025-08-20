#nullable disable

namespace DiscordLab.Bot.API.Updates;

using Newtonsoft.Json;

/// <summary>
/// Contains data for a GitHub release object.
/// </summary>
public class GitHubRelease
{
    /// <summary>
    /// Gets or sets the tag name for this release.
    /// </summary>
    [JsonProperty("tag_name")]
    public string TagName { get; set; }

    // ReSharper disable CollectionNeverUpdated.Global

    /// <summary>
    /// Gets or sets the assets for this release.
    /// </summary>
    [JsonProperty("assets")]
    public List<GitHubReleaseAsset> Assets { get; set; }

    // ReSharper restore CollectionNeverUpdated.Global

    /// <summary>
    /// Gets or sets a value indicating whether this is a prerelease release.
    /// </summary>
    [JsonProperty("prerelease")]
    public bool Prerelease { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this is a draft release.
    /// </summary>
    [JsonProperty("draft")]
    public bool Draft { get; set; }
}