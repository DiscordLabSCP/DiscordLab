using Discord;
using Discord.WebSocket;
using DiscordLab.Bot.API.Interfaces;
using Exiled.API.Features;
using JetBrains.Annotations;

namespace DiscordLab.ModerationLogs.Handlers
{
    public class DiscordBot : IRegisterable
    {
        private static Translation Translation => Plugin.Instance.Translation;

        public static DiscordBot Instance { get; private set; }

        private SocketTextChannel BanChannel { get; set; }
        private SocketTextChannel UnbanChannel { get; set; }
        private SocketTextChannel KickChannel { get; set; }
        private SocketTextChannel MuteChannel { get; set; }
        private SocketTextChannel UnmuteChannel { get; set; }
        private SocketTextChannel AdminChatChannel { get; set; }
        private SocketTextChannel ReportChannel { get; set; }

        private SocketGuild Guild { get; set; }

        public void Init()
        {
            Instance = this;
        }

        public void Unregister()
        {
            BanChannel = null;
            UnbanChannel = null;
            KickChannel = null;
            MuteChannel = null;
            UnmuteChannel = null;
            AdminChatChannel = null;
            ReportChannel = null;
        }

        private SocketGuild GetGuild()
        {
            return Guild ??= Bot.Handlers.DiscordBot.Instance.GetGuild(Plugin.Instance.Config.GuildId);
        }

        public SocketTextChannel GetBanChannel()
        {
            if (GetGuild() == null) return null;
            if (Plugin.Instance.Config.BanChannelId == 0) return null;
            return BanChannel ??=
                Guild.GetTextChannel(Plugin.Instance.Config.BanChannelId);
        }

        public SocketTextChannel GetUnbanChannel()
        {
            if (GetGuild() == null) return null;
            if (Plugin.Instance.Config.UnbanChannelId == 0) return null;
            return UnbanChannel ??=
                Guild.GetTextChannel(Plugin.Instance.Config.UnbanChannelId);
        }

        public SocketTextChannel GetKickChannel()
        {
            if (GetGuild() == null) return null;
            if (Plugin.Instance.Config.KickChannelId == 0) return null;
            return KickChannel ??=
                Guild.GetTextChannel(Plugin.Instance.Config.KickChannelId);
        }

        public SocketTextChannel GetMuteChannel()
        {
            if (GetGuild() == null) return null;
            if (Plugin.Instance.Config.MuteChannelId == 0) return null;
            return MuteChannel ??=
                Guild.GetTextChannel(Plugin.Instance.Config.MuteChannelId);
        }

        public SocketTextChannel GetUnmuteChannel()
        {
            if (GetGuild() == null) return null;
            if (Plugin.Instance.Config.UnmuteChannelId == 0) return null;
            return UnmuteChannel ??=
                Guild.GetTextChannel(Plugin.Instance.Config.UnmuteChannelId);
        }

        public SocketTextChannel GetAdminChatChannel()
        {
            if (GetGuild() == null) return null;
            if (Plugin.Instance.Config.AdminChatChannelId == 0) return null;
            return AdminChatChannel ??=
                Guild.GetTextChannel(Plugin.Instance.Config.AdminChatChannelId);
        }

        public SocketTextChannel GetReportChannel()
        {
            if (GetGuild() == null) return null;
            if (Plugin.Instance.Config.ReportChannelId == 0) return null;
            return ReportChannel ??=
                Guild.GetTextChannel(Plugin.Instance.Config.ReportChannelId);
        }
        
        public SocketTextChannel GetRemoteAdminChannel()
        {
            if (GetGuild() == null) return null;
            if (Plugin.Instance.Config.RemoteAdminChannelId == 0) return null;
            return Guild.GetTextChannel(Plugin.Instance.Config.RemoteAdminChannelId);
        }


        // These 2 functions are here, and public because they are used in DiscordLab.Moderation when the ban commands are used.
        public void SendBanMessage([CanBeNull] string targetName, string targetId, string reason, string issuerName,
            [CanBeNull] string issuerId, string duration)
        {
            SocketTextChannel channel = GetBanChannel();
            if (channel == null)
            {
                if (Plugin.Instance.Config.BanChannelId == 0) return;
                Log.Error("Either the guild is null or the channel is null. So the ban message has failed to send.");
                return;
            }

            EmbedBuilder embed = new();
            embed.WithTitle(Translation.PlayerBanned);
            embed.WithColor(Plugin.GetColor(Plugin.Instance.Config.BanColor));
            if (targetName != null) embed.AddField(Translation.Player, targetName);
            embed.AddField(Translation.PlayerId, targetId);
            embed.AddField(Translation.Reason, reason);
            embed.AddField(Translation.Issuer, issuerName);
            if (issuerId != null) embed.AddField(Translation.IssuerId, issuerId);
            embed.AddField(Translation.Duration, duration);
            channel.SendMessageAsync(embed: embed.Build());
        }

        public void SendUnbanMessage(string targetId)
        {
            SocketTextChannel channel = GetUnbanChannel();
            if (channel == null)
            {
                if (Plugin.Instance.Config.UnbanChannelId == 0) return;
                Log.Error("Either the guild is null or the channel is null. So the unban message has failed to send.");
                return;
            }

            EmbedBuilder embed = new();
            embed.WithTitle(Translation.PlayerUnbanned);
            embed.WithColor(Plugin.GetColor(Plugin.Instance.Config.UnbanColor));
            embed.AddField(Translation.PlayerId, targetId);
            channel.SendMessageAsync(embed: embed.Build());
        }
    }
}