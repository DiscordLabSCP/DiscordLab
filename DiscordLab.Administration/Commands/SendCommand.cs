using CommandSystem;
using Discord;
using Discord.WebSocket;
using DiscordLab.Bot.API.Features;
using DiscordLab.Bot.API.Interfaces;
using LabApi.Features.Wrappers;
using RemoteAdmin;

namespace DiscordLab.Administration.Commands
{
    public class SendCommand : IAutocompleteCommand
    {
        public static Config Config => Plugin.Instance.Config;

        public static Translation Translation => Plugin.Instance.Translation;

        public SlashCommandBuilder Data
        {
            get
            {
                SlashCommandBuilder builder = Translation.SendCommand;
                SlashCommandOptionBuilder option = builder.Options.First();
                option.IsAutocomplete = true;
                option.Type = ApplicationCommandOptionType.String;
                option.IsRequired = true;

                return builder;
            }
        }

        public ulong GuildId { get; } = Config.GuildId;
        
        public async Task Run(SocketSlashCommand command)
        {
            await command.DeferAsync();

            string response = Server.RunCommand((string)command.Data.Options.First().Value);

            TranslationBuilder builder = new(Translation.SendCommandResponse);
            builder.CustomReplacers.Add("response", () => response);

            await command.ModifyOriginalResponseAsync(m => m.Content = builder);
        }

        public async Task Autocomplete(SocketAutocompleteInteraction autocomplete)
        {
            IEnumerable<string> commands =
            [
                ..CommandProcessor.GetAllCommands().Select(x => "/" + x.Command),
                ..QueryProcessor.DotCommandHandler.AllCommands.Select(x => "." + x.Command)
            ];
            await autocomplete.RespondAsync(commands.Select(x => new AutocompleteResult(x, x)));
        }
    }
}