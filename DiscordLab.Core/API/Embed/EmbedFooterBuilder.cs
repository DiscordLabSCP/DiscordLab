using YamlDotNet.Serialization;

namespace DiscordLab.Core.API.Embed;

/// <summary>
/// Holds information for an Embed footer.
/// </summary>
public class EmbedFooterBuilder
{
    /// <summary>
    /// Gets or sets the text for this footer.
    /// </summary>
    [YamlMember(DefaultValuesHandling = DefaultValuesHandling.OmitNull)]
    public string? Text { get; set; }

    /// <summary>
    /// Gets or sets the icon URl for this footer.
    /// </summary>
    [YamlMember(DefaultValuesHandling = DefaultValuesHandling.OmitNull)]
    public string? IconUrl { get; set; }

    public EmbedFooterBuilder Clone() => (EmbedFooterBuilder)MemberwiseClone();
}