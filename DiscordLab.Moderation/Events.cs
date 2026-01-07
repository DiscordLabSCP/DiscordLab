using Discord;
using Discord.WebSocket;
using DiscordLab.Bot;
using DiscordLab.Bot.API.Extensions;
using DiscordLab.Bot.API.Features;
using DiscordLab.Bot.API.Utilities;
using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.Arguments.ServerEvents;
using LabApi.Events.CustomHandlers;
using LabApi.Features.Console;
using LabApi.Features.Wrappers;
using RemoteAdmin;

namespace DiscordLab.Moderation;

public class Events : CustomEventsHandler
{
    public static Config Config => Plugin.Instance.Config;

    public static Translation Translation => Plugin.Instance.Translation;

    public override void OnPlayerUnmuting(PlayerUnmutingEventArgs ev)
    {
        // otherwise OnPlayerUnmuted will get triggered twice.
        ev.IsAllowed = false;

        TempMuteManager.RemoveMute(ev.Player, ev.Issuer);
    }

    public override void OnPlayerUnmuted(PlayerUnmutedEventArgs ev)
    {
        if (Config.UnmuteLogChannelId == 0)
            return;

        if (!Client.TryGetOrAddChannel(Config.UnmuteLogChannelId, out SocketTextChannel channel))
        {
            Logger.Error(LoggingUtils.GenerateMissingChannelMessage("unmute logs", Config.UnmuteLogChannelId,
                Config.GuildId));
            return;
        }

        TranslationBuilder builder = new TranslationBuilder()
            .AddPlayer("target", ev.Player)
            .AddPlayer("player", ev.Issuer);

        Translation.UnmuteLog.SendToChannel(channel, builder);
    }

    public override void OnPlayerMuted(PlayerMutedEventArgs ev)
    {
        if (Config.MuteLogChannelId == 0)
            return;

        if (!Client.TryGetOrAddChannel(Config.MuteLogChannelId, out SocketTextChannel channel))
        {
            Logger.Error(
                LoggingUtils.GenerateMissingChannelMessage("mute logs", Config.MuteLogChannelId, Config.GuildId));
            return;
        }

        MessageContent translation = Translation.PermMuteLog;

        if (TempMuteManager.MuteConfig.Mutes.TryGetValue(ev.Player.UserId, out DateTime time))
        {
            translation = Translation.TempMuteLog;
        }
        else
        {
            time = DateTime.Now;
        }

        TranslationBuilder builder = new TranslationBuilder
            {
                Time = time
            }
            .AddPlayer("player", ev.Issuer)
            .AddPlayer("target", ev.Player);

        translation.SendToChannel(channel, builder);
    }

    public override void OnServerBanIssuing(BanIssuingEventArgs ev)
    {
        if (Config.BanLogChannelId == 0)
            return;

        if (!Client.TryGetOrAddChannel(Config.BanLogChannelId, out SocketTextChannel channel))
        {
            Logger.Error(
                LoggingUtils.GenerateMissingChannelMessage("ban logs", Config.BanLogChannelId, Config.GuildId));
            return;
        }

        TranslationBuilder builder = new TranslationBuilder
            {
                Time = new(ev.BanDetails.Expires)
            }
            .AddCustomReplacer("userid", ev.BanDetails.Id)
            .AddCustomReplacer("reason", ev.BanDetails.Reason);

        if (Player.TryGet(ev.BanDetails.Id, out Player player))
        {
            builder.AddPlayer("player", player);
        }

        if (Player.TryGet(ev.BanDetails.Issuer, out Player issuer))
        {
            builder.AddPlayer("issuer", issuer);
        }
        else
        {
            builder.AddCustomReplacer("issuerid", ev.BanDetails.Issuer);
        }

        Translation.BanLogEmbed.SendToChannel(channel, builder);
    }

    public override void OnServerBanRevoked(BanRevokedEventArgs ev)
    {
        if (Config.UnbanLogChannelId == 0)
            return;

        if (!Client.TryGetOrAddChannel(Config.UnbanLogChannelId, out SocketTextChannel channel))
        {
            Logger.Error(
                LoggingUtils.GenerateMissingChannelMessage("unban logs", Config.UnbanLogChannelId, Config.GuildId));
            return;
        }

        TranslationBuilder builder = new TranslationBuilder()
            .AddCustomReplacer("userid", ev.BanDetails.Id)
            .AddCustomReplacer("username", ev.BanDetails.OriginalName)
            .AddCustomReplacer("playerid", ev.BanDetails.Issuer);

        Translation.UnbanLog.SendToChannel(channel, builder);
    }

    public override void OnServerSentAdminChat(SentAdminChatEventArgs ev)
    {
        if (Config.AdminChatLogChannelId == 0)
            return;

        if (!Client.TryGetOrAddChannel(Config.AdminChatLogChannelId, out SocketTextChannel channel))
        {
            Logger.Error(
                LoggingUtils.GenerateMissingChannelMessage("admin chat logs", Config.AdminChatLogChannelId, Config.GuildId));
            return;
        }

        TranslationBuilder builder = new TranslationBuilder()
            .AddCustomReplacer("message", ev.Message)
            .AddCustomReplacer("sender", ev.Sender.Nickname);

        if (Player.TryGet(ev.Sender, out Player player))
            builder.AddPlayer("player", player);
        
        Translation.AdminChatLog.SendToChannel(channel, builder);
    }
}