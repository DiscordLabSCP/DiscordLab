using Discord;
using Discord.WebSocket;

namespace DiscordLab.Bot.API.Interfaces
{
    public interface ISlashCommand
    {
        /// <summary>
        /// Here you create your <see cref="SlashCommandBuilder"/> with the data of your command.
        /// </summary>
        SlashCommandBuilder Data { get; }

        /// <summary>
        /// Here is where your slash command runs
        /// </summary>
        /// <remarks>
        /// This type contains information about the command that was executed
        /// </remarks>
        Task Run(SocketSlashCommand command);
    }
}