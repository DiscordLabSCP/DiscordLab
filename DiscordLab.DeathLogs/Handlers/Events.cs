using Discord.WebSocket;
using DiscordLab.Bot.API.Interfaces;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;

namespace DiscordLab.DeathLogs.Handlers;

public class Events : IRegisterable
{
    public void Init()
    {
        Exiled.Events.Handlers.Player.Dying += OnPlayerDying;
    }
    
    public void Unregister()
    {
        Exiled.Events.Handlers.Player.Dying -= OnPlayerDying;
    }

    private void OnPlayerDying(DyingEventArgs ev)
    {
        if (ev.Attacker == null || ev.Player == ev.Attacker) return;
        SocketTextChannel channel;
        bool isCuffed = ev.Player.IsCuffed;

        if (isCuffed)
        {
            channel = DiscordBot.Instance.GetCuffedChannel();
            if (channel == null)
            {
                isCuffed = false;
                channel = DiscordBot.Instance.GetChannel();
            }
        } 
        else
        {
            channel = DiscordBot.Instance.GetChannel();
        }

        if (channel == null)
        {
            Log.Error("Either the guild is null or the channel is null. So the death message has failed to send.");
            return;
        }

        if (ev.Attacker == null) isCuffed = false;

        string message =
            (isCuffed ? Plugin.Instance.Translation.CuffedPlayerDeath :
                ev.Attacker != null ? Plugin.Instance.Translation.PlayerDeath :
                Plugin.Instance.Translation.PlayerDeathSelf)
            .Replace("{player}", ev.Player.Nickname)
            .Replace("{playerrole}", ev.Player.Role.Name);
        
        if(ev.Attacker != null) message = message
            .Replace("{attacker}", ev.Attacker.Nickname)
            .Replace("{attackerrole}", ev.Attacker.Role.Name);
        
        channel.SendMessageAsync(message);
    }
}