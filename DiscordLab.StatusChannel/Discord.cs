using Discord;
using Discord.WebSocket;
using DiscordLab.Bot.Extensions;
using Exiled.API.Features;

namespace DiscordLab.StatusChannel;

public class Discord
{
    private SocketTextChannel Channel { get; set; }
    
    public void Init()
    {
        Bot.Discord.Client.Ready += Ready;
    }
    
    private Task Ready()
    {
        Channel = Bot.Discord.Guild.GetTextChannel(Plugin.Instance.Config.ChannelId);
        SetStatus(true);
        SetCustomStatus(true);
        return Task.CompletedTask;
    }

    public void SetStatus(bool force = false)
    {
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
            ulong id = Plugin.Instance.Config.MessageId;
            try
            {
                SocketUserMessage message;
                if (id != 0)
                {
                    IMessage msg = await Channel.GetMessageAsync(id);
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
                    Plugin.Instance.Config.MessageId = msg.Id;
                    return;
                }

                if (message == null)
                {
                    var msg = await Channel.SendMessageAsync(null, false, embedBuilder.Build());
                    Plugin.Instance.Config.MessageId = msg.Id;
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
            var clientActivity = Bot.Discord.Client.Activity;
            if (clientActivity != null && clientActivity.ToString().Trim() == status) return;
            Bot.Discord.Client.SetCustomStatusAsync(status);
        } catch (Exception e)
        {
            Log.Error($"Error while setting custom status: {e}");
        }
    }
}