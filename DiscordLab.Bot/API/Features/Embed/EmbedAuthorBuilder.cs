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
        Base = new();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="EmbedAuthorBuilder"/> class.
    /// Replaces the base with the <see cref="Discord.EmbedAuthorBuilder"/> instance.
    /// </summary>
    /// <param name="builder">The <see cref="Discord.EmbedAuthorBuilder"/> instance.</param>
    public EmbedAuthorBuilder(Discord.EmbedAuthorBuilder builder)
    {
        Base = builder;
    }

    /// <summary>
    /// Gets or sets the author name.
    /// </summary>
    [YamlMember(DefaultValuesHandling = DefaultValuesHandling.OmitNull)]
    public string? Name
    {
        get => Base.Name;
        set => Base.Name = value;
    }

    /// <summary>
    /// Gets or sets the icon URL.
    /// </summary>
    [YamlMember(DefaultValuesHandling = DefaultValuesHandling.OmitNull)]
    public string? IconUrl
    {
        get => Base.IconUrl;
        set => Base.IconUrl = value;
    }

    /// <summary>
    /// Gets or sets the URL.
    /// </summary>
    [YamlMember(DefaultValuesHandling = DefaultValuesHandling.OmitNull)]
    public string? Url
    {
        get => Base.Url;
        set => Base.Url = value;
    }

    /// <summary>
    /// Gets the base of this builder.
    /// </summary>
    [YamlIgnore]
    public Discord.EmbedAuthorBuilder Base { get; init; }
}