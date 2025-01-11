using Discord.WebSocket;
using DiscordLab.Bot.API.Interfaces;

namespace DiscordLab.RoundLogs.Handlers
{
    public class DiscordBot : IRegisterable
    {
        public static DiscordBot Instance { get; private set; }

        private SocketGuild Guild { get; set; }
        
        private SocketTextChannel RoundStartChannel { get; set; }
        private SocketTextChannel RoundEndChannel { get; set; }
        
        private SocketTextChannel CuffedChannel { get; set; }
        private SocketTextChannel UncuffedChannel { get; set; }
        
        private SocketTextChannel NtfEnterChannel { get; set; }
        private SocketTextChannel ChaosEnterChannel { get; set; }
        
        public void Init()
        {
            Instance = this;
        }

        public void Unregister()
        {
            Guild = null;
            RoundStartChannel = null;
            RoundEndChannel = null;
            CuffedChannel = null;
            UncuffedChannel = null;
            NtfEnterChannel = null;
            ChaosEnterChannel = null;
        }
        
        private SocketGuild GetGuild()
        {
            return Guild ??= Bot.Handlers.DiscordBot.Instance.GetGuild(Plugin.Instance.Config.GuildId);
        }
        
        public SocketTextChannel GetRoundStartChannel()
        {
            if (GetGuild() == null) return null;
            if (Plugin.Instance.Config.RoundStartChannelId == 0) return null;
            return RoundStartChannel ??=
                Guild.GetTextChannel(Plugin.Instance.Config.RoundStartChannelId);
        }
        
        public SocketTextChannel GetRoundEndChannel()
        {
            if (GetGuild() == null) return null;
            if (Plugin.Instance.Config.RoundEndChannelId == 0) return null;
            return RoundEndChannel ??=
                Guild.GetTextChannel(Plugin.Instance.Config.RoundEndChannelId);
        }
        
        public SocketTextChannel GetCuffedChannel()
        {
            if (GetGuild() == null) return null;
            if (Plugin.Instance.Config.CuffedChannelId == 0) return null;
            return CuffedChannel ??=
                Guild.GetTextChannel(Plugin.Instance.Config.CuffedChannelId);
        }
        
        public SocketTextChannel GetUncuffedChannel()
        {
            if (GetGuild() == null) return null;
            if (Plugin.Instance.Config.UncuffedChannelId == 0) return null;
            return UncuffedChannel ??=
                Guild.GetTextChannel(Plugin.Instance.Config.UncuffedChannelId);
        }
        
        public SocketTextChannel GetNtfEnterChannel()
        {
            if (GetGuild() == null) return null;
            if (Plugin.Instance.Config.NtfSpawnChannelId == 0) return null;
            return NtfEnterChannel ??=
                Guild.GetTextChannel(Plugin.Instance.Config.NtfSpawnChannelId);
        }
        
        public SocketTextChannel GetChaosEnterChannel()
        {
            if (GetGuild() == null) return null;
            if (Plugin.Instance.Config.ChaosSpawnChannelId == 0) return null;
            return ChaosEnterChannel ??=
                Guild.GetTextChannel(Plugin.Instance.Config.ChaosSpawnChannelId);
        }
    }
}