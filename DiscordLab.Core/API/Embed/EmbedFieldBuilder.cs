using YamlDotNet.Serialization;

namespace DiscordLab.Core.API.Embed;

/// <summary>
/// Allows you to create embed fields for a <see cref="EmbedBuilder"/>.
/// </summary>
public class EmbedFieldBuilder
{
    /// <summary>
    /// Gets or sets the field name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the field value.
    /// </summary>
    public string? Value { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the field is inline.
    /// </summary>
    [YamlMember(DefaultValuesHandling = DefaultValuesHandling.OmitDefaults)]
    public bool IsInline { get; set; }

    public EmbedFieldBuilder Clone() => (EmbedFieldBuilder)MemberwiseClone();
}