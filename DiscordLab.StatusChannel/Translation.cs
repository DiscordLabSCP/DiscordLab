using System.ComponentModel;
using Discord;
using DiscordLab.Bot.API.Features;
using EmbedBuilder = DiscordLab.Bot.API.Features.Embed.EmbedBuilder;

namespace DiscordLab.StatusChannel;

public class Translation
{
    [Description("What will show when the server has players.")]
    public MessageContent Content { get; set; } = new EmbedBuilder
    {
        Title = "Server Status",
        Color = Color.Blue.ToString(),
        Description = "{playercount}/{maxplayers} currently online\n```{players}```"
    };

    [Description("What will show when the server is empty.")]
    public MessageContent EmptyContent { get; set; } = new EmbedBuilder
    {
        Title = "Server Status",
        Color = Color.Blue.ToString(),
        Description = "0/{maxplayers} currently online"
    };

    [Description("What will appear for each player when replacing the players variable above.")]
    public string PlayerItem { get; set; } = "- {player}";

    public string PlayerListCommandName { get; set; } = "players";

    public string PlayerListCommandDescription { get; set; } = "Get the current list of players on the server";
}