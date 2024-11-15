using Exiled.API.Interfaces;

namespace DiscordLab.StatusChannel;

public class Translation : ITranslation
{
    public string EmbedTitle { get; set; } = "Server Status";
    public string EmbedStartDescription { get; set; } = "{current}/{max} currently online";
    public string WaitingForPlayers { get; set; } = "Waiting for players...";
}