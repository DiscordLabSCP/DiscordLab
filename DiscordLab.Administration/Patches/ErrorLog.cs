using DiscordLab.Core;
using DiscordLab.Core.API.TranslationBuilders;
using HarmonyLib;
using LabApi.Features.Console;
using Attachment = DiscordLab.Core.API.Features.Attachment;

namespace DiscordLab.Administration.Patches;

[HarmonyPatch(typeof(Logger), nameof(Logger.Error))]
public static class ErrorLog
{
    public static void Postfix(object message)
    {
        TranslationBuilder builder = new();

        MessageInformation info = Plugin.Instance.Translation.ErrorLog.Build(builder);

        MemoryStream stream = new();
        StreamWriter writer = new(stream);
        writer.Write(message.ToString());
        writer.Flush();
        stream.Position = 0;

        Attachment attachment = new(stream,
            $"Error {DateTime.UtcNow.ToShortDateString()} {DateTime.UtcNow.ToLongTimeString()}.txt");

        Task.Run(async () =>
        {
            try
            {
                await MessageHandler.SendMessages(Plugin.Instance.Config.ErrorLogChannel, info with { Attachment = attachment });
            }
            catch (Exception ex)
            {
                Logger.Raw($"[ERROR] [{Plugin.Instance.Name}] {ex}", ConsoleColor.Red);
            }
            finally
            {
                await writer.DisposeAsync();
                await stream.DisposeAsync();
            }
        });
    }
}