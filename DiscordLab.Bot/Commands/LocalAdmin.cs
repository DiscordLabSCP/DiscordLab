using CommandSystem;
using DiscordLab.Bot.API.Modules;
using PluginAPI.Core;

namespace DiscordLab.Bot.Commands
{
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public class LocalAdmin : ICommand
    {
        public string Command { get; } = "discordlab";

        public string[] Aliases { get; } = new [] { "dl", "lab" };

        public string Description { get; } = "Do things directly with DiscordLab.";

        public bool Execute(
            ArraySegment<string> arguments,
            ICommandSender sender,
            out string response
        )
        {
            switch (arguments.FirstOrDefault())
            {
                case "list":
                    if (UpdateStatus.Statuses == null)
                    {
                        response = "No modules available. Please wait for your server to fully start.";
                        return false;
                    }
                    string modules = string.Join("\n", UpdateStatus.Statuses.Where(s => s.ModuleName != "DiscordLab.Bot").Select(s => s.ModuleName));
                    response =
                        $"Available modules:\n{modules}";
                    return true;
                case "install":
                    if (UpdateStatus.Statuses == null)
                    {
                        response = "No modules available. Please wait for your server to fully start.";
                        return false;
                    }
                    string module = arguments.ElementAtOrDefault(1);
                    if(string.IsNullOrWhiteSpace(module))
                    {
                        response = "Please provide a module name.";
                        return false;
                    }
                    API.Features.UpdateStatus status = UpdateStatus.Statuses.FirstOrDefault(s => string.Equals(s.ModuleName, module, StringComparison.CurrentCultureIgnoreCase)) ?? UpdateStatus.Statuses.FirstOrDefault(s => s.ModuleName.Split('.').Last().Equals(module, StringComparison.CurrentCultureIgnoreCase));
                    if (status == null)
                    {
                        response = "Module not found.";
                        return false;
                    }

                    Task.Run(async () => await UpdateStatus.DownloadPlugin(status));
                    ServerStatic.StopNextRound = ServerStatic.NextRoundAction.Restart;
                    response = "Downloaded module. Server will restart next round.";
                    return true;
                case "check":
                    Task.Run(UpdateStatus.GetStatus);
                    response = "Checking for updates... If any require an update, you will soon receive a log.";
                    return true;
                default:
                    response = "Invalid subcommand. Available subcommands: list, install";
                    return false;
            }
        }
    }
}