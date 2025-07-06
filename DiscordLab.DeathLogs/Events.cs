using Discord.WebSocket;
using DiscordLab.Bot;
using DiscordLab.Bot.API.Attributes;
using DiscordLab.Bot.API.Extensions;
using DiscordLab.Bot.API.Features;
using DiscordLab.Bot.API.Utilities;
using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.Handlers;
using LabApi.Features.Console;
using PlayerRoles;
using PlayerRoles.PlayableScps.Scp1507;
using PlayerRoles.PlayableScps.Scp3114;
using PlayerRoles.PlayableScps.Scp939;
using PlayerStatsSystem;

namespace DiscordLab.DeathLogs
{
    public static class Events
    {
        public static Config Config => Plugin.Instance.Config;

        public static Translation Translation => Plugin.Instance.Translation;
        
        // have to do this here over CustomEventsHandler because easier to maintain different logs in this case.
        [CallOnLoad]
        public static void Register()
        {
            PlayerEvents.Death += OnTeamKill;
            PlayerEvents.Death += OnCuffKill;
            PlayerEvents.Death += OnDeath;
            PlayerEvents.Death += OnOwnDeath;
        }

        [CallOnUnload]
        public static void Unregister()
        {
            PlayerEvents.Death -= OnTeamKill;
            PlayerEvents.Death -= OnCuffKill;
            PlayerEvents.Death -= OnDeath;
            PlayerEvents.Death -= OnOwnDeath;
        }

        public static void OnTeamKill(PlayerDeathEventArgs ev)
        {
            if (ev.Attacker == null || ev.Attacker.Team.GetFaction() != ev.Player.Team.GetFaction())
                return;

            if (Config.TeamKillChannelId == 0)
                return;

            if (!Client.TryGetOrAddChannel(Config.TeamKillChannelId, out SocketTextChannel channel))
            {
                Logger.Error(LoggingUtils.GenerateMissingChannelMessage("team kill logs", Config.TeamKillChannelId, Config.GuildId));

                return;
            }

            TranslationBuilder builder = new TranslationBuilder(Translation.TeamKill)
                .AddPlayer("target", ev.Player)
                .AddPlayer("player", ev.Attacker)
                .AddCustomReplacer("cause", ConvertToString(ev.DamageHandler))
                .AddCustomReplacer("role", ev.Player.Team.GetFaction().ToString());
            
            channel.SendMessage(builder);
        }

        public static void OnCuffKill(PlayerDeathEventArgs ev)
        {
            if (ev.Attacker == null || !ev.Player.IsDisarmed || (ev.Attacker.IsSCP && Config.ScpIgnoreCuffed))
                return;

            if (Config.CuffedChannelId == 0)
                return;

            if (!Client.TryGetOrAddChannel(Config.CuffedChannelId, out SocketTextChannel channel))
            {
                Logger.Error(LoggingUtils.GenerateMissingChannelMessage("cuff kill logs", Config.CuffedChannelId, Config.GuildId));

                return;
            }

            TranslationBuilder builder = new TranslationBuilder(Translation.CuffedPlayerDeath)
                .AddPlayer("target", ev.Player)
                .AddPlayer("player", ev.Attacker)
                .AddCustomReplacer("cause", ConvertToString(ev.DamageHandler));
            
            channel.SendMessage(builder);
        }

        public static void OnDeath(PlayerDeathEventArgs ev)
        {
            if (ev.Attacker == null || ev.Player.IsDisarmed ||
                ev.Attacker.Team.GetFaction() == ev.Player.Team.GetFaction())
                return;
            
            if (Config.ChannelId == 0)
                return;

            if (!Client.TryGetOrAddChannel(Config.ChannelId, out SocketTextChannel channel))
            {
                Logger.Error(LoggingUtils.GenerateMissingChannelMessage("kill logs", Config.ChannelId, Config.GuildId));

                return;
            }

            TranslationBuilder builder = new TranslationBuilder(Translation.PlayerDeath)
                .AddPlayer("target", ev.Player)
                .AddPlayer("player", ev.Attacker)
                .AddCustomReplacer("cause", ConvertToString(ev.DamageHandler));
            
            channel.SendMessage(builder);
        }

        public static void OnOwnDeath(PlayerDeathEventArgs ev)
        {
            if (ev.Attacker != null)
                return;
            
            if (Config.SelfChannelId == 0)
                return;

            if (!Client.TryGetOrAddChannel(Config.SelfChannelId, out SocketTextChannel channel))
            {
                Logger.Error(LoggingUtils.GenerateMissingChannelMessage("self kill logs", Config.SelfChannelId, Config.GuildId));

                return;
            }

            TranslationBuilder builder = new TranslationBuilder(Translation.PlayerDeathSelf)
                .AddPlayer("player", ev.Player)
                .AddCustomReplacer("cause", ConvertToString(ev.DamageHandler));
            
            channel.SendMessage(builder);
        }

