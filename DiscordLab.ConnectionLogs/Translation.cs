using System.ComponentModel;
using Exiled.API.Interfaces;

namespace DiscordLab.ConnectionLogs
{
    public class Translation : ITranslation
    {
        [Description("The message that will be sent when a player joins the server.")]
        public string PlayerJoin { get; set; } = "`{player}` (`{id}`) has joined the server.";

        [Description("The message that will be sent when a player leaves the server.")]
        public string PlayerLeave { get; set; } = "`{player}` (`{id}`) has left the server.";

        [Description("The message that will be sent when the round starts, just before the player list. The players placeholder will be replaced with the list of players using the round start players translation.")]
        public string RoundStart { get; set; } = "Round has started with the following people: \n```{players}\n```";
        
        [Description("The message that indicates what a player looks like in the round start message. Extra placeholder is ip, but only use that if the channel is private or risk being delisted.")]
        public string RoundStartPlayers { get; set; } = "{playername} ({playerid})";
    }
}