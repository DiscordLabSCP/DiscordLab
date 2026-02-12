using Discord;
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
    public static async void Postfix(object message)
    {
        if (Plugin.Instance.Config.ErrorLogChannelId == 0)
            return;

        if (!Client.TryGetOrAddChannel(Plugin.Instance.Config.ErrorLogChannelId, out SocketTextChannel channel))
        {
            Logger.Raw(
                $"[ERROR] [{Plugin.Instance.Name}] {LoggingUtils.GenerateMissingChannelMessage("error logs", Plugin.Instance.Config.ErrorLogChannelId, Plugin.Instance.Config.GuildId)}",
                ConsoleColor.Red);
            return;
        }

        TranslationBuilder builder = new TranslationBuilder();

        (Embed embed, string content) = Plugin.Instance.Translation.ErrorLog.Build(builder);

        MemoryStream stream = new MemoryStream();
        StreamWriter writer = new StreamWriter(stream);
        writer.Write(message.ToString());
        writer.Flush();
        stream.Position = 0;

        FileAttachment attachment = new(stream,
            $"Error {DateTime.UtcNow.ToShortDateString()} {DateTime.UtcNow.ToLongTimeString()}.txt");

        try
{
    await channel.SendFileAsync(attachment, content, embed: embed);
}
catch (Exception ex)
{
    Logger.Raw($"[ERROR] [{Plugin.Instance.Name}] {ex}", ConsoleColor.Red);
}
    }
}