        internal static string ConvertToString(DamageHandlerBase handler)
        {
            Dictionary<byte, string> translations = new()
                {
                    { DeathTranslations.Asphyxiated.Id, "Asphyxiation" },
                    { DeathTranslations.Bleeding.Id, "Bleeding" },
                    { DeathTranslations.Crushed.Id, "Crushed" },
                    { DeathTranslations.Decontamination.Id, "Decontamination" },
                    { DeathTranslations.Explosion.Id, "Explosion" },
                    { DeathTranslations.Falldown.Id, "Falldown" },
                    { DeathTranslations.Poisoned.Id, "Poison" },
                    { DeathTranslations.Recontained.Id, "Recontainment" },
                    { DeathTranslations.Scp049.Id, "SCP-049" },
                    { DeathTranslations.Scp096.Id, "SCP-096" },
                    { DeathTranslations.Scp173.Id, "SCP-173" },
                    { DeathTranslations.Scp207.Id, "SCP-207" },
                    { DeathTranslations.Scp939Lunge.Id, "SCP-939 Lunge" },
                    { DeathTranslations.Scp939Other.Id, "SCP-939" },
                    { DeathTranslations.Scp3114Slap.Id, "SCP-3114" },
                    { DeathTranslations.Tesla.Id, "Tesla" },
                    { DeathTranslations.Unknown.Id, "Unknown" },
                    { DeathTranslations.Warhead.Id, "Warhead" },
                    { DeathTranslations.Zombie.Id, "SCP-049-2" },
                    { DeathTranslations.BulletWounds.Id, "Firearm" },
                    { DeathTranslations.PocketDecay.Id, "Pocket Decay" },
                    { DeathTranslations.SeveredHands.Id, "Severed Hands" },
                    { DeathTranslations.FriendlyFireDetector.Id, "Friendly Fire" },
                    { DeathTranslations.UsedAs106Bait.Id, "Femur Breaker" },
                    { DeathTranslations.MicroHID.Id, "Micro H.I.D." },
                    { DeathTranslations.Hypothermia.Id, "Hypothermia" },
                    { DeathTranslations.MarshmallowMan.Id, "Marshmellow" },
                    { DeathTranslations.Scp1344.Id, "Severed Eyes" },
                };
            
            switch (handler)
            {
                case CustomReasonDamageHandler:
                    return "Unknown, plugin specific death.";
                case WarheadDamageHandler:
                    return "Warhead";
                case ExplosionDamageHandler:
                    return "Explosion";
                case Scp018DamageHandler:
                    return "SCP-018";
                case RecontainmentDamageHandler:
                    return "Recontainment";
                case MicroHidDamageHandler:
                    return "Micro H.I.D.";
                case DisruptorDamageHandler:
                    return "Particle Disruptor";
                case Scp939DamageHandler:
                    return "SCP-939";
                case JailbirdDamageHandler:
                    return "Jailbird";
                case Scp1507DamageHandler:
                    return "SCP-1507";
                case Scp956DamageHandler:
                    return "SCP-956";
                case SnowballDamageHandler:
                    return "Snowball";
                case Scp3114DamageHandler scp3114DamageHandler:
                    return scp3114DamageHandler.Subtype switch
                    {
                        Scp3114DamageHandler.HandlerType.Strangulation => "Strangled",
                        Scp3114DamageHandler.HandlerType.SkinSteal => "SCP-3114",
                        Scp3114DamageHandler.HandlerType.Slap => "SCP-3114",
                        _ => "Unknown",
                    };
                case Scp049DamageHandler scp049DamageHandler:
                    return scp049DamageHandler.DamageSubType switch
                    {
                        Scp049DamageHandler.AttackType.CardiacArrest => "Cardiac Arrest",
                        Scp049DamageHandler.AttackType.Instakill => "SCP-049",
                        Scp049DamageHandler.AttackType.Scp0492 => "SCP-049-2",
                        _ => "Unknown",
                    };
                case UniversalDamageHandler universal:
                    {
                        DeathTranslation translation = DeathTranslations.TranslationsById[universal.TranslationId];

                        if (translations.TryGetValue(translation.Id, out string s))
                            return s;
                        
                        break;
                    }
            }

            return "Unknown";
        }
    }
}