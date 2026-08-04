using DiscordLab.Core;
using DiscordLab.Core.API.Commands;

namespace DiscordLab.Administration;

public class Translation
{
    public MessageContent ServerStart { get; set; } = "Server has started";

    public MessageContent ServerShutdown { get; set; } = "Server has shutdown";

    public MessageContent ErrorLog { get; set; } = "An error has occured, below is the log.";

    public MessageContent RemoteAdmin { get; set; } =
        "Player {player} has executed the remote admin command: `{command}`";

    public MessageContent CommandLog { get; set; } = "Player {player} has executed the command: `{command}`";
    
    public MessageContent RemoteAdminCommandFailResponse { get; set; } =
        "Player {player} has attempted to run a remote admin command which failed: `{command}`";

    public MessageContent CommandLogFailResponse { get; set; } = "Player {player} has attempted to run a command which failed: `{command}`";

    public CommandBuilder SendCommand = new()
    {
        Name = "send",
        Description = "Sends a command to the server",
        DefaultPermission = DefaultCommandPermissions.Admins,
        Options =
        [
            new()
            {
                Name = "command",
                Description = "The command to send",
                IsRequired = true,
                Type = CommandOptionType.String
            }
        ]
    };

    public MessageContent SendCommandResponse { get; set; } = "The command has been sent, it returned: {response}";
}