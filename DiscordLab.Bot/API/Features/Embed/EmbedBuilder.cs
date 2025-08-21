namespace DiscordLab.Bot.API.Features.Embed;

using YamlDotNet.Serialization;

/// <summary>
/// Allows you to make an embed. Should be used in translations only.
/// </summary>
public class EmbedBuilder
{
    /// <summary>
    /// Gets or sets the embed title.
    /// </summary>
    public string Title
    {
        get => Base.Title;
        set => Base.Title = value;
    }

    /// <summary>
    /// Gets or sets the embed description.
    /// </summary>
    public string Description
    {
        get => Base.Description;
        set => Base.Description = value;
    }

    /// <summary>
    /// Gets or sets the embed fields.
    /// </summary>
    public IEnumerable<EmbedFieldBuilder> Fields
    {
        get => Base.Fields.Select(x => new EmbedFieldBuilder(x));
        set => Base.Fields = value.Select(x => x.Base).ToList();
    }

    /// <summary>
    /// Gets or sets the color of the embed. In string so #, 0x or the raw hex value will work.
    /// </summary>
    [YamlMember(DefaultValuesHandling = DefaultValuesHandling.OmitNull)]
    public string? Color
    {
        get => Base.Color?.ToString();
        set
        {
            if (value == null)
            {
                Base.Color = null;
                return;
            }

            Base.Color = Discord.Color.Parse(value);
        }
    }

    /// <summary>
    /// Gets or sets the thumbnail URL of the embed.
    /// </summary>
    [YamlMember(DefaultValuesHandling = DefaultValuesHandling.OmitNull)]
    public string? ThumbnailUrl
    {
        get => Base.ThumbnailUrl;
        set => Base.ThumbnailUrl = value;
    }

    /// <summary>
    /// Gets or sets the image URL of the embed.
    /// </summary>
    [YamlMember(DefaultValuesHandling = DefaultValuesHandling.OmitNull)]
    public string? ImageUrl
    {
        get => Base.ImageUrl;
        set => Base.ImageUrl = value;
    }

    /// <summary>
    /// Gets or sets the URL of the embed.
    /// </summary>
    [YamlMember(DefaultValuesHandling = DefaultValuesHandling.OmitNull)]
    public string? Url
    {
        get => Base.Url;
        set => Base.Url = value;
    }

    /// <summary>
    /// Gets or sets the footer of the embed.
    /// </summary>
    [YamlMember(DefaultValuesHandling = DefaultValuesHandling.OmitNull)]
    public EmbedFooterBuilder? Footer
    {
        get => Base.Footer != null ? new(Base.Footer) : null;
        set => Base.Footer = value?.Base;
    }

    /// <summary>
    /// Gets or sets the author of the embed.
    /// </summary>
    [YamlMember(DefaultValuesHandling = DefaultValuesHandling.OmitNull)]
    public EmbedAuthorBuilder? Author
    {
        get => Base.Author != null ? new(Base.Author) : null;
        set => Base.Author = value?.Base;
    }

    [YamlIgnore]
    private Discord.EmbedBuilder Base { get; } = new();

    /// <summary>
    /// Changes a <see cref="EmbedBuilder"/> into a <see cref="Discord.EmbedBuilder"/> instance.
    /// </summary>
    /// <param name="builder">The <see cref="EmbedBuilder"/> instance.</param>
    /// <returns>A copy of the <see cref="Discord.EmbedBuilder"/> instance.</returns>
    public static implicit operator Discord.EmbedBuilder(EmbedBuilder builder)
    {
        Discord.EmbedBuilder copy = new();

        if (builder.Base.Title != null)
            copy.WithTitle(builder.Base.Title);

        if (builder.Base.Description != null)
            copy.WithDescription(builder.Base.Description);

        if (builder.Base.Color.HasValue)
            copy.WithColor(builder.Base.Color.Value);

        if (builder.Base.Url != null)
            copy.WithUrl(builder.Base.Url);

        if (builder.Base.ImageUrl != null)
            copy.WithImageUrl(builder.Base.ImageUrl);

        if (builder.Base.ThumbnailUrl != null)
            copy.WithThumbnailUrl(builder.Base.ThumbnailUrl);

        if (builder.Base.Timestamp.HasValue)
            copy.WithTimestamp(builder.Base.Timestamp.Value);

        if (builder.Base.Footer != null)
            copy.WithFooter(builder.Base.Footer);

        if (builder.Base.Author != null)
            copy.WithAuthor(builder.Base.Author);

        foreach (Discord.EmbedFieldBuilder field in builder.Base.Fields)
        {
            copy.AddField(field.Name, field.Value, field.IsInline);
        }

        return copy;
    }
}