namespace DiscordLab.Bot.Commands;

using Discord;
using Discord.WebSocket;
using DiscordLab.Bot.API.Features;
using DiscordLab.Bot.API.Updates;

/// <inheritdoc />
public class DiscordCommand : AutocompleteCommand
{
    /// <inheritdoc />
    public override SlashCommandBuilder Data { get; } = new()
    {
        Name = "discordlab",
        Description = "DiscordLab related commands",
        DefaultMemberPermissions = GuildPermission.Administrator,
        Options =
        [
            new()
            {
                Type = ApplicationCommandOptionType.SubCommand,
                Name = "list",
                Description = "List all available DiscordLab modules",
            },

            new()
            {
                Type = ApplicationCommandOptionType.SubCommand,
                Name = "install",
                Description = "The module to install",
                Options =
                [
                    new()
                    {
                        Type = ApplicationCommandOptionType.String,
                        Name = "module",
                        Description = "The module to install",
                        IsRequired = true,
                        IsAutocomplete = true,
                    }
                ],
            },

            new()
            {
                Type = ApplicationCommandOptionType.SubCommand,
                Name = "check",
                Description = "Check for DiscordLab updates",
            },

            new()
            {
                Type = ApplicationCommandOptionType.SubCommand,
                Name = "update",
                Description = "Update DiscordLab forcefully, skips auto update checks",
            }
        ],
    };

    /// <inheritdoc />
    protected override ulong GuildId { get; } = 0;

    /// <inheritdoc />
    public override async Task Run(SocketSlashCommand command)
    {
        await command.DeferAsync(true);
        string subcommand = command.Data.Options.First().Name;
        switch (subcommand)
        {
            case "list":
            {
                string modules = string.Join(
                    "\n",
                    Module.CurrentModules.Where(s => s.Name != "DiscordLab.Bot")
                        .Select(s => $"{s.Name} (v{s.Version})"));
                await command.ModifyOriginalResponseAsync(m =>
                    m.Content = "List of available DiscordLab modules:\n\n" + modules);
                break;
            }

            case "install":
            {
                string moduleName = command.Data.Options.First().Options.First().Value.ToString();
                if (string.IsNullOrWhiteSpace(moduleName))
                {
                    await command.ModifyOriginalResponseAsync(m => m.Content = "Please provide a module name.");
                    return;
                }

                Module? module =
                    Module.CurrentModules.FirstOrDefault(s =>
                        string.Equals(s.Name, moduleName, StringComparison.CurrentCultureIgnoreCase)) ??
                    Module.CurrentModules.FirstOrDefault(s =>
                        s.Name.Split('.').Last().Equals(moduleName, StringComparison.CurrentCultureIgnoreCase));
                if (module == null || module.Name == "DiscordLab.Bot")
                {
                    await command.ModifyOriginalResponseAsync(m => m.Content = "Module not found.");
                    return;
                }

                await module.Download();
                ServerStatic.StopNextRound = ServerStatic.NextRoundAction.Restart;
                await command.ModifyOriginalResponseAsync(m =>
                    m.Content = "Downloaded module. Server will restart next round.");
                break;
            }

            case "check":
            {
                IEnumerable<Module> modules = await Updater.ManageUpdates();
                if (!modules.Any())
                {
                    await command.ModifyOriginalResponseAsync(m => m.Content = "No updates found.");
                    return;
                }

                await command.ModifyOriginalResponseAsync(m =>
                    m.Content = $"Updates found, modules that need updating:\n{Module.GenerateUpdateString(modules)}");
                break;
            }

            case "update":
            {
                IEnumerable<Module> modules = await Updater.ManageUpdates();

                if (!modules.Any())
                {
                    await command.ModifyOriginalResponseAsync(m => m.Content = "No updates found.");
                    return;
                }

                if (Plugin.Instance.Config.AutoUpdate)
                {
                    await command.ModifyOriginalResponseAsync(m =>
                        m.Content =
                            $"Updates found, modules that need updating:\n{Module.GenerateUpdateString(modules)}");
                    return;
                }

                // Force updates, because ManageUpdates checks for AutoUpdate, and will trigger the update.
                foreach (Module module in modules)
                {
                    await module.Download();
                }

                await command.ModifyOriginalResponseAsync(m =>
                    m.Content =
                        $"Updates found, modules that need updating:\n{Module.GenerateUpdateString(modules)}");
                break;
            }
        }
    }

    /// <inheritdoc />
    public override async Autocomplete(SocketAutocompleteInteraction autocomplete)
    {
        await autocomplete.RespondAsync(Module.CurrentModules
            .Where(x => x.Name != "DiscordLab.Bot" && x.Name.Contains((string)autocomplete.Data.Current.Value)).Take(25)
            .Select(x => new AutocompleteResult($"{x.Name} (v{x.Version})", x.Name)));
    }
}
