using Discord;
using Discord.WebSocket;
using DiscordLab.Bot.API.Interfaces;
using Exiled.API.Features;

namespace DiscordLab.Moderation.Commands
{
    public class SendCommand : ISlashCommand
    {
        private static Translation Translation => Plugin.Instance.Translation;

        public SlashCommandBuilder Data { get; } = new()
        {
            Name = Translation.SendCommandName,
            Description = Translation.SendCommandDescription,
            DefaultMemberPermissions = GuildPermission.ManageGuild,
            Options = new()
            {
                new()
                {
                    Name = Translation.SendCommandCommandOptionName,
                    Description = Translation.SendCommandCommandOptionDescription,
                    IsRequired = true,
                    Type = ApplicationCommandOptionType.String
                }
            }
        };
        
        public async Task Run(SocketSlashCommand command)
        {
            string commandToExecute = command.Data.Options.First(option => option.Name == Translation.SendCommandCommandOptionName)
                .Value.ToString();

            string response = Server.ExecuteCommand(commandToExecute);
            await command.RespondAsync(Translation.SendCommandResponse.Replace("{response}", response),
                ephemeral: true);
        }
    }
}