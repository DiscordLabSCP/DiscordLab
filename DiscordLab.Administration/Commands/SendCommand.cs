using DiscordLab.Core.API.Commands;
using DiscordLab.Core.API.TranslationBuilders;
using LabApi.Features.Wrappers;

namespace DiscordLab.Administration.Commands;

public class SendCommand : ICommand
{
    public static Config Config => Plugin.Instance.Config;

    public static Translation Translation => Plugin.Instance.Translation;

    public CommandBuilder Data => Translation.SendCommand;

    public async Task Execute(CommandInformation command)
    {
        await command.DeferResponse();

        string response = Server.RunCommand(command.OptionsDictionary["command"]);

        TranslationBuilder builder = new TranslationBuilder()
            .AddCustomReplacer("response", response);

        await command.Reply(Translation.SendCommandResponse.Build(builder));
    }
}