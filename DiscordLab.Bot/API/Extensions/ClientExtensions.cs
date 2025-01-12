using Discord.WebSocket;
using DiscordLab.Bot.API.Enums;

namespace DiscordLab.Bot.API.Extensions
{
    public static class ClientExtensions
    {
        public static GuildReturn TryGetGuild(this DiscordSocketClient client, ulong guildId, out SocketGuild guild)
        {
            if (guildId == 0)
            {
                guild = null;
                return GuildReturn.NoGuild;
            }
            SocketGuild tempGuild = client.GetGuild(guildId);
            if (tempGuild == null)
            {
                guild = null;
                return GuildReturn.InvalidGuild;
            }
            guild = tempGuild;
            return GuildReturn.Found;
        }
        
        public static ChannelReturn TryGetTextChannel(this DiscordSocketClient client, ulong channelId, out SocketTextChannel channel)
        {
            if (channelId == 0)
            {
                channel = null;
                return ChannelReturn.NoChannel;
            }
            SocketChannel tempChannel = client.GetChannel(channelId);
            if (tempChannel == null)
            {
                channel = null;
                return ChannelReturn.InvalidChannel;
            }
            if(tempChannel is not SocketTextChannel textChannel)
            {
                channel = null;
                return ChannelReturn.InvalidType;
            }
            channel = textChannel;
            return ChannelReturn.Found;
        }

        public static ChannelReturn TryGetTextChannel(this SocketGuild guild, ulong channelId,
            out SocketTextChannel channel)
        {
            if(channelId == 0)
            {
                channel = null;
                return ChannelReturn.NoChannel;
            }
            SocketTextChannel tempChannel = guild.GetTextChannel(channelId);
            if (tempChannel == null)
            {
                channel = null;
                return ChannelReturn.InvalidChannel;
            }
            channel = tempChannel;
            return ChannelReturn.Found;
        }
    }
}