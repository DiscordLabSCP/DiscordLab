using DiscordLab.Bot.API.Interfaces;
using DiscordLab.Bot.API.Modules;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;

namespace DiscordLab.BotStatus.Handlers;

public class Events : IRegisterable
{
    public void Init()
    {
        Exiled.Events.Handlers.Server.WaitingForPlayers += OnWaitingForPlayers;
        Exiled.Events.Handlers.Server.RoundStarted += OnRoundStarted;
        Exiled.Events.Handlers.Player.Verified += OnPlayerVerified;
        Exiled.Events.Handlers.Player.Left += OnPlayerLeave;
    }
    
    public void Unregister()
    {
        Exiled.Events.Handlers.Server.WaitingForPlayers -= OnWaitingForPlayers;
        Exiled.Events.Handlers.Server.RoundStarted -= OnRoundStarted;
        Exiled.Events.Handlers.Player.Verified -= OnPlayerVerified;
        Exiled.Events.Handlers.Player.Left -= OnPlayerLeave;
    }
    
    private void OnPlayerVerified(VerifiedEventArgs ev)
    {
        if(Round.InProgress) DiscordBot.Instance.SetStatus();
        else QueueSystem.QueueRun("DiscordLab.BotStatus.OnPlayerVerified", () => 
            DiscordBot.Instance.SetStatus()
        );
    }
    
    private void OnPlayerLeave(LeftEventArgs ev)
    {
        int players = Player.List.Count(p => p != ev.Player);
        if(Round.InProgress || players == 0) 
            DiscordBot.Instance.SetStatus(
                players
            );
        else 
            QueueSystem.QueueRun("DiscordLab.BotStatus.OnPlayerLeave", () => 
                DiscordBot.Instance.SetStatus()
            );
    }

    private void OnRoundStarted()
    {
        DiscordBot.Instance.SetStatus();
    }
    
    private void OnWaitingForPlayers()
    {
        DiscordBot.Instance.SetStatus();
    }
}