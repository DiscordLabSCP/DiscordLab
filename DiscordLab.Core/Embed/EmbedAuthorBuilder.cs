namespace DiscordLab.Bot.API.Features.Embed;

using YamlDotNet.Serialization;

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

    public EmbedAuthorBuilder Clone()
    {
        EmbedAuthorBuilder copy = new();

        copy.Name = Name;
        copy.Url = Url;
        copy.IconUrl = IconUrl;
        
        return copy;
    }
}