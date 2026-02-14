using Discord.WebSocket;
using DiscordLab.Bot;
using DiscordLab.Bot.API.Attributes;
using DiscordLab.Bot.API.Extensions;
using DiscordLab.Bot.API.Features;
using DiscordLab.Bot.API.Utilities;
using LabApi.Events;
using LabApi.Events.Arguments.ServerEvents;
using LabApi.Events.CustomHandlers;
using LabApi.Events.Handlers;
using LabApi.Features.Console;
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
        if (Config.ServerShutdownChannelId == 0)
            return;
        
        if (!Client.TryGetOrAddChannel(Config.ServerShutdownChannelId, out SocketTextChannel channel))
        {
            Logger.Error(LoggingUtils.GenerateMissingChannelMessage("server quit logs", Config.ServerShutdownChannelId, Config.GuildId));
            return;
        }
        
        Translation.ServerShutdown.SendToChannel(channel, new());
    }

    public static void OnServerStart()
    {
        ServerEvents.WaitingForPlayers -= OnServerStart;
        IsSubscribed = false;

        if (Config.ServerStartChannelId == 0)
            return;

        if (!Client.TryGetOrAddChannel(Config.ServerStartChannelId, out SocketTextChannel channel))
        {
            Logger.Error(LoggingUtils.GenerateMissingChannelMessage("server start logs", Config.ServerStartChannelId,
                Config.GuildId));
            return;
        }

        Translation.ServerStart.SendToChannel(channel, new());
    }

    public override void OnServerCommandExecuted(CommandExecutedEventArgs ev)
    {
        if (ev.Sender == null || !Player.TryGet(ev.Sender, out Player player))
            return;

        if (string.IsNullOrEmpty(ev.CommandName))
            return;

        SocketTextChannel channel;
        TranslationBuilder builder = new TranslationBuilder("player", player)
            .AddCustomReplacer("type", ev.CommandType.ToString())
            .AddCustomReplacer("arguments", () => string.Join(" ", ev.Arguments))
            .AddCustomReplacer("command", ev.CommandName)
            .AddCustomReplacer("commanddescription", () => ev.Command.Description ?? "Unknown")
            .AddCustomReplacer("commandsuccess", () => ev.ExecutedSuccessfully ? "Yes" : "No");

        MessageContent translation;
        if (ev.CommandType == CommandType.RemoteAdmin)
        {
            if (Config.RemoteAdminChannelId == 0)
                return;

            if (!Client.TryGetOrAddChannel(Config.RemoteAdminChannelId, out channel))
            {
                Logger.Error(LoggingUtils.GenerateMissingChannelMessage("remote admin logs",
                    Config.RemoteAdminChannelId, Config.GuildId));
                return;
            }
            
            translation = Config.UseSecondaryTranslationRemoteAdmin && !ev.ExecutedSuccessfully ? Translation.RemoteAdminCommandFailResponse : Translation.RemoteAdmin;
            translation.SendToChannel(channel, builder);
            return;
        }

        if (Config.CommandLogChannelId == 0)
            return;

        if (!Client.TryGetOrAddChannel(Config.CommandLogChannelId, out channel))
        {
            Logger.Error(LoggingUtils.GenerateMissingChannelMessage("command logs", Config.CommandLogChannelId,
                Config.GuildId));
            return;
        }
        
        translation = Config.UseSecondaryTranslationCommand && !ev.ExecutedSuccessfully ? Translation.CommandLogFailResponse : Translation.CommandLog;
        translation.SendToChannel(channel, builder);
    }
    
    public override void OnServerCommandExecuting(CommandExecutingEventArgs ev)
    {
        if (ev.Sender == null || !Player.TryGet(ev.Sender, out Player player))
            return;
        
        if (ev.CommandFound)
            return;

        if (string.IsNullOrEmpty(ev.CommandName))
            return;

        SocketTextChannel channel;
        TranslationBuilder builder = new TranslationBuilder("player", player)
            .AddCustomReplacer("type", ev.CommandType.ToString())
            .AddCustomReplacer("arguments", () => string.Join(" ", ev.Arguments))
            .AddCustomReplacer("command", ev.CommandName)
            .AddCustomReplacer("commanddescription", () => ev.Command.Description ?? "Unknown");

        if (ev.CommandType == CommandType.RemoteAdmin)
        {
            if (Config.RemoteAdminNotFoundChannelId == 0)
                return;

            if (!Client.TryGetOrAddChannel(Config.RemoteAdminNotFoundChannelId, out channel))
            {
                Logger.Error(LoggingUtils.GenerateMissingChannelMessage("remote admin logs (command not found)",
                    Config.RemoteAdminNotFoundChannelId, Config.GuildId));
                return;
            }

            Translation.RemoteAdminCommandNotFound.SendToChannel(channel, builder);
            return;
        }

        if (Config.CommandNotFoundChannelId == 0)
            return;

        if (!Client.TryGetOrAddChannel(Config.CommandNotFoundChannelId, out channel))
        {
            Logger.Error(LoggingUtils.GenerateMissingChannelMessage("command logs (command not found)", Config.CommandNotFoundChannelId,
                Config.GuildId));
            return;
        }

        Translation.CommandLogNotFound.SendToChannel(channel, builder);
    }
}