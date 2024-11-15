using Exiled.API.Interfaces;

namespace DiscordLab.ConnectionLogs;

public class Translation : ITranslation
{
    public string PlayerJoin { get; set; } = "`{player}` (`{id}`) has joined the server.";
    public string PlayerLeave { get; set; } = "`{player}` (`{id}`) has left the server.";
    public string RoundStart { get; set; } = "Round has started with the following people:";
}