using Discord;
using DiscordLab.Core.API.Commands;

namespace DiscordLab.Bot;

public class CommandHandler : Core.CommandHandler
{
    public static CommandHandler Instance = null!;

    private static List<SlashCommandOptionBuilder> LoopThroughOptions(IEnumerable<CommandOptionBuilder>? options) =>
        options?.Select(option =>
        {
            ApplicationCommandOptionType type = option.Type switch
            {
                CommandOptionType.Subcommand => ApplicationCommandOptionType.SubCommand,
                CommandOptionType.String => ApplicationCommandOptionType.String,
                CommandOptionType.Integer => ApplicationCommandOptionType.Integer,
                CommandOptionType.Boolean => ApplicationCommandOptionType.Boolean,
                CommandOptionType.User => ApplicationCommandOptionType.User,
                CommandOptionType.Channel => ApplicationCommandOptionType.Channel,
                CommandOptionType.Role => ApplicationCommandOptionType.Role,
                _ => throw new ArgumentOutOfRangeException()
            };

            return new SlashCommandOptionBuilder
            {
                Name = option.Name, Description = option.Description, Type = type,
                Options = option.Options == null ? [] : LoopThroughOptions(option.Options),
                IsAutocomplete = option.Autocomplete != null
            };
        }).ToList() ?? new List<SlashCommandOptionBuilder>();
    
    protected override async Task AddCommands(IEnumerable<ICommand> commands)
    {
        await Client.DefaultGuild!.BulkOverwriteApplicationCommandAsync(commands.Select(cmd =>
        {
            GuildPermission? permission = cmd.Data.DefaultPermission switch
            {
                DefaultCommandPermissions.Everyone => null,
                DefaultCommandPermissions.Moderators => GuildPermission.ModerateMembers,
                DefaultCommandPermissions.Admins => GuildPermission.Administrator,
                _ => throw new ArgumentOutOfRangeException()
            };

            SlashCommandBuilder builder = new()
            {
                Name = cmd.Data.Name,
                Description = cmd.Data.Description,
                DefaultMemberPermissions = permission,
                Options = LoopThroughOptions(cmd.Data.Options)
            };

            return builder.Build();
        }).ToArray<ApplicationCommandProperties>());
    }
}