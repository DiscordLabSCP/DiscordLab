using Discord;
using Discord.WebSocket;
using DiscordLab.Bot.API.Features;
using DiscordLab.Bot.API.Utilities;
using LabApi.Features.Wrappers;

namespace DiscordLab.Moderation.Commands;

public class Ban : AutocompleteCommand
{
    public static Translation Translation => Plugin.Instance.Translation;

    public override SlashCommandBuilder Data { get; } = new()
    {
        Name = Translation.BanCommandName,
        Description = Translation.BanCommandDescription,
        DefaultMemberPermissions = GuildPermission.ModerateMembers,
        Options =
        [
            new()
            {
                Name = Translation.BanUserOptionName,
                Description = Translation.BanUserOptionDescription,
                Type = ApplicationCommandOptionType.String,
                IsRequired = true
            },
            new()
            {
                Name = Translation.BanDurationOptionName,
                Description = Translation.BanDurationOptionDescription,
                Type = ApplicationCommandOptionType.String,
                IsRequired = true
            },
            new()
            {
                Name = Translation.BanReasonOptionName,
                Description = Translation.BanReasonOptionDescription,
                Type = ApplicationCommandOptionType.String,
                IsRequired = true
            }
        ]
    };

    public override ulong GuildId { get; } = Plugin.Instance.Config.GuildId;
        
    public override async Task Run(SocketSlashCommand command)
    {
        await command.DeferAsync();

        string userId = (string)command.Data.Options.ElementAt(0).Value;
        long duration = Misc.RelativeTimeToSeconds((string)command.Data.Options.ElementAt(1).Value, 60);
        string reason = (string)command.Data.Options.ElementAt(2).Value;

        TranslationBuilder successBuilder = new(Translation.BanSuccess)
        {
            Time = TempMuteManager.GetExpireDate(duration)
        };
        TranslationBuilder failBuilder = new(Translation.BanFailure);
            
        successBuilder.CustomReplacers.Add("userid", () => userId);
        failBuilder.CustomReplacers.Add("userid", () => userId);
            
        if (!CommandUtils.TryGetPlayerFromUnparsed(userId, out Player player))
        {
            bool result = userId.Contains("@") ? 
                Server.BanUserId(userId, reason, duration) : 
                Server.BanIpAddress(userId, reason, duration);

            await command.ModifyOriginalResponseAsync(m =>
                m.Content = !result ? failBuilder : successBuilder);
                
            return;
        }

        await command.ModifyOriginalResponseAsync(m =>
            m.Content = Server.BanPlayer(player, reason, duration) ? successBuilder : failBuilder);
    }

    public override async Task Autocomplete(SocketAutocompleteInteraction autocomplete)
    {
        await autocomplete.RespondAsync(Plugin.PlayersAutocompleteResults);
    }
}