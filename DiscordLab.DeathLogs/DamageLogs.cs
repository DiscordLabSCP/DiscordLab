using System.Globalization;
using System.Net.Http;
using CustomPlayerEffects;
using Discord;
using Discord.Rest;
using Discord.Webhook;
using Discord.WebSocket;
using DiscordLab.Bot;
using DiscordLab.Bot.API.Attributes;
using DiscordLab.Bot.API.Extensions;
using DiscordLab.Bot.API.Features;
using DiscordLab.Bot.API.Utilities;
using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.Handlers;
using LabApi.Features.Wrappers;
using NorthwoodLib.Pools;
using PlayerStatsSystem;
using UnityEngine;
using Logger = LabApi.Features.Console.Logger;

namespace DiscordLab.DeathLogs;

public static class DamageLogs
{
    public static List<string> DamageLogEntries { get; set; } = new();

    public static List<string> TeamDamageLogEntries { get; set; } = new();

    public static RestWebhook Webhook;

    public static RestWebhook TeamWebhook;

    private static Queue queue = new(5, SendLog);

    [CallOnLoad]
    public static void Register()
    {
        if (Plugin.Instance.Config.DamageLogChannelId == 0) return;
        PlayerEvents.Hurt += OnHurt;
    }

    [CallOnUnload]
    public static void Unregister()
    {
        if (Plugin.Instance.Config.DamageLogChannelId == 0) return;
        PlayerEvents.Hurt -= OnHurt;

        DamageLogEntries = null;
        TeamDamageLogEntries = null;
        Webhook = null;
        TeamWebhook = null;
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

        string log = new TranslationBuilder(Plugin.Instance.Translation.DamageLogEntry)
            .AddPlayer("target", ev.Player)
            .AddPlayer("player", ev.Attacker)
            .AddCustomReplacer("damage", handler.Damage.ToString(CultureInfo.InvariantCulture))
            .AddCustomReplacer("cause", type);

        if (ev.Player.Faction == ev.Attacker.Faction)
            TeamDamageLogEntries.Add(log);
        else
            DamageLogEntries.Add(log);

        queue.Process();
    }

    public static void SendLog() => Task.RunAndLog(async () =>
    {
        ulong guildId = Plugin.Instance.Config.GuildId;
        ulong channelId = Plugin.Instance.Config.DamageLogChannelId;

        if (Webhook != null && Client.TryGetOrAddChannel(channelId, out SocketTextChannel channel))
            Webhook = await GetOrCreateWebhook(channel);

        if (Webhook != null)
        {
            DiscordWebhookClient client = new(Webhook);
            
            foreach (Embed embed in CreateEmbeds(DamageLogEntries, Plugin.Instance.Translation.DamageLogEmbed))
            {
                await client.SendMessageAsync(embeds: [embed]);
            }

            client.Dispose();
        }
        else if (channelId != 0 && Webhook == null)
            Logger.Error(
                LoggingUtils.GenerateMissingChannelMessage(
                    "damage logs",
                    channelId,
                    guildId));

        ulong teamChannelId = Plugin.Instance.Config.TeamDamageLogChannelId;
        if (TeamWebhook != null && Client.TryGetOrAddChannel(teamChannelId, out SocketTextChannel teamChannel))
            TeamWebhook = await GetOrCreateWebhook(teamChannel);

        if (TeamWebhook != null)
        {
            DiscordWebhookClient client = new(TeamWebhook);
            
            foreach (Embed embed in CreateEmbeds(TeamDamageLogEntries, Plugin.Instance.Translation.TeamDamageLogEmbed))
            {
                await client.SendMessageAsync(embeds: [embed]);
            }

            client.Dispose();
        }
        else if (teamChannelId != 0 && TeamWebhook == null)
            Logger.Error(
                LoggingUtils.GenerateMissingChannelMessage(
                    "team damage logs",
                    teamChannelId,
                    guildId));
    });

    private static IEnumerable<Embed> CreateEmbeds(List<string> entries, Bot.API.Features.Embed.EmbedBuilder builder)
    {
        int count = entries.Count;

        if (count == 0)
            yield break;

        List<Embed> embeds = ListPool<Embed>.Shared.Rent();

        int currentIndex = 0;

        while (currentIndex < count)
        {
            EmbedBuilder embed = builder;

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
            embed.Description =
                new TranslationBuilder(embed.Description).AddCustomReplacer("entries",
                    string.Join("\n", currentEmbedLogs));
            embeds.Add(embed.Build());
            ListPool<string>.Shared.Return(currentEmbedLogs);
        }

        foreach (Embed embed in embeds)
        {
            yield return embed;
        }

        ListPool<Embed>.Shared.Return(embeds);
    }

    private static async Task<RestWebhook> GetOrCreateWebhook(SocketTextChannel channel)
    {
        IReadOnlyCollection<RestWebhook> webhooks = await channel.GetWebhooksAsync();
        RestWebhook webhook = webhooks.FirstOrDefault(x => x.Creator.Id == Client.SocketClient.CurrentUser.Id);
        if (webhook != null) return webhook;

        using HttpClient client = new();
        Stream stream = await client.GetStreamAsync(Client.SocketClient.CurrentUser.GetAvatarUrl());
        webhook = await channel.CreateWebhookAsync(Client.SocketClient.CurrentUser.GlobalName, stream);
        return webhook;
    }
}