// ReSharper disable MemberCanBePrivate.Global

namespace DiscordLab.Bot.API.Features;

using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using Discord.Rest;
using Discord.WebSocket;
using DiscordLab.Bot.API.Extensions;
using DiscordLab.Bot.API.Utilities;
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
    /// Sends this message to a channel.
    /// </summary>
    /// <param name="channel">The channel to send the message to.</param>
    /// <param name="builder">The <see cref="TranslationBuilder"/> instance to utilise.</param>
    public void SendToChannel(SocketTextChannel channel, TranslationBuilder builder)
    {
        if (Embed == null && Message == null)
            throw new ArgumentNullException($"A message failed to send to {channel.Name} ({channel.Id}) because both embed and message contents were undefined.");

        if (channel == null)
            throw new ArgumentNullException(nameof(channel));

        if (builder == null)
            throw new ArgumentNullException(nameof(builder));

        MethodBase method = new StackFrame(1).GetMethod();
        Task.RunAndLog(async () => await SendToChannelAsync(channel, builder), ex => LoggingUtils.LogMethodError(ex, method));
    }

    /// <summary>
    /// Sends this message to a channel, asynchronously.
    /// </summary>
    /// <param name="channel">The channel to send the message to.</param>
    /// <param name="builder">The <see cref="TranslationBuilder"/> instance to utilise.</param>
    /// <returns>The new message object.</returns>
    public async Task<RestUserMessage> SendToChannelAsync(SocketTextChannel channel, TranslationBuilder builder)
    {
        if (Embed == null && Message == null)
            throw new ArgumentNullException($"A message failed to send to {channel.Name} ({channel.Id}) because both embed and message contents were undefined.");

        if (channel == null)
            throw new ArgumentNullException(nameof(channel));

        if (builder == null)
            throw new ArgumentNullException(nameof(builder));

        (Discord.Embed? embed, string? content) = await MainThreadBuild(builder);

        return await channel.SendMessageAsync(content, embed: embed);
    }

    /// <summary>
    /// Modifies the <see cref="Discord.IUserMessage"/> instance with this message and builder.
    /// </summary>
    /// <param name="message">The <see cref="Discord.IUserMessage"/> instance.</param>
    /// <param name="builder">The <see cref="TranslationBuilder"/> instance to utilise.</param>
    public void ModifyMessage(Discord.IUserMessage message, TranslationBuilder builder)
    {
        if (Embed == null && Message == null)
            throw new ArgumentNullException($"Message {message.Id} (in #{message.Channel.Name} ({message.Channel.Id})) failed to be edited because both embed and message contents were undefined.");

        (Discord.Embed? embed, string? content) = Build(builder);

        Task.RunAndLog(async () => await message.ModifyAsync(msg =>
        {
            if (!string.IsNullOrEmpty(content))
                msg.Content = content;

            if (embed != null)
                msg.Embed = embed;
        }).ConfigureAwait(false));
    }

    /// <summary>
    /// Responds to a <see cref="SocketCommandBase"/> with this message and builder.
    /// </summary>
    /// <param name="command">The <see cref="SocketCommandBase"/> instance.</param>
    /// <param name="builder">The <see cref="TranslationBuilder"/> instance to utilise.</param>
    /// <returns>This task.</returns>
    public async Task InteractionRespond(SocketCommandBase command, TranslationBuilder builder)
    {
        if (Embed == null && Message == null)
            throw new ArgumentNullException($"Failed to respond to command {command.CommandName} because both embed and message contents were undefined.");

        (Discord.Embed? embed, string? content) = await MainThreadBuild(builder);

        await command.RespondAsync(content, embed: embed);
    }

    /// <summary>
    /// Modifies a <see cref="SocketCommandBase"/>'s response with the new message and builder.
    /// </summary>
    /// <param name="command">The <see cref="SocketCommandBase"/> instance.</param>
    /// <param name="builder">The <see cref="TranslationBuilder"/> instance to utilise.</param>
    /// <returns>This task.</returns>
    public async Task ModifyInteraction(SocketCommandBase command, TranslationBuilder builder)
    {
        if (Embed == null && Message == null)
            throw new ArgumentNullException($"Failed to modify command {command.CommandName}'s response because both embed and message contents were undefined.");

        (Discord.Embed? embed, string? content) = await MainThreadBuild(builder);

        await command.ModifyOriginalResponseAsync(msg =>
        {
            if (!string.IsNullOrEmpty(content))
                msg.Content = content;

            if (embed != null)
                msg.Embed = embed;
        });
    }

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
    public (Discord.Embed? Embed, string? Content) Build(TranslationBuilder builder)
    {
        string? content = Message != null && FieldMarkedTranslatable(TranslatableMessageField.Message) ? builder.Build(Message) : Message;

        if (content is { Length: > Discord.DiscordConfig.MaxMessageSize })
            throw new ArgumentException($"Message content is too long, length must be less or equal to {Discord.DiscordConfig.MaxMessageSize}. This is after compiling the message.", nameof(Message));

        if (Embed == null)
            return (null, content);

        Discord.EmbedBuilder embed = Embed;

        if (FieldMarkedTranslatable(TranslatableMessageField.EmbedTitle) && !string.IsNullOrEmpty(embed.Title))
            embed.Title = builder.Build(embed.Title);
        if (FieldMarkedTranslatable(TranslatableMessageField.EmbedDescription) && !string.IsNullOrEmpty(embed.Description))
            embed.Description = builder.Build(embed.Description);
        if (FieldMarkedTranslatable(TranslatableMessageField.EmbedUrl) && !string.IsNullOrEmpty(embed.Url))
            embed.Url = builder.Build(embed.Url);
        if (FieldMarkedTranslatable(TranslatableMessageField.EmbedThumbnailUrl) && !string.IsNullOrEmpty(embed.ThumbnailUrl))
            embed.ThumbnailUrl = builder.Build(embed.ThumbnailUrl);
        if (FieldMarkedTranslatable(TranslatableMessageField.EmbedImageUrl) && !string.IsNullOrEmpty(embed.ImageUrl))
            embed.ImageUrl = builder.Build(embed.ImageUrl);

        if (embed.Author != null)
        {
            if (FieldMarkedTranslatable(TranslatableMessageField.EmbedAuthorName) && !string.IsNullOrEmpty(embed.Author.Name))
                embed.Author.Name = builder.Build(embed.Author.Name);
            if (FieldMarkedTranslatable(TranslatableMessageField.EmbedAuthorUrl) && !string.IsNullOrEmpty(embed.Author.Url))
                embed.Author.Url = builder.Build(embed.Author.Url);
            if (FieldMarkedTranslatable(TranslatableMessageField.EmbedAuthorIconUrl) && !string.IsNullOrEmpty(embed.Author.IconUrl))
                embed.Author.IconUrl = builder.Build(embed.Author.IconUrl);
        }

        if (embed.Footer != null)
        {
            if (FieldMarkedTranslatable(TranslatableMessageField.EmbedFooterText) && !string.IsNullOrEmpty(embed.Footer.Text))
                embed.Footer.Text = builder.Build(embed.Footer.Text);
            if (FieldMarkedTranslatable(TranslatableMessageField.EmbedFooterIconUrl) && !string.IsNullOrEmpty(embed.Footer.IconUrl))
                embed.Footer.IconUrl = builder.Build(embed.Footer.IconUrl);
        }

        if (FieldMarkedTranslatable(TranslatableMessageField.EmbedFieldNames))
        {
            foreach (Discord.EmbedFieldBuilder field in embed.Fields)
            {
                if (!string.IsNullOrEmpty(field.Name))
                    field.Name = builder.Build(field.Name);
            }
        }

        if (FieldMarkedTranslatable(TranslatableMessageField.EmbedFieldValues))
        {
            foreach (Discord.EmbedFieldBuilder field in embed.Fields)
            {
                if (field.Value is string value && !string.IsNullOrEmpty(value))
                    field.Value = builder.Build(value);
            }
        }

        return (embed.Build(), content);
    }

    /// <inheritdoc cref="Build"/>
    /// <remarks>Does the building on the main thread, use this over Build if you use Task.Run.</remarks>
    public async Awaitable<(Discord.Embed? Embed, string? Content)> MainThreadBuild(TranslationBuilder builder)
    {
        await Awaitable.MainThreadAsync();
        return Build(builder);
    }
}