using Discord.WebSocket;
using DiscordLab.Bot.API.Interfaces;
using Exiled.API.Features;
using XPSystem.API;

namespace DiscordLab.XPSystem.Handlers;

public class Events : IRegisterable
{
    public void Init()
    {
        XPAPI.PlayerLevelUp += OnPlayerLevelUp;
    }
    
    public void Unregister()
    {
        XPAPI.PlayerLevelUp -= OnPlayerLevelUp;
    }
    
    private void OnPlayerLevelUp(XPPlayer player, int newLevel, int _)
    {
        SocketTextChannel channel = DiscordBot.Instance.GetChannel();
        if (channel == null)
        {
            Log.Info("Either the channel or guild could not be found. So the XPSystem level up message has failed to send.");
        }
        DiscordBot.Instance.GetChannel().SendMessageAsync(Plugin.Instance.Translation.LevelUp.Replace("{playername}", player.Nickname).Replace("{playerid}", player.UserId).Replace("{level}", newLevel.ToString()));
    }
}