using System.ComponentModel;
using Exiled.API.Interfaces;

namespace DiscordLab.DeathLogs;

public class Translation : ITranslation
{
    [Description("The message that will be sent when a player dies.")]
    public string PlayerDeath { get; set; } = "`{player}` (`{playerrole}`) has been killed by `{attacker}` as `{attackerrole}`.";
    [Description("The message that will be sent when a cuffed player dies, unless the cuffed channel is disabled.")]
    public string CuffedPlayerDeath { get; set; } = "`{player}` (`{playerrole}`) has been killed by `{attacker}` as `{attackerrole}` while cuffed.";
}