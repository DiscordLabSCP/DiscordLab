namespace DiscordLab.Bot.API.Interfaces
{
    using Discord.WebSocket;

    /// <summary>
    /// Allows you to make autocomplete commands.
    /// </summary>
    public interface IAutocompleteCommand : ISlashCommand
    {
        /// <summary>
        /// The method that is called once an autocomplete request is made.
        /// </summary>
        /// <param name="autocomplete">The autocomplete instance.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public Task Autocomplete(SocketAutocompleteInteraction autocomplete);
    }
}