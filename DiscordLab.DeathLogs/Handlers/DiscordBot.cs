using Discord.WebSocket;
using DiscordLab.Bot.API.Interfaces;

namespace DiscordLab.DeathLogs.Handlers
{
    public class DiscordBot : IRegisterable
    {
        public static DiscordBot Instance { get; private set; }

        private SocketTextChannel Channel { get; set; }
        private SocketTextChannel CuffedChannel { get; set; }
        private SocketTextChannel SelfChannel { get; set; }
        private SocketTextChannel TeamKillChannel { get; set; }

        public void Init()
        {
            Instance = this;
        }

        public void Unregister()
        {
            Channel = null;
            CuffedChannel = null;
            SelfChannel = null;
            TeamKillChannel = null;
        }
        
        public SocketGuild GetGuild()
        {
            return Bot.Handlers.DiscordBot.Instance.GetGuild(Plugin.Instance.Config.GuildId);
        }

        public SocketTextChannel GetChannel()
        {
            SocketGuild guild = GetGuild();
            if (GetGuild() == null) return null;
            if (Plugin.Instance.Config.ChannelId == 0) return null;
            return Channel ??= guild.GetTextChannel(Plugin.Instance.Config.ChannelId);
        }

        public SocketTextChannel GetCuffedChannel()
        {
            SocketGuild guild = GetGuild();
            if (guild == null) return null;
            if (Plugin.Instance.Config.CuffedChannelId == 0) return null;
            return CuffedChannel ??=
                guild.GetTextChannel(Plugin.Instance.Config.CuffedChannelId);
        }

        public SocketTextChannel GetSelfChannel()
        {
            SocketGuild guild = GetGuild();
            if (guild == null) return null;
            if (Plugin.Instance.Config.SelfChannelId == 0) return null;
            return SelfChannel ??=
                guild.GetTextChannel(Plugin.Instance.Config.SelfChannelId);
        }

        public SocketTextChannel GetTeamKillChannel()
        {
            SocketGuild guild = GetGuild();
            if (guild == null) return null;
            if (Plugin.Instance.Config.TeamKillChannelId == 0) return null;
            return TeamKillChannel ??=
                guild.GetTextChannel(Plugin.Instance.Config.TeamKillChannelId);
        }
    }
}