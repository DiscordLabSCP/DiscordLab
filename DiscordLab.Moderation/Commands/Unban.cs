using Discord;
using Discord.WebSocket;
using DiscordLab.Bot.API.Extensions;
using DiscordLab.Bot.API.Interfaces;
using DiscordLab.Moderation.Handlers;
using Exiled.API.Features;

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
        
        public ulong GuildId { get; set; } = Plugin.Instance.Config.GuildId;

        public async Task Run(SocketSlashCommand command)
        {
            await command.DeferAsync(true);
            
            if (command.User is not SocketGuildUser guildUser || !guildUser.Roles.Any(role => role.Id == Plugin.Instance.Config.UnbanCommandRole))
            {
                await command.ModifyOriginalResponseAsync(m => m.Content = Translation.NoPermissions);
                return;
            }
            
            string user = command.Data.Options.First(option => option.Name == Translation.BanCommandUserOptionName)
                .Value.ToString();

            string response = Server.ExecuteCommand($"/unban id {user}");
            if (!response.Contains("Done"))
            {
                await command.ModifyOriginalResponseAsync(m => m.Content = Translation.FailedExecuteCommand.LowercaseParams().Replace("{reason}", response));
            }
            else
            {
                await command.ModifyOriginalResponseAsync(m => m.Content = Translation.UnbanCommandSuccess.LowercaseParams().Replace("{player}", user));
                if (ModerationLogsHandler.Instance.IsEnabled)
                {
                    ModerationLogsHandler.Instance.SendUnbanLogMethod.Invoke(
                        ModerationLogsHandler.Instance.HandlerInstance, 
                        new object[] { user }
                    );
                }
            }
        }
    }
}