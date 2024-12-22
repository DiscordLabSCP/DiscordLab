using Discord;
using Discord.WebSocket;
using DiscordLab.Bot.API.Interfaces;
using DiscordLab.Bot.API.Modules;

namespace DiscordLab.Bot.Commands
{
    public class Discord : ISlashCommand
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
            string subcommand = command.Data.Options.First().Name;
            if (subcommand == "list")
            {
                if (UpdateStatus.Statuses == null)
                {
                    await command.RespondAsync("No modules available as of current, please wait for your server to fully start.");
                    return;
                }
                string modules = string.Join("\n", UpdateStatus.Statuses.Where(s => s.ModuleName != "DiscordLab.Bot").Select(s => s.ModuleName));
                await command.RespondAsync("List of available DiscordLab modules:\n\n" + modules, ephemeral:true);
            }
            else if (subcommand == "install")
            {
                if (UpdateStatus.Statuses == null)
                {
                    await command.RespondAsync("No modules available as of current, please wait for your server to fully start.");
                    return;
                }
                await command.DeferAsync(true);
                string module = command.Data.Options.First().Options.First().Value.ToString();
                if(string.IsNullOrWhiteSpace(module))
                {
                    await command.ModifyOriginalResponseAsync(m => m.Content = "Please provide a module name.");
                    return;
                }
                API.Features.UpdateStatus status = UpdateStatus.Statuses.FirstOrDefault(s => s.ModuleName == module);
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
    }
}