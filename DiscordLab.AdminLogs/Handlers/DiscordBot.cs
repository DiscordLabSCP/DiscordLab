using Discord.WebSocket;
using DiscordLab.Bot.API.Interfaces;

namespace DiscordLab.AdminLogs.Handlers
{
    public class DiscordBot : IRegisterable
    {
        public static DiscordBot Instance { get; private set; }

        private SocketTextChannel ErrorLogsChannel { get; set; }
        
        private SocketTextChannel ServerStartChannel { get; set; }
        
        private SocketGuild Guild { get; set; }

        public void Init()
        {
            Instance = this;
        }

        public void Unregister()
        {
            ErrorLogsChannel = null;
            ServerStartChannel = null;
        }

        private SocketGuild GetGuild()
        {
            return Guild ??= Bot.Handlers.DiscordBot.Instance.GetGuild(Plugin.Instance.Config.GuildId);
        }

        public SocketTextChannel GetErrorLogsChannel()
        {
            if (GetGuild() == null) return null;
            if (Plugin.Instance.Config.ErrorLogChannelId == 0) return null;
            return ErrorLogsChannel ??=
                Guild.GetTextChannel(Plugin.Instance.Config.ErrorLogChannelId);
        }
        
        public SocketTextChannel GetServerStartChannel()
        {
            if (GetGuild() == null) return null;
            if (Plugin.Instance.Config.ServerStartChannelId == 0) return null;
            return ServerStartChannel ??=
                Guild.GetTextChannel(Plugin.Instance.Config.ServerStartChannelId);
        }
    }
}