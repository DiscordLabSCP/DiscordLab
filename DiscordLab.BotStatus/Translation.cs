using Exiled.API.Interfaces;

namespace DiscordLab.BotStatus;

public class Translation : ITranslation
{
    public string StatusMessage { get; set; } = "{current}/{max} currently online";
    public string WaitingForPlayers { get; set; } = "Waiting for players...";
}