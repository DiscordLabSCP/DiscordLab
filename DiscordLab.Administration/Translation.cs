using System.ComponentModel;
using Discord;

namespace DiscordLab.Administration;

public class Translation
{
    public string ServerStart { get; set; } = "Server has started";

    public string ErrorLog { get; set; } = "An error has occured:\n{error}";

    public string RemoteAdmin { get; set; } = "Player {player} has executed the remote admin command: `{command}`";
        
    public string CommandLog { get; set; } = "Player {player} has executed the command: `{command}`";
        
    public string SendCommandName { get; set; } = "send";
        
    public string SendCommandDescription { get; set; } = "Sends a command to the server";
        
    public string SendCommandOptionName { get; set; } = "command";
        
    public string SendCommandOptionDescription { get; set; } = "The command to send";

    public string SendCommandResponse { get; set; } = "The command has been sent, it returned: {response}";
}