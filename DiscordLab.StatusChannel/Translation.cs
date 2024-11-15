using System.ComponentModel;
using Exiled.API.Interfaces;

namespace DiscordLab.StatusChannel;

public class Translation : ITranslation
{
    [Description("The text that shows in the embed title.")]
    public string EmbedTitle { get; set; } = "Server Status";
    [Description("The text that shows in the embed description when the server is online, just before the player list.")]
    public string EmbedStartDescription { get; set; } = "{current}/{max} currently online";
    [Description("The text that shows in the embed description when the server is offline or waiting for players.")]
    public string WaitingForPlayers { get; set; } = "Waiting for players...";
}