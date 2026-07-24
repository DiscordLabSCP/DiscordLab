using DiscordLab.Core.API.Commands;
using DiscordLab.Core.API.Updater;

namespace DiscordLab.Bot.Commands;

/// <inheritdoc />
public class DiscordCommand : ICommand
{
    /// <inheritdoc />
    public CommandBuilder Data { get; } = new()
    {
        Name = "discordlab",
        Description = "DiscordLab related commands",
        DefaultPermission = DefaultCommandPermissions.Admins,
        Options =
        [
            new()
            {
                Type = CommandOptionType.Subcommand,
                Name = "list",
                Description = "List all available DiscordLab modules",
            },

            new()
            {
                Type = CommandOptionType.Subcommand,
                Name = "install",
                Description = "The module to install",
                Options =
                [
                    new()
                    {
                        Type = CommandOptionType.String,
                        Name = "module",
                        Description = "The module to install",
                        IsRequired = true,
                    }
                ],
            },

            new()
            {
                Type = CommandOptionType.Subcommand,
                Name = "check",
                Description = "Check for DiscordLab updates",
            },

            new()
            {
                Type = CommandOptionType.Subcommand,
                Name = "update",
                Description = "Update DiscordLab forcefully, skips auto update checks",
            }
        ],
    };

    /// <inheritdoc />
    public async Task Execute(CommandInformation command)
    {
        await command.DeferResponse();
        CommandOptionInformation subcommand = command.Options.First();
        switch (subcommand.Name)
        {
            case "list":
            {
                string modules = string.Join(
                    "\n",
                    Module.CurrentModules.Where(s => s.Name != "DiscordLab.Bot")
                        .Select(s => $"{s.Name} (v{s.Version})"));
                await command.Reply("List of available DiscordLab modules:\n\n" + modules);
                break;
            }

            case "install":
            {
                string moduleName = subcommand.Options!.First().Value;
                if (string.IsNullOrWhiteSpace(moduleName))
                {
                    await command.Reply("Please provide a module name.");
                    return;
                }

                Module? module =
                    Module.CurrentModules.FirstOrDefault(s =>
                        string.Equals(s.Name, moduleName, StringComparison.CurrentCultureIgnoreCase)) ??
                    Module.CurrentModules.FirstOrDefault(s =>
                        s.Name.Split('.').Last().Equals(moduleName, StringComparison.CurrentCultureIgnoreCase));
                if (module == null || module.Name == "DiscordLab.Bot")
                {
                    await command.Reply("Module not found.");
                    return;
                }

                await module.Download();
                ServerStatic.StopNextRound = ServerStatic.NextRoundAction.Restart;
                await command.Reply("Downloaded module. Server will restart next round.");
                break;
            }

            case "check":
            {
                IEnumerable<Module> modules = await Updater.ManageUpdates();
                if (!modules.Any())
                {
                    await command.Reply("No updates found.");
                    return;
                }

                await command.Reply($"Updates found, modules that need updating:\n{Module.GenerateUpdateString(modules)}");
                break;
            }

            case "update":
            {
                IEnumerable<Module> modules = await Updater.ManageUpdates();

                if (!modules.Any())
                {
                    await command.Reply("No updates found.");
                    return;
                }

                if (!Core.Plugin.Instance.Config.AutoUpdate)
                {
                    await command.Reply($"Updates found, modules that need updating:\n{Module.GenerateUpdateString(modules)}");
                    return;
                }

                // Force updates, because ManageUpdates checks for AutoUpdate, and will trigger the update.
                foreach (Module module in modules)
                {
                    await module.Download();
                }

                await command.Reply($"Updates found, modules that need updating:\n{Module.GenerateUpdateString(modules)}");
                break;
            }
        }
    }
}