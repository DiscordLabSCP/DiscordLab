using System.ComponentModel;
using Discord;

namespace DiscordLab.StatusChannel
{
    public class Translation
    {
        [Description("What will show when the server has players.")]
        public EmbedBuilder Embed { get; set; } = new()
        {
            Title = "Server Status",
            Color = Color.Blue.RawValue,
            Description = "{playercount}/{maxplayers} currently online\\n```{players}```"
        };

        [Description("What will show when the server is empty.")]
        public EmbedBuilder EmbedEmpty { get; set; } = new()
        {
            Title = "Server Status",
            Color = Color.Blue.RawValue,
            Description = "0/{maxplayers} currently online"
        };

        [Description("What will appear for each player when replacing the players variable above.")]
        public string PlayerItem { get; set; } = "- {player}";

        public string PlayerListCommandName { get; set; } = "players";

        public string PlayerListCommandDescription { get; set; } = "Get the current list of players on the server";
    }
}