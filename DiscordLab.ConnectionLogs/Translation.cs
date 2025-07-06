using System.ComponentModel;

namespace DiscordLab.ConnectionLogs
{
    public class Translation
    {
        [Description("The message that will be sent when a player joins the server.")]
        public string PlayerJoin { get; set; } = "`{player}` (`{playerid}`) has joined the server.";

        [Description("The message that will be sent when a player leaves the server.")]
        public string PlayerLeave { get; set; } = "`{player}` (`{playerid}`) has left the server.";

        [Description("The message that will be sent when the round starts, just before the player list. The players placeholder will be replaced with the list of players using the round start players translation.")]
        public string RoundStart { get; set; } = "Round has started with the following people: \n```{players}\n```";
        
        [Description("The message that will be sent when the round ends, just before the player list. The players placeholder will be replaced with the list of players using the round start players translation.")]
        public string RoundEnd { get; set; } = "Round has ended with the following people: \n```{players}\n```";
        
        [Description("The message that indicates what a player looks like in the round start/end message.")]
        public string RoundPlayers { get; set; } = "{playername} ({playerid})";
    }
}