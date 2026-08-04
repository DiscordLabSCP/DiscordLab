namespace DiscordLab.Bot;

extern alias DiscordNet;
using ActivityType = DiscordNet::Discord.ActivityType;

using System.Collections.ObjectModel;
using Discord;
using Discord.Rest;
using Discord.Webhook;
using Discord.WebSocket;
using DiscordLab.Core;
using DiscordLab.Core.API.Enums;
using DiscordLab.Core.API.Extensions;
using LabApi.Loader;

public class MessageHandler : Core.MessageHandler
{
    private bool TryGetFromDestination(string destination, out string channel) =>
        Plugin.Instance.Config.ChannelIds.TryGetValue(destination, out channel);

    private static Embed FromGeneric(Core.API.Embed.EmbedBuilder embed)
    {
        EmbedBuilder builder = new()
        {
            Title = embed.Title,
            Description = embed.Description,
            Fields = embed.Fields?.Select(field => new EmbedFieldBuilder
                { IsInline = field.IsInline, Name = field.Name, Value = field.Value }).ToList(),
            Author = new()
            {
                IconUrl = embed.Author?.IconUrl,
                Name = embed.Author?.Name,
                Url = embed.Author?.Url
            },
            Color = Color.Parse(embed.Color),
            ThumbnailUrl = embed.ThumbnailUrl,
            ImageUrl = embed.ImageUrl,
            Url = embed.Url,
            Footer = new()
            {
                IconUrl = embed.Footer?.IconUrl,
                Text = embed.Footer?.Text
            },
        };

        if (embed.Timestamp)
            builder.Timestamp = DateTimeOffset.UtcNow;

        return builder.Build();
    }

    public static Embed[]? ToEmbeds(MessageInformation information) => information.Embeds?.Select(FromGeneric).ToArray();

    public static FileAttachment? ToAttachment(MessageInformation information) => information.Attachment.HasValue
        ? new FileAttachment(information.Attachment.Value.Stream, information.Attachment.Value.FileName)
        : null;

    private async Task<ulong> WebhookSend(DiscordWebhookClient client, string? content, FileAttachment? attachment, Embed[]? embeds)
    {
        if (!attachment.HasValue)
            return await client.SendMessageAsync(content, embeds: embeds);

        ulong msgId = await client.SendFileAsync(attachment.Value, content, embeds: embeds);

        return msgId;
    }

    internal async Task<ulong> SendToChannel(SocketTextChannel channel, string? content, Embed[]? embeds, FileAttachment? attachment)
    {
        if (!attachment.HasValue)
            return await channel.SendMessageAsync(content, embeds: embeds).Then(msg => msg.Id);

        RestUserMessage msg = await channel.SendFileAsync(attachment.Value, content, embeds: embeds);

        return msg.Id;
    }

    public override async Task<string?> SendMessage(string destination, MessageInformation information)
    {
        if (!TryGetFromDestination(destination, out string channel))
            return null;

        Embed[]? embeds = ToEmbeds(information);
        FileAttachment? attachment = ToAttachment(information);

        if (ulong.TryParse(channel, out ulong channelId) && Client.TryGetOrAddChannel(channelId, out SocketTextChannel socket))
            await SendToChannel(socket, information.Content, embeds, attachment).Then(id => id.ToString());

        // Webhook
        if (Uri.TryCreate(channel, UriKind.Absolute, out Uri _))
        {
            using DiscordWebhookClient client = new(channel);

            ulong msgId = await WebhookSend(client, information.Content, attachment, embeds);

            return msgId.ToString();
        }

        return null;
    }

