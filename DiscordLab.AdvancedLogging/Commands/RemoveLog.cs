using Discord;
using Discord.WebSocket;
using DiscordLab.AdvancedLogging.API.Features;
using DiscordLab.AdvancedLogging.Handlers;
using DiscordLab.Bot.API.Interfaces;
using DiscordLab.Bot.API.Modules;
using Newtonsoft.Json.Linq;

namespace DiscordLab.AdvancedLogging.Commands
{
    public class RemoveLog : ISlashCommand
    {
        public SlashCommandBuilder Data { get; } = new()
        {
            Name = "removelog",
            Description = "Removes a log from the list of logs.",
            DefaultMemberPermissions = GuildPermission.ManageGuild,
            Options = new()
            {
                new()
                {
                    Name = "log",
                    Description = "e.g. Player.Died",
                    Type = ApplicationCommandOptionType.String
                }
            }
        };
        
        public ulong GuildId { get; set; } = Plugin.Instance.Config.GuildId;

        public async Task Run(SocketSlashCommand command)
        {
            await command.DeferAsync(true);
            List<Log> logs = DiscordBot.Instance.GetLogs().ToList();
            string log = command.Data.Options.First().Value.ToString();
            Log logToRemove = logs.FirstOrDefault(l => l.Handler == log.Split('.')[0] && l.Event == log.Split('.')[1]);
            if (logToRemove == null)
            {
                await command.ModifyOriginalResponseAsync(m => m.Content = "Log not found.");
                return;
            }

            logs.Remove(logToRemove);

            WriteableConfig.WriteConfigOption("AdvancedLogging", JArray.FromObject(logs));

            await command.ModifyOriginalResponseAsync(m => m.Content = "Log removed.");
        }
    }
}