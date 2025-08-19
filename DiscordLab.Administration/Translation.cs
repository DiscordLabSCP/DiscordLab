using System.ComponentModel;
using Discord;
using DiscordLab.Bot.API.Features;

namespace DiscordLab.Administration;

public class Translation
{
    public MessageContent ServerStart { get; set; } = "Server has started";

    public MessageContent ErrorLog { get; set; } = "An error has occured:\n{error}";

    public MessageContent RemoteAdmin { get; set; } = "Player {player} has executed the remote admin command: `{command}`";
        
    public MessageContent CommandLog { get; set; } = "Player {player} has executed the command: `{command}`";
        
    public string SendCommandName { get; set; } = "send";
        
    public string SendCommandDescription { get; set; } = "Sends a command to the server";
        
    public string SendCommandOptionName { get; set; } = "command";
        
    public string SendCommandOptionDescription { get; set; } = "The command to send";

    public MessageContent SendCommandResponse { get; set; } = "The command has been sent, it returned: {response}";
}