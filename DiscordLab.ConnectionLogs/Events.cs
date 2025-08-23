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

namespace DiscordLab.ConnectionLogs;

public class Events : CustomEventsHandler
{
    public static Config Config => Plugin.Instance.Config;

    public static Translation Translation => Plugin.Instance.Translation;

    public override void OnPlayerJoined(PlayerJoinedEventArgs ev)
    {
        if (!Round.IsRoundInProgress)
            return;

        if (Config.JoinChannelId == 0)
            return;

        if (!Client.TryGetOrAddChannel(Config.JoinChannelId, out SocketTextChannel channel))
        {
            Logger.Error(LoggingUtils.GenerateMissingChannelMessage("join log", Config.JoinChannelId, Config.GuildId));
            return;
        }

        Translation.PlayerJoin.SendToChannel(channel, new("player", ev.Player));
    }

    public override void OnPlayerLeft(PlayerLeftEventArgs ev)
    {
        if (!Round.IsRoundInProgress)
            return;

        if (Config.LeaveChannelId == 0)
            return;

        if (!Client.TryGetOrAddChannel(Config.LeaveChannelId, out SocketTextChannel channel))
        {
            Logger.Error(
                LoggingUtils.GenerateMissingChannelMessage("leave log", Config.LeaveChannelId, Config.GuildId));
            return;
        }

        Translation.PlayerLeave.SendToChannel(channel, new("player", ev.Player));
    }

    public override void OnServerRoundStarted()
    {
        if (Config.RoundStartChannelId == 0)
            return;

        if (!Client.TryGetOrAddChannel(Config.RoundStartChannelId, out SocketTextChannel channel))
        {
            Logger.Error(LoggingUtils.GenerateMissingChannelMessage("round start log", Config.RoundStartChannelId,
                Config.GuildId));
            return;
        }

        Translation.RoundStart.SendToChannel(channel, new()
        {
            PlayerListItem = Translation.RoundPlayers
        });
    }

    public override void OnServerRoundEnded(RoundEndedEventArgs ev)
    {
        if (Config.RoundEndChannelId == 0)
            return;

        if (!Client.TryGetOrAddChannel(Config.RoundEndChannelId, out SocketTextChannel channel))
        {
            Logger.Error(LoggingUtils.GenerateMissingChannelMessage("round start log", Config.RoundEndChannelId,
                Config.GuildId));
            return;
        }

        TranslationBuilder builder = new TranslationBuilder()
            {
                PlayerListItem = Translation.RoundPlayers
            }
            .AddCustomReplacer("winner", ev.LeadingTeam.ToString());

        Translation.RoundEnd.SendToChannel(channel, builder);
    }
}