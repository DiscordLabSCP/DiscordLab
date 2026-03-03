using System.Drawing;

namespace DiscordLab.Bot.API.Features.Embed;

using YamlDotNet.Serialization;

/// <summary>
/// Contains information about an author field in an embed.
/// </summary>
public class EmbedAuthorBuilder
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EmbedAuthorBuilder"/> class.
    /// </summary>
    public EmbedAuthorBuilder()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="EmbedAuthorBuilder"/> class.
    /// Replaces the base with the <see cref="Discord.EmbedAuthorBuilder"/> instance.
    /// </summary>
    /// <param name="builder">The <see cref="Discord.EmbedAuthorBuilder"/> instance.</param>
    public EmbedAuthorBuilder(Discord.EmbedAuthorBuilder builder)
    {
        Name = builder.Name;
        IconUrl = builder.IconUrl;
        Url = builder.Url;
    }

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

    /// <summary>
    /// Gets the base of this builder.
    /// </summary>
    [YamlIgnore]
    [Obsolete("Please use the properties of the DiscordLab.Bot.API.Features.Embed.EmbedAuthorBuilder instead.")]
    public Discord.EmbedAuthorBuilder Base => this;

    /// <summary>
    /// Changes a <see cref="EmbedAuthorBuilder"/> into a <see cref="Discord.EmbedAuthorBuilder"/> instance.
    /// </summary>
    /// <param name="builder">The <see cref="EmbedAuthorBuilder"/> instance.</param>
    /// <returns>A copy of the <see cref="Discord.EmbedAuthorBuilder"/> instance.</returns>
    public static implicit operator Discord.EmbedAuthorBuilder(EmbedAuthorBuilder builder)
    {
        Discord.EmbedAuthorBuilder copy = new();

        if (builder.Name != null)
            copy.WithName(builder.Name);

        if (builder.Url != null)
            copy.WithUrl(builder.Url);

        if (builder.IconUrl != null)
            copy.WithIconUrl(builder.IconUrl);

        return copy;
    }
}