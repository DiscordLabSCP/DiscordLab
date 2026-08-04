using System.ComponentModel;

namespace DiscordLab.RoundLogs;

public class Config
{
    [Description("The channel to log to when someone's role changes.")]
    public string RoleChangeChannel { get; set; } = "default";

    [Description("The channel to log to when someone swaps from an SCP to another.")]
    public string ScpSwapChannel { get; set; } = "default";

    [Description("The channel to log to when NTF spawns.")]
    public string NtfSpawnChannel { get; set; } = "default";

    [Description("The channel to log to when Chaos spawns.")]
    public string ChaosSpawnChannel { get; set; } = "default";

    [Description("The channel to log to when someone gets cuffed.")]
    public string CuffedChannel { get; set; } = "default";

    [Description("The channel to log to when someone gets uncuffed.")]
    public string UncuffedChannel { get; set; } = "default";

    [Description("The channel to log to when the round starts.")]
    public string RoundStartedChannel { get; set; } = "default";

    [Description("The channel to log to when the round ends.")]
    public string RoundEndedChannel { get; set; } = "default";

    [Description("The channel to log to when decontamination starts.")]
    public string DecontaminationChannel { get; set; } = "default";
    
    [Description("The channel to log to when someone escapes.")]
    public string EscapeChannel { get; set; } = "default";
    
    [Description("The channel to log to when the warhead is activated.")]
    public string WarheadActivatedChannel { get; set; } = "default";
    
    [Description("The channel to log to when the warhead is deactivated.")]
    public string WarheadDeactivatedChannel { get; set; } = "default";
}