using Exiled.API.Interfaces;

namespace DiscordLab.StatusLog;

public class Translation : ITranslation
{
    public string PlayerJoin { get; set; } = "`{player}` (`{playerid}`) joined the server";
    public string PlayerLeave { get; set; } = "`{player}` (`{playerid}`) left the server";
    public string RoundStart { get; set; } = "Round has started with the following players:";
}