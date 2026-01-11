using Discord;
using Discord.WebSocket;
using DiscordLab.Bot.API.Features;
using LabApi.Features.Wrappers;

namespace DiscordLab.StatusChannel;

public class Command : SlashCommand
{
    public override SlashCommandBuilder Data { get; } = new()
    {
        Name = Plugin.Instance.Translation.PlayerListCommandName,
        Description = Plugin.Instance.Translation.PlayerListCommandDescription,
    };

    protected override ulong GuildId { get; } = Plugin.Instance.Config.GuildId;

    public override async Task Run(SocketSlashCommand command)
    {
        await Events.UsableContent.InteractionRespond(command, new()
        {
            PlayerListItem = Plugin.Instance.Translation.PlayerItem,
            PlayerList = Player.ReadyList.Where(player => !player.IsDummy || !player.ReferenceHub.serverRoles.HideFromPlayerList)
        });
    }
}