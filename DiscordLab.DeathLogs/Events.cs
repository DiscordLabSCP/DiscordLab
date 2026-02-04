using Discord.WebSocket;
using DiscordLab.Bot;
using DiscordLab.Bot.API.Attributes;
using DiscordLab.Bot.API.Features;
using DiscordLab.Bot.API.Utilities;
using InventorySystem.Items.Scp1509;
using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.Handlers;
using LabApi.Features.Console;
using PlayerRoles;
using PlayerRoles.PlayableScps.Scp1507;
using PlayerRoles.PlayableScps.Scp3114;
using PlayerRoles.PlayableScps.Scp939;
using PlayerStatsSystem;

namespace DiscordLab.DeathLogs;

public static class Events
{
    public static Config Config => Plugin.Instance.Config;

    public static Translation Translation => Plugin.Instance.Translation;

    // have to do this here over CustomEventsHandler because easier to maintain different logs in this case.
    [CallOnLoad]
    public static void Register()
    {
        PlayerEvents.Dying += OnTeamKill;
        PlayerEvents.Dying += OnCuffKill;
        PlayerEvents.Dying += OnDeath;
        PlayerEvents.Dying += OnOwnDeath;
    }

    [CallOnUnload]
    public static void Unregister()
    {
        PlayerEvents.Dying -= OnTeamKill;
        PlayerEvents.Dying -= OnCuffKill;
        PlayerEvents.Dying -= OnDeath;
        PlayerEvents.Dying -= OnOwnDeath;
    }

    public static void OnTeamKill(PlayerDyingEventArgs ev)
    {
        if (ev.Attacker == null || ev.Attacker.Team.GetFaction() != ev.Player.Team.GetFaction())
            return;

        if (Config.TeamKillChannelId == 0)
            return;

        if (!Client.TryGetOrAddChannel(Config.TeamKillChannelId, out SocketTextChannel channel))
        {
            Logger.Error(LoggingUtils.GenerateMissingChannelMessage("team kill logs", Config.TeamKillChannelId,
                Config.GuildId));

            return;
        }

        TranslationBuilder builder = new TranslationBuilder()
            .AddPlayer("target", ev.Player)
            .AddPlayer("player", ev.Attacker)
            .AddCustomReplacer("cause", ConvertToString(ev.DamageHandler))
            .AddCustomReplacer("role", ev.Player.Team.GetFaction().ToString());

        Translation.TeamKill.SendToChannel(channel, builder);
    }

    public static void OnCuffKill(PlayerDyingEventArgs ev)
    {
        if (ev.Attacker == null || !ev.Player.IsDisarmed || (ev.Attacker.IsSCP && Config.ScpIgnoreCuffed))
            return;

        if (Config.CuffedChannelId == 0)
            return;

        if (!Client.TryGetOrAddChannel(Config.CuffedChannelId, out SocketTextChannel channel))
        {
            Logger.Error(LoggingUtils.GenerateMissingChannelMessage("cuff kill logs", Config.CuffedChannelId,
                Config.GuildId));

            return;
        }

        TranslationBuilder builder = new TranslationBuilder()
            .AddPlayer("target", ev.Player)
            .AddPlayer("player", ev.Attacker)
            .AddCustomReplacer("cause", ConvertToString(ev.DamageHandler));

        Translation.CuffedPlayerDeath.SendToChannel(channel, builder);
    }

    public static void OnDeath(PlayerDyingEventArgs ev)
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

        TranslationBuilder builder = new TranslationBuilder()
            .AddPlayer("target", ev.Player)
            .AddPlayer("player", ev.Attacker)
            .AddCustomReplacer("cause", ConvertToString(ev.DamageHandler));

        Translation.PlayerDeath.SendToChannel(channel, builder);
    }

    public static void OnOwnDeath(PlayerDyingEventArgs ev)
    {
        if (ev.Attacker != null)
            return;

        if (Config.SelfChannelId == 0)
            return;

        if (!Client.TryGetOrAddChannel(Config.SelfChannelId, out SocketTextChannel channel))
        {
            Logger.Error(
                LoggingUtils.GenerateMissingChannelMessage("self kill logs", Config.SelfChannelId, Config.GuildId));

            return;
        }

        string converted = ConvertToString(ev.DamageHandler);

        // usually because of disconnect, only way to really track rn
        if (converted == "Unknown")
            return;

        TranslationBuilder builder = new TranslationBuilder()
            .AddPlayer("player", ev.Player)
            .AddCustomReplacer("cause", converted);

        Translation.PlayerDeathSelf.SendToChannel(channel, builder);
    }

    private static readonly Dictionary<byte, string> Translations = new()
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
        { DeathTranslations.Scp127Bullets.Id, "SCP-127" }
    };

    internal static string ConvertToString(DamageHandlerBase handler)
    {
        switch (handler)
        {
            case CustomReasonDamageHandler:
            case CustomReasonFirearmDamageHandler:
                return "Unknown, plugin specific death.";
            case GrayCandyDamageHandler:
                return "Metal Man";
            case Scp096DamageHandler:
                return "SCP-096";
            case Scp1509DamageHandler:
                return "SCP-1509";
            case SilentDamageHandler:
                return "Silent";
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
            case ScpDamageHandler scpDamageHandler:
            {
                return FromTranslationId(scpDamageHandler._translationId);
            }
            case UniversalDamageHandler universal:
            {
                return FromTranslationId(universal.TranslationId);
            }
            case FirearmDamageHandler firearm:
                return firearm.Firearm.Name;
        }

        return "Unknown";
    }

    private static string FromTranslationId(byte id)
    {
        DeathTranslation translation = DeathTranslations.TranslationsById[id];

        return Translations.GetValueOrDefault(translation.Id, "Unknown");
    }
}