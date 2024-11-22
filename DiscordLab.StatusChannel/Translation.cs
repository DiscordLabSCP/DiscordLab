using System.ComponentModel;
using Exiled.API.Interfaces;

namespace DiscordLab.StatusChannel
{
    public class Translation : ITranslation
    {
        [Description("The text that shows in the embed title.")]
        public string EmbedTitle { get; set; } = "Server Status";

        [Description(
            "The text that shows in the embed description when the server has players online. players placeholder is the list of players using the player list translation.")]
        public string EmbedDescription { get; set; } = "{current}/{max} currently online\n```{players}```";

        [Description("The text that shows in the embed description when the server has no players online.")]
        public string EmbedNoPlayers { get; set; } = "0/{max} currently online";

        [Description("The text that shows for each player in the players list in embed description.")]
        public string PlayersList { get; set; } = "- {player} ({playerid})";
    }
}