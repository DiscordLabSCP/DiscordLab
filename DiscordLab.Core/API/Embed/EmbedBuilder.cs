using YamlDotNet.Serialization;

namespace DiscordLab.Core.API.Embed;

/// <summary>
/// Allows you to make an embed. Should be used in translations only.
/// </summary>
public class EmbedBuilder
{
    public const int MaxDescriptionLength = 4096;
    
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

    public EmbedBuilder Clone()
    {
        if (
            string.IsNullOrEmpty(Title)
            && string.IsNullOrEmpty(Description)
            && string.IsNullOrEmpty(ThumbnailUrl)
            && string.IsNullOrEmpty(ImageUrl)
            && (Author == null || string.IsNullOrEmpty(Author.Name))
            && (Fields == null || !Fields.Any()))
        {
            throw new ArgumentNullException(nameof(Description), "An embed must contain at least on of the following: title, description, thumbnail, image, author (with a name) or at least 1 field.");
        }

        EmbedBuilder copy = (EmbedBuilder)MemberwiseClone();

        if(copy.Fields != null)
            copy.Fields = copy.Fields?.Select(field => field.Clone());
        if (copy.Author != null)
            copy.Author = copy.Author.Clone();
        if (copy.Footer != null)
            copy.Footer = copy.Footer.Clone();

        return copy;
    }
}