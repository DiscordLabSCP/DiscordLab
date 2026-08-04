using DiscordLab.Core;
using DiscordLab.Core.API.Commands;
using DiscordLab.Core.API.TranslationBuilders;
using LabApi.Features.Wrappers;

namespace DiscordLab.Moderation.Commands;

public class Ban : ICommand
{
    public static Translation Translation => Plugin.Instance.Translation;

    public CommandBuilder Data => Translation.BanCommand;

    public bool ShouldRegister => Plugin.Instance.Config.AddCommands;

    public async Task Execute(CommandInformation command)
    {
        await command.DeferResponse();

        string userId = command.OptionsDictionary["user"];
        long duration = Misc.RelativeTimeToSeconds(command.OptionsDictionary["duration"], 60);
        string reason = command.OptionsDictionary["reason"];

        TranslationBuilder successBuilder = new TranslationBuilder(Translation.BanSuccess)
            {
                Time = TempMuteManager.GetExpireDate(duration)
            }
            .AddCustomReplacer("userid", userId)
            .AddCustomReplacer("reason", reason);
        
        TranslationBuilder failBuilder = new TranslationBuilder(Translation.BanFailure)
            .AddCustomReplacer("userid", userId);

        bool result = userId.Contains("@")
            ? Server.BanUserId(userId, reason, duration)
            : Server.BanIpAddress(userId, reason, duration);

        TranslationBuilder builder = !result ? failBuilder : successBuilder;

        await command.Reply(builder);
    }
}