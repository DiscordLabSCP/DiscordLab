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
                string modules = string.Join("\n", UpdateStatus.Statuses.Where(s => s.ModuleName != "DiscordLab.Bot").Select(s => s.ModuleName));
                await command.RespondAsync("List of available DiscordLab modules:\n\n" + modules, ephemeral:true);
            }
            else if (subcommand == "install")
            {
                string module = command.Data.Options.First().Options.First().Value.ToString();
                if(string.IsNullOrWhiteSpace(module))
                {
                    await command.RespondAsync("Please provide a module name.", ephemeral: true);
                    return;
                }
                API.Features.UpdateStatus status = UpdateStatus.Statuses.FirstOrDefault(s => s.ModuleName == module);
                if (status == null)
                {
                    await command.RespondAsync("Module not found.", ephemeral: true);
                    return;
                }

                await UpdateStatus.DownloadPlugin(status);
                ServerStatic.StopNextRound = ServerStatic.NextRoundAction.Restart;
                await command.RespondAsync("Downloaded module. Server will restart next round.", ephemeral:true);
            }
        }
    }
}