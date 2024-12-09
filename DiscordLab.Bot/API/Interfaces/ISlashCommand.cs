using Discord;
using Discord.WebSocket;

namespace DiscordLab.Bot.API.Interfaces
{
    public interface ISlashCommand
    {
        /// <summary>
        /// Here you create your <see cref="SlashCommandBuilder"/> with the data of your command.
        /// </summary>
        public SlashCommandBuilder Data { get; }
        
        /// <summary>
        /// Set the GuildId where the command should be created here, you should reference Config.GuildId.
        /// </summary>
        public ulong GuildId { get; set; }

        /// <summary>
        /// Here is where your slash command runs.
        /// </summary>
        /// <remarks>
        /// This type contains information about the command that was executed.
        /// </remarks>
        public Task Run(SocketSlashCommand command);
    }
}