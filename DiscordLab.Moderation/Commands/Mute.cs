using Discord;
using Discord.WebSocket;
using DiscordLab.Bot.API.Features;
using DiscordLab.Bot.API.Utilities;
using LabApi.Features.Wrappers;
using VoiceChat;

namespace DiscordLab.Moderation.Commands;

public class Mute : AutocompleteCommand
{
    public static Translation Translation => Plugin.Instance.Translation;

    public override SlashCommandBuilder Data { get; } = new()
    {
        Name = Translation.MuteCommandName,
        Description = Translation.MuteCommandDescription,
        DefaultMemberPermissions = GuildPermission.ModerateMembers,
        Options =
        [
            new()
            {
                Name = Translation.MuteUserOptionName,
                Description = Translation.MuteUserOptionDescription,
                Type = ApplicationCommandOptionType.String,
                IsRequired = true
            },
            new()
            {
                Name = Translation.MuteDurationOptionName,
                Description = Translation.MuteDurationOptionDescription,
                Type = ApplicationCommandOptionType.String,
                IsRequired = false
            }
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

        TranslationBuilder builder;

        if (command.Data.Options.Count == 2)
        {
            string duration = (string)command.Data.Options.ElementAt(1).Value;
            DateTime time = TempMuteManager.GetExpireDate(duration);
            TempMuteManager.MutePlayer(player, time);
                
            builder = new(Translation.TempMuteSuccess, "player", player)
            {
                Time = time
            };
            
            builder.CustomReplacers.Add("duration", () => duration);

            await command.ModifyOriginalResponseAsync(m => m.Content = builder);
            return;
        }

        VoiceChatMutes.IssueLocalMute(player.UserId);

        builder = new(Translation.PermMuteSuccess, "player", player);

        await command.ModifyOriginalResponseAsync(m => m.Content = builder);
    }

    public override async Task Autocomplete(SocketAutocompleteInteraction autocomplete)
    {
        await autocomplete.RespondAsync(Plugin.PlayersAutocompleteResults);
    }
}