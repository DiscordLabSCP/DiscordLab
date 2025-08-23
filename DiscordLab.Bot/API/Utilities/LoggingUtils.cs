namespace DiscordLab.Bot.API.Utilities;

/// <summary>
/// Contains utilities for logging related tasks.
/// </summary>
public static class LoggingUtils
{
    /// <summary>
    /// Generates a message that will tell the user that the channel was not found.
    /// </summary>
    /// <param name="type">The submodule that this error comes from.</param>
    /// <param name="channelId">The channel ID that was missing.</param>
    /// <param name="guildId">The related guild ID, goes to the default guild ID if 0.</param>
    /// <returns>The error string.</returns>
    public static string GenerateMissingChannelMessage(string type, ulong channelId, ulong guildId)
    {
        if (guildId == 0)
            guildId = Plugin.Instance.Config.GuildId;

        return
            $"Could not find channel {channelId} under the guild {guildId}, please make sure the bot has access and you put in the right IDs. This was triggered from {type}.";
    }
}