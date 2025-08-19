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
        if (Plugin.Instance.Config.ErrorLogChannelId == 0)
            return;

        if (!Client.TryGetOrAddChannel(Plugin.Instance.Config.ErrorLogChannelId, out SocketTextChannel channel))
        {
            Logger.Raw(
                $"[ERROR] [{Plugin.Instance.Name}] {LoggingUtils.GenerateMissingChannelMessage("error logs", Plugin.Instance.Config.ErrorLogChannelId, Plugin.Instance.Config.GuildId)}", ConsoleColor.Red);
            return;
        }

        TranslationBuilder builder = new TranslationBuilder()
            .AddCustomReplacer("error", message.ToString());
            
        Plugin.Instance.Translation.ErrorLog.SendToChannel(channel, builder);
    }
}