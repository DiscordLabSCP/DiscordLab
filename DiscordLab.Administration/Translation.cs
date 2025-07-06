using System.ComponentModel;
using Discord;

namespace DiscordLab.Administration
{
    public class Translation
    {
        public string ServerStart { get; set; } = "Server has started";

        public string ErrorLog { get; set; } = "An error has occured:\n{error}";

        public string RemoteAdmin { get; set; } = "Player {player} has executed the remote admin command: `{command}`";
        
        public string CommandLog { get; set; } = "Player {player} has executed the command: `{command}`";
        
        public SlashCommandBuilder SendCommand { get; set; } = new()
        {
            Name = "send",
            Description = "Sends a command to the server",
            DefaultMemberPermissions = GuildPermission.Administrator,
            Options =
            [
                new()
                {
                    Name = "command",
                    Description = "The command to send"
                }
            ]
        };

        public string SendCommandResponse { get; set; } = "The command has been sent, it returned: {response}";
    }
}