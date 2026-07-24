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

    [Description("")]
    public Dictionary<string, string> ChannelIds { get; set; } = new()
    {
        ["default"] = "0"
    };

    /// <summary>
    /// Gets or sets the number of messages that should be cached from each channel.
    /// </summary>
    [YamlMember(DefaultValuesHandling = DefaultValuesHandling.OmitDefaults)]
    public int MessageCacheSize { get; set; }

    [YamlMember(DefaultValuesHandling = DefaultValuesHandling.OmitDefaults)]
    public bool Debug { get; set; } = false;
}