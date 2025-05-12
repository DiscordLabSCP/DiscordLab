using Discord;
using Discord.WebSocket;
using DiscordLab.Bot.API.Interfaces;
using DiscordLab.Bot.API.Modules;

namespace DiscordLab.Bot.Commands
{
    public class Discord : IAutocompleteCommand
    {
        public SlashCommandBuilder Data { get; } = new()
        {
            Name = "discordlab",
            Description = "DiscordLab related commands",
            DefaultMemberPermissions = GuildPermission.Administrator,
            Options = new()
            {
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
                    Description = "Install a DiscordLab module",
                    Options = new ()
                    {
                        new ()
                        {
                            Type = ApplicationCommandOptionType.String,
                            Name = "module",
                            Description = "The module to install",
                            IsRequired = true,
                            IsAutocomplete = true
                        }
                    }
                }
            }
        };
        
        public ulong GuildId { get; set; } = Plugin.Instance.Config.GuildId;
        
        public async Task Run(SocketSlashCommand command)
        {
            await command.DeferAsync(true);
            string subcommand = command.Data.Options.First().Name;
            if (subcommand == "list")
            {
                if (UpdateStatus.Statuses == null)
                {
                    await command.ModifyOriginalResponseAsync(m => m.Content = "No modules available as of current, please wait for your server to fully start.");
                    return;
                }
                string modules = string.Join("\n", UpdateStatus.Statuses.Where(s => s.ModuleName != "DiscordLab.Bot").Select(s => s.ModuleName));
                await command.ModifyOriginalResponseAsync(m => m.Content = "List of available DiscordLab modules:\n\n" + modules);
            }
            else if (subcommand == "install")
            {
                if (UpdateStatus.Statuses == null)
                {
                    await command.ModifyOriginalResponseAsync(m => m.Content = "No modules available as of current, please wait for your server to fully start.");
                    return;
                }
                string module = command.Data.Options.First().Options.First().Value.ToString();
                if(string.IsNullOrWhiteSpace(module))
                {
                    await command.ModifyOriginalResponseAsync(m => m.Content = "Please provide a module name.");
                    return;
                }
                API.Features.UpdateStatus status = UpdateStatus.Statuses.FirstOrDefault(s => string.Equals(s.ModuleName, module, StringComparison.CurrentCultureIgnoreCase)) ?? UpdateStatus.Statuses.FirstOrDefault(s => s.ModuleName.Split('.').Last().Equals(module, StringComparison.CurrentCultureIgnoreCase));
                if (status == null)
                {
                    await command.ModifyOriginalResponseAsync(m => m.Content = "Module not found.");
                    return;
                }

                await UpdateStatus.DownloadPlugin(status);
                ServerStatic.StopNextRound = ServerStatic.NextRoundAction.Restart;
                await command.ModifyOriginalResponseAsync(m => m.Content = "Downloaded module. Server will restart next round.");
            }
        }

        public async Task Autocomplete(SocketAutocompleteInteraction autocomplete)
        {
            if (UpdateStatus.Statuses == null)
            {
                await autocomplete.RespondAsync(new List<AutocompleteResult>());
                return;
            }
            await autocomplete.RespondAsync(result: UpdateStatus.Statuses
                .Where(s => s.ModuleName != "DiscordLab.Bot").Select(s => new AutocompleteResult
                {
                    Name = s.ModuleName,
                    Value = s.ModuleName
                }));
        }
    }
}