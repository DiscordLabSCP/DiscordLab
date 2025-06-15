using System.Globalization;
using CustomPlayerEffects;
using Discord;
using Discord.WebSocket;
using DiscordLab.Bot.API.Enums;
using DiscordLab.Bot.API.Extensions;
using DiscordLab.Bot.API.Interfaces;
using DiscordLab.Bot.API.Modules;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using PlayerStatsSystem;

namespace DiscordLab.DeathLogs.Handlers
{
    public class DamageHandler : IRegisterable
    {
        public static List<string> DamageLogs { get; set; } = new();
        
        public void Init()
        {
            if (Plugin.Instance.Config.DamageLogChannelId == 0) return;
            
            Exiled.Events.Handlers.Player.Hurt += OnHurt;
        }

        public void Unregister()
        {
            if (Plugin.Instance.Config.DamageLogChannelId == 0) return;
            
            Exiled.Events.Handlers.Player.Hurt -= OnHurt;
        }

        public static void OnHurt(HurtEventArgs ev)
        {
            if (ev.Player == null || ev.Attacker == null || ev.Player == ev.Attacker) return;
            
            if (ev.Amount <= 0) return;
            
            // passive damage checkers, don't want these spamming console.
            if (ev.DamageHandler.Base is Scp049DamageHandler { DamageSubType: Scp049DamageHandler.AttackType.CardiacArrest }) return;
            if (ev.Player.IsEffectActive<Corroding>() && ev.DamageHandler.Type == DamageType.Scp106) return;
            if (ev.Player.IsEffectActive<PocketCorroding>() && ev.DamageHandler.Type == DamageType.Scp106) return;
            if (ev.DamageHandler.Type == DamageType.Strangled) return;
            
            if (ev.Player.IsScp && ev.Attacker.IsScp && Plugin.Instance.Config.IgnoreScpDamage) return;
            
            string log = Plugin.Instance.Translation.DamageLogEntry.LowercaseParams()
                .Replace("{cause}", Events.ConvertToString(ev.DamageHandler.Type))
                .Replace("{damage}", ev.Amount.ToString(CultureInfo.CurrentCulture))
                .StaticReplace()
                .PlayerReplace("attacker", ev.Attacker)
                .PlayerReplace("player", ev.Player);
            
            DamageLogs.Add(log);
            
            QueueSystem.QueueRun($"DiscordLab.DeathLogs.Handlers.DamageHandler", SendLog);
        }

        public static void SendLog()
        {
            ChannelReturn channelReturn = DiscordBot.Instance.GetGuild().TryGetTextChannel(Plugin.Instance.Config.DamageLogChannelId, out SocketTextChannel channel);
            if (channelReturn != ChannelReturn.Found)
            {
                switch (channelReturn)
                {
                    case ChannelReturn.InvalidChannel or ChannelReturn.NoChannel:
                    {
                        Log.Error("Could not find damage logs channel, try putting in the ID again.");
                        return;
                    }
                    case ChannelReturn.InvalidGuild or ChannelReturn.NoGuild:
                    {
                        Log.Error("Could not find damage logs channel because the guild you inputted is invalid, try putting in the ID for your guild again.");
                        return;
                    }
                    case ChannelReturn.InvalidType:
                    {
                        Log.Error("You input an invalid channel to damage logs, make sure you put in the correct ID.");
                        return;
                    }
                }

                return;
            }
            
            channel.SendMessageAsync(embeds:CreateEmbeds());
            
            DamageLogs.Clear();
        }

        public static Embed[] CreateEmbeds()
        {
            List<Embed> embeds = new();
    
            if (DamageLogs.Count == 0)
                return embeds.ToArray();
    
            int currentIndex = 0;
    
            while (currentIndex < DamageLogs.Count)
            {
                EmbedBuilder embed = new()
                {
                    Color = Plugin.Instance.Config.DamageLogEmbedColor.GetColor(),
                    Title = Plugin.Instance.Translation.DamageLogEmbedTitle
                };
        
                List<string> currentEmbedLogs = new();
                int currentLength = 0;
        
                while (currentIndex < DamageLogs.Count)
                {
                    string logEntry = DamageLogs[currentIndex];
            
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
                embed.Description = string.Join("\n", currentEmbedLogs);
                embeds.Add(embed.Build());
            }
    
            return embeds.ToArray();
        }
    }
}