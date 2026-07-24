using YamlDotNet.Serialization;

namespace DiscordLab.Core.API.Commands;

public abstract class BaseCommandBuilder
{
    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;
    
    [YamlMember(DefaultValuesHandling = DefaultValuesHandling.OmitNull)]
    public IEnumerable<CommandOptionBuilder>? Options { get; set; }
}