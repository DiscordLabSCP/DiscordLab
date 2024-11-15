using DiscordLab.Bot.API.Interfaces;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using Round = PluginAPI.Core.Round;

namespace DiscordLab.StatusLog;

public class Events : IRegisterable
{
    public void Init()
    {
        Exiled.Events.Handlers.Player.Verified += OnPlayerVerified;
        Exiled.Events.Handlers.Player.Left += OnPlayerLeave;
        Exiled.Events.Handlers.Server.RoundStarted += OnRoundStart;
    }

    public void Unregister()
    {
        Exiled.Events.Handlers.Player.Verified -= OnPlayerVerified;
        Exiled.Events.Handlers.Player.Left -= OnPlayerLeave;
        Exiled.Events.Handlers.Server.RoundStarted -= OnRoundStart;
    }

    private string Replacer(string str, Player player)
    {
        return str.Replace("{player}", player.Nickname).Replace("{playerid}", player.UserId);
    }
    
    private void OnPlayerVerified(VerifiedEventArgs ev)
    {
        if (!string.IsNullOrEmpty(ev.Player.Nickname) && Round.IsRoundStarted)
            Plugin.Instance.Channel.SendMessageAsync(Replacer(Plugin.Instance.Translation.PlayerJoin, ev.Player));
    }
    
    private void OnPlayerLeave(LeftEventArgs ev)
    {
        if (!string.IsNullOrEmpty(ev.Player.Nickname) && Round.IsRoundStarted)
            Plugin.Instance.Channel.SendMessageAsync(Replacer(Plugin.Instance.Translation.PlayerLeave, ev.Player));
    }

    private void OnRoundStart()
    {
        var message = Plugin.Instance.Translation.RoundStart;
        message += "\n```";
        message += string.Join("\n", Player.List.Select(player => $"{player.Nickname} ({player.UserId})"));
        message += "```";
        Plugin.Instance.Channel.SendMessageAsync(message);
    }
}