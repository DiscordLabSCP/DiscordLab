using Discord.WebSocket;
using DiscordLab.Bot.API.Extensions;
using DiscordLab.Bot.API.Interfaces;
using Exiled.API.Enums;
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
            if (ev.Attacker == ev.Player) return;
            if (ev.Attacker.Role.Team != ev.Player.Role.Team) return;
            if(Plugin.Instance.Config.TeamKillChannelId == 0) return;
            SocketTextChannel channel = DiscordBot.Instance.GetTeamKillChannel();
            if (channel == null)
            {
                Log.Error("Either the guild is null or the channel is null. So the death message has failed to send.");
                return;
            }

            channel.SendMessageAsync(
                Plugin.Instance.Translation.TeamKill.LowercaseParams()
                    .Replace("{player}", ev.Player.Nickname)
                    .Replace("{attacker}", ev.Attacker.Nickname)
                    .Replace("{role}",ev.Player.Role.Name)
                    .Replace("{playerid}", ev.Player.UserId)
                    .Replace("{attackerid}", ev.Attacker.UserId)
                    .Replace("{cause}", ConvertToString(ev.DamageHandler.Type))
                    .PlayerReplace("player", ev.Player)
                    .PlayerReplace("attacker", ev.Attacker)
                    .StaticReplace()
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
                Plugin.Instance.Translation.CuffedPlayerDeath.LowercaseParams()
                    .Replace("{player}", ev.Player.Nickname)
                    .Replace("{attacker}", ev.Attacker.Nickname)
                    .Replace("{playerrole}", ev.Player.Role.Name)
                    .Replace("{attackerrole}", ev.Attacker.Role.Name)
                    .Replace("{playerid}", ev.Player.UserId)
                    .Replace("{attackerid}", ev.Attacker.UserId)
                    .Replace("{cause}", ConvertToString(ev.DamageHandler.Type))
                    .PlayerReplace("player", ev.Player)
                    .PlayerReplace("attacker", ev.Attacker)
                    .PlayerReplace("cuffer", ev.Player.Cuffer)
                    .StaticReplace()
            );

        }

        private void OnNormalDeath(DyingEventArgs ev)
        {
            if(ev.Attacker == null) return;
            if(ev.Attacker == ev.Player) return;
            if(ev.Attacker.Role.Team == ev.Player.Role.Team) return;
            if(ev.Player.IsCuffed) return;
            if (Plugin.Instance.Config.ChannelId == 0) return;
            SocketTextChannel channel = DiscordBot.Instance.GetChannel();
            if (channel == null)
            {
                Log.Error("Either the guild is null or the channel is null. So the death message has failed to send.");
                return;
            }
            channel.SendMessageAsync(
                Plugin.Instance.Translation.PlayerDeath.LowercaseParams()
                    .Replace("{player}", ev.Player.Nickname)
                    .Replace("{attacker}", ev.Attacker.Nickname)
                    .Replace("{playerrole}", ev.Player.Role.Name)
                    .Replace("{attackerrole}", ev.Attacker.Role.Name)
                    .Replace("{playerid}", ev.Player.UserId)
                    .Replace("{attackerid}", ev.Attacker.UserId)
                    .Replace("{cause}", ConvertToString(ev.DamageHandler.Type))
                    .PlayerReplace("player", ev.Player)
                    .PlayerReplace("attacker", ev.Attacker)
                    .StaticReplace()
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
                Plugin.Instance.Translation.PlayerDeathSelf.LowercaseParams()
                    .Replace("{player}", ev.Player.Nickname)
                    .Replace("{playerrole}", ev.Player.Role.Name)
                    .Replace("{playerid}", ev.Player.UserId)
                    .Replace("{cause}", ConvertToString(ev.DamageHandler.Type))
                    .PlayerReplace("player", ev.Player)
                    .StaticReplace()
            );
        }

        internal static string ConvertToString(DamageType type) => type switch
        {
            DamageType.A7 => "A7",
            DamageType.Unknown => "Unknown",
            DamageType.Falldown => "Fell down",
            DamageType.Warhead => "Warhead explosion",
            DamageType.Decontamination => "Decontamination",
            DamageType.Asphyxiation => "Asphyxiation",
            DamageType.Poison => "Poison",
            DamageType.Bleeding => "Bleeding",
            DamageType.Firearm => "Unknown Firearm",
            DamageType.MicroHid => "Micro HID",
            DamageType.Tesla => "Tesla Gate",
            DamageType.Scp => "Unknown SCP",
            DamageType.Explosion => "Explosion",
            DamageType.Scp018 => "SCP-018",
            DamageType.Scp207 => "SCP-207",
            DamageType.Recontainment => "Recontainment",
            DamageType.Crushed => "Crushed",
            DamageType.FemurBreaker => "Femur Breaker",
            DamageType.PocketDimension => "Pocket Dimension",
            DamageType.FriendlyFireDetector => "Friendly Fire",
            DamageType.SeveredHands => "Severed Hands",
            DamageType.SeveredEyes => "Severed Eyes",
            DamageType.Custom => "Unknown, plugin specific death.",
            DamageType.Scp049 => "SCP-049",
            DamageType.Scp096 => "SCP-096",
            DamageType.Scp173 => "SCP-173",
            DamageType.Scp939 => "SCP-939",
            DamageType.Scp0492 => "SCP-049-2",
            DamageType.Scp106 => "SCP-106",
            DamageType.Crossvec => "Cross-vec",
            DamageType.Logicer => "Logicer",
            DamageType.Revolver => "Revolver",
            DamageType.Shotgun => "Shotgun",
            DamageType.AK => "AK",
            DamageType.Com15 => "COM-15",
            DamageType.Com18 => "COM-18",
            DamageType.Fsp9 => "FSP-9",
            DamageType.E11Sr => "E11-SR",
            DamageType.Hypothermia => "Hypothermia",
            DamageType.ParticleDisruptor => "Particle Disruptor",
            DamageType.CardiacArrest => "Cardiac Arrest",
            DamageType.Com45 => "COM-45",
            DamageType.Jailbird => "Jailbird",
            DamageType.Frmg0 => "FR-MG-0",
            DamageType.Scp3114 => "SCP-3114",
            DamageType.Strangled => "Strangled",
            DamageType.Marshmallow => "Marshmallow",
            _ => type.ToString()
        };
    }
}