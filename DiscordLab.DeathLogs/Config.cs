using System.ComponentModel;

namespace DiscordLab.DeathLogs;

public class Config
{
    [Description("The channel where the normal death logs will be sent.")]
    public string Channel { get; set; } = "default";

    [Description(
        "The channel where the death logs of cuffed players will be sent. Keep as default value to disable. Disabling this will make it so logs are only sent to the normal death logs channel, but without the cuffed identifier.")]
    public string CuffedChannel { get; set; } = "default";

    [Description(
        "The channel where logs will be sent when a player dies by their own actions, or just they died because of something else.")]
    public string SelfChannel { get; set; } = "default";

    [Description("The channel where logs will be sent when a player dies by a teamkill.")]
    public string TeamKillChannel { get; set; } = "default";

    [Description(
        "If this is true, then the plugin will ignore the cuff state of the player and send the death logs to the normal death logs channel.")]
    public bool ScpIgnoreCuffed { get; set; } = true;

    [Description("The channel to send damage logs to, if any.")]
    public string DamageLogChannel { get; set; } = "default";
    
    [Description("The channel to send team damage logs to, if any.")]
    public string TeamDamageLogChannel { get; set; } = "default";

    [Description("Whether damage logs shouldn't be tracked if the attacker is an SCP.")]
    public bool IgnoreScpDamage { get; set; } = false;
    
    [Description("If your server turns on friendly fire at round end, or people are allowed to RDM at round end, enable this to avoid rate limits and spam.")]
    public bool IgnoreRoundEndDamage { get; set; } = false;
}