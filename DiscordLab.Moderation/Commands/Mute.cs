using DiscordLab.Core.API.Commands;
using DiscordLab.Core.API.TranslationBuilders;
using LabApi.Features.Wrappers;
using VoiceChat;

namespace DiscordLab.Moderation.Commands;

public class Mute : ICommand
{
    public static Translation Translation => Plugin.Instance.Translation;

    public CommandBuilder Data => Translation.MuteCommand;

    public bool ShouldRegister => Plugin.Instance.Config.AddCommands;

    public async Task Execute(CommandInformation command)
    {
        await command.DeferResponse();

        if (!Player.TryGet(command.OptionsDictionary["user"], out Player player))
        {
            await command.Reply(Translation.InvalidUser);
            return;
        }

        TranslationBuilder builder;

        if (command.Options.Count() == 2)
        {
            string duration = command.OptionsDictionary["duration"];
            DateTime time = TempMuteManager.GetExpireDate(duration);
            TempMuteManager.MutePlayer(player, time);

            builder = new PlayerTranslationBuilder(Translation.TempMuteSuccess, "player", player)
                {
                    Time = time
                }
                .AddCustomReplacer("duration", duration);

            await command.Reply(builder);
            return;
        }

        VoiceChatMutes.IssueLocalMute(player.UserId);

        builder = new PlayerTranslationBuilder(Translation.PermMuteSuccess, "player", player);

        await command.Reply(builder);
    }
}