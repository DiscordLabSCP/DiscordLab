using Discord;
using Discord.WebSocket;
using DiscordLab.Bot.API.Features;
using DiscordLab.Bot.API.Interfaces;
using DiscordLab.Bot.API.Utilities;
using LabApi.Features.Wrappers;

namespace DiscordLab.Moderation.Commands
{
    public class Ban : IAutocompleteCommand
    {
        public SlashCommandBuilder Data
        {
            get
            {
                SlashCommandBuilder builder = Plugin.SetupDurationBuilder(Plugin.Instance.Translation.BanCommand, true);
                SlashCommandOptionBuilder option = builder.Options[2];
                option.IsRequired = true;
                option.IsAutocomplete = false;
                option.Type = ApplicationCommandOptionType.String;

                return builder;
            }
        }

        public ulong GuildId { get; } = Plugin.Instance.Config.GuildId;
        
        public async Task Run(SocketSlashCommand command)
        {
            await command.DeferAsync();

            string userId = (string)command.Data.Options.ElementAt(0).Value;
            long duration = Misc.RelativeTimeToSeconds((string)command.Data.Options.ElementAt(1).Value, 60);
            string reason = (string)command.Data.Options.ElementAt(2).Value;

            TranslationBuilder successBuilder = new(Plugin.Instance.Translation.BanSuccess)
            {
                Time = TempMuteManager.GetExpireDate(duration)
            };
            TranslationBuilder failBuilder = new(Plugin.Instance.Translation.BanFailure);
            
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

        public async Task Autocomplete(SocketAutocompleteInteraction autocomplete)
        {
            await autocomplete.RespondAsync(Plugin.PlayersAutocompleteResults);
        }
    }
}