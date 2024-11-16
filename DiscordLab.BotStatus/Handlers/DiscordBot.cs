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

    public void SetStatus(int? count = null)
    {
        count ??= Server.PlayerCount;
        string status = Translation.StatusMessage.Replace("{current}", count.ToString())
            .Replace("{max}", Server.MaxPlayerCount.ToString());
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