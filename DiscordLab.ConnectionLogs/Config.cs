using System.ComponentModel;

namespace DiscordLab.ConnectionLogs;

public class Config
{
    [Description("The channel where the join logs will be sent.")]
    public string JoinChannel { get; set; } = "default";

    [Description("The channel where the leave logs will be sent.")]
    public string LeaveChannel { get; set; } = "default";

    [Description("The channel where the round start logs will be sent.")]
    public string RoundStartChannel { get; set; } = "default";

    [Description("The channel where the round end logs will be sent. Optional.")]
    public string RoundEndChannel { get; set; } = "default";
}