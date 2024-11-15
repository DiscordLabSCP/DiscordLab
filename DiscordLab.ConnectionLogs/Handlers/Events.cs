using DiscordLab.Bot.API.Interfaces;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;

namespace DiscordLab.ConnectionLogs.Handlers;

public class Events : IRegisterable
{
    public void Init()
    {
        Exiled.Events.Handlers.Player.Verified += OnPlayerVerified;
        Exiled.Events.Handlers.Player.Left += OnPlayerLeave;
        Exiled.Events.Handlers.Server.RoundStarted += OnRoundStarted;
    }
    
    public void Unregister()
    {
        Exiled.Events.Handlers.Player.Verified -= OnPlayerVerified;
        Exiled.Events.Handlers.Player.Left -= OnPlayerLeave;
        Exiled.Events.Handlers.Server.RoundStarted -= OnRoundStarted;
    }

    private void OnPlayerVerified(VerifiedEventArgs ev)
    {
        if (Round.InProgress && !string.IsNullOrEmpty(ev.Player.Nickname))
        {
            string message = Plugin.Instance.Translation.PlayerJoin.Replace("{player}", ev.Player.Nickname).Replace("{id}", ev.Player.UserId);
            DiscordBot.Instance.GetJoinChannel().SendMessageAsync(message);
        }
    }
    
    private void OnPlayerLeave(LeftEventArgs ev)
    {
        if (Round.InProgress && !string.IsNullOrEmpty(ev.Player.Nickname))
        {
            string message = Plugin.Instance.Translation.PlayerLeave.Replace("{player}", ev.Player.Nickname).Replace("{id}", ev.Player.UserId);
            DiscordBot.Instance.GetLeaveChannel().SendMessageAsync(message);
        }
    }

    private void OnRoundStarted()
    {
        string message = Plugin.Instance.Translation.RoundStart;
        DiscordBot.Instance.GetRoundStartChannel().SendMessageAsync(message +
                                                                    $"\n```{string.Join("\n", Player.List.Select(player => $"{player.Nickname} ({player.UserId})"))}```");
    }
}