    public override async Task SendToWebhook(string destination, MessageInformation information)
    {
        if (!TryGetFromDestination(destination, out string channel))
            return;

        Embed[]? embeds = ToEmbeds(information);
        FileAttachment? attachment = ToAttachment(information);

        if (Uri.TryCreate(channel, UriKind.Absolute, out Uri _))
        {
            using DiscordWebhookClient client = new(channel);

            await WebhookSend(client, information.Content, attachment, embeds);

            return;
        }

        if (!ulong.TryParse(channel, out ulong channelId))
            return;

        if (!Client.TryGetOrAddChannel(channelId, out SocketTextChannel socket))
            return;

        IReadOnlyCollection<RestWebhook> sockets = await socket.GetWebhooksAsync();
        RestWebhook? hook = sockets.FirstOrDefault(h => h.Creator == Client.SocketClient.CurrentUser);

        if (hook == null)
            return;

        using DiscordWebhookClient channelClient = new(hook);

        await WebhookSend(channelClient, information.Content, attachment, embeds);
    }

    public static void SetFrom(MessageProperties properties, MessageInformation info)
    {
        properties.Content = info.Content;
        properties.Embeds = ToEmbeds(info);
        
        FileAttachment? attachment = ToAttachment(info);
        if (attachment.HasValue)
            properties.Attachments = new ReadOnlyCollection<FileAttachment>([attachment.Value]);
    }

    public override async Task EditMessage(string destination, string identifier, MessageInformation information)
    {
        if (!ulong.TryParse(identifier, out ulong messageId))
            throw new ArgumentException($"Identifier must be a ulong", nameof(identifier));

        if (!TryGetFromDestination(destination, out string channel))
            return;

        if (!ulong.TryParse(channel, out ulong channelId) || !Client.TryGetOrAddChannel(channelId, out SocketTextChannel socket))
            return;

        await socket.ModifyMessageAsync(messageId, msg => SetFrom(msg, information));
    }

    public override async Task DeleteMessage(string destination, string identifier)
    {
        if (!ulong.TryParse(identifier, out ulong messageId))
            throw new ArgumentException($"Identifier must be a ulong", nameof(identifier));

        if (!TryGetFromDestination(destination, out string channel))
            return;

        if (!ulong.TryParse(channel, out ulong channelId) || !Client.TryGetOrAddChannel(channelId, out SocketTextChannel socket))
            return;

        await socket.DeleteMessageAsync(messageId);
    }

    public override async Task ThrowIfMissingMessage(string destination, string identifier)
    {
        if (!ulong.TryParse(identifier, out ulong messageId))
            throw new ArgumentException($"Identifier must be a ulong", nameof(identifier));

        if (!TryGetFromDestination(destination, out string channel))
            return;

        if (!ulong.TryParse(channel, out ulong channelId) || !Client.TryGetOrAddChannel(channelId, out SocketTextChannel socket))
            return;

        IMessage msg = await socket.GetMessageAsync(messageId);

        if (msg == null)
            throw new();
    }

    public override async Task SetStatus(StatusType status)
    {
        UserStatus userStatus = status switch
        {
            StatusType.Online => UserStatus.Online,
            StatusType.Idle => UserStatus.Idle,
            StatusType.DoNotDisturb => UserStatus.DoNotDisturb,
            _ => throw new ArgumentOutOfRangeException(nameof(status), status, null)
        };

        await Client.SocketClient.SetStatusAsync(userStatus);
    }

    public override async Task EditActivity(string activity, DiscordLab.Core.API.Enums.ActivityType type)
    {
        ActivityType activityType = type switch
        {
            Core.API.Enums.ActivityType.Playing => ActivityType.Playing,
            Core.API.Enums.ActivityType.Custom => ActivityType.CustomStatus,
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };

        await Client.SocketClient.SetGameAsync(activity, type: activityType);
    }

    public override string? GetMessageId(string destination)
    {
        if (Plugin.Instance.MessageConfig.MessageIds.TryGetValue(destination, out ulong messageId))
            return messageId.ToString();

        return null;
    }

    public override void SaveMessageId(string destination, string identifier)
    {
        if(!ulong.TryParse(identifier, out ulong messageId))
            throw new ArgumentException($"Identifier must be a ulong", nameof(identifier));
        
        Plugin.Instance.MessageConfig.MessageIds.Add(destination, messageId);
        Plugin.Instance.SaveConfig(Plugin.Instance.MessageConfig, "message-config.yml");
    }
}