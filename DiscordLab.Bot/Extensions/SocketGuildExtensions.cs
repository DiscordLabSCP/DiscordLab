using Discord;
using Discord.WebSocket;

namespace DiscordLab.Bot.Extensions;

public static class SocketGuildExtensions
{
    public static SocketTextChannel GetTextChannel(this SocketGuild guild, ulong id)
    {
        SocketChannel channel = guild.GetChannel(id);
        if(channel.GetChannelType() is not ChannelType.Text) throw new InvalidCastException("Channel is not a text channel.");
        return channel as SocketTextChannel;
    }
}