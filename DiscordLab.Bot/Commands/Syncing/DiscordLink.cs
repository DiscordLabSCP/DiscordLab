using Discord.Interactions;
using DiscordLab.Core;
using DiscordLab.Core.API.TranslationBuilders;

namespace DiscordLab.Bot.Commands.Syncing;

public class DiscordLink : InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("link", "Link your Discord to your Steam.")]
    public async Task Execute([Summary(description: "The ID of your account you play on")] string id)
    {
        string code = LinkInstance.CreateInstance(id, Context.User.Id);
        MessageContent content = Plugin.Instance.Translation.DiscordResponse;

        MessageInformation info = content.Build(new TranslationBuilder().AddCustomReplacer("code", code));
        await RespondAsync(info.Content, [.. info.Embeds?.Select(MessageHandler.FromGeneric) ?? []]);
    }
}