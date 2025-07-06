namespace DiscordLab.Bot.Commands
{
    using System.Diagnostics.CodeAnalysis;
    using CommandSystem;
    using DiscordLab.Bot.API.Updates;

    /// <inheritdoc />
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public class LocalAdminCommand : ICommand
    {
        /// <inheritdoc />
        public string Command { get; } = "discordlab";

        /// <inheritdoc />
        public string[] Aliases { get; } = ["dl", "lab"];

        /// <inheritdoc />
        public string Description { get; } = "Do things directly with DiscordLab.";

        /// <inheritdoc />
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, [UnscopedRef] out string response)
        {
            switch (arguments.FirstOrDefault())
            {
                case "list":
                {
                    string modules = string.Join("\n", Module.CurrentModules.Where(s => s.Name != "DiscordLab.Bot").Select(s => s.Name));
                    response = "List of available DiscordLab modules:\n\n" + modules;
                    return true;
                }

                case "install":
                {
                    string moduleName = arguments.ElementAtOrDefault(1);
                    if (string.IsNullOrWhiteSpace(moduleName))
                    {
                        response = "Please provide a module name.";
                        return false;
                    }

                    Module module = Module.CurrentModules.FirstOrDefault(s => string.Equals(s.Name, moduleName, StringComparison.CurrentCultureIgnoreCase)) ?? Module.CurrentModules.FirstOrDefault(s => s.Name.Split('.').Last().Equals(moduleName, StringComparison.CurrentCultureIgnoreCase));
                    if (module == null)
                    {
                        response = "Module not found.";
                        return false;
                    }

                    Task.Run(module.Download);
                    ServerStatic.StopNextRound = ServerStatic.NextRoundAction.Restart;
                    response = "Downloaded module. Server will restart next round.";
                    return true;
                }

                case "check":
                {
                    Task.Run(Updater.ManageUpdates);
                    response = "Checking for updates...";
                    return true;
                }

                default:
                {
                    response = "Invalid subcommand. Available subcommands: list, install, check";
                    return false;
                }
            }
        }
    }
}