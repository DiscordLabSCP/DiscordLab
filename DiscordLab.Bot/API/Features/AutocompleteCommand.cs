namespace DiscordLab.Bot.API.Features
{
    using Discord.WebSocket;

    /// <summary>
    /// Allows you to make autocomplete commands.
    /// </summary>
    public abstract class AutocompleteCommand : SlashCommand
    {
        /// <summary>
        /// The method that is called once an autocomplete request is made.
        /// </summary>
        /// <param name="autocomplete">The autocomplete instance.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public abstract Task Autocomplete(SocketAutocompleteInteraction autocomplete);
    }
}