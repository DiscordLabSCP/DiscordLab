using System.Diagnostics.CodeAnalysis;
using CommandSystem;

namespace DiscordLab.Bot.Commands.Syncing;

[CommandHandler(typeof(ClientCommandHandler))]
public class GameCommand : ICommand
{
    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, [UnscopedRef] out string response)
    {
        if (arguments.Count != 1 || !LinkInstance.SaveIfExists(arguments[0]))
        {
            response = "Invalid code";
            return false;
        }

        response = "Connected to Discord successfully!";
        return true;
    }

    public string Command { get; } = "discordlink";
    public string[] Aliases { get; } = [];
    public string Description { get; } = "Link this SCP:SL account to your Discord with a code";
}