using DiscordLab.Core.API.Commands;
using DiscordLab.Core.API.TranslationBuilders;

namespace DiscordLab.Core.API.Extensions;

public static class MessageContentExtensions
{
    extension(MessageContent content)
    {
        public Task Send(string destination, TranslationBuilder? builder = null, bool saveIds = false) =>
            Task.RunAndLog(async () => await content.SendAsync(destination, builder ?? new(), saveIds));

        public Task Modify(string destination, TranslationBuilder? builder = null) =>
            Task.RunAndLog(async () => await content.ModifyAsync(destination, builder ?? new()));
        
        public async Task SendAsync(string destination, TranslationBuilder builder, bool saveIds = false) => await MessageHandler.SendMessages(destination, content.Build(builder), saveIds);

        public async Task ModifyAsync(string destination, TranslationBuilder builder) => await MessageHandler.EditMessages(destination, content.Build(builder));

        public async Task InteractionResponseAsync(CommandInformation information, TranslationBuilder builder) =>
            await information.Reply(content.Build(builder));
    }
}