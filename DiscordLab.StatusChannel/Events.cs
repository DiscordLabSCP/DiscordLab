using Discord;
using Discord.WebSocket;
using DiscordLab.Bot;
using DiscordLab.Bot.API.Attributes;
using DiscordLab.Bot.API.Extensions;
using DiscordLab.Bot.API.Features;
using DiscordLab.Bot.API.Utilities;
using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.CustomHandlers;
using LabApi.Features.Console;
using LabApi.Features.Wrappers;
using LabApi.Loader;

namespace DiscordLab.StatusChannel;

public class Events : CustomEventsHandler
{
    // events

    public override void OnServerWaitingForPlayers() => EditMessage();

    public override void OnPlayerJoined(PlayerJoinedEventArgs _) => Process();

    public static void OnPlayerLeave(ReferenceHub _) => Process();

    public override void OnServerRoundStarted() => EditMessage();

    // static methods

    public static Config Config => Plugin.Instance.Config;

    public static Translation Translation => Plugin.Instance.Translation;

    public static SocketTextChannel Channel;

    public static IUserMessage Message;

    public static Queue Queue = new(5, EditMessage);

    [CallOnLoad]
    public static void Register()
    {
        ReferenceHub.OnPlayerRemoved += OnPlayerLeave;
    }

    [CallOnUnload]
    public static void Unregister()
    {
        Channel = null;
        Message = null;
        Queue = null;

        ReferenceHub.OnPlayerRemoved -= OnPlayerLeave;
    }

    public static void Process()
    {
        if (Round.IsRoundInProgress)
            EditMessage();
        else
            Queue.Process();
    }

    public static MessageContent UsableContent =>
        Player.ReadyList.Any() ? Translation.Content : Translation.EmptyContent;

    public static void EditMessage()
    {
        if (Message == null)
        {
            Task.RunAndLog(async () =>
            {
                await GetOrCreateMessage();
                EditMessage();
            });
            return;
        }

        try
        {
            UsableContent.ModifyMessage(Message, new()
            {
                PlayerListItem = Translation.PlayerItem,
                PlayerList = Player.ReadyList.Where(player => !player.IsDummy || !player.ReferenceHub.serverRoles.HideFromPlayerList)
            });
        }
        catch (Exception e)
        {
            Logger.Error(e);
        }
    }

    [CallOnReady]
    public static void Ready()
    {
        if (!Client.TryGetOrAddChannel(Config.ChannelId, out Channel))
        {
            Logger.Error(
                LoggingUtils.GenerateMissingChannelMessage("status channel", Config.ChannelId, Config.GuildId));
            Plugin.Instance.Disable();
        }
    }

    public static async Task GetOrCreateMessage()
    {
        ulong msgId = Plugin.Instance.MessageConfig.MessageId;
        Message = msgId != 0
            ? Channel.GetCachedMessage(msgId) as IUserMessage ??
              await Channel.GetMessageAsync(msgId) as IUserMessage
            : null;

        if (Message == null)
        {
            Message = await Translation.EmptyContent.SendToChannelAsync(Channel, new());

            Plugin.Instance.MessageConfig.MessageId = Message.Id;
            Plugin.Instance.SaveConfig(Plugin.Instance.MessageConfig, "message_config.yml");
        }
    }
}