using DiscordLab.Core.API.Extensions;
using DiscordLab.Core.API.TranslationBuilders;
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
        
        Translation.PlayerJoin.Send(Config.JoinChannel, new PlayerTranslationBuilder("player", ev.Player));
    }

    public override void OnPlayerLeft(PlayerLeftEventArgs ev)
    {
        if (!Round.IsRoundInProgress)
            return;
        
        if (ev.Player.ReferenceHub.serverRoles.HideFromPlayerList)
            return;

        Translation.PlayerLeave.Send(Config.LeaveChannel, new PlayerTranslationBuilder("player", ev.Player));
    }

    public override void OnServerRoundStarted()
    {
        Translation.RoundStart.Send(Config.RoundStartChannel, new AllPlayersTranslationBuilder(Translation.RoundPlayers));
    }

    public override void OnServerRoundEnded(RoundEndedEventArgs ev)
    {
        TranslationBuilder builder = new AllPlayersTranslationBuilder(Translation.RoundPlayers)
            .AddCustomReplacer("winner", ev.LeadingTeam.ToString());

        Translation.RoundEnd.Send(Config.RoundEndChannel, builder);
    }
}