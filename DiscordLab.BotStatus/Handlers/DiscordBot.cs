using DiscordLab.Bot.API.Interfaces;
using Exiled.API.Features;

namespace DiscordLab.BotStatus.Handlers;

public class DiscordBot : IRegisterable
{
    private static Translation Translation => Plugin.Instance.Translation;
    
    public static DiscordBot Instance { get; private set; }
    
    public void Init()
    {
        Instance = this;
    }
    
    public void Unregister()
    {
        // Nothing to unregister here yippie!
    }

    public void SetStatus()
    {
        string status = Round.InProgress ? Translation.StatusMessage.Replace("{current}", Server.PlayerCount.ToString()).Replace("{max}", Server.MaxPlayerCount.ToString()) : Translation.WaitingForPlayers;
        if (Bot.Handlers.DiscordBot.Instance.Client.Activity?.ToString().Trim() == status) return;
        try
        {
            Bot.Handlers.DiscordBot.Instance.Client.SetCustomStatusAsync(status);
        }
        catch (Exception e)
        {
            Log.Error("Error setting status: " + e);
        }
    }
}