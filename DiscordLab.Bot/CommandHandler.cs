using Discord;
using DiscordLab.Core.API.Commands;
using MEC;
using UnityEngine;

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
        for (int i = 0; i < 10; i++)
        {
            if (Client.DefaultGuild == null)
                await Task.Delay(1000);
        }

        if (Client.DefaultGuild == null)
            return;
        
        foreach (ICommand command in commands)
        {
            GuildPermission? permission = command.Data.DefaultPermission switch
            {
                DefaultCommandPermissions.Everyone => null,
                DefaultCommandPermissions.Moderators => GuildPermission.ModerateMembers,
                DefaultCommandPermissions.Admins => GuildPermission.Administrator,
                _ => throw new ArgumentOutOfRangeException()
            };

            SlashCommandBuilder builder = new()
            {
                Name = command.Data.Name,
                Description = command.Data.Description,
                DefaultMemberPermissions = permission,
                Options = LoopThroughOptions(command.Data.Options)
            };

            await Client.DefaultGuild.CreateApplicationCommandAsync(builder.Build());
        }
    }
}