// ReSharper disable MemberCanBePrivate.Global
namespace DiscordLab.Bot.API.Features;

using Discord.Rest;
using Discord.WebSocket;
using DiscordLab.Bot.API.Extensions;
using YamlDotNet.Serialization;

/// <summary>
/// Message config object for either string messages or embeds.
/// </summary>
public class MessageContent
{
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

        (Discord.Embed? embed, string? content) = Build(builder);

        channel.SendMessage(content, embed: embed);
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

        (Discord.Embed? embed, string? content) = Build(builder);

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

        Task.Run(async () => await message.ModifyAsync(msg =>
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

        (Discord.Embed? embed, string? content) = Build(builder);

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

        (Discord.Embed? embed, string? content) = Build(builder);

        await command.ModifyOriginalResponseAsync(msg =>
        {
            if (!string.IsNullOrEmpty(content))
                msg.Content = content;

            if (embed != null)
                msg.Embed = embed;
        });
    }

    private (Discord.Embed? Embed, string? Content) Build(TranslationBuilder builder)
    {
        if (Embed == null)
            return (null, Message != null ? builder.Build(Message) : null);

        Discord.EmbedBuilder embed = Embed;
        if (!string.IsNullOrEmpty(embed.Description))
            embed.Description = builder.Build(embed.Description);

        foreach (Discord.EmbedFieldBuilder field in embed.Fields.Where(field => field.Value is string value && !string.IsNullOrEmpty(value)))
        {
            field.Value = builder.Build((string)field.Value);
        }

        return (embed.Build(), Message != null ? builder.Build(Message) : null);
    }
}