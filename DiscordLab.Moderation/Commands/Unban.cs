using Discord;
using Discord.WebSocket;
using DiscordLab.Bot.API.Features;

namespace DiscordLab.Moderation.Commands;

public class Unban : AutocompleteCommand
{
    public static Translation Translation => Plugin.Instance.Translation;

    public override SlashCommandBuilder Data { get; } = new()
    {
        Name = Translation.UnbanCommandName,
        Description = Translation.UnbanCommandDescription,
        DefaultMemberPermissions = GuildPermission.ModerateMembers,
        Options =
        [
            new()
            {
                Name = Translation.UnbanUserOptionName,
                Description = Translation.UnbanUserOptionDescription,
                Type = ApplicationCommandOptionType.String,
                IsRequired = true,
                IsAutocomplete = true
            },
        ]
    };

    protected override ulong GuildId { get; } = Plugin.Instance.Config.GuildId;
        
    public override async Task Run(SocketSlashCommand command)
    {
        await command.DeferAsync();

        string id = (string)command.Data.Options.First().Value;

        BanHandler.RemoveBan(id, id.Contains("@") ? BanHandler.BanType.UserId : BanHandler.BanType.IP);

        TranslationBuilder builder = new(Translation.UnbanSuccess);
            
        builder.CustomReplacers.Add("userid", () => id);
            
        await command.ModifyOriginalResponseAsync(m => 
            m.Content = 
                builder);
    }
        
    public override async Task Autocomplete(SocketAutocompleteInteraction autocomplete)
    {
        IEnumerable<BanDetails> response =
        [
            ..BanHandler.GetBans(BanHandler.BanType.UserId),
            ..BanHandler.GetBans(BanHandler.BanType.IP)
        ];
        await autocomplete.RespondAsync(response.Where(x => x.Id.Contains((string)autocomplete.Data.Current.Value) || x.OriginalName.Contains((string)autocomplete.Data.Current.Value)).Take(25).Select(x => new AutocompleteResult($"{x.OriginalName} ({x.Id})", x.Id)));
    }
}