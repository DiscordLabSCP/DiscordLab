namespace DiscordLab.Bot.API.Extensions;

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
    public static void SendMessage(this SocketTextChannel channel, string? text = null, bool isTts = false, Embed? embed = null, Embed[]? embeds = null) =>
        Task.RunAndLog(async () => await channel.SendMessageAsync(text, isTts, embed, embeds: embeds).ConfigureAwait(false));

    /// <summary>
    /// Gets an option from a list of slash command options.
    /// </summary>
    /// <param name="options">The options to check from.</param>
    /// <param name="name">The option name to get.</param>
    /// <typeparam name="T">The type that this option should return.</typeparam>
    /// <returns>The found item, if any.</returns>
    public static T? GetOption<T>(this IReadOnlyCollection<SocketSlashCommandDataOption> options, string name)
    {
        if (options.FirstOrDefault(e => e.Name == name)?.Value is T t)
        {
            return t;
        }

        return default;
    }
}