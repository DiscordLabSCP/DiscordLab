using DiscordLab.Core.API.Commands;
using DiscordLab.Core.API.TranslationBuilders;
using LabApi.Features.Wrappers;

namespace DiscordLab.Moderation.Commands;

public class Unmute : ICommand
{
    public static Translation Translation => Plugin.Instance.Translation;

    public CommandBuilder Data => Translation.UnmuteCommand;

    public bool ShouldRegister => Plugin.Instance.Config.AddCommands;

    public async Task Execute(CommandInformation command)
    {
        await command.DeferResponse();

        if (!Player.TryGet(command.OptionsDictionary["user"], out Player player))
        {
            await command.Reply(Translation.InvalidUser);
            return;
        }

        TempMuteManager.RemoveMute(player);

        await command.Reply(new PlayerTranslationBuilder(Translation.UnmuteSuccess, "player", player));
    }
}