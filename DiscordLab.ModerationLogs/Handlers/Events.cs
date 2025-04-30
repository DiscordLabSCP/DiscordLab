using System.Globalization;
using Discord;
using Discord.WebSocket;
using DiscordLab.Bot.API.Interfaces;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.EventArgs.Server;

namespace DiscordLab.ModerationLogs.Handlers
{
    public class Events : IRegisterable
    {
        private static Translation Translation => Plugin.Instance.Translation;

        public void Init()
        {
            Exiled.Events.Handlers.Player.Banned += OnBanned;
            Exiled.Events.Handlers.Server.Unbanned += OnUnbanned;
            Exiled.Events.Handlers.Player.Kicking += OnKicking;
            Exiled.Events.Handlers.Player.IssuingMute += OnIssuingMute;
            Exiled.Events.Handlers.Player.RevokingMute += OnIssuingUnmute;
            Exiled.Events.Handlers.Player.SendingAdminChatMessage += OnSendingAdminChatMessage;
            Exiled.Events.Handlers.Server.LocalReporting += OnLocalReporting;
        }

        public void Unregister()
        {
            Exiled.Events.Handlers.Player.Banned -= OnBanned;
            Exiled.Events.Handlers.Server.Unbanned -= OnUnbanned;
            Exiled.Events.Handlers.Player.Kicking -= OnKicking;
            Exiled.Events.Handlers.Player.IssuingMute -= OnIssuingMute;
            Exiled.Events.Handlers.Player.RevokingMute -= OnIssuingUnmute;
            Exiled.Events.Handlers.Player.SendingAdminChatMessage -= OnSendingAdminChatMessage;
            Exiled.Events.Handlers.Server.LocalReporting -= OnLocalReporting;
        }

        private void OnLocalReporting(LocalReportingEventArgs ev)
        {
            if (!ev.IsAllowed) return;
            SocketTextChannel channel = DiscordBot.Instance.GetReportChannel();
            if (channel == null)
            {
                if (Plugin.Instance.Config.ReportChannelId == 0) return;
                Log.Error("Either the guild is null or the channel is null. So the kick message has failed to send.");
                return;
            }

            EmbedBuilder embed = new();
            embed.WithTitle(Translation.PlayerReported);
            embed.WithColor(Plugin.GetColor(Plugin.Instance.Config.ReportColor));
            embed.AddField(Translation.Target, ev.Target.Nickname);
            embed.AddField(Translation.TargetId, ev.Target.UserId);
            embed.AddField(Translation.Reason, ev.Reason);
            embed.AddField(Translation.Reporter, ev.Player.Nickname);
            embed.AddField(Translation.ReporterId, ev.Player.UserId);
            channel.SendMessageAsync(embed: embed.Build());
        }

        private void OnBanned(BannedEventArgs ev)
        {
            DiscordBot.Instance.SendBanMessage(ev.Details.OriginalName,
                ev.Details.Id,
                ev.Details.Reason,
                ev.Player.Nickname,
                ev.Player.UserId,
                 $"<t:{Plugin.ExpiresToString(new (ev.Details.Expires))}:R>"
            );
        }

        private void OnUnbanned(UnbannedEventArgs ev)
        {
            DiscordBot.Instance.SendUnbanMessage(ev.TargetId);
        }

        private void OnKicking(KickingEventArgs ev)
        {
            if (!ev.IsAllowed) return;
            SocketTextChannel channel = DiscordBot.Instance.GetKickChannel();
            if (channel == null)
            {
                if (Plugin.Instance.Config.KickChannelId == 0) return;
                Log.Error("Either the guild is null or the channel is null. So the kick message has failed to send.");
                return;
            }

            EmbedBuilder embed = new();
            embed.WithTitle(Translation.PlayerKicked);
            embed.WithColor(Plugin.GetColor(Plugin.Instance.Config.KickColor));
            embed.AddField(Translation.Player, ev.Target.Nickname);
            embed.AddField(Translation.PlayerId, ev.Target.UserId);
            embed.AddField(Translation.Reason, ev.Reason);
            embed.AddField(Translation.Issuer, ev.Player.Nickname);
            embed.AddField(Translation.IssuerId, ev.Player.UserId);
            channel.SendMessageAsync(Translation.PlayerKickedContent, embed: embed.Build());
        }

        private void OnIssuingMute(IssuingMuteEventArgs ev)
        {
            if (!ev.IsAllowed) return;
            SocketTextChannel channel = DiscordBot.Instance.GetMuteChannel();
            if (channel == null)
            {
                if (Plugin.Instance.Config.MuteChannelId == 0) return;
                Log.Error("Either the guild is null or the channel is null. So the mute message has failed to send.");
                return;
            }

            EmbedBuilder embed = new();
            embed.WithTitle(Translation.PlayerMuted);
            embed.WithColor(Plugin.GetColor(Plugin.Instance.Config.MuteColor));
            embed.AddField(Translation.Player, ev.Player.Nickname);
            embed.AddField(Translation.PlayerId, ev.Player.UserId);
            embed.AddField(Translation.Issuer, ev.Player.Nickname);
            embed.AddField(Translation.IssuerId, ev.Player.UserId);
            channel.SendMessageAsync(Translation.PlayerMutedContent, embed: embed.Build());
        }

        private void OnIssuingUnmute(RevokingMuteEventArgs ev)
        {
            if (!ev.IsAllowed) return;
            SocketTextChannel channel = DiscordBot.Instance.GetUnmuteChannel();
            if (channel == null)
            {
                if (Plugin.Instance.Config.UnmuteChannelId == 0) return;
                Log.Error("Either the guild is null or the channel is null. So the unmute message has failed to send.");
                return;
            }

            EmbedBuilder embed = new();
            embed.WithTitle(Translation.PlayerMuteRevoked);
            embed.WithColor(Plugin.GetColor(Plugin.Instance.Config.UnmuteColor));
            embed.AddField(Translation.Player, ev.Player.Nickname);
            embed.AddField(Translation.PlayerId, ev.Player.UserId);
            embed.AddField(Translation.Issuer, ev.Player.Nickname);
            embed.AddField(Translation.IssuerId, ev.Player.UserId);
            channel.SendMessageAsync(Translation.PlayerMuteRevokedContent, embed: embed.Build());
        }

        private void OnSendingAdminChatMessage(SendingAdminChatMessageEventsArgs ev)
        {
            if (!ev.IsAllowed) return;
            SocketTextChannel channel = DiscordBot.Instance.GetAdminChatChannel();
            if (channel == null)
            {
                if (Plugin.Instance.Config.AdminChatChannelId == 0) return;
                Log.Error(
                    "Either the guild is null or the channel is null. So the admin chat message has failed to send.");
                return;
            }

            EmbedBuilder embed = new();
            embed.WithTitle(Translation.AdminChatMessage);
            embed.WithColor(Plugin.GetColor(Plugin.Instance.Config.AdminChatColor));
            embed.AddField(Translation.Player, ev.Player.Nickname);
            embed.AddField(Translation.PlayerId, ev.Player.UserId);
            embed.AddField(Translation.Message, ev.Message);
            channel.SendMessageAsync(embed: embed.Build());
        }
    }
}