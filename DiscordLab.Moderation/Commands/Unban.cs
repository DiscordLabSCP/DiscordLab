using Discord;
using Discord.WebSocket;
using DiscordLab.Bot.API.Interfaces;
using Exiled.API.Features;
using RemoteAdmin;

namespace DiscordLab.Moderation.Commands
{
    public class Unban : ISlashCommand
    {
        private static Translation Translation => Plugin.Instance.Translation;

        public SlashCommandBuilder Data { get; } = new()
        {
            Name = Translation.UnbanCommandName,
            Description = Translation.UnbanCommandDescription,
            DefaultMemberPermissions = GuildPermission.ManageGuild,
            Options = new()
            {
                new()
                {
                    Name = Translation.UnbanCommandUserOptionName,
                    Description = Translation.UnbanCommandUserOptionDescription,
                    IsRequired = true,
                    Type = ApplicationCommandOptionType.String
                }
            }
        };

        public async Task Run(SocketSlashCommand command)
        {
            string user = command.Data.Options.First(option => option.Name == Translation.BanCommandUserOptionName)
                .Value.ToString();

            string response = Server.ExecuteCommand($"unban id {user}");
            if (!response.Contains("Done"))
            {
                await command.RespondAsync(Translation.FailedExecuteCommand.Replace("{reason}", response),
                    ephemeral: true);
            }
            else
            {
                await command.RespondAsync(Translation.UnbanCommandSuccess.Replace("{player}", user), ephemeral: true);
                if (Plugin.Instance.CheckModerationLogsEnabled())
                {
                    ModerationLogs.Handlers.DiscordBot.Instance.SendUnbanMessage(user);
                }
            }
        }
    }
}