using System.ComponentModel;
using DiscordLab.Bot.API.Features;

namespace DiscordLab.ConnectionLogs;

public class Translation
{
    [Description("The message that will be sent when a player joins the server.")]
    public MessageContent PlayerJoin { get; set; } = "`{player}` (`{playerid}`) has joined the server.";

    [Description("The message that will be sent when a player leaves the server.")]
    public MessageContent PlayerLeave { get; set; } = "`{player}` (`{playerid}`) has left the server.";

    [Description(
        "The message that will be sent when the round starts, just before the player list. The players placeholder will be replaced with the list of players using the round start players translation.")]
    public MessageContent RoundStart { get; set; } = "Round has started with the following people: \n```{players}\n```";

    [Description(
        "The message that will be sent when the round ends, just before the player list. The players placeholder will be replaced with the list of players using the round start players translation.")]
    public MessageContent RoundEnd { get; set; } = "Round has ended with the following people: \n```{players}\n```";

    [Description("The message that indicates what a player looks like in the round start/end message.")]
    public string RoundPlayers { get; set; } = "{playername} ({playerid})";
}