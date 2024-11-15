using Discord;
using Discord.WebSocket;
using DiscordLab.Bot.API.Extensions;
using DiscordLab.Bot.API.Interfaces;
using DiscordLab.Bot.API.Modules;
using Exiled.API.Features;
using Newtonsoft.Json.Linq;
using Color = Discord.Color;

namespace DiscordLab.StatusModule;

public class DiscordBot : IRegisterable
{
    private static SocketTextChannel Channel;
    
    public void Init()
    {
        Bot.DiscordBot.ReadyEvent += Ready;
    }
    
    public void Unregister()
    {
        Channel = null;
    }
    
    private void Ready()
    {
        if (Plugin.Instance.Config.ChannelId == 0)
        {
            Log.Error("Channel ID is not set.");
            return;
        }
        Channel = Bot.DiscordBot.Instance.Guild.GetTextChannel(Plugin.Instance.Config.ChannelId);
        SetStatus(true);
        SetCustomStatus(true);
    }

    public void SetStatus(bool force = false)
    {
        if (Channel == null) return;
        var playerList = Player.List;
        var players = string.Join("\n", playerList.Select(player => "- " + player.Nickname));
        var description = force ? players : !Round.IsEnded && Round.IsStarted ? players == "" ? Plugin.Instance.Translation.WaitingForPlayers : players : Plugin.Instance.Translation.WaitingForPlayers;
        var embedBuilder = new EmbedBuilder()
            .WithTitle(Plugin.Instance.Translation.EmbedTitle)
            .WithColor(Color.Blue)
            .WithDescription(
                $"{Plugin.Instance.Translation.EmbedStartDescription.Replace("{current}", Server.PlayerCount.ToString()).Replace("{max}", Server.MaxPlayerCount.ToString())}\n" +
                description
            );
        Task.Run(async () =>
        {
            JToken jId = WriteableConfig.GetConfig()["MessageId"];
            try
            {
                SocketUserMessage message;
                if (jId != null)
                {
                    var msg = await Channel.GetMessageAsync(jId.ToObject<ulong>());
                    if (!msg.IsUserMessage())
                    {
                        PluginAPI.Core.Log.Error("Message is not a user message.");
                        return;
                    }

                    message = msg as SocketUserMessage;
                }
                else
                {
                    var msg = await Channel.SendMessageAsync(null, false, embedBuilder.Build());
                    WriteableConfig.WriteConfigOption("MessageId", msg.Id);
                    return;
                }

                if (message == null)
                {
                    var msg = await Channel.SendMessageAsync(null, false, embedBuilder.Build());
                    WriteableConfig.WriteConfigOption("MessageId", msg.Id);
                    return;
                }

                await message.ModifyAsync(msg =>
                {
                    msg.Embed = embedBuilder.Build();
                    msg.Content = "";
                });
            } catch (Exception e)
            {
                Log.Error($"Error while setting status message: {e}");
            }
        });
    }

    public void SetCustomStatus(bool force = false)
    {
        var status = (!Round.IsEnded && Round.IsStarted) || force
            ? Plugin.Instance.Translation.BotStatus.Replace("{current}", Server.PlayerCount.ToString())
                .Replace("{max}", Server.MaxPlayerCount.ToString())
            : Plugin.Instance.Translation.WaitingForPlayers;
        try
        {
            var clientActivity = Bot.DiscordBot.Instance.Client.Activity;
            if (clientActivity != null && clientActivity.ToString().Trim() == status) return;
            Bot.DiscordBot.Instance.Client.SetCustomStatusAsync(status);
        } catch (Exception e)
        {
            Log.Error($"Error while setting custom status: {e}");
        }
    }
}