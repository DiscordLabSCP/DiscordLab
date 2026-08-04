using DiscordLab.Core;
using DiscordLab.Core.API.Attributes;
using DiscordLab.Core.API.Extensions;
using DiscordLab.Core.API.TranslationBuilders;
using LabApi.Events.Arguments.ServerEvents;
using LabApi.Events.CustomHandlers;
using LabApi.Events.Handlers;
using LabApi.Features.Enums;
using LabApi.Features.Wrappers;

namespace DiscordLab.Administration;

public class Events : CustomEventsHandler
{
    public static Config Config => Plugin.Instance.Config;

    public static Translation Translation => Plugin.Instance.Translation;

    private static bool IsSubscribed { get; set; }

    [CallOnLoad]
    public static void Load()
    {
        ServerEvents.WaitingForPlayers += OnServerStart;
        Shutdown.OnQuit += OnServerQuit;
        IsSubscribed = true;
    }

    [CallOnUnload]
    public static void Unload()
    {
        Shutdown.OnQuit -= OnServerQuit;
        
        if (!IsSubscribed) return;
        ServerEvents.WaitingForPlayers -= OnServerStart;
        IsSubscribed = false;
    }

    public static void OnServerQuit()
    {
        Translation.ServerShutdown.Send(Config.ServerShutdownChannel, new());
    }

    public static void OnServerStart()
    {
        ServerEvents.WaitingForPlayers -= OnServerStart;
        Translation.ServerStart.Send(Config.ServerStartChannel, new());
    }

    public override void OnServerCommandExecuted(CommandExecutedEventArgs ev)
    {
        if (ev.Sender == null || !Player.TryGet(ev.Sender, out Player player))
            return;

        if (string.IsNullOrEmpty(ev.CommandName))
            return;

        TranslationBuilder builder = new PlayerTranslationBuilder("player", player)
            .AddCustomReplacer("type", ev.CommandType.ToString())
            .AddCustomReplacer("arguments", () => !ev.Arguments.Any() ? " " : string.Join(" ", ev.Arguments))
            .AddCustomReplacer("command", ev.CommandName)
            .AddCustomReplacer("commanddescription", () => ev.Command.Description ?? "Unknown")
            .AddCustomReplacer("commandsuccess", () => ev.ExecutedSuccessfully ? "Yes" : "No");

        MessageContent translation;
        if (ev.CommandType == CommandType.RemoteAdmin)
        {
            translation = Config.UseSecondaryTranslationRemoteAdmin && !ev.ExecutedSuccessfully ? Translation.RemoteAdminCommandFailResponse : Translation.RemoteAdmin;
            translation.Send(Config.RemoteAdminChannel, builder);
            return;
        }

        translation = Config.UseSecondaryTranslationCommand && !ev.ExecutedSuccessfully ? Translation.CommandLogFailResponse : Translation.CommandLog;
        translation.Send(Config.CommandLogChannel, builder);
    }
}