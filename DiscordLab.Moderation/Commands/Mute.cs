using Discord;
using Discord.WebSocket;
using DiscordLab.Bot.API.Features;
using DiscordLab.Bot.API.Interfaces;
using DiscordLab.Bot.API.Utilities;
using LabApi.Features.Wrappers;
using VoiceChat;

namespace DiscordLab.Moderation.Commands
{
    public class Mute : IAutocompleteCommand
    {
        public SlashCommandBuilder Data { get; } = Plugin.SetupDurationBuilder(Plugin.Instance.Translation.MuteCommand);

        public ulong GuildId { get; } = Plugin.Instance.Config.GuildId;
        
        public async Task Run(SocketSlashCommand command)
        {
            await command.DeferAsync();

            if (!CommandUtils.TryGetPlayerFromUnparsed((string)command.Data.Options.First().Value, out Player player))
            {
                await command.ModifyOriginalResponseAsync(m => m.Content = Plugin.Instance.Translation.InvalidUser);
                return;
            }

            TranslationBuilder builder;

            if (command.Data.Options.Count == 2)
            {
                string duration = (string)command.Data.Options.ElementAt(1).Value;
                DateTime time = TempMuteManager.GetExpireDate(duration);
                TempMuteManager.MutePlayer(player, time);
                
                builder = new(Plugin.Instance.Translation.TempMuteSuccess, "player", player)
                {
                    Time = time
                };
            
                builder.CustomReplacers.Add("duration", () => duration);

                await command.ModifyOriginalResponseAsync(m => m.Content = builder);
                return;
            }

            VoiceChatMutes.IssueLocalMute(player.UserId);

            builder = new(Plugin.Instance.Translation.PermMuteSuccess, "player", player);

            await command.ModifyOriginalResponseAsync(m => m.Content = builder);
        }

        public async Task Autocomplete(SocketAutocompleteInteraction autocomplete)
        {
            await autocomplete.RespondAsync(Plugin.PlayersAutocompleteResults);
        }
    }
}