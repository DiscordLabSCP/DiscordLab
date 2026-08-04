using DiscordLab.Core;
using DiscordLab.Core.API.Attributes;
using DiscordLab.Core.API.Extensions;
using DiscordLab.Core.API.Features;
using DiscordLab.Core.API.TranslationBuilders;
using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.CustomHandlers;
using LabApi.Features.Wrappers;

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

    public static Queue Queue = new(5, EditMessage);

    [CallOnLoad]
    public static void Register()
    {
        ReferenceHub.OnPlayerRemoved += OnPlayerLeave;
    }

    [CallOnUnload]
    public static void Unregister()
    {
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
        UsableContent.Modify(Config.Channel, new AllPlayersTranslationBuilder(Translation.PlayerItem));
    }

    public static async Task CreateIfMissing()
    {
        await MessageHandler.SendMessagesCheckExists(Config.Channel, UsableContent.Build(new AllPlayersTranslationBuilder(Translation.PlayerItem)));
    }
}