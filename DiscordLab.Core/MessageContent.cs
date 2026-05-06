// ReSharper disable MemberCanBePrivate.Global

using DiscordLab.Bot.API.Features.Embed;
using DiscordLab.Core;
using NorthwoodLib.Pools;

namespace DiscordLab.Bot.API.Features;

using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using UnityEngine;
using YamlDotNet.Serialization;

/// <summary>
/// Message config object for either string messages or embeds.
/// </summary>
public class MessageContent
{
    private const TranslatableMessageField DefaultTranslatableFields =
        TranslatableMessageField.Message |
        TranslatableMessageField.EmbedDescription |
        TranslatableMessageField.EmbedFieldValues |
        TranslatableMessageField.EmbedFooterText;

    /// <summary>
    /// Gets or sets the embed to send, if any.
    /// </summary>
    [YamlMember(DefaultValuesHandling = DefaultValuesHandling.OmitNull)]
    public Embed.EmbedBuilder? Embed { get; set; }

    /// <summary>
    /// Gets or sets the string to send, if any.
    /// </summary>
    [YamlMember(DefaultValuesHandling = DefaultValuesHandling.OmitNull)]
    public string? Message { get; set; }

    /// <summary>
    /// Gets or sets which parts of the message should have their placeholders replaced.
    /// </summary>
    [YamlMember(DefaultValuesHandling = DefaultValuesHandling.OmitDefaults)]
    [DefaultValue(DefaultTranslatableFields)]
    public TranslatableMessageField TranslatedFields { get; set; } = DefaultTranslatableFields;

    /// <summary>
    /// Converts an embed into a <see cref="MessageContent"/> instance.
    /// </summary>
    /// <param name="embed">The embed.</param>
    /// <returns>The <see cref="MessageContent"/> instance.</returns>
    public static implicit operator MessageContent(Embed.EmbedBuilder embed) => new() { Embed = embed };

    /// <summary>
    /// Converts a string into a <see cref="MessageContent"/> instance.
    /// </summary>
    /// <param name="content">The content.</param>
    /// <returns>The <see cref="MessageContent"/> instance.</returns>
    public static implicit operator MessageContent(string content) => new() { Message = content };

    /// <summary>
    /// Checks whether the specified field has been marked as translatable in <see cref="TranslatedFields"/>.
    /// </summary>
    /// <param name="field">The field to check.</param>
    /// <returns>Whether the field is present in the <see cref="TranslatedFields"/>.</returns>
    public bool FieldMarkedTranslatable(TranslatableMessageField field) =>
        (TranslatedFields & field) != 0;

    /// <summary>
    /// Builds the embed and/or content assigned to this <see cref="MessageContent"/> using a <see cref="TranslationBuilder"/>.
    /// </summary>
    /// <param name="builder">The <see cref="TranslationBuilder"/> to use.</param>
    /// <returns>The embed and content with replaced values.</returns>
    /// <exception cref="ArgumentException">Throws when message content is too long after being built.</exception>
    public (EmbedBuilder? Embed, string? Content) Build(TranslationBuilder builder)
    {
        List<string> contents = ListPool<string>.Shared.Rent();

        string OrEmpty(string? content) => content ?? string.Empty;
        
        contents.Add(OrEmpty(Message));
        contents.Add(OrEmpty(Embed?.Title));
        contents.Add(OrEmpty(Embed?.Description));
        contents.Add(OrEmpty(Embed?.Url));
        contents.Add(OrEmpty(Embed?.ImageUrl));
        contents.Add(OrEmpty(Embed?.ThumbnailUrl));
        contents.Add(OrEmpty(Embed?.Footer?.Text));
        contents.Add(OrEmpty(Embed?.Footer?.IconUrl));
        
        foreach (EmbedFieldBuilder field in Embed?.Fields ?? [])
        {
            contents.Add(OrEmpty(field.Name));
            contents.Add(OrEmpty(field.Value));
        }

        string delimiter = $"|{Guid.NewGuid()}|";
        string combined = string.Join(delimiter, contents);
        string replaced = builder.Build(combined);
    }

    /// <inheritdoc cref="Build"/>
    /// <remarks>Does the building on the main thread, use this over Build if you use Task.Run.</remarks>
    public async Awaitable<(Discord.Embed? Embed, string? Content)> MainThreadBuild(TranslationBuilder builder)
    {
        await Awaitable.MainThreadAsync();
        return Build(builder);
    }
}