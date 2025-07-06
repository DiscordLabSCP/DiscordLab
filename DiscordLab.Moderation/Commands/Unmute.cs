using Discord;
using Discord.WebSocket;
using DiscordLab.Bot.API.Features;
using DiscordLab.Bot.API.Interfaces;
using DiscordLab.Bot.API.Utilities;
using LabApi.Features.Wrappers;

namespace DiscordLab.Moderation.Commands
{
    public class Unmute : IAutocompleteCommand
    {

        public SlashCommandBuilder Data
        {
            get
            {
                SlashCommandBuilder builder = Plugin.Instance.Translation.UnmuteCommand;
                SlashCommandOptionBuilder option = builder.Options[0];
                option.Type = ApplicationCommandOptionType.String;
                option.IsRequired = true;
                option.IsAutocomplete = true;

                return builder;
            }
        }

        public ulong GuildId { get; } = Plugin.Instance.Config.GuildId;
        
        public async Task Run(SocketSlashCommand command)
        {
            await command.DeferAsync();

            if (!CommandUtils.TryGetPlayerFromUnparsed((string)command.Data.Options.First().Value, out Player player))
            {
                await command.ModifyOriginalResponseAsync(m => m.Content = Plugin.Instance.Translation.InvalidUser);
                return;
            }

            TempMuteManager.RemoveMute(player);
            
            await command.ModifyOriginalResponseAsync(m => 
                m.Content = 
                    new TranslationBuilder(Plugin.Instance.Translation.UnmuteSuccess, "player", player));
        }
        
        public async Task Autocomplete(SocketAutocompleteInteraction autocomplete)
        {
            await autocomplete.RespondAsync(Plugin.PlayersAutocompleteResults);
        }
    }
}