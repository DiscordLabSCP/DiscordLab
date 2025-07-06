namespace DiscordLab.Bot.API.Extensions
{
    using Discord;
    using Discord.WebSocket;

    /// <summary>
    /// Extension methods to help with Discord based tasks.
    /// </summary>
    public static class DiscordExtensions
    {
        /// <summary>
        /// Runs a task that sends a message to the specified channel.
        /// </summary>
        /// <param name="channel">The channel to send the message to.</param>
        /// <param name="text">The text.</param>
        /// <param name="isTts">Whether the message is TTS.</param>
        /// <param name="embed">The embed.</param>
        /// <param name="embeds">The embeds.</param>
        /// <remarks>Text, embed or embeds is required here.</remarks>
        public static void SendMessage(this SocketTextChannel channel, string text = null, bool isTts = false, Embed embed = null, Embed[] embeds = null) =>
            Task.Run(async () => await channel.SendMessageAsync(text, isTts, embed, embeds: embeds).ConfigureAwait(false));
    }
}