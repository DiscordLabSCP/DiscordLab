using Discord;
using Discord.WebSocket;
using DiscordLab.Bot.API.Features;
using DiscordLab.Bot.API.Utilities;
using LabApi.Features.Wrappers;

namespace DiscordLab.Moderation.Commands;

public class Unmute : AutocompleteCommand
{
    public static Translation Translation => Plugin.Instance.Translation;

    public override SlashCommandBuilder Data { get; } = new()
    {
        Name = Translation.UnmuteCommandName,
        Description = Translation.UnmuteCommandDescription,
        DefaultMemberPermissions = GuildPermission.ModerateMembers,
        Options =
        [
            new()
            {
                Name = Translation.UnbanUserOptionName,
                Description = Translation.UnbanUserOptionDescription,
                Type = ApplicationCommandOptionType.String,
                IsRequired = true
            },
        ]
    };

    public override ulong GuildId { get; } = Plugin.Instance.Config.GuildId;
        
    public override async Task Run(SocketSlashCommand command)
    {
        await command.DeferAsync();

        if (!CommandUtils.TryGetPlayerFromUnparsed((string)command.Data.Options.First().Value, out Player player))
        {
            await command.ModifyOriginalResponseAsync(m => m.Content = Translation.InvalidUser);
            return;
        }

        TempMuteManager.RemoveMute(player);
            
        await command.ModifyOriginalResponseAsync(m => 
            m.Content = 
                new TranslationBuilder(Translation.UnmuteSuccess, "player", player));
    }
        
    public override async Task Autocomplete(SocketAutocompleteInteraction autocomplete)
    {
        await autocomplete.RespondAsync(Plugin.PlayersAutocompleteResults);
    }
}