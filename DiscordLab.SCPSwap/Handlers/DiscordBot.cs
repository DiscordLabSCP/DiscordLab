using Discord.WebSocket;
using DiscordLab.Bot.API.Interfaces;

namespace DiscordLab.SCPSwap.Handlers
{
    public class DiscordBot : IRegisterable
    {
        public static DiscordBot Instance { get; private set; }
        
        private SocketTextChannel Channel { get; set; }
        
        public void Init()
        {
            Instance = this;
        }

        public void Unregister()
        {
            Channel = null;
        }
        
        public SocketTextChannel GetChannel()
        {
            SocketGuild guild = Bot.Handlers.DiscordBot.Instance.GetGuild(Plugin.Instance.Config.GuildId);
            if (guild == null) return null;
            if (Plugin.Instance.Config.ChannelId == 0) return null;
            return Channel ??= guild.GetTextChannel(Plugin.Instance.Config.ChannelId);
        }
    }
}