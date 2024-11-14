using Discord;
using Discord.WebSocket;

namespace DiscordLab.Bot.Extensions;

public static class IMessageExtensions
{
    public static bool IsUserMessage(this IMessage message) => message is SocketUserMessage;
}