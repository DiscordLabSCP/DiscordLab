using System.Globalization;
using Discord;
using Discord.Rest;
using Discord.WebSocket;
using DiscordLab.Bot.API.Extensions;
using DiscordLab.Bot.API.Interfaces;
using Exiled.API.Features;
using Newtonsoft.Json.Linq;

namespace DiscordLab.StatusChannel.Handlers
{
    public class DiscordBot : IRegisterable
    {
        private static Translation Translation => Plugin.Instance.Translation;

        public static DiscordBot Instance { get; private set; }

        private SocketTextChannel StatusChannel { get; set; }

        public void Init()
        {
            Instance = this;
        }

        public void Unregister()
        {
            StatusChannel = null;
        }

        private SocketTextChannel GetChannel()
        {
            SocketGuild guild = Bot.Handlers.DiscordBot.Instance.GetGuild(Plugin.Instance.Config.GuildId);
            if (guild == null) return null;
            if (Plugin.Instance.Config.ChannelId == 0) return null;
            return StatusChannel ??=
                guild.GetTextChannel(Plugin.Instance.Config.ChannelId);
        }

        public void SetStatusMessage(IEnumerable<Player> players = null)
        {
            players ??= Player.List.Where(p => !p.IsNPC).ToList();
            IEnumerable<Player> playerList = players.ToList();
            string playersString = string.Join("\n",
                playerList.Select(p =>
                    Translation.PlayersList.LowercaseParams().Replace("{player}", p.Nickname).Replace("{playerid}", p.UserId).PlayerReplace("player", p).StaticReplace()));
            string fullDescription = !playerList.Any()
                ? Translation.EmbedNoPlayers.LowercaseParams()
                    .Replace("{max}", Server.MaxPlayerCount.ToString()).StaticReplace()
                : Translation.EmbedDescription.LowercaseParams()
                    .Replace("{players}", playersString)
                    .Replace("{current}", playerList.Count().ToString())
                    .Replace("{max}", Server.MaxPlayerCount.ToString())
                    .StaticReplace();
            EmbedBuilder embed = new EmbedBuilder()
                .WithTitle(Translation.EmbedTitle)
                .WithColor(uint.Parse(Plugin.Instance.Config.Color, NumberStyles.HexNumber))
                .WithDescription(fullDescription);
            JToken jId = Bot.API.Modules.WriteableConfig.GetConfig()["StatusChannelMessageId"];
            if (jId == null)
            {
                Task.Run(async () =>
                {
                    SocketTextChannel channel = GetChannel();
                    if (channel == null)
                    {
                        Log.Error(
                            "Either the guild is null or the channel is null. So the status message has failed to send.");
                        return;
                    }

                    RestUserMessage msg = await channel.SendMessageAsync(null, false, embed.Build());
                    Bot.API.Modules.WriteableConfig.WriteConfigOption("StatusChannelMessageId", msg.Id);
                });
            }
            else
            {
                SocketTextChannel channel = GetChannel();
                if (channel == null)
                {
                    Log.Error(
                        "Either the guild is null or the channel is null. So the status message has failed to be edited.");
                    return;
                }

                Task.Run(async () =>
                {
                    ulong id = jId.ToObject<ulong>();
                    IMessage oldMessage = channel.GetCachedMessage(id) ?? await channel.GetMessageAsync(id);
                    if (oldMessage == null ||
                        oldMessage.Author.Id != Bot.Handlers.DiscordBot.Instance.Client.CurrentUser.Id)
                    {
                        RestUserMessage msg = await channel.SendMessageAsync(null, false, embed.Build());
                        Bot.API.Modules.WriteableConfig.WriteConfigOption("StatusChannelMessageId", msg.Id);
                        return;
                    }

                    IUserMessage message = (IUserMessage)oldMessage;

                    await message.ModifyAsync(msg =>
                    {
                        msg.Content = null;
                        msg.Embed = embed.Build();
                    });
                });
            }
        }
    }
}