using System.ComponentModel;
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
using Exiled.API.Features.DamageHandlers;
using Exiled.Events.EventArgs.Player;
using PlayerStatsSystem;
using AttackerDamageHandler = PlayerStatsSystem.AttackerDamageHandler;
using ScpDamageHandler = PlayerStatsSystem.ScpDamageHandler;

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
            if (ev.DamageHandler.Base is Scp049DamageHandler { DamageSubType: Scp049DamageHandler.AttackType.CardiacArrest }) return;
            if (ev.Player.IsEffectActive<Corroding>() && ev.DamageHandler.Type == DamageType.Scp106) return;
            if (ev.Player.IsEffectActive<PocketCorroding>() && ev.DamageHandler.Type == DamageType.Scp106) return;
            if (ev.Player.IsScp && ev.Attacker.IsScp && Plugin.Instance.Config.IgnoreScpDamage) return;
            
            
            string log = Plugin.Instance.Translation.DamageLogEntry.LowercaseParams()
                .Replace("{cause}", Events.ConvertToString(ev.DamageHandler.Type))
                .Replace("{damage}", ev.Amount.ToString(CultureInfo.CurrentCulture))
                .StaticReplace()
                .PlayerReplace("attacker", ev.Attacker)
                .PlayerReplace("player", ev.Player);
            
            DamageLogs.Add(log);
            
            QueueSystem.QueueRun("DiscordLab.DeathLogs.Handlers.DamageHandler", SendLog);
        }

        public static void SendLog()
        {
            EmbedBuilder embed = new ();

            embed.Color = Plugin.Instance.Config.DamageLogEmbedColor.GetColor();
            
            embed.Title = Plugin.Instance.Translation.DamageLogEmbedTitle;
            
            embed.Description = string.Join("\n", DamageLogs);
            
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
            
            channel.SendMessageAsync(embed:embed.Build());
            
            DamageLogs.Clear();
        }
    }
}