namespace DiscordLab.Bot.API.Updates
{
    using System.Text.Json.Serialization;

    /// <summary>
    /// Contains data for a GitHub release object.
    /// </summary>
    public class GitHubRelease
    {
        /// <summary>
        /// Gets or sets the tag name for this release.
        /// </summary>
        [JsonPropertyName("tag_name")]
        public string TagName { get; set; }

        /// <summary>
        /// Gets or sets the assets for this release.
        /// </summary>
        [JsonPropertyName("assets")]
        public List<GitHubReleaseAsset> Assets { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this is a prerelease release.
        /// </summary>
        [JsonPropertyName("prerelease")]
        public bool Prerelease { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this is a draft release.
        /// </summary>
        [JsonPropertyName("draft")]
        public bool Draft { get; set; }
    }
}