namespace DiscordLab.Bot.API.Features.Embed;

using YamlDotNet.Serialization;

/// <summary>
/// Allows you to create embed fields for a <see cref="EmbedBuilder"/>.
/// </summary>
public class EmbedFieldBuilder
{
    /// <summary>
    /// Gets or sets the field name.
    /// </summary>
    public string Name
    {
        get => Builder.Name;
        set => Builder.Name = value;
    }

    /// <summary>
    /// Gets or sets the field value.
    /// </summary>
    public string Value
    {
        get => Builder.Value.ToString();
        set => Builder.Value = value;
    }

    /// <summary>
    /// Gets or sets a value indicating whether the field is inline.
    /// </summary>
    public bool IsInline
    {
        get => Builder.IsInline;
        set => Builder.IsInline = value;
    }

    /// <summary>
    /// Gets the base builder.
    /// </summary>
    [YamlIgnore]
    internal Discord.EmbedFieldBuilder Builder { get; } = new();
}