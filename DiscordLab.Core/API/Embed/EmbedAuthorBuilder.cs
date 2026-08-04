using YamlDotNet.Serialization;

namespace DiscordLab.Core.API.Embed;

/// <summary>
/// Contains information about an author field in an embed.
/// </summary>
public class EmbedAuthorBuilder
{
    /// <summary>
    /// Gets or sets the author name.
    /// </summary>
    [YamlMember(DefaultValuesHandling = DefaultValuesHandling.OmitNull)]
    public string? Name { get; set; }

    /// <summary>
    /// Gets or sets the icon URL.
    /// </summary>
    [YamlMember(DefaultValuesHandling = DefaultValuesHandling.OmitNull)]
    public string? IconUrl { get; set; }

    /// <summary>
    /// Gets or sets the URL.
    /// </summary>
    [YamlMember(DefaultValuesHandling = DefaultValuesHandling.OmitNull)]
    public string? Url { get; set; }

    public EmbedAuthorBuilder Clone() => (EmbedAuthorBuilder)MemberwiseClone();
}