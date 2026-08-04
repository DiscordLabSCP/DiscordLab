using DiscordLab.Core.API.Embed;
using DiscordLab.Core.API.TranslationBuilders;

namespace DiscordLab.Core;

/// <summary>
/// Represents a specific field that the <see cref="TranslationBuilder.Build"/> can be executed on.
/// </summary>
[Flags]
public enum TranslatableMessageField
{
    /// <summary>
    /// Does not mark any fields for translation.
    /// </summary>
    None = 0,

    /// <summary>
    /// Marks the <see cref="MessageContent.Message"/> for parsing placeholders.
    /// </summary>
    Message = 1 << 0,

    /// <summary>
    /// Marks the embed's <see cref="EmbedBuilder.Title"/> for parsing placeholders.
    /// </summary>
    EmbedTitle = 1 << 1,

    /// <summary>
    /// Marks the embed's <see cref="EmbedBuilder.Description"/> for parsing placeholders.
    /// </summary>
    EmbedDescription = 1 << 2,

    /// <summary>
    /// Marks the embed's <see cref="EmbedBuilder.ThumbnailUrl"/> for parsing placeholders.
    /// </summary>
    EmbedThumbnailUrl = 1 << 3,

    /// <summary>
    /// Marks the embed's <see cref="EmbedBuilder.ImageUrl"/> for parsing placeholders.
    /// </summary>
    EmbedImageUrl = 1 << 4,

    /// <summary>
    /// Marks the embed's <see cref="EmbedBuilder.Url"/> for parsing placeholders.
    /// </summary>
    EmbedUrl = 1 << 5,

    /// <summary>
    /// Marks all the embed's <see cref="EmbedFieldBuilder.Name"/> for parsing placeholders.
    /// </summary>
    EmbedFieldNames = 1 << 6,

    /// <summary>
    /// Marks all the embed's <see cref="EmbedFieldBuilder.Value"/> for parsing placeholders.
    /// </summary>
    EmbedFieldValues = 1 << 7,

    /// <summary>
    /// Marks the embed's <see cref="EmbedAuthorBuilder.Name"/> for parsing placeholders.
    /// </summary>
    EmbedAuthorName = 1 << 8,

    /// <summary>
    /// Marks the embed's <see cref="EmbedAuthorBuilder.Url"/> for parsing placeholders.
    /// </summary>
    EmbedAuthorUrl = 1 << 9,

    /// <summary>
    /// Marks the embed's <see cref="EmbedAuthorBuilder.IconUrl"/> for parsing placeholders.
    /// </summary>
    EmbedAuthorIconUrl = 1 << 10,

    /// <summary>
    /// Marks the embed's <see cref="EmbedFooterBuilder.Text"/> for parsing placeholders.
    /// </summary>
    EmbedFooterText = 1 << 11,

    /// <summary>
    /// Marks the embed's <see cref="EmbedFooterBuilder.IconUrl"/> for parsing placeholders.
    /// </summary>
    EmbedFooterIconUrl = 1 << 12,
}