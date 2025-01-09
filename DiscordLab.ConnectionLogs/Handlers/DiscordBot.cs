using Discord;
using Discord.Rest;
using Discord.WebSocket;
using DiscordLab.Bot.API.Interfaces;
using Exiled.API.Features;
using Newtonsoft.Json.Linq;

namespace DiscordLab.ConnectionLogs.Handlers
{
    public class DiscordBot : IRegisterable
    {
        public static DiscordBot Instance { get; private set; }

        private SocketTextChannel JoinChannel { get; set; }
        private SocketTextChannel LeaveChannel { get; set; }
        private SocketTextChannel RoundStartChannel { get; set; }
        private SocketTextChannel RoundEndChannel { get; set; }

        public void Init()
        {
            Instance = this;
        }

        public void Unregister()
        {
            JoinChannel = null;
            LeaveChannel = null;
            RoundStartChannel = null;
            RoundEndChannel = null;
        }
        
        public SocketGuild GetGuild()
        {
            return Bot.Handlers.DiscordBot.Instance.GetGuild(Plugin.Instance.Config.GuildId);
        }

        public SocketTextChannel GetJoinChannel()
        {
            SocketGuild guild = GetGuild();
            if (guild == null) return null;
            if (Plugin.Instance.Config.JoinChannelId == 0) return null;
            return JoinChannel ??=
                guild.GetTextChannel(Plugin.Instance.Config.JoinChannelId);
        }

        public SocketTextChannel GetLeaveChannel()
        {
            SocketGuild guild = GetGuild();
            if (guild == null) return null;
            if (Plugin.Instance.Config.LeaveChannelId == 0) return null;
            return LeaveChannel ??=
                guild.GetTextChannel(Plugin.Instance.Config.LeaveChannelId);
        }

        public SocketTextChannel GetRoundStartChannel()
        {
            SocketGuild guild = GetGuild();
            if (guild == null) return null;
            if (Plugin.Instance.Config.RoundStartChannelId == 0) return null;
            return RoundStartChannel ??=
               guild.GetTextChannel(Plugin.Instance.Config.RoundStartChannelId);
        }
        
        public SocketTextChannel GetRoundEndChannel()
        {
            SocketGuild guild = GetGuild();
            if (guild == null) return null;
            if (Plugin.Instance.Config.RoundEndChannelId == 0) return null;
            return RoundEndChannel ??=
                guild.GetTextChannel(Plugin.Instance.Config.RoundEndChannelId);
        }
    }
}