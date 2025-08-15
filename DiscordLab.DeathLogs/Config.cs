using System.ComponentModel;

namespace DiscordLab.DeathLogs;

public class Config
{
    [Description("The channel where the normal death logs will be sent.")]
    public ulong ChannelId { get; set; } = 0;

    [Description(
        "The channel where the death logs of cuffed players will be sent. Keep as default value to disable. Disabling this will make it so logs are only sent to the normal death logs channel, but without the cuffed identifier.")]
    public ulong CuffedChannelId { get; set; } = 0;

    [Description(
        "The channel where logs will be sent when a player dies by their own actions, or just they died because of something else.")]
    public ulong SelfChannelId { get; set; } = 0;

    [Description("The channel where logs will be sent when a player dies by a teamkill.")]
    public ulong TeamKillChannelId { get; set; } = 0;
        
    [Description("If this is true, then the plugin will ignore the cuff state of the player and send the death logs to the normal death logs channel.")]
    public bool ScpIgnoreCuffed { get; set; } = true;

    [Description("The channel to send death logs to, if any.")]
    public ulong DamageLogChannelId { get; set; } = 0;
        
    [Description("Whether damage logs shouldn't be tracked if the attacker is an SCP.")]
    public bool IgnoreScpDamage { get; set; } = false;

    public ulong GuildId { get; set; } = 0;
}