using Discord;
using Discord.Rest;
using Discord.WebSocket;
using DiscordLab.Bot.API.Interfaces;
using Exiled.API.Features;
using Newtonsoft.Json.Linq;

namespace DiscordLab.StatusChannel.Handlers;

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

    public SocketTextChannel GetChannel()
    {
        if(Bot.Handlers.DiscordBot.Instance.Guild == null) return null;
        return StatusChannel ??= Bot.Handlers.DiscordBot.Instance.Guild.GetTextChannel(Plugin.Instance.Config.ChannelId);
    }

    public void SetStatusMessage(IEnumerable<Player> players = null)
    {
        players ??= Player.List.ToList();
        string description = string.Join("\n", players.Select(player => "- " + player.Nickname));
        string prefix =
            $"{Plugin.Instance.Translation.EmbedStartDescription.Replace("{current}", players.Count().ToString()).Replace("{max}", Server.MaxPlayerCount.ToString())}\n";
        string fullDescription = prefix + description;
        EmbedBuilder embed = new EmbedBuilder()
            .WithTitle(Translation.EmbedTitle)
            .WithColor(Color.Blue)
            .WithDescription(fullDescription);
        JToken jId = Bot.API.Modules.WriteableConfig.GetConfig()["StatusChannelMessageId"];
        if (jId == null)
        {
            Task.Run(async () =>
            {
                SocketTextChannel channel = GetChannel();
                if (channel == null)
                {
                    Log.Error("Either the guild is null or the channel is null. So the status message has failed to send.");
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
                Log.Error("Either the guild is null or the channel is null. So the status message has failed to be edited.");
                return;
            }

            Task.Run(async () =>
            {
                ulong id = jId.ToObject<ulong>();
                IMessage oldMessage = channel.GetCachedMessage(id) ?? await channel.GetMessageAsync(id);
                if(oldMessage == null || oldMessage.Author.Id != Bot.Handlers.DiscordBot.Instance.Client.CurrentUser.Id)
                {
                    RestUserMessage msg = await channel.SendMessageAsync(null, false, embed.Build());
                    Bot.API.Modules.WriteableConfig.WriteConfigOption("StatusChannelMessageId", msg.Id);
                    return;
                }
                
                await channel.ModifyMessageAsync(id, msg =>
                {
                    msg.Content = null;
                    msg.Embed = embed.Build();
                });
            });
        }
    }
}