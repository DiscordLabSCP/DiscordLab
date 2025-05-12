using Discord.WebSocket;

namespace DiscordLab.Bot.API.Interfaces
{
    public interface IAutocompleteCommand : ISlashCommand
    {
        /// <summary>
        /// Control what happens when an autocomplete request is sent.
        /// </summary>
        /// <param name="autocomplete">The slash command instance</param>
        /// <returns>The Task completion status</returns>
        public Task Autocomplete(SocketAutocompleteInteraction autocomplete);
    }
}