using CommandSystem;
using Discord;
using Discord.WebSocket;
using DiscordLab.Bot.API.Features;
using LabApi.Features.Wrappers;
using RemoteAdmin;

namespace DiscordLab.Administration.Commands;

public class SendCommand : AutocompleteCommand
{
    public static Config Config => Plugin.Instance.Config;

    public static Translation Translation => Plugin.Instance.Translation;

    public override SlashCommandBuilder Data { get; } = new()
    {
        Name = Translation.SendCommandName,
        Description = Translation.SendCommandDescription,
        DefaultMemberPermissions = GuildPermission.ModerateMembers,
        Options =
        [
            new()
            {
                Name = Translation.SendCommandOptionName,
                Description = Translation.SendCommandOptionDescription,
                Type = ApplicationCommandOptionType.String,
                IsRequired = true
            }
        ]
    };

    public override ulong GuildId { get; } = Config.GuildId;
        
    public override async Task Run(SocketSlashCommand command)
    {
        await command.DeferAsync();

        string response = Server.RunCommand((string)command.Data.Options.First().Value);

        TranslationBuilder builder = new TranslationBuilder()
            .AddCustomReplacer("response", response);

        await Translation.SendCommandResponse.ModifyInteraction(command, builder);
    }

    public override async Task Autocomplete(SocketAutocompleteInteraction autocomplete)
    {
        IEnumerable<string> commands =
        [
            ..CommandProcessor.GetAllCommands().Select(x => "/" + x.Command),
            ..QueryProcessor.DotCommandHandler.AllCommands.Select(x => "." + x.Command)
        ];
        await autocomplete.RespondAsync(commands.Select(x => new AutocompleteResult(x, x)));
    }
}