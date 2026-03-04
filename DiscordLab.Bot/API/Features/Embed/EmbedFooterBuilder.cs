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
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="EmbedFooterBuilder"/> class.
    /// Replaces the base with the <see cref="Discord.EmbedFooterBuilder"/> instance.
    /// </summary>
    /// <param name="builder">The <see cref="Discord.EmbedFooterBuilder"/> instance.</param>
    public EmbedFooterBuilder(Discord.EmbedFooterBuilder builder)
    {
        Text = builder.Text;
        IconUrl = builder.IconUrl;
    }

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

    /// <summary>
    /// Gets the base builder object.
    /// </summary>
    [YamlIgnore]
    [Obsolete("Please use the properties of the DiscordLab.Bot.API.Features.Embed.EmbedFooterBuilder instead.")]
    public Discord.EmbedFooterBuilder Base => this;

    /// <summary>
    /// Changes a <see cref="EmbedFooterBuilder"/> into a <see cref="Discord.EmbedFooterBuilder"/> instance.
    /// </summary>
    /// <param name="builder">The <see cref="EmbedFooterBuilder"/> instance.</param>
    /// <returns>A copy of the <see cref="Discord.EmbedFooterBuilder"/> instance.</returns>
    public static implicit operator Discord.EmbedFooterBuilder(EmbedFooterBuilder builder)
    {
        Discord.EmbedFooterBuilder copy = new();

        if (builder.Text != null)
            copy.WithText(builder.Text);

        if (builder.IconUrl != null)
            copy.WithIconUrl(builder.IconUrl);

        return copy;
    }
}