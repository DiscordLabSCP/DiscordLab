using DiscordLab.Bot.API.Interfaces;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;

namespace DiscordLab.StatusModule;

public class Events : IRegisterable
{
    public void Init()
    {
        Exiled.Events.Handlers.Player.Verified += OnPlayerVerified;
        Exiled.Events.Handlers.Player.Left += OnPlayerLeave;
        Exiled.Events.Handlers.Server.WaitingForPlayers += OnWaitingForPlayers;
    }
    
    public void Unregister()
    {
        Exiled.Events.Handlers.Player.Verified -= OnPlayerVerified;
        Exiled.Events.Handlers.Player.Left -= OnPlayerLeave;
        Exiled.Events.Handlers.Server.WaitingForPlayers -= OnWaitingForPlayers;
    }

    private void OnPlayerVerified(VerifiedEventArgs ev)
    {
        if(!Round.IsStarted) return;
        Plugin.Instance.Discord.SetStatus();
        Plugin.Instance.Discord.SetCustomStatus();
    }

    private void OnPlayerLeave(LeftEventArgs ev)
    {
        if(!Round.IsStarted) return;
        Plugin.Instance.Discord.SetStatus();
        Plugin.Instance.Discord.SetCustomStatus();
    }

    private void OnWaitingForPlayers()
    {
        Plugin.Instance.Discord.SetStatus();
        Plugin.Instance.Discord.SetCustomStatus();
    }
}