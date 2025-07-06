using Discord;
using Discord.WebSocket;
using DiscordLab.Bot.API.Features;
using DiscordLab.Bot.API.Interfaces;

namespace DiscordLab.Moderation.Commands
{
    public class Unban : IAutocompleteCommand
    {
        public SlashCommandBuilder Data
        {
            get
            {
                SlashCommandBuilder builder = Plugin.Instance.Translation.UnbanCommand;
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

            string id = (string)command.Data.Options.First().Value;

            BanHandler.RemoveBan(id, id.Contains("@") ? BanHandler.BanType.UserId : BanHandler.BanType.IP);

            TranslationBuilder builder = new(Plugin.Instance.Translation.UnbanSuccess);
            
            builder.CustomReplacers.Add("userid", () => id);
            
            await command.ModifyOriginalResponseAsync(m => 
                m.Content = 
                    builder);
        }
        
        public async Task Autocomplete(SocketAutocompleteInteraction autocomplete)
        {
            IEnumerable<BanDetails> response =
            [
                ..BanHandler.GetBans(BanHandler.BanType.UserId),
                ..BanHandler.GetBans(BanHandler.BanType.IP)
            ];
            await autocomplete.RespondAsync(response.Select(x => new AutocompleteResult($"{x.OriginalName} ({x.Id})", x.Id)));
        }
    }
}