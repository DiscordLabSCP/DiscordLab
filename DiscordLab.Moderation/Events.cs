using DiscordLab.Core;
using DiscordLab.Core.API.Extensions;
using DiscordLab.Core.API.TranslationBuilders;
using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.Arguments.ServerEvents;
using LabApi.Events.CustomHandlers;
using LabApi.Features.Wrappers;

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
        TranslationBuilder builder = new PlayerTranslationBuilder()
            .AddPlayer("target", ev.Player)
            .AddPlayer("player", ev.Issuer);

        Translation.UnmuteLog.Send(Config.UnmuteLogChannel, builder);
    }

    public override void OnPlayerMuted(PlayerMutedEventArgs ev)
    {
        MessageContent translation = Translation.PermMuteLog;

        if (TempMuteManager.MuteConfig.Mutes.TryGetValue(ev.Player.UserId, out DateTime time))
        {
            translation = Translation.TempMuteLog;
        }
        else
        {
            time = DateTime.Now;
        }

        TranslationBuilder builder = new PlayerTranslationBuilder
            {
                Time = time
            }
            .AddPlayer("player", ev.Issuer)
            .AddPlayer("target", ev.Player);

        translation.Send(Config.MuteLogChannel, builder);
    }

    public override void OnServerBanIssuing(BanIssuingEventArgs ev)
    {
        PlayerTranslationBuilder builder = new()
            {
                Time = new(ev.BanDetails.Expires, DateTimeKind.Utc)
            };

        builder
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

        Translation.BanLogEmbed.Send(Config.BanLogChannel, builder);
    }

    public override void OnServerBanRevoked(BanRevokedEventArgs ev)
    {
        TranslationBuilder builder = new TranslationBuilder()
            .AddCustomReplacer("userid", ev.BanDetails.Id)
            .AddCustomReplacer("username", ev.BanDetails.OriginalName)
            .AddCustomReplacer("playerid", ev.BanDetails.Issuer);

        Translation.UnbanLog.Send(Config.UnbanLogChannel, builder);
    }

    public override void OnServerSentAdminChat(SentAdminChatEventArgs ev)
    {
        PlayerTranslationBuilder builder = new();
            
        builder
            .AddCustomReplacer("message", ev.Message)
            .AddCustomReplacer("sender", ev.Sender.Nickname);

        if (Player.TryGet(ev.Sender, out Player player))
            builder.AddPlayer("player", player);
        
        Translation.AdminChatLog.Send(Config.AdminChatLogChannel, builder);
    }
}