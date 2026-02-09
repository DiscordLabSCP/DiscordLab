namespace DiscordLab.Bot;

using System.ComponentModel;
using YamlDotNet.Serialization;

/// <summary>
/// The config of this plugin.
/// </summary>
public sealed class Config
{
    /// <summary>
    /// Gets or sets the token for the bot.
    /// </summary>
    [Description("The token of the bot.")]
    public string Token { get; set; } = "token";

    /// <summary>
    /// Gets or sets the default guild ID.
    /// </summary>
    [Description("The default guild ID. Each module that has their guild ID set to 0 has their guild ID set to this.")]
    public ulong GuildId { get; set; } = 0;

    /// <summary>
    /// Gets or sets a value indicating whether the plugin should check for DiscordLab updates.
    /// </summary>
    [Description("Whether the plugin should check for DiscordLab updates.")]
    public bool AutoUpdate { get; set; } = true;

    /// <summary>
    /// Gets or sets the proxy URL. Shouldn't be set if proxy is not needed.
    /// </summary>
    [Description(
        "The proxy URL to use. Should only be used in very specific cases like Discord being banned in your country. Please set to empty to not use.")]
    public string ProxyUrl { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a value indicating whether debug logging should be enabled.
    /// </summary>
    [Description("Enable debugging mode, useful to enable when needing to debug for developers.")]
    public bool Debug { get; set; } = false;

    /// <summary>
    /// Gets or sets the number of messages that should be cached from each channel.
    /// </summary>
    [YamlMember(DefaultValuesHandling = DefaultValuesHandling.OmitDefaults)]
    public int MessageCacheSize { get; set; }
}