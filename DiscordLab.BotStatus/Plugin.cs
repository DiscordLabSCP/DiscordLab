using DiscordLab.Core;
using DiscordLab.Core.API.Enums;
using DiscordLab.Core.API.Extensions;
using DiscordLab.Core.API.Features;
using DiscordLab.Core.API.TranslationBuilders;
using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.Handlers;
using LabApi.Features;
using LabApi.Features.Console;
using LabApi.Features.Wrappers;

namespace DiscordLab.BotStatus;

public class Plugin : Plugin<Config, Translation>
{
    public static Plugin Instance;

    public override string Name { get; } = "DiscordLab.BotStatus";
    public override string Description { get; } = "Allows your bot's status to update with player counts.";
    public override string Author { get; } = "LumiFae";
    public override Version Version => GetType().Assembly.GetName().Version;
    public override Version RequiredApiVersion { get; } = new(LabApiProperties.CompiledVersion);

    public override void Enable()
    {
        Instance = this;

        PlayerEvents.Joined += OnPlayerJoin;
        ReferenceHub.OnPlayerRemoved += OnPlayerLeave;

        ServerEvents.WaitingForPlayers += OnWaitingForPlayers;
    }

    public override void Disable()
    {
        ServerEvents.WaitingForPlayers -= OnWaitingForPlayers;

        PlayerEvents.Joined -= OnPlayerJoin;
        ReferenceHub.OnPlayerRemoved -= OnPlayerLeave;

        Instance = null;
    }

    public static void OnWaitingForPlayers()
    {
        UpdateStatus();
    }

    public static void OnPlayerJoin(PlayerJoinedEventArgs _)
    {
        if (Round.IsRoundInProgress)
            UpdateStatus();
        else
            Queue.Process();
    }

    public static void OnPlayerLeave(ReferenceHub _)
    {
        if (Round.IsRoundInProgress)
            UpdateStatus();
        else
            Queue.Process();
    }

    private static Queue Queue { get; } = new(5, UpdateStatus);

    private static void UpdateStatus()
    {
        int playerCount = Player.List.Count(p => p.IsPlayer && p.IsReady && !p.IsNpc);
        TranslationBuilder builder = new(playerCount == 0
            ? Instance.Translation.EmptyContent
            : Instance.Translation.NormalContent);

        Task.RunAndLog(() => MessageHandler.EditActivities(builder, Instance.Config.ActivityType));
        switch (playerCount)
        {
            case 0 when Instance.Config.IdleOnEmpty:
                Task.RunAndLog(async () => await MessageHandler.SetStatuses(StatusType.Idle).ConfigureAwait(false), OnException);
                break;
            case > 0 when Instance.Config.IdleOnEmpty:
                Task.RunAndLog(async () => await MessageHandler.SetStatuses(StatusType.Online).ConfigureAwait(false), OnException);
                break;
        }
    }

    private static void OnException(Exception ex)
    {
        if (ex is InvalidOperationException)
            return;
        
        Logger.Error(ex);
    }
}