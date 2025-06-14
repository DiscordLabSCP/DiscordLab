using System.ComponentModel;
using Exiled.API.Interfaces;

namespace DiscordLab.DeathLogs
{
    public class Translation : ITranslation
    {
        [Description("The message that will be sent when a player dies.")]
        public string PlayerDeath { get; set; } =
            "`{player}` (`{playerrole}`) has been killed by `{attacker}` as `{attackerrole}`. They died from: `{cause}`";

        [Description("The message that will be sent when a cuffed player dies, unless the cuffed channel is disabled.")]
        public string CuffedPlayerDeath { get; set; } =
            "`{player}` (`{playerrole}`) has been killed by `{attacker}` as `{attackerrole}` while cuffed. They died from: `{cause}`";

        [Description(
            "The message that will be sent when a player dies by their own actions, or just they died because of something else.")]
        public string PlayerDeathSelf { get; set; } = "`{player}` (`{playerrole}`) has died. They died from: `{cause}`";

        [Description("The message that will be sent when a player dies due to someone on their own team.")]
        public string TeamKill { get; set; } =
            "`{player}` has been team-killed by `{attacker}`, they were both {role}. They died from: `{cause}`";

        [Description("The title of the embed for when sending damage logs")]
        public string DamageLogEmbedTitle { get; set; } = "Damage Log";

        [Description("What each instance of damage will look like in the logs.")]
        public string DamageLogEntry { get; set; } = "{timet} | `{attacker}` did `{damage}` damage to `{player}` | Cause: `{cause}`";
    }
}