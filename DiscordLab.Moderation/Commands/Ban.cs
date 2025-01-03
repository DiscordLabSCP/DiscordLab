using Discord;
using Discord.WebSocket;
using DiscordLab.Bot.API.Extensions;
using DiscordLab.Bot.API.Interfaces;
using DiscordLab.Moderation.Handlers;
using Exiled.API.Features;

namespace DiscordLab.Moderation.Commands
{
    public class Ban : ISlashCommand
    {
        private static Translation Translation => Plugin.Instance.Translation;

        public SlashCommandBuilder Data { get; } = new()
        {
            Name = Translation.BanCommandName,
            Description = Translation.BanCommandDescription,
            DefaultMemberPermissions = GuildPermission.BanMembers,
            Options = new()
            {
                new()
                {
                    Name = Translation.BanCommandUserOptionName,
                    Description = Translation.BanCommandUserOptionDescription,
                    IsRequired = true,
                    Type = ApplicationCommandOptionType.String
                },
                new()
                {
                    Name = Translation.BanCommandReasonOptionName,
                    Description = Translation.BanCommandReasonOptionDescription,
                    IsRequired = true,
                    Type = ApplicationCommandOptionType.String
                },
                new()
                {
                    Name = Translation.BanCommandDurationOptionName,
                    Description = Translation.BanCommandDurationOptionDescription,
                    IsRequired = true,
                    Type = ApplicationCommandOptionType.String
                }
            }
        };
        
        public ulong GuildId { get; set; } = Plugin.Instance.Config.GuildId;

        public async Task Run(SocketSlashCommand command)
        {
            await command.DeferAsync(true);
            string user = command.Data.Options.First(option => option.Name == Translation.BanCommandUserOptionName)
                .Value.ToString();
            string reason = command.Data.Options.First(option => option.Name == Translation.BanCommandReasonOptionName)
                .Value.ToString();
            string duration = command.Data.Options
                .First(option => option.Name == Translation.BanCommandDurationOptionName).Value.ToString();

            string response = Server.ExecuteCommand($"/oban {user} {duration} {reason}");
            if (!response.Contains("has been banned"))
            {
                await command.ModifyOriginalResponseAsync(m=> m.Content = Translation.FailedExecuteCommand.LowercaseParams().Replace("{reason}", response));
            }
            else
            {
                await command.ModifyOriginalResponseAsync(m => m.Content = Translation.BanCommandSuccess.LowercaseParams().Replace("{player}", user));
                if (ModerationLogsHandler.Instance.IsEnabled)
                {
                    ModerationLogsHandler.Instance.SendBanLogMethod.Invoke(
                        ModerationLogsHandler.Instance.HandlerInstance, 
                        new object[] { null, user, reason, $"<@{command.User.Id}>", null, duration }
                    );
                }
            }
        }
    }
}