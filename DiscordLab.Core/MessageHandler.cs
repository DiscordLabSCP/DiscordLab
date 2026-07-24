using System.Diagnostics.CodeAnalysis;
using DiscordLab.Core.API.Enums;
using DiscordLab.Core.API.Extensions;

namespace DiscordLab.Core;

public abstract class MessageHandler
{
    public static HashSet<MessageHandler> Handlers = new();

    public static async Task SendMessages(string destination, MessageInformation information,
        bool saveIds = false) => await Task.WhenAll(Handlers.Select(async handler =>
    {
        string? messageId = await handler.SendMessage(destination, information);
        if (saveIds && messageId != null)
            handler.SaveMessageId(destination, messageId);
    }));

    public static async Task SendMessagesCheckExists(string destination, MessageInformation information) =>
        await Task.WhenAll(Handlers.Select(async handler =>
        {
            if (!handler.TryGetMessageId(destination, out string? id))
                return handler.SendMessage(destination, information)
                    .Then(result => handler.SaveMessageId(destination, result!));

            try
            {
                await handler.ThrowIfMissingMessage(destination, id);
            }
            catch (Exception)
            {
                return handler.SendMessage(destination, information)
                    .Then(result => handler.SaveMessageId(destination, result!));
            }

            return Task.CompletedTask;
        }));

    public static async Task SendToWebhooks(string destination, MessageInformation information) =>
        await Task.WhenAll(Handlers.Select(handler => handler.SendToWebhook(destination, information)));

    public static async Task EditMessages(string destination, MessageInformation information) => await Task.WhenAll(
        Handlers.Select(handler => handler.TryGetMessageId(destination, out string? id) ? handler.EditMessage(destination, id, information) : Task.CompletedTask));

    public static async Task DeleteMessages(string destination) => await Task.WhenAll(
        Handlers.Select(handler => handler.TryGetMessageId(destination, out string? id) ? handler.DeleteMessage(destination, id) : Task.CompletedTask));

    public static async Task SetStatuses(StatusType status) =>
        await Task.WhenAll(Handlers.Select(handler => handler.SetStatus(status)));
    
    public static async Task EditActivities(string activity, ActivityType type = ActivityType.Custom) =>
        await Task.WhenAll(Handlers.Select(handler => handler.EditActivity(activity, type)));
    
    public abstract Task<string?> SendMessage(string destination, MessageInformation information);

    public abstract Task SendToWebhook(string destination, MessageInformation information);

    public abstract Task EditMessage(string destination, string identifier, MessageInformation information);

    public abstract Task DeleteMessage(string destination, string identifier);

    public abstract Task ThrowIfMissingMessage(string destination, string identifier);

    public bool TryGetMessageId(string destination, [NotNullWhen(true)] out string? messageId)
    {
        messageId = GetMessageId(destination);
        return messageId != null;
    }
    
    public abstract string? GetMessageId(string destination);

    public abstract void SaveMessageId(string destination, string identifier);

    public virtual Task SetStatus(StatusType status)
    {
        return Task.CompletedTask;
    }
    
    public virtual Task EditActivity(string activity, ActivityType type)
    {
        return Task.CompletedTask;
    }
}