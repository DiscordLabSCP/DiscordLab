namespace DiscordLab.Bot.API.Utilities;

using System.Reflection;
using System.Text;
using LabApi.Features.Console;
using NorthwoodLib.Pools;

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

    /// <summary>
    /// Logs an exception that is thrown from a method.
    /// </summary>
    /// <param name="exception">The exception that got caught.</param>
    /// <param name="method">The method that the exception was thrown from.</param>
    /// <param name="type">The type the method comes from, isn't required but is useful.</param>
    /// <param name="message">The message that gets logged to the server.</param>
    public static void LogMethodError(Exception exception, MethodBase method, Type? type = null, string? message = null)
    {
        message ??= $"Got an exception whilst trying to run {GetFullName(method, type)}:\n{exception}";

        Logger.Error(message);
    }

    /// <summary>
    /// Gets the full name of a method from it's <see cref="MethodInfo"/> and/or <see cref="Type"/>.
    /// </summary>
    /// <param name="method">The method that you want the name of.</param>
    /// <param name="type">The type that the method is from, isn't required unless dynamic method is called, otherwise just the name of the method will print.</param>
    /// <returns>The full method name.</returns>
    public static string GetFullName(MethodBase method, Type? type = null)
    {
        StringBuilder builder = StringBuilderPool.Shared.Rent();

        if (method.DeclaringType != null && type != null)
        {
            builder.Append((method.DeclaringType ?? type).FullName);
            builder.Append(':');
        }

        builder.Append(method.Name);

        return StringBuilderPool.Shared.ToStringReturn(builder);
    }
}