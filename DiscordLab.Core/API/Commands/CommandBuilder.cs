using YamlDotNet.Serialization;

namespace DiscordLab.Core.API.Commands;

public class CommandBuilder : BaseCommandBuilder
{
    [YamlMember(DefaultValuesHandling = DefaultValuesHandling.OmitDefaults)]
    public DefaultCommandPermissions DefaultPermission { get; set; } = DefaultCommandPermissions.Everyone;
}