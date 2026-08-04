using DiscordLab.Core.API.Commands;
using DiscordLab.Core.API.Extensions;
using DiscordLab.Core.API.TranslationBuilders;

namespace DiscordLab.StatusChannel;

public class Command : ICommand
{
    public CommandBuilder Data => Plugin.Instance.Translation.Command;
    
    public bool ShouldRegister => Plugin.Instance.Config.AddCommand;

    public async Task Execute(CommandInformation information)
    {
        await Events.UsableContent.InteractionResponseAsync(information, new AllPlayersTranslationBuilder(Plugin.Instance.Translation.PlayerItem));
    }
}