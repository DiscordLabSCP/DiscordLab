namespace DiscordLab.Core.API.Commands;

public record struct CommandOptionInformation(string Name, string Value, IEnumerable<CommandOptionInformation>? Options = null);