using System.Globalization;
using CustomPlayerEffects;
using Discord;
using Discord.WebSocket;
using DiscordLab.Bot;
using DiscordLab.Bot.API.Attributes;
using DiscordLab.Bot.API.Extensions;
using DiscordLab.Bot.API.Features;
using DiscordLab.Bot.API.Utilities;
using GameCore;
using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.Handlers;
using LabApi.Features.Console;
using PlayerStatsSystem;

namespace DiscordLab.DeathLogs
{
    public static class DamageLogs
    {
        public static List<string> DamageLogEntries { get; set; } = new();

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
        }

        public static void OnHurt(PlayerHurtEventArgs ev)
        {
            if (ev.Attacker == null || ev.Player == ev.Attacker) return;

            if (ev.DamageHandler is not StandardDamageHandler handler)
                return;
            
            if (handler.TotalDamageDealt <= 0) return;

            string type = Events.ConvertToString(ev.DamageHandler);
            
            // passive damage checkers, don't want these spamming console.
            if (type == "Cardiac Arrest") return;
            if (ev.Player.HasEffect<Corroding>() && type == "SCP-106") return;
            if (ev.Player.HasEffect<PocketCorroding>() && type == "SCP-106") return;
            if (type == "Strangled") return;
            
            if (ev.Player.IsSCP && ev.Attacker.IsSCP && Plugin.Instance.Config.IgnoreScpDamage) return;

            string log = new TranslationBuilder(Plugin.Instance.Translation.DamageLogEntry)
                .AddPlayer("target", ev.Player)
                .AddPlayer("player", ev.Attacker)
                .AddCustomReplacer("damage", handler.TotalDamageDealt.ToString(CultureInfo.InvariantCulture))
                .AddCustomReplacer("cause", type);
            
            DamageLogEntries.Add(log);
            
            queue.Process();
        }

        public static void SendLog()
        {
            if (!Client.TryGetOrAddChannel(Plugin.Instance.Config.DamageLogChannelId, out SocketTextChannel channel))
            {
                Logger.Error(
                    LoggingUtils.GenerateMissingChannelMessage(
                        "damage logs", 
                        Plugin.Instance.Config.DamageLogChannelId, 
                        Plugin.Instance.Config.GuildId));
                return;
            }
            
            channel.SendMessage(embeds:CreateEmbeds());
            
            DamageLogEntries.Clear();
        }

        public static Embed[] CreateEmbeds()
        {
            List<Embed> embeds = new();
    
            if (DamageLogEntries.Count == 0)
                return embeds.ToArray();
    
            int currentIndex = 0;
    
            while (currentIndex < DamageLogEntries.Count)
            {
                EmbedBuilder embed = Plugin.Instance.Translation.DamageLogEmbed;
        
                List<string> currentEmbedLogs = new();
                int currentLength = 0;
        
                while (currentIndex < DamageLogEntries.Count)
                {
                    string logEntry = DamageLogEntries[currentIndex];
            
                    int newLength = currentLength + logEntry.Length + (currentEmbedLogs.Count > 0 ? 1 : 0);
            
                    if (newLength > EmbedBuilder.MaxDescriptionLength && currentEmbedLogs.Count > 0)
                        break;
            
                    if (logEntry.Length > EmbedBuilder.MaxDescriptionLength)
                    {
                        logEntry = logEntry.Substring(0, EmbedBuilder.MaxDescriptionLength - 3) + "...";
                        currentEmbedLogs.Add(logEntry);
                        currentIndex++;
                        break;
                    }
            
                    currentEmbedLogs.Add(logEntry);
                    currentLength = newLength;
                    currentIndex++;
                }

                if (currentEmbedLogs.Count <= 0) continue;
                embed.Description = new TranslationBuilder(embed.Description).AddCustomReplacer("entries", string.Join("\n", currentEmbedLogs));
                embeds.Add(embed.Build());
            }
    
            return embeds.ToArray();
        }
    }
}