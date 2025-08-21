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
        get => Builder.Title;
        set => Builder.Title = value;
    }

    /// <summary>
    /// Gets or sets the embed description.
    /// </summary>
    public string Description
    {
        get => Builder.Description;
        set => Builder.Description = value;
    }

    /// <summary>
    /// Gets or sets the embed fields.
    /// </summary>
    [YamlMember(DefaultValuesHandling = DefaultValuesHandling.OmitDefaults)]
    public IEnumerable<EmbedFieldBuilder> Fields
    {
        get => Builder.Fields.Select(x => new EmbedFieldBuilder
            { Name = x.Name, Value = x.Value.ToString(), IsInline = x.IsInline });
        set => Builder.Fields = value.Select(x => x.Builder).ToList();
    }

    /// <summary>
    /// Gets or sets the color of the embed. In string so #, 0x or the raw hex value will work.
    /// </summary>
    [YamlMember(DefaultValuesHandling = DefaultValuesHandling.OmitNull)]
    public string? Color
    {
        get => Builder.Color?.ToString();
        set
        {
            if (value == null)
            {
                Builder.Color = null;
                return;
            }

            Builder.Color = Discord.Color.Parse(value);
        }
    }

    [YamlIgnore]
    private Discord.EmbedBuilder Builder { get; } = new();

    /// <summary>
    /// Changes a <see cref="EmbedBuilder"/> into a <see cref="Discord.EmbedBuilder"/> instance.
    /// </summary>
    /// <param name="builder">The <see cref="EmbedBuilder"/> instance.</param>
    /// <returns>A copy of the <see cref="Discord.EmbedBuilder"/> instance.</returns>
    public static implicit operator Discord.EmbedBuilder(EmbedBuilder builder)
    {
        Discord.EmbedBuilder copy = new();

        if (builder.Builder.Title != null)
            copy.WithTitle(builder.Builder.Title);

        if (builder.Builder.Description != null)
            copy.WithDescription(builder.Builder.Description);

        if (builder.Builder.Color.HasValue)
            copy.WithColor(builder.Builder.Color.Value);

        if (builder.Builder.Url != null)
            copy.WithUrl(builder.Builder.Url);

        if (builder.Builder.ImageUrl != null)
            copy.WithImageUrl(builder.Builder.ImageUrl);

        if (builder.Builder.ThumbnailUrl != null)
            copy.WithThumbnailUrl(builder.Builder.ThumbnailUrl);

        if (builder.Builder.Timestamp.HasValue)
            copy.WithTimestamp(builder.Builder.Timestamp.Value);

        if (builder.Builder.Footer != null)
            copy.WithFooter(builder.Builder.Footer);

        if (builder.Builder.Author != null)
            copy.WithAuthor(builder.Builder.Author);

        foreach (Discord.EmbedFieldBuilder field in builder.Builder.Fields)
        {
            copy.AddField(field.Name, field.Value, field.IsInline);
        }

        return copy;
    }
}