using Discord;
using Discord.WebSocket;
using DiscordLab.Bot;
using DiscordLab.Bot.API.Extensions;
using DiscordLab.Bot.API.Features;
using DiscordLab.Bot.API.Utilities;
using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.Arguments.ServerEvents;
using LabApi.Events.CustomHandlers;
using LabApi.Features.Console;
using LabApi.Features.Wrappers;

namespace DiscordLab.Moderation
{
    public class Events : CustomEventsHandler
    {
        public static Config Config => Plugin.Instance.Config;

        public static Translation Translation => Plugin.Instance.Translation;
        
        public override void OnPlayerUnmuting(PlayerUnmutingEventArgs ev)
        {
            // otherwise OnPlayerUnmuted will get triggered twice.
            ev.IsAllowed = false;
            
            TempMuteManager.RemoveMute(ev.Player, ev.Issuer);
        }

        public override void OnPlayerUnmuted(PlayerUnmutedEventArgs ev)
        {
            if (Config.UnmuteLogChannelId == 0)
                return;

            if (!Client.TryGetOrAddChannel(Config.UnmuteLogChannelId, out SocketTextChannel channel))
            {
                Logger.Error(LoggingUtils.GenerateMissingChannelMessage("unmute logs", Config.UnmuteLogChannelId, Config.GuildId));
                return;
            }

            TranslationBuilder builder = new TranslationBuilder(Translation.UnmuteLog)
                .AddPlayer("target", ev.Player)
                .AddPlayer("player", ev.Issuer);
            
            channel.SendMessage(builder);
        }

        public override void OnPlayerMuted(PlayerMutedEventArgs ev)
        {
            if (Config.MuteLogChannelId == 0)
                return;

            if (!Client.TryGetOrAddChannel(Config.MuteLogChannelId, out SocketTextChannel channel))
            {
                Logger.Error(LoggingUtils.GenerateMissingChannelMessage("mute logs", Config.MuteLogChannelId, Config.GuildId));
                return;
            }
            
            string translation = Translation.PermMuteLog;

            if (TempMuteManager.MuteConfig.Mutes.TryGetValue(ev.Player.UserId, out DateTime time))
            {
                translation = Translation.TempMuteLog;
            }
            
            TranslationBuilder builder = new TranslationBuilder(translation)
            {
                Time = time
            }.AddPlayer("player", ev.Issuer).AddPlayer("target", ev.Player);
            
            channel.SendMessage(builder);
        }

        public override void OnPlayerBanned(PlayerBannedEventArgs ev)
        {
            if (Config.BanLogChannelId == 0)
                return;

            if (!Client.TryGetOrAddChannel(Config.BanLogChannelId, out SocketTextChannel channel))
            {
                Logger.Error(LoggingUtils.GenerateMissingChannelMessage("ban logs", Config.BanLogChannelId, Config.GuildId));
                return;
            }

            EmbedBuilder builder = Translation.BanLogEmbed;
            
            foreach (EmbedFieldBuilder field in builder.Fields)
            {
                TranslationBuilder tBuilder = new((string)field.Value, "player", ev.Issuer);
                tBuilder.CustomReplacers.Add("userid", () => ev.PlayerId);
                field.Value = tBuilder.Build();
            }
            
            channel.SendMessage(embed:builder.Build());
        }
        
        public override void OnServerBanRevoked(BanRevokedEventArgs ev)
        {
            if (Config.UnbanLogChannelId == 0)
                return;

            if (!Client.TryGetOrAddChannel(Config.UnbanLogChannelId, out SocketTextChannel channel))
            {
                Logger.Error(LoggingUtils.GenerateMissingChannelMessage("unban logs", Config.UnbanLogChannelId, Config.GuildId));
                return;
            }

            TranslationBuilder builder = new(Translation.UnbanLog);
            
            builder.CustomReplacers.Add("userid", () => ev.BanDetails.Id);
            builder.CustomReplacers.Add("username", () => ev.BanDetails.OriginalName);
            builder.CustomReplacers.Add("playerid", () => ev.BanDetails.Issuer);

            if (Player.TryGet(ev.BanDetails.Issuer, out Player player))
                builder.AddPlayer("player", player);
            
            channel.SendMessage(builder);
        }
    }
}