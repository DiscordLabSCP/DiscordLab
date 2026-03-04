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
    [YamlMember(DefaultValuesHandling = DefaultValuesHandling.OmitNull)]
    public string? Title { get; set; }

    /// <summary>
    /// Gets or sets the embed description.
    /// </summary>
    [YamlMember(DefaultValuesHandling = DefaultValuesHandling.OmitNull)]
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the embed fields.
    /// </summary>
    [YamlMember(DefaultValuesHandling = DefaultValuesHandling.OmitNull)]
    public IEnumerable<EmbedFieldBuilder>? Fields { get; set; }

    /// <summary>
    /// Gets or sets the color of the embed. In string so #, 0x or the raw hex value will work.
    /// </summary>
    [YamlMember(DefaultValuesHandling = DefaultValuesHandling.OmitNull)]
    public string? Color { get; set; }

    /// <summary>
    /// Gets or sets the thumbnail URL of the embed.
    /// </summary>
    [YamlMember(DefaultValuesHandling = DefaultValuesHandling.OmitNull)]
    public string? ThumbnailUrl { get; set; }

    /// <summary>
    /// Gets or sets the image URL of the embed.
    /// </summary>
    [YamlMember(DefaultValuesHandling = DefaultValuesHandling.OmitNull)]
    public string? ImageUrl { get; set; }

    /// <summary>
    /// Gets or sets the URL of the embed.
    /// </summary>
    [YamlMember(DefaultValuesHandling = DefaultValuesHandling.OmitNull)]
    public string? Url { get; set; }

    /// <summary>
    /// Gets or sets the footer of the embed.
    /// </summary>
    [YamlMember(DefaultValuesHandling = DefaultValuesHandling.OmitNull)]
    public EmbedFooterBuilder? Footer { get; set; }

    /// <summary>
    /// Gets or sets the author of the embed.
    /// </summary>
    [YamlMember(DefaultValuesHandling = DefaultValuesHandling.OmitNull)]
    public EmbedAuthorBuilder? Author { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether a timestamp will be added to the footer of this embed.
    /// </summary>
    [YamlMember(DefaultValuesHandling = DefaultValuesHandling.OmitDefaults)]
    public bool Timestamp { get; set; }

    /// <summary>
    /// Changes a <see cref="EmbedBuilder"/> into a <see cref="Discord.EmbedBuilder"/> instance.
    /// </summary>
    /// <param name="builder">The <see cref="EmbedBuilder"/> instance.</param>
    /// <returns>A copy of the <see cref="Discord.EmbedBuilder"/> instance.</returns>
    public static implicit operator Discord.EmbedBuilder(EmbedBuilder builder)
    {
        if (
            string.IsNullOrEmpty(builder.Title)
            && string.IsNullOrEmpty(builder.Description)
            && string.IsNullOrEmpty(builder.ThumbnailUrl)
            && string.IsNullOrEmpty(builder.ImageUrl)
            && (builder.Author == null || string.IsNullOrEmpty(builder.Author.Name))
            && (builder.Fields == null || !builder.Fields.Any()))
        {
            throw new ArgumentNullException("An embed must contain at least on of the following: title, description, thumbnail, image, author (with a name) or at least 1 field.");
        }

        Discord.EmbedBuilder copy = new();

        if (builder.Title != null)
            copy.WithTitle(builder.Title);

        if (builder.Description != null)
            copy.WithDescription(builder.Description);

        if (builder.Color != null)
            copy.WithColor(Discord.Color.Parse(builder.Color));

        if (builder.Url != null)
            copy.WithUrl(builder.Url);

        if (builder.ImageUrl != null)
            copy.WithImageUrl(builder.ImageUrl);

        if (builder.ThumbnailUrl != null)
            copy.WithThumbnailUrl(builder.ThumbnailUrl);

        if (builder.Timestamp)
            copy.WithCurrentTimestamp();

        if (builder.Footer != null)
            copy.WithFooter(builder.Footer);

        if (builder.Author != null)
            copy.WithAuthor(builder.Author);

        if (builder.Fields != null)
        {
            foreach (var field in builder.Fields)
            {
                copy.AddField(field);
            }
        }

        return copy;
    }
}