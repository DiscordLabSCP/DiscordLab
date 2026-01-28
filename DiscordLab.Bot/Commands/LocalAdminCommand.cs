using LabApi.Features.Console;

namespace DiscordLab.Bot.Commands;

using System.Diagnostics.CodeAnalysis;
using CommandSystem;
using DiscordLab.Bot.API.Extensions;
using DiscordLab.Bot.API.Updates;

/// <inheritdoc />
[CommandHandler(typeof(GameConsoleCommandHandler))]
public class LocalAdminCommand : ICommand
{
    /// <inheritdoc />
    public string Command { get; } = "discordlab";

    /// <inheritdoc />
    public string[] Aliases { get; } = ["dl", "dlab"];

    /// <inheritdoc />
    public string Description { get; } = "Do things directly with DiscordLab.";

    /// <inheritdoc />
    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        switch (arguments.FirstOrDefault())
        {
            case "list":
            {
                string modules = string.Join(
                    "\n",
                    Module.CurrentModules.Where(s => s.Name != "DiscordLab.Bot")
                        .Select(s => $"{s.Name} (v{s.Version})"));
                response = "List of available DiscordLab modules:\n\n" + modules;
                return true;
            }

            case "install":
            {
                string? moduleName = arguments.ElementAtOrDefault(1);
                if (string.IsNullOrWhiteSpace(moduleName))
                {
                    response = "Please provide a module name.";
                    return false;
                }

                Module? module =
                    Module.CurrentModules.FirstOrDefault(s =>
                        string.Equals(s.Name, moduleName, StringComparison.CurrentCultureIgnoreCase)) ??
                    Module.CurrentModules.FirstOrDefault(s =>
                        s.Name.Split('.').Last().Equals(moduleName, StringComparison.CurrentCultureIgnoreCase));
                if (module == null || module.Name == "DiscordLab.Bot")
                {
                    response = "Module not found.";
                    return false;
                }

                Task.RunAndLog(module.Download);
                ServerStatic.StopNextRound = ServerStatic.NextRoundAction.Restart;
                response = "Downloaded module. Server will restart next round.";
                return true;
            }

            case "check":
            {
                Task.RunAndLog(Updater.ManageUpdates);
                response = "Checking for updates...";
                return true;
            }

            case "update":
            {
                Task.RunAndLog(async () =>
                {
                    IEnumerable<Module> modules = await Updater.ManageUpdates();

                    if (!modules.Any())
                    {
                        return;
                    }

                    if (Plugin.Instance.Config.AutoUpdate)
                    {
                        return;
                    }

                    // Force updates, because ManageUpdates checks for AutoUpdate, and will trigger the update.
                    foreach (Module module in modules)
                    {
                        await module.Download();
                    }

                    Logger.Info($"Updates found, modules that need updating:\n{Module.GenerateUpdateString(modules)}");
                });
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