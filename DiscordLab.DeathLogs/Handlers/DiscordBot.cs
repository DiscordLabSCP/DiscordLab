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

        public SocketTextChannel GetChannel()
        {
            if (Bot.Handlers.DiscordBot.Instance.Guild == null) return null;
            if (Plugin.Instance.Config.ChannelId == 0) return null;
            return Channel ??= Bot.Handlers.DiscordBot.Instance.Guild.GetTextChannel(Plugin.Instance.Config.ChannelId);
        }

        public SocketTextChannel GetCuffedChannel()
        {
            if (Bot.Handlers.DiscordBot.Instance.Guild == null) return null;
            if (Plugin.Instance.Config.CuffedChannelId == 0) return null;
            return CuffedChannel ??=
                Bot.Handlers.DiscordBot.Instance.Guild.GetTextChannel(Plugin.Instance.Config.CuffedChannelId);
        }

        public SocketTextChannel GetSelfChannel()
        {
            if (Bot.Handlers.DiscordBot.Instance.Guild == null) return null;
            if (Plugin.Instance.Config.SelfChannelId == 0) return null;
            return SelfChannel ??=
                Bot.Handlers.DiscordBot.Instance.Guild.GetTextChannel(Plugin.Instance.Config.SelfChannelId);
        }

        public SocketTextChannel GetTeamKillChannel()
        {
            if (Bot.Handlers.DiscordBot.Instance.Guild == null) return null;
            if (Plugin.Instance.Config.TeamKillChannelId == 0) return null;
            return TeamKillChannel ??=
                Bot.Handlers.DiscordBot.Instance.Guild.GetTextChannel(Plugin.Instance.Config.TeamKillChannelId);
        }
    }
}