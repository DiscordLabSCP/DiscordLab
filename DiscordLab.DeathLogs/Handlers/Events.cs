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
        SocketTextChannel channel;
        bool isCuffed = ev.Player.IsCuffed;
        
        if (ev.Attacker == null) isCuffed = false;
        if(ev.Attacker != null && ev.Attacker.Role.Type == ev.Player.Role.Type && ev.Attacker != ev.Player) isCuffed = false;
        
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
            if(ev.Attacker != null && ev.Attacker != ev.Player)
                channel = DiscordBot.Instance.GetChannel();
            else if(ev.Attacker.Role.Type == ev.Player.Role.Type)
                channel = DiscordBot.Instance.GetTeamKillChannel();
            else
                channel = DiscordBot.Instance.GetSelfChannel();
        }

        if (channel == null)
        {
            Log.Error("Either the guild is null or the channel is null. So the death message has failed to send.");
            return;
        }

        string message;
        
        if(isCuffed) message = Plugin.Instance.Translation.CuffedPlayerDeath;
        else if(ev.Attacker != null) message = Plugin.Instance.Translation.PlayerDeath;
        else if (ev.Attacker != null && ev.Attacker.Role.Type == ev.Player.Role.Type) message = Plugin.Instance.Translation.TeamKill;
        else message = Plugin.Instance.Translation.PlayerDeathSelf;
        
        if(ev.Attacker != null) message = message
            .Replace("{attacker}", ev.Attacker.Nickname)
            .Replace("{attackerrole}", ev.Attacker.Role.Name);

        message = message
            .Replace("{player}", ev.Player.Nickname)
            .Replace("{playerrole}", ev.Player.Role.Name)
            .Replace("{role}", ev.Player.Role.Name);
        
        channel.SendMessageAsync(message);
    }
}