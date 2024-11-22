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

        [Description("The message that will be sent when the round starts, just before the player list.")]
        public string RoundStart { get; set; } = "Round has started with the following people:";
    }
}