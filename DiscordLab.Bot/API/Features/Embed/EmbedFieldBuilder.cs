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
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="EmbedFieldBuilder"/> class.
    /// Replaces the base with the <see cref="Discord.EmbedFieldBuilder"/> instance.
    /// </summary>
    /// <param name="builder">The <see cref="Discord.EmbedFieldBuilder"/> instance.</param>
    public EmbedFieldBuilder(Discord.EmbedFieldBuilder builder)
    {
        Name = builder.Name;
        IsInline = builder.IsInline;

        if (builder.Value is string value)
            Value = value;
    }

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

    /// <summary>
    /// Gets the base builder.
    /// </summary>
    [YamlIgnore]
    [Obsolete("Please use the properties of the DiscordLab.Bot.API.Features.Embed.EmbedFieldBuilder instead.")]
    public Discord.EmbedFieldBuilder Base => this;

    /// <summary>
    /// Changes a <see cref="EmbedFieldBuilder"/> into a <see cref="Discord.EmbedFieldBuilder"/> instance.
    /// </summary>
    /// <param name="builder">The <see cref="EmbedFieldBuilder"/> instance.</param>
    /// <returns>A copy of the <see cref="Discord.EmbedFieldBuilder"/> instance.</returns>
    public static implicit operator Discord.EmbedFieldBuilder(EmbedFieldBuilder builder)
    {
        Discord.EmbedFieldBuilder copy = new();

        copy.WithName(builder.Name);

        if (builder.Value != null)
            copy.WithValue(builder.Value);

        copy.WithIsInline(builder.IsInline);

        return copy;
    }
}