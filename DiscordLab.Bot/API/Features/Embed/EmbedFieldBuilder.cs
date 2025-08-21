namespace DiscordLab.Bot.API.Features.Embed;

using YamlDotNet.Serialization;

/// <summary>
/// Allows you to create embed fields for a <see cref="EmbedBuilder"/>.
/// </summary>
public class EmbedFieldBuilder
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EmbedFieldBuilder"/> class.
    /// </summary>
    public EmbedFieldBuilder()
    {
        Base = new();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="EmbedFieldBuilder"/> class.
    /// Replaces the base with the <see cref="Discord.EmbedFieldBuilder"/> instance.
    /// </summary>
    /// <param name="builder">The <see cref="Discord.EmbedFieldBuilder"/> instance.</param>
    public EmbedFieldBuilder(Discord.EmbedFieldBuilder builder)
    {
        Base = builder;
    }

    /// <summary>
    /// Gets or sets the field name.
    /// </summary>
    public string Name
    {
        get => Base.Name;
        set => Base.Name = value;
    }

    /// <summary>
    /// Gets or sets the field value.
    /// </summary>
    public string Value
    {
        get => Base.Value.ToString();
        set => Base.Value = value;
    }

    /// <summary>
    /// Gets or sets a value indicating whether the field is inline.
    /// </summary>
    [YamlMember(DefaultValuesHandling = DefaultValuesHandling.OmitDefaults)]
    public bool IsInline
    {
        get => Base.IsInline;
        set => Base.IsInline = value;
    }

    /// <summary>
    /// Gets the base builder.
    /// </summary>
    [YamlIgnore]
    public Discord.EmbedFieldBuilder Base { get; init; }
}