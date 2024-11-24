using Discord.WebSocket;
using DiscordLab.Bot.API.Interfaces;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;

namespace DiscordLab.DeathLogs.Handlers
{
    public class Events : IRegisterable
    {
        public void Init()
        {
            Exiled.Events.Handlers.Player.Dying += OnTeamKillDeath;
            Exiled.Events.Handlers.Player.Dying += OnCuffKillDeath;
            Exiled.Events.Handlers.Player.Dying += OnNormalDeath;
            Exiled.Events.Handlers.Player.Dying += OnSuicide;
        }

        public void Unregister()
        {
            Exiled.Events.Handlers.Player.Dying -= OnTeamKillDeath;
            Exiled.Events.Handlers.Player.Dying -= OnCuffKillDeath;
            Exiled.Events.Handlers.Player.Dying -= OnNormalDeath;
            Exiled.Events.Handlers.Player.Dying -= OnSuicide;
        }

        private void OnTeamKillDeath(DyingEventArgs ev)
        {
            if (ev.Attacker == null) return;
            if (ev.Attacker.Role.Type != ev.Player.Role.Type) return;
            if(Plugin.Instance.Config.TeamKillChannelId == 0) return;
            SocketTextChannel channel = DiscordBot.Instance.GetTeamKillChannel();
            if (channel == null)
            {
                Log.Error("Either the guild is null or the channel is null. So the death message has failed to send.");
                return;
            }

            channel.SendMessageAsync(
                Plugin.Instance.Translation.TeamKill
                    .Replace("{player}", ev.Player.Nickname)
                    .Replace("{attacker}", ev.Attacker.Nickname)
                    .Replace("{role}",ev.Player.Role.Name)
            );
        }

        private void OnCuffKillDeath(DyingEventArgs ev)
        {
            if(ev.Attacker == null) return;
            if(ev.Attacker == ev.Player) return;
            if(ev.Attacker.IsScp && Plugin.Instance.Config.ScpIgnoreCuffed) return;
            if(!ev.Player.IsCuffed) return;
            if (Plugin.Instance.Config.CuffedChannelId == 0)
            {
                ev.Player.Cuffer = null;
                OnNormalDeath(ev);
                return;
            }
            SocketTextChannel channel = DiscordBot.Instance.GetCuffedChannel();
            if (channel == null)
            {
                Log.Error("Either the guild is null or the channel is null. So the death message has failed to send.");
                return;
            }

            channel.SendMessageAsync(
                Plugin.Instance.Translation.CuffedPlayerDeath
                    .Replace("{player}", ev.Player.Nickname)
                    .Replace("{attacker}", ev.Attacker.Nickname)
                    .Replace("{playerrole}", ev.Player.Role.Name)
                    .Replace("{attackerrole}", ev.Attacker.Role.Name)
            );

        }

        private void OnNormalDeath(DyingEventArgs ev)
        {
            if(ev.Attacker == null) return;
            if(ev.Attacker == ev.Player) return;
            if(ev.Attacker.Role.Type == ev.Player.Role.Type) return;
            if(ev.Player.IsCuffed) return;
            if (Plugin.Instance.Config.ChannelId == 0) return;
            SocketTextChannel channel = DiscordBot.Instance.GetChannel();
            if (channel == null)
            {
                Log.Error("Either the guild is null or the channel is null. So the death message has failed to send.");
                return;
            }
            channel.SendMessageAsync(
                Plugin.Instance.Translation.PlayerDeath
                    .Replace("{player}", ev.Player.Nickname)
                    .Replace("{attacker}", ev.Attacker.Nickname)
                    .Replace("{playerrole}", ev.Player.Role.Name)
                    .Replace("{attackerrole}", ev.Attacker.Role.Name)
            );
        }

        private void OnSuicide(DyingEventArgs ev)
        {
            if (ev.Attacker != null && ev.Attacker != ev.Player) return;
            if (Plugin.Instance.Config.SelfChannelId == 0) return;
            SocketTextChannel channel = DiscordBot.Instance.GetSelfChannel();
            if (channel == null)
            {
                Log.Error("Either the guild is null or the channel is null. So the death message has failed to send.");
                return;
            }
            channel.SendMessageAsync(
                Plugin.Instance.Translation.PlayerDeathSelf
                    .Replace("{player}", ev.Player.Nickname)
                    .Replace("{playerrole}", ev.Player.Role.Name)
            );
        }
    }
}