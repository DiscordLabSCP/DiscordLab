using DiscordLab.Core.API.Commands;
using DiscordLab.Core.API.TranslationBuilders;

namespace DiscordLab.Moderation.Commands;

public class Unban : ICommand
{
    public static Translation Translation => Plugin.Instance.Translation;

    public CommandBuilder Data => Translation.UnbanCommand;

    public bool ShouldRegister => Plugin.Instance.Config.AddCommands;

    public async Task Execute(CommandInformation command)
    {
        await command.DeferResponse();

        string id = command.OptionsDictionary["user"];

        BanHandler.RemoveBan(id, id.Contains("@") ? BanHandler.BanType.UserId : BanHandler.BanType.IP);

        TranslationBuilder builder = new TranslationBuilder(Translation.UnbanSuccess)
            .AddCustomReplacer("userid", id);

        await command.Reply(builder);
    }
}