using Discord;
using Discord.WebSocket;
using DiscordLab.Bot;
using DiscordLab.Bot.API.Attributes;
using DiscordLab.Bot.API.Features;
using DiscordLab.Bot.API.Utilities;
using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.CustomHandlers;
using LabApi.Features.Console;
using LabApi.Features.Wrappers;
using LabApi.Loader;

namespace DiscordLab.StatusChannel
{
    public class Events : CustomEventsHandler
    {
        // events

        public override void OnServerWaitingForPlayers() => Task.Run(GetOrCreateMessage);
        
        public override void OnPlayerJoined(PlayerJoinedEventArgs _) => Process();

        public override void OnPlayerLeft(PlayerLeftEventArgs _) => Process();

        public override void OnServerRoundStarted() => EditMessage();
        
        // static methods
        
        public static Config Config => Plugin.Instance.Config;

        public static Translation Translation => Plugin.Instance.Translation;
        
        public static SocketTextChannel Channel;

        public static IUserMessage Message;

        public static Queue Queue = new(5, EditMessage);

        [CallOnUnload]
        public static void Unregister()
        {
            Channel = null;
            Message = null;
            Queue = null;
        }
        
        public static void Process()
        {
            if(Round.IsRoundInProgress)
                EditMessage();
            else
                Queue.Process();
        }

        public static EmbedBuilder GetEmbed()
        {
            EmbedBuilder embed = !Player.ReadyList.Any() ? Translation.EmbedEmpty : Translation.Embed;
            TranslationBuilder builder = new(embed.Description);
            
            if (Player.ReadyList.Any())
            {
                builder.PlayerListItem = Translation.PlayerItem;
            }

            embed.Description = builder;

            return embed;
        }

        public static void EditMessage()
        {
            if (Channel == null && Message == null) return;
            if (Message == null)
            {
                Task.Run(async () =>
                {
                    await GetOrCreateMessage();
                    EditMessage();
                });
                return;
            }
            
            Task.Run(async () => await Message.ModifyAsync(x => x.Embed = GetEmbed().Build()).ConfigureAwait(false));
        }

        [CallOnReady]
        public static void Ready()
        {
            if (!Client.TryGetOrAddChannel(Config.ChannelId, out Channel))
            {
                Logger.Error(LoggingUtils.GenerateMissingChannelMessage("status channel", Config.ChannelId, Config.GuildId));
                Plugin.Instance.Disable();
                return;
            }
        }

        public static async Task GetOrCreateMessage()
        {
            ulong msgId = Plugin.Instance.MessageConfig.MessageId;
            Message = msgId != 0 ? Channel.GetCachedMessage(msgId) as IUserMessage ??
                      await Channel.GetMessageAsync(msgId) as IUserMessage : null;
            
            if (Message == null)
            {
                EmbedBuilder embed = Plugin.Instance.Translation.EmbedEmpty;
                try
                {
                    embed.Description = new TranslationBuilder(embed.Description).Build();
                }
                catch (Exception e)
                {
                    Logger.Error(e);
                    return;
                }

                Message = await Channel.SendMessageAsync(embed: embed.Build());

                Plugin.Instance.MessageConfig.MessageId = Message.Id;
                Plugin.Instance.SaveConfig(Plugin.Instance.MessageConfig, "message_config.yml");
            }
        }
    }
}