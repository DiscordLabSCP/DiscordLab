using Discord.WebSocket;
using DiscordLab.Bot;
using DiscordLab.Bot.API.Extensions;
using DiscordLab.Bot.API.Features;
using DiscordLab.Bot.API.Utilities;
using HarmonyLib;
using LabApi.Features.Console;

namespace DiscordLab.Administration.Patches;

[HarmonyPatch(typeof(Logger), nameof(Logger.Error))]
public static class ErrorLog
{
    public static void Postfix(object message)
    {
        var plugin = Plugin.Instance;

        if (plugin.Config.ErrorLogChannelId == 0)
            return;

        if (!Client.TryGetOrAddChannel(plugin.Config.ErrorLogChannelId, out SocketTextChannel channel))
        {
            Logger.Raw(
                $"[ERROR] [{plugin.Name}] " +
                LoggingUtils.GenerateMissingChannelMessage(
                    "error logs",
                    plugin.Config.ErrorLogChannelId,
                    plugin.Config.GuildId),
                ConsoleColor.Red);

            return;
        }

        string errorText = message?.ToString() ?? "Unknown error";

        var builder = new TranslationBuilder()
            .AddCustomReplacer("error", errorText);

        plugin.Translation.ErrorLog.SendToChannel(channel, builder);
    }
}
