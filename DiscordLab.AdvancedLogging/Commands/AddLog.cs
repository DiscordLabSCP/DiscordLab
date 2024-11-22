using Discord;
using Discord.WebSocket;
using DiscordLab.Bot.API.Interfaces;

namespace DiscordLab.AdvancedLogging.Commands
{
    public class AddLog : ISlashCommand
    {
        public SlashCommandBuilder Data { get; } = new()
        {
            Name = "addlog",
            Description = "Add your own custom logger, check the documentation for more info",
            DefaultMemberPermissions = GuildPermission.ManageGuild
        };

        public async Task Run(SocketSlashCommand command)
        {
            ModalBuilder modal = new()
            {
                Title = "Create a new custom log",
                CustomId = "addlogmodal"
            };
            modal.AddTextInput("Handler, i.e. 'Player' for Handlers.Player", "handler", TextInputStyle.Short, "Player",
                null, null, true);
            modal.AddTextInput("Event, i.e. 'Died' for Player.Died", "event", TextInputStyle.Short, "Died", null, null,
                true);
            modal.AddTextInput("Message, i.e. 'Player {Player.Nickname} died'", "message", TextInputStyle.Paragraph,
                "Player {Player.Nickname} died", null, null, true);
            modal.AddTextInput("Nulls, do nothing when null, comma separated", "nullables", TextInputStyle.Paragraph,
                "Attacker", null, null, false);
            modal.AddTextInput("Channel, use channel ID", "channel", TextInputStyle.Short, "", null, null, true);

            await command.RespondWithModalAsync(modal.Build());
        }
    }
}