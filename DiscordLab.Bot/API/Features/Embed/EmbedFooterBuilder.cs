namespace DiscordLab.Bot.API.Features.Embed;

using YamlDotNet.Serialization;

/// <summary>
/// Holds information for an Embed footer.
/// </summary>
public class EmbedFooterBuilder
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EmbedFooterBuilder"/> class.
    /// </summary>
    public EmbedFooterBuilder()
    {
        Base = new();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="EmbedFooterBuilder"/> class.
    /// Replaces the base with the <see cref="Discord.EmbedFooterBuilder"/> instance.
    /// </summary>
    /// <param name="builder">The <see cref="Discord.EmbedFooterBuilder"/> instance.</param>
    public EmbedFooterBuilder(Discord.EmbedFooterBuilder builder)
    {
        Base = builder;
    }

    /// <summary>
    /// Gets or sets the text for this footer.
    /// </summary>
    [YamlMember(DefaultValuesHandling = DefaultValuesHandling.OmitNull)]
    public string? Text
    {
        get => Base.Text;
        set => Base.Text = value;
    }

    /// <summary>
    /// Gets or sets the icon URl for this footer.
    /// </summary>
    [YamlMember(DefaultValuesHandling = DefaultValuesHandling.OmitNull)]
    public string? IconUrl
    {
        get => Base.IconUrl;
        set => Base.IconUrl = value;
    }

    /// <summary>
    /// Gets the base builder object.
    /// </summary>
    [YamlIgnore]
    public Discord.EmbedFooterBuilder Base { get; init; }
}