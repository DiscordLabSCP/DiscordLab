using System.Globalization;
using CustomPlayerEffects;
using DiscordLab.Core;
using DiscordLab.Core.API.Attributes;
using DiscordLab.Core.API.Embed;
using DiscordLab.Core.API.Extensions;
using DiscordLab.Core.API.TranslationBuilders;
using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.Handlers;
using LabApi.Features.Wrappers;
using NorthwoodLib.Pools;
using PlayerStatsSystem;
using UnityEngine;
using Queue = DiscordLab.Core.API.Features.Queue;

namespace DiscordLab.DeathLogs;

public static class DamageLogs
{
    private static Config Config => Plugin.Instance.Config;

    private static Translation Translation => Plugin.Instance.Translation;
    
    public static List<string> DamageLogEntries { get; set; } = new();

    public static List<string> TeamDamageLogEntries { get; set; } = new();

    private static Queue Queue { get; } = new(5, SendLog);

    [CallOnLoad]
    public static void Register()
    {
        PlayerEvents.Hurt += OnHurt;
    }

    [CallOnUnload]
    public static void Unregister()
    {
        PlayerEvents.Hurt -= OnHurt;

        DamageLogEntries = null;
        TeamDamageLogEntries = null;
    }

    public static void OnHurt(PlayerHurtEventArgs ev)
    {
        if (Round.IsRoundEnded && Plugin.Instance.Config.IgnoreRoundEndDamage) return;
        if (ev.Attacker == null || ev.Player == ev.Attacker) return;

        if (ev.DamageHandler is not StandardDamageHandler handler)
            return;

        if (handler.Damage <= 0) return;

        string type = Events.ConvertToString(ev.DamageHandler);

        // passive damage checkers, don't want these spamming console.
        switch (type)
        {
            case "Cardiac Arrest":
            case "Unknown" when Mathf.Approximately(handler.Damage, 2.1f):
                return;
        }

        if (ev.Player.HasEffect<Corroding>() && type == "SCP-106")
            return;
        if (ev.Player.HasEffect<PocketCorroding>() && type == "SCP-106")
            return;
        if (type == "Strangled")
            return;

        if (ev.Player.IsSCP && ev.Attacker.IsSCP && Plugin.Instance.Config.IgnoreScpDamage)
            return;

        string log = new PlayerTranslationBuilder(Plugin.Instance.Translation.DamageLogEntry)
            .AddPlayer("target", ev.Player)
            .AddPlayer("player", ev.Attacker)
            .AddCustomReplacer("damage", handler.Damage.ToString(CultureInfo.InvariantCulture))
            .AddCustomReplacer("cause", type);

        if (ev.Player.Faction == ev.Attacker.Faction)
            TeamDamageLogEntries.Add(log);
        else
            DamageLogEntries.Add(log);

        Queue.Process();
    }

    public static void SendLog() => Task.RunAndLog(async () =>
    {
        List<string> damageLogEntries = DamageLogEntries.ToList();
        List<string> teamDamageLogEntries = TeamDamageLogEntries.ToList();
        
        TeamDamageLogEntries.Clear();
        DamageLogEntries.Clear();

        await MessageHandler.SendToWebhooks(Config.DamageLogChannel,
            new(null, CreateEmbeds(damageLogEntries, Translation.DamageLogEmbed)));

        await MessageHandler.SendToWebhooks(Config.TeamDamageLogChannel,
            new(null, CreateEmbeds(teamDamageLogEntries, Translation.TeamDamageLogEmbed)));
    });

    private static IEnumerable<EmbedBuilder> CreateEmbeds(List<string> entries, EmbedBuilder builder)
    {
        int count = entries.Count;

        if (count == 0)
            yield break;

        List<EmbedBuilder> embeds = ListPool<EmbedBuilder>.Shared.Rent();

        int currentIndex = 0;

        while (currentIndex < count)
        {
            EmbedBuilder embed = builder.Clone();

            List<string> currentEmbedLogs = ListPool<string>.Shared.Rent();
            int currentLength = 0;

            while (currentIndex < count)
            {
                string logEntry = entries[currentIndex];

                int newLength = currentLength + logEntry.Length + (currentEmbedLogs.Count > 0 ? 1 : 0);

                if (newLength > EmbedBuilder.MaxDescriptionLength && currentEmbedLogs.Count > 0)
                    break;

                if (logEntry.Length > EmbedBuilder.MaxDescriptionLength)
                {
                    logEntry = logEntry[..(EmbedBuilder.MaxDescriptionLength - 3)] + "...";
                    currentEmbedLogs.Add(logEntry);
                    currentIndex++;
                    break;
                }

                currentEmbedLogs.Add(logEntry);
                currentLength = newLength;
                currentIndex++;
            }

            if (currentEmbedLogs.Count <= 0) continue;
            TranslationBuilder translation =
                new TranslationBuilder().AddCustomReplacer("entries",
                    string.Join("\n", currentEmbedLogs));
            embeds.Add(embed.CloneWithTranslation(translation));
            ListPool<string>.Shared.Return(currentEmbedLogs);
        }

        foreach (EmbedBuilder embed in embeds)
        {
            yield return embed;
        }

        ListPool<EmbedBuilder>.Shared.Return(embeds);
    }
}