namespace DiscordLab.Bot.API.Extensions;

using System.Collections.Concurrent;
using Discord;
using Discord.WebSocket;
using DiscordLab.Bot.API.Attributes;
using LabApi.Features.Console;

/// <summary>
/// Extension methods to help with Discord based tasks.
/// </summary>
public static class DiscordExtensions
{
    private static readonly ConcurrentDictionary<ulong, (string? Text, List<Embed> Embeds)> FrameQueue = new();

    /// <summary>
    /// Runs a task that sends a message to the specified channel.
    /// </summary>
    /// <param name="channel">The channel to send the message to.</param>
    /// <param name="text">The text.</param>
    /// <param name="isTts">Whether the message is TTS.</param>
    /// <param name="embed">The embed.</param>
    /// <param name="embeds">The embeds.</param>
    /// <remarks>Text, embed or embeds is required here.</remarks>
    public static void SendMessage(this SocketTextChannel channel, string? text = null, bool isTts = false, Embed? embed = null, Embed[]? embeds = null)
    {
        if (isTts)
        {
            PrivateSendMessage(channel, text, isTts, embed, embeds);
            return;
        }

        List<Embed> embedList = [];
        if (embed != null)
            embedList.Add(embed);
        if (embeds != null)
            embedList.AddRange(embeds);

        FrameQueue.AddOrUpdate(channel.Id, _ => (text, embedList), (_, val) =>
        {
            val.Embeds.AddRange(embedList);
            if (val.Text != null)
                val.Text += $"\n{text}";
            else
                val.Text = text;

            return val;
        });
    }

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
            return t;

        return default;
    }

    [CallOnLoad]
    private static void Setup()
    {
        StaticUnityMethods.OnLateUpdate += OnLateUpdate;
    }

    [CallOnUnload]
    private static void Unload()
    {
        StaticUnityMethods.OnLateUpdate -= OnLateUpdate;
    }

    private static void OnLateUpdate()
    {
        try
        {
            if (FrameQueue.Count == 0)
                return;

            foreach (KeyValuePair<ulong, (string? Text, List<Embed> Embeds)> kvp in FrameQueue)
            {
                if (!Client.TryGetOrAddChannel(kvp.Key, out SocketTextChannel? channel))
                    continue;

                PrivateSendMessage(channel, kvp.Value.Text, embeds: kvp.Value.Embeds.ToArray());
            }

            FrameQueue.Clear();
        }
        catch (Exception ex)
        {
            Logger.Error(ex);
        }
    }

    private static void PrivateSendMessage(SocketTextChannel channel, string? text = null, bool isTts = false, Embed? embed = null, Embed[]? embeds = null) =>
        Task.Run(async () => await channel.SendMessageAsync(text, isTts, embed, embeds: embeds).ConfigureAwait(false));
}