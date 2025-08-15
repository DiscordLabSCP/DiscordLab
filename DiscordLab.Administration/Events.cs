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
        IsSubscribed = true;
    }

    [CallOnUnload]
    public static void Unload()
    {
        if (!IsSubscribed) return;
        ServerEvents.WaitingForPlayers -= OnServerStart;
        IsSubscribed = false;
    }
        
    public static void OnServerStart()
    {
        ServerEvents.WaitingForPlayers -= OnServerStart;
        IsSubscribed = false;

        if (Config.ServerStartChannelId == 0)
            return;

        if (!Client.TryGetOrAddChannel(Config.ServerStartChannelId, out SocketTextChannel channel))
        {
            Logger.Error(LoggingUtils.GenerateMissingChannelMessage("server start logs", Config.ServerStartChannelId, Config.GuildId));
            return;
        }
            
        channel.SendMessage(Translation.ServerStart);
    }

    public override void OnServerCommandExecuted(CommandExecutedEventArgs ev)
    {
        if (ev.Sender == null || !Player.TryGet(ev.Sender, out Player player))
            return;
            
        SocketTextChannel channel;
        TranslationBuilder builder;

        Dictionary<string, Func<string>> customReplacers = new()
        {
            ["type"] = () => ev.CommandType.ToString(),
            ["arguments"] = () => string.Join(" ", ev.Arguments),
            ["command"] = () => ev.Command.Command,
            ["commanddescription"] = () => ev.Command.Description,
        };
            
        if (ev.CommandType == CommandType.RemoteAdmin)
        {
            if (Config.RemoteAdminChannelId == 0)
                return;

            if (!Client.TryGetOrAddChannel(Config.RemoteAdminChannelId, out channel))
            {
                Logger.Error(LoggingUtils.GenerateMissingChannelMessage("remote admin logs", Config.RemoteAdminChannelId, Config.GuildId));
                return;
            }

            builder = new(Translation.RemoteAdmin, "player", player)
            {
                CustomReplacers = customReplacers
            };
                
            channel.SendMessage(builder);
            return;
        }

        if (Config.CommandLogChannelId == 0)
            return;

        if (!Client.TryGetOrAddChannel(Config.CommandLogChannelId, out channel))
        {
            Logger.Error(LoggingUtils.GenerateMissingChannelMessage("command logs", Config.CommandLogChannelId, Config.GuildId));
            return;
        }

        builder = new(Translation.CommandLog, "player", player)
        {
            CustomReplacers = customReplacers
        };
            
        channel.SendMessage(builder);
    }
}