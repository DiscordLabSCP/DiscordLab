using Discord.WebSocket;
using DiscordLab.Bot.API.Extensions;
using DiscordLab.Bot.API.Interfaces;
using InventorySystem.Disarming;
using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.Handlers;
using LabApi.Features.Console;
using PlayerRoles;
using LabApi.Features.Extensions;

namespace DiscordLab.DeathLogs.Handlers
{
    public class Events : IRegisterable
    {
        public void Init()
        {
            PlayerEvents.Dying += OnTeamKillDeath;
            PlayerEvents.Dying += OnCuffKillDeath;
            PlayerEvents.Dying += OnNormalDeath;
            PlayerEvents.Dying += OnSuicide;
        }

        public void Unregister()
        {
            PlayerEvents.Dying -= OnTeamKillDeath;
            PlayerEvents.Dying -= OnCuffKillDeath;
            PlayerEvents.Dying -= OnNormalDeath;
            PlayerEvents.Dying -= OnSuicide;
        }

        private void OnTeamKillDeath(PlayerDyingEventArgs ev)
        {
            if (ev.Attacker == null) return;
            if (ev.Attacker == ev.Player) return;
            if (RoleExtensions.GetTeam(ev.Attacker.Role) != RoleExtensions.GetTeam(ev.Player.Role)) return;
            if(Plugin.Instance.Config.TeamKillChannelId == 0) return;
            SocketTextChannel? channel = DiscordBot.Instance.GetTeamKillChannel();
            if (channel == null)
            {
                Logger.Error("Either the guild is null or the channel is null. So the death message has failed to send.");
                return;
            }

            channel.SendMessageAsync(
                Plugin.Instance.Translation.TeamKill.LowercaseParams()
                    .Replace("{player}", ev.Player.Nickname)
                    .Replace("{attacker}", ev.Attacker.Nickname)
                    .Replace("{role}", ev.Player.Role.GetFullName())
                    .Replace("{playerid}", ev.Player.UserId)
                    .Replace("{attackerid}", ev.Attacker.UserId)
                    .PlayerReplace("player", ev.Player)
                    .PlayerReplace("attacker", ev.Attacker)
                    .StaticReplace()
            );
        }

        private void OnCuffKillDeath(PlayerDyingEventArgs ev)
        {
            if(ev.Attacker == null) return;
            if(ev.Attacker == ev.Player) return;
            if(ev.Attacker.IsSCP && Plugin.Instance.Config.ScpIgnoreCuffed) return;
            // is cuffed
            if(!ev.Player.IsDisarmed) return;
            if (Plugin.Instance.Config.CuffedChannelId == 0)
            {
                ev.Player.DisarmedBy = null;
                OnNormalDeath(ev);
                return;
            }
            SocketTextChannel? channel = DiscordBot.Instance.GetCuffedChannel();
            if (channel == null)
            {
                Logger.Error("Either the guild is null or the channel is null. So the death message has failed to send.");
                return;
            }

            channel.SendMessageAsync(
                Plugin.Instance.Translation.CuffedPlayerDeath.LowercaseParams()
                    .Replace("{player}", ev.Player.Nickname)
                    .Replace("{attacker}", ev.Attacker.Nickname)
                    .Replace("{playerrole}", ev.Player.Role.GetFullName())
                    .Replace("{attackerrole}", ev.Attacker.Role.GetFullName())
                    .Replace("{playerid}", ev.Player.UserId)
                    .Replace("{attackerid}", ev.Attacker.UserId)
                    .PlayerReplace("player", ev.Player)
                    .PlayerReplace("attacker", ev.Attacker)
                    .PlayerReplace("cuffer", ev.Player.DisarmedBy!)
                    .StaticReplace()
            );

        }

        private void OnNormalDeath(PlayerDyingEventArgs ev)
        {
            if(ev.Attacker == null) return;
            if(ev.Attacker == ev.Player) return;
            if(RoleExtensions.GetTeam(ev.Attacker.Role) == RoleExtensions.GetTeam(ev.Player.Role)) return;
            if(ev.Player.IsDisarmed) return;
            if (Plugin.Instance.Config.ChannelId == 0) return;
            SocketTextChannel? channel = DiscordBot.Instance.GetChannel();
            if (channel == null)
            {
                Logger.Error("Either the guild is null or the channel is null. So the death message has failed to send.");
                return;
            }
            channel.SendMessageAsync(
                Plugin.Instance.Translation.PlayerDeath.LowercaseParams()
                    .Replace("{player}", ev.Player.Nickname)
                    .Replace("{attacker}", ev.Attacker.Nickname)
                    .Replace("{playerrole}", ev.Player.Role.GetFullName())
                    .Replace("{attackerrole}", ev.Attacker.Role.GetFullName())
                    .Replace("{playerid}", ev.Player.UserId)
                    .Replace("{attackerid}", ev.Attacker.UserId)
                    .PlayerReplace("player", ev.Player)
                    .PlayerReplace("attacker", ev.Attacker)
                    .StaticReplace()
            );
        }

        private void OnSuicide(PlayerDyingEventArgs ev)
        {
            if (ev.Attacker != null && ev.Attacker != ev.Player) return;
            if (Plugin.Instance.Config.SelfChannelId == 0) return;
            SocketTextChannel? channel = DiscordBot.Instance.GetSelfChannel();
            if (channel == null)
            {
                Logger.Error("Either the guild is null or the channel is null. So the death message has failed to send.");
                return;
            }
            channel.SendMessageAsync(
                Plugin.Instance.Translation.PlayerDeathSelf.LowercaseParams()
                    .Replace("{player}", ev.Player.Nickname)
                    .Replace("{playerrole}", ev.Player.Role.GetFullName())
                    .Replace("{playerid}", ev.Player.UserId)
                    .PlayerReplace("player", ev.Player)
                    .StaticReplace()
            );
        }
    }
}