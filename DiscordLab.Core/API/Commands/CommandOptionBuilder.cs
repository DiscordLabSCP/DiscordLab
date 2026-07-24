using YamlDotNet.Serialization;

namespace DiscordLab.Core.API.Commands;

public class CommandOptionBuilder : BaseCommandBuilder
{
    [YamlIgnore]
    public CommandOptionType Type { get; set; }

    [YamlIgnore] 
    public bool IsRequired { get; set; }

    [YamlIgnore]
    public Func<CommandOptionInformation, IEnumerable<(string Name, string Value)>>? Autocomplete;
}