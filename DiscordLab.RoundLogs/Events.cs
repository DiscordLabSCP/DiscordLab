using DiscordLab.Core;
using DiscordLab.Core.API.Extensions;
using DiscordLab.Core.API.TranslationBuilders;
using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.Arguments.ServerEvents;
using LabApi.Events.Arguments.WarheadEvents;
using LabApi.Events.CustomHandlers;
using PlayerRoles;
using LabApi.Features.Extensions;
using LabApi.Features.Wrappers;

namespace DiscordLab.RoundLogs;

public class Events : CustomEventsHandler
{
    public static Config Config => Plugin.Instance.Config;

    public static Translation Translation => Plugin.Instance.Translation;

    public override void OnPlayerChangedRole(PlayerChangedRoleEventArgs ev)
    {
        if (ev.ChangeReason is RoleChangeReason.Respawn or RoleChangeReason.RoundStart
            or RoleChangeReason.RespawnMiniwave or RoleChangeReason.LateJoin or RoleChangeReason.Died
            or RoleChangeReason.Destroyed)
            return;

        if (ev.OldRole == ev.NewRole.RoleTypeId)
            return;

        TranslationBuilder builder = new PlayerTranslationBuilder("player", ev.Player)
            .AddCustomReplacer("oldrole", () => ev.OldRole.GetFullName())
            .AddCustomReplacer("newrole", ev.NewRole.RoleName)
            .AddCustomReplacer("reason", ev.ChangeReason.ToString())
            .AddCustomReplacer("spawnflags", string.Join(", ", ev.SpawnFlags.GetFlags()));

        if (ev.NewRole.Team == ev.OldRole.GetTeam() && ev.NewRole.Team == Team.SCPs)
        {
            Translation.ScpSwapLog.Send(Config.ScpSwapChannel, builder);
            return;
        }

        Translation.RoleChangeLog.Send(Config.RoleChangeChannel, builder);
    }

    public override void OnServerWaveRespawned(WaveRespawnedEventArgs ev)
    {
        bool isFoundation = ev.Wave is MtfWave or MiniMtfWave;

        string channel = isFoundation ? Config.NtfSpawnChannel : Config.ChaosSpawnChannel;

        MessageContent content = isFoundation ? Translation.NtfSpawn : Translation.ChaosSpawn;

        TranslationBuilder builder = new PlayerListTranslationBuilder(ev.Players, Translation.PlayerListItem);

        content.Send(channel, builder);
    }

    public override void OnPlayerCuffed(PlayerCuffedEventArgs ev)
    {
        TranslationBuilder builder = new PlayerTranslationBuilder()
            .AddPlayer("target", ev.Target)
            .AddPlayer("player", ev.Player);

        Translation.Cuffed.Send(Config.CuffedChannel, builder);
    }

    public override void OnPlayerUncuffed(PlayerUncuffedEventArgs ev)
    {
        TranslationBuilder builder = new PlayerTranslationBuilder()
            .AddPlayer("target", ev.Target)
            .AddPlayer("player", ev.Player);

        Translation.Uncuffed.Send(Config.UncuffedChannel, builder);
    }

    public override void OnServerRoundStarted()
    {
        Translation.RoundStart.Send(Config.RoundStartedChannel, new());
    }

    public override void OnServerRoundEnded(RoundEndedEventArgs ev)
    {
        TranslationBuilder builder = new TranslationBuilder()
            .AddCustomReplacer("winner", ev.LeadingTeam.ToString());

        Translation.RoundEnd.Send(Config.RoundEndedChannel, builder);
    }

    public override void OnServerLczDecontaminationStarted()
    {
        Translation.Decontamination.Send(Config.DecontaminationChannel, new());
    }

    public override void OnPlayerEscaped(PlayerEscapedEventArgs ev)
    {
        TranslationBuilder builder = new PlayerTranslationBuilder("player", ev.Player)
            .AddCustomReplacer("type", () => ev.EscapeScenarioType.ToString())
            .AddCustomReplacer("newrole", () => ev.NewRole.GetFullName())
            .AddCustomReplacer("oldrole", () => ev.OldRole.GetFullName());
        
        Translation.Escape.Send(Config.EscapeChannel, builder);
    }

    public override void OnWarheadStarted(WarheadStartedEventArgs ev)
    {
        Translation.WarheadActivated.Send(Config.WarheadActivatedChannel, new PlayerTranslationBuilder("player", ev.Player));
    }

    public override void OnWarheadStopped(WarheadStoppedEventArgs ev)
    {
        Translation.WarheadDeactivated.Send(Config.WarheadActivatedChannel, new PlayerTranslationBuilder("player", ev.Player));
    }
}