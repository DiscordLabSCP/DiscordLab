using Discord;
using DiscordLab.Bot;
using DiscordLab.Bot.API.Attributes;
using DiscordLab.Bot.API.Features;
using DiscordLab.Dependency;
using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.Handlers;
using LabApi.Features;
using LabApi.Features.Wrappers;

namespace DiscordLab.BotStatus;

public class Plugin : Plugin<Config, Translation>
{
    public static Plugin Instance;
        
    public override string Name { get; } = "DiscordLab.BotStatus";
    public override string Description { get; } = "Allows your bot's status to update with player counts.";
    public override string Author { get; } = "LumiFae";
    public override Version Version { get; } = typeof(Plugin).Assembly.GetName().Version;
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
        if(Round.IsRoundInProgress)
            UpdateStatus();
        else
            Queue.Process();
    }

    public static void OnPlayerLeave(ReferenceHub _)
    {
        if(Round.IsRoundInProgress)
            UpdateStatus();
        else
            Queue.Process();
    }

    private static Queue Queue { get; } = new(5, UpdateStatus);
        
    private static void UpdateStatus()
    {
        TranslationBuilder builder = new(Server.PlayerCount == 0 ? Instance.Translation.EmptyContent : Instance.Translation.NormalContent);
        Task.Run(async () => await Client.SocketClient.SetGameAsync(builder, type:Instance.Config.ActivityType).ConfigureAwait(false));
        switch (Server.PlayerCount)
        {
            case 0 when Instance.Config.IdleOnEmpty:
                Task.Run(async () => await Client.SocketClient.SetStatusAsync(UserStatus.Idle).ConfigureAwait(false));
                break;
            case > 0 when Instance.Config.IdleOnEmpty &&
                          Client.SocketClient.Status == UserStatus.Idle:
                Task.Run(async () => await Client.SocketClient.SetStatusAsync(UserStatus.Online).ConfigureAwait(false));
                break;
        }
    }
}